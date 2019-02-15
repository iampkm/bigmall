using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.ViewObject
{
    public class SNCodeInventoryDto
    {
        [Description("门店")]
        public string StoreName { get; set; }
        [Description("供应商")]
        public string SupplierName { get; set; }
       
        [Description("商品编码")]
        public string ProductCode { get; set; }
        [Description("商品名称")]
        public string ProductName { get; set; }
        [Description("品类")]
        public string CategoryName { get; set; }
        [Description("品牌")]
        public string BrandName { get; set; }
        [Description("串码")]
        public string SNCode { get; set; }
         [Description("库存")]
        public int Quantity { get; set; }
    }
}
