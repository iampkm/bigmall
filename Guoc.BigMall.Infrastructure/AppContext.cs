using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.IoC;
namespace Guoc.BigMall.Infrastructure
{
   public class AppContext
    {      
       public static IContainerManager Current
       {
           get
           {              
               return Singleton<IContainerManager>.Instance;
           }
       }
    }
}
