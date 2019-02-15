using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Guoc.BigMall.Infrastructure;
using Dapper.DBContext;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Application;
namespace Guoc.BigMall.Admin.Services
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class PermissionAttribute : AuthorizeAttribute
    {
        IContextFacade _context;
        IRoleFacade _roleFacade;
        public PermissionAttribute()
        {
            this._context = AppContext.Current.Resolve<IContextFacade>();
            this._roleFacade = AppContext.Current.Resolve<IRoleFacade>();
        }

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //   // base.OnActionExecuting(filterContext);           
        //    if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
        //    { 
        //        // not login user
        //        filterContext.Result = new RedirectResult("/Account/Login" );
        //        return;
        //    }
        //    string controllerName = filterContext.RouteData.Values["controller"].ToString();
        //    string actionName = filterContext.RouteData.Values["action"].ToString();
        //    string requestURL = "/" + controllerName + "/" + actionName;
        //}

        //public void OnException(ExceptionContext filterContext)
        //{
        //    filterContext.ExceptionHandled = true;
        //    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
        //    {
        //        filterContext.Result = new JsonResult
        //        {
        //            Data = new { success = false, error = filterContext.Exception.Message },
        //            JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //        };
        //    }
        //    else {
        //        filterContext.Result = new RedirectResult("/Error.html");
        //    }
        //}



        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            //验证登录
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            ////验证访问资源权限
            //var urlSegments = httpContext.Request.Url.Segments;
            //var url = string.Join("", urlSegments, 0, urlSegments.Length - (urlSegments.Length > 3 ? 1 : 0)).TrimEnd('/');
            //if (!_roleFacade.HavePower(_context.CurrentAccount.RoleId, url))
            //    throw new FriendlyException("对不起您没有该项操作权限。");

            return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //current request is authenticated
            //var routeDate = filterContext.RouteData;
            //string controllerName = routeDate.Values["controller"].ToString();
            //string actionName = routeDate.Values["action"].ToString();
            //string requestURL = string.Format("/{0}/{1}", controllerName, actionName);
            // if admin return true

            // if did not setting requestUrl   return true
            // filterContext.Result = new HttpUnauthorizedResult();
            // if setting request url, but current user not has this url ,return false
            //if(string.Equals(requestURL,"",StringComparison.OrdinalIgnoreCase))
            //{
            //    filterContext.Result = new RedirectResult("/Home/Index");
            //    return;
            //}
            //var menuQuery = AppContext.Current.Resolve<IMenuQuery>();
            //var contextService = AppContext.Current.Resolve<IContextService>();
            //var query = AppContext.Current.Resolve<IQuery>();
            //var roleMenus = menuQuery.LoadMenu(contextService.CurrentAccount.RoleId).ToList();
            //if (!roleMenus.Any())
            //{
            //    //没有设置权限，可以访问所有
            //    return;
            //}
            //var allMenus = query.FindAll<Menu>();
            //var routeDate = filterContext.RouteData;
            //string controllerName = routeDate.Values["controller"].ToString();
            //string actionName = routeDate.Values["action"].ToString();
            //string requestURL = string.Format("/{0}/{1}", controllerName, actionName);
            //// 检查了 设置了权限，必须拥有该权限才能访问
            //if (query.Exists<Menu>(n => string.Equals(n.Url, requestURL, StringComparison.InvariantCultureIgnoreCase)))
            //{
            //    if (roleMenus.Exists(n => string.Equals(n.Url, requestURL, StringComparison.InvariantCultureIgnoreCase)))
            //    {
            //        // 可以访问
            //    }
            //    else
            //    {
            //        //拒绝访问
            //    }
            //}

            base.OnAuthorization(filterContext);

        }
    }
}