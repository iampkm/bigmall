using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum ProductPriceType
    {

        [Description("售价")]
        SalePrice = 1,
        [Description("限价")]
        LimitedPrice = 2,
    }
}
