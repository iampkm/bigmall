using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.ViewObject
{
    public class SaleOrderSummaryDto
    {
        [Description("单号")]
        public string SaleOrderCode { get; set; }
        [Description("日期")]
        public string PaidDateStr { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int ProductId { get; set; }
        [Description("商品编码")]
        public string ProductCode { get; set; }
        [Description("商品名称")]
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        [Description("类别")]
        public string CategoryName { get; set; }
        [Description("品牌")]
        public string BrandName { get; set; }
        [Description("门店")]
        public string StoreName { get; set; }
      
        [Description("客户")]
        public string Buyer { get; set; }
        public int StoreId { get; set; }
        public int CreatedBy { get; set; }
        [Description("业务员")]
        public string BusinessUser { get; set; }
        [Description("数量")]
        public int Quantity { get; set; }
        [Description("单价")]
        public decimal RealPrice { get; set; }
        [Description("金额")]
        public decimal Amount { get; set; }

      
      
    }
}
