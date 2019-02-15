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
using System;

namespace Guoc.BigMall.Application.Facade
{
    public class ReturnSaleOrderFacade : SaleOrderFacadeBase, IReturnSaleOrderFacade
    {
        private ISAPService _sapService;
        private IBrandRechargeVoucherFacade _brandRechargeVoucherFacade;
        private ICategoryRechargeVoucherFacade _categoryRechargeVoucherFacade;

        #region ctor
        public ReturnSaleOrderFacade(
            IDBContext db,
            IContextFacade context,
            ILogger log,
            IProductPriceFacade productPriceFacade,
            IProductFacade productFacade,
            IStoreInventoryFacade storeInventory,
            ISupplierFacade supplierFacade,
            IStoreFacade storeFacade,
            IStoreInventoryFacade storeInventoryFacade,
            ISAPService sapService,
            IBrandRechargeVoucherFacade brandRechargeVoucherFacade,
            ICategoryRechargeVoucherFacade categoryRechargeVoucherFacade)
            : base(db, context, log, productPriceFacade, productFacade, storeInventory, supplierFacade, storeInventoryFacade, storeFacade)
        {
            _sapService = sapService;
            _brandRechargeVoucherFacade = brandRechargeVoucherFacade;
            _categoryRechargeVoucherFacade = categoryRechargeVoucherFacade;
        }
        #endregion

        #region Interface Implements
        public void CreateReturnSaleOrder(SaleOrderModel model)
        {
            _log.Info("开始创建退货单");

            model.OrderType = OrderType.Return.Value();
            model.BillType = SaleOrderBillType.SaleOrder;
            model.Status = SaleOrderStatus.Create;
            model.RoStatus = ReturnSaleOrderStatus.Create.Value();

            this.CheckSaleOrderStatus(model);

            this.CheckReturnProduct(model);

            this.CheckProductHasReturned(model);

            var orderGifts = model.Items.SelectMany(x => x.GiftItems).ToList();
            orderGifts.ForEach(x => x.GiftQuantity = x.GiftReturnQuantity);

            var order = base.DoCreateSaleOrder(model);

            //计算卡券退还金额
            this.CalculateRefundVoucherAmount(order);

            _db.Insert(order);
            _db.SaveChange();

            var returnOrder = this.GetSaleOrderByCode(order.Code);


            this.InvokeSapService(returnOrder, InvokeSapServiceType.ReturnOrderCreated);

            var history = this.DoCreateHistory(model, returnOrder);
            _db.DataBase.AddExecute(history.CreateSql("SaleOrder", returnOrder.Code), history);

            returnOrder.IsPushSap = true;
            returnOrder.RoStatus = ReturnSaleOrderStatus.WaitInStock.Value();

            _db.Update(returnOrder);
            _db.Update(returnOrder.Items.ToArray());
            _db.SaveChange();

            _log.Info("退货单:{0} 创建成功!", order.Code);
        }

        //计算卡券退还金额
        private void CalculateRefundVoucherAmount(SaleOrder order)
        {
            var sourceSaleOrderCode = order.SourceSaleOrderCode;
            var sourceOrder = _db.Table<SaleOrder>().FirstOrDefault(s => s.Code == sourceSaleOrderCode);
            Ensure.NotNull(sourceOrder, "找不到原销售订单。");
            var productType = OrderProductType.Product.Value();
            var sourceOrderItems = _db.Table<SaleOrderItem>().Where(m => m.SaleOrderId == sourceOrder.Id && m.GiftType == productType).ToList();

            order.Items.Where(m => m.GiftType == productType).ToList().ForEach(item =>
            {
                var sourceItem = sourceOrderItems.FirstOrDefault(m => m.ProductId == item.ProductId && m.SNCode == item.SNCode);
                item.CategoryPreferential = Math.Round(sourceItem.CategoryPreferential / sourceItem.Quantity * item.Quantity, 2);
                item.BrandPreferential = Math.Round(sourceItem.BrandPreferential / sourceItem.Quantity * item.Quantity, 2);
            });
        }

        public SaleOrderModel GetReturnSaleOrderByCode(string code)
        {
            var condition = new SearchSaleOrder() { OrderCode = code, OrderType = OrderType.Return.Value(), BillType = SaleOrderBillType.SaleOrder.Value() };
            var model = base.GetSaleOrderModel(condition);

            return model;
        }

        public SaleOrderModel GetReturnSaleOrderBySaleCode(string saleCode)
        {
            var condition = new SearchSaleOrder() { SourceSaleOrderCode = saleCode, OrderType = OrderType.Return.Value(), BillType = SaleOrderBillType.SaleOrder.Value() };
            var model = base.GetSaleOrderModel(condition);

            return model;
        }

        public IEnumerable<SaleOrderModel> GetReturnSaleOrders(Pager page, SearchSaleOrder condition)
        {
            condition.OrderType = OrderType.Return.Value();
            condition.BillType = SaleOrderBillType.SaleOrder.Value();
            var models = base.GetSaleOrderModels(page, condition);

            return models;
        }

        public void InStock(ReturnBatchOrderModel model)
        {
            Ensure.NotNullOrEmpty(model.Code, "退货单号为空。");
            var condition = new SearchSaleOrder() { OrderCode = model.Code, OrderType = OrderType.Return.Value(), BillType = SaleOrderBillType.SaleOrder.Value() };

            var order = base.GetSaleOrder(condition);

            Ensure.NotNull(order, "退货单不存在。");
            Ensure.EqualThan(order.RoStatus, ReturnSaleOrderStatus.WaitInStock.Value(), "待入库状态的退单才能入库。");

            //1.还批次库存
            //base.IncreaseStoreInventoryBatch(order);
            ////2.还总库存
            //base.IncreaseStoreInventory(order);
            //3.同步数据到SAP
            //var saps = base.SaveStoreInventoryHistorySap(order);

            _brandRechargeVoucherFacade.RefundBrandRechargeVoucher(_db, order);//退还品牌优惠金额
            _categoryRechargeVoucherFacade.RefundCategoryRechargeVoucher(_db, order);//退还品类优惠金额

            var historyCode = base.InStock(order);

            //5.修改退单状态：已入库
            base.UpdateSaleOrderStatusToInStock(order);

            order.PaidDate = DateTime.Now;
            order.IsPushSap = false;
            _db.Update(order);
            _db.SaveChange();

            //4.调用SAP接口
            var saps = _db.Table<StoreInventoryHistorySAP>().Where(h => h.Code == historyCode).ToList();
            this.InvokeSapService(order, InvokeSapServiceType.ReturnOrderInStock, saps);
            order.IsPushSap = true;
            _db.Update<SaleOrder>(order);
            _db.SaveChange();
        }
        #endregion

        protected override void CheckFJCodeExist(string fjCode) { }

        protected override void CheckStoreInventory(SaleOrderItem item, int storeId) { }

        private void InvokeSapService(SaleOrder order, InvokeSapServiceType type, List<StoreInventoryHistorySAP> saps = null)
        {
            if (type == InvokeSapServiceType.ReturnOrderCreated)
            {
                _sapService.SubmitSaleOrder(order);
                //invoke sap service
            }
            else if (type == InvokeSapServiceType.ReturnOrderInStock)
            {
                _sapService.SubmitDelivery(saps);
                _db.Update(saps.ToArray());

                //invoke sap service
            }
        }
    }
}