using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class ProductPurchasePrice
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SapIdentity { get; set; }
        public int Status { get; set; }
        public decimal SapSalePrice { get; set; }
        public int SapPriceUnit { get; set; }
        public string ProductCode { get; set; }
        public string SupplierCode { get; set; }
        public int SupplierId { get; set; }

    }
}
