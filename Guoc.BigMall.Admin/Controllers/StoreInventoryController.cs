using Guoc.BigMall.Infrastructure.File;
using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Infrastructure.Extension;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class StoreInventoryController : Controller
    {

        IContextFacade _context;
        IDBContext _db;
        IStoreInventoryFacade _storeInventoryFacade;
        IExcel _excelService;
        IProductPriceFacade _productPriceFacade;
        public StoreInventoryController(IContextFacade context, IDBContext db, IStoreInventoryFacade storeInventoryFacade, IExcel excel, IProductPriceFacade productPriceFacade)
        {
            this._context = context;
            this._db = db;
            this._storeInventoryFacade = storeInventoryFacade;
            this._excelService = excel;
            this._productPriceFacade = productPriceFacade;
        }
        [Permission]
        public ActionResult Index()
        {
            return View();
        }
        private void SetCurrentStore()
        {
            ViewBag.View = _context.CurrentAccount.ShowSelectStore() ? "true" : "false";
            //ViewBag.StoreId = _context.CurrentAccount.StoreId;
            //ViewBag.StoreName = _context.CurrentAccount.StoreName;
        }

        public ActionResult LoadData(Pager page, SearchStoreInventory condition)
        {
            // if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            page.IsPaging = condition.ToExcel ? false : true;
            var rows = _storeInventoryFacade.GetPageList(page, condition);
            if (condition.ToExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("库存查询报表_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }
            return Json(new { success = true, data = rows, total = page.Total, sum = page.SumColumns });
        }

        public ActionResult History()
        {
            ViewBag.View = _context.CurrentAccount.ShowSelectStore() ? "true" : "false";
            //ViewBag.StoreId = _context.CurrentAccount.StoreId;
            ViewBag.StoreName = _context.CurrentAccount.StoreName;
            return View();
        }
        public JsonResult LoadDataHistory(Pager page, SearchStoreInventoryHistory condition)
        {
            // if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            var rows = _storeInventoryFacade.GetPageHistoryList(page, condition);

            return Json(new { success = true, data = rows, total = page.Total, sum = page.SumColumns });
        }
        public ActionResult Batch()
        {
            ViewBag.View = _context.CurrentAccount.ShowSelectStore() ? "true" : "false";
            //ViewBag.StoreId = _context.CurrentAccount.StoreId;
            ViewBag.StoreName = _context.CurrentAccount.StoreName;
            return View();
        }
        public JsonResult LoadDataBatch(Pager page, SearchStoreInventoryBatch condition)
        {
            // if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            var rows = _storeInventoryFacade.GetPageBatchList(page, condition);

            return Json(new { success = true, data = rows, total = page.Total });
        }
        public ActionResult Down(SearchStoreInventory condition)
        {
            var rows = _storeInventoryFacade.GetSNCodeProduct(condition);
            var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
            var fileName = string.Format("库存商品_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
            return File(data, "application/ms-excel", fileName);
        }

        public ActionResult ProductPriceIndex()
        {
            return View();
        }

        public ActionResult QueryStoreProductPrice(Pager page, SearchStoreProductPrice condition)
        {
            if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            var rows = this._productPriceFacade.QueryStoreProductPrice(page, condition);
            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("库存商品价格_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult InventorySummary()
        {
            return View();
        }

        public ActionResult LoadInventorySummary(Pager page, SearchInventorySummary condition)
        {
            if (condition.StoreIds.IsNullOrEmpty())
                condition.StoreIds = _context.CurrentAccount.CanViewStores;

            var rows = this._storeInventoryFacade.GetInventorySummary(page, condition);

            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = "库存汇总_{0}.xlsx".FormatWith(DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total });
        }
    }
}