using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Infrastructure.Utils;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.Service;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;

namespace Guoc.BigMall.Application.Facade
{
    public abstract class SaleOrderFacadeBase
    {
        #region properties
        protected IDBContext _db;
        protected IContextFacade _context;
        protected ILogger _log;
        protected BillSequenceService _billSequenceService;
        protected IProductPriceFacade _productPriceFacade;
        protected IProductFacade _productFacade;
        protected IStoreInventoryFacade _storeInventory;
        protected ISupplierFacade _supplierFacade;
        protected IStoreFacade _storeFacade;
        protected IStoreInventoryFacade _storeInventoryFacade;
        #endregion

        #region ctor
        public SaleOrderFacadeBase(
            IDBContext db,
            IContextFacade context,
            ILogger log,
            IProductPriceFacade productPriceFacade,
            IProductFacade productFacade,
            IStoreInventoryFacade storeInventory,
            ISupplierFacade supplierFacade,
            IStoreInventoryFacade storeInventoryFacade,
            IStoreFacade storeFacade)
        {
            _db = db;
            _log = log;
            _context = context;
            _billSequenceService = new BillSequenceService(_db);
            _productPriceFacade = productPriceFacade;
            _productFacade = productFacade;
            _storeInventory = storeInventory;
            _supplierFacade = supplierFacade;
            _storeFacade = storeFacade;
            _storeInventoryFacade = storeInventoryFacade;
        }
        #endregion

        #region Create SaleOrder
        protected virtual SaleOrder DoCreateSaleOrder(SaleOrderModel model)
        {
            var store = _storeFacade.LoadStore(model.StoreId);
            var giftStore = _storeFacade.LoadStore(model.StoreIdGift);

            var order = model.MapTo<SaleOrder>();
            order.OrderType = model.OrderType.Value;
            order.BillType = model.BillType;
            order.Status = model.Status;
            order.RoStatus = model.RoStatus;
            order.Code = _billSequenceService.GenerateNewCode(order.GetBillIdentity());
            order.SourceSapCode = model.SourceSapCode;
            //order.OrderAmount = model.Items.Sum(x => x.SalePrice);
            order.StoreId = model.StoreId;
            order.StoreCode = store == null ? "" : store.Code;
            order.StoreIdGift = model.StoreIdGift;
            order.StoreCodeGift = giftStore == null ? "" : giftStore.Code;
            order.ParentCode = "{0}{1}".FormatWith("P", _billSequenceService.GenerateNewCode(order.GetBillIdentity()));
            order.CreatedBy = _context == null ? -1 : _context.CurrentAccount.AccountId; //model.CreatedBy;
            order.CreatedOn = DateTime.Now;
            order.UpdatedBy = _context == null ? -1 : _context.CurrentAccount.AccountId; //model.CreatedBy;
            order.UpdatedOn = DateTime.Now;
            //order.PaidDate = DateTime.Now;
            order.PaidDate = null;
            order.IsPushSap = false;
            //门店客户对应关系
            var storeCustomerMap = _db.Table<StoreCustomerMap>().FirstOrDefault(n => n.StoreCode == order.StoreCode);
            if (storeCustomerMap == null) throw new FriendlyException(string.Format("门店{0}找不到对应客户", order.StoreCode));
            order.CustomerCode = storeCustomerMap.CustomerCode;
            this.DoAddSaleOrderItems(order, model.Items);

            return order;
        }

        protected virtual void DoAddSaleOrderItems(SaleOrder order, List<SaleOrderItemModel> items, string operType = "add")
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var product = _productFacade.LoadProductById(order.StoreId, item.ProductId);
                var productPrice = _productPriceFacade.GetStoreProductPrice(order.StoreId, item.ProductId);

                if (product == null)
                    throw new FriendlyException("商品不存在。");
                if (product.HasSNCode && item.SNCode.IsNullOrEmpty())
                    throw new FriendlyException("串码商品{0}销售制单必须选择串码。".FormatWith(product.Code));

                var orderItem = new SaleOrderItem();
                orderItem.ProductId = item.ProductId;
                orderItem.ProductCode = product.Code;
                orderItem.ProductName = product.Name;
                orderItem.Unit = product.Unit;
                orderItem.SalePrice = productPrice.SalePrice;
                orderItem.MinSalePrice = productPrice.MinSalePrice;//限价
                orderItem.RealPrice = item.RealPrice; ;
                //orderItem.SupplierId = item.SupplierId;//todo
                orderItem.CategoryCardNumber = item.CategoryPreferential > 0 ? item.CategoryCardNumber : null;
                orderItem.CategoryPreferential = item.CategoryPreferential;
                orderItem.BrandPreferential = item.BrandPreferential;
                orderItem.Quantity = item.Quantity;
                orderItem.GiftType = OrderProductType.Product.Value();
                orderItem.SNCode = item.SNCode;
                orderItem.FJCode = item.FJCode;
                orderItem.SaleClerkOne = item.SaleClerkOne;
                orderItem.SaleClerkTwo = item.SaleClerkTwo;
                orderItem.SourceSapRow = item.SourceSapRow;
                orderItem.SourceSaleOrderRow = item.SourceSaleOrderRow;
                //校验富基Code
                if (operType.Equals("add"))
                    CheckFJCodeExist(item.FJCode);

                // 校验库存
                //CheckStoreInventory(orderItem, order.StoreId);

                if (orderItem.RealPrice <= 0) { throw new FriendlyException("商品[{0}],销售价不能为零".FormatWith(product.Code)); }

                //检查是否有特价商品 如果有则修改其订单状态为待审核
                if (order.OrderType == OrderType.Order.Value() && orderItem.RealPrice < productPrice.MinSalePrice)
                    order.Status = SaleOrderStatus.WaitAudit;

                //添加礼品
                orderItem.ParentProductId = this.GenerateParentProductId(item.ProductId, i + 1, 4);//明细唯一码
                DoAddGifts(order, orderItem.ParentProductId.Value, item.GiftItems);

                order.SetItem(orderItem);
            }
        }

        protected virtual void DoAddGifts(SaleOrder order, int parentProductId, List<SaleOrderGiftItem> gifts)
        {
            foreach (var item in gifts)
            {
                var product = _productFacade.LoadProductById(order.StoreIdGift, item.GiftProductId);
                if (product == null)
                    throw new FriendlyException("商品不存在。");
                if (product.HasSNCode)
                    throw new FriendlyException("数据错误，串码商品{0}不能是赠品。".FormatWith(product.Code));

                var gift = new SaleOrderItem();
                gift.ProductId = item.GiftProductId;
                gift.ProductCode = product == null ? string.Empty : product.Code;
                gift.ProductName = product == null ? string.Empty : product.Name;
                gift.SalePrice = 0;
                gift.SaleOrderId = order.Id;
                //gift.SupplierId = 1; //todo
                gift.RealPrice = 0;
                gift.Unit = product.Unit;
                gift.Quantity = item.GiftQuantity;
                gift.ParentProductId = parentProductId;
                gift.GiftType = OrderProductType.Gift.Value();
                gift.SourceSapRow = item.SourceSapRow;
                gift.SourceSaleOrderRow = item.SourceSaleOrderRow;

                order.SetItem(gift);
            }
        }

        private int GenerateParentProductId(int productId, int index, int length)
        {
            var parentProductId = "{0}{1}".FormatWith(productId, index.ToString().PadLeft(length, '0'));
            return int.Parse(parentProductId);
        }

        protected virtual ProcessHistory DoCreateHistory(SaleOrderModel model, SaleOrder order)
        {
            var reason = string.Format("创建{0}", order.GetBillIdentityDescription());
            var history = new ProcessHistory(model.CreatedBy, model.UpdatedByName, (int)order.Status, order.Id, order.GetBillIdentity().ToString(), reason);

            return history;
        }
        #endregion

        #region Read To SaleOrderDto
        protected virtual IEnumerable<SaleOrderDto> GetSaleOrderDtos(Pager page, SearchSaleOrder condition)
        {
            var masterSqlString = @"
                SELECT DISTINCT so.*,s.Name as StoreName,a.NickName as CreatedByName,a1.NickName as AuditedByName
                FROM SaleOrder so
                INNER JOIN SaleOrderItem soi ON so.Id = soi.SaleOrderId
                LEFT JOIN Product p ON soi.ProductId = p.Id
                LEFT JOIN Category c ON p.CategoryId = c.Id
                LEFT JOIN Brand b ON p.BrandId = b.Id
                LEFT JOIN Store s ON so.StoreId = s.Id
                LEFT JOIN Account a on a.id =so.CreatedBy
                LEFT JOIN Account a1 on a1.id =so.AuditedBy
                WHERE 1=1 {0} ";

            var paggerSqlString = @"
                ORDER BY so.Id DESC 
                OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ";

            var itemsSqlString = @"
                SELECT DISTINCT soi2.*,p.HasSNCode
                FROM SaleOrder so
                INNER JOIN SaleOrderItem soi ON so.Id = soi.SaleOrderId
                INNER JOIN SaleOrderItem soi2 ON so.Id = soi2.SaleOrderId
                LEFT JOIN Product p ON soi.ProductId = p.Id
                LEFT JOIN Category c ON p.CategoryId = c.Id
                LEFT JOIN Brand b ON p.BrandId = b.Id
                LEFT JOIN Store s ON so.StoreId = s.Id
                LEFT JOIN Account a on a.id =so.CreatedBy
                LEFT JOIN Account a1 on a1.id =so.AuditedBy
                WHERE 1=1 {0} ";

            dynamic param = new ExpandoObject();
            string where = string.Empty;

            if (condition.Ids.IsNotEmpty())
            {
                where += " AND so.Id IN @Ids";
                param.Ids = condition.Ids.Split(',');
            }

            if (condition.OrderType.HasValue)
            {
                where += " AND so.OrderType = @OrderType";
                param.OrderType = condition.OrderType;
            }

            if (condition.BillType.HasValue)
            {
                where += " AND so.BillType = @BillType";
                param.BillType = condition.BillType;
            }

            if (condition.FJCode.IsNotEmpty())
            {
                where += " AND soi.FJCode = @FJCode";
                param.FJCode = condition.FJCode;
            }

            if (condition.SourceSaleOrderCode.IsNotEmpty())
            {
                where += " AND so.SourceSaleOrderCode = @SourceSaleOrderCode";
                param.SourceSaleOrderCode = condition.SourceSaleOrderCode;
            }

            if (condition.SaleOrderId.HasValue)
            {
                where += " AND soi.SaleOrderId = @SaleOrderId";
                param.SaleOrderId = condition.SaleOrderId;
            }

            if (condition.StoreId.NotNullOrEmpty())
            {
                where += " AND s.Id IN @StoreId";
                param.StoreId = condition.StoreId.Split(',');
            }

            if (condition.CreateOnFrom.HasValue)
            {
                where += " AND so.CreatedOn >= @CreateOnFrom ";
                param.CreateOnFrom = condition.CreateOnFrom.Value;
            }

            if (condition.CreateOnTo.HasValue)
            {
                where += " AND so.CreatedOn <= @CreateOnTo ";
                param.CreateOnTo = condition.CreateOnTo.Value.AddDays(1);
            }

            if (condition.CategoryCode.IsNotEmpty())
            {
                where += " AND c.Code = @CategoryCode ";
                param.CategoryCode = condition.CategoryCode;
            }

            if (condition.CategoryId.HasValue)
            {
                where += " AND c.Id = @CategoryId ";
                param.CategoryId = condition.CategoryId.Value;
            }

            if (condition.BrandCode.IsNotEmpty())
            {
                where += " AND b.Code = @BrandCode ";
                param.BrandCode = condition.BrandCode;
            }

            if (condition.BrandIds.NotNullOrEmpty())
            {
                where += " AND b.Id IN @BrandIds ";
                param.BrandIds = condition.BrandIds.Split(',');
            }

            if (condition.ProductCode.IsNotEmpty())
            {
                where += " AND soi.ProductCode LIKE @ProductCode ";
                param.ProductCode = string.Format("%{0}%", condition.ProductCode);
            }

            if (condition.ProductName.IsNotEmpty())
            {
                where += " AND soi.ProductName LIKE @ProductName ";
                param.ProductName = string.Format("%{0}%", condition.ProductName);
            }

            if (condition.StoreCode.IsNotEmpty())
            {
                where += " AND s.Code = @StoreCode ";
                param.StoreCode = condition.StoreCode;
            }

            if (condition.CreateBy.IsNotEmpty())
            {
                where += " AND so.CreatedBy = @CreateBy ";
                param.CreateBy = condition.CreateBy;
            }

            if (condition.Status.HasValue)
            {
                where += " AND so.Status = @Status ";
                param.Status = condition.Status.Value;
            }

            if (condition.RoStatus.IsNotEmpty())
            {
                where += " AND so.RoStatus IN @RoStatus ";
                param.RoStatus = condition.RoStatus.Split(',');
            }

            if (condition.OrderCode.IsNotEmpty())
            {
                where += " AND so.Code = @OrderCode ";
                param.OrderCode = condition.OrderCode;
            }
            if (condition.SAPCode.IsNotEmpty())
            {
                where += " AND so.SAPCode = @SAPCode ";
                param.SAPCode = condition.SAPCode;
            }

            if (condition.SNCode.IsNotEmpty())
            {
                where += " AND soi.SNCode like @SNCode ";
                param.SNCode = string.Format("%{0}%", condition.SNCode);
            }
            if (condition.ProductIds.IsNotEmpty())
            {
                where += "  AND P.ID IN @productIds ";
                param.productIds = condition.ProductIds.Split(',').ToIntArray();
            }
            var offset = (page.PageIndex - 1) * page.PageSize;

            var pagerSql = paggerSqlString.FormatWith(offset, page.PageSize);
            var maserSql = masterSqlString.FormatWith(where);
            var itemsSql = itemsSqlString.FormatWith(where);

            IEnumerable<SaleOrderDto> orders = Enumerable.Empty<SaleOrderDto>();
            IEnumerable<SaleOrderItemDto> items = Enumerable.Empty<SaleOrderItemDto>();

            if (page.IsPaging)
            {
                var sqlTotal = "SELECT COUNT(1) FROM ({0}) AS t".FormatWith(maserSql);
                page.Total = _db.DataBase.ExecuteScalar<int>(sqlTotal, param);

                orders = _db.Table<SaleOrderDto>().Query(maserSql + pagerSql, param);
            }
            else
            {
                orders = _db.Table<SaleOrderDto>().Query(maserSql, param);
            }

            if (orders.Any())
                items = _db.Table<SaleOrderItemDto>().Query(itemsSql, param);

            orders.ToList().ForEach(order =>
            {
                var orderItems = items.Where(x => x.SaleOrderId == order.Id);

                order.SetItems(orderItems.ToList());
            });

            return orders;
        }
        #endregion

        #region Read To SaleOrderModel
        protected virtual IEnumerable<SaleOrderModel> GetSaleOrderModels(Pager page, SearchSaleOrder condition)
        {
            var dtos = this.GetSaleOrderDtos(page, condition);

            var orderItems = dtos.SelectMany(x => x.Items).ToList();

            var models = dtos.Translate(orderItems);

            return models;
        }

        protected virtual SaleOrderModel GetSaleOrderModel(SearchSaleOrder condition)
        {
            var page = new Pager() { IsPaging = false, PageIndex = 1, PageSize = 1 };
            var models = this.GetSaleOrderModels(page, condition);

            return models.FirstOrDefault();
        }
        #endregion

        #region Read To SaleOrder
        protected virtual IEnumerable<SaleOrder> GetSaleOrders(Pager page, SearchSaleOrder condition)
        {
            var orders = this.GetSaleOrderDtos(page, condition);

            return orders.MapTo<IEnumerable<SaleOrder>>();
        }

        protected virtual SaleOrder GetSaleOrder(SearchSaleOrder condition)
        {
            var page = new Pager() { IsPaging = false, PageIndex = 1, PageSize = 1 };
            var models = this.GetSaleOrders(page, condition);

            return models.FirstOrDefault();
        }

        protected virtual SaleOrder GetSaleOrderByCode(string orderCode)
        {
            var filter = new SearchSaleOrder() { OrderCode = orderCode };
            var order = this.GetSaleOrder(filter);

            return order;
        }
        #endregion

        #region Update SaleOrder
        protected virtual SaleOrder DoUpdateSaleOrder(SaleOrder order, SaleOrderModel model)
        {
            order.Items.Clear();

            order.StoreId = model.StoreId;
            order.Remark = model.Remark;
            order.Phone = model.Phone;
            order.Buyer = model.Buyer;
            order.UpdatedBy = _context == null ? -1 : _context.CurrentAccount.AccountId;
            order.UpdatedOn = DateTime.Now;

            this.DoAddSaleOrderItems(order, model.Items, "edit");

            return order;
        }
        #endregion

        #region CheckStoreInventory
        protected virtual void CheckFJCodeExist(string fjCode)
        {
            if (_db.Table<SaleOrderItem>().Exists(x => x.FJCode == fjCode))
                throw new FriendlyException("富基单号：{0} 已存在".FormatWith(fjCode));
        }

        protected virtual void CheckStoreInventory(SaleOrderItem item, int storeId)
        {
            if (item.SNCode.IsNullOrEmpty())
                CheckInventory(item, storeId);
            else
                CheckInventoryBatch(item, storeId);
        }

        protected virtual void CheckInventory(SaleOrderItem item, int storeId)
        {
            var storeInventory = _storeInventory.GetInventory(storeId, item.ProductId);

            if (storeInventory == null)
                throw new FriendlyException("商品 " + item.ProductName + "(" + item.ProductCode + ") 不存在");

            if (storeInventory.Quantity < item.Quantity)
                throw new FriendlyException("商品 " + item.ProductName + "(" + item.ProductCode + ") 库存不足");
        }

        protected virtual void CheckInventoryBatch(SaleOrderItem item, int storeId)
        {
            var storeInventoryBatch = _storeInventory.GetInventoryBatch(storeId, item.SNCode, item.ProductId);

            if (storeInventoryBatch == null)
                throw new FriendlyException("商品 " + item.ProductName + "(" + item.ProductCode + ") 不存在");

            if (storeInventoryBatch.Quantity < item.Quantity)
                throw new FriendlyException("商品 " + item.ProductName + "(" + item.ProductCode + ") 库存不足");
        }
        #endregion

        #region SaleOrder OutStock
        //1.校验订单当前状态是否可出库
        protected virtual void CheckSaleOrderCanOutStock(SaleOrder order)
        {
            if (order.Status.NotIn(SaleOrderStatus.WaitOutStock, SaleOrderStatus.Audited))
                throw new FriendlyException("当前订单：{0} 状态为：{1} 不能出库".FormatWith(order.Code, order.Status.Description()));
        }

        //2.减批次库存
        protected virtual void DecreaseStoreInventoryBatch(SaleOrder order)
        {
            if (order.IsNull() || order.Items.Count() == 0)
                throw new FriendlyException("销售单据不存在或销售明细为空");

            this.DecreaseStoreInventoryBatchWithSNCode(order, OrderProductType.Product);
            this.DecreaseStoreInventoryBatchWithSNCode(order, OrderProductType.Gift);
            this.DecreaseStoreInventoryBatchWithNoSNCode(order, OrderProductType.Product);
            this.DecreaseStoreInventoryBatchWithNoSNCode(order, OrderProductType.Gift);
        }

        //2.1.减批次库存-有串码，根据串码减相应的库存
        protected virtual void DecreaseStoreInventoryBatchWithSNCode(SaleOrder order, OrderProductType productType)
        {
            var storeId = productType == OrderProductType.Gift ? order.StoreIdGift : order.StoreId;//判断从良品仓出还是从赠品仓出
            var orderItems = order.Items.Where(x => x.SNCode.IsNotEmpty() && x.GiftType == productType.Value());
            if (orderItems.Any() == false) return;

            var productIds = orderItems.Select(x => x.ProductId).Distinct();
            var snCodes = orderItems.Select(x => x.SNCode);
            var inventoryBatches = _storeInventory.GetInventoryBatch(storeId, snCodes, productIds);

            foreach (var item in orderItems)
            {
                var productInInventoryBatch = inventoryBatches.FirstOrDefault(x => x.SNCode == item.SNCode);
                var productInOrderItems = orderItems.Where(x => x.SNCode == item.SNCode);
                if (productInInventoryBatch == null || productInInventoryBatch.Quantity < productInOrderItems.Sum(x => x.Quantity))
                    throw new FriendlyException("商品{0} 批次库存不足".FormatWith(item.ProductCode));

                //记录修改历史
                _db.Insert(new StoreInventoryHistory(productInInventoryBatch.Id, item.ProductId, storeId, productInInventoryBatch.Quantity, -item.Quantity,
                    productInInventoryBatch.Price, productInInventoryBatch.BatchNo, order.Id, order.Code, order.GetBillIdentity(), order.CreatedBy, DateTime.Now, productInInventoryBatch.SupplierId, item.RealPrice, productInInventoryBatch.SNCode, item.CategoryPreferential, item.BrandPreferential));

                productInInventoryBatch.Quantity -= item.Quantity;
                //平均成本
                item.AvgCostPrice = productInInventoryBatch.Price;

                _db.Update(productInInventoryBatch);
            }
        }

        //2.2.减批次库存-没有串码，减最早入库的商品库存
        /*
         * SaleOrder Code = 1
         *  SaleOrderItem ProductId = 2     Quantity  1
         *      Gift ProductId = 4          Quantity  1
         *  SaleOrderItem ProductId = 3     Quantity  1
         *      Gift ProductId = 4          Quantity  1
         *
         * SaleOrder Code = 2
         *  SaleOrderItem ProductId = 2     Quantity  1
         *      Gift ProductId = 4          Quantity  1
         *  SaleOrderItem ProductId = 4     Quantity  1
         *      Gift ProductId = 2          Quantity  1
         *      
         * SaleOrder Code = 3
         *  SaleOrderItem ProductId = 2     Quantity  1
         *      Gift ProductId = 2          Quantity  1
         *  SaleOrderItem ProductId = 3     Quantity  1
         *      Gift ProductId = 3          Quantity  1
         *
         */
        protected virtual void DecreaseStoreInventoryBatchWithNoSNCode(SaleOrder order, OrderProductType productType)
        {
            var storeId = productType == OrderProductType.Gift ? order.StoreIdGift : order.StoreId;//判断从良品仓出还是从赠品仓出
            var orderItems = order.Items.Where(x => x.SNCode.IsEmpty() && x.GiftType == productType.Value());
            if (orderItems.Any() == false) return;

            //合并相同的商品
            var items = new List<SaleOrderItem>();
            foreach (var item in orderItems)
            {
                var stockOutItem = items.FirstOrDefault(m => m.ProductId == item.ProductId && m.SNCode == item.SNCode);
                if (stockOutItem != null)
                {
                    stockOutItem.Quantity += item.Quantity;
                    continue;
                }
                var copyItem = new SaleOrderItem();//用明细项的新实例来合并数量，避免引用更改
                item.MapTo(copyItem);
                items.Add(copyItem);
            }

            var productIds = items.Select(x => x.ProductId);
            var inventoryBatches = _storeInventory.GetInventoryBatch(storeId, productIds);

            foreach (var item in items)
            {
                var productInInventoryBatch = inventoryBatches.Where(x => x.ProductId == item.ProductId).OrderBy(x => x.CreatedOn);
                //var productInOrderItems = items.Where(x => x.ProductId == item.ProductId);

                if (productInInventoryBatch.Sum(x => x.Quantity) < item.Quantity)
                    throw new FriendlyException("商品{0} 批次库存不足".FormatWith(item.ProductCode));

                var totalPrice = 0M;
                var leftQty = item.Quantity;
                var categoryPreferential = item.CategoryPreferential / item.Quantity;//单品品类优惠额度
                var brandPreferential = item.BrandPreferential / item.Quantity;//单品品牌优惠额度

                foreach (var batchItem in productInInventoryBatch)
                {
                    if (leftQty == 0) break;

                    //扣减库存
                    var reduceQty = Math.Min(leftQty, batchItem.Quantity);

                    batchItem.Quantity -= reduceQty;

                    var totalCategoryPreferential = Math.Round(categoryPreferential * reduceQty, 2);//当前批次总的品类优惠金额
                    var totalBrandPreferential = Math.Round(brandPreferential * reduceQty, 2);//当前批次总的品牌优惠金额

                    _db.Insert(new StoreInventoryHistory(batchItem.Id, item.ProductId, storeId, batchItem.Quantity, -reduceQty,
                            batchItem.Price, batchItem.BatchNo, order.Id, order.Code, order.GetBillIdentity(), order.CreatedBy, DateTime.Now, batchItem.SupplierId, item.RealPrice, null, totalCategoryPreferential, totalBrandPreferential));

                    leftQty -= reduceQty;

                    //累积批次总成本
                    totalPrice += (batchItem.Price * reduceQty);

                    _db.Update(batchItem);
                }

                if (productType != OrderProductType.Gift)
                {
                    //item.AvgCostPrice = totalPrice / item.Quantity;
                    var avgCostPrice = totalPrice / item.Quantity;
                    orderItems.Where(m => m.ProductId == item.ProductId && m.SNCode == item.SNCode).Select(m => { m.AvgCostPrice = avgCostPrice; return m; });
                }
            }
        }

        //3.减总库存
        protected virtual void DecreaseStoreInventory(SaleOrder order)
        {
            DoDecreaseStoreInventory(order, OrderProductType.Product);
            DoDecreaseStoreInventory(order, OrderProductType.Gift);
        }

        protected virtual void DoDecreaseStoreInventory(SaleOrder order, OrderProductType productType)
        {
            if (order.IsNull() || order.Items.Count() == 0)
                throw new FriendlyException("销售单据不存在或销售明细为空");

            var orderItems = order.Items.Where(x => x.GiftType == productType.Value());
            if (orderItems.IsEmpty())
            {
                if (productType == OrderProductType.Gift) return;
                throw new FriendlyException("销售明细为空");
            }

            var storeId = productType == OrderProductType.Gift ? order.StoreIdGift : order.StoreId;//判断从良品仓出还是从赠品仓出
            var productIds = orderItems.Select(x => x.ProductId).Distinct();
            var inventories = _storeInventory.GetInventory(storeId, productIds);

            var products = orderItems.GroupBy
            (
                x => new { x.ProductId, x.ProductCode },
                x => new { x.Quantity },
                (key, items) => new
                {
                    ProductId = key.ProductId,
                    ProductCode = key.ProductCode,
                    Quantity = items.Sum(t => t.Quantity)
                }
            );

            foreach (var item in products)
            {
                var inventory = inventories.First(x => x.ProductId == item.ProductId);
                if (inventory.Quantity < item.Quantity)
                    throw new FriendlyException("商品{0} 总仓库存不足".FormatWith(item.ProductCode));

                inventory.Quantity -= item.Quantity;
                //inventory.SaleQuantity -= item.Quantity;

                _db.Update(inventory);
            }
        }

        //4.发给SAP的数据  [dbo].[StoreInventoryHistorySAP]      返回 StoreInventoryHistorySAP 表的Code
        protected virtual string SaveStoreInventoryHistorySap(SaleOrder order)
        {
            var historyCode = _billSequenceService.GenerateNewCode(BillIdentity.SapHistoryOrder);
            DoSaveStoreInventoryHistorySap(order, OrderProductType.Product, historyCode);
            DoSaveStoreInventoryHistorySap(order, OrderProductType.Gift, historyCode);
            //if (giftResult != null) productResult.AddRange(giftResult);
            //return productResult;
            return historyCode;
        }

        private List<StoreInventoryHistorySAP> DoSaveStoreInventoryHistorySap(SaleOrder order, OrderProductType productType, string historyCode)
        {
            var items = order.Items.Where(x => x.GiftType == productType.Value());
            if (items.IsEmpty())
            {
                if (productType == OrderProductType.Gift) return null;
                throw new FriendlyException("销售明细为空");
            }

            var storeId = productType == OrderProductType.Gift ? order.StoreIdGift : order.StoreId;//判断从良品仓出还是赠品仓进/出
            var store = _storeFacade.LoadStore(storeId);
            List<StoreInventoryHistorySAP> saps = new List<StoreInventoryHistorySAP>();
            foreach (var item in items)
            {
                var model = new StoreInventoryHistorySAP();
                model.Code = historyCode;
                model.Type = order.OrderType == OrderType.Order.Value() ? StoreInventoryHistorySapType.OutStock : StoreInventoryHistorySapType.InStock;
                model.ProductId = item.ProductId;
                model.ProductCode = item.ProductCode;
                model.StoreId = storeId;
                model.StoreCode = store.Code;
                model.BillSapCode = order.SapCode;
                model.BillSapRow = item.SapRow;
                model.Unit = item.Unit;
                model.BillItemId = item.Id;
                model.SAPCode = ""; //todo
                model.SAPRow = "";//todo
                model.Quantity = order.BillType == SaleOrderBillType.BatchOrder ? item.ActualQuantity : item.Quantity;  // 批发订单按照实发数来记录;
                model.SNCodes = item.SNCode;
                model.BillCode = order.Code;
                model.BillType = order.GetBillIdentity();
                model.CreatedOn = DateTime.Now;
                model.CreatedBy = _context == null ? -1 : _context.CurrentAccount.AccountId;

                _db.Insert(model);
                saps.Add(model);
            }
            return saps;
        }

        //5.调用Sap接口
        //InvokeSapService

        //6.修改订单状态：已出库
        protected virtual void UpdateSaleOrderStatusToOutStock(SaleOrder order)
        {
            order.Status = SaleOrderStatus.OutStock;
        }
        #endregion

        #region Return InStock
        ///*
        // * 1.还原批次库存
        // * 
        // * 1.1.有串码，根据串码还原相应的库存
        // * 1.2.没有串码，还原到最早入库的商品库存
        // */
        //protected virtual void IncreaseStoreInventoryBatch(SaleOrder order)
        //{
        //    if (order.IsNull() || order.Items.Count() == 0)
        //        throw new FriendlyException("单据不存在或明细为空");

        //    var storeId = order.StoreId;

        //    foreach (var item in order.Items)
        //    {
        //        var productId = item.ProductId;
        //        var snCode = item.SNCode;
        //        var quantty = item.Quantity;

        //        var inventory = _storeInventory.GetInventory(storeId, productId);

        //        if (snCode.IsNotEmpty())
        //        {
        //            var inventoryBatch = _db.Table<StoreInventoryBatch>().FirstOrDefault(x => x.ProductId == productId && x.SNCode == snCode);
        //            if (inventoryBatch == null)
        //                throw new FriendlyException("商品{0} 批次库存不存在".FormatWith(item.ProductCode));

        //            //记录修改历史
        //            _db.Insert(new StoreInventoryHistory(inventoryBatch.Id, item.ProductId, storeId, inventory.Quantity, item.Quantity,
        //                inventoryBatch.Price, inventoryBatch.BatchNo, order.Id, order.Code, order.GetBillIdentity(), order.CreatedBy, order.UpdatedOn, inventoryBatch.SupplierId, item.RealPrice));

        //            inventoryBatch.Quantity += quantty;

        //            _db.Update(inventoryBatch);
        //        }
        //        else
        //        {
        //            var productBatch = _db.Table<StoreInventoryBatch>()
        //                                  .OrderBy(x => x.CreatedOn)
        //                                  .FirstOrDefault(x => x.StoreId == storeId && x.ProductId == productId && x.Quantity > 0);

        //            if (productBatch == null)
        //                throw new FriendlyException("商品{0} 批次库存不存在".FormatWith(item.ProductCode));

        //            productBatch.Quantity += item.Quantity;

        //            //记录修改历史
        //            _db.Insert(new StoreInventoryHistory(productBatch.Id, item.ProductId, storeId, inventory.Quantity, -item.Quantity,
        //                productBatch.Price, productBatch.BatchNo, order.Id, order.Code, order.GetBillIdentity(), order.CreatedBy, order.UpdatedOn, productBatch.SupplierId, item.RealPrice));

        //            _db.Update(productBatch);
        //        }
        //    }
        //}

        ////2.还原总库存
        //protected virtual void IncreaseStoreInventory(SaleOrder order)
        //{
        //    if (order.IsNull() || order.Items.Count() == 0)
        //        throw new FriendlyException("单据不存在或明细为空");

        //    var storeId = order.StoreId;

        //    foreach (var item in order.Items)
        //    {
        //        var productId = item.ProductId;
        //        var snCode = item.SNCode;
        //        var quantty = item.Quantity;

        //        var inventory = _storeInventory.GetInventory(storeId, productId);

        //        inventory.Quantity += item.Quantity;
        //        inventory.SaleQuantity += item.Quantity;

        //        _db.Update(inventory);
        //    }
        //}


        protected virtual string InStock(SaleOrder order)
        {
            Ensure.NotNull(order, "单据不存在。");
            Ensure.NotNullOrEmpty(order.Items, "单据明细为空。");
            Ensure.NotEqualThan(order.StoreId, order.StoreIdGift, "良品仓和赠品仓不能相同。");

            this.DoInStock(order, OrderProductType.Product);
            this.DoInStock(order, OrderProductType.Gift);

            var historyCode = this.SaveStoreInventoryHistorySap(order);

            return historyCode;
        }

        private StoreInventoryResult DoInStock(SaleOrder order, OrderProductType productType)
        {
            var storeId = productType == OrderProductType.Gift ? order.StoreIdGift : order.StoreId;//判断从良品仓退还是从赠品仓退
            var returnItems = order.Items.Where(m => m.GiftType == productType.Value());
            if (returnItems.IsEmpty()) return null;

            var currentAccount = _context.CurrentAccount;
            Ensure.When(() => !currentAccount.HaveAllStores).Then(() => Ensure.In(storeId, currentAccount.StoreArray, "抱歉，您没有该门店的操作权限。"));
            Ensure.NotNullOrEmpty(order.SourceSaleOrderCode, "原单号为空！");

            var sourceSaleOrderCode = order.SourceSaleOrderCode;
            var oriSaleOrder = _db.Table<SaleOrder>().FirstOrDefault(o => o.Code == sourceSaleOrderCode);
            Ensure.NotNull(oriSaleOrder, "原单不存在！");

            //合并相同的商品
            var items = new List<SaleOrderItem>();
            foreach (var item in returnItems)
            {
                var stockInItem = items.FirstOrDefault(m => m.ProductId == item.ProductId && m.SNCode == item.SNCode);
                if (stockInItem != null)
                {
                    stockInItem.Quantity += item.Quantity;
                    continue;
                }
                var copyItem = new SaleOrderItem();//用明细项的新实例来合并数量，避免引用更改
                item.MapTo(copyItem);
                items.Add(copyItem);
            }

            var stockInModel = new StockInModel()
            {
                InStockType = InStockType.Return,
                StockInBillId = order.Id,
                StockInBillCode = order.Code,
                StockInBillType = order.GetBillIdentity(),
                StockOutBillId = oriSaleOrder.Id,
                StockOutBillType = oriSaleOrder.GetBillIdentity(),
                StoreId = storeId,
                CreatedBy = currentAccount.AccountId,
                CreatedOn = DateTime.Now,
                Items = items.Select(item => new StockInItemModel()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SNCode = item.SNCode,
                    BrandPreferential = item.BrandPreferential,
                    CategoryPreferential = item.CategoryPreferential,
                }).ToList()
            };

            return _storeInventoryFacade.InStock(_db, stockInModel, false);
        }

        //3.发给SAP的数据  [dbo].[StoreInventoryHistorySAP]
        //4.调用第三方接口
        //5.修改退单状态：已入库
        protected virtual void UpdateSaleOrderStatusToInStock(SaleOrder order)
        {
            //Expression<Func<SaleOrder, SaleOrder>> columns = o =>
            //        new SaleOrder() { RoStatus = (int)ReturnSaleOrderStatus.InStock };

            //Expression<Func<SaleOrder, bool>> where = o => o.Code == order.Code;

            //_db.Update<SaleOrder>(columns, where);
            order.RoStatus = ReturnSaleOrderStatus.InStock.Value();
        }
        #endregion

        #region Create ReturnOrder
        protected virtual void CheckSaleOrderStatus(SaleOrderModel model)
        {
            Ensure.NotNullOrEmpty(model.SourceSaleOrderCode, "销售单号不能为空");

            var condition = new SearchSaleOrder() { OrderCode = model.SourceSaleOrderCode, OrderType = OrderType.Order.Value() };
            var order = this.GetSaleOrder(condition);

            Ensure.NotNull(order, "销售单：{0} 不存在".FormatWith(model.SourceSaleOrderCode));
            Ensure.SmallerOrEqualThan(SaleOrderStatus.OutStock.Value(), order.Status.Value(), "当前销售单：{0} 状态为：{1} 不能退货".FormatWith(order.Code, order.Status.Description()));
        }

        protected virtual void CheckReturnProduct(SaleOrderModel model)
        {
            var condition = new SearchSaleOrder() { OrderCode = model.SourceSaleOrderCode, OrderType = OrderType.Order.Value() };
            var order = this.GetSaleOrder(condition);

            Ensure.NotNull(order, "销售单：{0} 不存在".FormatWith(model.SourceSaleOrderCode));

            foreach (var item in model.Items)
            {
                var orderProduct = order.Items.FirstOrDefault(x => x.ProductId == item.ProductId);

                Ensure.NotNull(orderProduct, "商品：{0} 不是订单：{1} 中的商品".FormatWith(item.ProductCode, model.SourceSaleOrderCode));

                //foreach (var orderGift in orderProduct.GiftItems)
                //{
                //    var gift = item.GiftItems.FirstOrDefault(x => x.GiftProductId == orderGift.GiftProductId);

                //    if (gift == null)
                //        throw new FriendlyException("请退还订单：{0} 中的赠品：{1}".FormatWith(model.SourceSaleOrderCode, orderGift.GiftProductName));

                //    if (gift.GiftQuantity != orderGift.GiftQuantity)
                //        throw new FriendlyException("赠品：{0} 退还数量与订单所赠数量不一致".FormatWith(gift.GiftProductId));
                //}
            }
        }

        protected virtual void CheckProductHasReturned(SaleOrderModel model)
        {
            if (model.SourceSaleOrderCode.IsEmpty())
                return;

            var condition = new SearchSaleOrder() { SourceSaleOrderCode = model.SourceSaleOrderCode, OrderType = OrderType.Return.Value() };
            var returnOrders = this.GetSaleOrder(condition);

            if (returnOrders != null)
            {
                foreach (var item in model.Items)
                {
                    if (returnOrders.Items.Any(x => x.ProductId == item.ProductId))
                        throw new FriendlyException("订单：{0} 中的商品：{1} 已退".FormatWith(model.SourceSaleOrderCode, item.ProductCode));
                }
            }
        }
        #endregion
        #region Read To GetSaleOrderItemDtos
        protected virtual IEnumerable<SaleOrderDto> GetSaleOrderItemDtos(Pager page, SearchSaleOrder condition)
        {
            var masterSqlString = @"
                SELECT DISTINCT so.*,s.Name as StoreName,a.NickName as CreatedByName,a1.NickName as AuditedByName,soi.productcode,soi.productname,soi.sncode
                FROM SaleOrder so
                INNER JOIN SaleOrderItem soi ON so.Id = soi.SaleOrderId
                INNER JOIN Product p ON soi.ProductId = p.Id
                INNER JOIN Category c ON p.CategoryId = c.Id
                INNER JOIN Brand b ON p.BrandId = b.Id
                INNER JOIN Store s ON so.StoreId = s.Id
                INNER JOIN Account a on a.id =so.CreatedBy
                LEFT JOIN Account a1 on a1.id =so.AuditedBy
                WHERE 1=1 {0} ";

            var paggerSqlString = @"
                ORDER BY so.Code DESC 
                OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ";

            var itemsSqlString = @"
                SELECT DISTINCT soi2.*
                FROM SaleOrder so
                INNER JOIN SaleOrderItem soi ON so.Id = soi.SaleOrderId
                INNER JOIN SaleOrderItem soi2 ON so.Id = soi2.SaleOrderId
                INNER JOIN Product p ON soi.ProductId = p.Id
                INNER JOIN Category c ON p.CategoryId = c.Id
                INNER JOIN Brand b ON p.BrandId = b.Id
                INNER JOIN Store s ON so.StoreId = s.Id
                INNER JOIN Account a on a.id =so.CreatedBy
                LEFT JOIN Account a1 on a1.id =so.AuditedBy
                WHERE 1=1 {0} ";

            dynamic param = new ExpandoObject();
            string where = string.Empty;

            if (condition.OrderType.HasValue)
            {
                where += " AND so.OrderType = @OrderType";
                param.OrderType = condition.OrderType;
            }

            if (condition.BillType.HasValue)
            {
                where += " AND so.BillType = @BillType";
                param.BillType = condition.BillType;
            }

            if (condition.FJCode.IsNotEmpty())
            {
                where += " AND soi.FJCode = @FJCode";
                param.FJCode = condition.FJCode;
            }

            if (condition.SourceSaleOrderCode.IsNotEmpty())
            {
                where += " AND so.SourceSaleOrderCode = @SourceSaleOrderCode";
                param.SourceSaleOrderCode = condition.SourceSaleOrderCode;
            }

            if (condition.SaleOrderId.HasValue)
            {
                where += " AND soi.SaleOrderId = @SaleOrderId";
                param.SaleOrderId = condition.SaleOrderId;
            }

            if (condition.StoreId.NotNullOrEmpty())
            {
                where += " AND s.Id IN @StoreId";
                param.StoreId = condition.StoreId.Split(',');
            }

            if (condition.CreateOnFrom.HasValue)
            {
                where += " AND so.CreatedOn >= @CreateOnFrom ";
                param.CreateOnFrom = condition.CreateOnFrom.Value;
            }

            if (condition.CreateOnTo.HasValue)
            {
                where += " AND so.CreatedOn <= @CreateOnTo ";
                param.CreateOnTo = condition.CreateOnTo.Value.AddDays(1);
            }

            if (condition.CategoryCode.IsNotEmpty())
            {
                where += " AND c.Code = @CategoryCode ";
                param.CategoryCode = condition.CategoryCode;
            }

            if (condition.BrandCode.IsNotEmpty())
            {
                where += " AND b.Code = @BrandCode ";
                param.BrandCode = condition.BrandCode;
            }

            if (condition.ProductCode.IsNotEmpty())
            {
                where += " AND soi.ProductCode LIKE @ProductCode ";
                param.ProductCode = string.Format("%{0}%", condition.ProductCode);
            }

            if (condition.ProductName.IsNotEmpty())
            {
                where += " AND soi.ProductName LIKE @ProductName ";
                param.ProductName = string.Format("%{0}%", condition.ProductName);
            }

            if (condition.StoreCode.IsNotEmpty())
            {
                where += " AND s.Code = @StoreCode ";
                param.StoreCode = condition.StoreCode;
            }

            if (condition.CreateBy.IsNotEmpty())
            {
                where += " AND so.CreatedBy = @CreateBy ";
                param.CreateBy = condition.CreateBy;
            }

            if (condition.Status.HasValue)
            {
                where += " AND so.Status = @Status ";
                param.Status = condition.Status.Value;
            }

            if (condition.OrderCode.IsNotEmpty())
            {
                where += " AND so.Code = @OrderCode ";
                param.OrderCode = condition.OrderCode;
            }

            if (condition.SNCode.IsNotEmpty())
            {
                where += " AND soi.SNCode like @SNCode ";
                param.SNCode = string.Format("%{0}%", condition.SNCode);
            }
            if (condition.ProductIds.IsNotEmpty())
            {
                where += "  AND P.ID IN @productIds ";
                param.productIds = condition.ProductIds.Split(',').ToIntArray();
            }
            var offset = (page.PageIndex - 1) * page.PageSize;

            var pagerSql = paggerSqlString.FormatWith(offset, page.PageSize);
            var maserSql = masterSqlString.FormatWith(where);
            var itemsSql = itemsSqlString.FormatWith(where);

            IEnumerable<SaleOrderDto> orders = Enumerable.Empty<SaleOrderDto>();
            IEnumerable<SaleOrderItemDto> items = Enumerable.Empty<SaleOrderItemDto>();

            if (page.IsPaging)
            {
                var sqlTotal = "SELECT COUNT(1) FROM ({0}) AS t".FormatWith(maserSql);
                page.Total = _db.DataBase.ExecuteScalar<int>(sqlTotal, param);

                orders = _db.Table<SaleOrderDto>().Query(maserSql + pagerSql, param);
            }
            else
            {
                orders = _db.Table<SaleOrderDto>().Query(maserSql, param);
            }

            if (orders.Any())
                items = _db.Table<SaleOrderItemDto>().Query(itemsSql, param);

            orders.ToList().ForEach(order =>
            {
                var orderItems = items.Where(x => x.SaleOrderId == order.Id);

                order.SetItems(orderItems.ToList());
            });

            return orders;
        }

        protected virtual IEnumerable<SaleOrderModel> GetSaleOrderItemModels(Pager page, SearchSaleOrder condition)
        {
            var dtos = this.GetSaleOrderItemDtos(page, condition);

            var orderItems = dtos.SelectMany(x => x.Items).ToList();

            var models = dtos.Translate(orderItems);

            return models;
        }
        #endregion

        #region read to saleorderlistDetail

        /// <summary>
        /// 销售单综合查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IEnumerable<SaleOrderListDetailDto> GetSaleOrderListDetail(Pager page, SearchSaleOrder condition)
        {
            string sql = @"SELECT  *  FROM    ( SELECT    so.Id ,
        so.Code ,so.CreatedOn ,so.PaidDate ,so.OrderAmount , so.Status ,so.RoStatus ,so.Buyer ,so.Phone ,so.OrderType,so.BillType,
        s.Name AS StoreName ,so.ParentCode,so.SAPCode,so.IsPushSap,
        ROW_NUMBER() OVER ( ORDER BY so.Id DESC ) AS ROWS,
		p.Id as ProductId,p.Code as ProductCode,p.Name as ProductName,p.Spec as Specification,p.Unit,(CASE WHEN so.OrderType=2 THEN -i.Quantity ELSE i.Quantity END) AS Quantity,(CASE WHEN so.OrderType=2 THEN -i.ActualQuantity ELSE i.ActualQuantity END) AS ActualQuantity,i.SalePrice,i.RealPrice,i.MinSalePrice,i.FJCode,i.SNCode,
		c.FullName as CategoryName,b.Name as BrandName,
        a.NickName as CreatedByName,a1.NickName as AuditedByName   
        FROM      SaleOrder so
        inner join SaleOrderItem i on so.Id = i.SaleOrderId
		INNER JOIN Store s ON so.StoreId = s.Id
		left join product p on p.Id = i.ProductId
		left join Category c on c.Id = p.CategoryId
		left join Brand b on b.Id = p.BrandId
        LEFT JOIN Account a on a.id =so.CreatedBy
        LEFT JOIN Account a1 on a1.id =so.AuditedBy		
WHERE   1 = 1  {0}  ) AS t";
            dynamic param = new ExpandoObject();
            string where = string.Empty;

            if (condition.OrderType.HasValue)
            {
                where += " AND so.OrderType = @OrderType";
                param.OrderType = condition.OrderType;
            }

            if (condition.BillType.HasValue)
            {
                where += " AND so.BillType = @BillType";
                param.BillType = condition.BillType;
            }

            if (condition.FJCode.IsNotEmpty())
            {
                where += " AND i.FJCode like @FJCode";
                param.FJCode = string.Format("{0}%", condition.FJCode);
            }
            if (condition.SAPCode.IsNotEmpty())
            {
                where += " AND so.SAPCode = @SAPCode";
                param.SAPCode = condition.SAPCode;
            }

            if (condition.SourceSaleOrderCode.IsNotEmpty())
            {
                where += " AND so.SourceSaleOrderCode = @SourceSaleOrderCode";
                param.SourceSaleOrderCode = condition.SourceSaleOrderCode;
            }

            if (condition.SaleOrderId.HasValue)
            {
                where += " AND i.SaleOrderId = @SaleOrderId";
                param.SaleOrderId = condition.SaleOrderId;
            }

            if (condition.StoreId.NotNullOrEmpty())
            {
                where += " AND s.Id IN @StoreId";
                param.StoreId = condition.StoreId.Split(',');
            }

            if (condition.CreateOnFrom.HasValue)
            {
                where += " AND so.CreatedOn >= @CreateOnFrom ";
                param.CreateOnFrom = condition.CreateOnFrom.Value;
            }

            if (condition.CreateOnTo.HasValue)
            {
                where += " AND so.CreatedOn < @CreateOnTo ";
                param.CreateOnTo = condition.CreateOnTo.Value.AddDays(1);
            }

            if (condition.SaleTimeFrom.HasValue)
            {
                where += " AND so.PaidDate >= @SaleTimeFrom ";
                param.SaleTimeFrom = condition.SaleTimeFrom.Value;
            }

            if (condition.SaleTimeTo.HasValue)
            {
                where += " AND so.PaidDate < @SaleTimeTo ";
                param.SaleTimeTo = condition.SaleTimeTo.Value.AddDays(1);
            }

            if (condition.CategoryCode.IsNotEmpty())
            {
                where += " AND c.Code LIKE @CategoryCode ";
                param.CategoryCode = string.Format("{0}%", condition.CategoryCode);
            }

            if (condition.CategoryId.HasValue)
            {
                where += " AND c.Id = @CategoryId ";
                param.CategoryId = condition.CategoryId.Value;
            }

            if (condition.BrandCode.IsNotEmpty())
            {
                where += " AND b.Code = @BrandCode ";
                param.BrandCode = condition.BrandCode;
            }

            if (condition.BrandIds.NotNullOrEmpty())
            {
                where += " AND b.Id IN @BrandIds ";
                param.BrandIds = condition.BrandIds.Split(',');
            }

            if (condition.ProductCode.IsNotEmpty())
            {
                where += " AND i.ProductCode LIKE @ProductCode ";
                param.ProductCode = string.Format("%{0}%", condition.ProductCode);
            }

            if (condition.ProductName.IsNotEmpty())
            {
                where += " AND i.ProductName LIKE @ProductName ";
                param.ProductName = string.Format("%{0}%", condition.ProductName);
            }

            if (condition.StoreCode.IsNotEmpty())
            {
                where += " AND s.Code = @StoreCode ";
                param.StoreCode = condition.StoreCode;
            }

            if (condition.CreateBy.IsNotEmpty())
            {
                where += " AND so.CreatedBy = @CreateBy ";
                param.CreateBy = condition.CreateBy;
            }

            if (condition.Status.HasValue)
            {
                where += " AND so.Status = @Status ";
                param.Status = condition.Status.Value;
            }

            if (condition.RoStatus.IsNotEmpty())
            {
                where += " AND so.RoStatus IN @RoStatus ";
                param.RoStatus = condition.RoStatus.Split(',');
            }

            if (condition.OrderCode.IsNotEmpty())
            {
                where += " AND so.Code = @OrderCode ";
                param.OrderCode = condition.OrderCode;
            }

            if (condition.SNCode.IsNotEmpty())
            {
                where += " AND i.SNCode like @SNCode ";
                param.SNCode = string.Format("%{0}%", condition.SNCode);
            }
            if (condition.ProductIds.IsNotEmpty())
            {
                where += "  AND P.ID IN @productIds ";
                param.productIds = condition.ProductIds.Split(',').ToIntArray();
            }

            if (condition.IsPushSap.HasValue)
            {
                where += " AND so.IsPushSap = @IsPushSap ";
                param.IsPushSap = condition.IsPushSap.Value;
            }

            if (page.toExcel)
            {
                sql = string.Format(sql, where);
                return _db.Table<SaleOrderListDetailDto>().Query(sql, param);
            }
            else
            {
                sql += " WHERE  ROWS BETWEEN {1} AND {2} order by t.Id desc";
            }

            page.Total = _db.DataBase.ExecuteScalar<int>(string.Format(@"SELECT COUNT(*) FROM 
  SaleOrder so
        inner join SaleOrderItem i on so.Id = i.SaleOrderId
		INNER JOIN Store s ON so.StoreId = s.Id
		left join product p on p.Id = i.ProductId
		left join Category c on c.Id = p.CategoryId
		left join Brand b on b.Id = p.BrandId
        LEFT JOIN Account a on a.id =so.CreatedBy
        LEFT JOIN Account a1 on a1.id =so.AuditedBy	
WHERE 1=1 {0}", where), param);

            sql = string.Format(sql, where, (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize);
            return _db.Table<SaleOrderListDetailDto>().Query(sql, param);
        }

        #endregion



    }

    internal static class SaleOrderExtension
    {
        internal static SaleOrderModel Translate(this SaleOrderDto order)
        {
            var masterDto = new SaleOrderModel();
            masterDto.MapFrom(order);

            return masterDto;
        }

        internal static void SetOrderItems(this SaleOrderModel order, List<SaleOrderItemDto> items)
        {
            var itemDtos = new List<SaleOrderItemModel>();

            items.Where(si => si.GiftType == OrderProductType.Product.Value()).ToList().MapTo(itemDtos);

            foreach (var item in itemDtos)
            {
                var gifts = items.Where(si => si.GiftType == OrderProductType.Gift.Value() && si.ParentProductId == item.ParentProductId);

                item.GiftItems.MapFrom(gifts.ToList());

                order.Items.Add(item);
            }
        }

        internal static IEnumerable<SaleOrderModel> Translate(this IEnumerable<SaleOrderDto> orders, IEnumerable<SaleOrderItemDto> items)
        {
            foreach (var order in orders.Distinct())
            {
                order.Items.Clear();

                var masterDto = order.Translate();

                var orderItems = items.Where(x => x.SaleOrderId == order.Id).ToList();

                masterDto.SetOrderItems(orderItems);

                yield return masterDto;
            }
        }
    }
}