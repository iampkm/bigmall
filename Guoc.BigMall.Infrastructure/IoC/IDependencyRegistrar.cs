using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Guoc.BigMall.Infrastructure.IoC
{
   public interface IDependencyRegistrar
    {
       void Register(IContainerManager builder, ITypeFinder typeFinder);
    }
}
