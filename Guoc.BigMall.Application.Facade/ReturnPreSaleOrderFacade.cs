using System.Linq;
using System.Collections.Generic;

using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;
using Guoc.BigMall.Domain;
using Guoc.BigMall.Infrastructure.Utils;
using System;
using Guoc.BigMall.Infrastructure;
using System.Linq.Expressions;

namespace Guoc.BigMall.Application.Facade
{
    public class ReturnPreSaleOrderFacade : SaleOrderFacadeBase, IReturnPreSaleOrderFacade
    {
        private ISAPService _sapService;
        private IBrandRechargeVoucherFacade _brandRechargeVoucherFacade;
        private ICategoryRechargeVoucherFacade _categoryRechargeVoucherFacade;

        #region ctor
        public ReturnPreSaleOrderFacade(
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
        public void CreateReturnPreSaleOrder(SaleOrderModel model)
        {
            _log.Info("开始创建预售退货单");


            var condition = new SearchSaleOrder() { OrderCode = model.SourceSaleOrderCode, OrderType = OrderType.Order.Value(), BillType = SaleOrderBillType.PreSaleOrder.Value() };
            var presaleOrder = this.GetSaleOrder(condition);
            Ensure.NotNull(presaleOrder, "预售订单不存在！");
            Ensure.EqualThan(presaleOrder.Status, SaleOrderStatus.OutStock, "预售订单当前状态为“{0}”，不能做预售退货操作。".FormatWith(presaleOrder.Status.Description()));

            model.OrderType = OrderType.Return.Value();
            model.BillType = SaleOrderBillType.PreSaleOrder;
            model.Status = SaleOrderStatus.Create;
            model.RoStatus = ReturnSaleOrderStatus.WaitInStock.Value();

            this.CheckSaleOrderStatus(model);

            this.CheckReturnProduct(model);

            this.CheckProductHasReturned(model);

            var orderGifts = model.Items.SelectMany(x => x.GiftItems).ToList();
            orderGifts.ForEach(x => x.GiftQuantity = x.GiftReturnQuantity);

            var order = this.DoCreateSaleOrder(model);

            //计算卡券退还金额
            this.CalculateRefundVoucherAmount(order);

            _db.Insert(order);

            var history = this.DoCreateHistory(model, order);
            _db.DataBase.AddExecute(history.CreateSql("SaleOrder", order.Code), history);

            _db.SaveChange();

            _log.Info("预售退货单:{0} 创建成功!", order.Code);
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

        public SaleOrderModel GetReturnPreSaleOrderByCode(string code)
        {
            var condition = new SearchSaleOrder() { OrderCode = code, OrderType = OrderType.Return.Value(), BillType = SaleOrderBillType.PreSaleOrder.Value() };
            var model = base.GetSaleOrderModel(condition);

            return model;
        }

        public SaleOrderModel GetReturnPreSaleOrderBySaleCode(string saleCode)
        {
            var condition = new SearchSaleOrder() { SourceSaleOrderCode = saleCode, OrderType = OrderType.Return.Value(), BillType = SaleOrderBillType.PreSaleOrder.Value() };
            var model = base.GetSaleOrderModel(condition);

            return model;
        }

        public IEnumerable<SaleOrderModel> GetReturnPreSaleOrders(Pager page, SearchSaleOrder condition)
        {
            condition.OrderType = OrderType.Return.Value();
            condition.BillType = SaleOrderBillType.PreSaleOrder.Value();
            var models = base.GetSaleOrderModels(page, condition);

            return models;
        }

        public void AbandonReturnPreSaleOrder(SaleOrderAbandonModel condition)
        {
            var currentTime = DateTime.Now;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);
            Ensure.NotNull(order, "退单号：{0} 不存在".FormatWith(condition.OrderCode));

            if (order.RoStatus >= ReturnSaleOrderStatus.InStock.Value())
                throw new FriendlyException("退单号：{0} 当前状态为: {1} 不可作废".FormatWith(order.Code, ((ReturnSaleOrderStatus)order.RoStatus).Description()));

            Expression<Func<SaleOrder, bool>> where = o =>
                    o.Code == condition.OrderCode;

            Expression<Func<SaleOrder, SaleOrder>> columns = o =>
                new SaleOrder() { RoStatus = ReturnSaleOrderStatus.Cancel.Value() };

            _db.Update(columns, where);

            _db.SaveChange();
        }

        public void InStock(ReturnBatchOrderModel model)
        {
            Ensure.NotNullOrEmpty(model.Code, "退货单号为空。");
            var condition = new SearchSaleOrder() { OrderCode = model.Code, OrderType = OrderType.Return.Value(), BillType = SaleOrderBillType.PreSaleOrder.Value() };

            var order = this.GetSaleOrder(condition);

            //取原预售单做退货关单处理
            condition = new SearchSaleOrder() { OrderCode = order.SourceSaleOrderCode, OrderType = OrderType.Order.Value(), BillType = SaleOrderBillType.PreSaleOrder.Value() };
            var presaleOrder = this.GetSaleOrder(condition);

            _log.Info("预售退单{0}根据原预售订单{1}退货入库。", order.Code, order.SourceSaleOrderCode);

            Ensure.NotNull(presaleOrder, "退单关联的预售单不存在。");
            Ensure.NotIn(presaleOrder.Status, new[] { SaleOrderStatus.Convert, SaleOrderStatus.Cancel, SaleOrderStatus.Returned }, "原预售单状态已发生变更，当前不可走预售退货流程。");

            ////1.还批次库存
            //base.IncreaseStoreInventoryBatch(order);
            ////2.还总库存
            //base.IncreaseStoreInventory(order);
            ////3.同步数据到SAP
            //base.SaveStoreInventoryHistorySap(order);

            _brandRechargeVoucherFacade.RefundBrandRechargeVoucher(_db, order);//退还品牌优惠金额
            _categoryRechargeVoucherFacade.RefundCategoryRechargeVoucher(_db, order);//退还品类优惠金额

            var historyCode = base.InStock(order);

            //5.修改退单状态：已入库
            base.UpdateSaleOrderStatusToInStock(order);

            order.UpdatedBy = _context.CurrentAccount.AccountId;
            order.UpdatedOn = DateTime.Now;
            order.PaidDate = DateTime.Now;
            order.IsPushSap = false;
            _db.Update(order);
            _db.SaveChange();

            _log.Info("预售退单{0}商玛特退货入库成功。", order.Code);

            //4.调用SAP接口
            this.InvokeSapService(presaleOrder, InvokeSapServiceType.ReturnOrderInStock);
            presaleOrder.Status = SaleOrderStatus.Returned;//预售单状态改为：已退货
            order.IsPushSap = true;
            _db.Update(presaleOrder);
            _db.Update(order);
            _db.SaveChange();
        }
        #endregion

        protected override void CheckFJCodeExist(string fjCode) { }

        protected override void CheckStoreInventory(SaleOrderItem item, int storeId) { }

        private void InvokeSapService(SaleOrder order, InvokeSapServiceType type)
        {
            if (type == InvokeSapServiceType.ReturnOrderCreated)
            {
                //invoke sap service
            }
            else if (type == InvokeSapServiceType.ReturnOrderInStock)
            {
                _sapService.ClosePreSaleOrder(order);
            }
        }
    }
}