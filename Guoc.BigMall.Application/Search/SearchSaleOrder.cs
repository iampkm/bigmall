using System;

using Guoc.BigMall.Domain.ValueObject;

namespace Guoc.BigMall.Application.Search
{
    public class SearchSaleOrder
    {
        public string Ids { get; set; }
        public int? SaleOrderId { get; set; }
        public string StoreId { get; set; }
        public DateTime? CreateOnFrom { get; set; }
        public DateTime? CreateOnTo { get; set; }
        public DateTime? SaleTimeFrom { get; set; }
        public DateTime? SaleTimeTo { get; set; }
        public string CategoryCode { get; set; }
        public int? CategoryId { get; set; }
        public string BrandCode { get; set; }
        public string BrandIds { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string StoreCode { get; set; }
        public string CreateBy { get; set; }
        public int? Status { get; set; }
        //public int? RoStatus { get; set; }
        public string RoStatus { get; set; }
        public string OrderCode { get; set; }
        public string SourceSaleOrderCode { get; set; }
        public string FJCode { get; set; }
        public string SNCode { get; set; }
        public int? OrderType { get; set; }
        public int? BillType { get; set; }
        public string ProductIds { get; set; }
        public string SAPCode { get; set; }
        public bool? IsPushSap { get; set; }
    }
}