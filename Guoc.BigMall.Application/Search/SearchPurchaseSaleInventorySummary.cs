using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Search
{
    public class SearchPurchaseSaleInventorySummary
    {
        public string StoreId { get; set; }
        public string ProductCodeOrBarCode { get; set; }
        public string ProductName { get; set; }
        public string Time { get; set; }
        public string CategoryId { get; set; }
        public string BrandId { get; set; }

        public bool checkedQuantity { get; set; }
        public string SupplierId { get; set; }
    }
}
