using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.Search;
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
    public class BrandController : Controller
    {
        IContextFacade _context;
        IDBContext _db;
        IBrandFacade _brandFacade;

        public BrandController(IContextFacade context, IDBContext db, IBrandFacade brandFacade)
        {
            this._context = context;
            this._db = db;
            this._brandFacade = brandFacade;

        }
        // GET: Brand
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult LoadData(Pager page, BrandSearch search)
        {
            var rows = _brandFacade.GetList(page, search);
            return Json(new { success = true, data = rows, total = page.Total }, JsonRequestBehavior.AllowGet);
        }
    }
}