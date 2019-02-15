using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum RechargeVoucherStatus
    {
        /// <summary>
        /// 待审
        /// </summary>
        [Description("待审")]
        WaitAudit = 0,

        /// <summary>
        /// 可用
        /// </summary>
        [Description("可用")]
        Normal = 1,

        /// <summary>
        /// 驳回
        /// </summary>
        [Description("驳回")]
        Rejected = 2,

        /// <summary>
        /// 中止
        /// </summary>
        [Description("中止")]
        Aborted = 3,
    }
}
