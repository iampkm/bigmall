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
    public class PreSaleOrderFacade : SaleOrderFacadeBase, IPreSaleOrderFacade
    {
        private ISAPService _sapService;
        private IBrandRechargeVoucherFacade _brandRechargeVoucherFacade;
        private ICategoryRechargeVoucherFacade _categoryRechargeVoucherFacade;

        #region ctor
        public PreSaleOrderFacade(
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

        #region Create PreSaleOrder
        public void CreatePreSaleOrder(SaleOrderModel model)
        {
            _log.Info("开始创预售单");

            model.OrderType = OrderType.Order.Value();
            model.BillType = SaleOrderBillType.PreSaleOrder;
            model.Status = SaleOrderStatus.Create;

            var order = this.DoCreateSaleOrder(model);
            order.PaidDate = null;
            _db.Insert(order);

            _brandRechargeVoucherFacade.ReduceBrandRechargeVoucher(_db, order);//扣减品牌优惠
            _categoryRechargeVoucherFacade.ReduceCategoryRechargeVoucher(_db, order);//扣减品类优惠

            //_db.SaveChange();

            //var orignalOrder = this.GetSaleOrderByCode(model.Code);
            //this.InvokeSapService(orignalOrder, InvokeSapServiceType.OrderCreated);

            //var history = this.DoCreateHistory(model, order);
            //_db.DataBase.AddExecute(history.CreateSql("SaleOrder", order.Code), history);

            //_db.SaveChange();

            //_log.Info("预售单:{0} 创建成功!", order.Code);

            var history = this.DoCreateHistory(model, order);
            _db.DataBase.AddExecute(history.CreateSql("SaleOrder", order.Code), history);

            _db.SaveChange();

            _log.Info("预售单:{0} 创建成功!", order.Code);

            //提交SAP
            order.Items.Clear();
            var items = _db.Table<SaleOrderItem>().Where(m => m.SaleOrderId == order.Id).ToList();
            order.SetItems(items);
            this.InvokeSapService(order, InvokeSapServiceType.OrderCreated);
        }
        #endregion

        #region Read To SaleOrderDto
        public IEnumerable<SaleOrderDto> GetPreSaleOrderDtos(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.PreSaleOrder.Value();

            return base.GetSaleOrderDtos(page, condition);
        }
        #endregion

        #region Read To SaleOrderModel
        public IEnumerable<SaleOrderModel> GetPreSaleOrderModels(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.PreSaleOrder.Value();

            return base.GetSaleOrderModels(page, condition);
        }

        public SaleOrderModel GetPreSaleOrderModel(SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.PreSaleOrder.Value();

            return base.GetSaleOrderModel(condition);
        }

        public SaleOrderModel GetPreSaleOrderModelByFJCode(string fjCode)
        {
            Ensure.NotNullOrEmpty(fjCode, "富基编号不能为空");

            var condition = new SearchSaleOrder();
            condition.BillType = SaleOrderBillType.PreSaleOrder.Value();
            condition.FJCode = fjCode;

            return base.GetSaleOrderModel(condition);
        }

        public SaleOrderModel GetPreSaleOrderModelByCode(string orderCode)
        {
            Ensure.NotNullOrEmpty(orderCode, "预售单编号不能为空");

            var condition = new SearchSaleOrder();
            condition.BillType = SaleOrderBillType.PreSaleOrder.Value();
            condition.OrderCode = orderCode;

            return base.GetSaleOrderModel(condition);
        }
        #endregion

        #region Read To SaleOrder
        public IEnumerable<SaleOrderModel> GetPreSaleOrders(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.PreSaleOrder.Value();
            return base.GetSaleOrderModels(page, condition);
            //return base.GetSaleOrders(page, condition);
        }

        public SaleOrder GetPreSaleOrder(SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.PreSaleOrder.Value();

            return base.GetSaleOrder(condition);
        }

        public SaleOrder GetPreSaleOrderByCode(string orderCode)
        {
            Ensure.NotNullOrEmpty(orderCode, "预售单编号不能为空");

            var condition = new SearchSaleOrder();
            condition.BillType = SaleOrderBillType.PreSaleOrder.Value();
            condition.OrderCode = orderCode;

            return base.GetSaleOrder(condition);
        }
        #endregion

        #region Update SaleOrder
        public void UpdatePreSaleOrder(SaleOrderModel model)
        {
            Ensure.NotNullOrEmpty(model.Code, "预售单编号不能为空");

            var orignalOrder = this.GetSaleOrderByCode(model.Code);

            var newOrder = this.DoUpdateSaleOrder(orignalOrder, model);
            _db.Update(newOrder);

            _db.Delete<SaleOrderItem>(x => x.SaleOrderId == newOrder.Id);
            _db.Insert(newOrder.Items.ToArray());

            this.InvokeSapService(newOrder, InvokeSapServiceType.OrderUpdated);

            var history = this.DoCreateHistory(model, newOrder);
            _db.DataBase.AddExecute(history.CreateSql("SaleOrder", newOrder.Code), history);

            _db.SaveChange();
        }

        public void AuditedPreSaleOrder(SaleOrderAuditedModel condition)
        {
            var remark = condition.AuditedRemark ?? string.Empty;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);

            if (order == null)
                throw new FriendlyException("预售单号：{0} 不存在");

            order.AuditedOrder(remark, currentAccountId);

            this.InvokeSapService(order, InvokeSapServiceType.OrderAudited);

            //_db.SaveChange();
        }

        public void AbandonPreSaleOrder(SaleOrderAbandonModel condition)
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

            _log.Info("预售作废关单=====================================>");
            _sapService.ClosePreSaleOrder(order);

            _db.SaveChange();
        }

        public void RejectPreSaleOrder(SaleOrderRejectModel condition)
        {
            var remark = condition.RejectRemark ?? string.Empty;
            var currentTime = DateTime.Now;
            var currentAccountId = _context == null ? -1 : _context.CurrentAccount.AccountId;

            var order = this.GetSaleOrderByCode(condition.OrderCode);

            if (order == null)
                throw new FriendlyException("预售单号：{0} 不存在".FormatWith(condition.OrderCode));

            if (order.Status != SaleOrderStatus.WaitAudit)
                throw new FriendlyException("预售单号：{0} 当前不是待审核状态".FormatWith(condition.OrderCode));

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

        #region PreSaleOrder OutStock
        public void OutStock(string preOrderCode)
        {
            var condition = new SearchSaleOrder() { OrderCode = preOrderCode, OrderType = OrderType.Order.Value(), BillType = SaleOrderBillType.PreSaleOrder.Value() };
            var order = this.GetPreSaleOrder(condition);

            //1.校验订单状态是否为待出库状态
            base.CheckSaleOrderCanOutStock(order);

            //2.减批次库存
            base.DecreaseStoreInventoryBatch(order);

            //3.减总库存
            base.DecreaseStoreInventory(order);

            //4.发数据到SAP
            var historyCode = base.SaveStoreInventoryHistorySap(order);
            //6.修改订单状态：已出库
            base.UpdateSaleOrderStatusToOutStock(order);

            order.IsPushSap = false;
            _db.Update(order);
            _db.SaveChange();
            //5.调用Sap接口
            var saps = _db.Table<StoreInventoryHistorySAP>().Where(h => h.Code == historyCode).ToList();
            this.InvokeSapService(order, InvokeSapServiceType.OrderOutStock, saps);
        }
        #endregion

        #region PreSaleOrder To SaleOrder
        public void ConvertToSaleOrder(string preOrderCode, string fjCode)
        {
            if (preOrderCode.IsEmpty())
                throw new FriendlyException("预售单号不能为空");
            if (fjCode.IsEmpty())
                throw new FriendlyException("富基小票号不能为空");

            if (_db.Table<SaleOrderItem>().Exists(m => m.FJCode == fjCode))
                throw new FriendlyException("富基小票号已存在，请重新录入。");

            var condition = new SearchSaleOrder() { OrderCode = preOrderCode };
            var preSaleOrder = base.GetSaleOrder(condition);

            if (preSaleOrder.Status != SaleOrderStatus.OutStock)
                throw new FriendlyException("当前预售单状态：{0} 不可生成销售单".FormatWith(preSaleOrder.Status.Description()));

            preSaleOrder.Status = SaleOrderStatus.Convert;
            preSaleOrder.UpdatedBy = _context == null ? -1 : _context.CurrentAccount.AccountId;
            preSaleOrder.UpdatedOn = DateTime.Now;
            preSaleOrder.PaidDate = DateTime.Now;
            preSaleOrder.Items.ForEach(item => item.FJCode = fjCode);
            preSaleOrder.IsPushSap = false;


            //var saleOrder = new SaleOrder();
            //preSaleOrder.MapTo(saleOrder);
            //saleOrder.Status = SaleOrderStatus.OutStock;
            ////saleOrder.BillType = SaleOrderBillType.SaleOrder;
            //saleOrder.Code = _billSequenceService.GenerateNewCode(BillIdentity.PreSaleOrder);
            //saleOrder.SourceSaleOrderCode = preSaleOrder.Code;

            //_db.Insert(saleOrder);
            _db.Update(preSaleOrder);
            _db.Update(preSaleOrder.Items.ToArray());
            _db.SaveChange();

            this.InvokeSapService(preSaleOrder, InvokeSapServiceType.ConvertToSaleOrder);
            //preSaleOrder.IsPushSap = true;
            //_db.Update(preSaleOrder);
            //_db.SaveChange();
        }
        #endregion

        private void InvokeSapService(SaleOrder order, InvokeSapServiceType type, List<StoreInventoryHistorySAP> saps = null)
        {
            if ((order.Status != SaleOrderStatus.WaitAudit
                && type == InvokeSapServiceType.OrderCreated) || type == InvokeSapServiceType.OrderAudited)
            {
                _sapService.SubmitSaleOrder(order);
                order.Status = SaleOrderStatus.WaitOutStock;
                order.IsPushSap = true;
                _db.Update(order);
                _db.Update(order.Items.ToArray());
                _db.SaveChange();
            }
            else if (type == InvokeSapServiceType.OrderUpdated)
            {
                //_sapService.SubmitSaleOrder(order);
                //invoke sap service
            }
            //else if (type == InvokeSapServiceType.OrderAudited)
            //{
            //    _sapService.SubmitSaleOrder(order);
            //    order.Status = SaleOrderStatus.WaitOutStock;
            //}
            else if (type == InvokeSapServiceType.OrderOutStock)
            {
                _sapService.SubmitDelivery(saps);
                order.IsPushSap = true;
                _db.Update(saps.ToArray());
                _db.Update(order);
                _db.SaveChange();
            }
            else if (type == InvokeSapServiceType.ConvertToSaleOrder)
            {
                _sapService.ConvertPreSaleOrder(order);
                order.IsPushSap = true;
                _db.Update(order);
                _db.SaveChange();
            }
        }


        public IEnumerable<SaleOrderListDetailDto> GetPreSaleOrderListDetail(Pager page, SearchSaleOrder condition)
        {
            condition.BillType = SaleOrderBillType.PreSaleOrder.Value();
            return base.GetSaleOrderListDetail(page, condition);
        }
    }
}