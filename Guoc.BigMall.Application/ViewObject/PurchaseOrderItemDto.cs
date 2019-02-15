using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.ViewObject
{
    public class PurchaseOrderItemDto
    {
        public PurchaseOrderItemDto()
        {
            this.SNCodes = new List<string>();
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public List<string> SNCodes { get; set; }


        public string ProductCode { get; set; }

        public string ProductName { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        public decimal CostPrice { get; set; }

        /// <summary>
        /// 预订数量
        /// </summary>
        public int Quantity { get; set; }

        public decimal Amount
        {
            get
            {
                return CostPrice * Quantity;
            }

        }


        public decimal ActualAmount
        {
            get
            {
                return CostPrice * ActualQuantity;
            }
        }

        /// <summary>
        /// 实收数量
        /// </summary>
        public int ActualQuantity { get; set; }


        public int SNQuantity { get; set; }
        public bool IsSnCode { get { return true; } set { value = this.IsSnCode; } }
        public string BgColor
        {
            get
            {
                return Quantity == ActualQuantity ? "bg-success" : "bg-danger";

            }
        }

        public int SupplierId { get; set; }

        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        /// <summary>
        /// 库存数量：退单使用
        /// </summary>
        public int InventoryQuantity { get; set; }

        public long BatchNo { get; set; }
        public bool HasSNCode { get; set; }

        public int MaxQuaitity { get { return Math.Abs( Quantity); } }
        public string AllSNcodes { get; set; }
    }
}
