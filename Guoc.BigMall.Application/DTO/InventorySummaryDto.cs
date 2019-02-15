using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class InventorySummaryDto
    {
        public int StoreId { get; set; }
        [Description("门店编码")]
        public string StoreCode { get; set; }
        [Description("门店名称")]
        public string StoreName { get; set; }
        public int ProductId { get; set; }
        [Description("商品编码")]
        public string ProductCode { get; set; }
        [Description("商品名称")]
        public string ProductName { get; set; }
        public int BrandId { get; set; }
        [Description("品牌编码")]
        public string BrandCode { get; set; }
        [Description("品牌名称")]
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        [Description("品类编码")]
        public string CategoryCode { get; set; }
        [Description("品类名称")]
        public string CategoryName { get; set; }
        public DateTime? CreatedOn { get; set; }
        [Description("日期")]
        public string CreatedOnString
        {
            get
            {
                return this.CreatedOn.HasValue ? this.CreatedOn.Value.ToString("yyyy-MM-dd") : null;
            }
        }
        [Description("数量")]
        public int Quantity { get; set; }
        [Description("成本总金额")]
        public decimal CostAmount { get; set; }
    }
}
