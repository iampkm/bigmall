using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Guoc.BigMall.Infrastructure.IoC;
using Guoc.BigMall.Infrastructure.Log;
namespace Guoc.BigMall.Infrastructure.Task
{
    public class ScheduleContext
    {
     
       static Timer _timer;
       static ISchedule _schedule;
        /// <summary>
        /// 任务配置文件Task.config根路径
        /// </summary>
       public static string TaskConfigPath;
       public static IContainerManager Container;
       public static void Start(){
           if (string.IsNullOrEmpty(TaskConfigPath)) { throw new Exception("自动任务配置文件根路径未配置TaskConfigPath"); }
           if (Container== null) { throw new Exception("请设置容器"); }
           _schedule = new DefaultSchedule(ScheduleContext.TaskConfigPath);
           ILogger log = Container.Resolve<ILogger>();
           log.Info("自动任务已启动");
           _timer = new Timer(1000);
           _timer.Elapsed += timer_Elapsed;
           _timer.Enabled = true;
       }

       static void timer_Elapsed(object sender, ElapsedEventArgs e)
       {
           foreach (var item in _schedule.GetTasks())
           {            
               if (item.TaskTrigger.Trigger(e.SignalTime))
               {
                   System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(item.Task.Execute));
                   thread.Start();
               }
           }           
       }

       public static void Close()
       {
           _timer.Enabled = false;
           _timer = null;
       }
    }
}
