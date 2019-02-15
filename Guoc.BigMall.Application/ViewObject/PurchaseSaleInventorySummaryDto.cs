using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.ViewObject
{
    public class PurchaseSaleInventorySummaryDto
    {
        [Description("开始日期")]
        public string StartDate { get { return this.StartTime.Value.ToShortDateString(); } }
        [Description("结束日期")]
        public string EndDate { get { return this.EndTime.Value.ToShortDateString(); } }
        [Description("类别")]
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public string BrandCode { get; set; }
        [Description("品牌")]
        public string BrandName { get; set; }
        [Description("商品编码")]
        public string ProductCode { get; set; }
        [Description("商品名称")]
        public string ProductName { get; set; }
        [Description("期初数")]
        public decimal FirstQuantity { get; set; }
        [Description("本期采购数")]
        public decimal PurchaseQuantity { get; set; }
        [Description("本期采购金额")]
        public decimal PurchaseAmount { get; set; }
        [Description("本期销售数")]
        public decimal SaleQuantity { get; set; }
        [Description("本期销售金额")]
        public decimal SaleAmount { get; set; }
        [Description("本期销售金额(券后)")]
        public decimal SaleAmountAfterPreferential { get; set; }
        [Description("本期赠送数")]
        public decimal GiftsQuantity { get; set; }
        [Description("本期赠送金额")]
        public decimal GiftsAmount { get; set; }

        public int StoreId { get; set; }

        public string StoreName { get; set; }




        [Description("期末数")]
        public decimal LastQuantity { get; set; }
        [Description("周转率%")]
        public decimal Rate
        {
            get
            {
                if (FirstQuantity + LastQuantity == 0)
                {
                    return 0;
                }
                else
                {
                    return ((SaleQuantity * 2) / (FirstQuantity + LastQuantity));
                }
            }
        }

        public string SupplierName { get; set; }
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
