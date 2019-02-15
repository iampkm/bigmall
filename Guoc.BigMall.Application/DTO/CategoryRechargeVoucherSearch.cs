using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class CategoryRechargeVoucherSearch
    {
        public string StoreIds { get; set; }
        public string CardNumber { get; set; }
        public DateTime[] DateRange { get; set; }
        public int? CategoryId { get; set; }
        public RechargeVoucherStatus? Status { get; set; }
    }
}
