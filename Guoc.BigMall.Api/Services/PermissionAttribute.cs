using Guoc.BigMall.Infrastructure.AuthJWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Security.Principal;
using System.IdentityModel.Tokens.Jwt;

namespace Guoc.BigMall.Api.Services
{
    /// <summary>
    ///  权限校验
    /// </summary>
    public class PermissionAttribute : AuthorizeAttribute
    {        
        /// <summary>
        /// 未授权
        /// </summary>
        /// <param name="actionContext"></param>
        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
           // actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "很抱歉，您无权访问，请先登录！");
            actionContext.Response = 
                actionContext.ControllerContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new { code = 401, message = "很抱歉，您无权访问，请先登录！" });
        }
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //if (!IsAuthorized(actionContext)) {
            //    HandleUnauthorizedRequest(actionContext);
            //}

            if (!IsUserAuthorized(actionContext))
            {
                HandleUnauthorizedRequest(actionContext);
            }

        }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            return CheckRequestAuth(actionContext);
        }

        /// <summary>
        ///  检查授权
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private bool CheckRequestAuth(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.GetValues("Token").Count()==0) {
                return false;
            }
            if (actionContext.Request.Headers.GetValues("appid").Count()==0)
            {
                return false;
            }
            // get value from header
            string token = actionContext.Request.Headers.GetValues("token").FirstOrDefault();
            string appid = actionContext.Request.Headers.GetValues("appid").FirstOrDefault();

            // 校验token是否合法
            if (token == "123abc" && appid == "111")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsUserAuthorized(HttpActionContext actionContext)
        {
            var authHeader = FetchFromHeader(actionContext); // fetch authorization token from header
           
            if (authHeader != null)
            {
                var auth = new AuthenticationModule();
                JwtSecurityToken userPayloadToken = auth.GenerateUserClaimFromJWT(authHeader);

                if (userPayloadToken != null)
                {

                    var identity = auth.PopulateUserIdentity(userPayloadToken);
                    string[] roles = { "All" };
                    var genericPrincipal = new GenericPrincipal(identity, roles);
                    Thread.CurrentPrincipal = genericPrincipal;
                    var authenticationIdentity = Thread.CurrentPrincipal.Identity as JWTAuthenticationIdentity;
                    if (authenticationIdentity != null && !String.IsNullOrEmpty(authenticationIdentity.UserName))
                    {
                        authenticationIdentity.UserId = identity.UserId;
                        authenticationIdentity.UserName = identity.UserName;
                    }
                    return true;
                }

            }
            return false;
        }

        private string FetchFromHeader(HttpActionContext actionContext)
        {
            string requestToken = null;

            var authRequest = actionContext.Request.Headers.Authorization;
            if (authRequest != null)
            {
                requestToken = authRequest.Parameter;
            }

            return requestToken;
        }
    }
}