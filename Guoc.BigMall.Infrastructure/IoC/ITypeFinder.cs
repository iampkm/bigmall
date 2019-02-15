using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace Guoc.BigMall.Infrastructure.IoC
{
   public interface ITypeFinder
    {
       Assembly[] LoadAssemblies();

       IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);
       IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);
       IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);
       IEnumerable<Type> FindClassesOfType<T>(Assembly[] assemblies, bool onlyConcreteClasses = true);
    }
}
