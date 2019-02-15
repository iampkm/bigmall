using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class AreaController : Controller
    {

        IAreaFacade _areaFacade;
        public AreaController(IAreaFacade AreaFacade)
        {
            this._areaFacade = AreaFacade;
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadAreaTree(string parentCode)
        {
            var childNodes = _areaFacade.GetAreaTree(parentCode == string.Empty ? null : parentCode);           
            return Json(childNodes);
        }
    }
}