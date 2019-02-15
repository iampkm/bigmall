using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Search
{
    public class SearchInventorySummary
    {
        public string StoreIds { get; set; }
        public string ProductIds { get; set; }
        public string BrandIds { get; set; }
        public string CategoryIds { get; set; }
        public string CategoryCode { get; set; }
        public string Time { get; set; }
        public int SummaryMethod { get; set; }
    }
}
