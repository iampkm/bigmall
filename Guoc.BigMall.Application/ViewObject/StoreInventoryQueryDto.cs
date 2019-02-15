using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.ViewObject
{
    public class StoreInventoryQueryDto
    {
        /// <summary>
        /// 商品SKUID
        /// </summary>
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        [Description("门店")]
        public string StoreName { get; set; }
        [Description("类别")]
        public string CategoryName { get; set; }
        [Description("品牌")]
        public string BrandName { get; set; }
         [Description("商品编码")]
        public string ProductCode { get; set; }
         [Description("商品名称")]

        public string ProductName { get; set; }
        [Description("供应商")]
         public string SupplierName { get; set; }
        [Description("库存数")]
        public int Quantity { get; set; }
        /// <summary>
        /// 锁定数量
        /// </summary>
        [Description("锁定数量")]
        public int OccupyQuantity { get; set; }
        /// <summary>
        /// 可用数量
        /// </summary>
           [Description("可用数量")]
        public int UsableQuantity { get; set; }

        /// <summary>
        /// 可售数量
        /// </summary>
        public int SaleQuantity { get; set; }
        /// <summary>
        /// 预订数量
        /// </summary>
        public int OrderQuantity { get; set; }
        
        
        public decimal AvgCostPrice { get; set; }

        public decimal Amount
        {
            get
            {
                return AvgCostPrice * Quantity;
            }
        }
        [Description("售价金额")]
        public decimal SalePrice { get; set; }
        public decimal SaleAmount
        {
            get
            {
                return SalePrice * Quantity;
            }
        }

        /// <summary>
        /// 预警数量
        /// </summary>
        public int WarnQuantity { get; set; }
        /// <summary>
        /// 是否退市（例如不再进货销售）
        /// </summary>
        public bool IsQuit { get; set; }

        public string BarCode { get; set; }

      
        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; } 

        /// <summary>
        /// 毛利额
        /// </summary>
        public string ProfitAmount
        {
            get
            {
                return (SaleAmount - this.Amount).ToString("f2");
            }
        }

        /// <summary>
        /// 毛利率
        /// </summary>
        public string ProfitPercent
        {
            get
            {
                if (SaleAmount == 0) { return "0%"; }
                var result = decimal.Round((SaleAmount - this.Amount) / SaleAmount * 100, 2);
                return result.ToString("f2") + "%";
            }
        }
        [Description("串码")]
        public string SNCode { get; set; }
    }
}
