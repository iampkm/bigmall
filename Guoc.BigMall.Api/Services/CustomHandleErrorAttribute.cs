using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Log;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
namespace Guoc.BigMall.Api.Services
{
    public class CustomHandleErrorAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {

            if (actionExecutedContext.Exception != null)
            {
                var log = AppContext.Current.Resolve<ILogger>();
                var errMsg = actionExecutedContext.Exception.Message;
                if (actionExecutedContext.Exception is FriendlyException)
                {
                    log.Info(errMsg);
                }
                else
                {
                    log.Error(actionExecutedContext.Exception,"系统异常");
                    errMsg = "您的请求出现错误，请稍后重试!";
                }            
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError, new { code = 500, message = errMsg });
            }
        }
    }
}