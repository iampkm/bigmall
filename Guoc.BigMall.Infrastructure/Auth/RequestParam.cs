using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.Auth
{
    /// <summary>
    ///  开发API 验证结果
    /// </summary>
   public class RequestParam
    {
       public RequestParam() {
           this.Params = new NameValueCollection();
       }
       /// <summary>
       /// 请求方式： Get，Post，Put，Delete
       /// </summary>
       public string Method { get; set; }
       /// <summary>
       ///  时间戳 yyyyMMddHHmmss  （header中传递）
       /// </summary>
       public string TimeStamp { get; set; }
       /// <summary>
       /// app Id  （header中传递）
       /// </summary>
       public string AppId { get; set; }

       /// <summary>
       /// 签名 （header中传递）
       /// </summary>
       public string Sign { get; set; }

       public NameValueCollection Params { get; set; }

       
       /// <summary>
       ///  app 密匙( 从服务端获取)
       /// </summary>
       public string AppSecret { get; set; }
    }
}
