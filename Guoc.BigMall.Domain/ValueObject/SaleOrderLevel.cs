using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum SaleOrderLevel
    {
        /// <summary>
        /// 普通订单
        /// </summary>
        [Description("普通订单")]
        General = 1,
        /// <summary>
        /// 订单
        /// </summary>
        [Description("会员订单")]
        Vip = 2
    }
}
