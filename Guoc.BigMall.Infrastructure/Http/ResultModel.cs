using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.Http
{
   public class ResultModel<T>
    {
        public int code { get; private set; }
        public bool success { get; private set; }
        public string message { get; private set; }
        public T data { get; private set; }

        private static ResultModel<T> SetResult(int code, bool success, String message,T data)
        {
            ResultModel<T> result = new ResultModel<T>();
            result.code = code;
            result.success = success;
            result.message = message;
            result.data = data;
            return result;
        }

        public static ResultModel<T> Ok() {
            return SetResult(200, true, "处理成功",default(T));
        }

        public static ResultModel<T> Ok(T data)
        {
            return SetResult(200, true, "处理成功",data);
        }

        public static ResultModel<T> Error()
        {
            return Error("500 内部服务错误");
        }
        public static ResultModel<T> Error(string message)
        {
            return SetResult(500, true, message, default(T));
        }

        /// <summary>
        /// 未授权
        /// </summary>
        /// <returns></returns>
        public static ResultModel<T> unauthorized()
        {
            return SetResult(401, true, "该请求未授权", default(T));
        }
    }
}
