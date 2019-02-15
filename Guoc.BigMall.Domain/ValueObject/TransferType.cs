using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum TransferType
    {
        /// <summary>
        /// 总仓分配
        /// </summary>
        [Description("总仓分配")]
        Allocate = 1,

        /// <summary>
        /// 向总仓申请
        /// </summary>
        [Description("向总仓申请")]
        StoreApply = 2,

        /// <summary>
        /// 门店间调拨
        /// </summary>
        [Description("门店间调拨")]
        StoreTransfer = 3,
    }
}
