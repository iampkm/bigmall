using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext;
namespace Guoc.BigMall.Domain.Entity
{
    /// <summary>
    /// 区域
    /// </summary>
   public class Area:Entity<string> 
    {
       public Area()
        {
            this.Level = 1;
        }
        public string Name { get; set; }
        public string FullName { get; set; }
        public int Level { get; set; }

        public string ParentId { get; set; }
    }
}
