using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class ProductController : Controller
    {

        IContextFacade _context;
        IDBContext _db;
        IProductFacade _productFacade;
        public ProductController(IContextFacade context, IDBContext db, IRoleFacade roleFacade, IProductFacade productFacade)
        {
            this._context = context;
            this._db = db;
            this._productFacade = productFacade;
        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadData(Pager page, SearchProduct search)
        {
            var rows = _productFacade.GetList(page, search);
            return Json(new { success = true, data = rows, total = page.Total, JsonRequestBehavior.AllowGet });
        }

        public JsonResult LoadSNCodeData(Pager page, SearchProduct search)
        {
            var rows = _productFacade.GetSNcodeList(page, search);
            return Json(new { success = true, data = rows, total = page.Total, JsonRequestBehavior.AllowGet });
        }

        public JsonResult LoadSupplierProduct(Pager page, SearchProduct search)
        {
            var rows = _productFacade.LoadSupplierProduct(page, search);
            return Json(new { success = true, data = rows, total = page.Total, JsonRequestBehavior.AllowGet });
        }

        public JsonResult LoadStoreProduct(Pager page, SearchProduct search)
        {
            var rows = _productFacade.LoadStoreProduct(page, search);
            return Json(new { success = true, data = rows, total = page.Total, JsonRequestBehavior.AllowGet });
        }

        public JsonResult LoadStoreGiftProduct(Pager page, SearchProduct search)
        {
            search.StoreId = search.StoreIdGift;
            var rows = _productFacade.LoadStoreProduct(page, search);
            return Json(new { success = true, data = rows, total = page.Total, JsonRequestBehavior.AllowGet });
        }

        public JsonResult LoadProduct(SearchProduct search)
        {
            var rows = _productFacade.LoadProduct(search);
            return Json(new { success = true, data = rows, JsonRequestBehavior.AllowGet });
        }
    }
}