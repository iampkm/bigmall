using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    /// <summary>
    /// 调拨单状态
    /// 总仓分配：......................待发货->已发货->待收货->已收货->已完成
    /// 门店申请：初始->待审核->已审核->待发货->已发货->待收货->已收货->已完成
    /// 门店调拨：初始->................待发货->已发货->待收货->已收货->已完成
    /// </summary>
    public enum TransferStatus
    {
        /// <summary>
        /// 驳回
        /// </summary>
        [Description("驳回")]
        Reject = -2,

        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        Canceled = -1,

        /// <summary>
        /// 初始
        /// </summary>
        [Description("初始")]
        Initial = 0,

        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        WaitAudit = 1,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        Audited = 2,

        /// <summary>
        /// 待发货
        /// </summary>
        [Description("待发货")]
        WaitShipping = 3,

        /// <summary>
        /// 已发货
        /// </summary>
        [Description("已发货")]
        Shipped = 4,

        /// <summary>
        /// 待收货
        /// </summary>
        [Description("待收货")]
        WaitReceiving = 5,

        /// <summary>
        /// 已收货
        /// </summary>
        [Description("已收货")]
        Received = 6,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Finished = 7,
    }
}
