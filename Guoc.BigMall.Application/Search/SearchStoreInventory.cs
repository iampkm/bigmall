using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Search
{
    public class SearchStoreInventory
    {
        public string SupplierId { get; set; }

        public string StoreId { get; set; }

        public string ProductCodeOrBarCode { get; set; }

        public string ProductName { get; set; }
        public string CodeOrBarCode { get; set; }
        public string CategoryId { get; set; }


        public string BrandId { get; set; }
        public string SalePriceStart { get; set; }
        public string SalePriceEnd { get; set; }
        public string ContractPriceStart { get; set; }
        public string ContractPriceEnd { get; set; }
        public string Order { get; set; }
        public string Mark { get; set; }
        public string StoreInventoryQuantity { get; set; }
        public bool ToExcel { get; set; }
        public string ProductCodeIds { get; set; }
    }
}
