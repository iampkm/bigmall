using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Extension;
using Newtonsoft.Json;
using System.Web.Mvc;
namespace Guoc.BigMall.Admin.Controllers
{

    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountFacade _accountFacade;
        private readonly IRoleFacade _roleFacade;
        private readonly IStoreFacade _storeFacade;
        IContextFacade _context;
        public AccountController(IContextFacade contextService, IAuthenticationService authenticationService, IAccountFacade accountFacade, IRoleFacade roleFacade, IStoreFacade storeFacade)
        {
            this._context = contextService;
            this._authenticationService = authenticationService;
            this._accountFacade = accountFacade;
            this._roleFacade = roleFacade;
            this._storeFacade = storeFacade;
        }

        [Permission]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadData(Pager page, searchAccount request)
        {
            var rows = _accountFacade.GetPageList(page, 0, request.UserName, request.NickName, request.StoreId);
            return Json(new { success = true, data = rows, total = page.Total }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            model.IpAddress = Request.UserHostAddress;
            var account = this._accountFacade.Login(model);
            var accountInfo = JsonConvert.SerializeObject(account);
            this._authenticationService.SignIn(account.UserName, accountInfo, model.RememberMe);
            // return RedirectToAction("DashBoard", "Home"); 
            var url = string.IsNullOrEmpty(model.ReturnUrl) ? "/Home/DashBoard" : model.ReturnUrl;
            return Json(new { success = true, returnUrl = url });
        }

        public ActionResult LogOut()
        {
            _authenticationService.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [Permission]
        public ActionResult Create()
        {
            //加载权限资源  
            ViewBag.View = _context.CurrentAccount.ShowSelectStore() ? "true" : "false";
            ViewBag.Roles = _roleFacade.QueryAll();
           // ViewBag.StoreTags = _storeFacade.GetStoreTags();
            return View();
        }

        [HttpPost]
        public JsonResult Create(AccountCreateModel model)
        {
            _accountFacade.Create(model);
            return Json(new { success = true });
        }

        [Permission]
        public ActionResult Edit(int id)
        {
            var model = _accountFacade.GetAccountById(id);
            ViewBag.Roles = _roleFacade.QueryAll();
            //ViewBag.StoreName = "";
            //ViewBag.CanViewStores = "";
            //ViewBag.CanViewStoreIds = "";
            //加载权限资源  
            ViewBag.View = _context.CurrentAccount.ShowSelectStore() ? "true" : "false";
            //if (model.StoreId > 0)
            //{
            //    ViewBag.StoreName = _query.Find<Store>(model.StoreId).Name;
            //    ViewBag.StoreId = model.StoreId.ToString();
            //}
            //if (!string.IsNullOrEmpty(model.CanViewStores))
            //{
            //    var storeIds = model.CanViewStores.Split(',').ToIntArray();
            //    var nameArray = _storeFacade.Find<Store>(storeIds).Select(n => n.Name).ToArray();
            //    ViewBag.CanViewStores = string.Join(",", nameArray);
            //    var idArray = _query.Find<Store>(storeIds).Select(n => n.Id).ToArray();
            //    ViewBag.CanViewStoreIds = string.Join(",", idArray); ;
            //}

            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(AccountEditModel model)
        {
            _accountFacade.Edit(model);
            return Json(new { success = true });
        }

        [Permission]
        public JsonResult Actived(string ids)
        {
            _accountFacade.ActiveAccount(ids.Split(',').ToIntArray());
            return Json(new { success = true });
        }

        [Permission]
        public JsonResult Disabled(string ids)
        {
            _accountFacade.DisabledAccount(ids.Split(',').ToIntArray());
            return Json(new { success = true });
        }

        [Permission]
        public JsonResult Deleted(int id)
        {
            _accountFacade.DeleteAccount(id);
            return Json(new { success = true });
        }

        [Permission]
        public JsonResult ResetPassword(string ids)
        {
            _accountFacade.ResetPassword(ids.Split(',').ToIntArray());
            return Json(new { success = true });
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangePassword(string oldPassword, string newPassword)
        {
            // 获取当前用户
            var contextService = AppContext.Current.Resolve<IContextFacade>();
            _accountFacade.ChangePassword(contextService.CurrentAccount.AccountId, oldPassword, newPassword);
            return Json(new { success = true });
        }
    }
}