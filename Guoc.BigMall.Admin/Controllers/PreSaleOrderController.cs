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

namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class PreSaleOrderController : Controller
    {
        IExcel _excelService;
        IDBContext _db;
        IPreSaleOrderFacade _PreSaleOrderFacade;
        IReturnPreSaleOrderFacade _returnOrderFacade;
        IContextFacade _context;
        public PreSaleOrderController(IDBContext db, IPreSaleOrderFacade PreSaleOrderFacade, IExcel iexcel, IContextFacade context, IReturnPreSaleOrderFacade returnOrderFacade)
        {
            this._excelService = iexcel;
            this._db = db;
            this._PreSaleOrderFacade = PreSaleOrderFacade;
            this._context = context;
            this._returnOrderFacade = returnOrderFacade;
        }
        #region 预售订单
        public ActionResult Index()
        {
            ViewBag.ShowStatus = (int)SaleOrderStatus.Create;
            ViewBag.OrderType = (int)OrderType.Order;
            return View();
        }
        public ActionResult GetPreSaleOrderList(Pager page, SearchSaleOrder condition)
        {
            //if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            if (condition.StoreId.IsNullOrEmpty())
                condition.StoreId = _context.CurrentAccount.CanViewStores;

            var rows = _PreSaleOrderFacade.GetPreSaleOrders(page, condition);

            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("预售订单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
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
            _PreSaleOrderFacade.CreatePreSaleOrder(model);

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

            _PreSaleOrderFacade.AuditedPreSaleOrder(model);

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult Reject(SaleOrderRejectModel model)
        {
            if (model.OrderCode.IsEmpty())
                return Json(new { success = false, error = "订单编号不能为空" });

            _PreSaleOrderFacade.RejectPreSaleOrder(model);

            return Json(new { success = true });
        }

        public ActionResult ShowDetail(string code)
        {
            var order = _PreSaleOrderFacade.GetPreSaleOrderModelByCode(code);
            ViewBag.PreSaleOrder = JsonConvert.SerializeObject(order);
            ViewBag.OutStock = (int)SaleOrderStatus.OutStock;
            ViewBag.RejectAudit = (int)SaleOrderStatus.Create;
            return View();
        }

        public ActionResult Edit(string code)
        {
            var order = _PreSaleOrderFacade.GetPreSaleOrderModelByCode(code);
            ViewBag.PreSaleOrder = JsonConvert.SerializeObject(order);
            return View();
        }

        [HttpPost]
        public JsonResult Edit(SaleOrderModel PreSaleOrderModel)
        {
            _PreSaleOrderFacade.UpdatePreSaleOrder(PreSaleOrderModel);
            return Json(new { success = true });
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
            var order = _PreSaleOrderFacade.GetPreSaleOrderModelByCode(code);
            ViewBag.PreSaleOrder = JsonConvert.SerializeObject(order);
            return View();
        }

        [HttpPost]
        public ActionResult StockOut(PreSaleOrderOutStockModel model)
        {
            _PreSaleOrderFacade.OutStock(model.OrderCode);
            return Json(new { success = true });
        }

        public JsonResult AbandonPreSaleOrder(SaleOrderAbandonModel model)
        {
            _PreSaleOrderFacade.AbandonPreSaleOrder(model);
            return Json(new { success = true });
        }

        public ActionResult ConvertToSaleOrderIndex()
        {
            ViewBag.ShowStatus = (int)SaleOrderStatus.OutStock;
            return View();
        }

        public ActionResult ConvertToSaleOrder(string code)
        {
            var order = _PreSaleOrderFacade.GetPreSaleOrderModelByCode(code);
            ViewBag.PreSaleOrder = JsonConvert.SerializeObject(order);
            return View();
        }

        [HttpPost]
        public ActionResult ConvertToSaleOrder(PreSaleOrderAbandonModel model)
        {
            _PreSaleOrderFacade.ConvertToSaleOrder(model.OrderCode, model.FJCode);
            return Json(new { success = true });
        }


        #endregion

        #region 预售退单
        /// <summary>
        /// 预售退单
        /// </summary>
        /// <param name="page"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult GetReturnOrderList(Pager page, SearchSaleOrder condition)
        {
            //condition.RoStatus = new[] { ReturnSaleOrderStatus.WaitInStock, ReturnSaleOrderStatus.InStock };

            //if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            if (condition.StoreId.IsNullOrEmpty())
                condition.StoreId = _context.CurrentAccount.CanViewStores;

            var rows = _returnOrderFacade.GetReturnPreSaleOrders(page, condition);

            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("预售订单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total });
        }
        public ActionResult GetPreSaleOrderByFJCode(string code)
        {
            var rows = _PreSaleOrderFacade.GetPreSaleOrderModelByFJCode(code);
            return Json(new { success = true, data = rows });
        }
        public ActionResult RoList()
        {
            //ViewBag.OrderType = (int)OrderType.Return;
            return View();
        }

        public ActionResult CreateRo()
        {
            ViewBag.StatusDesc = SaleOrderStatus.Create.Description();
            ViewBag.CreatedBy = _context.CurrentAccount.AccountId;
            ViewBag.CreatedByName = _context.CurrentAccount.NickName;
            ViewBag.DefaultStoreId = _context.CurrentAccount.StoreArray.FirstOrDefault();
            return View();
        }


        [HttpPost]
        public JsonResult CreateRo(SaleOrderModel returnOrderModel)
        {
            returnOrderModel.CreatedBy = _context.CurrentAccount.AccountId;
            returnOrderModel.UpdatedByName = _context.CurrentAccount.NickName;
            _returnOrderFacade.CreateReturnPreSaleOrder(returnOrderModel);
            return Json(new { success = true });
        }

        public JsonResult AbandonReturnPreSaleOrder(SaleOrderAbandonModel model)
        {
            _returnOrderFacade.AbandonReturnPreSaleOrder(model);
            return Json(new { success = true });
        }

        #endregion

        #region 入库
        public ActionResult StockInIndex()
        {
            ViewBag.ShowRoStatus = ReturnSaleOrderStatus.WaitInStock.Value();
            return View();
        }

        public ActionResult StockIn(string code)
        {
            var order = _returnOrderFacade.GetReturnPreSaleOrderByCode(code);
            ViewBag.PreSaleOrder = JsonConvert.SerializeObject(order);
            return View();
        }

        [HttpPost]
        public ActionResult StockIn(ReturnBatchOrderModel model)
        {
            _returnOrderFacade.InStock(model);
            return Json(new { success = true });
        }
        #endregion

        #region 综合查询
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

        public ActionResult GetCompositePreSaleOrders(Pager page, SearchSaleOrder condition)
        {
            //if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            if (condition.StoreId.IsNullOrEmpty())
                condition.StoreId = _context.CurrentAccount.CanViewStores;

            var rows = _PreSaleOrderFacade.GetPreSaleOrderListDetail(page, condition);

            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("预售单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total });
        }
        #endregion
    }
}