
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Guoc.BigMall.Infrastructure.Queue;
using Dapper;
using Dapper.DBContext;
using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Infrastructure.IoC;
using Guoc.BigMall.Infrastructure.Caching;
using Guoc.BigMall.Infrastructure;
namespace Guoc.BigMall.Api.Services
{
    public class BootStrap
    {
        public static void InitAutofacContainer(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            // Get your HttpConfiguration.
           // var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);
            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();

            // 注册系统组件代码
            //  ASP.NET MVC Autofac RegisterDependency     
           // Assembly webAssembly = Assembly.GetExecutingAssembly();
           // builder.RegisterControllers(webAssembly);
            // register admin service
           // builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
           // builder.RegisterType<ContextService>().As<IContextService>().InstancePerLifetimeScope();
            // register database connection
            builder.RegisterType<DapperDBContext>().As<IDBContext>().WithParameter("connectionStringName", "masterDB");
           // builder.RegisterType<QueryService>().As<IQuery>().WithParameter("connectionStringName", "masterDB");
            builder.RegisterType<NLogWriter>().As<ILogger>();

            //注册各层业务组件
            var typeFinder = new DefaultTypeFinder(new string[] { "Guoc.BigMall.Application", "Guoc.BigMall.Application.Facade", "Guoc.BigMall.Domain" });
            var assemblies = typeFinder.LoadAssemblies();
            builder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("Service")).AsSelf();  // 领域服务
            builder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("Facade")).AsImplementedInterfaces();

            //// 注册领域服务
            //var domainTypeFinder = new DefaultTypeFinder(new string[] {"CQSS.APP.Domain" });
            //var domainAssemblies = domainTypeFinder.LoadAssemblies();
            //builder.RegisterAssemblyTypes(domainAssemblies).Where(t => t.Name.EndsWith("Service")).AsSelf();
            //注册销售单队列处理，单例
           // builder.RegisterType<PosSyncFacade>().As<IQueueHander<string>>();
            builder.RegisterType<SimpleQueue<string>>().As<ISimpleQueue<string>>().SingleInstance();

            // 注册缓存
           // builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
           // builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().SingleInstance();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            // 初始化应用程序上下文
            Singleton<IContainerManager>.Instance = new AutofacContainer(container);

            // OWIN 注入
            //app.UseAutofacMiddleware(container);
            //app.UseAutofacWebApi(config);
        }
    }
}