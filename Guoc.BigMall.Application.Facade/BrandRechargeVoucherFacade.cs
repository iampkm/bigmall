using Guoc.BigMall.Infrastructure.Utils;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.Service;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Application.DTO;

namespace Guoc.BigMall.Application.Facade
{
    public class BrandRechargeVoucherFacade : IBrandRechargeVoucherFacade
    {
        IDBContext _db;
        IContextFacade _context;
        BillSequenceService _sequenceService;
        public BrandRechargeVoucherFacade(IDBContext dbContext, IContextFacade context, BillSequenceService sequenceService)
        {
            _db = dbContext;
            _context = context;
            _sequenceService = sequenceService;
        }

        public List<BrandRechargeVoucherDto> GetBrandVoucherList(Pager page, BrandRechargeVoucherSearch searchArgs)
        {
            var where = "";
            dynamic queryParams = new ExpandoObject();

            var storeIds = searchArgs.StoreIds.NotNullOrEmpty() ? searchArgs.StoreIds : _context.CurrentAccount.CanViewStores;
            if (storeIds.NotNullOrEmpty())
            {
                where += " AND StoreId IN @StoreIds";
                queryParams.StoreIds = storeIds.Split(',');
            }

            if (searchArgs.BrandId.HasValue)
            {
                where += " AND BrandId=@BrandId";
                queryParams.BrandId = searchArgs.BrandId.Value;
            }

            if (searchArgs.Status.HasValue)
            {
                where += " AND Status=@Status";
                queryParams.Status = searchArgs.Status.Value;
            }

            if (searchArgs.DateRange != null && searchArgs.DateRange.Length == 2)
            {
                where += " AND StartDate<=@StartDate AND EndDate>=@EndDate";
                queryParams.StartDate = searchArgs.DateRange[0];
                queryParams.EndDate = searchArgs.DateRange[1];
            }

            if (searchArgs.CategoryId.HasValue)
            {
                where += @"
                AND EXISTS ( SELECT 1
                             FROM   ParticipantCategory p2 ,
                                    Category c
                             WHERE  p2.BrandRechargeVoucherId = v.Id
                                    AND p2.Type = 2
                                    AND c.Code LIKE p2.CategoryCode + '%'
                                    AND c.Id = @CategoryId )";
                queryParams.CategoryId = searchArgs.CategoryId.Value;
            }

            var fields = @"
                v.* ,
                STUFF(( SELECT  ',' + p.CategoryCode + ' - '
                                + p.CategoryName
                        FROM    ParticipantCategory p
                        WHERE   p.BrandRechargeVoucherId = v.Id
                                AND p.Type = 2
                        ORDER BY p.CategoryCode
                        FOR
                        XML PATH('')
                        ), 1, 1, '') AS Categories ,
                STUFF(( SELECT  '[,]' + e.ProductCode + ' - ' + e.ProductName
                        FROM    ExceptProduct e
                        WHERE   e.BrandRechargeVoucherId = v.Id
                                AND e.Type = 2
                      FOR
                        XML PATH('')
                      ), 1, 3, '') AS ExceptProducts";

            var basicSql = @"
                SELECT  {0}
                FROM    BrandRechargeVoucher v
                WHERE   1 = 1 " + where;

            var dataSql = basicSql.FormatWith(fields);
            var countSql = basicSql.FormatWith("COUNT(1)");
            var pageSql = "{0} ORDER BY StartDate ASC,EndDate ASC,CreatedOn ASC OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY".FormatWith(dataSql, (page.PageIndex - 1) * page.PageSize, page.PageSize);

            page.Total = _db.DataBase.ExecuteScalar<int>(countSql, queryParams);
            var data = _db.DataBase.Query<BrandRechargeVoucherDto>(pageSql, queryParams);
            return data;
        }

        public void Create(CreateBrandVoucherModel model)
        {
            var brandVoucher = model.MapTo<BrandRechargeVoucher>();

            this.CheckStorePermission(brandVoucher);

            var store = _db.Table<Store>().FirstOrDefault(s => s.Id == brandVoucher.StoreId);
            Ensure.NotNull(store, "门店不存在！");
            brandVoucher.StoreCode = store.Code;
            brandVoucher.StoreName = store.Name;

            var brand = _db.Table<Brand>().FirstOrDefault(b => b.Id == model.BrandId);
            Ensure.NotNull(brand, "品牌不存在！");
            brandVoucher.BrandCode = brand.Code;
            brandVoucher.BrandName = brand.Name;

            var categoryIds = model.CategoryIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToIntArray().Distinct();
            Ensure.NotNullOrEmpty(categoryIds, "未选择品类！");

            var groupedCategoryIds = categoryIds.GroupByIndexSubsection(1000).ToList();
            groupedCategoryIds.ForEach(group =>
            {
                var groupCategories = _db.Table<Category>().Where(c => c.Id.In(group)).ToList();
                Ensure.EqualThan(group.Count(), groupCategories.Count, "部分品类不存在！");

                var participantCategories = groupCategories.Select(c => new ParticipantCategory(RechargeVoucherType.BrandRechargeVoucher, c.Id, c.Code, c.Name));
                brandVoucher.ParticipantCategories.AddRange(participantCategories);
            });

            if (model.ExceptProductIds.NotNull())
            {
                var groupedProductIds = model.ExceptProductIds.Distinct().GroupByIndexSubsection(1000).ToList();
                groupedProductIds.ForEach(group =>
                {
                    var groupProducts = _db.Table<Product>().Where(p => p.Id.In(group)).ToList();
                    Ensure.EqualThan(group.Count(), groupProducts.Count, "部分商品不存在！");

                    var exceptProducts = groupProducts.Select(p => new ExceptProduct(RechargeVoucherType.BrandRechargeVoucher, p.Id, p.Code, p.Name));
                    brandVoucher.ExceptProducts.AddRange(exceptProducts);
                });
            }

            brandVoucher.Code = _sequenceService.GenerateNewCode(BillIdentity.BrandRechargeVoucher);

            var currentAccount = _context.CurrentAccount;
            brandVoucher.CreatedOn = DateTime.Now;
            brandVoucher.CreatedBy = currentAccount.AccountId;
            brandVoucher.CreatedByName = currentAccount.NickName;
            brandVoucher.Balance = brandVoucher.Amount;
            brandVoucher.Status = RechargeVoucherStatus.WaitAudit;

            _db.Insert(brandVoucher);
            _db.SaveChange();
        }

        private BrandRechargeVoucher FindBrandVoucherById(int id)
        {
            var brandVoucher = _db.Table<BrandRechargeVoucher>().FirstOrDefault(v => v.Id == id);
            Ensure.NotNull(brandVoucher, "品类充值券不存在！");
            return brandVoucher;
        }

        private void CheckStorePermission(BrandRechargeVoucher brandVoucher)
        {
            var currentAccount = _context.CurrentAccount;
            Ensure.In(brandVoucher.StoreId, currentAccount.StoreArray, "抱歉，您没有操作权限！");
        }

        //品牌券审核通过
        public void PassAudit(int id)
        {
            var brandVoucher = this.FindBrandVoucherById(id);

            this.CheckStorePermission(brandVoucher);

            var currentAccount = _context.CurrentAccount;
            brandVoucher.PassAudit(currentAccount.AccountId, currentAccount.NickName);

            _db.Update(brandVoucher);
            _db.SaveChange();
        }

        //品牌券审核驳回
        public void RejectAudit(int id)
        {
            var brandVoucher = this.FindBrandVoucherById(id);

            this.CheckStorePermission(brandVoucher);

            var currentAccount = _context.CurrentAccount;
            brandVoucher.RejectAudit(currentAccount.AccountId, currentAccount.NickName);

            _db.Update(brandVoucher);
            _db.SaveChange();
        }

        //品牌券审核中止使用
        public void Abort(int id)
        {
            var brandVoucher = this.FindBrandVoucherById(id);

            this.CheckStorePermission(brandVoucher);

            brandVoucher.Abort();

            _db.Update(brandVoucher);
            _db.SaveChange();
        }


        #region 使用券

        //扣减券额
        public void ReduceBrandRechargeVoucher(IDBContext dbContext, SaleOrder order)
        {
            Ensure.NotNull(order, "销售单为空。");
            Ensure.NotNullOrEmpty(order.Items, "销售单明细为空。");
            Ensure.True(order.Items.All(m => m.Quantity > 0), "销售单商品明细的数量必须大于零。");
            Ensure.False(order.Items.Any(m => m.BrandPreferential < 0), "品牌优惠金额不能为负数。");

            //筛选参与品牌优惠的明细
            var preferentialItems = order.Items.Where(m => m.GiftType == OrderProductType.Product.Value() && m.BrandPreferential > 0).OrderByDescending(m => m.BrandPreferential).ToList();
            if (preferentialItems.IsEmpty()) return;

            var nowDate = DateTime.Now;
            var productIds = preferentialItems.Select(m => m.ProductId);
            var products = dbContext.Table<Product>().Where(p => p.Id.In(productIds)).ToList();
            var categoryIds = products.Select(p => p.CategoryId);
            var categories = dbContext.Table<Category>().Where(c => c.Id.In(categoryIds)).ToList();
            var brandIds = products.Select(p => p.BrandId);
            var vouchers = dbContext.Table<BrandRechargeVoucher>().Where(v => v.StoreId == order.StoreId && v.BrandId.In(brandIds) && v.Status == RechargeVoucherStatus.Normal && v.StartDate <= nowDate && v.EndDate >= nowDate).ToList();//获取所有可用的品牌充值券

            vouchers.ForEach(v =>
            {
                var participantCategories = dbContext.Table<ParticipantCategory>().Where(c => c.BrandRechargeVoucherId == v.Id).ToList();
                v.ParticipantCategories.AddRange(participantCategories);

                var exceptProducts = dbContext.Table<ExceptProduct>().Where(p => p.BrandRechargeVoucherId == v.Id).ToList();
                v.ExceptProducts.AddRange(exceptProducts);
            });

            preferentialItems.ForEach(m =>
            {
                var product = products.FirstOrDefault(p => p.Id == m.ProductId);
                Ensure.NotNull(product, "商品{0}不存在。".FormatWith(m.ProductCode));

                var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
                Ensure.NotNull(category, "商品{0}关联的品类不存在。".FormatWith(product.Code));

                var preferentialPrice = m.BrandPreferential / m.Quantity;//单品优惠金额
                var voucher = vouchers.Where(v => v.BrandId == product.BrandId && v.ExceptProducts.All(p => p.ProductId != m.ProductId) && v.ParticipantCategories.Any(c => category.Code.StartsWith(c.CategoryCode)) && v.Balance >= m.BrandPreferential && preferentialPrice <= (m.RealPrice * (v.Limit / 100)))
                                      .OrderBy(v => v.StartDate)
                                      .ThenBy(v => v.EndDate)
                                      .FirstOrDefault();
                Ensure.NotNull(voucher, "商品{0}没有可用的品牌券或品牌券限额、余额不满足要求。".FormatWith(product.Code));

                var history = new RechargeVoucherHistory()
                {
                    VoucherType = RechargeVoucherType.BrandRechargeVoucher,
                    StoreId = voucher.StoreId,
                    BillCode = order.Code,
                    BillType = order.GetBillIdentity(),
                    ProductId = m.ProductId,
                    ProductCode = product.Code,
                    SNCode = m.SNCode,
                    VoucherId = voucher.Id,
                    VoucherCode = voucher.Code,
                    Quantity = m.Quantity,
                    BalanceBeforeChange = voucher.Balance,
                    ChangeAmount = -m.BrandPreferential,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _context.CurrentAccount.AccountId,
                };

                voucher.Balance -= m.BrandPreferential;//余额
                voucher.Reduced += m.BrandPreferential;//已用

                dbContext.Insert(history);
            });

            dbContext.Update(vouchers.ToArray());
        }

        //退还券额
        public void RefundBrandRechargeVoucher(IDBContext dbContext, SaleOrder order)
        {
            Ensure.NotNull(order, "销售单为空。");
            Ensure.NotNullOrEmpty(order.Items, "销售单明细为空。");
            Ensure.True(order.OrderType == OrderType.Return.Value() || order.Status == SaleOrderStatus.Cancel, "仅退单或订单作废状态可退还品牌优惠金额。");

            var sourceSaleOrder = order;
            if (order.OrderType == OrderType.Return.Value())
            {
                var orderType = OrderType.Order.Value();
                var sourceSaleOrderCode = order.SourceSaleOrderCode;
                sourceSaleOrder = dbContext.Table<SaleOrder>().FirstOrDefault(s => s.Code == sourceSaleOrderCode && s.OrderType == orderType);
                Ensure.NotNull(sourceSaleOrder, "退单关联的原订单{0}不存在。".FormatWith(sourceSaleOrderCode));
            }

            var sourceSaleOrderBillIdentity = sourceSaleOrder.GetBillIdentity();
            var histories = dbContext.Table<RechargeVoucherHistory>().Where(h => h.VoucherType == RechargeVoucherType.BrandRechargeVoucher && h.StoreId == sourceSaleOrder.StoreId && h.BillCode == sourceSaleOrder.Code && h.BillType == sourceSaleOrderBillIdentity && h.ChangeAmount < 0).ToList();
            if (histories.Count == 0) return;//原单未使用品牌优惠

            var voucherIds = histories.Select(h => h.VoucherId).ToList();
            var vouchers = dbContext.Table<BrandRechargeVoucher>().Where(v => v.Id.In(voucherIds)).ToList();
            var items = order.Items.Where(m => m.GiftType == OrderProductType.Product.Value()).ToList();

            items.ForEach(m =>
            {
                if (m.Quantity <= 0) return;

                var history = histories.FirstOrDefault(h => h.ProductId == m.ProductId && h.SNCode == m.SNCode);
                if (history == null) return;

                Ensure.SmallerOrEqualThan(m.Quantity, history.Quantity, "商品{0}数量必须≤参与品牌优惠的数量。".FormatWith(m.ProductCode));

                var voucher = vouchers.FirstOrDefault(v => v.Id == history.VoucherId);
                Ensure.NotNull(voucher, "找不到品牌充值券。");

                var refundAmount = Math.Round((Math.Abs(history.ChangeAmount) / history.Quantity) * m.Quantity, 2);

                history = new RechargeVoucherHistory()
                {
                    VoucherType = RechargeVoucherType.CategoryRechargeVoucher,
                    StoreId = voucher.StoreId,
                    BillCode = order.Code,
                    BillType = order.GetBillIdentity(),
                    ProductId = m.ProductId,
                    ProductCode = m.ProductCode,
                    SNCode = m.SNCode,
                    VoucherId = voucher.Id,
                    VoucherCode = voucher.Code,
                    Quantity = m.Quantity,
                    BalanceBeforeChange = voucher.Balance,
                    ChangeAmount = refundAmount,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _context.CurrentAccount.AccountId,
                };

                voucher.Balance += refundAmount;//余额
                voucher.Reduced -= refundAmount;//已用

                dbContext.Insert(history);
            });

            dbContext.Update(vouchers.ToArray());
        }

        #endregion
    }
}
