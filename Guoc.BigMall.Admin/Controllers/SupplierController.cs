using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.ViewObject;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class SupplierController:Controller
    {
        IDBContext _db;
        ISupplierFacade _supplierFacade;
        public SupplierController(IDBContext db, ISupplierFacade supplierFacade) {

            this._db = db;
            this._supplierFacade = supplierFacade;
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadData(Pager page, string name, string code)
        {
            var rows = _supplierFacade.GetPageList(page, name, code);

            return Json(new { success = true, data = rows, total = page.Total }, JsonRequestBehavior.AllowGet);
        }

    }
}