﻿using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Events;
namespace Guoc.BigMall.Infrastructure.Caching.Events
{
    public static class EventPublisherExtensions
    {
        public static void EntityInserted<T>(this IEventPublisher eventPublisher, T entity) where T : class
        {
            eventPublisher.Publish(new EntityInserted<T>(entity));
        }

        public static void EntityUpdated<T>(this IEventPublisher eventPublisher, T entity) where T : class
        {
            eventPublisher.Publish(new EntityUpdated<T>(entity));
        }

        public static void EntityDeleted<T>(this IEventPublisher eventPublisher, T entity) where T : class
        {
            eventPublisher.Publish(new EntityDeleted<T>(entity));
        }
    }
}