using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.ViewObject
{
   public class StoreProductPriceDto
    {
        [Description("门店")]
        public string StoreName { get; set; }
       [Description("门店编码")]
        public string StoreCode { get; set; }
         [Description("商品编码")]
        public string ProductCode { get; set; }
         [Description("品名")]
        public string ProductName { get; set; }
         [Description("品类")]
        public string CategoryName { get; set; }
         [Description("品牌")]
        public string BrandName { get; set; }
         [Description("规格")]
        public string Specification { get; set; }
         [Description("建议零售价")]
       /// <summary>
       ///  售价
       /// </summary>
        public decimal SalePrice { get; set; }
         [Description("最低限价")]

       /// <summary>
       /// 最低限价
       /// </summary>
        public decimal MinSalePrice { get; set; }
         [Description("库存")]
        public int Quantity { get; set; }
    }
}
