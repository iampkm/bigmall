using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
using System.ComponentModel;

namespace Guoc.BigMall.Application.ViewObject
{
    public class StocktakingPlanProductDto
    {
        [Description("商品编码")]
        public string ProductCode { get; set; }
        [Description("商品名称")]
        public string ProductName { get; set; }      

        [Description("品类")]
        public string CategoryName { get; set; }
        [Description("品类代码")]
        public string CategoryCode { get; set; }
        [Description("品牌名")]
        public string BrandName { get; set; }
        [Description("品牌代码")]
        public string BrandCode { get; set; }

        [Description("门店编码")]
        public string StoreCode { get; set; }
        [Description("门店名称")]
        public string StoreName { get; set; }
        [Description("串码商品")]
        public string IsSNProduct { get; set; }
        [Description("当前库存")]
        public int Quantity { get; set; }
        [Description("盘点明细ID")]
        public int PlanItemId { get; set; }
        [Description("盘点数")]
        public string InventoryQuantity { get; set; }

        [Description("盘盈串码")]
        public string SurplusSNCode { get; set; }

        [Description("盘亏串码")]
        public string MissingSNCode { get; set; }
    }

    public class PlanProductDto
    {
        public int PlanItemId { get; set; }
        public int Quantity { get; set; }
        //public string SNCode { get; set; }
        public string SurplusSNCode { get; set; }
        public string MissingSNCode { get; set; }
    }
}
