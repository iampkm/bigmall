using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure
{
    /// <summary>
    /// 系统友好异常：通常用来返回给前端显示
    /// </summary>
   public class FriendlyException:Exception
    {
       public FriendlyException(string message) : base(message) { }

       public FriendlyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
