using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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

namespace Guoc.BigMall.Application.Facade
{
    public class ExchangeOrderFacade : SaleOrderFacadeBase, IExchangeOrderFacade
    {
        private ISAPService _sapService;

        #region ctor
        public ExchangeOrderFacade(
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

        #region Create ExchangeOrder
        public void CreateExchangeOrder(SaleOrderModel model)
        {
            _log.Info("开始创建换货单");

            model.OrderType = OrderType.Order.Value();
            model.BillType = SaleOrderBillType.ExchangeOrder;
            model.Status = SaleOrderStatus.Create;

            var order = this.DoCreateSaleOrder(model);
            _db.Insert(order);

            this.InvokeSapService(order, InvokeSapServiceType.OrderCreated);

            var history = this.DoCreateHistory(model, order);
            _db.DataBase.AddExecute(history.CreateSql("SaleOrder", order.Code), history);

            _db.SaveChange();

            _log.Info("换货单:{0} 创建成功!", order.Code);
        }
        #endregion

        #region Read To SaleOrderDto
        public IEnumerable<SaleOrderDto> GetExchangeOrderDtos(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.ExchangeOrder.Value();

            return base.GetSaleOrderDtos(page, condition);
        }
        #endregion

        #region Read To SaleOrderModel
        public IEnumerable<SaleOrderModel> GetExchangeOrderModels(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.ExchangeOrder.Value();

            return base.GetSaleOrderModels(page, condition);
        }

        public SaleOrderModel GetExchangeOrderModel(SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.ExchangeOrder.Value();

            return base.GetSaleOrderModel(condition);
        }

        public SaleOrderModel GetExchangeOrderModelByFJCode(string fjCode)
        {
            Ensure.NotNullOrEmpty(fjCode, "富基编号不能为空");

            var condition = new SearchSaleOrder();
            condition.BillType = SaleOrderBillType.ExchangeOrder.Value();
            condition.FJCode = fjCode;

            return base.GetSaleOrderModel(condition);
        }

        public SaleOrderModel GetExchangeOrderModelByCode(string exchangeOrderCode)
        {
            Ensure.NotNullOrEmpty(exchangeOrderCode, "换机单编号不能为空");

            var condition = new SearchSaleOrder();
            condition.BillType = SaleOrderBillType.SaleOrder.Value();
            condition.OrderCode = exchangeOrderCode;

            return base.GetSaleOrderModel(condition);
        }
        #endregion

        #region Read To SaleOrder
        public IEnumerable<SaleOrder> GetExchangeOrders(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.ExchangeOrder.Value();

            return base.GetSaleOrders(page, condition);
        }

        public SaleOrder GetExchangeOrder(SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.ExchangeOrder.Value();

            return base.GetSaleOrder(condition);
        }

        public SaleOrder GetExchangeOrderByCode(string exchangeOrderCode)
        {
            Ensure.NotNullOrEmpty(exchangeOrderCode, "换机单编号不能为空");

            var condition = new SearchSaleOrder();
            condition.BillType = SaleOrderBillType.ExchangeOrder.Value();
            condition.OrderCode = exchangeOrderCode;

            return base.GetSaleOrder(condition);
        }
        #endregion

        #region Update ExchangeOrder
        public void UpdateExchangeOrder(SaleOrderModel model)
        {
            Ensure.NotNullOrEmpty(model.Code, "换机单编号不能为空");

            var orignalOrder = this.GetExchangeOrderByCode(model.Code);

            var newOrder = base.DoUpdateSaleOrder(orignalOrder, model);
            _db.Update(newOrder);

            _db.Delete<SaleOrderItem>(x => x.SaleOrderId == newOrder.Id);
            _db.Insert(newOrder.Items.ToArray());

            this.InvokeSapService(newOrder, InvokeSapServiceType.OrderUpdated);

            var history = this.DoCreateHistory(model, newOrder);
            _db.DataBase.AddExecute(history.CreateSql("SaleOrder", newOrder.Code), history);

            _db.SaveChange();
        }

        public void AuditedExchangeOrder(SaleOrderAuditedModel condition)
        {
            var remark = condition.AuditedRemark ?? string.Empty;
            var currentTime = DateTime.Now;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);

            if (order == null)
                throw new FriendlyException("换机单：{0} 不存在");

            if (order.Status != SaleOrderStatus.WaitAudit)
                throw new FriendlyException("换机单：{0} 当前不是待审核状态");

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

        public void AbandonExchangeOrder(SaleOrderAbandonModel condition)
        {
            var currentTime = DateTime.Now;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);
            Ensure.NotNull(order, "换机单：{0} 不存在".FormatWith(condition.OrderCode));

            if (order.Status.Value() >= SaleOrderStatus.OutStock.Value())
                throw new FriendlyException("换机单：{0} 当前状态为: {1} 不可作废".FormatWith(order.Code, order.Status.Description()));

            Expression<Func<SaleOrder, bool>> where = o =>
                    o.Code == condition.OrderCode;

            Expression<Func<SaleOrder, SaleOrder>> columns = o =>
                new SaleOrder() { Status = SaleOrderStatus.Cancel };

            _db.Update(columns, where);

            this.InvokeSapService(order, InvokeSapServiceType.OrderAbandoned);

            _db.SaveChange();
        }

        public void RejectExchangeOrder(SaleOrderRejectModel condition)
        {
            var remark = condition.RejectRemark ?? string.Empty;
            var currentTime = DateTime.Now;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);

            if (order == null)
                throw new FriendlyException("换机单：{0} 不存在".FormatWith(condition.OrderCode));

            if (order.Status != SaleOrderStatus.WaitAudit)
                throw new FriendlyException("换机单：{0} 当前不是待审核状态".FormatWith(condition.OrderCode));

            Expression<Func<SaleOrder, bool>> where = o =>
                    o.Code == condition.OrderCode && o.Status == SaleOrderStatus.WaitAudit;

            Expression<Func<SaleOrder, SaleOrder>> columns = o => new SaleOrder()
            {
                Status = SaleOrderStatus.Create,
                AuditedRemark = remark,
                AuditedOn = currentTime,
                AuditedBy = currentAccountId
            };

            _db.Update(columns, where);

            this.InvokeSapService(order, InvokeSapServiceType.OrderRejected);

            _db.SaveChange();
        }
        #endregion

        #region ExchangeOrder InOutStock
        public void InOutStock(string exchangeOrderCode)
        {
            //InStock
            //OutStock
        }
        #endregion

        private void InvokeSapService(SaleOrder order, InvokeSapServiceType type)
        {
            if (order.Status != SaleOrderStatus.WaitAudit
                && type == InvokeSapServiceType.OrderCreated)
            {
                var res = true; //invoke sap service
                if (res)
                    order.Status = SaleOrderStatus.WaitOutStock;
                else
                    throw new FriendlyException("调用SAP接口失败,订单创建失败");
            }
            else if (type == InvokeSapServiceType.OrderUpdated)
            {
                //invoke sap service
            }
            else if (type == InvokeSapServiceType.OrderAudited)
            {
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
                //invoke sap service
            }
        }
    }
}