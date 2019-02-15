using Guoc.BigMall.Admin.Services;
using System.Web;
using System.Web.Mvc;

namespace Guoc.BigMall.Admin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
            filters.Add(new JsonRequestBehaviorAttribute());
            filters.Add(new ModelValidationFilterAttribute());
        }
    }
}
