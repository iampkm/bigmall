using System.Collections.Generic;

namespace Guoc.BigMall.Application.DTO
{
    public class SaleOrderItemModel
    {
        public SaleOrderItemModel()
        {
            this.GiftItems = new List<SaleOrderGiftItem>();
        }

        public int Id { get; set; }
        public int SaleOrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal AvgCostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal RealPrice { get; set; }
        public int Quantity { get; set; }
        public int SupplierId { get; set; }
        public int ParentProductId { get; set; }
        public string SNCode { get; set; }
        public string SourceSapRow { get; set; }
        public int? SourceSaleOrderRow { get; set; }
        public string SapRow { get; set; }
        public string FJCode { get; set; }
        public int ReturnQuantity
        {
            get { return 1; }

        }
        public decimal Amount
        {
            get
            {
                return RealPrice * Quantity;
            }
        }
        public List<SaleOrderGiftItem> GiftItems { get; set; }
        public string SaleClerkOne { get; set; }
        public string SaleClerkTwo { get; set; }
        public decimal ActualAmount
        {
            get
            {
                return SalePrice * Quantity;
            }
        }

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
        /// 实际出入库数量：批发出入库使用
        /// </summary>
        public int ActualQuantity { get; set; }

        public bool HasSNCode { get; set; }

    }

    public class SaleOrderGiftItem
    {
        public int Id { get; set; }
        public int GiftProductId { get; set; }
        public string GiftProductName { get; set; }
        public int GiftQuantity { get; set; }
        public int GiftReturnQuantity { get; set; }
        public string SourceSapRow { get; set; }
        public int? SourceSaleOrderRow { get; set; }
    }

}
