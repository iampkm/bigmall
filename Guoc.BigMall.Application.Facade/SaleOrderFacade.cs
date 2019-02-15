using Dapper.DBContext;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.ValueObject;
using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Guoc.BigMall.Application.Facade
{
    public class SaleOrderFacade : SaleOrderFacadeBase, ISaleOrderFacade
    {
        private ISAPService _sapService;
        private IBrandRechargeVoucherFacade _brandRechargeVoucherFacade;
        private ICategoryRechargeVoucherFacade _categoryRechargeVoucherFacade;

        #region ctor
        public SaleOrderFacade(
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

        #region Create SaleOrder
        void ISaleOrderFacade.CreateSaleOrder(SaleOrderModel model)
        {
            _log.Info("开始创建销售单");

            model.OrderType = OrderType.Order.Value();
            model.BillType = SaleOrderBillType.SaleOrder;
            model.Status = SaleOrderStatus.Create;

            var order = this.DoCreateSaleOrder(model);
            _db.Insert(order);

            _brandRechargeVoucherFacade.ReduceBrandRechargeVoucher(_db, order);//扣减品牌优惠
            _categoryRechargeVoucherFacade.ReduceCategoryRechargeVoucher(_db, order);//扣减品类优惠

            var history = this.DoCreateHistory(model, order);
            _db.DataBase.AddExecute(history.CreateSql("SaleOrder", order.Code), history);

            _db.SaveChange();

            _log.Info("销售单:{0} 创建成功!", order.Code);

            order.Items.Clear();
            var items = _db.Table<SaleOrderItem>().Where(m => m.SaleOrderId == order.Id).ToList();
            order.SetItems(items);
            this.InvokeSapService(order, InvokeSapServiceType.OrderCreated);

            order.IsPushSap = true;
            _db.Update(order);
            _db.Update(order.Items.ToArray());
            _db.SaveChange();
        }
        #endregion

        #region Read To SaleOrderDto
        IEnumerable<SaleOrderDto> ISaleOrderFacade.GetSaleOrderDtos(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.SaleOrder.Value();

            return base.GetSaleOrderDtos(page, condition);
        }
        #endregion

        #region Read To SaleOrderModel
        IEnumerable<SaleOrderModel> ISaleOrderFacade.GetSaleOrderModels(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.SaleOrder.Value();

            return base.GetSaleOrderModels(page, condition);
        }

        SaleOrderModel ISaleOrderFacade.GetSaleOrderModel(SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.SaleOrder.Value();

            return base.GetSaleOrderModel(condition);
        }

        SaleOrderModel ISaleOrderFacade.GetSaleOrderModelByFJCode(string fjCode)
        {
            Ensure.NotNullOrEmpty(fjCode, "富基编号不能为空");

            var condition = new SearchSaleOrder();
            //condition.BillType = SaleOrderBillType.SaleOrder.Value();
            condition.FJCode = fjCode;

            return base.GetSaleOrderModel(condition);
        }

        SaleOrderModel ISaleOrderFacade.GetSaleOrderModelByCode(string orderCode)
        {
            Ensure.NotNullOrEmpty(orderCode, "订单编号不能为空");

            var condition = new SearchSaleOrder();
            condition.BillType = SaleOrderBillType.SaleOrder.Value();
            condition.OrderCode = orderCode;

            return base.GetSaleOrderModel(condition);
        }
        #endregion

        #region Read To SaleOrder
        IEnumerable<SaleOrder> ISaleOrderFacade.GetSaleOrders(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.SaleOrder.Value();

            return base.GetSaleOrders(page, condition);
        }

        SaleOrder ISaleOrderFacade.GetSaleOrder(SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.SaleOrder.Value();

            return base.GetSaleOrder(condition);
        }

        SaleOrder ISaleOrderFacade.GetSaleOrderByCode(string orderCode)
        {
            Ensure.NotNullOrEmpty(orderCode, "订单编号不能为空");

            var condition = new SearchSaleOrder();
            condition.BillType = SaleOrderBillType.SaleOrder.Value();
            condition.OrderCode = orderCode;

            return base.GetSaleOrder(condition);
        }
        #endregion

        #region Update SaleOrder
        void ISaleOrderFacade.UpdateSaleOrder(SaleOrderModel model)
        {
            Ensure.NotNullOrEmpty(model.Code, "订单编号不能为空");

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

        void ISaleOrderFacade.AuditedSaleOrder(SaleOrderAuditedModel condition)
        {
            var remark = condition.AuditedRemark ?? string.Empty;
            //var currentTime = DateTime.Now;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);

            if (order == null)
                throw new FriendlyException("订单号：{0} 不存在".FormatWith(order.Code));

            this.InvokeSapService(order, InvokeSapServiceType.OrderAudited);

            order.AuditedOrder(remark, currentAccountId);

            _db.Update(order);
            _db.Update(order.Items.ToArray());
            _db.SaveChange();
        }

        void ISaleOrderFacade.AbandonSaleOrder(SaleOrderAbandonModel condition)
        {
            var currentTime = DateTime.Now;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);
            Ensure.NotNull(order, "订单号：{0} 不存在".FormatWith(condition.OrderCode));

            if (order.Status.Value() >= SaleOrderStatus.OutStock.Value())
                throw new FriendlyException("订单号：{0} 当前状态为: {1} 不可作废".FormatWith(order.Code, order.Status.Description()));

            order.Status = SaleOrderStatus.Cancel;

            Expression<Func<SaleOrder, bool>> where = o =>
                    o.Code == condition.OrderCode;

            Expression<Func<SaleOrder, SaleOrder>> columns = o =>
                new SaleOrder() { Status = SaleOrderStatus.Cancel };

            _db.Update(columns, where);

            _brandRechargeVoucherFacade.RefundBrandRechargeVoucher(_db, order);//退还品牌优惠金额
            _categoryRechargeVoucherFacade.RefundCategoryRechargeVoucher(_db, order);//退还品类优惠金额

            this.InvokeSapService(order, InvokeSapServiceType.OrderAbandoned);

            _db.SaveChange();
        }

        void ISaleOrderFacade.RejectSaleOrder(SaleOrderRejectModel condition)
        {
            var remark = condition.RejectRemark ?? string.Empty;
            var currentTime = DateTime.Now;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);

            if (order == null)
                throw new FriendlyException("订单号：{0} 不存在".FormatWith(condition.OrderCode));

            if (order.Status != SaleOrderStatus.WaitAudit)
                throw new FriendlyException("订单号：{0} 当前不是待审核状态".FormatWith(condition.OrderCode));

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

            this.InvokeSapService(order, InvokeSapServiceType.OrderRejected);

            _db.SaveChange();
        }
        #endregion

        #region SaleOrder OutStock
        void ISaleOrderFacade.OutStock(string orderCode)
        {
            Ensure.NotNullOrEmpty(orderCode, "出库单号为空！");
            var condition = new SearchSaleOrder() { OrderCode = orderCode, OrderType = OrderType.Order.Value(), BillType = SaleOrderBillType.SaleOrder.Value() };
            var order = this.GetSaleOrder(condition);
            Ensure.NotNull(order, "订单{0}不存在！".FormatWith(orderCode));

            //1.校验订单状态是否为待出库状态
            base.CheckSaleOrderCanOutStock(order);

            //2.减批次库存
            base.DecreaseStoreInventoryBatch(order);

            //3.减总库存
            base.DecreaseStoreInventory(order);

            //4.发数据到SAP
            var historyCode = base.SaveStoreInventoryHistorySap(order);
            base.UpdateSaleOrderStatusToOutStock(order);

            order.PaidDate = DateTime.Now;
            order.IsPushSap = false;
            _db.Update(order);
            _db.SaveChange();

            //5.调用Sap接口
            var saps = _db.Table<StoreInventoryHistorySAP>().Where(h => h.Code == historyCode).ToList();
            this.InvokeSapService(order, InvokeSapServiceType.OrderOutStock, saps);

            order.IsPushSap = true;
            _db.Update(order);
            _db.SaveChange();
        }
        #endregion

        private void InvokeSapService(SaleOrder order, InvokeSapServiceType type, List<StoreInventoryHistorySAP> saps = null)
        {
            if (order.Status != SaleOrderStatus.WaitAudit
                && type == InvokeSapServiceType.OrderCreated)
            {
                _sapService.SubmitSaleOrder(order);
                order.Status = SaleOrderStatus.WaitOutStock;
                //var res = true; //invoke sap service
                //if (res)
                //    order.Status = SaleOrderStatus.WaitOutStock;
                //else
                //    throw new FriendlyException("调用SAP接口失败,订单创建失败");
            }
            else if (type == InvokeSapServiceType.OrderUpdated)
            {
                _sapService.SubmitSaleOrder(order);
                //invoke sap service
            }
            else if (type == InvokeSapServiceType.OrderAudited)
            {
                _sapService.SubmitSaleOrder(order);
                //invoke sap service
            }
            else if (type == InvokeSapServiceType.OrderAbandoned)
            {
                //invoke sap service
            }
            else if (type == InvokeSapServiceType.OrderRejected)
            {
                //invoke sap service
            }
            else if (type == InvokeSapServiceType.OrderOutStock)
            {
                _sapService.SubmitDelivery(saps);
                _db.Update(saps.ToArray());
                //invoke sap service
            }
        }


        public IEnumerable<SaleOrderModel> GetSaleOrderItems(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.SaleOrder.Value();

            return base.GetSaleOrderItemModels(page, condition);
        }


        public IEnumerable<SaleOrderListDetailDto> GetSaleOrderListDetail(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.SaleOrder.Value();
            return base.GetSaleOrderListDetail(page, condition);
        }
    }
}