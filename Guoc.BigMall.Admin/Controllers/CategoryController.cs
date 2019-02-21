using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Guoc.BigMall.Admin.Controllers
{
      [Permission]
    public class CategoryController : Controller
    {
        ICategoryFacade _categoryFacade;

        public CategoryController(ICategoryFacade categoryFacade)
        {
            this._categoryFacade = categoryFacade;
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadCategoryTree(string parentCode)
        {
            var childNodes = _categoryFacade.GetCategoryTree(parentCode == string.Empty ? null : parentCode);
            return Json(childNodes);
        }

        public JsonResult Create(string parentCode, string categoryName)
        {
            var id = _categoryFacade.Create(parentCode, categoryName);
            return Json(new { success = true, id = id });
        }

        public JsonResult Edit(string code, string categoryName)
        {
            _categoryFacade.Update(code, categoryName);
            return Json(new { success = true });
        }

        public JsonResult Remove(string code)
        {
            _categoryFacade.Delete(code);
            return Json(new { success = true });
        }
    }
}