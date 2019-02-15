using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
namespace Guoc.BigMall.Admin.Services
{
    public class ModelValidationFilterAttribute : ActionFilterAttribute
    {  
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var viewData = filterContext.Controller.ViewData;
            if (!viewData.ModelState.IsValid)
            {
                var errorMessage = "";
                foreach (var key in viewData.ModelState.Keys)
                {
                    var state = viewData.ModelState[key];
                    if (state.Errors.Any())
                    {
                        errorMessage = state.Errors.First().ErrorMessage;
                        break;
                    }
                }
                // ajax 直接返回错误验证结果
                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new { success = false, error = errorMessage },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new ContentResult() { Content = string.Format("参数异常:{0}", errorMessage) };
                }
            }

            base.OnActionExecuting(filterContext);
        }

    }
}