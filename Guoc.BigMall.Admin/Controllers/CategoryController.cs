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
    }
}