using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Lifetime;
using Guoc.BigMall.Infrastructure.IoC;
namespace Guoc.BigMall.Api.Services
{
    public class AutofacContainer : IContainerManager
    {
        IContainer _container;
        ILifetimeScope _scope;
        public AutofacContainer(IContainer container)
        {
            _container = container;
            //  _scope = _container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            _scope = _container.BeginLifetimeScope();
        }
        public T Resolve<T>() where T : class
        {
            return _scope.Resolve<T>();

        }

        public T Resolve<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                return _scope.Resolve<T>();
            }
            else
            {
                return _scope.ResolveKeyed<T>(key);
            }
        }

        public T[] ResolveAll<T>() where T : class
        {
            return _scope.Resolve<IEnumerable<T>>().ToArray();
        }



    }
}
