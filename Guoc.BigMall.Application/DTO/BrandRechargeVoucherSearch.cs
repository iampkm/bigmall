using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class BrandRechargeVoucherSearch
    {
        public string StoreIds { get; set; }
        public DateTime[] DateRange { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public RechargeVoucherStatus? Status { get; set; }
    }
}
