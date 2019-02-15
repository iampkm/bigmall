using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum StocktakingPlanStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Canceled = -1,
        /// <summary>
        /// 待盘点
        /// </summary>
        [Description("待盘点")]
        ToBeInventory = 1,
        /// <summary>
        /// 初盘
        /// </summary>
        [Description("初盘")]
        FirstInventory = 2,
        /// <summary>
        /// 复盘
        /// </summary>
        [Description("复盘")]
        Replay = 3,
        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Complete = 4
    }
}
