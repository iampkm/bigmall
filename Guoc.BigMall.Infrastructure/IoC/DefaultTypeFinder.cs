using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Log;
namespace Guoc.BigMall.Infrastructure.IoC
{
   public class DefaultTypeFinder:ITypeFinder
    {
       List<string> _assemblies;
       public DefaultTypeFinder(string[] BusinessAssemblies)
       {
           _assemblies = new List<string>();
           _assemblies.AddRange(BusinessAssemblies);
       }
       public Assembly[] LoadAssemblies()
       {
           IList<Assembly> assemblies = new List<Assembly>();
           foreach (var assmeblyName in _assemblies)
           {
               try
               {
                   assemblies.Add(Assembly.Load(assmeblyName));
               }
               catch (Exception ex)
               {
                   Debug.WriteLine("msg:{0},StackTrace:{1}", ex.Message, ex.StackTrace);
                   throw ex;
               }
           }
           return assemblies.ToArray();
       }

       public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
       {
           return FindClassesOfType(typeof(T), onlyConcreteClasses);
       }


       public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
       {
           return FindClassesOfType(assignTypeFrom, LoadAssemblies(), onlyConcreteClasses);
       }

       public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
       {
           var result = new List<Type>();
           try
           {
               foreach (var assembly in assemblies)
               {
                   Type[] types = null;
                   types = assembly.GetTypes();
                   if (types != null)
                   {
                       foreach (var t in types)
                       {
                           if (assignTypeFrom.IsAssignableFrom(t) || (assignTypeFrom.IsGenericTypeDefinition && DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                           {
                               if (!t.IsInterface)
                               {
                                   if (onlyConcreteClasses)
                                   {
                                       if (t.IsClass && !t.IsAbstract)
                                       {
                                           result.Add(t);
                                       }
                                   }
                                   else
                                   {
                                       result.Add(t);
                                   }
                               }
                           }
                       }
                   }
               }
           }
           catch (ReflectionTypeLoadException ex)
           {
               var msg = string.Empty;
               foreach (var e in ex.LoaderExceptions)
                   msg += e.Message + Environment.NewLine;

               var fail = new Exception(msg, ex);
               Debug.WriteLine(fail.Message, fail);

               throw fail;
           }
           return result;
       }

       public IEnumerable<Type> FindClassesOfType<T>(Assembly[] assemblies, bool onlyConcreteClasses = true)
       {
           return FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
       }

       protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
       {
           try
           {
               var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
               foreach (var implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
               {
                   if (!implementedInterface.IsGenericType)
                       continue;

                   var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                   return isMatch;
               }
               return false;
           }
           catch
           {
               return false;
           }
       }
    }
}
