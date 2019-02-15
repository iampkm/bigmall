using AutoMapper;

namespace Guoc.BigMall.Infrastructure.AutoMapper
{
    public interface IAutoMapperRegistrar
    {
        void Register(IMapperConfigurationExpression cfg);
    }
}
