
namespace Guoc.BigMall.Application.DTO
{
    public class SaleOrderItemDto
    {
        public int Id { get; set; }
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
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        public int SupplierId { get; set; }
        public int? ParentProductId { get; set; }
        public string SNCode { get; set; }
        public int GiftType { get; set; }
        public string FJCode { get; set; }
        public string SaleClerkOne { get; set; }
        public string SaleClerkTwo { get; set; }
        public string SapRow { get; set; }
        public string SourceSapRow { get; set; }
        /// <summary>
        /// 商玛特原单行号
        /// </summary>
        public int SourceSaleOrderRow { get; set; }
        public string Unit { get; set; }
        public string CategoryCardNumber { get; set; }
        public decimal CategoryPreferential { get; set; }
        public decimal BrandPreferential { get; set; }
        public bool HasSNCode { get; set; }
    }
}