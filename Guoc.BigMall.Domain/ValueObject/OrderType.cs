using System.ComponentModel;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum OrderType
    {
        [Description("订单")]
        /// <summary>
        /// 订单
        /// </summary>
        Order = 1,
        [Description("退单")]
        /// <summary>
        /// 退单
        /// </summary>
        Return = 2,
    }

    public enum OrderProductType
    {
        [Description("商品")]
        Product = 1,
        [Description("礼品")]
        Gift =  2
    }
}
