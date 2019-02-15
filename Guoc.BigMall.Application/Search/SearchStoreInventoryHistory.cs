using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Search
{
    public class SearchStoreInventoryHistory
    {

        public string BillCode { get; set; }

        public int BillType { get; set; }

        public string StoreId { get; set; }

        public string ProductCodeOrBarCode { get; set; }

        public string BatchNo { get; set; }

        public string ProductName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }


        public string Time { get; set; }
        public string SNCode { get; set; }
    }
}
