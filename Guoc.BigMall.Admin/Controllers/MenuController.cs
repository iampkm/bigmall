using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Guoc.BigMall.Infrastructure.Extension;
using Dapper.DBContext;
using Guoc.BigMall.Domain.Entity;

namespace Guoc.BigMall.Admin.Controllers
{
      [Permission]
    public class MenuController : Controller
    {
        // GET: Menu
        private readonly IAuthenticationService _authenticationService;
        IMenuFacade _menuFacade;
       // IRoleFacade _roleFcade;
        IContextFacade _context;
        IDBContext _db;
        public MenuController(IAuthenticationService authenticationService, IMenuFacade menuFacade,  IContextFacade context,IDBContext db)
        {
           
            this._menuFacade = menuFacade;
            this._context = context;
            this._authenticationService = authenticationService;
            this._db = db;
            
        }
         [Permission]
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult LoadData(Pager page, string name)
        {
            var rows = _menuFacade.GetList(page, name);
            return Json(new { success = true, data = rows, total = page.Total }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadMenu()
        {
            var tree = _menuFacade.LoadMenuTree();
            return Json(new { success = true, data = tree }, JsonRequestBehavior.AllowGet);
        }

        [Permission]
        public ActionResult Create()
        {
            var dic = typeof(MenuUrlType).GetValueToDescription();
            ViewBag.menutypes = dic;
            return View();
        }
        [Permission]
        [HttpPost]
        public JsonResult Create(MenuModel model)
        {
            
            _menuFacade.Create(model);
            return Json(new { success = true });
        }
        [Permission]
        public ActionResult Edit(int id)
        {
            var model = _db.Table<Menu>().FirstOrDefault(n => n.Id == id);  // ._query.Find<Menu>(id);
            var parentModel = _db.Table<Menu>().FirstOrDefault(n => n.ParentId == model.ParentId);// _query.Find<Menu>(model.ParentId);
            var parentName = "";
            if (parentModel != null)
            {
                parentName = parentModel.Name;
            }
            ViewBag.parentName = parentName;
            // 枚举
            var dic = typeof(MenuUrlType).GetValueToDescription();
            ViewBag.menutypes = dic;
            return View(model);
        }
        [Permission]
        [HttpPost]
        public JsonResult Edit(MenuModel model)
        {
            
            _menuFacade.Edit(model);
            return Json(new { success = true });
        }

        [Permission]
        public JsonResult Delete(string ids)
        {
            
            _menuFacade.Delete(ids);
            return Json(new { success = true });
        }

    }
}