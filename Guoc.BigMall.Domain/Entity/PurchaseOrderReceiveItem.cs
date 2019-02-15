using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class PurchaseOrderReceiveItem : BaseEntity
    {
       public int PurchaseOrderReceiveId { get; set; }
       public int PurchaseOrderItemId { get; set; }
       public int PurchaseQuality { get; set; }
       public int Quality { get; set; }
       public decimal Price { get; set; }
       public int ProductId { get; set; }
       public string SNCodes { get; set; }


      
    }
}
