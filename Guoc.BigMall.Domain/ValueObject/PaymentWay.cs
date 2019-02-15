using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum PaymentWay
    {
        [Description("线下支付")]
        Cash = 1,
        [Description("支付宝")]
        AliPay = 2,
        [Description("微信支付")]
        WechatPay = 3,
        [Description("银联转账")]
        UnionPay = 4,
    }
}
