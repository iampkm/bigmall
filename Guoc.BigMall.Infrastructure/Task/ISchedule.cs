using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guoc.BigMall.Infrastructure.Task
{
   public interface ISchedule
    {
       IList<WorkTask> GetTasks();
    }
}
