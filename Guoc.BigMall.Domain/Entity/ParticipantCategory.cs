using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class ParticipantCategory : BaseEntity
    {
        public RechargeVoucherType Type { get; set; }
        public int CategoryRechargeVoucherId { get; set; }
        public int BrandRechargeVoucherId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }

        public ParticipantCategory()
        {

        }

        public ParticipantCategory(RechargeVoucherType type, int categoryId, string categoryCode, string categoryName)
        {
            this.Type = type;
            this.CategoryId = categoryId;
            this.CategoryCode = categoryCode;
            this.CategoryName = categoryName;
        }
    }
}
