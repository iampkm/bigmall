using System;
using System.Linq;
using System.Web.Mvc;

using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure.File;
using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Facade;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;
using Newtonsoft.Json;
using Guoc.BigMall.Domain.Entity;

namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class SaleOrderController : Controller
    {
        IExcel _excelService;
        IDBContext _db;
        ISaleOrderFacade _saleOrderFacade;
        IReturnSaleOrderFacade _returnOrderFacade;
        IContextFacade _context;
        public SaleOrderController(IDBContext db, ISaleOrderFacade saleOrderFacade, IExcel iexcel, IContextFacade context, IReturnSaleOrderFacade returnOrderFacade)
        {
            this._excelService = iexcel;
            this._db = db;
            this._saleOrderFacade = saleOrderFacade;
            this._context = context;
            this._returnOrderFacade = returnOrderFacade;
        }
        #region 销售订单
        public ActionResult Index()
        {
            ViewBag.ShowStatus = (int)SaleOrderStatus.Create;
            ViewBag.OrderType = (int)OrderType.Order;
            return View();
        }

        public ActionResult GetSaleOrderList(Pager page, SearchSaleOrder condition)
        {
            if (condition.StoreId.IsNullOrEmpty())
                condition.StoreId = _context.CurrentAccount.CanViewStores;

            var rows = _saleOrderFacade.GetSaleOrderModels(page, condition);

            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("销售订单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult CreateSo()
        {
            ViewBag.StatusDesc = SaleOrderStatus.Create.Description();
            ViewBag.CreatedBy = _context.CurrentAccount.AccountId;
            ViewBag.CreatedByName = _context.CurrentAccount.NickName;
            ViewBag.DefaultStoreId = _context.CurrentAccount.StoreArray.FirstOrDefault();
            return View();
        }


        [HttpPost]
        public JsonResult CreateSo(SaleOrderModel model)
        {
            model.UpdatedBy = _context.CurrentAccount.AccountId;
            model.UpdatedByName = _context.CurrentAccount.NickName;
            model.CreatedBy = _context.CurrentAccount.AccountId;

            _saleOrderFacade.CreateSaleOrder(model);

            return Json(new { success = true });
        }

        public ActionResult AuditIndex()
        {
            ViewBag.OrderType = (int)OrderType.Order;
            ViewBag.ShowStatus = (int)SaleOrderStatus.WaitAudit;
            return View();
        }

        [HttpPost]
        public ActionResult Audit(SaleOrderAuditedModel model)
        {
            if (model.OrderCode.IsEmpty())
                return Json(new { success = false, error = "订单编号不能为空" });

            _saleOrderFacade.AuditedSaleOrder(model);

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult Reject(SaleOrderRejectModel model)
        {
            if (model.OrderCode.IsEmpty())
                return Json(new { success = false, error = "订单编号不能为空" });

            _saleOrderFacade.RejectSaleOrder(model);

            return Json(new { success = true });
        }

        public ActionResult ShowDetail(string code)
        {
            var condition = new SearchSaleOrder() { OrderCode = code };
            var order = _saleOrderFacade.GetSaleOrderModel(condition);
            ViewBag.SaleOrder = JsonConvert.SerializeObject(order);
            ViewBag.OutStock = (int)SaleOrderStatus.OutStock;
            ViewBag.RejectAudit = (int)SaleOrderStatus.Create;
            return View();
        }

        public ActionResult Edit(string code)
        {
            var condition = new SearchSaleOrder() { OrderCode = code };
            var order = _saleOrderFacade.GetSaleOrderModel(condition);
            ViewBag.SaleOrder = JsonConvert.SerializeObject(order);
            return View();
        }

        [HttpPost]
        public JsonResult Edit(SaleOrderModel saleOrderModel)
        {
            _saleOrderFacade.UpdateSaleOrder(saleOrderModel);
            return Json(new { success = true });
        }
        public ActionResult Print(int id)
        {
            var condition = new SearchSaleOrder() { SaleOrderId = id };
            var order = _saleOrderFacade.GetSaleOrderModel(condition);
            //ViewBag.SaleOrder = JsonConvert.SerializeObject(order);
            //ViewBag.OutStock = (int)SaleOrderStatus.OutStock;
            //ViewBag.RejectAudit = (int)SaleOrderStatus.Create;

            return PartialView("SaleOrderTemplate", order);
        }
        #endregion

        #region 出库
        public ActionResult StockOutIndex()
        {
            ViewBag.ShowStatus = (int)SaleOrderStatus.WaitOutStock;
            return View();
        }


        public ActionResult StockOut(string code)
        {
            var condition = new SearchSaleOrder() { OrderCode = code };
            var order = _saleOrderFacade.GetSaleOrderModel(condition);
            ViewBag.SaleOrder = JsonConvert.SerializeObject(order);
            return View();
        }

        [HttpPost]
        public ActionResult StockOut(SaleOrderOutStockModel model)
        {
            _saleOrderFacade.OutStock(model.Code);
            return Json(new { success = true });
        }

        public JsonResult AbandonSaleOrder(SaleOrderAbandonModel model)
        {
            _saleOrderFacade.AbandonSaleOrder(model);
            return Json(new { success = true });
        }
        #endregion

        #region 销售退单
        /// <summary>
        /// 销售退单
        /// </summary>
        /// <param name="page"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult GetReturnOrderList(Pager page, SearchSaleOrder condition)
        {
            //if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            if (condition.StoreId.IsNullOrEmpty())
                condition.StoreId = _context.CurrentAccount.CanViewStores;

            var rows = _returnOrderFacade.GetReturnSaleOrders(page, condition);

            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("销售订单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total });
        }



        public ActionResult GetSaleOrderByFJCode(string code)
        {
            var rows = _saleOrderFacade.GetSaleOrderModelByFJCode(code);
            return Json(new { success = true, data = rows });
        }
        public ActionResult RoList()
        {
            ViewBag.OrderType = (int)OrderType.Return;
            return View();
        }

        public ActionResult CreateRo()
        {
            ViewBag.StatusDesc = SaleOrderStatus.Create.Description();
            ViewBag.CreatedBy = _context.CurrentAccount.AccountId;
            ViewBag.CreatedByName = _context.CurrentAccount.NickName;
            ViewBag.DefaultStoreId = _context.CurrentAccount.StoreArray.FirstOrDefault();
            ViewBag.OutStock = SaleOrderStatus.OutStock.Value();
            ViewBag.Convert = SaleOrderStatus.Convert.Value();
            ViewBag.OrderType = OrderType.Order.Value();
            return View();
        }


        [HttpPost]
        public JsonResult CreateRo(SaleOrderModel orderModel)
        {
            orderModel.CreatedBy = _context.CurrentAccount.AccountId;
            orderModel.UpdatedByName = _context.CurrentAccount.NickName;
            _returnOrderFacade.CreateReturnSaleOrder(orderModel);
            return Json(new { success = true });
        }
        #endregion

        #region 入库
        public ActionResult StockInIndex()
        {
            ViewBag.ShowStatus = (int)ReturnSaleOrderStatus.WaitInStock;
            return View();
        }

        public ActionResult StockIn(string code)
        {
            var order = _returnOrderFacade.GetReturnSaleOrderByCode(code);
            ViewBag.SaleOrder = JsonConvert.SerializeObject(order);
            return View();
        }

        [HttpPost]
        public ActionResult StockIn(ReturnBatchOrderModel model)
        {
            _returnOrderFacade.InStock(model);
            return Json(new { success = true });
        }
        #endregion

        #region  零售综合查询
        /// <summary>
        /// 综合查询
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchIndex()
        {
            ViewBag.OrderType = JsonConvert.SerializeObject(typeof(OrderType).GetValueToDescription().ToArray());
            ViewBag.OrderStatus = JsonConvert.SerializeObject(typeof(SaleOrderStatus).GetValueToDescription().ToArray());
            ViewBag.OrderRoStatus = JsonConvert.SerializeObject(typeof(ReturnSaleOrderStatus).GetValueToDescription().ToArray());
            return View();
        }
        /// <summary>
        ///  零售综合查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult GetCompositeSaleOrders(Pager page, SearchSaleOrder condition)
        {
            if (condition.StoreId.IsNullOrEmpty())
                condition.StoreId = _context.CurrentAccount.CanViewStores;
            var rows = _saleOrderFacade.GetSaleOrderListDetail(page, condition);
            ViewBag.OrderType = (int)OrderType.Order;
            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("零售单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total });
        }

        #endregion
    }
}