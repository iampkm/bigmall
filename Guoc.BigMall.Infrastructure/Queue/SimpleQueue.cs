using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Guoc.BigMall.Infrastructure.Log;
namespace Guoc.BigMall.Infrastructure.Queue
{
    /// <summary>
    /// 确保此类在单例中运行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleQueue<T>:ISimpleQueue<T>
    {
        private static BlockingCollection<T> _requestOrderQueue;
        IQueueHander<T> _queueHander;
        ILogger _log;
        public SimpleQueue(ILogger log, IQueueHander<T> hander)
        {
            _requestOrderQueue = new BlockingCollection<T>();
            _log = log;
            _queueHander = hander;

            //启动队列处理配置
            //为订单开启队列
            _log.Info("启动队列");
            System.Threading.Tasks.Task.Factory.StartNew(() =>
              {
                  var list = _requestOrderQueue.GetConsumingEnumerable();
                  foreach (var item in list)
                  {
                      // 处理订单
                      try
                      {
                          _queueHander.Hander(item);
                      }
                      catch (Exception ex)
                      {
                          _log.Error(ex);
                      }
                  }
              });

        }

        public bool Add(T t)
        {
            return _requestOrderQueue.TryAdd(t);           
        }

       
    }
}
