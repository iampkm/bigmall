using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper.DBContext;
using Newtonsoft.Json;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Admin.Services;

namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class StoreController : Controller
    {
        IContextFacade _context;
        IStoreFacade _storeFacade;
        public StoreController(IContextFacade context, IStoreFacade storeFacade)
        {
            this._context = context;
            this._storeFacade = storeFacade;
        }

        /// <summary>
        /// 仅返回当前登录账户有权限的门店。
        /// </summary>
        public JsonResult LoadStoreTree(string name, string code, string groupId)
        {
            // 按区域和门店，加载二级树形结构
            var storeTree = this._storeFacade.LoadStore(_context.CurrentAccount.CanViewStores, name, code, groupId).ToArray();
            return Json(storeTree);
        }

        /// <summary>
        /// 返回所有门店。
        /// </summary>
        public JsonResult LoadAllStoreTree(string name, string code, string groupId)
        {
            // 按区域和门店，加载二级树形结构
            var storeTree = this._storeFacade.LoadStore(null, name, code, groupId).ToArray();
            return Json(storeTree);
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadData(Pager page, StoreSearch searchArgs)
        {
            searchArgs.StoreIds = _context.CurrentAccount.CanViewStores;

            var rows = _storeFacade.GetPageList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Create(StoreModel model)
        {
            model.CreatedBy = _context.CurrentAccount.AccountId;
            model.CreatedOn = DateTime.Now;
            _storeFacade.Create(model);
            return Json(new { success = true });
        }
        public ActionResult Edit(int id)
        {
            var model = _storeFacade.GetById(id);
            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(StoreModel model)
        {
            model.CreatedBy = _context.CurrentAccount.AccountId;
            model.CreatedOn = DateTime.Now;
            _storeFacade.Edit(model);
            return Json(new { success = true });
        }

        public JsonResult Delete(string ids)
        {
            _storeFacade.Delete(ids);
            return Json(new { success = true });
        }
    }
}