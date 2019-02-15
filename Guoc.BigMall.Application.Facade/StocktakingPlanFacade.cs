using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
using System.Dynamic;
using Guoc.BigMall.Domain.Entity;
using Newtonsoft.Json;
using Guoc.BigMall.Domain.Service;
using Guoc.BigMall.Domain.ValueObject;
using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Domain;
using Guoc.BigMall.Infrastructure.Utils;
using Guoc.BigMall.Application.DTO;
namespace Guoc.BigMall.Application.Facade
{
    public class StocktakingPlanFacade : IStocktakingPlannFacade
    {
        IDBContext _db;
        ILogger _log;
        BillSequenceService _billService;
        ISAPService _sapService;
        IContextFacade _context;
        IStoreInventoryFacade _storeInventoryFacade;


        public StocktakingPlanFacade(IDBContext dbContext, IContextFacade context, IStoreInventoryFacade storeInventoryFacade, ILogger log, ISAPService sapService)
        {
            _db = dbContext;
            _log = log;
            _billService = new BillSequenceService(_db);
            this._sapService = sapService;
            this._context = context;
            this._storeInventoryFacade = storeInventoryFacade;
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IEnumerable<StocktakingPlanDto> GetPageList(Pager page, SearchInventoryPlan condition)
        {
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrEmpty(condition.ProductCodeOrBarCode))
            {
                where += "and p.Code like @ProductCodeOrBarCode ";
                param.ProductCodeOrBarCode = string.Format("%{0}%", condition.ProductCodeOrBarCode);
            }
            if (!string.IsNullOrEmpty(condition.ProductName))
            {
                where += "and p.Name like @ProductName ";
                param.ProductName = string.Format("%{0}%", condition.ProductName);
            }
            if (!string.IsNullOrEmpty(condition.StoreId) && condition.StoreId != "0")
            {
                where += "and si.StoreId in @StoreId ";
                param.StoreId = condition.StoreId.Split(',').ToIntArray(); ;
            }
            if (!string.IsNullOrEmpty(condition.BrandId))
            {
                where += "and b.Id in  @BrandId ";
                param.BrandId = condition.BrandId.Split(',').ToIntArray();
            }
            if (!string.IsNullOrEmpty(condition.CategoryId))
            {
                where += "and c.Id=@CategoryId ";
                param.CategoryId = condition.CategoryId;
            }
            if (!string.IsNullOrWhiteSpace(condition.Time))
            {
                var times = condition.Time.Split(',');
                where += "and sp.StocktakingDate >=@StartDate  and sp.StocktakingDate < @EndDate";
                param.StartDate = Convert.ToDateTime(times[0]);
                param.EndDate = Convert.ToDateTime(times[1]).AddDays(1);
            }
            if (condition.Status.HasValue)
            {
                where += " and sp.Status =  @Status ";
                param.Status = condition.Status.Value;
            }
            if (condition.Difference)
            {
                where += " and (( spi.IsSNProduct= 0 and abs(spi.ComplexQuantity-spi.Quantity)>0 ) or (spi.IsSNProduct = 1 and len(spi.SurplusSNCode) + len(spi.MissingSNCode)>0 ))";
            }
            if (condition.IsSNProduct.HasValue)
            {
                where += " and spi.IsSNProduct = @IsSNProduct ";
                param.IsSNProduct = condition.IsSNProduct.Value;
            }
            if (!string.IsNullOrEmpty(condition.Code))
            {
                where += "and sp.Code like @Code ";
                param.Code = string.Format("{0}%", condition.Code);
            }

            string sql = @"SELECT  c.Code CategoryCode ,
                                    c.Name CategoryName ,
                                    b.Code BrandCode ,
                                    b.Name BrandName ,
                                    p.Code ProductCode ,
                                    p.Name ProductName ,
                                    sp.Status,
                                    s.Code StoreCode ,
                                    s.Name StoreName ,
                                    spi.Quantity ,
                                    spi.FirstQuantity ,                                 
                                    sp.StocktakingDate,
                                    spi.ComplexQuantity ,
                                    sp.ComplexDate,
                                    sp.Code PlanCode,
                                    sp.CreatedOn,
                                    spi.SurplusSNCode,                                 
                                    spi.MissingSNCode ,spi.IsSNProduct,sp.IsPushSap
                            FROM    StocktakingPlan sp
                                    INNER JOIN StocktakingPlanItem spi ON sp.Id = spi.StocktakingPlanId
                                    INNER JOIN dbo.Product p ON spi.ProductId = p.Id
                                    INNER JOIN dbo.Category c ON p.CategoryId = c.Id
                                    INNER JOIN dbo.Brand b ON b.Id = p.BrandId
                                    INNER JOIN dbo.StoreInventory si ON spi.ProductId = si.ProductId AND si.StoreId = sp.StoreId 
                                    INNER JOIN dbo.Store s ON si.StoreId = s.Id
                    WHERE 1=1 {0} 
                    ORDER BY spi.Id DESC OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY".FormatWith(where, page.PageSize * (page.PageIndex - 1), page.PageSize);

            var rows = this._db.DataBase.Query<StocktakingPlanDto>(sql, param);

            string sqlCount = @"SELECT  COUNT(1)
                                     FROM    StocktakingPlan sp
                                    INNER JOIN StocktakingPlanItem spi ON sp.Id = spi.StocktakingPlanId
                                    INNER JOIN dbo.Product p ON spi.ProductId = p.Id
                                    INNER JOIN dbo.Category c ON p.CategoryId = c.Id
                                    INNER JOIN dbo.Brand b ON b.Id = p.BrandId
                                    INNER JOIN dbo.StoreInventory si ON spi.ProductId = si.ProductId AND si.StoreId = sp.StoreId 
                                    INNER JOIN dbo.Store s ON si.StoreId = s.Id
                            WHERE  1=1 {0} ".FormatWith(where);
            page.Total = _db.DataBase.ExecuteScalar<int>(sqlCount, param);
            return rows;
        }

        public IEnumerable<StocktakingPlanProductDto> GetInventoryPlanProduct(int planId)
        {
            string sql = @"SELECT  s.Code StoreCode,s.Name StoreName,p.Code ProductCode,p.Name ProductName,(CASE WHEN spi.IsSNProduct = 1 THEN '是' ELSE '否' END) AS IsSNProduct,spi.Quantity,spi.Id PlanItemId, '0' InventoryQuantity,SurplusSNCode,MissingSNCode 
,c.FullName CategoryName,c.Code CategoryCode,b.Name  BrandName,b.Code  BrandCode                               
							    FROM    dbo.StocktakingPlan sp
                                        LEFT JOIN dbo.StocktakingPlanItem spi ON sp.Id = spi.StocktakingPlanId
                                        LEFT JOIN dbo.Product p ON p.Id = spi.ProductId
                                        LEFT JOIN dbo.Store s ON s.Id = sp.StoreId
                                        Left join category c on c.Id =p.CategoryId
										left join brand b on b.Id = p.BrandId
                                WHERE  sp.Id = {0} AND  sp.Status IN ( 1, 2, 3 )".FormatWith(planId);

            var rows = this._db.DataBase.Query<StocktakingPlanProductDto>(sql, null);
            return rows;
        }

        public int CreatePlan(int storeId, int userId, string userName)
        {
            StocktakingPlan entity = new StocktakingPlan();
            entity.StoreId = storeId;
            entity.StoreCode = _db.Table<Store>().FirstOrDefault(n => n.Id == storeId).Code;
            entity.CreatedBy = userId;
            entity.CreatedByName = userName;
            entity.CreatedOn = DateTime.Now;
            entity.UpdatedBy = userId;
            entity.UpdatedOn = DateTime.Now;
            entity.UpdatedByName = userName;
            entity.Status = StocktakingPlanStatus.ToBeInventory;
            entity.Method = 1;
            entity.Code = _billService.GenerateNewCode(BillIdentity.StoreStocktakingPlan);

            _db.Insert(entity);
            _db.SaveChange();

            //插入盘点明细
            AddPlanItem(storeId, entity.Id);

            return entity.Id;
        }

        public void AddPlanItem(int storeId, int planId)
        {
            string sql = @"INSERT INTO dbo.StocktakingPlanItem 
                                        ( StocktakingPlanId ,
                                          ProductId ,
                                          IsSNProduct ,
                                          CostPrice ,
                                          SalePrice ,
                                          Quantity ,
                                          ProductCode ,
                                          Unit 
                                        )
                                SELECT  {0},si.ProductId,p.HasSNCode IsSNProduct,si.LastCostPrice CostPrice , si.StoreSalePrice SalePrice ,si.Quantity,p.Code,p.Unit from StoreInventory si left join Product p
on si.ProductId = p.Id WHERE  si.StoreId = {1} ORDER BY p.Code".FormatWith(planId, storeId);
            _db.DataBase.ExecuteSql(sql);
            // _db.DataBase.AddExecute(sql);
            // _db.SaveChange();
        }

        //盘点导入
        public void InsertPlanItem(List<PlanProductDto> planProductItems)
        {
            Ensure.NotNullOrEmpty(planProductItems, "导入盘点明细为空！");

            var itemIds = planProductItems.Select(m => m.PlanItemId).ToArray();
            var items = _db.Table<StocktakingPlanItem>().Where(s => s.Id.In(itemIds)).ToList();
            Ensure.EqualThan(items.Count, planProductItems.Count, "导入盘点明细有误！");
            Ensure.EqualThan(items.GroupBy(m => m.StocktakingPlanId).Count(), 1, "导入盘点明细有误！");

            var stocktakingPlanId = items.First().StocktakingPlanId;
            var planEntity = _db.Table<StocktakingPlan>().FirstOrDefault(s => s.Id == stocktakingPlanId && s.Status >= StocktakingPlanStatus.ToBeInventory && s.Status < StocktakingPlanStatus.Complete);
            Ensure.NotNull(planEntity, "盘点单不存在，请先导出盘点计划！");
            // Ensure.In(planEntity.StoreId, _context.CurrentAccount.StoreArray, "抱歉，您没有操作权限。");

            //检查库存变动
            // CheckInventoryChange(planEntity);

            foreach (var item in items)
            {
                var planProductEntity = planProductItems.FirstOrDefault(p => p.PlanItemId == item.Id);
                if (planProductEntity == null) continue;

                var product = _db.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                Ensure.NotNull(product, "盘点商品{0}不存在！".FormatWith(item.ProductCode));

                var surplusSNCode = (planProductEntity.SurplusSNCode ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();//盘盈串码
                var missingSNCode = (planProductEntity.MissingSNCode ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();//盘亏串码

                //非串码商品
                Ensure.False(product.HasSNCode == false && (surplusSNCode.Count > 0 || missingSNCode.Count > 0), "非串码商品{0}不能使用串码盘点！".FormatWith(item.ProductCode));

                surplusSNCode.ForEach(snCode =>
                {
                    //未登记
                    var registered = _db.Table<StoreInventoryBatch>().Exists(i => i.SNCode == snCode);
                    Ensure.True(registered, "商品{0}的盘盈串码{1}未在本系统登记，请核实串码！".FormatWith(item.ProductCode, snCode));

                    //有库存
                    var inventoryBatch = _db.Table<StoreInventoryBatch>().FirstOrDefault(i => i.SNCode == snCode && i.Quantity > 0);
                    if (inventoryBatch != null)
                    {
                        Ensure.EqualThan(inventoryBatch.StoreId, planEntity.StoreId, "商品{0}的盘盈串码{1}在别的仓有库存，请核实串码！".FormatWith(item.ProductCode, snCode));
                        throw new FriendlyException("商品{0}的盘盈串码{1}在系统中有库存，不能盘盈！".FormatWith(item.ProductCode, snCode));
                    }
                });

                missingSNCode.ForEach(snCode =>
                {
                    //未登记
                    var registered = _db.Table<StoreInventoryBatch>().Exists(i => i.SNCode == snCode);
                    Ensure.True(registered, "商品{0}的盘亏串码{1}未在本系统登记，请核实串码！".FormatWith(item.ProductCode, snCode));

                    //无库存
                    var inventoryBatch = _db.Table<StoreInventoryBatch>().FirstOrDefault(i => i.StoreId == planEntity.StoreId && i.SNCode == snCode && i.Quantity > 0);
                    Ensure.NotNull(inventoryBatch, "商品{0}的盘亏串码{1}在系统中并无库存，请核实串码！".FormatWith(item.ProductCode, snCode));
                });

                item.SurplusSNCode = string.Join(",", surplusSNCode);
                item.MissingSNCode = string.Join(",", missingSNCode);

                if (product.HasSNCode == false)
                {
                    Ensure.GreaterOrEqualThan(planProductEntity.Quantity, 0, "商品{0}盘点数量必须≥0！".FormatWith(item.ProductCode));

                    if (planEntity.Status == StocktakingPlanStatus.ToBeInventory)
                        item.FirstQuantity = planProductEntity.Quantity;
                    else
                        item.ComplexQuantity = planProductEntity.Quantity;
                }
            }

            if (planEntity.Status == StocktakingPlanStatus.ToBeInventory)
            {
                planEntity.Status = StocktakingPlanStatus.FirstInventory;
                planEntity.StocktakingDate = DateTime.Now;
            }
            else //if (planEntity.Status == StocktakingPlanStatus.FirstInventory)
            {
                planEntity.Status = StocktakingPlanStatus.Replay;
                planEntity.ComplexDate = DateTime.Now;
            }

            _db.Update(planEntity);
            _db.Update(items.ToArray());
            _db.SaveChange();
        }

        #region 盘点结转

        public void Confirm(int storeId)
        {
            var entity = _db.Table<StocktakingPlan>().FirstOrDefault(s => s.StoreId == storeId && s.Status == StocktakingPlanStatus.Replay);
            Ensure.NotNull(entity, "盘点单不存在！");
            entity.Items = _db.Table<StocktakingPlanItem>().Where(n => n.StocktakingPlanId == entity.Id).ToList();
            Ensure.NotNullOrEmpty(entity.Items, "盘点明细为空！");
            /*------------------------------------------------------------------------------------------------
             * 盘盈：有实货，无库存
             * 盘亏：有库存，无实货
             *------------------------------------------------------------------------------------------------*/

            var currentAccount = _context.CurrentAccount;

            entity.Items.ForEach(item =>
            {
                var inventory = _db.Table<StoreInventory>().FirstOrDefault(i => i.StoreId == entity.StoreId && i.ProductId == item.ProductId);
                Ensure.NotNull(inventory, "商品{0}总库存不存在。".FormatWith(item.ProductCode));

                //无串码商品
                if (item.IsSNProduct == false)
                {
                    if (item.Quantity == item.ComplexQuantity) return;
                    if (item.Quantity < item.ComplexQuantity)   //盘盈
                        this.InventorySurplusWithNoSNCode(entity, item, inventory, currentAccount);
                    else   //盘亏
                        this.InventoryLossWithNoSNCode(entity, item, inventory, currentAccount);
                    return;
                }

                //串码盘亏
                this.InventoryLossWithSNCode(entity, item, inventory, currentAccount);

                //串码盘盈
                this.InventorySurplusWithSNCode(entity, item, inventory, currentAccount);
            });

            //盘点差异推送至SAP
            _sapService.SubmitInventoryDifference(entity);
            entity.IsPushSap = true;

            //盘点完成
            entity.Status = StocktakingPlanStatus.Complete;
            _db.Update(entity);
            _db.SaveChange();
        }

        //串码盘盈
        private void InventorySurplusWithSNCode(StocktakingPlan entity, StocktakingPlanItem item, StoreInventory inventory, AccountInfo currentAccount)
        {
            if (item.SurplusSNCode.IsNullOrEmpty()) return;

            var snCodes = item.SurplusSNCode.Split(',').ToList();

            var quantity = 1;
            var totalPrice = 0M;//总成本
            StoreInventoryHistory history;
            ProductPurchasePrice purchasePrice = null;
            snCodes.ForEach(snCode =>
            {
                //有库存
                var inventoryBatch = _db.Table<StoreInventoryBatch>().FirstOrDefault(i => i.SNCode == snCode && i.Quantity > 0);
                if (inventoryBatch != null)
                {
                    Ensure.EqualThan(inventoryBatch.StoreId, entity.StoreId, "商品{0}的盘盈串码{1}在别的仓有库存，请核实串码！".FormatWith(item.ProductCode, snCode));
                    throw new FriendlyException("商品{0}的盘盈串码{1}在系统中有库存，不能盘盈！".FormatWith(item.ProductCode, snCode));
                }

                //获取串码最后一次出库历史
                var lastStockOut = _db.Table<StoreInventoryHistory>().OrderByDesc(h => h.CreatedOn).FirstOrDefault(h => h.ProductId == item.ProductId && h.SNCode == snCode && h.ChangeQuantity < 0);
                if (lastStockOut != null && lastStockOut.StoreId == entity.StoreId)//最后一次出库在当前仓，用最后一次出库的批次还库存
                {
                    inventoryBatch = _db.Table<StoreInventoryBatch>().FirstOrDefault(i => i.StoreId == lastStockOut.StoreId && i.ProductId == lastStockOut.ProductId && i.SNCode == lastStockOut.SNCode && i.BatchNo > lastStockOut.BatchNo);
                    inventoryBatch.Quantity += quantity;
                    totalPrice += inventoryBatch.Price * quantity;
                    history = new StoreInventoryHistory(inventoryBatch.Id, item.ProductId, inventoryBatch.StoreId, inventory.Quantity, quantity, inventoryBatch.Price, inventoryBatch.BatchNo, entity.Id, entity.Code, BillIdentity.StoreStocktakingPlan, currentAccount.AccountId, DateTime.Now, inventoryBatch.SupplierId, 0, inventoryBatch.SNCode);
                    _db.Update(inventoryBatch);
                    _db.Insert(history);
                }
                else //新建库存批次
                {
                    if (purchasePrice == null)
                    {
                        var now = DateTime.Now;
                        purchasePrice = _db.Table<ProductPurchasePrice>().FirstOrDefault(p => p.ProductId == item.ProductId && p.Status == 1 && p.StartTime <= now && p.EndTime >= now);
                    }
                    Ensure.NotNull(purchasePrice, "在系统中找不到盘盈商品{0}的采购成本，无法新建库存批次！".FormatWith(item.ProductCode));

                    var batchNo = long.Parse(_billService.GenerateNewCode(BillIdentity.BatchNo));
                    inventoryBatch = new StoreInventoryBatch(item.ProductId, entity.StoreId, purchasePrice.SupplierId, quantity, purchasePrice.PurchasePrice, purchasePrice.PurchasePrice, batchNo, null, 0, currentAccount.AccountId, snCode);
                    history = new StoreInventoryHistory(item.ProductId, inventoryBatch.StoreId, inventory.Quantity, quantity, inventoryBatch.Price, inventoryBatch.BatchNo, entity.Id, entity.Code, BillIdentity.StoreStocktakingPlan, currentAccount.AccountId, inventoryBatch.SupplierId, inventoryBatch.SNCode);
                    totalPrice += inventoryBatch.Price * quantity;
                    inventoryBatch.History.Add(history);
                    _db.Insert(inventoryBatch);
                }
            });

            //增加总库存
            this.AddStoreInventory(inventory, snCodes.Count, (totalPrice / snCodes.Count));
        }

        //串码盘亏
        private void InventoryLossWithSNCode(StocktakingPlan entity, StocktakingPlanItem item, StoreInventory inventory, AccountInfo currentAccount)
        {
            if (item.MissingSNCode.IsNullOrEmpty()) return;

            var snCodes = item.MissingSNCode.Split(',').ToList();
            Ensure.SmallerOrEqualThan(snCodes.Count, inventory.Quantity, "商品{0}总库存不足，不能盘亏！".FormatWith(item.ProductCode));

            var quantity = 1;
            StoreInventoryHistory history = null;
            snCodes.ForEach(snCode =>
            {
                //无库存
                var inventoryBatch = _db.Table<StoreInventoryBatch>().FirstOrDefault(i => i.StoreId == entity.StoreId && i.ProductId == item.ProductId && i.SNCode == snCode && i.Quantity > 0);
                Ensure.NotNull(inventoryBatch, "商品{0}的盘亏串码{1}在系统中并无库存，请核实串码！".FormatWith(item.ProductCode, snCode));

                //扣减批次库存
                history = new StoreInventoryHistory(inventoryBatch.Id, item.ProductId, inventoryBatch.StoreId, inventory.Quantity, -quantity, inventoryBatch.Price, inventoryBatch.BatchNo, entity.Id, entity.Code, BillIdentity.StoreStocktakingPlan, currentAccount.AccountId, DateTime.Now, inventoryBatch.SupplierId, 0, inventoryBatch.SNCode);
                inventoryBatch.Quantity -= quantity;
                _db.Update(inventoryBatch);
                _db.Insert(history);
            });

            //增加总库存
            inventory.Quantity -= snCodes.Count;
            _db.Update(inventory);
        }

        //非串码盘盈
        private void InventorySurplusWithNoSNCode(StocktakingPlan entity, StocktakingPlanItem item, StoreInventory inventory, AccountInfo currentAccount)
        {
            var differenceQuantity = item.ComplexQuantity - item.Quantity;
            Ensure.GreaterThan(differenceQuantity, 0, "商品{0}复盘数量必须大于库存数量才能盘盈！".FormatWith(item.ProductCode));

            //取成本价
            var now = DateTime.Now;
            var purchasePrice = _db.Table<ProductPurchasePrice>().FirstOrDefault(p => p.ProductId == item.ProductId && p.Status == 1 && p.StartTime <= now && p.EndTime >= now);
            if (purchasePrice == null)
                purchasePrice = _db.Table<ProductPurchasePrice>().OrderByDesc(p => p.EndTime).FirstOrDefault(p => p.ProductId == item.ProductId && p.Status == 1);//当前时间无采购价，取最近的一次采购价
            if (purchasePrice == null)
            {
                var lastBatch = _db.Table<StoreInventoryBatch>().OrderByDesc(b => b.CreatedOn).FirstOrDefault(b => b.StoreId == entity.StoreId && b.ProductId == item.ProductId);
                if (lastBatch == null)
                    lastBatch = _db.Table<StoreInventoryBatch>().OrderByDesc(b => b.CreatedOn).FirstOrDefault(b => b.ProductId == item.ProductId);
                Ensure.NotNull(lastBatch, "在系统中找不到盘盈商品{0}的采购成本以及任何入库记录，无法新建库存批次！".FormatWith(item.ProductCode));

                purchasePrice = new ProductPurchasePrice()
                {
                    SupplierId = lastBatch.SupplierId,
                    PurchasePrice = lastBatch.Price,
                };
            }
            //Ensure.NotNull(purchasePrice, "在系统中找不到盘盈商品{0}的采购成本，无法新建库存批次！".FormatWith(item.ProductCode));

            //新建库存批次
            var batchNo = long.Parse(_billService.GenerateNewCode(BillIdentity.BatchNo));
            var inventoryBatch = new StoreInventoryBatch(item.ProductId, entity.StoreId, purchasePrice.SupplierId, differenceQuantity, purchasePrice.PurchasePrice, purchasePrice.PurchasePrice, batchNo, null, 0, currentAccount.AccountId);
            var history = new StoreInventoryHistory(0, item.ProductId, inventoryBatch.StoreId, inventory.Quantity, differenceQuantity, inventoryBatch.Price, inventoryBatch.BatchNo, entity.Id, entity.Code, BillIdentity.StoreStocktakingPlan, currentAccount.AccountId, DateTime.Now, inventoryBatch.SupplierId);
            inventoryBatch.History.Add(history);
            _db.Insert(inventoryBatch);

            //增加总库存
            this.AddStoreInventory(inventory, differenceQuantity, purchasePrice.PurchasePrice);
        }

        //非串码盘亏
        private void InventoryLossWithNoSNCode(StocktakingPlan entity, StocktakingPlanItem item, StoreInventory inventory, AccountInfo currentAccount)
        {
            var differenceQuantity = item.Quantity - item.ComplexQuantity;
            Ensure.GreaterThan(differenceQuantity, 0, "商品{0}复盘数量必须小于库存数量才能盘亏！".FormatWith(item.ProductCode));
            Ensure.SmallerOrEqualThan(differenceQuantity, inventory.Quantity, "商品{0}盘亏数量必须≤库存总量！".FormatWith(item.ProductCode));

            var productBatchs = _db.Table<StoreInventoryBatch>().Where(i => i.StoreId == entity.StoreId && i.ProductId == item.ProductId && i.Quantity > 0).OrderBy(i => i.CreatedOn).ToList();//按批次时间先进先出
            Ensure.SmallerOrEqualThan(differenceQuantity, productBatchs.Sum(i => i.Quantity), "商品{0}盘亏数量必须≤批次库存总量！".FormatWith(item.ProductCode));

            var leftQty = differenceQuantity;
            foreach (var inventoryBatch in productBatchs)
            {
                if (leftQty == 0) break;

                //扣减库存量
                var reduceQty = Math.Min(leftQty, inventoryBatch.Quantity);

                //扣减批次库存
                inventoryBatch.Quantity -= reduceQty;

                _db.Insert(new StoreInventoryHistory(inventoryBatch.Id, inventory.ProductId, entity.StoreId, inventory.Quantity, -reduceQty,
                        inventoryBatch.Price, inventoryBatch.BatchNo, entity.Id, entity.Code, BillIdentity.StoreStocktakingPlan, currentAccount.AccountId, DateTime.Now, inventoryBatch.SupplierId));

                leftQty -= reduceQty;

                _db.Update(inventoryBatch);
            }

            //扣减总库存
            inventory.Quantity -= differenceQuantity;
            _db.Update(inventory);
        }

        //增加总库存(盘盈)
        private void AddStoreInventory(StoreInventory inventory, int quantity, decimal costPrice)
        {
            var avgCostPrice = _storeInventoryFacade.CalculatedAveragePrice(inventory.AvgCostPrice, inventory.Quantity, costPrice, quantity);
            inventory.Quantity += quantity;//增加总库存
            inventory.AvgCostPrice = avgCostPrice;
            _db.Update(inventory);
        }

        //检查库存变动
        private void CheckInventoryChange(StocktakingPlan entity)
        {
            var hasInventoryChange = _db.Table<StoreInventoryHistory>().Exists(h => h.StoreId == entity.StoreId && h.CreatedOn >= entity.CreatedOn);
            if (hasInventoryChange)
            {
                entity.Status = StocktakingPlanStatus.Canceled;
                _db.SaveChange();
                throw new FriendlyException("盘点期间发生过库存变动，请重新导出盘点！");
            }
        }

        #endregion
    }
}
