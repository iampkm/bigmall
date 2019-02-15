using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class StocktakingPlanItem
    {
        public int Id { get; set; }
        public int StocktakingPlanId { get; set; }
        public int ProductId { get; set; }
        public bool IsSNProduct { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public int FirstQuantity { get; set; }
        public int ComplexQuantity { get; set; }
        public int CheckQuantity { get; set; }
        //public string SNCode { get; set; }
        /// <summary>
        /// 盘盈串码
        /// </summary>
        public string SurplusSNCode { get; set; }
        /// <summary>
        /// 盘亏串码
        /// </summary>
        public string MissingSNCode { get; set; }


        public string ProductCode { get; set; }
        public string Unit { get; set; }

        /// <summary>
        ///  盘点数 - 库存数
        /// </summary>
        /// <returns></returns>
        public int GetDifferenceQuantity()
        {
            return this.ComplexQuantity - this.Quantity;
        }

        ///// <summary>
        ///// 是否串码商品：SNCode 不为空就是串码商品
        ///// </summary>
        ///// <returns></returns>
        //public bool IsSNCodeProduct()
        //{
        //    return !string.IsNullOrEmpty(SNCode);
        //}

    }
}
