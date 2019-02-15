using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class SystemSetupController : Controller
    {
        // GET: SystemSetup
        private readonly IAuthenticationService _authenticationService;
        ISystemSetupFacade _systemSetupFacade;
        IContextFacade _context;
        IDBContext _db;
        public  SystemSetupController(IAuthenticationService authenticationService, IContextFacade context,IDBContext db, ISystemSetupFacade systemSetupFacade)
        {

            this._systemSetupFacade = systemSetupFacade;
            this._context = context;
            this._authenticationService = authenticationService;
            this._db = db;
            
        }
    
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult LoadData(Pager page, string name, string value)
        {
            var rows = _systemSetupFacade.GetList(page, name, value);
            return Json(new { success = true, data = rows, total = page.Total }, JsonRequestBehavior.AllowGet);
        
        }

      
        public ActionResult Create()
        { 
            return View();
        }
     
        [HttpPost]
        public JsonResult Create(SystemSetupModel model)
        {

            _systemSetupFacade.Create(model);
            return Json(new { success = true });
        }
       
        public ActionResult Edit(int id)
        {
            var model = _db.Table<SystemSetup>().FirstOrDefault(n => n.Id == id);  
            
            return View(model);
        }
      
        [HttpPost]
        public JsonResult Edit(SystemSetupModel model)
        {

            _systemSetupFacade.Edit(model);
            return Json(new { success = true });
        }

      
        public JsonResult Delete(string ids)
        {

            _systemSetupFacade.Delete(ids);
            return Json(new { success = true });
        }
    }
}