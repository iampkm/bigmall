using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
namespace Guoc.BigMall.Api.Services
{
    public class ModelValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {

            if (actionContext.ModelState.IsValid == false)
            {
                string error = string.Empty;
                foreach (var key in actionContext.ModelState.Keys)
                {
                    var state = actionContext.ModelState[key];
                    if (state.Errors.Any())
                    {
                        error = state.Errors.First().ErrorMessage;
                        break;
                    }
                }                
                actionContext.Response =
                    actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, new { code = 400, message = error });

            }
        }

    }
}