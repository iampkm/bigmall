using System.Collections.Generic;
using System.Linq;

using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;
using Guoc.BigMall.Infrastructure.Utils;
using Guoc.BigMall.Infrastructure;
using System;

namespace Guoc.BigMall.Application.Facade
{
    public class ReturnBatchSaleOrderFacade : SaleOrderFacadeBase, IReturnBatchSaleOrderFacade
    {
        private ISAPService _sapService;

        #region ctor
        public ReturnBatchSaleOrderFacade(
            IDBContext db,
            IContextFacade context,
            ILogger log,
            IProductPriceFacade productPriceFacade,
            IProductFacade productFacade,
            IStoreInventoryFacade storeInventory,
            ISupplierFacade supplierFacade,
            IStoreFacade storeFacade,
            IStoreInventoryFacade storeInventoryFacade,
            ISAPService sapService)
            : base(db, context, log, productPriceFacade, productFacade, storeInventory, supplierFacade, storeInventoryFacade, storeFacade)
        {
            _sapService = sapService;
        }
        #endregion

        #region Interface Implements
        public void CreateReturnBatchSaleOrder(SaleOrderModel model)
        {
            _log.Info("开始创批发退货单");

            model.OrderType = OrderType.Return.Value();
            model.BillType = SaleOrderBillType.BatchOrder;
            model.Status = SaleOrderStatus.Create;
            model.RoStatus = ReturnSaleOrderStatus.Create.Value();

            this.CheckSaleOrderStatus(model);

            this.CheckReturnProduct(model);

            this.CheckProductHasReturned(model);

            var orderGifts = model.Items.SelectMany(x => x.GiftItems).ToList();
            orderGifts.ForEach(x => x.GiftQuantity = x.GiftReturnQuantity);

            var order = this.DoCreateSaleOrder(model);
            _db.Insert(order);

            //this.InvokeSapService(order, InvokeSapServiceType.OrderCreated);

            var history = this.DoCreateHistory(model, order);
            _db.DataBase.AddExecute(history.CreateSql("SaleOrder", order.Code), history);

            _db.SaveChange();

            _log.Info("批发退货单:{0} 创建成功!", order.Code);
        }

        public SaleOrderModel GetReturnBatchSaleOrderByCode(string code)
        {
            var condition = new SearchSaleOrder() { OrderCode = code, OrderType = OrderType.Return.Value(), BillType = SaleOrderBillType.BatchOrder.Value() };
            var model = base.GetSaleOrderModel(condition);

            return model;
        }

        public SaleOrderModel GetReturnBatchSaleOrderBySaleCode(string saleCode)
        {
            var condition = new SearchSaleOrder() { SourceSaleOrderCode = saleCode, OrderType = OrderType.Return.Value(), BillType = SaleOrderBillType.BatchOrder.Value() };
            var model = base.GetSaleOrderModel(condition);

            return model;
        }

        public IEnumerable<SaleOrderModel> GetReturnBatchSaleOrders(Pager page, SearchSaleOrder condition)
        {
            condition.OrderType = OrderType.Return.Value();
            condition.BillType = SaleOrderBillType.BatchOrder.Value();
            var models = base.GetSaleOrderModels(page, condition);

            return models;
        }

        public void InStock(ReturnBatchOrderModel model)
        {
            var condition = new SearchSaleOrder() { OrderCode = model.Code, OrderType = OrderType.Return.Value(), BillType = SaleOrderBillType.BatchOrder.Value() };

            var order = base.GetSaleOrder(condition);

            Ensure.NotNull(order, "退货单不存在。");
            Ensure.EqualThan(order.RoStatus, ReturnSaleOrderStatus.WaitInStock.Value(), "待入库状态的退单才能入库。");

            //校验商品实收数
            foreach (var item in order.Items)
            {
                var modelItem = model.Items.FirstOrDefault(n => n.ProductId == item.ProductId);
                if (modelItem == null)
                {
                    throw new FriendlyException(string.Format("商品{0}未入库", item.ProductCode));
                }
                if (!string.IsNullOrEmpty(modelItem.SNCode))
                {
                    item.ActualQuantity = modelItem.GetSNCodeList().Count; // 以串码数为实发数
                    item.SNCode = modelItem.SNCode.Trim();
                }
                else
                {
                    item.ActualQuantity = modelItem.ActualQuantity;
                }
                if (item.ActualQuantity != item.Quantity) { throw new FriendlyException(string.Format("商品{0}实收与应收数不一致", item.ProductCode)); }
            }
            // 校验订单状态
            if (order.RoStatus != ReturnSaleOrderStatus.WaitInStock.Value())
            {
                throw new FriendlyException("当前订单：{0} 非待入库， 不能入库".FormatWith(order.Code));
            }

            BatchStockIn(order);
            ////3.同步数据到SAP
            var historyCode = base.SaveStoreInventoryHistorySap(order);

            //5.修改退单状态：已入库
            base.UpdateSaleOrderStatusToInStock(order);

            order.PaidDate = DateTime.Now;
            order.IsPushSap = false;
            _db.Update(order);
            _db.Update(order.Items.ToArray());
            _db.SaveChange();

            ////4.调用SAP接口
            var saps = _db.Table<StoreInventoryHistorySAP>().Where(h => h.Code == historyCode).ToList();
            this.InvokeSapService(order, InvokeSapServiceType.ReturnOrderInStock, saps);
            order.IsPushSap = true;
            _db.Update(order);
            _db.SaveChange();
        }
        #endregion

        /// <summary>
        /// 批发 退单入库
        /// </summary>
        /// <param name="order"></param>
        private void BatchStockIn(SaleOrder order)
        {
            var productIds = order.Items.Select(x => x.ProductId).Distinct();
            var productIdArray = productIds.ToArray();
            var products = _db.Table<Product>().Where(n => n.Id.In(productIdArray)).ToList();
            var inventories = _storeInventory.GetInventory(order.StoreId, productIds);
            var inventoryBatchs = _db.Table<StoreInventoryBatch>().Where(n => n.StoreId == order.StoreId && n.ProductId.In(productIdArray)).ToList();

            //原销售单
            var sourceSaleOrderCode = order.SourceSaleOrderCode;
            var sourceOrder = _db.Table<SaleOrder>().FirstOrDefault(n => n.Code == sourceSaleOrderCode);
            if (sourceOrder == null) { throw new FriendlyException("原销售单{0} 不存在".FormatWith(sourceSaleOrderCode)); }
            var sourceOrderItems = _db.Table<SaleOrderItem>().Where(n => n.SaleOrderId == sourceOrder.Id).ToList();
            sourceOrder.Items = sourceOrderItems;
            foreach (var item in order.Items)
            {
                // 检查串码商品是否录入串码
                var product = products.FirstOrDefault(n => n.Id == item.ProductId);
                if (product == null) { throw new FriendlyException("商品{0} 不存在".FormatWith(item.ProductCode)); }
                if (product.HasSNCode && string.IsNullOrEmpty(item.SNCode))
                {
                    throw new FriendlyException("商品{0}请录入串码".FormatWith(item.ProductCode));
                }
                // 加总库存               
                var inventory = inventories.First(x => x.ProductId == item.ProductId);

                inventory.Quantity += item.ActualQuantity;
                _db.Update(inventory);

                //加原销售批次库存
                if (!string.IsNullOrEmpty(item.SNCode))
                {
                    //串码商品                  
                    foreach (var snCode in item.GetSNCodeList())
                    {
                        var sourceProductItem = sourceOrder.Items.FirstOrDefault(n => n.ProductId == item.ProductId);
                        if (sourceProductItem == null || string.IsNullOrEmpty(sourceProductItem.SNCode) || !sourceProductItem.SNCode.Contains(snCode))
                        {
                            throw new FriendlyException("商品{0},串码{1}非原批发销售单".FormatWith(item.ProductCode,snCode));
                        }
                        var productInInventoryBatch = inventoryBatchs.FirstOrDefault(n => n.SNCode == snCode);
                        if (productInInventoryBatch == null) { throw new FriendlyException("串码商品{0} 不存在".FormatWith(item.ProductCode)); }
                        //记录修改历史
                        _db.Insert(new StoreInventoryHistory(productInInventoryBatch.Id, item.ProductId, order.StoreId, productInInventoryBatch.Quantity, 1,
                            productInInventoryBatch.Price, productInInventoryBatch.BatchNo, order.Id, order.Code, order.GetBillIdentity(), order.CreatedBy, DateTime.Now, productInInventoryBatch.SupplierId, item.RealPrice, productInInventoryBatch.SNCode));

                        productInInventoryBatch.Quantity = 1;   // 串码商品只会有1个
                        //平均成本
                        item.AvgCostPrice = productInInventoryBatch.Price;

                        _db.Update(productInInventoryBatch);
                    }
                }
                else
                {
                    //非串码商品                 

                    var batchItem = inventoryBatchs.Where(x => x.ProductId == item.ProductId).OrderByDescending(n => n.Id).FirstOrDefault();
                    if (batchItem == null) { throw new FriendlyException("商品{0} 不存在".FormatWith(item.ProductCode)); }
                    var batchQuantity = batchItem.Quantity;
                    batchItem.Quantity += item.ActualQuantity;

                    _db.Insert(new StoreInventoryHistory(batchItem.Id, item.ProductId, order.StoreId, batchQuantity, item.ActualQuantity,
                               batchItem.Price, batchItem.BatchNo, order.Id, order.Code, order.GetBillIdentity(), order.CreatedBy, DateTime.Now, batchItem.SupplierId, item.RealPrice));
                    _db.Update(batchItem);
                }
            }
        }


        protected override void CheckFJCodeExist(string fjCode) { }

        protected override void CheckStoreInventory(SaleOrderItem item, int storeId) { }

        private void InvokeSapService(SaleOrder order, InvokeSapServiceType type, List<StoreInventoryHistorySAP> saps = null)
        {
            if (type == InvokeSapServiceType.ReturnOrderCreated)
            {
                //invoke sap service
            }
            else if (type == InvokeSapServiceType.ReturnOrderInStock)
            {
                _sapService.SubmitDelivery(saps);
                _db.Update(saps.ToArray());
            }
        }
    }
}