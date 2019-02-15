using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
   public class StoreModel
    {
       public StoreModel() {
           CreatedOn = DateTime.Now;
       }
       public int Id { get; set; }
        public string Name { get; set; }
        public int AreaId { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string Tag { get; set; }
    }
}
