using System.Collections.Generic;
namespace Guoc.BigMall.Domain.Entity
{
    public class SaleOrderItem : BaseEntity
    {
        public int SaleOrderId { get; set; }
        /// <summary>
        /// SKU编码
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// SKU编码
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// SKU编码
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// 平均成本
        /// </summary>
        public decimal AvgCostPrice { get; set; }
        /// <summary>
        /// 销售价
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// 实际折后价
        /// </summary>
        public decimal RealPrice { get; set; }
        /// <summary>
        /// 限价
        /// </summary>
        public decimal MinSalePrice { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 品类卡
        /// </summary>
        public string CategoryCardNumber { get; set; }
        /// <summary>
        /// 品类充值券优惠额
        /// </summary>
        public decimal CategoryPreferential { get; set; }
        /// <summary>
        /// 品牌充值券优惠额
        /// </summary>
        public decimal BrandPreferential { get; set; }

        /// <summary>
        ///  批发销售单，实发，实收数
        /// </summary>
        public int ActualQuantity { get; set; }
        public int SupplierId { get; set; }
        public int? ParentProductId { get; set; }
        public string SNCode { get; set; }
        public int GiftType { get; set; }
        public string FJCode { get; set; }
        /// <summary>
        ///  sap 行号
        /// </summary>
        public string SapRow { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }


        /// <summary>
        ///  SAP 原销售单 行号 （退单时使用）
        /// </summary>
        public string SourceSapRow { get; set; }
        public int? SourceSaleOrderRow { get; set; }
        public decimal GetRowTotal()
        {
            return this.RealPrice * this.Quantity;
        }
        public string SaleClerkOne { get; set; }
        public string SaleClerkTwo { get; set; }

        public List<string> GetSNCodeList()
        {
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(this.SNCode))
            {
                return list;
            }

            list.AddRange(this.SNCode.Split(','));
            return list;
        }
    }
}