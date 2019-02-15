using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Collections;
using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Infrastructure.Utils;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;
using Guoc.BigMall.Application.Configuration;

namespace Guoc.BigMall.Application.Facade
{
    public class BatchSaleOrderFacade : SaleOrderFacadeBase, IBatchSaleOrderFacade
    {
        private ISAPService _sapService;

        #region ctor
        public BatchSaleOrderFacade(
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

        #region CreateSaleOrder
        void IBatchSaleOrderFacade.CreateBatchSaleOrder(SaleOrderModel model)
        {
            _log.Info("开始创建批发销售单");

            model.OrderType = OrderType.Order.Value();
            model.BillType = SaleOrderBillType.BatchOrder;
            model.Status = SaleOrderStatus.Create;

            var order = this.DoCreateSaleOrder(model);
            _db.Insert(order);

            this.InvokeSapService(order, InvokeSapServiceType.OrderCreated);

            var history = this.DoCreateHistory(model, order);
            _db.DataBase.AddExecute(history.CreateSql("SaleOrder", order.Code), history);

            _db.SaveChange();

            _log.Info("批发销售单编号:{0} 创建成功!", order.Code);
        }
        #endregion

        #region Read To SaleOrderDto
        IEnumerable<SaleOrderDto> IBatchSaleOrderFacade.GetBatchSaleOrderDtos(Pager page, SearchSaleOrder condition)
        {
            condition.OrderType = OrderType.Order.Value();
            condition.BillType = SaleOrderBillType.BatchOrder.Value();

            return base.GetSaleOrderDtos(page, condition);
        }
        #endregion

        #region Read To SaleOrderModel
        public IEnumerable<SaleOrderModel> GetBatchSaleOrderModels(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.BatchOrder.Value();

            return base.GetSaleOrderModels(page, condition);
        }

        SaleOrderModel IBatchSaleOrderFacade.GetBatchSaleOrderModel(SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.BatchOrder.Value();

            return base.GetSaleOrderModel(condition);
        }

        SaleOrderModel IBatchSaleOrderFacade.GetBatchSaleOrderModelByFJCode(string fjCode)
        {
            Ensure.NotNullOrEmpty(fjCode, "富基编号不能为空");

            var condition = new SearchSaleOrder();
            condition.BillType = SaleOrderBillType.BatchOrder.Value();
            condition.FJCode = fjCode;

            return base.GetSaleOrderModel(condition);
        }

        SaleOrderModel IBatchSaleOrderFacade.GetBatchSaleOrderModelByCode(string orderCode)
        {
            Ensure.NotNullOrEmpty(orderCode, "批发销售单编号不能为空");

            var condition = new SearchSaleOrder();
            condition.BillType = SaleOrderBillType.BatchOrder.Value();
            condition.OrderCode = orderCode;

            return base.GetSaleOrderModel(condition);
        }
        #endregion

        #region Read To SaleOrder
        IEnumerable<SaleOrder> IBatchSaleOrderFacade.GetBatchSaleOrders(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.BatchOrder.Value();

            return base.GetSaleOrders(page, condition);
        }

        SaleOrder IBatchSaleOrderFacade.GetBatchSaleOrder(SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.BatchOrder.Value();

            return base.GetSaleOrder(condition);
        }

        SaleOrder IBatchSaleOrderFacade.GetBatchSaleOrderByCode(string orderCode)
        {
            Ensure.NotNullOrEmpty(orderCode, "批发销售单编号不能为空");

            var condition = new SearchSaleOrder();
            condition.BillType = SaleOrderBillType.BatchOrder.Value();
            condition.OrderCode = orderCode;

            return base.GetSaleOrder(condition);
        }
        #endregion

        #region Update SaleOrder
        void IBatchSaleOrderFacade.UpdateBatchSaleOrder(SaleOrderModel model)
        {
            Ensure.NotNullOrEmpty(model.Code, "批发销售单编号不能为空");

            var orignalOrder = this.GetSaleOrderByCode(model.Code);

            var newOrder = base.DoUpdateSaleOrder(orignalOrder, model);
            _db.Update(newOrder);

            _db.Delete<SaleOrderItem>(x => x.SaleOrderId == newOrder.Id);
            _db.Insert(newOrder.Items.ToArray());

            this.InvokeSapService(newOrder, InvokeSapServiceType.OrderUpdated);

            var history = this.DoCreateHistory(model, newOrder);
            _db.DataBase.AddExecute(history.CreateSql("SaleOrder", newOrder.Code), history);

            _db.SaveChange();
        }

        void IBatchSaleOrderFacade.AuditedBatchSaleOrder(SaleOrderAuditedModel condition)
        {
            var remark = condition.AuditedRemark ?? string.Empty;
            var currentTime = DateTime.Now;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);

            if (order == null)
                throw new FriendlyException("批发销售单号：{0} 不存在");

            if (order.Status != SaleOrderStatus.WaitAudit)
                throw new FriendlyException("批发销售单号：{0} 当前不是待审核状态");

            Expression<Func<SaleOrder, bool>> where = o =>
                   o.Code == condition.OrderCode && o.Status == SaleOrderStatus.WaitAudit;

            Expression<Func<SaleOrder, SaleOrder>> columns = o => new SaleOrder()
            {
                Status = SaleOrderStatus.WaitOutStock,
                AuditedRemark = remark,
                AuditedOn = currentTime,
                AuditedBy = currentAccountId
            };

            _db.Update(columns, where);

            this.InvokeSapService(order, InvokeSapServiceType.OrderAudited);

            _db.SaveChange();
        }

        void IBatchSaleOrderFacade.AbandonBatchSaleOrder(SaleOrderAbandonModel condition)
        {
            var currentTime = DateTime.Now;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);
            Ensure.NotNull(order, "批发销售单号：{0} 不存在".FormatWith(condition.OrderCode));

            if (order.Status.Value() >= SaleOrderStatus.OutStock.Value())
                throw new FriendlyException("批发销售单号：{0} 当前状态为: {1} 不可作废".FormatWith(order.Code, order.Status.Description()));

            Expression<Func<SaleOrder, bool>> where = o =>
                    o.Code == condition.OrderCode;

            Expression<Func<SaleOrder, SaleOrder>> columns = o =>
                new SaleOrder() { Status = SaleOrderStatus.Cancel };

            _db.Update(columns, where);

            _db.SaveChange();
        }

        void IBatchSaleOrderFacade.RejectBatchSaleOrder(SaleOrderRejectModel condition)
        {
            var remark = condition.RejectRemark ?? string.Empty;
            var currentTime = DateTime.Now;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);

            if (order == null)
                throw new FriendlyException("批发销售单号：{0} 不存在".FormatWith(condition.OrderCode));

            if (order.Status != SaleOrderStatus.WaitAudit)
                throw new FriendlyException("批发销售单号：{0} 当前不是待审核状态".FormatWith(condition.OrderCode));

            Expression<Func<SaleOrder, bool>> where = o =>
                    o.Code == condition.OrderCode && o.Status == SaleOrderStatus.WaitAudit;

            Expression<Func<SaleOrder, SaleOrder>> columns = o => new SaleOrder()
            {
                Status = SaleOrderStatus.Cancel,
                AuditedRemark = remark,
                AuditedOn = currentTime,
                AuditedBy = currentAccountId
            };

            _db.Update(columns, where);

            _db.SaveChange();
        }
        #endregion

        #region BatchSaleOrder OutStock
        void IBatchSaleOrderFacade.OutStock(SaleOrderOutStockModel model)
        {
            var condition = new SearchSaleOrder() { OrderCode = model.Code, OrderType = OrderType.Order.Value(), BillType = SaleOrderBillType.BatchOrder.Value() };
            var order = this.GetSaleOrder(condition);

            foreach (var item in order.Items)
            {
                var modelItem = model.Items.FirstOrDefault(n => n.ProductId == item.ProductId);
                if (modelItem == null)
                {
                    throw new FriendlyException(string.Format("商品{0}未出库", item.ProductCode));
                }

                var product = _db.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                Ensure.NotNull(product, "商品{0}不存在！".FormatWith(item.ProductCode));

                if (!string.IsNullOrEmpty(modelItem.SNCode))
                {
                    item.ActualQuantity = modelItem.GetSNCodeList().Count; // 以串码数为实发数
                    item.SNCode = modelItem.SNCode.Trim();

                    Ensure.False(product.HasSNCode && item.ActualQuantity > SystemConfig.ItemMaxSNCodeQuantity, "商品{0}一次最多只能出库{1}个串码。".FormatWith(item.ProductCode, SystemConfig.ItemMaxSNCodeQuantity));
                }
                else
                {
                    item.ActualQuantity = modelItem.ActualQuantity;
                }
                if (item.ActualQuantity != item.Quantity) { throw new FriendlyException(string.Format("商品{0}实发数要等于应发数", item.ProductCode)); }
            }

            //1.校验订单状态是否为待出库状态
            base.CheckSaleOrderCanOutStock(order);

            // 3 减库存
            BatchStockOut(order);

            //4.发数据到SAP
            var historyCode = base.SaveStoreInventoryHistorySap(order);
            //6.修改订单状态：已出库
            base.UpdateSaleOrderStatusToOutStock(order);

            order.PaidDate = DateTime.Now;
            order.IsPushSap = false;
            _db.Update(order);
            _db.Update(order.Items.ToArray());
            _db.SaveChange();

            //5.调用Sap接口
            var saps = _db.Table<StoreInventoryHistorySAP>().Where(h => h.Code == historyCode).ToList();
            //this.InvokeSapService(order, InvokeSapServiceType.OrderOutStock, saps);

            //order.IsPushSap = true;
            //_db.Update(order);
            //_db.Update(order.Items.ToArray());
            //_db.SaveChange();
            PostOrderToASP(order, saps);
        }

        /// <summary>
        /// 批发销售单出库
        /// </summary>
        /// <param name="order"></param>
        private void BatchStockOut(SaleOrder order)
        {
            var productIds = order.Items.Select(x => x.ProductId).Distinct();
            var productIdArray = productIds.ToArray();
            var products = _db.Table<Product>().Where(n => n.Id.In(productIdArray)).ToList();
            var inventories = _storeInventory.GetInventory(order.StoreId, productIds);
            var inventoryBatchs = _storeInventory.GetInventoryBatch(order.StoreId, productIds);
            foreach (var item in order.Items)
            {
                // 检查串码商品是否录入串码
                var product = products.FirstOrDefault(n => n.Id == item.ProductId);
                if (product == null) { throw new FriendlyException("商品{0} 不存在".FormatWith(item.ProductCode)); }
                if (product.HasSNCode && string.IsNullOrEmpty(item.SNCode))
                {
                    throw new FriendlyException("商品{0}请录入串码".FormatWith(item.ProductCode));
                }

                // 减总库存               
                var inventory = inventories.FirstOrDefault(x => x.ProductId == item.ProductId);
                if (inventory == null)
                { throw new FriendlyException("商品{0}从未在该门店采购入库".FormatWith(item.ProductCode)); }
                if (inventory.Quantity < item.ActualQuantity)
                { throw new FriendlyException("商品{0} 总仓库存不足".FormatWith(item.ProductCode)); }

                inventory.Quantity -= item.ActualQuantity;
                _db.Update(inventory);

                //减批次库存
                if (!string.IsNullOrEmpty(item.SNCode))
                {
                    //串码商品                  
                    foreach (var snCode in item.GetSNCodeList())
                    {
                        var productInInventoryBatch = inventoryBatchs.FirstOrDefault(n => n.SNCode == snCode);
                        if (productInInventoryBatch == null) { throw new FriendlyException("串码商品{0} 不存在".FormatWith(item.ProductCode)); }
                        //记录修改历史
                        _db.Insert(new StoreInventoryHistory(productInInventoryBatch.Id, item.ProductId, order.StoreId, productInInventoryBatch.Quantity, -1,
                            productInInventoryBatch.Price, productInInventoryBatch.BatchNo, order.Id, order.Code, order.GetBillIdentity(), order.CreatedBy, DateTime.Now, productInInventoryBatch.SupplierId, item.RealPrice, productInInventoryBatch.SNCode));

                        productInInventoryBatch.Quantity = 0;   // 串码商品只会有1个
                        //平均成本
                        item.AvgCostPrice = productInInventoryBatch.Price;

                        _db.Update(productInInventoryBatch);
                    }
                }
                else
                {
                    //非串码商品
                    var productInInventoryBatch = inventoryBatchs.Where(x => x.ProductId == item.ProductId);

                    if (productInInventoryBatch.Sum(x => x.Quantity) < item.ActualQuantity)
                        throw new FriendlyException("商品{0} 批次库存不足".FormatWith(item.ProductCode));

                    var totalPrice = 0M;
                    var leftQty = item.ActualQuantity;
                    foreach (var batchItem in productInInventoryBatch)
                    {
                        //扣减库存
                        var reduceQty = Math.Min(leftQty, batchItem.Quantity);
                        batchItem.Quantity -= reduceQty;
                        _db.Insert(new StoreInventoryHistory(batchItem.Id, item.ProductId, order.StoreId, batchItem.Quantity, -reduceQty,
                                batchItem.Price, batchItem.BatchNo, order.Id, order.Code, order.GetBillIdentity(), order.CreatedBy, DateTime.Now, batchItem.SupplierId, item.RealPrice));
                        leftQty -= reduceQty;
                        //累积批次总成本
                        totalPrice += (batchItem.Price * reduceQty);
                        _db.Update(batchItem);
                        if (leftQty == 0) break;
                    }
                }
            }
        }


        #endregion

        private void InvokeSapService(SaleOrder order, InvokeSapServiceType type, List<StoreInventoryHistorySAP> saps = null)
        {
            if (type == InvokeSapServiceType.OrderUpdated)
            {
                //invoke sap service
            }
            else if (type == InvokeSapServiceType.OrderOutStock)
            {
                _sapService.SubmitDelivery(saps);
                _db.Update(saps.ToArray());
                //invoke sap service
            }
            else if (type == InvokeSapServiceType.ReturnOrderCreated)
            {
                //invoke sap service
            }
        }


        public IEnumerable<SaleOrderListDetailDto> GetBatchSaleOrderListDetail(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.BatchOrder.Value();
            return base.GetSaleOrderListDetail(page, condition);
        }


        public void PostOrderToSap(string code)
        {
            var order = _db.Table<SaleOrder>().FirstOrDefault(n => n.Code == code);
            var orderItems = _db.Table<SaleOrderItem>().Where(n => n.SaleOrderId == order.Id).ToList();
            order.Items = orderItems;
            var saps = _db.Table<StoreInventoryHistorySAP>().Where(n => n.BillCode == code).ToList();


            // var saps = _db.Table<StoreInventoryHistorySAP>().Where(h => h.Code == historyCode).ToList();
            //  this.InvokeSapService(order, InvokeSapServiceType.OrderOutStock, saps);
            // 推送SAP
            PostOrderToASP(order, saps);

        }

        private void PostOrderToASP(SaleOrder order, List<StoreInventoryHistorySAP> saps)
        {
            _sapService.SubmitDelivery(saps);
            _db.Update(saps.ToArray());

            order.IsPushSap = true;
            _db.Update(order);
            _db.Update(order.Items.ToArray());
            _db.SaveChange();
        }

        public List<SaleOrderModel> GetPrintList(string ids)
        {
            if (string.IsNullOrEmpty(ids)) { throw new FriendlyException("参数不能为空"); }

            var page = new Pager()
            {
                IsPaging = false,
            };
            var condition = new SearchSaleOrder()
            {
                Ids = ids,
            };
            var data = this.GetBatchSaleOrderModels(page, condition).ToList();
            return data;
        }
    }
}