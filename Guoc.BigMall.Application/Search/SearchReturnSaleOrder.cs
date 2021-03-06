﻿using System;

namespace Guoc.BigMall.Application.Search
{
    public class SearchReturnSaleOrder
    {
        public int? SaleOrderId { get; set; }
        public string StoreId { get; set; }
        public DateTime? CreateOnFrom { get; set; }
        public DateTime? CreateOnTo { get; set; }
        public string CategoryCode { get; set; }
        public string BrandCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string StoreCode { get; set; }
        public string CreateBy { get; set; }
        public int? Status { get; set; }
        public string OrderCode { get; set; }  //这个代表的是退货单 而不是销售单
        public string SourceSaleOrderCode { get; set; }
        public string SNCode { get; set; }
        public int? OrderType { get; set; }
    }
}