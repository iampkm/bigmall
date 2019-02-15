using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum RechargeVoucherType
    {
        /// <summary>
        /// 品类充值券
        /// </summary>
        CategoryRechargeVoucher = 1,

        /// <summary>
        /// 品牌充值券
        /// </summary>
        BrandRechargeVoucher = 2
    }
}
