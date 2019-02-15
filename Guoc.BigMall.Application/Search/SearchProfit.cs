using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Search
{
    public class SearchProfit
    {
        public string StoreId { get; set; }
        public string ProductCodeOrBarCode { get; set; }

        public string SupplierId { get; set; }
        public string Time { get; set; }
        public string CategoryId { get; set; }
        public string BrandId { get; set; }

        public string BillType { get; set; }


        public string Buyer { get; set; }
        public bool ToExcel { get; set; }
         
    }
}
