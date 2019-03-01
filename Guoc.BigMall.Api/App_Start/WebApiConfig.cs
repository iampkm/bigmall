using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Guoc.BigMall.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {           

            // Web API 配置和服务
            config.Filters.Add(new Services.ModelValidationFilterAttribute());
            config.Filters.Add(new Services.CustomHandleErrorAttribute());
            // Web API 路由
            config.MapHttpAttributeRoutes();
            config.Formatters.Remove(config.Formatters.XmlFormatter); // 移除XML 格式
           
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // 添加json返回
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
