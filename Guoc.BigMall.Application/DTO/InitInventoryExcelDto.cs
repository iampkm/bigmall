using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
   public class InitInventoryExcelDto
    {
       public string StoreCode { get; set; }
       public string ProductCode { get; set; }
       public int Quantity { get; set; }
       public decimal CostPrice { get; set; }
       public string SNCode { get; set; }
       public string SupplierCode { get; set; }
    }
}
