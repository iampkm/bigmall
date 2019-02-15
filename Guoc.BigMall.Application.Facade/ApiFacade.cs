using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Domain;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Domain.Objects;
using Guoc.BigMall.Application.ViewObject;
using System.Linq.Expressions;
using Guoc.BigMall.Domain.Service;
using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Infrastructure.Utils;
namespace Guoc.BigMall.Application.Facade
{
    public class ApiFacade : IApiFacade
    {
        IDBContext _db;
        ISAPService _sapService;
        IPurchaseFacade _purchaseFacade;
        ITransferOrderFacade _tranferFacade;
        ISaleOrderFacade _saleOrderFacade;
        IProductFacade _productFacade;
        IProductPriceFacade _productPriceFacade;
        IStoreInventoryFacade _storeInventoryFacade;
        BillSequenceService _sequenceService;
        ILogger _log;


        public ApiFacade(IDBContext db, ISAPService sapService, IPurchaseFacade purchaseFacade, ITransferOrderFacade tranferFacade, ISaleOrderFacade saleOrderFacad, IProductFacade productFacade, IProductPriceFacade productPriceFacade, IStoreInventoryFacade storeInventoryFacade,
            ILogger log)
        {
            this._db = db;
            this._sapService = sapService;
            this._purchaseFacade = purchaseFacade;
            this._productFacade = productFacade;
            this._tranferFacade = tranferFacade;
            this._saleOrderFacade = saleOrderFacad;
            _productPriceFacade = productPriceFacade;
            _storeInventoryFacade = storeInventoryFacade;
            _sequenceService = new BillSequenceService(this._db);
            this._log = log;
        }

        public string ClosePurchase(string code)
        {
            _log.Info("采购单{0}自动关单------------------------------------>", code);

            var message = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return "单据编码为空！";
                var order = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Code == code);
                if (order == null)
                    return "获取单据信息失败";
                if (order.Status != CBPurchaseOrderStatus.Audited)
                    return "非审核状态无法关单";
                var limteDate = order.CreatedOn.AddDays(7);
                if (DateTime.Now.CompareTo(limteDate) <= 0)
                    return "未到关单时间！";
                if (string.IsNullOrWhiteSpace(order.SapCode))
                    return "sap编码不存在！";

                _sapService.ClosePurchaseOrder(order.SapCode);
                order.Status = CBPurchaseOrderStatus.Closed;
                order.IsPushSap = true;
                ProcessHistory phistory = new ProcessHistory()
                {
                    CreatedBy = 0,
                    CreatedByName = "系统",
                    CreatedOn = DateTime.Now,
                    FormId = order.Id,
                    FormType = order.BillType == PurchaseOrderBillType.StockOrder ? BillIdentity.StockPurchaseOrder.ToString() : BillIdentity.StorePurchaseOrder.ToString(),
                    Status = (int)order.Status,
                    Remark = "采购单出入库补发成功！"
                };
                _db.Insert(phistory);
                _db.Update<PurchaseOrder>(order);
                _db.SaveChange();
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                _log.Error(ex, "采购单{0}自动关单发生错误。".FormatWith(code));
            }
            _log.Info("{0}\r\n采购单{0}自动关单------------------------------------|", message, code);
            return message;
        }


        public string PurchaseOrderInOrOut(string code)
        {
            _log.Info("采购单{0}出入库自动补发SAP------------------------------------>", code);

            var message = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return "单据编码为空！";
                var order = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Code == code);
                if (order == null)
                    return "获取单据信息失败";
                var orderItems = _db.Table<PurchaseOrderItem>().Where(n => n.PurchaseOrderId == order.Id).ToList();
                order.SetItems(orderItems);
                var sapLists = _db.Table<StoreInventoryHistorySAP>().Where(n => n.BillCode == order.Code).ToList().Where(n => n.SAPCode == null).ToList();
                if (sapLists.Count == 0)
                    return "未找到需要推送到SAP的数据！";
                if (sapLists.GroupBy(n => n.Type).Count() > 1)
                    return "单据{0}不可能同时包含出库和入库未推送的信息！".FormatWith(code);

                //var isInStock = order.OrderType == PurchaseOrderType.PurchaseReturn ? false : order.OrderType == PurchaseOrderType.PurchaseOrder ? true : order.OrderType == PurchaseOrderType.PurchaseChange && order.Status == CBPurchaseOrderStatus.Finished ? true : false;
                var isInStock = sapLists.First().Type == StoreInventoryHistorySapType.InStock;
                ProcessHistory phistory = new ProcessHistory()
                {
                    CreatedBy = 0,
                    CreatedByName = "系统",
                    CreatedOn = DateTime.Now,
                    FormId = order.Id,
                    FormType = sapLists.FirstOrDefault().BillType.ToString(),
                    Status = (int)order.Status,
                    Remark = "采购单{0}补发成功！".FormatWith(isInStock ? "入库" : "出库")
                };
                _purchaseFacade.SapStockIn(order, sapLists, phistory, isInStock);
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                _log.Error(ex, "采购单{0}出/入库自动补发SAP发生错误。".FormatWith(code));
            }
            _log.Info("{0}\r\n采购单{0}出入库自动补发SAP------------------------------------|", message, code);
            return message;
        }


        public string PurchaseOrderToSap(string code)
        {
            _log.Info("采购单{0}自动重推SAP------------------------------------>", code);

            var message = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return "单据编码为空！";
                var order = _db.Table<PurchaseOrder>().FirstOrDefault(n => n.Code == code && n.IsPushSap == false);
                if (order == null)
                    return "获取单据信息失败";
                var orderItems = _db.Table<PurchaseOrderItem>().Where(n => n.PurchaseOrderId == order.Id).ToList();
                order.SetItems(orderItems);
                switch (order.OrderType)
                {
                    case PurchaseOrderType.PurchaseOrder:
                        return "采购订单无需推送至 SAP!";
                    default:
                        if (order.Status != CBPurchaseOrderStatus.Create)
                            return "采购退货单非初始状态无法操作{0}".FormatWith(order.Status);
                        break;
                }

                _purchaseFacade.SubmitToSap(order, "系统", "自动任务补发单据成功");
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                _log.Error(ex, "自动推送采购单{0}发生错误。".FormatWith(code));
            }


            _log.Info("{0}\r\n采购单{1}自动重推SAP------------------------------------|", message, code);
            return message;
        }


        public string TransferOrdeToSap(string code)
        {
            _log.Info("调拨单{0}自动重推SAP------------------------------------>", code);

            var message = string.Empty;

            if (string.IsNullOrWhiteSpace(code))
                return "单据编码为空！";

            try
            {
                var order = _db.Table<TransferOrder>().FirstOrDefault(n => n.Code == code && n.IsPushSap == false);
                if (order == null)
                    return "获取单据信息失败";
                var orderItems = _db.Table<TransferOrderItem>().Where(n => n.TransferOrderId == order.Id).ToList();
                order.Items = orderItems;
                switch (order.Type)
                {
                    case TransferType.Allocate:
                        return "总仓分配无需推送至SAP!";
                    case TransferType.StoreApply:
                        if (order.Status != TransferStatus.Audited)
                            return "向总仓申请的调拨单据非已审状态无法操作!";
                        break;
                    case TransferType.StoreTransfer:
                        if (order.Status != TransferStatus.Initial)
                            return "门店间调拨单据非初始状态无法操作!";
                        break;
                }
                _tranferFacade.SubmitToSap(order);
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                _log.Error(ex, "自动推送调拨单{0}发生错误。".FormatWith(code));
            }
            _log.Info("{0}\r\n调拨单{1}自动重推SAP------------------------------------|", message, code);
            return message;
        }


        public string TransferOrderInOrOut(string code)
        {
            _log.Info("调拨单{0}出入库补发SAP------------------------------------>", code);

            var message = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return "单据编码为空！";
                var order = _db.Table<TransferOrder>().FirstOrDefault(n => n.Code == code && n.IsPushSap == false);
                if (order == null)
                    return "获取单据信息失败";
                var orderItems = _db.Table<TransferOrderItem>().Where(n => n.TransferOrderId == order.Id).ToList();
                order.Items = orderItems;
                StoreInventoryHistorySAP history;
                switch (order.Status)
                {
                    case TransferStatus.Shipped:
                        history = _db.Table<StoreInventoryHistorySAP>().FirstOrDefault(n => n.BillCode == order.Code && n.Type == StoreInventoryHistorySapType.OutStock);
                        if (history == null)
                            return "找不到调拨单{0}对应的出库单。".FormatWith(order.Code);
                        _tranferFacade.SapOutStock(order, history.Code, history.CreatedOn, new AccountInfo(0, "系统"));
                        break;
                    case TransferStatus.Received:
                        history = _db.Table<StoreInventoryHistorySAP>().FirstOrDefault(n => n.BillCode == order.Code && n.Type == StoreInventoryHistorySapType.InStock);
                        if (history == null)
                            return "找不到调拨单{0}对应的出库单。".FormatWith(order.Code);
                        _tranferFacade.SapInStock(order, history.Code, history.CreatedOn, new AccountInfo(0, "系统"));
                        break;
                    default:
                        return "单据状态错误无法操作！{0}".FormatWith(order.Status.ToString());
                }
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                _log.Error(ex, "调拨单{0}出/入库自动补发SAP发生错误。".FormatWith(code));
            }
            _log.Info("{0}\r\n调拨单{1}出入库补发SAP------------------------------------|", message, code);
            return message;
        }

        public string SaleOrdeToSap(string code)
        {
            _log.Info("销售单{0}自动重推SAP------------------------------------>", code);

            var message = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return "单据编码为空！";
                var order = _db.Table<SaleOrder>().FirstOrDefault(n => n.Code == code && n.IsPushSap == false);
                if (order == null)
                    return "获取单据信息失败";
                if (order.BillType == SaleOrderBillType.BatchOrder)
                    return "批发单据无需推送！";

                if (!((order.OrderType == Domain.ValueObject.OrderType.Order.Value() && order.Status == SaleOrderStatus.Create)
                    || (order.OrderType == Domain.ValueObject.OrderType.Return.Value() && order.RoStatus == ReturnSaleOrderStatus.Create.Value())
                    ))
                    return "单据状态不正确，无法操作!{0}".FormatWith(order.IsReturned() ? ((ReturnSaleOrderStatus)order.RoStatus).ToString() : order.Status.ToString());

                var items = _db.Table<SaleOrderItem>().Where(m => m.SaleOrderId == order.Id).ToList();
                order.Items.Clear();
                order.SetItems(items);


                if (order.OrderType == Domain.ValueObject.OrderType.Return.Value())
                    order.RoStatus = ReturnSaleOrderStatus.WaitInStock.Value();
                else
                    order.Status = SaleOrderStatus.WaitOutStock;

                order.UpdatedOn = DateTime.Now;
                order.UpdatedBy = 0;
                order.IsPushSap = true;

                _sapService.SubmitSaleOrder(order);
                _db.Insert(new ProcessHistory(0, "系统", order.Status.Value(), order.Id, order.GetBillIdentity().ToString(), "自动推单至SAP成功"));

                _db.Update(order);
                _db.Update(order.Items.ToArray());
                _db.SaveChange();
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                _log.Error(ex, "自动推送销售单{0}发生错误。".FormatWith(code));
            }
            _log.Info("{0}\r\n销售单{1}自动重推SAP------------------------------------|", message, code);
            return message;
        }


        public string SaleOrderOutToSap(string code)
        {
            _log.Info("销售单{0}出库自动补发SAP------------------------------------>", code);

            var message = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return "单据编码为空！";
                var order = _db.Table<SaleOrder>().FirstOrDefault(n => n.Code == code && n.IsPushSap == false);
                if (order == null)
                    return "获取单据信息失败";

                if (order.OrderType != Domain.ValueObject.OrderType.Order.Value())
                    return "退单不能出库！";

                if (order.Status != SaleOrderStatus.OutStock)
                    return "单据状态不正确，无法操作!{0}".FormatWith(order.Status);
                var saps = _db.Table<StoreInventoryHistorySAP>().Where(h => h.BillCode == order.Code && h.Type == StoreInventoryHistorySapType.OutStock).ToList();
                if (saps.Count() == 0)
                    return "销售单{0}出库历史不存在!".FormatWith(order.Code);


                _sapService.SubmitDelivery(saps);

                order.UpdatedOn = DateTime.Now;
                order.UpdatedBy = 0;
                order.IsPushSap = true;

                _db.Insert(new ProcessHistory(0, "系统", order.Status.Value(), order.Id, order.GetBillIdentity().ToString(), "自动出库推送至SAP成功"));

                _db.Update(saps.ToArray());
                _db.Update(order);
                _db.SaveChange();
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                _log.Error(ex, "销售单{0}出库自动补发SAP发生错误。".FormatWith(code));
            }
            _log.Info("{0}\r\n销售单{1}出库自动补发SAP------------------------------------|", message, code);
            return message;
        }


        public string SaleOrderInToSap(string code)
        {
            _log.Info("零售单{0}入库自动补发SAP------------------------------------>", code);

            var message = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return "单据编码为空！";
                var order = _db.Table<SaleOrder>().FirstOrDefault(n => n.Code == code && n.IsPushSap == false);
                if (order == null)
                    return "获取单据信息失败";

                if (order.BillType != SaleOrderBillType.SaleOrder)
                    return "仅零售退单可调用此接口推送入库信息。";

                if (order.OrderType != Domain.ValueObject.OrderType.Return.Value())
                    return "订单不能入库！";

                if (order.RoStatus != ReturnSaleOrderStatus.InStock.Value())
                    return "单据状态不正确，无法操作!{0}".FormatWith((ReturnSaleOrderStatus)order.RoStatus);
                var saps = _db.Table<StoreInventoryHistorySAP>().Where(h => h.BillCode == order.Code && h.Type == StoreInventoryHistorySapType.InStock).ToList();
                if (saps.Count() == 0)
                    return "零售单{0}入库历史不存在!".FormatWith(order.Code);

                _sapService.SubmitDelivery(saps);

                order.UpdatedOn = DateTime.Now;
                order.UpdatedBy = 0;
                order.IsPushSap = true;

                _db.Insert(new ProcessHistory(0, "系统", order.Status.Value(), order.Id, order.GetBillIdentity().ToString(), "自动入库推送至SAP成功"));

                _db.Update(saps.ToArray());
                _db.Update(order);
                _db.SaveChange();
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                _log.Error(ex, "零售单{0}入库自动补发SAP发生错误。".FormatWith(code));
            }
            _log.Info("{0}\r\n零售单{1}入库自动补发SAP------------------------------------|", message, code);
            return message;
        }


        public string PreOrderReturnInStock(string code)
        {
            _log.Info("预售单{0}退货入库自动补发SAP------------------------------------>", code);
            var message = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return "单据编码为空！";

                var orderType = Domain.ValueObject.OrderType.Return.Value();
                var returnPreSaleOrder = _db.Table<SaleOrder>().FirstOrDefault(n => n.Code == code && n.OrderType == orderType && n.BillType == SaleOrderBillType.PreSaleOrder && n.IsPushSap == false);
                if (returnPreSaleOrder == null)
                    return "获取单据信息失败";

                if (returnPreSaleOrder.RoStatus != (int)ReturnSaleOrderStatus.InStock)
                    return "单据退货状态不正确，无法操作!{0}".FormatWith(returnPreSaleOrder.RoStatus);

                orderType = Domain.ValueObject.OrderType.Order.Value();
                var presaleOrder = _db.Table<SaleOrder>().FirstOrDefault(o => o.Code == returnPreSaleOrder.SourceSaleOrderCode && o.OrderType == orderType && o.BillType == SaleOrderBillType.PreSaleOrder);
                if (presaleOrder == null)
                    return "退单关联的预售单不存在。";

                if (presaleOrder.Status.In(SaleOrderStatus.Convert, SaleOrderStatus.Cancel, SaleOrderStatus.Returned))
                    return "原预售单状态已发生变更，当前不可走预售退货流程。";

                returnPreSaleOrder.UpdatedBy = 0;
                returnPreSaleOrder.UpdatedOn = DateTime.Now;
                returnPreSaleOrder.IsPushSap = true;

                presaleOrder.UpdatedBy = 0;
                presaleOrder.UpdatedOn = DateTime.Now;
                presaleOrder.Status = SaleOrderStatus.Returned;//预售单状态改为：已退货

                _sapService.ClosePreSaleOrder(presaleOrder);

                _db.Insert(new ProcessHistory(0, "系统", returnPreSaleOrder.Status.Value(), returnPreSaleOrder.Id, returnPreSaleOrder.GetBillIdentity().ToString(), "自动入库推送至SAP成功"));
                _db.Update(returnPreSaleOrder);
                _db.Update(presaleOrder);
                _db.SaveChange();
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                _log.Error(ex, "预售单{0}退货入库自动补发SAP发生错误。".FormatWith(code));
            }
            _log.Info("{0}\r\n预售单{1}退货入库自动补发SAP------------------------------------|", message, code);
            return message;
        }


        public string PreConvertOrder(string code)
        {
            _log.Info("预售单{0}转正自动补发SAP------------------------------------>", code);
            var message = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return "单据编码为空！";
                var order = _db.Table<SaleOrder>().FirstOrDefault(n => n.Code == code && n.IsPushSap == false);
                if (order == null)
                    return "获取单据信息失败";

                if (order.BillType != SaleOrderBillType.PreSaleOrder)
                    return "仅预售订单可调用此接口推送转正信息。";

                if (order.OrderType != Domain.ValueObject.OrderType.Order.Value())
                    return "预售退单不能转正！";

                if (order.Status != SaleOrderStatus.Convert)
                    return "当前预售单状态：{0} 不可生成销售单".FormatWith(order.Status.Description());
                var items = _db.Table<SaleOrderItem>().Where(m => m.SaleOrderId == order.Id).ToList();
                order.Items.Clear();
                order.SetItems(items);


                order.UpdatedOn = DateTime.Now;
                order.UpdatedBy = 0;
                order.IsPushSap = true;
                _sapService.ConvertPreSaleOrder(order);
                _db.Insert(new ProcessHistory(0, "系统", order.Status.Value(), order.Id, order.GetBillIdentity().ToString(), "预售转正推送至SAP成功"));
                _db.Update(order);
                _db.SaveChange();
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                _log.Error(ex, "预售单{0}转正补发SAP发生错误。".FormatWith(code));
            }
            _log.Info("{0}\r\n预售单{1}转正自动补发SAP------------------------------------|", message, code);
            return message;

        }

        public string AbandonSaleOrder(string code)
        {
            _log.Info("零售单订单{0}手动作废------------------------------------>", code);

            var message = string.Empty;
            var condition = new SaleOrderAbandonModel()
            {
                OrderCode = code,
            };

            try
            {
                var order = _db.Table<SaleOrder>().FirstOrDefault(o => o.Code == code);

                if (order.BillType != SaleOrderBillType.SaleOrder || order.OrderType != Domain.ValueObject.OrderType.Order.Value())
                    return "仅零售单订单可作废！";

                _saleOrderFacade.AbandonSaleOrder(condition);
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                _log.Error(ex, "零售单订单{0}手动作废发生错误。".FormatWith(code));
            }
            _log.Info("{0}\r\n零售单订单{1}手动作废------------------------------------|", message, code);
            return message;
        }

        public string CreateROWithNoSourceSO(string storeCode, string snCode, string productCode, int quantity, decimal realPrice)
        {
            _log.Info("开始创建退货单（无原销售单）------------------------------------>");

            var orderCode = string.Empty;

            try
            {
                var store = _db.Table<Store>().FirstOrDefault(s => s.Code == storeCode);
                Ensure.NotNull(store, "录入的仓位{0}不存在！".FormatWith(storeCode));
                var product = _db.Table<Product>().FirstOrDefault(p => p.Code == productCode);
                Ensure.NotNull(product, "录入的商品{0}不存在！".FormatWith(productCode));
                //var productPrice = _productPriceFacade.GetStoreProductPrice(store.Id, product.Id);
                //Ensure.NotNull(productPrice, "系统中无商品价格！");

                var order = new SaleOrder();
                order.OrderType = Domain.ValueObject.OrderType.Return.Value();
                order.BillType = SaleOrderBillType.SaleOrder;
                order.Status = SaleOrderStatus.Create;
                order.RoStatus = ReturnSaleOrderStatus.Create.Value();
                order.Code = orderCode = _sequenceService.GenerateNewCode(order.GetBillIdentity());
                //order.SourceSapCode = model.SourceSapCode;
                order.StoreId = store.Id;
                order.StoreCode = store.Code;
                order.ParentCode = "{0}{1}".FormatWith("P", _sequenceService.GenerateNewCode(order.GetBillIdentity()));
                order.CreatedBy = 1;
                order.CreatedOn = DateTime.Now;
                order.UpdatedBy = 1;
                order.UpdatedOn = DateTime.Now;
                order.PaidDate = DateTime.Now;
                order.IsPushSap = false;
                //门店客户对应关系
                var storeCustomerMap = _db.Table<StoreCustomerMap>().FirstOrDefault(n => n.StoreCode == order.StoreCode);
                if (storeCustomerMap == null) throw new FriendlyException(string.Format("门店{0}找不到对应客户", order.StoreCode));
                order.CustomerCode = storeCustomerMap.CustomerCode;

                if (product.HasSNCode && snCode.IsNullOrEmpty())
                    throw new FriendlyException("串码商品{0}销售制单必须录入串码。".FormatWith(product.Code));

                var orderItem = new SaleOrderItem();
                orderItem.ProductId = product.Id;
                orderItem.ProductCode = product.Code;
                orderItem.ProductName = product.Name;
                orderItem.Unit = product.Unit;
                //orderItem.SalePrice = productPrice.SalePrice;
                //orderItem.MinSalePrice = productPrice.MinSalePrice;//限价
                orderItem.RealPrice = realPrice; ;
                //orderItem.SupplierId = item.SupplierId;//todo
                orderItem.Quantity = quantity;
                orderItem.GiftType = OrderProductType.Product.Value();
                orderItem.SNCode = snCode.IsNullOrEmpty() ? null : snCode;
                //orderItem.FJCode = fjCode;
                //orderItem.SourceSapRow = item.SourceSapRow;
                //orderItem.SourceSaleOrderRow = item.SourceSaleOrderRow;

                //校验富基Code
                //if (operType.Equals("add"))
                //    CheckFJCodeExist(item.FJCode);

                if (orderItem.RealPrice <= 0) { throw new FriendlyException("商品[{0}],销售价不能为零".FormatWith(product.Code)); }

                //检查是否有特价商品 如果有则修改其订单状态为待审核
                //if (order.OrderType == Domain.ValueObject.OrderType.Order.Value() && orderItem.RealPrice < productPrice.MinSalePrice)
                //    order.Status = SaleOrderStatus.WaitAudit;

                //添加礼品
                var parentProductId = "{0}{1}".FormatWith(product.Id, "1".PadLeft(4, '0'));
                orderItem.ParentProductId = int.Parse(parentProductId);//明细唯一码

                order.SetItem(orderItem);

                _db.Insert(order);
                _db.SaveChange();
            }
            catch (Exception ex)
            {
                _log.Error(ex, "创建退货单（无原销售单）失败！");
                throw new FriendlyException("创建退货单（无原销售单）失败！原因：{0}".FormatWith(ex.Message));
            }

            try
            {
                var returnOrder = _db.Table<SaleOrder>().FirstOrDefault(s => s.Code == orderCode);
                returnOrder.Items = _db.Table<SaleOrderItem>().Where(m => m.SaleOrderId == returnOrder.Id).ToList();

                _sapService.SubmitSaleOrder(returnOrder);

                var reason = string.Format("创建{0}", returnOrder.GetBillIdentityDescription());
                var history = new ProcessHistory(1, "", (int)returnOrder.Status, returnOrder.Id, returnOrder.GetBillIdentity().ToString(), reason);

                _db.DataBase.AddExecute(history.CreateSql("SaleOrder", returnOrder.Code), history);

                returnOrder.IsPushSap = true;
                returnOrder.RoStatus = ReturnSaleOrderStatus.WaitInStock.Value();

                _db.Update(returnOrder);
                _db.Update(returnOrder.Items.ToArray());
                _db.SaveChange();
            }
            catch (Exception ex)
            {
                _log.Error(ex, "创建退货单（无原销售单）成功后推送SAP失败！");
                throw new FriendlyException("退货单{0}创建成功后推送SAP失败！原因：{1}".FormatWith(orderCode, ex.Message));
            }

            _log.Info("退货单:{0} 创建成功！（无原销售单）------------------------------------|", orderCode);
            return orderCode;
        }

        public void ROInStockWithNoSourceSO(string returnOrderCode, string supplierCode, decimal costPrice)
        {
            _log.Info("退货单{0}开始入库（无原销售单）------------------------------------>", returnOrderCode);

            string historyCode;
            SaleOrder returnOrder;
            try
            {
                var supplier = _db.Table<Supplier>().FirstOrDefault(s => s.Code == supplierCode);
                Ensure.NotNull(supplier, "供应商不存在！");
                returnOrder = _db.Table<SaleOrder>().FirstOrDefault(s => s.Code == returnOrderCode);
                Ensure.NotNull(returnOrder, "退货单不存在！");
                Ensure.EqualThan(returnOrder.RoStatus, ReturnSaleOrderStatus.WaitInStock.Value(), "待入库状态的退单才能入库。");
                returnOrder.Items = _db.Table<SaleOrderItem>().Where(m => m.SaleOrderId == returnOrder.Id).ToList();
                Ensure.NotNullOrEmpty(returnOrder.Items, "单据明细为空！");

                #region 入库

                var storeId = returnOrder.StoreId;//判断从良品仓退还是从赠品仓退
                var returnItems = returnOrder.Items;

                //合并相同的商品
                var items = new List<SaleOrderItem>();
                foreach (var item in returnItems)
                {
                    var stockInItem = items.FirstOrDefault(m => m.ProductId == item.ProductId && m.SNCode == item.SNCode);
                    if (stockInItem != null)
                    {
                        stockInItem.Quantity += item.Quantity;
                        continue;
                    }
                    var copyItem = new SaleOrderItem();//用明细项的新实例来合并数量，避免引用更改
                    item.MapTo(copyItem);
                    items.Add(copyItem);
                }

                var stockInModel = new StockInModel()
                {
                    InStockType = InStockType.Normal,
                    StockInBillId = returnOrder.Id,
                    StockInBillCode = returnOrder.Code,
                    StockInBillType = returnOrder.GetBillIdentity(),
                    //StockOutBillId = oriSaleOrder.Id,
                    //StockOutBillType = oriSaleOrder.GetBillIdentity(),
                    StoreId = storeId,
                    CreatedBy = returnOrder.CreatedBy,
                    CreatedOn = DateTime.Now,
                    Items = items.Select(item => new StockInItemModel()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        SNCode = item.SNCode,
                        CostPrice = costPrice,
                        ContractPrice = costPrice,
                        SupplierId = supplier.Id,
                        //BrandPreferential = item.BrandPreferential,
                        //CategoryPreferential = item.CategoryPreferential,
                    }).ToList()
                };

                _storeInventoryFacade.InStock(_db, stockInModel, false);

                #endregion

                #region 记录SAP历史

                historyCode = _sequenceService.GenerateNewCode(BillIdentity.SapHistoryOrder);
                foreach (var item in returnOrder.Items)
                {
                    var model = new StoreInventoryHistorySAP();
                    model.Code = historyCode;
                    model.Type = StoreInventoryHistorySapType.InStock;
                    model.ProductId = item.ProductId;
                    model.ProductCode = item.ProductCode;
                    model.StoreId = storeId;
                    model.StoreCode = returnOrder.StoreCode;
                    model.BillSapCode = returnOrder.SapCode;
                    model.BillSapRow = item.SapRow;
                    model.Unit = item.Unit;
                    model.BillItemId = item.Id;
                    model.SAPCode = ""; //todo
                    model.SAPRow = "";//todo
                    model.Quantity = item.Quantity;
                    model.SNCodes = item.SNCode;
                    model.BillCode = returnOrder.Code;
                    model.BillType = returnOrder.GetBillIdentity();
                    model.CreatedOn = DateTime.Now;
                    model.CreatedBy = 1;

                    _db.Insert(model);
                }

                #endregion

                //5.修改退单状态：已入库
                returnOrder.RoStatus = ReturnSaleOrderStatus.InStock.Value();
                returnOrder.IsPushSap = false;

                _db.Update(returnOrder);
                _db.SaveChange();
            }
            catch (Exception ex)
            {
                _log.Error(ex, "退货单{0}入库失败！", returnOrderCode);
                throw new FriendlyException("退货单入库失败！原因：{0}".FormatWith(ex.Message));
            }

            try
            {
                //4.调用SAP接口
                var saps = _db.Table<StoreInventoryHistorySAP>().Where(h => h.Code == historyCode).ToList();
                _sapService.SubmitDelivery(saps);
                returnOrder.IsPushSap = true;

                _db.Update(saps.ToArray());
                _db.Update(returnOrder);
                _db.SaveChange();
            }
            catch (Exception ex)
            {
                _log.Error(ex, "退货单{0}入库成功后推送SAP失败！");
                throw new FriendlyException("退货单{0}入库成功后推送SAP失败！原因：{1}".FormatWith(returnOrderCode, ex.Message));
            }

            _log.Info("退货单{0}入库成功！（无原销售单）------------------------------------|", returnOrderCode);
        }

        public void initInventory(string excel, string file)
        {
            var lists = new List<InitInventoryExcelDto>();
            try
            {
                lists = ConvertPurchaseOrder(excel);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0},convert failed", file), ex);
            }

            //先按门店分 1 品1 单生成
            foreach (var line in lists)
            {
                try
                {

                    var entity = new PurchaseOrder();
                    // 按门店和供应商分组数据                       
                    entity.SapCode = "";
                    entity.CreatedOn = DateTime.Parse("2018-10-25");
                    entity.CreatedBy = 1;
                    entity.UpdatedOn = entity.CreatedOn;
                    entity.UpdatedBy = 1;
                    entity.Remark = file + "#2018-10-25期初库存导入";
                    entity.OrderType = PurchaseOrderType.PurchaseOrder;
                    //默认已审
                    entity.Status = CBPurchaseOrderStatus.Audited;
                    entity.IsPushSap = true;
                    var supplierEntity = _db.Table<Supplier>().FirstOrDefault(n => n.Code == line.SupplierCode.Trim());
                    if (supplierEntity == null) { throw new FriendlyException(string.Format("供应商{0}不存在", line.SupplierCode)); }
                    entity.SupplierCode = line.SupplierCode;
                    entity.SupplierId = supplierEntity.Id;

                    var storeCode = line.StoreCode.Trim();
                    var store = _db.Table<Store>().FirstOrDefault(n => n.Code == storeCode);
                    if (store == null) { throw new FriendlyException(string.Format("门店{0}不存在", storeCode)); }
                    entity.StoreId = store.Id;
                    entity.StoreCode = storeCode;
                    entity.BillType = store.IsMainStore() ? PurchaseOrderBillType.StockOrder : PurchaseOrderBillType.StoreOrder;



                    //处理明细
                    var productCode = line.ProductCode.Trim();
                    var product = _db.Table<Product>().FirstOrDefault(n => n.Code == productCode);
                    if (product == null) { throw new FriendlyException(string.Format("{1}商品{0}不存在", productCode, file)); }
                    var item = new PurchaseOrderItem();
                    item.ProductId = product.Id;
                    item.ProductCode = line.ProductCode.Trim();
                    item.CostPrice = line.CostPrice;
                    item.Quantity = line.Quantity;
                    item.SNCodes = line.SNCode;
                    item.SNQuantity = string.IsNullOrEmpty(item.SNCodes) ? line.Quantity : 1;
                    item.IsSnCode = product.HasSNCode;
                    item.ActualQuantity = line.Quantity;
                    entity.Amount += item.CostPrice * item.Quantity;
                    entity.AddItem(item);

                    entity.Code = _sequenceService.GenerateNewCode(entity.GetBillIdentity());
                    _db.Insert(entity);
                    _db.SaveChange();

                }
                catch (FriendlyException fe)
                {
                    _log.Info(fe.Message);
                }
                catch (Exception ex)
                {

                    _log.Error(ex);
                }

            }

        }

        //串码商品 匹配excel
        //private List<InitInventoryExcelDto> ConvertPurchaseOrder(string value)
        //{
        //    List<InitInventoryExcelDto> result = new List<InitInventoryExcelDto>(2000);
        //    string[] productIdArray = value.Trim('\n').Split('\n');
        //    foreach (var item in productIdArray)
        //    {
        //        if (item.Contains("\t"))
        //        {
        //            InitInventoryExcelDto dto = new InitInventoryExcelDto();
        //            string[] line = item.Split('\t');
        //            dto.StoreCode = line[1].Trim();
        //            dto.ProductCode = line[2].Trim();
        //            dto.Quantity = (int)Convert.ToDecimal(line[3].Trim());
        //            dto.CostPrice = decimal.Parse(line[8].Trim());
        //            dto.SNCode = line[10].Trim();
        //            dto.SupplierCode = line[11].Trim();
        //            result.Add(dto);
        //        }
        //    }
        //    return result;
        //}

        // 非串码商品
        private List<InitInventoryExcelDto> ConvertPurchaseOrder(string value)
        {
            List<InitInventoryExcelDto> result = new List<InitInventoryExcelDto>(2000);
            string[] productIdArray = value.Trim('\n').Split('\n');
            foreach (var item in productIdArray)
            {
                if (item.Contains("\t"))
                {
                    InitInventoryExcelDto dto = new InitInventoryExcelDto();
                    string[] line = item.Split('\t');
                    dto.StoreCode = line[1].Trim();
                    dto.ProductCode = line[2].Trim();
                    dto.Quantity = (int)Convert.ToDecimal(line[3].Trim());
                    dto.CostPrice = decimal.Parse(line[9].Trim());
                    //dto.SNCode = line[10].Trim();
                    dto.SupplierCode = line[12].Trim();
                    result.Add(dto);
                }
            }
            return result;
        }
    }
}
