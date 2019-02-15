using System;

namespace Guoc.BigMall.Domain.Entity
{
    public class ProductPrice
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public decimal SalePrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SapIdentity { get; set; }
        /// <summary>
        /// 价格类型：ProductPriceType ： 1 建议零售价，2 特价（限价）
        /// </summary>
        public int PriceType { get; set; }
        public int Status { get; set; }
        public string SapPriceType { get; set; }
        public decimal SapSalePrice { get; set; }
        public int SapPriceUnit { get; set; }
        public string ProductCode { get; set; }
    }
}