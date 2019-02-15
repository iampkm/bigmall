using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Admin.Controllers;
namespace Guoc.BigMall.Admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // 系统初始化
            BootStrap.InitAutofacContainer();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            var httpStatusCode = (exception is HttpException) ? (exception as HttpException).GetHttpCode() : 500;
            Response.Clear();
            Server.ClearError();
            Response.TrySkipIisCustomErrors = true;
            //var routeData = new RouteData();
            //routeData.Values.Add("controller", "Common");
            switch (httpStatusCode)
            {
                case 404:
                    //routeData.Values.Add("action", "PageNotFound");
                    Response.Redirect("/Common/PageNotFound");
                    break;
                default:
                    //routeData.Values["action"] = "Error";
                    Response.Redirect("/Common/Error?errorMsg=" + (exception is FriendlyException ? exception.Message : string.Empty));
                    var log = AppContext.Current.Resolve<ILogger>();
                    log.Error(exception);
                    break;
            }

            //IController errorController = AppContext.Current.Resolve<CommonController>();
            //errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }
    }
}
