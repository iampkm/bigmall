using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    /// <summary>
    /// 门店客户对应关系
    /// </summary>
   public class StoreCustomerMap:BaseEntity
    {
       public string CustomerCode { get; set; }

       public string StoreCode { get; set; }

       public string Description { get; set; }
    }
}
