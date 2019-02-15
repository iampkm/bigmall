using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
   public enum SaleOrderBillType
    {
        [Description("零售单")]
        SaleOrder =1,
        [Description("批发单")]
        BatchOrder = 2,
        [Description("预售单")]
        PreSaleOrder = 3,
        [Description("换机单")]
        ExchangeOrder = 4
    }
}
