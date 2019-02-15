using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.ViewObject
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Spec { get; set; }
        public string Unit { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string SecondSpec { get; set; }
        public string CategoryCode { get; set; }
        public string BrandCode { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string StoreName { get; set; }
        public string SupplierName { get; set; }
        public string StoreCode { get; set; }
        public string SupplierCode { get; set; }
        public decimal CostPrice { get; set; }
        public int InventoryQuantity { get; set; }
        public string SNCode { get; set; }

        public decimal SalePrice { get; set; }

        /// <summary>
        /// 批次成本价
        /// </summary>
        public decimal BatchCostPrice { get; set; }
        public bool HasSNCode { get; set; }
        public string IsSNCode 
        {
            get
            {
                return HasSNCode == true ? "是" : "否";
            } 
        }

        public int Quantity { get; set; }
   
    }
}
