using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Log;
namespace Guoc.BigMall.Admin.Services
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {            
            //记录系统日志
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");
            var log = AppContext.Current.Resolve<ILogger>();
            if (filterContext.Exception is FriendlyException)
            {
                var friendException = filterContext.Exception as FriendlyException;
                log.Info(friendException.Message);
                if (friendException.Message == "账号已过期")
                {
                    filterContext.ExceptionHandled = true;
                    UrlHelper url = new UrlHelper(filterContext.RequestContext);
                    filterContext.Result = new RedirectResult(url.Action("Login", "Account"));
                    return;
                }
            }
            else
            {
                log.Error(filterContext.Exception);
            }
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = new JsonResult
                {
                    Data = new { success = false, error = filterContext.Exception.Message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                base.OnException(filterContext);
            } 
        }
    }
}