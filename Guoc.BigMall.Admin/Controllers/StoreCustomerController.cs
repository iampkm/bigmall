using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.ViewObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class StoreCustomerController : Controller
    {

        private readonly IAuthenticationService _authenticationService;
        //IStoreCustomerFacade _storeCustomerFacade;
        // private IQuery _query;
        IContextFacade _context;
        public StoreCustomerController(IContextFacade contextService, IAuthenticationService authenticationService)//,  IStoreCustomerFacade storeCustomerFacade)
        {
            this._context = contextService;
            this._authenticationService = authenticationService;
            //this._storeCustomerFacade=storeCustomerFacade;

        }

        public ActionResult Index()
        {
            return View();
        }
        //public JsonResult LoadData(Pager page, string name)
        //{
        //    var rows = _storeCustomerFacade.GetList(page, name);
        //    return Json(new { success = true, data = rows, total = page.Total }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult LoadMenu()
        //{
        //    var tree = _storeCustomerFacade.LoadMenuTree();
        //    return Json(new { success = true, data = tree }, JsonRequestBehavior.AllowGet);
        //}

        //[Permission]
        //public ActionResult Create()
        //{
        //    var dic = typeof(MenuUrlType).GetValueToDescription();
        //    ViewBag.menutypes = dic;
        //    return View();
        //}
        //[Permission]
        //[HttpPost]
        //public JsonResult Create(MenuModel model)
        //{

        //    _storeCustomerFacade.Create(model);
        //    return Json(new { success = true });
        //}
        //[Permission]
        //public ActionResult Edit(int id)
        //{
        //    var model = _db.Table<Menu>().FirstOrDefault(n => n.Id == id);  // ._query.Find<Menu>(id);
        //    var parentModel = _db.Table<Menu>().FirstOrDefault(n => n.ParentId == model.ParentId);// _query.Find<Menu>(model.ParentId);
        //    var parentName = "";
        //    if (parentModel != null)
        //    {
        //        parentName = parentModel.Name;
        //    }
        //    ViewBag.parentName = parentName;
        //    // 枚举
        //    var dic = typeof(MenuUrlType).GetValueToDescription();
        //    ViewBag.menutypes = dic;
        //    return View(model);
        //}
        //[Permission]
        //[HttpPost]
        //public JsonResult Edit(MenuModel model)
        //{

        //    _storeCustomerFacade.Edit(model);
        //    return Json(new { success = true });
        //}

        //[Permission]
        //public JsonResult Delete(string ids)
        //{

        //    _storeCustomerFacade.Delete(ids);
        //    return Json(new { success = true });
        //}
    }
}