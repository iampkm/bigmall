using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class ExceptProduct : BaseEntity
    {
        public RechargeVoucherType Type { get; set; }
        public int CategoryRechargeVoucherId { get; set; }
        public int BrandRechargeVoucherId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public ExceptProduct()
        {

        }

        public ExceptProduct(RechargeVoucherType type, int productId, string productCode, string productName)
        {
            this.Type = type;
            this.ProductId = productId;
            this.ProductCode = productCode;
            this.ProductName = productName;
        }
    }
}
