using System.Collections.Generic;
using System.Collections;
using Guoc.BigMall.Infrastructure.IoC;
namespace Guoc.BigMall.Infrastructure.Events
{
    /// <summary>
    /// Event subscription service
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        IContainerManager _container;
        public SubscriptionService(IContainerManager container)
        {
            _container = container;
        }
        /// <summary>
        /// Get subscriptions
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        public IList<IConsumer<T>> GetSubscriptions<T>()
        {
            return this._container.ResolveAll<IConsumer<T>>();
        }
    }
}
