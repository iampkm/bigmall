using Dapper.DBContext;
using Guoc.BigMall.Application.Configuration;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.Objects;
using Guoc.BigMall.Domain.Service;
using Guoc.BigMall.Domain.ValueObject;
using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Infrastructure.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
namespace Guoc.BigMall.Application.Facade
{
    public class PurchaseFacade : IPurchaseFacade
    {
        IDBContext _db;
        ProcessHistoryService _processHistoryService;
        IStoreInventoryFacade _istoreInventoryFacade;
        BillSequenceService _sequenceService;
        ILogger _log;
        ProductPurchasePriceService _purchasePriceService;
        ISAPService _sapService;
        public PurchaseFacade(IDBContext dbContext, ILogger log, IStoreInventoryFacade istoreInventoryFacade, BillSequenceService sequenceService, ProductPurchasePriceService purchasePriceService, ISAPService sapService)
        {
            this._db = dbContext;
            _processHistoryService = new ProcessHistoryService(this._db);
            this._log = log;
            this._istoreInventoryFacade = istoreInventoryFacade;
            this._sequenceService = sequenceService;
            this._purchasePriceService = purchasePriceService;
            _sapService = sapService;


        }
        public IEnumerable<PurchaseOrderDetailListDto> GetDetailList(Pager page, Search.SearchPurchaseOrder condition)
        {
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrEmpty(condition.Code))
            {
                where += "and t0.Code like @Code ";
                param.Code = string.Format("%{0}%", condition.Code);
            }
            if (!string.IsNullOrEmpty(condition.SupplierId))
            {
                where += "and t1. Id in  @SupplierId ";
                param.SupplierId = condition.SupplierId.Split(',').ToIntArray();
            }
            if (!string.IsNullOrEmpty(condition.StoreId) && condition.StoreId != "0")
            {
                where += "and t2.Id in @StoreId ";
                param.StoreId = condition.StoreId.Split(',').ToIntArray(); ;
            }


            if (!string.IsNullOrEmpty(condition.Status))
            {
                where += "and t0.Status in (" + condition.Status + ") ";
            }
            //if (!string.IsNullOrEmpty(condition.ProductCodeOrBarCode))
            //{
            //    where += string.Format(" and  p.Code='{0}' ", condition.ProductCodeOrBarCode);
            //}
            //if (!string.IsNullOrEmpty(condition.BrandId))
            //{
            //    where += string.Format(" and  p.BrandId in ({0}) ", condition.BrandId);
            //}
            if (!string.IsNullOrWhiteSpace(condition.OrderType))
            {
                where += " and t0.ordertype =@ordertype ";
                param.ordertype = Convert.ToInt32(condition.OrderType);
            }

            if (!string.IsNullOrWhiteSpace(condition.Time))
            {
                var times = condition.Time.Split(',');
                where += "and t0.CreatedOn >=@StartDate  and t0.CreatedOn < @EndDate";
                param.StartDate = Convert.ToDateTime(times[0]);
                param.EndDate = Convert.ToDateTime(times[1]).AddDays(1);
            }

            if (!string.IsNullOrEmpty(condition.BillType) && condition.BillType != "0")
            {
                where += " and t0.BillType in @BillType ";
                param.BillType = condition.BillType.Split(',').ToIntArray();
            }
            if (!string.IsNullOrEmpty(condition.SapCode))
            {
                where += " and t0.SapCode like @SapCode ";
                param.SapCode = string.Format("{0}%", condition.SapCode);
            }
            if (condition.IsPushSap.HasValue)
            {
                where += " and t0.IsPushSap = @IsPushSap ";
                param.IsPushSap = condition.IsPushSap.Value;
            }
            string sql = @"select * from ( select ROW_NUMBER() over(order by t0.CreatedOn desc) as rownum,   t0.Id,t0.Code,t0.CreatedOn ,t0.Status,t0.Remark,t0.BillType,t0.SapCode AS SapOrderId,t0.Amount ,
t1.Code as SupplierCode,t1.Name as SupplierName,t2.Name as StoreName,t2.Code as StoreCode ,a.NickName as CreatedByName,t0.OrderType

from dbo.PurchaseOrder t0 

left join supplier t1 on t1.Code = t0.SupplierCode 
left join Store t2 on t2.id = t0.StoreId

left join Account a on a.Id = t0.CreatedBy  
          
            where 1=1 {0}  ) T ";
            sql = string.Format(sql, where);
            sql += string.Format(" where  T.rownum BETWEEN {0} AND {1} ", (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize);


            var rows = this._db.DataBase.Query<PurchaseOrderDetailListDto>(sql, param);
            string sqlcount = string.Format(@"
 select  COUNT(1) from dbo.PurchaseOrder t0 

left join supplier t1 on t1.Code = t0.SupplierCode 
left join Store t2 on t2.id = t0.StoreId

left join Account a on a.Id = t0.CreatedBy  
          
            where 1=1 {0}  ", where);
            page.Total = _db.DataBase.ExecuteScalar<int>(sqlcount, param);
            return rows;

        }


        public IEnumerable<PurchaseOrderDto> GetOrderList(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("id不能为空");
            var idArray = id.Split(',').ToIntArray();
            string sql = @"select t0.Id,t0.Code,t1.Id AS SupplierId,t0.SupplierCode,t0.StoreCode,t0.CreatedOn ,t0.Remark
,t0.Status,t0.type AS BillType,t0.SapCode AS SapOrderId,
t1.Name as SupplierName,t2.Name as StoreName,t3.NickName as CreatedByName,
p.Name as ProductName,p.Code as ProductCode,p.Spec AS Specification,p.Unit,p.id AS  ProductId,i.CostPrice,i.Quantity
from PurchaseOrder t0 
left join supplier t1 on t0.SupplierId = t1.Id
left join store t2 on t0.StoreId = t2.Id
left join account t3 on t3.Id = t0.CreatedBy
inner join PurchaseOrderItem i on i.PurchaseOrderId = t0.Id 
left join product p on p.Id = i.ProductId
where t0.Id in @Id  order by t0.id desc";
            var rows = _db.DataBase.Query<PurchaseOrderDetailDto>(sql, new { Id = idArray });
            List<PurchaseOrderDto> list = new List<PurchaseOrderDto>();
            var groupModels = rows.GroupBy(n => n.Id);
            foreach (var models in groupModels)
            {
                var row = models.FirstOrDefault();
                PurchaseOrderDto model = new PurchaseOrderDto()
                {
                    Id = row.Id,
                    SupplierId = row.SupplierId,
                    SupplierCode = row.SupplierCode,
                    SupplierName = row.SupplierName,
                    StoreCode = row.StoreCode,
                    StoreName = row.StoreName,
                    Status = row.Status,
                    //OrderType = row.OrderType,
                    //PaymentWay = row.PaymentWay,
                    CreatedOn = row.CreatedOn,
                    CreatedByName = row.CreatedByName,
                    Code = row.Code,
                    ParentCode = row.ParentCode,
                    Remark = row.Remark,
                    //FinanceRemark = row.FinanceRemark,
                    BillType = row.BillType,
                    SapOrderId = row.SapOrderId,
                    //Shipping = row.Shipping,
                    //Address = row.Address,
                    //Phone = row.Phone,
                    //Buyer = row.Buyer
                };
                foreach (var item in models)
                {
                    PurchaseOrderItemDto tempItem = new PurchaseOrderItemDto();
                    tempItem.ProductId = item.ProductId;
                    tempItem.ProductCode = item.ProductCode;
                    tempItem.ProductName = item.ProductName;
                    tempItem.Specification = item.Specification;
                    tempItem.Unit = item.Unit;
                    tempItem.CostPrice = item.CostPrice;
                    tempItem.Quantity = item.Quantity;
                    model.Items.Add(tempItem);
                }
                list.Add(model);

            }
            return list;
        }


        public PurchaseOrderDto GetById(int id)
        {
            string sql = @"SELECT  t0.Id ,
        t0.Code ,
        t1.Id AS SupplierId ,
        t0.SupplierCode ,
        t2.Id AS StoreId ,
        t2.Code as StoreCode ,
        t0.CreatedOn ,
        t3.NickName AS CreatedByName ,
        t0.Status ,
        t1.Code AS SupplierCode ,
        t1.Name AS SupplierName ,
        t2.Name AS StoreName ,
        t0.Remark ,
        t0. BillType , t0.UpdatedOn ,
        t4.NickName AS UpdatedByName ,
        t0.SapCode AS SapOrderId ,t2.Address,t2.Phone,t0.ordertype
FROM    PurchaseOrder t0
        LEFT JOIN Supplier t1 ON t0.SupplierId = t1.Id
        LEFT JOIN Store t2 ON t0.StoreId = t2.Id
        LEFT JOIN Account t3 ON t3.Id = t0.CreatedBy
        LEFT  JOIN Account t4 on t4.Id=t0.UpdatedBy
where t0.Id= @Id ";
            var model = _db.DataBase.QuerySingle<PurchaseOrderDto>(sql, new { Id = id });

            string sqlitem = @"SELECT i.id,  i.PurchaseOrderId ,
          i.ProductId ,          i.Quantity ,          i.CostPrice ,          i.ActualQuantity ,   
        p.Name AS ProductName ,
        p.Code AS ProductCode ,
        p.Spec AS Specification ,
        p.Unit ,p.Id  as ProductId,  p.HasSNCode ,t.AllSNcodes,tr.InventoryQuantity
FROM    PurchaseOrderItem i
        INNER JOIN Product p ON i.ProductId = p.Id
        INNER JOIN  dbo.PurchaseOrder pr ON pr.Id=i.PurchaseOrderId
        left JOIN ( SELECT  b.StoreId ,
                            b.SupplierId ,
                            b.ProductId ,
                            SUM(b.Quantity) AS InventoryQuantity
                    FROM    StoreInventoryBatch b
                    GROUP BY b.StoreId ,
                            b.SupplierId ,
                            b.ProductId
                    ) tr ON tr.StoreId = pr.StoreId
						    AND tr.SupplierId=pr.SupplierId
                            AND tr.ProductId = i.ProductId
        left JOIN Supplier s ON tr.SupplierId = s.Id
        LEFT JOIN  (   SELECT DISTINCT
                            BillCode ,BillItemId,
                            STUFF(( SELECT  ',' +  t.SNCodes
                                    FROM    dbo.StoreInventoryHistorySAP t
                                    WHERE   t.BillCode = tc.BillCode AND t.BillItemId=tc.BillItemId
                                  FOR
                                    XML PATH('')
                                  ), 1, 1, '') AS AllSNcodes
                     FROM   dbo.StoreInventoryHistorySAP tc
                     GROUP BY tc.BillCode ,tc.BillItemId 
					)  t ON  t.BillCode=pr.code and BillItemId=i.id
WHERE   i.PurchaseOrderId = @Id";


            var productItems = _db.DataBase.Query<PurchaseOrderItemDto>(sqlitem, new { Id = id, StoreId = model.StoreId }).ToList();
            if (productItems.Count == 0)
            {
                productItems.Add(new PurchaseOrderItemDto());
            }

            model.Items = productItems;

            return model;
        }


        public void StockIn(PurchaseOrderModel model)
        {

            var entity = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Id == model.Id);
            if (entity.Status != CBPurchaseOrderStatus.Audited)
                throw new FriendlyException("不是已审状态无法收货！");
            entity.Remark = model.Remark;


            var entityItems = _db.Table<PurchaseOrderItem>().Where(n => n.PurchaseOrderId == model.Id).ToList();
            entity.SetItems(entityItems);

            var receiveList = JsonConvert.DeserializeObject<List<PurchaseOrderItem>>(model.Items);
            if (receiveList.Where(n => n.SNQuantity > 0).Count() == 0)
                throw new FriendlyException("无需要入库的商品，请检查入库数量！");
            //检测串码与数量是否匹配
            _istoreInventoryFacade.JudgeSnCodes(receiveList);
            // 更改收货明细
            var allReceive = entity.UpdateReceivedGoodsItems(receiveList);

            foreach (var item in entity.Items)
            {
                var product = _db.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                Ensure.NotNull(product, "商品不存在。");
                Ensure.False(product.HasSNCode && item.SNQuantity > SystemConfig.ItemMaxSNCodeQuantity, "商品{0}一次最多只能入库{0}个串码。".FormatWith(product.Code, SystemConfig.ItemMaxSNCodeQuantity));
            }


            entity.StockIn(model.EditBy);
            // 更改入库状态
            if (allReceive)
                entity.Status = CBPurchaseOrderStatus.Finished;

            entity.IsPushSap = false;
            _db.Update(entity);
            // 记录流程日志
            var reason = "采购订单收货入库";
            var billIdentity = entity.BillType == PurchaseOrderBillType.StoreOrder ? BillIdentity.StorePurchaseOrder : BillIdentity.StockPurchaseOrder;

            _processHistoryService.Track(model.EditBy, model.EditByName, (int)entity.Status, entity.Id, billIdentity.ToString(), reason);
            // 入库
            var historySaps = this.StockIn(entity);

            //更改商品
            _db.SaveChange();
            ////通知SAP 收货成功
            ProcessHistory phistory = new ProcessHistory()
            {
                CreatedBy = model.EditBy,
                CreatedByName = model.EditByName,
                CreatedOn = DateTime.Now,
                FormId = entity.Id,
                FormType = billIdentity.ToString(),
                Status = (int)entity.Status,
                Remark = reason
            };
            this.SapStockIn(entity, historySaps, phistory, true);
        }

        public List<StoreInventoryHistorySAP> StockIn(PurchaseOrder entity)
        {
            if (entity == null) { throw new FriendlyException("单据不存在"); }
            if (entity.Items.Count() == 0) { throw new FriendlyException("单据明细为空"); }

            var store = _db.Table<Store>().FirstOrDefault(n => n.Id == entity.StoreId);
            if (store == null)
                throw new FriendlyException("未获取门店信息！");
            var inventorys = new List<StoreInventory>();
            var inventoryAdds = new List<StoreInventory>();
            var inventoryBatchs = new List<StoreInventoryBatch>();
            var inventoryHistorys = new List<StoreInventoryHistory>();
            var batchNo = long.Parse(_sequenceService.GenerateNewCode(BillIdentity.BatchNo));
            var sapHistorys = new List<StoreInventoryHistorySAP>();
            var billIdentity = entity.BillType == PurchaseOrderBillType.StoreOrder ? BillIdentity.StorePurchaseOrder : BillIdentity.StockPurchaseOrder;
            var historyCode = long.Parse(_sequenceService.GenerateNewCode(BillIdentity.SapHistoryOrder)).ToString();
            foreach (var item in entity.Items)
            {
                if (item.SNQuantity == 0)
                    continue;
                var inventoryQuantity = 0;
                var inventory = _db.Table<StoreInventory>().FirstOrDefault(n => n.ProductId == item.ProductId && n.StoreId == entity.StoreId);
                if (inventory == null)
                {
                    var avgCostPrice = this.CalculatedAveragePrice(0, 0, item.CostPrice, item.SNQuantity);
                    inventoryAdds.Add(new StoreInventory(entity.StoreId, item.ProductId, item.SNQuantity, avgCostPrice));
                }
                else
                {
                    // 入库      
                    inventoryQuantity = inventory.Quantity;
                    inventory.Quantity += item.SNQuantity;
                    inventory.SaleQuantity += item.SNQuantity;
                    inventory.AvgCostPrice = CalculatedAveragePrice(inventory.AvgCostPrice, inventoryQuantity, item.CostPrice, item.SNQuantity);  // 修改库存均价
                    inventory.LastCostPrice = item.CostPrice > 0 ? item.CostPrice : inventory.LastCostPrice;
                    inventorys.Add(inventory);
                }
                // 回写采购入库批次
                item.BatchNo = batchNo;


                //记录库存批次
                if (!item.IsSnCode) //非串码商品
                {

                    var batch = new StoreInventoryBatch(item.ProductId, entity.StoreId, entity.SupplierId, item.SNQuantity, item.CostPrice, item.CostPrice, batchNo, null, 0, entity.UpdatedBy, null);
                    batch.PurchaseQuantity = item.Quantity;
                    inventoryBatchs.Add(batch);
                    //记录库存流水
                    var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, inventoryQuantity, item.SNQuantity, item.CostPrice, batchNo, entity.Id, entity.Code, billIdentity, entity.UpdatedBy, entity.SupplierId, null);
                    inventoryHistorys.Add(history);
                }
                else     ///记录采购批次串码
                {

                    var sncodes = item.SNCodes.Split(',');
                    foreach (var code in sncodes)
                    {
                        if (_db.Table<StoreInventoryBatch>().Exists(n => n.SNCode == code))
                            throw new FriendlyException(string.Format("商品:{0}的串码[{1}]已存在，请确认后操作！", item.ProductCode, code));
                        var batch = new StoreInventoryBatch(item.ProductId, entity.StoreId, entity.SupplierId, 1, item.CostPrice, item.CostPrice, batchNo, null, 0, entity.UpdatedBy, code);
                        batch.PurchaseQuantity = 1;
                        inventoryBatchs.Add(batch);

                        //记录库存流水
                        var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, inventoryQuantity, 1, item.CostPrice, batchNo, entity.Id, entity.Code, billIdentity, entity.UpdatedBy, entity.SupplierId, code);
                        inventoryHistorys.Add(history);
                    }
                }
                //记录库存流水(SAP)
                var sap = new StoreInventoryHistorySAP(historyCode, StoreInventoryHistorySapType.OutStock, item.ProductId, item.ProductCode, entity.StoreId, store.Code, item.SNQuantity, item.SNCodes, entity.Code, billIdentity, entity.UpdatedBy);
                sap.BillItemId = item.Id;
                sapHistorys.Add(sap);



                ////更新商品是否串码商品
                //var product=_db.Table<Product>().FirstOrDefault(n=>n.Id==item.ProductId);
                //product.HasSNCode=item.IsSnCode;
                //products.Add(product);

                item.IsSnCode = false;
                item.SNQuantity = 0;
                item.SNCodes = null;

            }


            _db.Update(entity.Items.ToArray());
            _db.Update(inventorys.ToArray());
            _db.Insert(inventoryAdds.ToArray());
            _db.Insert(inventoryHistorys.ToArray());
            _db.Insert(inventoryBatchs.ToArray());
            _db.Insert(sapHistorys.ToArray());
            return sapHistorys;

        }

        public void SapStockIn(PurchaseOrder purchaseOrder, List<StoreInventoryHistorySAP> historySapList, ProcessHistory phistory, bool isInStock)
        {
            var historyCode = historySapList.FirstOrDefault().Code;
            var historySaps = _db.Table<StoreInventoryHistorySAP>().Where(n => n.Code == historyCode).ToList();
            var poReceive = new POReceive()
            {
                POCode = purchaseOrder.Code,
                SapPOCode = purchaseOrder.SapCode,
                InStockCode = historyCode,
                InStockDate = purchaseOrder.UpdatedOn,
                Items = historySaps.Select(history =>
                {
                    var item = purchaseOrder.Items.First(h => h.ProductId == history.ProductId);
                    return new POReceiveItem()
                    {
                        InStockRow = history.Id,
                        SapPORow = item.SapRow,
                        ProductCode = item.ProductCode,
                        Quantity = history.Quantity,
                        SNCodes = history.SNCodes,
                        Unit = item.Unit,
                        StoreCode = history.StoreCode,
                    };
                }).ToList(),
            };
            // purchaseOrder.IsPushSap = false;
            _sapService.PurchaseOrderInStock(poReceive); ;//推送SAP

            //记录SAP出库单信息
            poReceive.Items.ForEach(item =>
            {
                var historyUpdates = historySaps.Where(n => n.Code == poReceive.InStockCode && n.Id == item.InStockRow).FirstOrDefault();
                historyUpdates.SAPCode = poReceive.SapInStockCode;
                historyUpdates.SAPRow = item.SapInStockRow;

                //_db.Update<StoreInventoryHistorySAP>(h =>
                //    new StoreInventoryHistorySAP()
                //    {
                //        SAPCode = poReceive.SapInStockCode,
                //        SAPRow = item.SapInStockRow
                //    },
                //    h => h.Code == poReceive.InStockCode && h.Id == item.InStockRow);


            });
            purchaseOrder.IsPushSap = true;
            _db.Insert(new ProcessHistory(purchaseOrder.UpdatedBy, "系统", purchaseOrder.Status.Value(), purchaseOrder.Id, historySapList.FirstOrDefault().BillType.Name(), "SAP{0}成功".FormatWith(isInStock ? "入库" : "出库")));
            _db.Update(historySaps.ToArray());
            _db.Update(purchaseOrder);
            _db.Insert(phistory);
            _db.SaveChange();
        }
        /// <summary>
        /// 计算库存移动平均价
        /// </summary>
        /// <param name="CurrentAvgCostPrice">当前库存均价</param>
        /// <param name="CurrentQuantity">当前库存</param>
        /// <param name="price">新进单价</param>
        /// <param name="quantity">新进数量</param>
        /// <returns></returns>
        private decimal CalculatedAveragePrice(decimal CurrentAvgCostPrice, int CurrentQuantity, decimal price, int quantity)
        {

            // 修改库存均价

            int totalQuantity = CurrentQuantity + quantity;

            var avgCostPrice = totalQuantity == 0 ? price : Math.Round((CurrentAvgCostPrice * CurrentQuantity + price * quantity) / totalQuantity, 4);

            return avgCostPrice;

        }
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




        public IEnumerable<PurchaseOrderDetailListDto> GetRefundDetailList(Pager page, Search.SearchPurchaseOrder condition)
        {
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrEmpty(condition.Code))
            {
                where += "and pr.Code =@Code ";
                param.Code = condition.Code;
            }

            if (!string.IsNullOrEmpty(condition.SapCode))
            {
                where += "and pr.SapCode =@SapCode ";
                param.SapCode = condition.SapCode;
            }

            if (!string.IsNullOrEmpty(condition.StoreId) && condition.StoreId != "0")
            {
                where += "and s.Id in @StoreId ";
                param.StoreId = condition.StoreId.Split(',').ToIntArray(); ;
            }

            if (!string.IsNullOrEmpty(condition.Status))
            {
                where += "and pr.Status in( " + condition.Status + ") ";
            }
            if (!string.IsNullOrWhiteSpace(condition.Time))
            {
                var times = condition.Time.Split(',');
                where += "and pr.CreatedOn >=@StartDate  and pr.CreatedOn < @EndDate";
                param.StartDate = Convert.ToDateTime(times[0]);
                param.EndDate = Convert.ToDateTime(times[1]).AddDays(1);
            }

            if (!string.IsNullOrEmpty(condition.BillType) && condition.BillType != "0")
            {
                where += " and pr.BillType = @BillType ";
                param.BillType = Convert.ToInt32(condition.BillType);
            }
            if (!string.IsNullOrWhiteSpace(condition.Creater) && condition.Creater != "0")
            {
                where += " and pr.CreatedBy =@Creater ";
                param.Creater = Convert.ToInt32(condition.Creater);
            }
            if (!string.IsNullOrWhiteSpace(condition.OrderType))
            {
                where += " and pr.ordertype =@ordertype ";
                param.ordertype = Convert.ToInt32(condition.OrderType);
            }


            if (!string.IsNullOrEmpty(condition.ProductCodeOrBarCode))
            {
                where += " and p.id in  @ProductCodeOrBarCode";
                param.ProductCodeOrBarCode = condition.ProductCodeOrBarCode.Split(',').ToArray();

            }
            if (!string.IsNullOrEmpty(condition.BrandId))
            {
                where += " and  b.id in @brandId";
                param.brandId = condition.BrandId.Split(',').ToArray();
            }
            if (!string.IsNullOrWhiteSpace(condition.categoryId))
            {
                where += " and  c.id in @categoryId";
                param.categoryId = condition.categoryId.Split(',').ToArray();

            }

            var filter = "";
            if (condition.IsPushSap.HasValue)
            {
                filter += " and T.IsPushSap = @IsPushSap ";
                param.IsPushSap = condition.IsPushSap.Value;
            }

            if (!string.IsNullOrWhiteSpace(condition.SupplierId))
            {
                where += " and su.id = " + condition.SupplierId;
            }

            var dataSql = @"
                SELECT  {0}
                FROM    ( SELECT    pr.Id ,
                                    pr.Code ,
                                    pr.StoreId ,
                                    pr.StoreCode ,
                                    pr.OrderType ,
                                    pr.BillType ,
                                    pr.SupplierCode ,
                                    pr.SupplierId ,
                                    pr.Status ,
                                    pr.CompanyCode ,
                                    pr.CreatedOn ,
                                    pr.CreatedBy ,
                                    pr.SapCode ,
                                    pr.PurchaseGroupId ,
                                    pr.PurchaseTime ,
                                    pr.Amount ,
                                    pr.UpdatedOn ,
                                    pr.UpdatedBy ,
                                    pr.Remark ,
                                    ( CASE WHEN pr.OrderType = 1
                                                AND EXISTS ( SELECT 1
                                                             FROM   StoreInventoryHistorySAP h
                                                             WHERE  h.BillCode = pr.Code
                                                                    AND h.SAPCode IS NULL )
                                           THEN 0
                                           ELSE pr.IsPushSap
                                      END ) AS IsPushSap ,
                                    pr.SapCode AS saporderid ,
                                    s.Name ,
                                    a.NickName AS CreatedByName ,
                                    su.Name AS suppliername ,
                                    s.Name AS storename ,
                                    p.Code AS productCode ,
                                    p.Name AS productName ,
                                    pri.Quantity ,
                                    c.Name AS CategoryName ,
                                    b.Name AS BrandName ,
                                    pri.CostPrice ,
                                    pri.ActualQuantity
                          FROM      dbo.PurchaseOrder pr
                                    INNER JOIN dbo.PurchaseOrderItem pri ON pr.Id = pri.PurchaseOrderId
                                                                            AND pri.Quantity > 0
                                    INNER JOIN dbo.Store s ON pr.StoreId = s.Id
                                    INNER JOIN dbo.Account a ON a.Id = pr.CreatedBy
                                    INNER JOIN dbo.Supplier su ON su.Id = pr.SupplierId
                                    INNER JOIN dbo.Product p ON p.Id = pri.ProductId
                                    INNER JOIN dbo.Brand b ON b.Id = p.BrandId
                                    INNER JOIN dbo.Category c ON p.CategoryId = c.Id
                          WHERE     1 = 1 {1}
                        ) T
                WHERE   1 = 1 {2}".FormatWith("{0}", where, filter);

            if (page.IsPaging == false)
            {
                return this._db.DataBase.Query<PurchaseOrderDetailListDto>(dataSql.FormatWith("*"), param);
            }

            var pageSql = "{0} ORDER BY T.Id DESC OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY".FormatWith(dataSql.FormatWith("*"), (page.PageIndex - 1) * page.PageSize, page.PageSize);
            var rows = this._db.DataBase.Query<PurchaseOrderDetailListDto>(pageSql, param);

            var countSql = dataSql.FormatWith("COUNT(1)");
            page.Total = _db.DataBase.ExecuteScalar<int>(countSql, param);
            return rows;
        }


        public void Cancel(int id, int editBy, string editor, string reason)
        {
            var entity = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Id == id);
            entity.Cancel(editBy);
            _db.Update(entity);

            var billIdentity = entity.BillType == PurchaseOrderBillType.StoreOrder ? BillIdentity.StorePurchaseRefundOrder : BillIdentity.StockPurchaseRefundOrder;
            _processHistoryService.Track(editBy, editor, (int)entity.Status, entity.Id, billIdentity.ToString(), reason);
            _db.SaveChange();
        }


        public IEnumerable<ProductDto> GetRefundProduct(Pager page, Search.SearchProduct condition)
        {
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrEmpty(condition.Name))
            {
                where += "and p.Name like @ProductName ";
                param.ProductName = string.Format("%{0}%", condition.Name.Trim());
            }
            if (!string.IsNullOrEmpty(condition.Code))
            {
                where += "and(  p.Code like @ProductCode) ";
                param.ProductCode = string.Format("%{0}%", condition.Code.Trim());
            }

            string sql = @"SELECT * FROM (
                       SELECT ROW_NUMBER() OVER ( ORDER BY p.Id DESC ) AS rownum ,
        p.Id   ,
        p.Name   ,
        p.Code   ,
        p.Spec AS  Specification ,
        p.Unit ,
        i.StoreId ,
        e.Code AS SapStoreCode ,
        PP.Price AS CostPrice ,
        t.inventoryQuantity - i.LockedQuantity AS inventoryQuantity ,
        t.SupplierId ,
        s.Name AS SupplierName ,
        s.Code AS SupplierCode
 FROM   StoreInventory i
        INNER JOIN dbo.ProductPurchasePrice PP ON PP.Id=i.ProductId
        LEFT JOIN Product p ON p.Id = i.ProductId
        LEFT JOIN ( SELECT  b.StoreId ,
                            b.SupplierId ,
                            b.ProductId ,
                            SUM(b.Quantity) AS inventoryQuantity
                    FROM    StoreInventoryBatch b
                    GROUP BY b.StoreId ,
                            b.SupplierId ,
                            b.ProductId
                  ) t ON t.StoreId = i.StoreId
                         AND t.ProductId = i.ProductId
        LEFT JOIN Supplier s ON t.SupplierId = s.Id
        LEFT JOIN Store e ON e.Id = i.StoreId
 WHERE  1 = 1
        AND t.inventoryQuantity - i.LockedQuantity > 0 {0}  )T   where rownum between {1} and {2} ";
            sql = string.Format(sql, where, (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize);
            var rows = this._db.DataBase.Query<ProductDto>(sql, param);
            string sqlCount = @"select count(*)
from  StoreInventory i
        INNER JOIN dbo.ProductPurchasePrice PP ON PP.Id=i.ProductId
        LEFT JOIN Product p ON p.Id = i.ProductId
        LEFT JOIN ( SELECT  b.StoreId ,
                            b.SupplierId ,
                            b.ProductId ,
                            SUM(b.Quantity) AS inventoryQuantity
                    FROM    StoreInventoryBatch b
                    GROUP BY b.StoreId ,
                            b.SupplierId ,
                            b.ProductId
                  ) t ON t.StoreId = i.StoreId
                         AND t.ProductId = i.ProductId
        LEFT JOIN Supplier s ON t.SupplierId = s.Id
        LEFT JOIN Store e ON e.Id = i.StoreId
 WHERE  1 = 1
        AND t.inventoryQuantity - i.LockedQuantity > 0 {0}";
            sqlCount = string.Format(sqlCount, where);
            page.Total = this._db.DataBase.ExecuteScalar<int>(sqlCount, param);

            return rows;
        }


        public void RefundCreate(PurchaseOrderModel model)
        {

            var entity = model.MapTo<PurchaseOrder>();

            entity.CreatedBy = model.EditBy;
            entity.UpdatedBy = model.EditBy;

            var allItems = JsonConvert.DeserializeObject<List<PurchaseOrderItem>>(model.Items);


            var store = _db.Table<Store>().FirstOrDefault(n => n.Id == entity.StoreId);
            if (store == null)
            {
                throw new FriendlyException("获取门店信息失败！");
            }

            if (store.IsMainStore())
            {
                throw new FriendlyException("您选择的门店是总仓，商玛特只能创建门店退货单！");
            }
            var supplier = _db.Table<Supplier>().FirstOrDefault(n => n.Id == entity.SupplierId);
            if (supplier == null)
            {
                throw new FriendlyException("获取供应商信息失败！");
            }
            entity.StoreCode = store.Code;
            entity.SupplierCode = supplier.Code;
            entity.BillType = PurchaseOrderBillType.StoreOrder;
            entity.Code = _sequenceService.GenerateNewCode(BillIdentity.StorePurchaseRefundOrder);


            foreach (var item in allItems)
            {
                if (item.Quantity <= 0)
                    throw new FriendlyException(string.Format("商品【{0}】数量必须大于零！", item.ProductId));
                var product = _db.Table<Product>().FirstOrDefault(n => n.Id == item.ProductId);
                if (product == null)
                {
                    _log.Info(string.Format("获取商品信息失败【{0}】！", item.ProductId));
                    throw new FriendlyException(string.Format("获取商品信息失败【{0}】！", item.ProductId));
                }

                Ensure.False(product.HasSNCode && item.Quantity > SystemConfig.ItemMaxSNCodeQuantity, "商品{0}一个明细项最多只能退{1}个串码。".FormatWith(product.Code, SystemConfig.ItemMaxSNCodeQuantity));

                item.ProductCode = product.Code;
                item.Unit = product.Unit;
                // 检查库存
                CheckOrderQuantity(entity, item);

                entity.AddItem(item);
            }
            entity.Amount = entity.GetOrderAmount();
            _db.Insert(entity);
            var history = new ProcessHistory(entity.UpdatedBy, model.EditByName, (int)entity.Status, entity.Id, BillIdentity.StorePurchaseRefundOrder.ToString(), entity.Remark);
            _db.DataBase.ExecuteSql(history.CreateSql(entity.GetType().Name, entity.Code), history);
            _db.SaveChange();

            ////发动单据到SAP

            entity.SetItems(_db.Table<PurchaseOrderItem>().Where(n => n.PurchaseOrderId == entity.Id).ToList());

            this.SubmitToSap(entity, model.EditByName, "创建采购退货单，SAP审核通过");

        }
        public void SubmitToSap(PurchaseOrder purchaseOrder, string EditByName, string remark)
        {
            var order = new Order()
            {
                Code = purchaseOrder.Code,
                OrderType = purchaseOrder.OrderType == PurchaseOrderType.PurchaseReturn ? Guoc.BigMall.Domain.Objects.OrderType.ZP02 : Guoc.BigMall.Domain.Objects.OrderType.ZSM1,
                OrderDate = purchaseOrder.CreatedOn,
                Remark = purchaseOrder.Remark,
                SupplierCode = purchaseOrder.SupplierCode,
                PurchaseGroup = purchaseOrder.OrderType == PurchaseOrderType.PurchaseReturn ? "SM1" : "SM2",
                Items = purchaseOrder.Items.Select(item =>
                {
                    var product = _db.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                    var category = _db.Table<Category>().FirstOrDefault(c => c.Id == product.CategoryId);
                    return new OrderItem()
                    {
                        ItemRow = item.Id,
                        ProductCode = item.ProductCode,
                        CategoryCode = category.Code,
                        Price = item.CostPrice,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        ToStoreCode = purchaseOrder.StoreCode,
                    };
                }).ToList(),
            };
            _sapService.SubmitPurchaseOrder(order);//推送SAP


            //记录SAP单据信息
            purchaseOrder.SapCode = order.SapCode;
            order.Items.ForEach(item =>
            {
                var orderItem = purchaseOrder.Items.Where(n => n.Id == item.ItemRow).FirstOrDefault();
                orderItem.SapRow = item.SapRow;
                //_db.Update<PurchaseOrderItem>(h =>
                //    new PurchaseOrderItem()
                //    {
                //       SapRow=item.SapRow,
                //    },
                //   h=> h.Id == item.ItemRow);
            });

            ////推送成功后更改状态为已审核
            //purchaseOrder.Status = CBPurchaseOrderStatus.Audited;//等待Sap人工审核
            purchaseOrder.IsPushSap = true;
            _db.Update(purchaseOrder);
            _db.Update(purchaseOrder.Items.ToArray());
            var auditedhistory = new ProcessHistory(purchaseOrder.UpdatedBy, EditByName, (int)purchaseOrder.Status, purchaseOrder.Id, BillIdentity.StorePurchaseRefundOrder.ToString(), remark);
            _db.DataBase.ExecuteSql(auditedhistory.CreateSql(purchaseOrder.GetType().Name, purchaseOrder.Code), auditedhistory);
            _db.SaveChange();
        }
        private void CheckOrderQuantity(PurchaseOrder entity, PurchaseOrderItem item)
        {
            if (item.Quantity <= 0)
            {
                throw new FriendlyException(string.Format("{0}商品数量须大于零", item.ProductCode));
            }

            var storeInventory = _db.Table<StoreInventory>().FirstOrDefault(n => n.StoreId == entity.StoreId && n.ProductId == item.ProductId);

            if (storeInventory == null)
                throw new FriendlyException(string.Format("商品{0}不存在", item.ProductCode));

            if (storeInventory.Quantity < item.Quantity)
                throw new FriendlyException(string.Format("商品{0}库存不足", item.ProductCode));

            var storeInventorybatch = _db.DataBase.ExecuteScalar<int>(@"SELECT SUM(Quantity) AS Quantity  FROM dbo.StoreInventoryBatch
								WHERE Quantity>0 AND SupplierId=@SupplierId AND StoreId=@StoreId AND ProductId=@ProductId
								GROUP BY ProductId, StoreId,SupplierId  ", new { SupplierId = entity.SupplierId, StoreId = entity.StoreId, ProductId = item.ProductId });
            if (item.Quantity > storeInventorybatch)
            {
                throw new FriendlyException(string.Format("商品{0}库存不足", item.ProductCode));
            }

        }
        public void RefundEdit(PurchaseOrderModel model)
        {
            var entity = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Id == model.Id);
            if (entity.Status != CBPurchaseOrderStatus.Create && entity.Status != CBPurchaseOrderStatus.Audited)
                throw new FriendlyException("{0}单据不是初始/已审核状态，无法编辑！");
            if (entity.BillType == PurchaseOrderBillType.StockOrder)
                throw new FriendlyException("{0}单据为仓库退货单，无法编辑！");
            entity.UpdatedBy = model.EditBy;
            entity.UpdatedOn = DateTime.Now;
            entity.Status = CBPurchaseOrderStatus.Create;
            //删除明细

            _db.Delete<PurchaseOrderItem>(n => n.PurchaseOrderId == entity.Id);
            //新增明细

            var allItems = JsonConvert.DeserializeObject<List<PurchaseOrderItem>>(model.Items);

            foreach (var item in allItems)
            {
                if (item.Quantity <= 0)
                    throw new FriendlyException(string.Format("商品【{0}】数量必须大于零！", item.ProductId));
                var product = _db.Table<Product>().FirstOrDefault(n => n.Id == item.ProductId);
                if (product == null)
                {
                    _log.Info(string.Format("获取商品信息失败【{0}】！", item.ProductId));
                    throw new FriendlyException(string.Format("获取商品信息失败【{0}】！", item.ProductId));

                }
                item.ProductCode = product.Code;
                item.Unit = product.Unit;
                // 检查库存
                CheckOrderQuantity(entity, item);

                entity.AddItem(item);
            }
            entity.Amount = entity.GetOrderAmount();
            _db.Insert(entity.Items.ToArray());

            var history = new ProcessHistory(entity.UpdatedBy, model.EditByName, (int)entity.Status, entity.Id, BillIdentity.StorePurchaseRefundOrder.ToString(), entity.Remark);
            _db.DataBase.ExecuteSql(history.CreateSql(entity.GetType().Name, entity.Code), history);
            _db.SaveChange();

            ////发动单据到SAP

            this.SubmitToSap(entity, model.EditByName, "编辑采购退货单，SAP审核通过");

        }


        public void StockOut(PurchaseOrderModel model)
        {

            var entity = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Id == model.Id);
            if (entity.Status != CBPurchaseOrderStatus.Audited)
                throw new FriendlyException("不是已审状态无法出库！");
            if (!entity.IsPushSap)
                throw new FriendlyException("单据未推送至Sap无法出库！");
            var entityItems = _db.Table<PurchaseOrderItem>().Where(n => n.PurchaseOrderId == model.Id).ToList();
            if (entity.OrderType == PurchaseOrderType.PurchaseChange)//退货
            {
                entityItems = entityItems.Where(n => n.Quantity < 0).ToList();
            }
            entity.SetItems(entityItems);

            var receiveList = JsonConvert.DeserializeObject<List<PurchaseOrderItem>>(model.Items);
            //检测扫码个数与数量是否匹配
            _istoreInventoryFacade.JudgeSnCodeProduct(receiveList);

            // 更改明细
            entity.UpdateReturnedGoodsItems(receiveList);

            entity.StockIn(model.EditBy);
            // 更改状态
            entity.Status = CBPurchaseOrderStatus.OutStock;
            var reason = "换货出库";
            var billIdentity = BillIdentity.StorePurchaseChange;
            if (entity.OrderType == PurchaseOrderType.PurchaseReturn)//退货
            {
                entity.Status = CBPurchaseOrderStatus.Finished;
                reason = "退货出库";
                billIdentity = entity.BillType == PurchaseOrderBillType.StoreOrder ? BillIdentity.StorePurchaseRefundOrder : BillIdentity.StockPurchaseRefundOrder;
            }

            //更新价格
            // this.UpdatePurchasePrice(entity);
            entity.IsPushSap = false;
            _db.Update(entity);
            // 记录流程日志
            _processHistoryService.Track(model.EditBy, model.EditByName, (int)entity.Status, entity.Id, billIdentity.ToString(), reason);
            // 出库
            var result = this.StockOut(entity, billIdentity);


            _db.SaveChange();
            ////通知SAP 出库成功
            ProcessHistory phistory = new ProcessHistory()
            {
                CreatedBy = model.EditBy,
                CreatedByName = model.EditByName,
                CreatedOn = DateTime.Now,
                FormId = entity.Id,
                FormType = billIdentity.ToString(),
                Status = (int)entity.Status,
                Remark = reason
            };
            this.SapStockIn(entity, result, phistory, false);


        }
        public List<StoreInventoryHistorySAP> StockOut(PurchaseOrder entity, BillIdentity billIdentity)
        {
            if (entity == null)
                throw new FriendlyException("单据不存在");
            if (entity.Items.Count() == 0)
                throw new FriendlyException("单据明细为空");
            var store = _db.Table<Store>().FirstOrDefault(n => n.Id == entity.StoreId);
            if (store == null)
                throw new FriendlyException("未获取门店信息！");

            var inventoryUpdates = new List<StoreInventory>();
            var inventoryBatchUpdates = new List<StoreInventoryBatch>();
            var inventoryHistorys = new List<StoreInventoryHistory>();
            var sapHistorys = new List<StoreInventoryHistorySAP>();
            var sapCode = long.Parse(_sequenceService.GenerateNewCode(BillIdentity.SapHistoryOrder)).ToString();
            var batchNo = long.Parse(_sequenceService.GenerateNewCode(BillIdentity.BatchNo));
            foreach (var item in entity.Items)
            {
                var inventory = _db.Table<StoreInventory>().FirstOrDefault(n => n.ProductId == item.ProductId && n.StoreId == entity.StoreId);
                if (inventory == null)
                    throw new FriendlyException(string.Format("商品{0}不存在", item.ProductCode));
                var quantity = Math.Abs(item.Quantity);//退货时是负数
                if (quantity > inventory.Quantity)
                    throw new FriendlyException(string.Format("商品{0}库存不足", item.ProductCode));

                //记录原库存
                var sourceInventoryQuantity = inventory.Quantity;

                // 减总库存
                inventory.Quantity -= quantity;
                inventoryUpdates.Add(inventory);
                //批次库存
                var inventoryBatch = _db.Table<StoreInventoryBatch>().Where(n => n.SupplierId == entity.SupplierId && n.StoreId == entity.StoreId && n.ProductId == item.ProductId && n.Quantity > 0).ToList();
                if (inventoryBatch.Count == 0)
                    throw new FriendlyException(string.Format("商品{0}不存在", item.ProductId));
                var batchQuantity = inventoryBatch.Sum(n => n.Quantity);
                if (quantity > batchQuantity)
                    throw new FriendlyException(string.Format("商品{0}库存不足", item.ProductCode));
                item.BatchNo = batchNo;
                if (!item.IsSnCode) //非串码商品
                {
                    //减批次库存
                    var batchProducts = inventoryBatch.OrderBy(n => n.CreatedOn);
                    var leftQuantity = quantity;
                    foreach (var batchProduct in batchProducts)
                    {
                        if (batchProduct.Quantity >= leftQuantity)
                        {
                            batchProduct.Quantity -= leftQuantity;
                            inventoryBatchUpdates.Add(batchProduct);
                            // 库存扣减历史记录：
                            var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, sourceInventoryQuantity, (-1) * quantity, item.CostPrice, item.BatchNo, entity.Id, entity.Code, billIdentity, entity.UpdatedBy, entity.SupplierId, null);
                            history.StoreInventoryBatchId = batchProduct.Id;
                            inventoryHistorys.Add(history);
                            break;
                        }
                        else
                        {
                            leftQuantity -= batchProduct.Quantity;
                            batchProduct.Quantity = 0;
                            inventoryBatchUpdates.Add(batchProduct);
                            // 库存扣减历史记录：
                            var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, sourceInventoryQuantity, (-1) * batchProduct.Quantity, item.CostPrice, item.BatchNo, entity.Id, entity.Code, billIdentity, entity.UpdatedBy, entity.SupplierId, null);
                            history.StoreInventoryBatchId = batchProduct.Id;
                            inventoryHistorys.Add(history);
                        }
                    }

                }
                else     ///串码
                {

                    var sncodes = item.SNCodes.Split(',');
                    foreach (var code in sncodes)
                    {
                        if (string.IsNullOrWhiteSpace(code))
                            continue;
                        var batchProducts = inventoryBatch.FirstOrDefault(n => n.SNCode == code);
                        if (batchProducts == null)
                            throw new FriendlyException(string.Format("商品{0}未找到串码{1}！", item.ProductCode, code));
                        batchProducts.Quantity = 0;
                        inventoryBatchUpdates.Add(batchProducts);
                        // 库存扣减历史记录：
                        var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, sourceInventoryQuantity, (-1), item.CostPrice, item.BatchNo, entity.Id, entity.Code, billIdentity, entity.UpdatedBy, entity.SupplierId, code);
                        inventoryHistorys.Add(history);
                    }
                }

                // 库存扣减历史记录：
                var sap = new StoreInventoryHistorySAP(sapCode, StoreInventoryHistorySapType.OutStock, item.ProductId, item.ProductCode, entity.StoreId, store.Code, quantity, item.SNCodes, entity.Code, billIdentity, entity.UpdatedBy);
                sap.BillItemId = item.Id;
                sapHistorys.Add(sap);
                item.IsSnCode = false;
                item.SNQuantity = 0;
                item.SNCodes = null;

            }
            _db.Update(entity.Items.ToArray());
            _db.Update(inventoryUpdates.ToArray());
            _db.Update(inventoryBatchUpdates.ToArray());
            _db.Insert(inventoryHistorys.ToArray());
            _db.Insert(sapHistorys.ToArray());
            return sapHistorys;
        }


        public void UpdatePurchasePrice(PurchaseOrder entity)
        {
            foreach (var item in entity.Items)
            {
                var productPurchasePrice = _purchasePriceService.QueryCurrentPurchasePrice(item.ProductId);

                if (productPurchasePrice != null)
                    item.CostPrice = productPurchasePrice.PurchasePrice;
            }
        }


        public void ChangeCreate(PurchaseOrderModel model)
        {
            var entity = model.MapTo<PurchaseOrder>();

            entity.CreatedBy = model.EditBy;
            entity.UpdatedBy = model.EditBy;

            var allItems = JsonConvert.DeserializeObject<List<PurchaseOrderItem>>(model.Items);


            var store = _db.Table<Store>().FirstOrDefault(n => n.Id == entity.StoreId);
            if (store == null)
            {
                throw new FriendlyException("获取门店信息失败！");
            }
            var stockPurchase = store.Code.ToLower().StartsWith("a");
            if (stockPurchase)//A开头的仓库是总仓，其它为门店  根据编码来区分的，目前订的规则是A开头的是总仓，M开头的是门店
            {
                throw new FriendlyException("您选择的门店是总仓，商玛特只能创建门店换货单！");
            }
            var supplier = _db.Table<Supplier>().FirstOrDefault(n => n.Id == entity.SupplierId);
            if (supplier == null)
            {
                throw new FriendlyException("获取供应商信息失败！");
            }
            entity.StoreCode = store.Code;
            entity.SupplierCode = supplier.Code;
            entity.BillType = PurchaseOrderBillType.StoreOrder;
            entity.Code = _sequenceService.GenerateNewCode(BillIdentity.StorePurchaseChange);
            List<PurchaseOrderItem> others = new List<PurchaseOrderItem>();
            foreach (var item in allItems)
            {
                if (item.Quantity <= 0)
                    throw new FriendlyException(string.Format("商品【{0}】数量必须大于零！", item.ProductId));
                var product = _db.Table<Product>().FirstOrDefault(n => n.Id == item.ProductId);
                if (product == null)
                {
                    _log.Info(string.Format("获取商品信息失败【{0}】！", item.ProductId));
                    throw new FriendlyException(string.Format("获取商品信息失败【{0}】！", item.ProductId));

                }
                item.ProductCode = product.Code;
                item.Unit = product.Unit;
                // 检查库存
                CheckOrderQuantity(entity, item);
                var otherItem = item.Clone();
                otherItem.Quantity *= (-1);
                entity.AddItem(item);
                others.Add(otherItem);
            }
            entity.AddItemsForChange(others);

            _db.Insert(entity);
            var history = new ProcessHistory(entity.UpdatedBy, model.EditByName, (int)entity.Status, entity.Id, BillIdentity.StorePurchaseChange.ToString(), entity.Remark);
            _db.DataBase.ExecuteSql(history.CreateSql(entity.GetType().Name, entity.Code), history);
            _db.SaveChange();

        }


        public void ChangeEdit(PurchaseOrderModel model)
        {

            var entity = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Id == model.Id);
            if (entity.Status != CBPurchaseOrderStatus.Create && entity.Status != CBPurchaseOrderStatus.Reject)
                throw new FriendlyException("{0}单据不是初始/驳回/状态，无法编辑！");

            entity.UpdatedBy = model.EditBy;
            entity.UpdatedOn = DateTime.Now;
            entity.Status = CBPurchaseOrderStatus.Create;
            //删除明细

            _db.Delete<PurchaseOrderItem>(n => n.PurchaseOrderId == entity.Id);
            //新增明细

            var allItems = JsonConvert.DeserializeObject<List<PurchaseOrderItem>>(model.Items);
            List<PurchaseOrderItem> others = new List<PurchaseOrderItem>();
            foreach (var item in allItems)
            {
                if (item.Quantity <= 0)
                    throw new FriendlyException(string.Format("商品【{0}】数量必须大于零！", item.ProductId));
                var product = _db.Table<Product>().FirstOrDefault(n => n.Id == item.ProductId);
                if (product == null)
                {
                    _log.Info(string.Format("获取商品信息失败【{0}】！", item.ProductId));
                    throw new FriendlyException(string.Format("获取商品信息失败【{0}】！", item.ProductId));
                }
                item.ProductCode = product.Code;
                item.Unit = product.Unit;
                // 检查库存
                CheckOrderQuantity(entity, item);

                var otherItem = item.Clone();
                otherItem.Quantity *= (-1);
                entity.AddItem(item);
                others.Add(otherItem);
            }
            entity.AddItemsForChange(others);
            _db.Insert(entity.Items.ToArray());
            _db.Update(entity);

            var history = new ProcessHistory(entity.UpdatedBy, model.EditByName, (int)entity.Status, entity.Id, BillIdentity.StorePurchaseChange.ToString(), entity.Remark);
            _db.DataBase.ExecuteSql(history.CreateSql(entity.GetType().Name, entity.Code), history);
            _db.SaveChange();

        }


        public void Audit(int id, int editBy, string editor, string reason)
        {
            var entity = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Id == id);
            if (entity.Status != CBPurchaseOrderStatus.Create)
            {
                throw new FriendlyException(string.Format("{0}单据非初始状态不可进行审核操作！", entity.Code));

            }

            entity.UpdatedBy = editBy;
            entity.UpdatedOn = DateTime.Now;
            entity.Remark = reason;
            entity.Status = CBPurchaseOrderStatus.Audited;
            entity.IsPushSap = false;
            _db.Update(entity);
            _db.SaveChange();
            ////发动单据到SAP
            entity.SetItems(_db.Table<PurchaseOrderItem>().Where(n => n.PurchaseOrderId == entity.Id).ToList());
            this.SubmitToSap(entity, editor, "采购换货单审核，推送SAP成功");

        }

        public void Reject(int id, int editBy, string editor, string reason)
        {
            var entity = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Id == id);
            if (entity.Status != CBPurchaseOrderStatus.Create)
            {
                throw new FriendlyException(string.Format("{0}单据非初始状态不可驳回操作！", entity.Code));
            }
            entity.Status = CBPurchaseOrderStatus.Reject;
            entity.UpdatedBy = editBy;
            entity.UpdatedOn = DateTime.Now;
            entity.Remark = reason;
            _db.Update(entity);

            _processHistoryService.Track(editBy, editor, (int)entity.Status, entity.Id, BillIdentity.StorePurchaseChange.ToString(), reason);
            _db.SaveChange();
        }


        public void ChangeCancel(int id, int editBy, string editor, string reason)
        {
            var entity = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Id == id);

            if (entity.Status == CBPurchaseOrderStatus.Audited)//需发送至SAP
            {
                //// 作废
                //_sapService.Cance(ntity.Code);
            }
            entity.Remark = reason;
            entity.Cancel(editBy);
            _db.Update(entity);
            _processHistoryService.Track(editBy, editor, (int)entity.Status, entity.Id, BillIdentity.StorePurchaseChange.ToString(), reason);

            _db.SaveChange();
        }


        public PurchaseOrderDto GetReceiveById(int id)
        {
            string sql = @"SELECT  t0.Id ,
        t0.Code ,
        t1.Id AS SupplierId ,
        t0.SupplierCode ,
        t2.Id AS StoreId ,
        t2.Code as StoreCode ,
        t0.CreatedOn ,
        t3.NickName AS CreatedByName ,
        t0.Status ,
        t1.Code AS SupplierCode ,
        t1.Name AS SupplierName ,
        t2.Name AS StoreName ,
        t0.Remark ,
        t0. BillType , t0.UpdatedOn ,
        t4.NickName AS UpdatedByName ,
        t0.SapCode AS SapOrderId ,t2.Address,t2.Phone,t0.ordertype
FROM    PurchaseOrder t0
        LEFT JOIN Supplier t1 ON t0.SupplierId = t1.Id
        LEFT JOIN Store t2 ON t0.StoreId = t2.Id
        LEFT JOIN Account t3 ON t3.Id = t0.CreatedBy
        LEFT  JOIN Account t4 on t4.Id=t0.UpdatedBy
where t0.Id= @Id ";
            var model = _db.DataBase.QuerySingle<PurchaseOrderDto>(sql, new { Id = id });

            string sqlitem = @"SELECT  i.* ,
        p.Name AS ProductName ,
        p.Code AS ProductCode ,
        p.Spec AS Specification ,
        p.Unit ,p.Id  as ProductId,t.InventoryQuantity,P.HasSNCode
FROM    PurchaseOrderItem i
       INNER JOIN Product p ON i.ProductId = p.Id
       INNER JOIN  dbo.PurchaseOrder pr ON pr.Id=i.PurchaseOrderId
       LEFT JOIN  ( SELECT  b.StoreId ,
					b.SupplierId ,
					b.ProductId ,
					SUM(b.Quantity) AS InventoryQuantity
					FROM    StoreInventoryBatch b
					GROUP BY b.StoreId ,
					b.SupplierId ,
					b.ProductId
					)  t ON t.SupplierId=pr.SupplierId AND t.StoreId=pr.StoreId AND  t.ProductId=i.ProductId
WHERE  i.Quantity>i.ActualQuantity and  i.PurchaseOrderId = @Id";


            var productItems = _db.DataBase.Query<PurchaseOrderItemDto>(sqlitem, new { Id = id, StoreId = model.StoreId }).ToList();
            if (productItems.Count == 0)
            {
                productItems.Add(new PurchaseOrderItemDto());
            }

            model.Items = productItems;

            return model;
        }


        public void ChangeStockIn(PurchaseOrderModel model)
        {
            var entity = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Id == model.Id);
            if (entity.Status != CBPurchaseOrderStatus.OutStock)
            {
                throw new Exception("出库状态才能操作入库！");
            }
            if (!entity.IsPushSap)
                throw new FriendlyException("单据出库时未推送至Sap，无法操作入库！");
            entity.UpdatedBy = model.EditBy;
            entity.UpdatedOn = DateTime.Now;
            var entityItems = _db.Table<PurchaseOrderItem>().Where(n => n.PurchaseOrderId == model.Id).ToList();
            entity.SetItems(entityItems);

            var receiveList = JsonConvert.DeserializeObject<List<PurchaseOrderItem>>(model.Items);

            this.JudgeChangeProduct(receiveList, entity.Code);

            // 更改收货明细
            entity.UpdateChangedGoodsItems(receiveList);


            // 更改入库状态

            entity.Status = CBPurchaseOrderStatus.Finished;
            entity.IsPushSap = false;
            _db.Update(entity);
            // 记录流程日志
            var reason = "收货入库";
            var billIdentity = entity.BillType == PurchaseOrderBillType.StoreOrder ? BillIdentity.StorePurchaseOrder : BillIdentity.StockPurchaseOrder;

            _processHistoryService.Track(model.EditBy, model.EditByName, (int)entity.Status, entity.Id, billIdentity.ToString(), reason);
            // 入库
            var result = this.ChangeStockIn(entity);

            _db.SaveChange();
            ////通知SAP 出库成功
            ProcessHistory phistory = new ProcessHistory()
            {
                CreatedBy = model.EditBy,
                CreatedByName = model.EditByName,
                CreatedOn = DateTime.Now,
                FormId = entity.Id,
                FormType = billIdentity.ToString(),
                Status = (int)entity.Status,
                Remark = reason
            };
            this.SapStockIn(entity, result, phistory, true);

        }
        //检测换货入库的数量,串码
        public void JudgeChangeProduct(List<PurchaseOrderItem> changeList, string billCode)
        {    //数量
            this.ChangeStockInCheckCounts(changeList);
            //sncode 检测
            var checkSncode = changeList.Where(x => x.SNQuantity > 0 && x.IsSnCode == true);
            foreach (var item in checkSncode)
            {
                if (item.Quantity > 0)
                {
                    this.ChangeDirectStockInSncodesCheck(item.SNCodes, item.ProductCode);///、直接入库
                }
                else
                {//驳回入库
                    this.ChangeRejectStockInSncodesCheck(item.SNCodes, item.ProductCode, billCode);
                }

            }
            //检测扫码个数与数量是否匹配
            var checklist = changeList.Where(n => n.SNQuantity > 0).ToList();
            _istoreInventoryFacade.JudgeSnCodeProduct(checklist);
        }
        public void ChangeStockInCheckCounts(List<PurchaseOrderItem> changeList)
        {
            var productlist = changeList.Where(n => n.Quantity > 0);
            foreach (var item in productlist)
            {
                var otherItem = changeList.FirstOrDefault(n => n.ProductId == item.ProductId && n.Quantity < 0);
                var otherQty = otherItem == null ? 0 : otherItem.SNQuantity;
                var qtyCounts = item.SNQuantity + otherQty;
                if (item.Quantity != qtyCounts)
                {
                    throw new FriendlyException(string.Format("商品{0}入库数量不正确，请确认后操作！", item.ProductCode));
                }


            }
        }

        public void ChangeDirectStockInSncodesCheck(string snCodes, string productCode)
        {
            var sncodes = snCodes.Split(',');
            foreach (var sncode in sncodes)
            {
                if (_db.Table<StoreInventoryBatch>().Exists(x => x.SNCode == sncode))
                {
                    throw new FriendlyException(string.Format("商品{0}的串码{1}已存在，无法入库！", productCode, sncode));
                }
            }
        }
        public void ChangeRejectStockInSncodesCheck(string snCodes, string productCode, string billCode)
        {
            var historys = _db.Table<StoreInventoryHistory>().Where(n => n.BillCode == billCode && n.BillType == BillIdentity.StorePurchaseChange && n.ChangeQuantity < 0);
            if (historys == null)
                throw new FriendlyException("该单据未找到出库记录");

            var sncodes = snCodes.Split(',');
            foreach (var sncode in sncodes)
            {
                if (!historys.Exists(x => x.SNCode == sncode))
                //if (!_db.Table<StoreInventoryHistory>().Exists(n => n.BillCode == billCode && n.BillType == BillIdentity.StorePurchaseChange&&n.ChangeQuantity<0&&n.SNCode == sncode))
                {
                    throw new FriendlyException(string.Format("商品{0}的串码{1}不是出库的串码，无法入库！", productCode, sncode));
                }
            }
        }
        public List<StoreInventoryHistorySAP> ChangeStockIn(PurchaseOrder entity)
        {

            if (entity == null) { throw new FriendlyException("单据不存在"); }
            if (entity.Items.Count() == 0) { throw new FriendlyException("单据明细为空"); }


            var batchNo = long.Parse(_sequenceService.GenerateNewCode(BillIdentity.BatchNo));

            var billIdentity = entity.BillType == PurchaseOrderBillType.StoreOrder ? BillIdentity.StorePurchaseOrder : BillIdentity.StockPurchaseOrder;
            var sapCode = long.Parse(_sequenceService.GenerateNewCode(BillIdentity.SapHistoryOrder)).ToString();


            ///直接入库
            return this.ChangeDirectStockIn(entity, batchNo, billIdentity, sapCode);

            //加回原来扣减的批次上  ---业务确认  换单的商品 必须全换
            //hangeReturnStockIn(entity,  batchNo, billIdentity,sapCode);






        }

        public List<StoreInventoryHistorySAP> ChangeDirectStockIn(PurchaseOrder entity, long batchNo, BillIdentity billIdentity, string sapCode)
        {
            var items = entity.Items.Where(n => n.Quantity > 0 && n.SNQuantity > 0).ToList();
            var inventorys = new List<StoreInventory>();
            var inventoryBatchs = new List<StoreInventoryBatch>();
            var inventoryHistorys = new List<StoreInventoryHistory>();
            var sapHistorys = new List<StoreInventoryHistorySAP>();

            foreach (var item in items)
            {
                var inventory = _db.Table<StoreInventory>().FirstOrDefault(n => n.ProductId == item.ProductId && n.StoreId == entity.StoreId);
                if (inventory == null)
                    throw new FriendlyException(string.Format("商品{0}不存在", item.ProductCode));

                // 回写采购入库批次
                item.BatchNo = batchNo;
                // 入库      
                var inventoryQuantity = inventory.Quantity;
                inventory.Quantity += item.SNQuantity;
                inventory.SaleQuantity += item.SNQuantity;
                inventory.AvgCostPrice = CalculatedAveragePrice(inventory.AvgCostPrice, inventoryQuantity, item.CostPrice, item.SNQuantity);  // 修改库存均价
                inventory.LastCostPrice = item.CostPrice > 0 ? item.CostPrice : inventory.LastCostPrice;
                inventorys.Add(inventory);

                //记录库存批次
                if (!item.IsSnCode) //非串码商品
                {

                    var batch = new StoreInventoryBatch(item.ProductId, entity.StoreId, entity.SupplierId, item.SNQuantity, item.CostPrice, item.CostPrice, batchNo, null, 0, entity.UpdatedBy, null);
                    batch.PurchaseQuantity = item.Quantity;
                    inventoryBatchs.Add(batch);
                    //记录库存流水
                    var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, inventoryQuantity, item.SNQuantity, item.CostPrice, batchNo, entity.Id, entity.Code, billIdentity, entity.UpdatedBy, entity.SupplierId, null);
                    inventoryHistorys.Add(history);
                }
                else     ///记录采购批次串码
                {

                    var sncodes = item.SNCodes.Split(',');
                    foreach (var code in sncodes)
                    {
                        if (_db.Table<StoreInventoryBatch>().Exists(n => n.SNCode == code))
                            throw new FriendlyException(string.Format("商品:{0}的串码[{1}]已存在，请确认后操作！", item.ProductCode, code));
                        var batch = new StoreInventoryBatch(item.ProductId, entity.StoreId, entity.SupplierId, 1, item.CostPrice, item.CostPrice, batchNo, null, 0, entity.UpdatedBy, code);
                        batch.PurchaseQuantity = 1;
                        inventoryBatchs.Add(batch);

                        //记录库存流水
                        var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, inventoryQuantity, 1, item.CostPrice, batchNo, entity.Id, entity.Code, billIdentity, entity.UpdatedBy, entity.SupplierId, code);
                        inventoryHistorys.Add(history);
                    }
                }
                //记录库存流水(SAP)
                var sap = new StoreInventoryHistorySAP(sapCode, StoreInventoryHistorySapType.InStock, item.ProductId, item.ProductCode, entity.StoreId, entity.StoreCode, item.Quantity, item.SNCodes, entity.Code, billIdentity, entity.UpdatedBy);
                sap.BillItemId = item.Id;
                sapHistorys.Add(sap);


                item.IsSnCode = false;
                item.SNQuantity = 0;
                item.SNCodes = null;
            }
            _db.Update(entity.Items.ToArray());
            _db.Update(inventorys.ToArray());
            _db.Insert(inventoryHistorys.ToArray());
            _db.Insert(inventoryBatchs.ToArray());
            _db.Insert(sapHistorys.ToArray());
            return sapHistorys;
        }

        public void ChangeReturnStockIn(PurchaseOrder entity, long batchNo, BillIdentity billIdentity, string sapCode)
        {
            var items = entity.Items.Where(n => n.Quantity < 0 && n.SNQuantity > 0).ToList();
            var inventorys = new List<StoreInventory>();
            var inventoryBatchs = new List<StoreInventoryBatch>();
            var inventoryHistorys = new List<StoreInventoryHistory>();
            var sapHistorys = new List<StoreInventoryHistorySAP>();

            foreach (var item in items)
            {
                var inventory = _db.Table<StoreInventory>().FirstOrDefault(n => n.ProductId == item.ProductId && n.StoreId == entity.StoreId);
                if (inventory == null)
                    throw new FriendlyException(string.Format("商品{0}不存在", item.ProductCode));

                // 回写采购入库批次
                item.BatchNo = batchNo;
                // 入库      
                var inventoryQuantity = inventory.Quantity;
                inventory.Quantity += item.SNQuantity;
                inventory.SaleQuantity += item.SNQuantity;
                inventorys.Add(inventory);

                //记录库存批次
                if (!item.IsSnCode) //非串码商品
                {
                    //var storeInventoryBatchId=_db.Table<StoreInventoryHistory>().FirstOrDefault(n => n.BillCode == entity.Code && n.ProductId == item.ProductId && n.ChangeQuantity < 0).StoreInventoryBatchId;
                    //var storeInventory=_db.Table<StoreInventoryBatch>().FirstOrDefault(n => n.Id == storeInventoryBatchId);
                    //storeInventory.Quantity += item.SNQuantity;
                    //inventoryBatchs.Add(storeInventory);
                    var storeInventoryHistorys = _db.Table<StoreInventoryHistory>().Where(n => n.BillCode == entity.Code && n.ProductId == item.ProductId && n.ChangeQuantity < 0).OrderByDesc(X => X.CreatedOn).ToList();
                    var backQuantity = item.SNQuantity;
                    foreach (var inventoryHistory in storeInventoryHistorys)
                    {
                        var storeInventory = _db.Table<StoreInventoryBatch>().FirstOrDefault(n => n.Id == inventoryHistory.StoreInventoryBatchId);
                        if (Math.Abs(inventoryHistory.ChangeQuantity) >= backQuantity)
                        {
                            storeInventory.Quantity += backQuantity;
                            inventoryBatchs.Add(storeInventory);
                            break;
                        }
                        else
                        {
                            backQuantity += inventoryHistory.ChangeQuantity;
                            storeInventory.Quantity -= inventoryHistory.ChangeQuantity;
                            inventoryBatchs.Add(storeInventory);
                        }
                    }

                    //记录库存流水
                    var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, inventoryQuantity, item.SNQuantity, item.CostPrice, batchNo, entity.Id, entity.Code, billIdentity, entity.UpdatedBy, entity.SupplierId, null);
                    inventoryHistorys.Add(history);
                }
                else     ///记录采购批次串码
                {

                    var sncodes = item.SNCodes.Split(',');
                    foreach (var code in sncodes)
                    {

                        var storeInventoryBatchId = _db.Table<StoreInventoryHistory>().FirstOrDefault(n => n.BillCode == entity.Code && n.SNCode == code && n.ChangeQuantity < 0).StoreInventoryBatchId;
                        if (storeInventoryBatchId == 0)
                            throw new FriendlyException(string.Format("未找到单据{0}中商品串码为{1}的出库记录，无法入库", entity.Code, code));
                        var storeInventory = _db.Table<StoreInventoryBatch>().FirstOrDefault(n => n.Id == storeInventoryBatchId);
                        storeInventory.Quantity = 1;
                        inventoryBatchs.Add(storeInventory);

                        //记录库存流水
                        var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, inventoryQuantity, 1, item.CostPrice, batchNo, entity.Id, entity.Code, billIdentity, entity.UpdatedBy, entity.SupplierId, code);
                        inventoryHistorys.Add(history);
                    }
                }
                //记录库存流水(SAP)
                var sap = new StoreInventoryHistorySAP(sapCode, StoreInventoryHistorySapType.InStock, item.ProductId, item.ProductCode, entity.StoreId, entity.StoreCode, item.Quantity, item.SNCodes, entity.Code, billIdentity, entity.UpdatedBy);
                sap.BillItemId = item.Id;
                sapHistorys.Add(sap);

                item.IsSnCode = false;
                item.SNQuantity = 0;
                item.SNCodes = null;
            }
            _db.Update(entity.Items.ToArray());
            _db.Update(inventorys.ToArray());
            _db.Update(inventoryBatchs.ToArray());
            _db.Insert(inventoryHistorys.ToArray());
            _db.Insert(sapHistorys.ToArray());
        }



        public void Close(int id)
        {
            var order = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Id == id);
            if (order == null)
                throw new FriendlyException("获取单据信息失败");
            if (order.Status != CBPurchaseOrderStatus.Audited)
                throw new FriendlyException("非审核状态无法关单");
            var limteDate = order.CreatedOn.AddDays(7);
            if (DateTime.Now.CompareTo(limteDate) <= 0)
                throw new FriendlyException("未到关单时间！");
            if (string.IsNullOrWhiteSpace(order.SapCode))
                throw new FriendlyException("sap编码不存在！");
            _sapService.ClosePurchaseOrder(order.SapCode);

            order.Status = CBPurchaseOrderStatus.Closed;
            _db.Update<PurchaseOrder>(order);
            _db.SaveChange();

        }


        public void InitStockIn(string codes)
        {
            string[] data = codes.Trim('\n').Split('\n');
            foreach (var code in data)
            {
                try
                {
                    var itemCode = code.Trim();
                    var entity = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Code == itemCode);
                    if (entity == null) { throw new FriendlyException(string.Format("{0}单据为空", code)); }
                    var entityItems = _db.Table<PurchaseOrderItem>().Where(n => n.PurchaseOrderId == entity.Id).ToList();
                    entity.SetItems(entityItems);

                    if (entity.Status == CBPurchaseOrderStatus.Finished)
                    {
                        throw new FriendlyException(string.Format("{0}单据已入库", code));
                    }

                    InitStockIn(entity);
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
            }
        }

        public void InitStockIn(PurchaseOrder entity)
        {
            if (entity == null) { throw new FriendlyException("单据不存在"); }
            if (entity.Items.Count() == 0) { throw new FriendlyException("单据明细为空"); }

            var store = _db.Table<Store>().FirstOrDefault(n => n.Id == entity.StoreId);
            if (store == null)
                throw new FriendlyException("未获取门店信息！");
            var inventorys = new List<StoreInventory>();
            var inventoryAdds = new List<StoreInventory>();
            var inventoryBatchs = new List<StoreInventoryBatch>();
            var inventoryHistorys = new List<StoreInventoryHistory>();
            var batchNo = long.Parse(_sequenceService.GenerateNewCode(BillIdentity.BatchNo));
            var sapHistorys = new List<StoreInventoryHistorySAP>();
            var billIdentity = entity.BillType == PurchaseOrderBillType.StoreOrder ? BillIdentity.StorePurchaseOrder : BillIdentity.StockPurchaseOrder;
            var historyCode = long.Parse(_sequenceService.GenerateNewCode(BillIdentity.SapHistoryOrder)).ToString();
            foreach (var item in entity.Items)
            {
                if (item.SNQuantity == 0)
                    continue;
                var inventoryQuantity = 0;
                var inventory = _db.Table<StoreInventory>().FirstOrDefault(n => n.ProductId == item.ProductId && n.StoreId == entity.StoreId);
                if (inventory == null)
                {
                    var avgCostPrice = this.CalculatedAveragePrice(0, 0, item.CostPrice, item.SNQuantity);
                    inventoryAdds.Add(new StoreInventory(entity.StoreId, item.ProductId, item.SNQuantity, avgCostPrice));
                }
                else
                {
                    // 入库      
                    inventoryQuantity = inventory.Quantity;
                    inventory.Quantity += item.SNQuantity;
                    inventory.SaleQuantity += item.SNQuantity;
                    inventory.AvgCostPrice = CalculatedAveragePrice(inventory.AvgCostPrice, inventoryQuantity, item.CostPrice, item.SNQuantity);  // 修改库存均价
                    inventory.LastCostPrice = item.CostPrice > 0 ? item.CostPrice : inventory.LastCostPrice;
                    inventorys.Add(inventory);
                }
                // 回写采购入库批次
                item.BatchNo = batchNo;


                //记录库存批次
                if (!item.IsSnCode) //非串码商品
                {

                    var batch = new StoreInventoryBatch(item.ProductId, entity.StoreId, entity.SupplierId, item.SNQuantity, item.CostPrice, item.CostPrice, batchNo, null, 0, entity.UpdatedBy, null);
                    batch.PurchaseQuantity = item.Quantity;
                    inventoryBatchs.Add(batch);
                    //记录库存流水
                    var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, inventoryQuantity, item.SNQuantity, item.CostPrice, batchNo, entity.Id, entity.Code, billIdentity, entity.UpdatedBy, entity.SupplierId, null);
                    inventoryHistorys.Add(history);
                }
                else     ///记录采购批次串码
                {

                    var sncodes = item.SNCodes.Split(',');
                    foreach (var code in sncodes)
                    {
                        if (_db.Table<StoreInventoryBatch>().Exists(n => n.SNCode == code))
                            throw new FriendlyException(string.Format("商品:{0}的串码[{1}]已存在，请确认后操作！", item.ProductCode, code));
                        var batch = new StoreInventoryBatch(item.ProductId, entity.StoreId, entity.SupplierId, 1, item.CostPrice, item.CostPrice, batchNo, null, 0, entity.UpdatedBy, code);
                        batch.PurchaseQuantity = 1;
                        inventoryBatchs.Add(batch);

                        //记录库存流水
                        var history = new StoreInventoryHistory(item.ProductId, entity.StoreId, inventoryQuantity, 1, item.CostPrice, batchNo, entity.Id, entity.Code, billIdentity, entity.UpdatedBy, entity.SupplierId, code);
                        inventoryHistorys.Add(history);
                    }
                }



            }
            entity.Status = CBPurchaseOrderStatus.Finished;
            _db.Update(entity);
            _db.Update(entity.Items.ToArray());
            _db.Update(inventorys.ToArray());
            _db.Insert(inventoryAdds.ToArray());
            _db.Insert(inventoryHistorys.ToArray());
            _db.Insert(inventoryBatchs.ToArray());
            _db.SaveChange();
        }
    }
}
