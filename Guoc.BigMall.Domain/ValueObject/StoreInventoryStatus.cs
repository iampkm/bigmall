using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum StoreInventoryStatus
    {
        [Description("淘汰")]
        Canceled = -1,
        [Description("正常")]
        Normal = 1,
        [Description("新品")]
        New = 2,
        [Description("计划淘汰")]
        WouldCancel = 3
    }
}
