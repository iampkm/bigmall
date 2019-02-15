using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.AutoMapper
{
    public static class AutoMapperConfig
    {
        public static void RegisterAll()
        {
            Mapper.Initialize(AutoMapperConfig.RegisterAllMapperConfigurationExpression);
        }

        private static void RegisterAllMapperConfigurationExpression(IMapperConfigurationExpression cfg)
        {
            var interfaceType = typeof(IAutoMapperRegistrar);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("Guoc")).ToList();
            var mapperTypes = assemblies.SelectMany(a => a.GetTypes().Where(t => !t.IsGenericType && t.IsClass && interfaceType.IsAssignableFrom(t))).ToList();
            var mappers = mapperTypes.Select(t => Activator.CreateInstance(t) as IAutoMapperRegistrar);

            foreach (var mapper in mappers)
                mapper.Register(cfg);
        }
    }
}
