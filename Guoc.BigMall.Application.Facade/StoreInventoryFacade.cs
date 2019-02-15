using Guoc.BigMall.Application.ViewObject;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.ValueObject;
using Guoc.BigMall.Domain.Service;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Infrastructure;
using System.Linq.Expressions;
using Guoc.BigMall.Infrastructure.Utils;
using Guoc.BigMall.Application.Search;

namespace Guoc.BigMall.Application.Facade
{
    public class StoreInventoryFacade : IStoreInventoryFacade
    {
        IDBContext _db;
        BillSequenceService _sequenceService;
        public StoreInventoryFacade(IDBContext dbContext, BillSequenceService sequenceService)
        {
            _db = dbContext;
            _sequenceService = sequenceService;
        }
        public IEnumerable<StoreInventoryQueryDto> GetPageList(Pager page, Search.SearchStoreInventory condition)
        {
            dynamic param = new ExpandoObject();
            string where = "";
            string orderby = "t.Id DESC";

            if (!string.IsNullOrEmpty(condition.StoreId) && condition.StoreId != "0")
            {
                where += "and t0.StoreId in @StoreId ";
                param.StoreId = condition.StoreId.Split(',').ToIntArray(); ;
            }
            if (!string.IsNullOrEmpty(condition.ProductCodeOrBarCode))
            {
                where += "and (t1.Code=@ProductCodeOrBarCode ) ";
                param.ProductCodeOrBarCode = condition.ProductCodeOrBarCode;
            }

            if (!string.IsNullOrEmpty(condition.ProductName))
            {
                where += "and t1.Name like @ProductName ";
                param.ProductName = string.Format("%{0}%", condition.ProductName);
            }
            if (!string.IsNullOrEmpty(condition.Order))
            {
                if (condition.Order.Equals("Quantity"))
                {
                    orderby += "t.Quantity desc";
                }
                else if (condition.Order.Equals("OccupyQuantity"))
                {
                    orderby += "t.OccupyQuantity desc";
                }
                else if (condition.Order.Equals("UsableQuantity"))
                {
                    orderby += "t.UsableQuantity desc";
                }
            }
            if (!string.IsNullOrEmpty(condition.BrandId))
            {
                where += "and t3.Id in  @BrandId ";
                param.BrandId = condition.BrandId.Split(',').ToIntArray();
            }
            if (!string.IsNullOrEmpty(condition.CategoryId))
            {
                where += "and t4.Id=@CategoryId ";
                param.CategoryId = condition.CategoryId;
            }
            if (!string.IsNullOrEmpty(condition.Mark) && !string.IsNullOrEmpty(condition.StoreInventoryQuantity))
            {
                where += "and t0.Quantity " + condition.Mark + " @Quantity ";
                param.Quantity = condition.StoreInventoryQuantity;
            }
            if (!string.IsNullOrEmpty(condition.ProductCodeIds))
            {
                where += " and t1.Id in  @ProductCodeIds";
                param.ProductCodeIds = condition.ProductCodeIds.Split(',').ToArray();
            }

            var dataSql = @"
            select * from (
                SELECT  t0.* ,
                        t1.Code AS ProductCode ,
                        t1.Name AS ProductName ,
                        t1.Spec AS Specification ,
                        ( SELECT TOP 1
                                    SalePrice
                            FROM      ProductPrice sp
                            WHERE     sp.ProductId = t1.Id
                                    AND sp.PriceType = 1
                                    AND sp.StartTime <= GETDATE()
                                    AND sp.EndTime >= GETDATE()
                                    AND ( sp.StoreId = 0
                                            OR sp.StoreId = t0.StoreId
                                        )
                            ORDER BY  sp.Slevel DESC ,
                                    sp.Id DESC
                        ) AS SalePrice ,
                        t2.Name AS StoreName ,
                        t0.Quantity - t0.LockedQuantity AS UsableQuantity ,
                        t0.LockedQuantity AS OccupyQuantity ,
                        t3.Name AS BrandName ,
                        t4.FullName AS CategoryName 
                FROM    StoreInventory t0
                        INNER JOIN Product t1 ON t0.ProductId = t1.Id
                        INNER JOIN Store t2 ON t2.Id = t0.StoreId
                        INNER JOIN Brand t3 ON t3.Id = t1.BrandId
                        INNER JOIN Category t4 ON t4.Id = t1.CategoryId
                where 1=1 {0} ) t ORDER BY {1}".FormatWith(where, orderby);

            if (page.IsPaging == false)
                return this._db.DataBase.Query<StoreInventoryQueryDto>(dataSql, param);

            var pageSql = "{0} OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY".FormatWith(dataSql, (page.PageIndex - 1) * page.PageSize, page.PageSize);
            var rows = this._db.DataBase.Query<StoreInventoryQueryDto>(pageSql, param);

            string sqlcount = string.Format(@" select  count(1)
                                               from StoreInventory t0
                                                INNER JOIN Product t1 ON t0.ProductId = t1.Id
                                                INNER JOIN Store t2 ON t2.Id = t0.StoreId
                                                INNER JOIN Brand t3 ON t3.Id = t1.BrandId
                                                INNER JOIN Category t4 ON t4.Id = t1.CategoryId
                                        where 1=1 {0}", where);
            page.Total = _db.DataBase.ExecuteScalar<int>(sqlcount, param);
            return rows;
        }

        public IEnumerable<StoreInventoryHistoryQueryDto> GetPageHistoryList(Pager page, Search.SearchStoreInventoryHistory condition)
        {

            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrEmpty(condition.StoreId) && condition.StoreId != "0")
            {
                where += "and t0.StoreId in @StoreId ";
                param.StoreId = condition.StoreId.Split(',').ToIntArray(); ;
            }

            if (!string.IsNullOrEmpty(condition.BillCode))
            {
                where += "and t0.BillCode=@BillCode ";
                param.BillCode = condition.BillCode;
            }
            if (!string.IsNullOrEmpty(condition.BatchNo))
            {
                where += "and t0.BatchNo=@BatchNo ";
                param.BatchNo = condition.BatchNo;
            }
            if (condition.BillType > 0)
            {
                where += "and t0.BillType=@BillType ";
                param.BillType = condition.BillType;
            }

            if (!string.IsNullOrEmpty(condition.ProductCodeOrBarCode))
            {
                where += "and t1.Code=@ProductCodeOrBarCode  ";
                param.ProductCodeOrBarCode = condition.ProductCodeOrBarCode;
            }
            if (!string.IsNullOrEmpty(condition.ProductName))
            {
                where += "and t1.Name like @Name ";
                param.Name = string.Format("%{0}%", condition.ProductName);
            }
            if (!string.IsNullOrEmpty(condition.SNCode))
            {
                where += "and t0.SNCode like @SNCode  ";
                param.SNCode = string.Format("%{0}%", condition.SNCode);
            }
            if (!string.IsNullOrWhiteSpace(condition.Time))
            {
                var times = condition.Time.Split(',');
                where += "and t0.CreatedOn >=@StartDate  and t0.CreatedOn < @EndDate";
                param.StartDate = Convert.ToDateTime(times[0]);
                param.EndDate = Convert.ToDateTime(times[1]).AddDays(1);
            }
            string sql = @"select * from ( select  ROW_NUMBER()  OVER(ORDER BY t0.CreatedOn desc ) AS rownum, t0.*,t1.Code as ProductCode ,t1.Name as ProductName,t1.Spec as Specification,t2.name as StoreName
from storeinventoryhistory t0 left join product t1 on t0.productId = t1.Id
inner join store t2 on t2.Id = t0.StoreId where 1=1 {0} ) t
where rownum between {1} and {2} ";
            sql = string.Format(sql, where, (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize);
            var rows = this._db.DataBase.Query<StoreInventoryHistoryQueryDto>(sql, param);

            string sqlCount = @"select count(*) 
 from storeinventoryhistory t0 left join product t1 on t0.productId = t1.Id
inner join store t2 on t2.Id = t0.StoreId where 1=1 {0}";
            sqlCount = string.Format(sqlCount, where);
            page.Total = this._db.DataBase.ExecuteScalar<int>(sqlCount, param);

            return rows;
        }

        public IEnumerable<StoreInventoryBatchQueryDto> GetPageBatchList(Pager page, Search.SearchStoreInventoryBatch condition)
        {
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrWhiteSpace(condition.SupplierId))
            {
                where += "and t0.SupplierId in  @SupplierId ";
                param.SupplierId = condition.SupplierId.Split(',').ToIntArray();
            }
            if (!string.IsNullOrEmpty(condition.BatchNo))
            {
                where += "and t0.BatchNo=@BatchNo ";
                param.BatchNo = condition.BatchNo;
            }

            if (!string.IsNullOrEmpty(condition.StoreId) && condition.StoreId != "0")
            {
                where += "and t0.StoreId in @StoreId ";
                param.StoreId = condition.StoreId.Split(',').ToIntArray(); ;
            }

            if (!string.IsNullOrEmpty(condition.ProductCodeOrBarCode))
            {
                where += "and t1.Code=@ProductCodeOrBarCode ";
                param.ProductCodeOrBarCode = condition.ProductCodeOrBarCode;
            }
            if (!string.IsNullOrEmpty(condition.ProductName))
            {
                where += "and t1.Name like @Name ";
                param.Name = string.Format("%{0}%", condition.ProductName);
            }
            if (!string.IsNullOrEmpty(condition.SNCode))
            {
                where += "and t0.SNCode like @Name ";
                param.Name = string.Format("%{0}%", condition.SNCode);
            }
            string sql = @"select * from ( select  ROW_NUMBER()  OVER(ORDER BY t0.Id desc ) AS rownum, t0.*,t1.Code as ProductCode ,t1.Name as ProductName,t1.Spec Specification,t2.name as StoreName,t3.Name as SupplierName
from storeinventorybatch t0 left join product t1 on t0.productId = t1.Id
inner join store t2 on t2.Id = t0.StoreId
left join supplier t3 on t3.Id = t0.SupplierId where 1=1 {0}) t
where rownum between {1} and {2} ";

            sql = string.Format(sql, where, (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize);
            var rows = this._db.DataBase.Query<StoreInventoryBatchQueryDto>(sql, param);

            string sqlCount = @"select count(*) 
from storeinventorybatch t0 left join product t1 on t0.productId = t1.Id
inner join store t2 on t2.Id = t0.StoreId
left join supplier t3 on t3.Id = t0.SupplierId where 1=1 {0}";
            sqlCount = string.Format(sqlCount, where);
            page.Total = this._db.DataBase.ExecuteScalar<int>(sqlCount, param);

            return rows;
        }

        //public void StockIn(PurchaseOrder entity)
        //{
        //    if (entity == null) { throw new Exception("单据不存在"); }
        //    if (entity.Items.Count() == 0) { throw new Exception("单据明细为空"); }

        //    //记录库存批次
        //    var productIdArray = entity.Items.Select(n => n.ProductId).ToArray();
        //    var inventorys = _db.DataBase.Query<StoreInventory>("select * from storeinventory where productId in @ProductIds and StoreId=@StoreId", new { ProductIds = productIdArray, StoreId = entity.StoreId });
        //    var inventoryBatchs = new List<StoreInventoryBatch>();
        //    var inventoryHistorys = new List<StoreInventoryHistory>();
        //    //var storeInventoryBatchSNCodes = new List<StoreInventoryBatchSNCode>();
        //    var batchNo = long.Parse(_sequenceService.GenerateNewCode(BillIdentity.BatchNo));
        //   // PurchaseOrderReceive asn = new PurchaseOrderReceive(){ Code=long.Parse(_sequenceService.GenerateNewCode(BillIdentity.AsnOrder)), CreateOn =DateTime.Now,PurchaseOrderId=entity.Id, CreatedBy=entity.UpdatedBy};
        //    var asnItems = new List<PurchaseOrderReceiveItem>();
        //    foreach (var item in entity.Items)
        //    {
        //        if (item.SNQuantity == 0)
        //            continue;

        //        var inventory = inventorys.FirstOrDefault(n => n.ProductId == item.ProductId);
        //        if (inventory == null)
        //            throw new Exception(string.Format("商品{0}不存在", item.ProductId));

        //        // 回写采购入库批次
        //        item.BatchNo = batchNo;
        //        //item.RefundableQuantity = item.ActualQuantity; // 入库后，可退数默认为实收数

        //        // 入库      
        //        var inventoryQuantity = inventory.Quantity;
        //        inventory.Quantity += item.SNQuantity;//item.ActualQuantity;
        //        inventory.SaleQuantity += item.SNQuantity;// item.ActualQuantity;
        //        inventory.AvgCostPrice = CalculatedAveragePrice(inventory.AvgCostPrice, inventoryQuantity, item.CostPrice, item.SNQuantity);  // 修改库存均价
        //        inventory.LastCostPrice = item.CostPrice > 0 ? item.CostPrice : inventory.LastCostPrice;
        //        //记录库存流水
        //        var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, inventoryQuantity, item.SNQuantity,
        //            item.CostPrice, batchNo, entity.Id, entity.Code, BillIdentity.StockPurchaseOrder, entity.UpdatedBy, entity.SupplierId,item.SNCodes);
        //        inventoryHistorys.Add(history);


        //        //记录库存批次
        //        if (!item.IsSnCode) //串码商品
        //        {
        //            //var batchQuantity = CalculatedBatchQuantity(inventoryQuantity, item.SNQuantity);
        //            var batch = new StoreInventoryBatch(item.ProductId, entity.StoreId, entity.SupplierId, item.SNQuantity,
        //                item.CostPrice, item.CostPrice, batchNo, null, 0, entity.UpdatedBy,"");
        //            batch.PurchaseQuantity = item.SNQuantity;
        //            inventoryBatchs.Add(batch);
        //        }
        //        else     ///记录采购批次串码
        //        {
        //            if (string.IsNullOrWhiteSpace(item.SNCodes.Replace('|',' ')))
        //            {
        //                continue;
        //            }
        //            var sncodes = item.SNCodes.Split('|');
        //            foreach (var code in sncodes)
        //            {
        //                if (string.IsNullOrWhiteSpace(code))
        //                    continue;
        //                var batch = new StoreInventoryBatch(item.ProductId, entity.StoreId, entity.SupplierId, 1,
        //                item.CostPrice, item.CostPrice, batchNo, null, 0, entity.UpdatedBy,code);
        //                batch.PurchaseQuantity = 1;
        //                inventoryBatchs.Add(batch);

        //            }
        //        }
        //        //采购收货单
        //        asnItems.Add(new PurchaseOrderReceiveItem()
        //        {
        //            Price = item.CostPrice,
        //            ProductId = item.ProductId,
        //            PurchaseOrderItemId = item.Id,
        //            PurchaseQuality = item.SNQuantity,
        //            Quality = item.Quantity
        //        });

        //    }
        //    asn.SetItems(asnItems);

        //    _db.Update(entity.Items.ToArray());
        //    _db.Update(inventorys.ToArray());
        //    _db.Insert(inventoryHistorys.ToArray());
        //    _db.Insert(inventoryBatchs.ToArray());
        //    _db.Insert(asn);

        //    //_db.Insert(storeInventoryBatchSNCodes.ToArray());
        //}
        /// <summary>
        /// 计算批次入库数量。批次入库需考虑库存为负时，抵扣负库存情况
        /// </summary>
        /// <param name="inventoryQuantity">商品当前总库存数</param>
        /// <param name="quantity">入库数</param>
        /// <returns></returns>
        private int CalculatedBatchQuantity(int inventoryQuantity, int quantity)
        {
            var batchQuantity = quantity;
            if (inventoryQuantity < 0)
            {
                batchQuantity = inventoryQuantity + quantity;
                batchQuantity = batchQuantity > 0 ? batchQuantity : 0;
            }
            return batchQuantity;
        }

        public void JudgeSnCodes(List<PurchaseOrderItem> receiveList)
        {

            foreach (var item in receiveList)
            {

                if (string.IsNullOrWhiteSpace(item.SNCodes) && item.SNQuantity == 0)//传入的条码/串码 为空 表示未收货
                {
                    continue;
                }

                //if (_db.Table<StoreInventoryBatch>().Exists(n => n.ProductId == item.ProductId))
                //{
                //    var count = _db.DataBase.ExecuteScalar<int>("select  COUNT(1) from StoreInventoryBatch where productId=@ProductId  AND SNCode IS  NOT NULL AND SNCode!=''", new { ProductId = item.ProductId });
                //    var result = count > 0 ? true : false;//是否串码
                //    if (item.IsSnCode != result)
                //    {
                //        throw new FriendlyException(string.Format("你所选择的商品{0} “是或串码”与数据不符合，请确认后操作！", item.ProductCode));
                //    }
                //}

                if (item.IsSnCode)//串码
                {
                    if (string.IsNullOrWhiteSpace(item.SNCodes))
                    {
                        throw new FriendlyException(string.Format("商品:{0}选择是串码商品，但未扫入串码，请确认后操作！", item.ProductCode));
                    }
                    var sncodes = item.SNCodes.Split(',').ToList(); //得到 串码/编码
                    var counts = sncodes.Distinct().Count();  //不重复的串码数量
                    if (counts != item.SNQuantity)            //串码的数量 !=收货数量报错（收货的数量==串码数量）
                        throw new FriendlyException(string.Format("商品:{0}的扫码[{1}]有误，请确认后操作！", item.ProductCode, item.SNCodes));


                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(item.SNCodes))
                    {
                        throw new FriendlyException(string.Format("商品:{0}选择非串码商品，但扫入了串码，请确认后操作！", item.ProductCode));
                    }
                }

            }
        }

        public bool LockProductInventory(int storeId, int productId, int qty)
        {
            try
            {
                Expression<Func<StoreInventory, bool>> where = x => x.StoreId == storeId && x.ProductId == productId;

                Expression<Func<StoreInventory, StoreInventory>> columns = x => new StoreInventory()
                {
                    LockedQuantity = x.LockedQuantity + qty,
                    Quantity = x.Quantity - qty
                };

                _db.Update(columns, where);

                _db.SaveChange();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public StoreInventory GetInventory(int storeId, int productId)
        {
            var storeInventory = _db.Table<StoreInventory>().FirstOrDefault(o => o.ProductId == productId && o.StoreId == storeId);

            return storeInventory;
        }

        public List<StoreInventory> GetInventory(int storeId, IEnumerable<int> productIds)
        {
            var sql = "SELECT * FROM StoreInventory WHERE StoreId = @StoreId AND ProductId IN @ProductIds";

            var storeInventorires = _db.Table<StoreInventory>().Query(sql, new { StoreId = storeId, ProductIds = productIds.ToArray() });

            return storeInventorires.ToList();
        }

        public StoreInventoryBatch GetInventoryBatch(int storeId, string snCode, int productId)
        {
            var storeInventoryBatch = _db.Table<StoreInventoryBatch>().FirstOrDefault(o => o.StoreId == storeId && o.SNCode == snCode && o.ProductId == productId && o.Quantity > 0);

            return storeInventoryBatch;
        }

        public List<StoreInventoryBatch> GetInventoryBatch(int storeId, IEnumerable<int> productIds)
        {
            var sql = "SELECT * FROM StoreInventoryBatch WHERE StoreId = @StoreId AND ProductId IN @ProductIds AND Quantity > 0";

            var data = _db.Table<StoreInventoryBatch>().Query(sql, new { StoreId = storeId, ProductIds = productIds.ToArray() });

            return data.ToList();
        }

        public List<StoreInventoryBatch> GetInventoryBatch(int storeId, IEnumerable<string> snCodes, IEnumerable<int> productIds)
        {
            var sql = "SELECT * FROM StoreInventoryBatch WHERE StoreId = @StoreId AND ProductId IN @ProductIds AND SNCode IN @SNCodes AND Quantity > 0";

            var data = _db.Table<StoreInventoryBatch>().Query(sql, new { StoreId = storeId, ProductIds = productIds.ToArray(), SNCodes = snCodes.ToArray() });

            return data.ToList();
        }

        /// <summary>
        /// 查询商品出库批次。
        /// </summary>
        public List<StoreInventoryBatchDto> GetStockOutInventoryBatch(int storeId, int billId, BillIdentity billType, int productId)
        {
            var sql = @"
                SELECT  b.*,
                        h.ChangeQuantity
                FROM    StoreInventoryBatch b ,
                        StoreInventoryHistory h
                WHERE   b.StoreId = h.StoreId
                        AND b.ProductId = h.ProductId
                        AND b.BatchNo = h.BatchNo
						AND (b.SNCode IS NULL OR b.SNCode=h.SNCode)
                        AND h.StoreId = @StoreId
                        AND h.BillId = @BillId
                        AND h.BillType = @BillType
                        AND h.ProductId = @ProductId
                        AND h.ChangeQuantity < 0";
            return _db.DataBase.Query<StoreInventoryBatchDto>(sql, new { StoreId = storeId, BillId = billId, BillType = billType, ProductId = productId }).ToList();
        }

        /// <summary>
        /// 查询商品在门店最近的一条批次库存。
        /// </summary>
        public StoreInventoryBatch GetLastInventoryBatchWithNoSNCode(int storeId, int productId)
        {
            var sql = @"
            SELECT TOP 1
                    *
            FROM    StoreInventoryBatch
            WHERE   StoreId = @StoreId
                    AND ProductId = @ProductId
                    AND SNCode IS NULL
            ORDER BY CreatedOn DESC";

            var storeInventoryBatch = _db.DataBase.QuerySingle<StoreInventoryBatch>(sql, new { StoreId = storeId, ProductId = productId });
            return storeInventoryBatch;
        }

        public StoreInventoryHistory GetHistoryWithSNCode(int storeId, int billId, BillIdentity billType, int productId, string snCode)
        {
            return _db.Table<StoreInventoryHistory>().FirstOrDefault(h => h.StoreId == storeId && h.BillId == billId && h.BillType == billType && h.ProductId == productId && h.SNCode == snCode);
        }

        public void JudgeSnCodeProduct(List<PurchaseOrderItem> receiveList)
        {
            foreach (var item in receiveList)
            {
                if (string.IsNullOrWhiteSpace(item.SNCodes) && item.SNQuantity == 0)

                    throw new FriendlyException("未检测到商品！");

            }

            this.JudgeSnCodes(receiveList);
        }


        #region 统一出库

        public StoreInventoryResult OutStock(IDBContext dbContext, StockOutModel model)
        {
            Ensure.NotNull(model, "出库数据为空。");
            Ensure.NotNullOrEmpty(model.Items, "出库明细为空。");
            Ensure.False(model.Items.GroupBy(g => g.ProductId).Any(g => g.Count() > 1), "出库明细中包含重复的商品。");

            //1.减批次库存
            var result = this.DecreaseStoreInventoryBatch(dbContext, model);

            //2.减总库存
            this.DecreaseStoreInventory(dbContext, model);

            //3.发数据到SAP
            this.SaveStoreInventoryHistorySap(dbContext, result);

            return result;
        }

        //1.减批次库存
        private StoreInventoryResult DecreaseStoreInventoryBatch(IDBContext dbContext, StockOutModel model)
        {
            var store = dbContext.Table<Store>().FirstOrDefault(s => s.Id == model.StoreId);
            Ensure.NotNull(store, "门店不存在。");
            Ensure.False(model.Items.GroupBy(m => m.ProductId).Any(g => g.Count() > 1), "出库明细重复。");

            var result = new StoreInventoryResult()
            {
                StoreId = model.StoreId,
                StoreCode = store.Code,
                BillId = model.BillId,
                BillCode = model.BillCode,
                BillType = model.BillType,
                CreatedBy = model.CreatedBy,
                CreatedOn = model.CreatedOn,
                StoreInventoryChangeType = StoreInventoryHistorySapType.OutStock,
            };

            StoreInventoryItemResult itemResult = null;
            model.Items.ForEach(item =>
            {
                var product = dbContext.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                Ensure.NotNull(product, "出库的商品不存在。");
                Ensure.GreaterThan(item.Quantity, 0, "商品{0}出库数量必须大于零。".FormatWith(product.Code));

                if (item.SNCodes.Count > 0)
                    itemResult = this.DecreaseStoreInventoryBatchWithSNCode(dbContext, model, item, product);
                else
                    itemResult = this.DecreaseStoreInventoryBatchWithNoSNCode(dbContext, model, item, product);
                result.Items.Add(itemResult);
            });
            return result;
        }

        //1.1.减批次库存-有串码，根据串码减相应的库存
        private StoreInventoryItemResult DecreaseStoreInventoryBatchWithSNCode(IDBContext dbContext, StockOutModel model, StockOutItemModel item, Product product)
        {
            Ensure.True(product.HasSNCode, "非串码商品{0}不应该使用串码出库。".FormatWith(product.Code));
            Ensure.False(item.SNCodes.GroupBy(s => s.SNCode).Any(g => g.Count() > 1), "出库明细包含重复的串码。");
            Ensure.EqualThan(item.Quantity, item.SNCodes.Count, "商品{0}出库数量与串码个数不一致。".FormatWith(product.Code));

            var result = new StoreInventoryItemResult(item.ProductId, product.Code);
            item.SNCodes.ForEach(sn =>
            {
                var quantity = 1;
                Ensure.NotNullOrEmpty(sn.SNCode, "出库明细包含为空的串码。");

                var inventory = this.GetInventory(model.StoreId, item.ProductId);
                Ensure.NotNull(inventory, "商品{0}总库存不存在。".FormatWith(product.Code));

                var inventoryBatch = dbContext.Table<StoreInventoryBatch>().FirstOrDefault(x => x.StoreId == model.StoreId && x.ProductId == item.ProductId && x.SNCode == sn.SNCode && x.Quantity > 0);
                Ensure.NotNull(inventoryBatch, "商品{0}批次库存不存在。".FormatWith(product.Code));
                Ensure.GreaterOrEqualThan(inventoryBatch.Quantity, quantity, "商品{0}批次库存不足。".FormatWith(product.Code));

                //记录修改历史
                dbContext.Insert(new StoreInventoryHistory(inventoryBatch.Id, item.ProductId, model.StoreId, inventory.Quantity, -quantity,
                    inventoryBatch.Price, inventoryBatch.BatchNo, model.BillId, model.BillCode, model.BillType, model.CreatedBy, model.CreatedOn, inventoryBatch.SupplierId, sn.SalePrice, inventoryBatch.SNCode, item.CategoryPreferential, item.BrandPreferential));

                inventoryBatch.Quantity -= quantity;

                dbContext.Update(inventoryBatch);

                result.BatchNos.Add(new ChangedBatch(inventoryBatch.BatchNo, quantity));
                result.SNCodes.Add(sn.SNCode);
            });
            return result;
        }

        //1.2.减批次库存-没有串码，减最早入库的商品库存
        private StoreInventoryItemResult DecreaseStoreInventoryBatchWithNoSNCode(IDBContext dbContext, StockOutModel model, StockOutItemModel item, Product product)
        {
            Ensure.False(product.HasSNCode, "串码商品{0}必须使用串码出库。".FormatWith(product.Code));
            Ensure.GreaterThan(item.Quantity, 0, "商品{0}出库数量必须大于零。".FormatWith(product.Code));

            var inventory = this.GetInventory(model.StoreId, item.ProductId);
            Ensure.NotNull(inventory, "商品{0}总库存不存在。".FormatWith(product.Code));

            var productBatchs = dbContext.Table<StoreInventoryBatch>()
                                   .Where(x => x.StoreId == model.StoreId && x.ProductId == item.ProductId && x.Quantity > 0)
                                   .OrderBy(x => x.CreatedOn)
                                   .ToList();

            var leftQty = item.Quantity;
            Ensure.GreaterOrEqualThan(productBatchs.Sum(x => x.Quantity), leftQty, "商品{0}批次库存不足。".FormatWith(product.Code));

            var categoryPreferential = item.CategoryPreferential / item.Quantity;//单品品类优惠额度
            var brandPreferential = item.BrandPreferential / item.Quantity;//单品品牌优惠额度
            var result = new StoreInventoryItemResult(item.ProductId, product.Code);
            foreach (var batchItem in productBatchs)
            {
                if (leftQty == 0) break;

                //扣减库存
                var reduceQty = Math.Min(leftQty, batchItem.Quantity);

                batchItem.Quantity -= reduceQty;

                var totalCategoryPreferential = Math.Round(categoryPreferential * reduceQty, 2);//当前批次总的品类优惠金额
                var totalBrandPreferential = Math.Round(brandPreferential * reduceQty, 2);//当前批次总的品牌优惠金额

                dbContext.Insert(new StoreInventoryHistory(batchItem.Id, inventory.ProductId, model.StoreId, inventory.Quantity, -reduceQty,
                        batchItem.Price, batchItem.BatchNo, model.BillId, model.BillCode, model.BillType, model.CreatedBy, model.CreatedOn, batchItem.SupplierId, item.SalePrice, batchItem.SNCode, totalCategoryPreferential, totalBrandPreferential));

                leftQty -= reduceQty;

                dbContext.Update(batchItem);

                result.BatchNos.Add(new ChangedBatch(batchItem.BatchNo, reduceQty));
                if (batchItem.SNCode.NotNullOrEmpty())
                    result.SNCodes.Add(batchItem.SNCode);
            }
            return result;
        }

        //3.减总库存
        private void DecreaseStoreInventory(IDBContext dbContext, StockOutModel model)
        {
            model.Items.ForEach(item =>
            {
                var product = dbContext.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                Ensure.NotNull(product, "出库的商品不存在。");

                var inventory = this.GetInventory(model.StoreId, item.ProductId);
                Ensure.NotNull(inventory, "商品{0}总库存不存在。".FormatWith(product.Code));

                Ensure.GreaterOrEqualThan(inventory.Quantity, item.Quantity, "商品{0}总库存不足。".FormatWith(product.Code));

                inventory.Quantity -= item.Quantity;

                dbContext.Update(inventory);
            });
        }

        //4.发给SAP的数据  [dbo].[StoreInventoryHistorySAP]
        public void SaveStoreInventoryHistorySap(IDBContext dbContext, StoreInventoryResult storeInventoryChangedResult, string historyCode = null)
        {
            historyCode = historyCode ?? _sequenceService.GenerateNewCode(BillIdentity.SapHistoryOrder);
            storeInventoryChangedResult.HistoryCode = historyCode;

            storeInventoryChangedResult.Items.GroupBy(g => g.ProductId).ToList().ForEach(g =>
            {
                var item = g.First();
                var storeInventoryHistorySAP = new StoreInventoryHistorySAP()
                {
                    Code = historyCode,
                    Type = storeInventoryChangedResult.StoreInventoryChangeType,
                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    StoreId = storeInventoryChangedResult.StoreId,
                    StoreCode = storeInventoryChangedResult.StoreCode,
                    Quantity = g.Sum(m => m.Quantity),
                    SNCodes = item.SNCodes.IsNotEmpty() ? string.Join(",", g.SelectMany(m => m.SNCodes)) : null,
                    BillCode = storeInventoryChangedResult.BillCode,
                    BillType = storeInventoryChangedResult.BillType,
                    CreatedOn = storeInventoryChangedResult.CreatedOn,
                    CreatedBy = storeInventoryChangedResult.CreatedBy,
                };

                dbContext.Insert(storeInventoryHistorySAP);
            });
        }

        #endregion

        #region 统一入库

        public StoreInventoryResult InStock(IDBContext dbContext, StockInModel model, bool generateSapHistory = true)
        {
            Ensure.NotNull(model, "入库数据为空。");
            Ensure.NotNullOrEmpty(model.Items, "入库明细为空。");
            //Ensure.False(model.Items.GroupBy(g => g.ProductId).Any(g => g.Count() > 1), "入库明细中包含重复的商品。");
            Ensure.False(model.Items.Where(m => m.SNCode.NotNullOrEmpty()).GroupBy(g => new { g.ProductId, g.SNCode }).Any(g => g.Count() > 1), "入库明细中包含重复的串码商品。");
            var store = dbContext.Table<Store>().FirstOrDefault(s => s.Id == model.StoreId);
            Ensure.NotNull(store, "门店不存在。");

            List<StoreInventoryItemResult> itemResult;
            var result = new StoreInventoryResult()
            {
                StoreId = model.StoreId,
                StoreCode = store.Code,
                BillId = model.StockInBillId,
                BillCode = model.StockInBillCode,
                BillType = model.StockInBillType,
                CreatedBy = model.CreatedBy,
                CreatedOn = model.CreatedOn,
                StoreInventoryChangeType = StoreInventoryHistorySapType.InStock,
            };

            //1.增加批次库存
            if (model.InStockType == InStockType.Return)
                itemResult = this.ReturnInStock(dbContext, model); //退货入库
            else
                itemResult = this.NormalInStock(dbContext, model); //正常入库

            result.Items.AddRange(itemResult);

            //2.增加总库存
            this.AddStoreInventory(dbContext, model);

            //3.发数据到SAP
            if (generateSapHistory)
                this.SaveStoreInventoryHistorySap(dbContext, result);

            return result;
        }

        //1.1.退货入库
        private List<StoreInventoryItemResult> ReturnInStock(IDBContext dbContext, StockInModel model)
        {
            var result = new List<StoreInventoryItemResult>();

            model.Items.GroupBy(g => new { g.ProductId, g.SNCode }).ToList().ForEach(g =>
            {
                var item = g.First();
                var product = dbContext.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                Ensure.NotNull(product, "入库的商品不存在。");
                Ensure.GreaterThan(g.Sum(m => m.Quantity), 0, "商品{0}入库数量必须大于零。".FormatWith(product.Code));

                if (item.SNCode.NotNullOrEmpty())
                    result.Add(this.ReturnInStockWithSNCode(dbContext, model, item, product));
                else
                {
                    if (g.Count() > 1)
                    {
                        item = new StockInItemModel()
                        {
                            ProductId = item.ProductId,
                            Quantity = g.Sum(m => m.Quantity),
                        };
                    }
                    result.Add(this.ReturnInStockWithNoSNCode(dbContext, model, item, product));
                }
            });
            return result;
        }

        //1.1.1.串码商品，退货入库
        private StoreInventoryItemResult ReturnInStockWithSNCode(IDBContext dbContext, StockInModel model, StockInItemModel item, Product product)
        {
            Ensure.True(product.HasSNCode, "非串码商品{0}不能以串码方式入库。".FormatWith(product.Code));
            //Ensure.EqualThan(item.Quantity, item.SNCodes.Count, "商品{0}入库数量与串码个数不一致。".FormatWith(product.Code));
            Ensure.EqualThan(item.Quantity, 1, "串码{0}商品{1}入库数量必须为1。".FormatWith(item.SNCode, product.Code));

            var inventory = this.GetInventory(model.StoreId, item.ProductId);
            Ensure.NotNull(inventory, "商品{0}总库存不存在。".FormatWith(product.Code));

            var result = new StoreInventoryItemResult(item.ProductId, product.Code);

            //item.SNCodes.ForEach(sn =>
            //{
            //Ensure.NotNullOrEmpty(sn.SNCode, "入库商品{0}包含为空的串码。".FormatWith(product.Code));

            var existsInventoryBatch = _db.Table<StoreInventoryBatch>().FirstOrDefault(o => o.SNCode == item.SNCode && o.Quantity > 0);
            if (existsInventoryBatch != null)
            {
                Ensure.EqualThan(model.StoreId, existsInventoryBatch.StoreId, "串码{0}商品{1}在别的仓已存在库存，不能入库。".FormatWith(item.SNCode, product.Code));
                throw new FriendlyException("串码{0}商品{1}在本仓已存在库存，不能重复入库。".FormatWith(item.SNCode, product.Code));
            }

            var history = dbContext.Table<StoreInventoryHistory>().FirstOrDefault(h => h.StoreId == model.StoreId && h.ProductId == item.ProductId && h.SNCode == item.SNCode && h.BillId == model.StockOutBillId && h.BillType == model.StockOutBillType && h.ChangeQuantity < 0);
            Ensure.NotNull(history, "找不到串码{0}商品{1}的出库记录。".FormatWith(item.SNCode, product.Code));

            var inventoryBatch = dbContext.Table<StoreInventoryBatch>().FirstOrDefault(b => b.StoreId == model.StoreId && b.ProductId == item.ProductId && b.SNCode == item.SNCode && b.BatchNo == history.BatchNo);
            Ensure.NotNull(inventoryBatch, "找不到串码{0}商品{1}出库的库存批次。".FormatWith(item.SNCode, product.Code));

            var quantity = 1;
            history = new StoreInventoryHistory(inventoryBatch.Id, item.ProductId, model.StoreId, inventory.Quantity, quantity, inventoryBatch.Price, inventoryBatch.BatchNo, model.StockInBillId, model.StockInBillCode, model.StockInBillType, model.CreatedBy, model.CreatedOn, inventoryBatch.SupplierId, 0, item.SNCode, item.CategoryPreferential, item.BrandPreferential);

            inventoryBatch.Quantity = quantity;

            dbContext.Update(inventoryBatch);
            dbContext.Insert(history);//记录库存历史

            result.SNCodes.Add(item.SNCode);
            result.BatchNos.Add(new ChangedBatch(inventoryBatch.BatchNo, quantity));
            //});
            return result;
        }

        //1.1.2.非串码商品，退货入库
        private StoreInventoryItemResult ReturnInStockWithNoSNCode(IDBContext dbContext, StockInModel model, StockInItemModel item, Product product)
        {
            Ensure.False(product.HasSNCode, "串码商品{0}只能以串码方式入库。".FormatWith(product.Code));

            var inventory = this.GetInventory(model.StoreId, item.ProductId);
            Ensure.NotNull(inventory, "商品{0}总库存不存在。".FormatWith(product.Code));

            var histories = dbContext.Table<StoreInventoryHistory>().Where(h => h.StoreId == model.StoreId && h.ProductId == item.ProductId && h.BillId == model.StockOutBillId && h.BillType == model.StockOutBillType && h.ChangeQuantity < 0).ToList();
            Ensure.NotNull(histories, "找不到商品{0}的出库记录。".FormatWith(product.Code));
            Ensure.SmallerOrEqualThan(item.Quantity, histories.Sum(h => Math.Abs(h.ChangeQuantity)), "商品{0}退货入库数量必须小于等于出库数量。".FormatWith(product.Code));

            var batchNos = histories.Select(h => h.BatchNo).ToArray();
            var inventoryBatchs = dbContext.Table<StoreInventoryBatch>()
                                           .Where(b => b.StoreId == model.StoreId && b.ProductId == item.ProductId && b.BatchNo.In(batchNos))
                                           .OrderByDesc(b => b.CreatedOn)   //按 后出先入 的方式还库存
                                           .ToList();
            Ensure.NotNullOrEmpty(inventoryBatchs, "找不到商品{0}出库的库存批次。".FormatWith(product.Code));
            Ensure.EqualThan(inventoryBatchs.Count, histories.Count, "商品{0}出库批次与出库记录不一致。".FormatWith(product.Code));

            var leftQty = item.Quantity;
            var categoryPreferential = item.CategoryPreferential / item.Quantity;//单品品类优惠额度
            var brandPreferential = item.BrandPreferential / item.Quantity;//单品品牌优惠额度
            var result = new StoreInventoryItemResult(item.ProductId, product.Code);
            foreach (var inventoryBatch in inventoryBatchs)
            {
                if (leftQty == 0) break;

                var history = histories.First(h => h.BatchNo == inventoryBatch.BatchNo);
                var addQty = Math.Min(leftQty, Math.Abs(history.ChangeQuantity));

                inventoryBatch.Quantity += addQty;
                leftQty -= addQty;

                var totalCategoryPreferential = Math.Round(categoryPreferential * addQty, 2);//当前批次总的品类优惠金额
                var totalBrandPreferential = Math.Round(brandPreferential * addQty, 2);//当前批次总的品牌优惠金额

                dbContext.Update(inventoryBatch);
                dbContext.Insert(new StoreInventoryHistory(inventoryBatch.Id, item.ProductId, model.StoreId, inventory.Quantity, addQty, inventoryBatch.Price, inventoryBatch.BatchNo, model.StockInBillId, model.StockInBillCode, model.StockInBillType, model.CreatedBy, model.CreatedOn, inventoryBatch.SupplierId, 0, null, totalCategoryPreferential, totalBrandPreferential));//记录库存历史

                result.BatchNos.Add(new ChangedBatch(inventoryBatch.BatchNo, addQty));
            }
            return result;
        }

        //1.2.正常入库
        private List<StoreInventoryItemResult> NormalInStock(IDBContext dbContext, StockInModel model)
        {
            var defaultBatchNo = long.Parse(_sequenceService.GenerateNewCode(BillIdentity.BatchNo));

            var result = new List<StoreInventoryItemResult>();

            model.Items.ForEach(item =>
            {
                var product = dbContext.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                Ensure.NotNull(product, "入库的商品不存在。");
                Ensure.GreaterThan(item.Quantity, 0, "商品{0}入库数量必须大于零。".FormatWith(product.Code));

                if (item.SNCode.NotNullOrEmpty())
                    result.Add(this.NormalInStockWithSNCode(dbContext, model, item, product, defaultBatchNo));
                else
                {
                    var batchCount = model.Items.Where(p => p.SNCode.IsNullOrEmpty()).GroupBy(g => g.ProductId).Max(g => g.Count());
                    var newBatchNo = defaultBatchNo;
                    if (batchCount > 1 && result.Any(r => r.ProductId == item.ProductId))
                        newBatchNo = long.Parse(_sequenceService.GenerateNewCode(BillIdentity.BatchNo));

                    result.Add(this.NormalInStockWithNoSNCode(dbContext, model, item, product, newBatchNo));
                }
            });
            return result;
        }

        //1.2.1.串码商品，正常入库
        private StoreInventoryItemResult NormalInStockWithSNCode(IDBContext dbContext, StockInModel model, StockInItemModel item, Product product, long batchNo)
        {
            Ensure.True(product.HasSNCode, "非串码商品{0}不能以串码方式入库。".FormatWith(product.Code));
            //Ensure.EqualThan(item.Quantity, item.SNCodes.Count, "商品{0}入库数量与串码个数不一致。".FormatWith(product.Code));
            Ensure.EqualThan(item.Quantity, 1, "串码{0}商品{1}入库数量必须为1。".FormatWith(item.SNCode, product.Code));

            var result = new StoreInventoryItemResult(item.ProductId, product.Code);

            //item.SNCodes.ForEach(sn =>
            //{
            //Ensure.NotNullOrEmpty(sn.SNCode, "入库商品{0}包含为空的串码。".FormatWith(product.Code));

            var inventory = this.GetInventory(model.StoreId, item.ProductId);

            //var inventoryBatch = this.GetInventoryBatch(model.StoreId, item.SNCode, item.ProductId);
            var inventoryBatch = _db.Table<StoreInventoryBatch>().FirstOrDefault(o => o.SNCode == item.SNCode && o.Quantity > 0);
            if (inventoryBatch != null)
            {
                Ensure.EqualThan(model.StoreId, inventoryBatch.StoreId, "串码{0}商品{1}在别的仓已存在库存，不能入库。".FormatWith(item.SNCode, product.Code));
                throw new FriendlyException("串码{0}商品{1}在本仓已存在库存，不能重复入库。".FormatWith(item.SNCode, product.Code));
            }

            var quantity = 1;
            inventoryBatch = new StoreInventoryBatch(item.ProductId, model.StoreId, item.SupplierId, quantity, item.ContractPrice, item.CostPrice, batchNo, item.ProductionDate, item.ShelfLife, model.CreatedBy, item.SNCode);
            var history = new StoreInventoryHistory(0, item.ProductId, model.StoreId, (inventory.NotNull() ? inventory.Quantity : 0), quantity, item.CostPrice, batchNo, model.StockInBillId, model.StockInBillCode, model.StockInBillType, model.CreatedBy, model.CreatedOn, item.SupplierId, 0, item.SNCode);
            inventoryBatch.History.Add(history);//记录库存历史
            dbContext.Insert(inventoryBatch);

            result.SNCodes.Add(item.SNCode);
            //});
            result.BatchNos.Add(new ChangedBatch(batchNo, item.Quantity));
            return result;
        }

        //1.2.2.非串码商品，正常入库
        private StoreInventoryItemResult NormalInStockWithNoSNCode(IDBContext dbContext, StockInModel model, StockInItemModel item, Product product, long batchNo)
        {
            Ensure.False(product.HasSNCode, "串码商品{0}只能以串码方式入库。".FormatWith(product.Code));

            var inventory = this.GetInventory(model.StoreId, item.ProductId);
            var inventoryBatch = new StoreInventoryBatch(item.ProductId, model.StoreId, item.SupplierId, item.Quantity, item.ContractPrice, item.CostPrice, batchNo, item.ProductionDate, item.ShelfLife, model.CreatedBy);
            var history = new StoreInventoryHistory(0, item.ProductId, model.StoreId, (inventory.NotNull() ? inventory.Quantity : 0), item.Quantity, item.CostPrice, batchNo, model.StockInBillId, model.StockInBillCode, model.StockInBillType, model.CreatedBy, model.CreatedOn, item.SupplierId);
            inventoryBatch.History.Add(history);//记录库存历史
            dbContext.Insert(inventoryBatch);

            var result = new StoreInventoryItemResult(item.ProductId, product.Code);
            result.BatchNos.Add(new ChangedBatch(batchNo, item.Quantity));
            return result;
        }

        //2.增加总库存
        private void AddStoreInventory(IDBContext dbContext, StockInModel model)
        {
            model.Items.GroupBy(g => g.ProductId).ToList().ForEach(g =>
            {
                var productId = g.First().ProductId;
                var totalQty = g.Sum(m => m.Quantity);
                var product = dbContext.Table<Product>().FirstOrDefault(p => p.Id == productId);
                Ensure.NotNull(product, "入库的商品不存在。");
                Ensure.GreaterThan(totalQty, 0, "商品{0}入库数量必须大于零。".FormatWith(product.Code));

                var inventory = this.GetInventory(model.StoreId, productId);
                var price = g.Sum(m => m.CostPrice * m.Quantity) / totalQty;

                if (inventory == null)
                {
                    var avgCostPrice = this.CalculatedAveragePrice(0, 0, price, totalQty);
                    dbContext.Insert(new StoreInventory(model.StoreId, productId, totalQty, avgCostPrice));
                }
                else
                {
                    var avgCostPrice = this.CalculatedAveragePrice(inventory.AvgCostPrice, inventory.Quantity, price, totalQty);
                    inventory.Quantity += totalQty;
                    inventory.AvgCostPrice = avgCostPrice;
                    dbContext.Update(inventory);
                }
            });
        }

        /// <summary>
        /// 计算库存移动平均价
        /// </summary>
        /// <param name="CurrentAvgCostPrice">当前库存均价</param>
        /// <param name="CurrentQuantity">当前库存</param>
        /// <param name="price">新进单价</param>
        /// <param name="quantity">新进数量</param>
        /// <returns></returns>
        public decimal CalculatedAveragePrice(decimal currentAvgCostPrice, int currentQuantity, decimal price, int quantity)
        {
            int totalQuantity = currentQuantity + quantity;
            var avgCostPrice = totalQuantity == 0 ? price : Math.Round((currentAvgCostPrice * currentQuantity + price * quantity) / totalQuantity, 4);
            return avgCostPrice;
        }

        #endregion


        public IEnumerable<SNCodeInventoryDto> GetSNCodeProduct(SearchStoreInventory condition)
        {
            dynamic param = new ExpandoObject();
            string where = "";


            if (!string.IsNullOrEmpty(condition.StoreId) && condition.StoreId != "0")
            {
                where += "and t1.StoreId in @StoreId ";
                param.StoreId = condition.StoreId.Split(',').ToIntArray(); ;
            }
            if (!string.IsNullOrEmpty(condition.ProductCodeOrBarCode))
            {
                where += "and p.Code=@ProductCodeOrBarCode  ";
                param.ProductCodeOrBarCode = condition.ProductCodeOrBarCode;
            }

            if (!string.IsNullOrEmpty(condition.ProductName))
            {
                where += "and p.Name like @ProductName ";
                param.ProductName = string.Format("%{0}%", condition.ProductName);
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

            string sql = @"
                SELECT  p.Code AS ProductCode ,
                        p.Name AS ProductName ,
                        t1.StoreId ,
                        t1.SNCode ,
                        s.Name AS StoreName ,
                        c.FullName AS CategoryName ,
                        b.Name AS BrandName ,
                        r.Name AS SupplierName ,
                        SUM(t1.Quantity) AS Quantity
                FROM    StoreInventoryBatch t1
                        LEFT JOIN Product p ON t1.ProductId = p.Id
                        LEFT JOIN Store s ON s.Id = t1.StoreId
                        LEFT JOIN Category c ON c.Id = p.CategoryId
                        LEFT JOIN Brand b ON b.Id = p.BrandId
                        LEFT JOIN Supplier r ON r.Id = t1.SupplierId
                WHERE   1 = 1 {0}
                GROUP BY p.Code ,
                        p.Name ,
		                t1.StoreId ,
                        t1.SNCode ,
                        s.Name ,
                        c.FullName ,
                        b.Name ,
                        r.Name".FormatWith(where);

            if (!string.IsNullOrEmpty(condition.Mark) && !string.IsNullOrEmpty(condition.StoreInventoryQuantity))
            {
                //sql = string.Format("select * from ({0}) t0 where 1=1 {1}", sql, "and t0.Quantity " + condition.Mark + " @Quantity ");
                sql += " HAVING  SUM(t1.Quantity) {0} @Quantity".FormatWith(condition.Mark);
                param.Quantity = condition.StoreInventoryQuantity;
            }

            var rows = this._db.DataBase.Query<SNCodeInventoryDto>(sql, param);

            return rows;
        }

        public IEnumerable<InventorySummaryDto> GetInventorySummary(Pager page, SearchInventorySummary condition)
        {
            var where = "";
            dynamic param = new ExpandoObject();

            if (condition.ProductIds.NotNullOrEmpty())
            {
                where += " AND p.Id IN @ProductIds";
                param.ProductIds = condition.ProductIds.Split(',');
            }

            if (condition.BrandIds.NotNullOrEmpty())
            {
                where += " AND p.BrandId IN @BrandIds";
                param.BrandIds = condition.BrandIds.Split(',');
            }

            if (condition.CategoryIds.NotNullOrEmpty())
            {
                where += " AND p.CategoryId IN @CategoryIds";
                param.CategoryIds = condition.CategoryIds.Split(',');
            }

            if (condition.CategoryCode.NotNullOrEmpty())
            {
                where += " AND c.Code LIKE @CategoryCode";
                param.CategoryCode = condition.CategoryCode + "%";
            }

            if (condition.StoreIds.NotNullOrEmpty())
            {
                where += " AND i.StoreId IN @StoreIds";
                param.StoreIds = condition.StoreIds.Split(',');
            }

            if (condition.Time.NotNullOrEmpty())
            {
                where += " AND i.CreatedOn >= @CreatedOnFrom AND i.CreatedOn < @CreatedOnTo";
                var time = condition.Time.Split(',');
                param.CreatedOnFrom = time[0];
                param.CreatedOnTo = DateTime.Parse(time[1]).AddDays(1);
            }

            var basicSql = @"
                WITH    t AS ( SELECT   i.StoreId ,
                                        i.ProductId ,
                                        p.BrandId ,
                                        p.CategoryId ,
                                        i.SupplierId ,
                                        i.Quantity ,
                                        i.Price ,
                                        FORMAT(i.CreatedOn, 'yyyy-MM-dd') AS CreatedOn
                               FROM     StoreInventoryBatch i
                                        INNER JOIN Product p ON p.Id = i.ProductId
                                        INNER JOIN Category c ON c.Id = p.CategoryId
                               WHERE    1 = 1 {0}
                             )".FormatWith(where);

            var orderBy = "";
            var summarySql = "";
            switch (condition.SummaryMethod)
            {
                case 2:     //品牌汇总
                    summarySql = @"
                    SELECT  t.BrandId ,
                            b.Code AS BrandCode ,
                            b.Name AS BrandName ,
                            SUM(t.Quantity) AS Quantity ,
                            SUM(t.Quantity * t.Price) AS CostAmount
                    FROM    t
                            INNER JOIN Brand b ON b.Id = t.BrandId
                    GROUP BY t.BrandId ,
                            b.Code ,
                            b.Name";
                    orderBy = "t.BrandId ASC";
                    break;
                case 3:     //品类汇总
                    summarySql = @"
                    SELECT  t.CategoryId ,
                            c.Code AS CategoryCode ,
                            c.Name AS CategoryName ,
                            SUM(t.Quantity) AS Quantity ,
                            SUM(t.Quantity * t.Price) AS CostAmount
                    FROM    t
                            INNER JOIN Category c ON c.Id = t.CategoryId
                    GROUP BY t.CategoryId ,
                            c.Code ,
                            c.Name";
                    orderBy = "t.CategoryId ASC";
                    break;
                case 4:     //日期汇总
                    summarySql = @"
                    SELECT  t.CreatedOn ,
                            SUM(t.Quantity) AS Quantity ,
                            SUM(t.Quantity * t.Price) AS CostAmount
                    FROM    t
                    GROUP BY t.CreatedOn";
                    orderBy = "t.CreatedOn ASC";
                    break;
                case 5:     //门店汇总
                    summarySql = @"
                    SELECT  t.StoreId ,
                            s.Code AS StoreCode ,
                            s.Name AS StoreName ,
                            SUM(t.Quantity) AS Quantity ,
                            SUM(t.Quantity * t.Price) AS CostAmount
                    FROM    t
                            INNER JOIN Store s ON s.Id = t.StoreId
                    GROUP BY t.StoreId ,
                            s.Code ,
                            s.Name";
                    orderBy = "t.StoreId ASC";
                    break;
                default:     //商品汇总
                    summarySql = @"
                    SELECT  t.ProductId ,
                            p.Code AS ProductCode ,
                            p.Name AS ProductName ,
                            SUM(t.Quantity) AS Quantity ,
                            SUM(t.Quantity * t.Price) AS CostAmount
                    FROM    t
                            INNER JOIN Product p ON p.Id = t.ProductId
                    GROUP BY t.ProductId ,
                            p.Code ,
                            p.Name";
                    orderBy = "t.ProductId ASC";
                    break;
            }

            if (page.IsPaging == false)
                return _db.DataBase.Query<InventorySummaryDto>(basicSql + summarySql + " ORDER BY " + orderBy, param);

            var pageSql = "{0} ORDER BY {1} OFFSET {2} ROWS FETCH NEXT {3} ROWS ONLY".FormatWith(basicSql + summarySql, orderBy, page.PageSize * (page.PageIndex - 1), page.PageSize);
            var countSql = "{0} SELECT COUNT(1) FROM ({1}) AS temp".FormatWith(basicSql, summarySql);

            var rows = _db.DataBase.Query<InventorySummaryDto>(pageSql, param);
            page.Total = _db.DataBase.ExecuteScalar<int>(countSql, param);
            return rows;
        }
    }
}
