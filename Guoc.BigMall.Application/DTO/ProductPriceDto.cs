using System;

namespace Guoc.BigMall.Application.DTO
{
    public class ProductPriceDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public decimal SalePrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SapIdentity { get; set; }
        public int PriceType { get; set; }
        public int Status { get; set; }
        public string SapPriceType { get; set; }
        public decimal SapSalePrice { get; set; }
        public int SapPriceUnit { get; set; }
        public string ProductCode { get; set; }
    }
}