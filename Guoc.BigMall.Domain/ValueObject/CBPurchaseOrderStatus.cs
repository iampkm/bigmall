using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum CBPurchaseOrderStatus
    {
        [Description("作废")]
        Cancel = -1,

        [Description("初始")]
        Create = 1,

        [Description("驳回")]
        Reject = 2,

        [Description("已审")]
        Audited = 3,

        [Description("已出库")]
        OutStock = 4,

        [Description("已入库")]
        InStock = 5,

        [Description("已关闭")]
        Closed = 9, 
        [Description("已完成")]
        Finished = 10,

      
       
    }
}
