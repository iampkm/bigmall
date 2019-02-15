using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.ViewObject
{
   public class TransferOrderPrintDto
    {
       public string Code { get; set; }

       public string FromStoreCode { get; set; }
       public string FromStoreName { get; set; }

       public string FromStore {
           get { return string.Format("[{0}]{1}", this.FromStoreCode, this.FromStoreName); }
       }
       public string ToStoreCode { get; set; }
       public string ToStoreName { get; set; }

       public string ToStore
       {
           get { return string.Format("[{0}]{1}", this.ToStoreCode, this.ToStoreName); }
       }
       public string Remark { get; set; }
       public DateTime CreatedOn { get; set; }
       public string CreatedByName { get; set; }

       public DateTime AuditOn { get; set; }
       public string AuditByName { get; set; }


       public string ProductCode { get; set; }

       public string ProductName { get; set; }

       public int Quantity { get; set; }
       public int ActualShipmentQuantity { get; set; }
       public int ActualReceivedQuantity { get; set; }

    }
}
