using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.IoC
{
   public interface IContainerManager
    {
       /// <summary>
       /// 获取容器中的实例
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <returns></returns>
       T Resolve<T>() where T : class;
       T Resolve<T>(string key) where T : class;

       T[] ResolveAll<T>() where T : class;  
    }
}
