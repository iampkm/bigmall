using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Guoc.BigMall.Infrastructure.Queue;
using Dapper;
using Dapper.DBContext;
using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Infrastructure.IoC;
using Guoc.BigMall.Infrastructure.Caching;
using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Events;
using Guoc.BigMall.Admin;
using Guoc.BigMall.Infrastructure.AutoMapper;
using Guoc.BigMall.Infrastructure.File;
using Guoc.BigMall.Domain;
namespace Guoc.BigMall.Admin.Services
{
    public class BootStrap
    {
        public static void InitAutofacContainer()
        {
            var builder = new ContainerBuilder();
            // Get your HttpConfiguration.
            // var config = GlobalConfiguration.Configuration;

            //  ASP.NET MVC Autofac RegisterDependency     
            Assembly webAssembly = Assembly.GetExecutingAssembly();
            builder.RegisterControllers(webAssembly);
            // OPTIONAL: Register the Autofac model binder provider.
            // builder.regist();

            // 注册系统组件代码
            // register admin service
            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            //builder.RegisterType<ContextService>().As<IContextService>().InstancePerLifetimeScope();
            // register database connection            
            builder.RegisterType<DapperDBContext>().As<IDBContext>().WithParameter("connectionStringName", Configer.DBName);
            //  builder.RegisterType<QueryService>().As<IQuery>().WithParameter("connectionStringName", "masterDB");
            builder.RegisterType<NLogWriter>().As<ILogger>();

            //注册各层业务组件
            var typeFinder = new DefaultTypeFinder(new string[] { "Guoc.BigMall.Application", "Guoc.BigMall.Application.Facade", "Guoc.BigMall.Domain" });
            var assemblies = typeFinder.LoadAssemblies();
            builder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("Service")).AsSelf();  // 领域服务
            builder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("Facade")).AsImplementedInterfaces();

            // 注册模拟接口
            //if (Configer.Sap_Mock)
            //{
            //    builder.RegisterType<SAPServiceMock>().As<ISAPService>();
            //}
            //else
            //{
            //    builder.RegisterType<SAPService>().As<ISAPService>();
            //}

            //注册销售单队列处理，单例
            // builder.RegisterType<PosSyncFacade>().As<IQueueHander<string>>();
            builder.RegisterType<SimpleQueue<string>>().As<ISimpleQueue<string>>().SingleInstance();

            // 注册缓存
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().SingleInstance();

            //数据导出
            builder.RegisterType<ExcelService>().As<IExcel>();
            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            // config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            // 初始化应用程序上下文
            Singleton<IContainerManager>.Instance = new AutofacContainer(container);

            //初始化AutoMapper
            AutoMapperConfig.RegisterAll();
        }
    }
}