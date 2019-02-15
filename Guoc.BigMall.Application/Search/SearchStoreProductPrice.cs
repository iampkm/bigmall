using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Search
{
   public class SearchStoreProductPrice
    {
        public string StoreId { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string CategoryId { get; set; }

        public string CategoryCode { get; set; }
        public string BrandId { get; set; }

        public string Mark { get; set; }
        public string Quantity { get; set; }
    }
}
