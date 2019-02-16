using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
   public class Site:BaseEntity
    {
         public string Name { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        public string Layout { get; set; }
        public string DispalyOrder { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
