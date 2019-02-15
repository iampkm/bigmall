using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Guoc.BigMall.Infrastructure.Extension
{
    public static class AutoMappingExtension
    {
        public static IMappingExpression<TSource, TTarget> Ignore<TSource, TTarget>(this IMappingExpression<TSource, TTarget> mapping, Expression<Func<TTarget, object>> selector)
        {
            return mapping.ForMember(selector, op => op.Ignore());
        }

        public static IMappingExpression<TSource, TTarget> Mapping<TSource, TTarget>(this IMappingExpression<TSource, TTarget> mapping, Expression<Func<TSource, object>> selector2, Expression<Func<TTarget, object>> selector)
        {
            return mapping.ForMember(selector, op => op.MapFrom(selector2));
        }

        public static TTarget MapTo<TTarget>(this object source)
        {
            return (TTarget)Mapper.Map(source, source.GetType(), typeof(TTarget));
        }

        public static void MapTo<TTarget>(this object source, TTarget target)
        {
            Mapper.Map(source, target);
        }

        public static List<TTargetElement> MapToList<TTargetElement>(this IEnumerable source)
        {
            return (List<TTargetElement>)Mapper.Map(source, source.GetType(), typeof(List<TTargetElement>));
        }

        public static void MapFrom<TSource>(this object target, TSource source)
        {
            Mapper.Map(source, target);
        }
    }
}
