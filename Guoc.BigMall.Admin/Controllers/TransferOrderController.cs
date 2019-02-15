using Guoc.BigMall.Application;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.ValueObject;
using Guoc.BigMall.Infrastructure.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Guoc.BigMall.Admin.Services;
using Newtonsoft.Json;
using Guoc.BigMall.Infrastructure.Utils;
using Guoc.BigMall.Infrastructure.File;

namespace Guoc.BigMall.Admin.Controllers
{
    public class TransferOrderController : Controller
    {
        IExcel _excelService;
        IContextFacade _context = null;
        ITransferOrderFacade _transferOrderFacade = null;
        public TransferOrderController(IContextFacade context, ITransferOrderFacade transferOrderFacade, IExcel excel)
        {
            this._context = context;
            this._excelService = excel;
            this._transferOrderFacade = transferOrderFacade;
        }

        public ActionResult Index()
        {
            ViewBag.TypeList = typeof(TransferType).GetValueToDescription();
            ViewBag.StatusList = typeof(TransferStatus).GetValueToDescription();
            return View();
        }

        public JsonResult LoadList(Pager page, TransferOrderSearch searchArgs)
        {
            var rows = _transferOrderFacade.GetPageList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult LoadDetailList(Pager page, TransferOrderSearch searchArgs)
        {
            var rows = _transferOrderFacade.GetTransferOrderDetailList(page, searchArgs);

            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("调拨单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult CreateIndex()
        {
            ViewBag.TypeList = typeof(TransferType).GetValueToDescription().Where(n => n.Key != TransferType.Allocate.Value()).ToList();
            return View();
        }

        public JsonResult LoadCreatedList(Pager page, TransferOrderSearch searchArgs)
        {
            searchArgs.Status = TransferStatus.Initial;
            var rows = _transferOrderFacade.GetPageList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult Create()
        {
            ViewBag.CreatedBy = _context.CurrentAccount.AccountId;
            ViewBag.CreatedByName = _context.CurrentAccount.NickName;
            ViewBag.StatusDesc = TransferStatus.Initial.Description();
            ViewBag.DefaultStoreId = _context.CurrentAccount.StoreArray.FirstOrDefault();
            return View();
        }

        [HttpPost]
        public JsonResult Create(TransferCreateModel model)
        {
            _transferOrderFacade.Create(model);
            return Json(new { success = true, code = model.Code, id = model.Id });
        }

        public ActionResult Edit(int id)
        {
            var transferOrder = _transferOrderFacade.GetTransferOrderById(id);
            Ensure.In(transferOrder.Status, new[] { TransferStatus.Initial, TransferStatus.WaitAudit, TransferStatus.Reject }, "调拨单状态必须是“初始”、“待审”、“驳回”才允许编辑。");
            ViewBag.Model = JsonConvert.SerializeObject(transferOrder);
            return View();
        }

        [HttpPost]
        public JsonResult Edit(TransferEditModel model)
        {
            _transferOrderFacade.Edit(model);
            return Json(new { success = true, code = model.Code, id = model.Id });
        }

        public ActionResult View(int id)
        {
            var transferOrder = _transferOrderFacade.GetTransferOrderById(id);
            ViewBag.Model = JsonConvert.SerializeObject(transferOrder);
            return View();
        }

        public JsonResult ApplyAudit(int id)
        {
            _transferOrderFacade.ApplyAudit(id);
            return Json(new { success = true });
        }

        public ActionResult AuditIndex()
        {
            return View();
        }

        public JsonResult LoadAuditList(Pager page, TransferOrderSearch searchArgs)
        {
            searchArgs.Status = TransferStatus.WaitAudit;
            searchArgs.Type = TransferType.StoreApply;
            var rows = _transferOrderFacade.GetPageList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult Audit(int id)
        {
            var transferOrder = _transferOrderFacade.GetTransferOrderById(id);
            ViewBag.Model = JsonConvert.SerializeObject(transferOrder);
            return View();
        }

        public JsonResult PassAudit(int id, string auditRemark)
        {
            _transferOrderFacade.PassAudit(id, auditRemark);
            return Json(new { success = true });
        }

        public JsonResult RejectAudit(int id, string auditRemark)
        {
            _transferOrderFacade.RejectAudit(id, auditRemark);
            return Json(new { success = true });
        }

        public ActionResult StockOutIndex()
        {
            ViewBag.TypeList = typeof(TransferType).GetValueToDescription();
            return View();
        }

        public JsonResult LoadStockOutList(Pager page, TransferOrderSearch searchArgs)
        {
            searchArgs.Status = TransferStatus.WaitShipping;
            if (searchArgs.FromStoreId.IsNullOrEmpty())
                searchArgs.FromStoreId = _context.CurrentAccount.CanViewStores;

            var rows = _transferOrderFacade.GetPageList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult StockOut(int id)
        {
            var transferOrder = _transferOrderFacade.GetTransferOrderById(id);
            Ensure.EqualThan(transferOrder.Status, TransferStatus.WaitShipping, "调拨单状态必须是“待发货”才能出库。");
            ViewBag.Model = JsonConvert.SerializeObject(transferOrder);
            return View();
        }

        [HttpPost]
        public JsonResult OutStock(TransferStockOutModel model)
        {
            _transferOrderFacade.OutStock(model);
            return Json(new { success = true });
        }

        public ActionResult StockInIndex()
        {
            ViewBag.TypeList = typeof(TransferType).GetValueToDescription();
            return View();
        }

        public JsonResult LoadStockInList(Pager page, TransferOrderSearch searchArgs)
        {
            searchArgs.Status = TransferStatus.WaitReceiving;
            if (searchArgs.ToStoreId.IsNullOrEmpty())
                searchArgs.ToStoreId = _context.CurrentAccount.CanViewStores;

            var rows = _transferOrderFacade.GetPageList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult StockIn(int id)
        {
            var transferOrder = _transferOrderFacade.GetTransferOrderById(id);
            Ensure.EqualThan(transferOrder.Status, TransferStatus.WaitReceiving, "调拨单状态必须是“待收货”才能入库。");
            ViewBag.Model = JsonConvert.SerializeObject(transferOrder);
            return View();
        }

        [HttpPost]
        public JsonResult InStock(TransferStockInModel model)
        {
            _transferOrderFacade.InStock(model);
            return Json(new { success = true });
        }

        /// <summary>
        /// 打印出库单
        /// </summary>
        /// <param name="ids">多个ID 逗号分隔</param>
        /// <returns></returns>
        public ActionResult Print(string ids)
        {
            var model = _transferOrderFacade.GetPrintList(ids);

            return PartialView("TransferOrderPrintTemplate", model);
        }
    }
}