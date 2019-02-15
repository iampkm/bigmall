using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Search
{
    public class SearchProduct
    {

        public string Name { get; set; }

        public string Code { get; set; }

        public string StoreId { get; set; }

        public string StoreIdGift { get; set; }
        public string SupplierId { get; set; }

        public string BrandIds { get; set; }
        public string CategoryId { get; set; }
        public string Ids { get; set; }
        public string SNCodeLike { get; set; }
        public string SNCodes { get; set; }
        public bool HasSalePrice { get; set; }
        public bool ContainsNoStock { get; set; }
    }
}
