using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guoc.BigMall.Admin
{
    public static class IEnumerableExtension
    {
        public static string ToJson<T>(this IEnumerable<T> source)
        {
            if (source == null) return null;
            return JsonConvert.SerializeObject(source);
        }
    }
}