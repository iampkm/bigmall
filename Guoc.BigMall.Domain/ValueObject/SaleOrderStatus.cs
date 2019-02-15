using System.ComponentModel;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum SaleOrderStatus
    {
        [Description("已退货")]
        Returned = -2,
        [Description("作废")]
        Cancel = -1,
        [Description("初始")]
        Create = 1,
        [Description("待审核")]
        WaitAudit = 2,
        [Description("已审核")]
        Audited = 3,
        [Description("待出库")]
        WaitOutStock = 4,
        [Description("已出库")]
        OutStock = 5,
        [Description("预售转正")]
        Convert = 6
    }

    public enum ReturnSaleOrderStatus
    {
        [Description("作废")]
        Cancel = -1,
        [Description("初始")]
        Create = 1,
        [Description("待审核")]
        WaitAudit = 2,
        [Description("已审核")]
        Audited = 3,
        [Description("待入库")]
        WaitInStock = 4,
        [Description("已入库")]
        InStock = 5
    }
}