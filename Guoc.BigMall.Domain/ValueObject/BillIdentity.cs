using System.ComponentModel;

namespace Guoc.BigMall.Domain.ValueObject
{
    /// <summary>
    /// 单据类型
    /// </summary>
    public enum BillIdentity
    {
        [Description("销售订单")]
        SaleOrder = 11,
        [Description("销售退单")]
        SaleRefund = 12,
        [Description("批发订单")]
        BatchOrder = 13,
        [Description("批发退单")]
        BatchRefund = 14,
        [Description("预售订单")]
        PreSaleOrder = 15,
        [Description("预售退单")]
        PreSaleRefund = 16,
        [Description("换机单")]
        ExchangeOrder = 17,
        [Description("销售明细唯一码")]
        SaleOrderItemCode = 18,//用于关联赠品和销售明细

        [Description("总仓采购订单")]
        StockPurchaseOrder = 20,
        [Description("门店采购订单")]
        StorePurchaseOrder = 21,
        [Description("仓库采购退单")]
        StockPurchaseRefundOrder = 23,
        [Description("门店采购退单")]
        StorePurchaseRefundOrder = 24,
        [Description("采购换单")]
        StorePurchaseChange = 25,

        [Description("门店盘点")]
        StoreStocktakingPlan = 53,
        [Description("盘点单")]
        StoreStocktaking = 54,
        [Description("盘点修正单")]
        StoreStocktakingEdit = 55,

        [Description("调拨单")]
        TransferOrder = 60,
        [Description("残损单")]
        LossOrder = 61,

        [Description("批次号")]
        BatchNo = 90,

        [Description("出入库历史单据")]
        SapHistoryOrder = 70,

        [Description("品类充值券")]
        CategoryRechargeVoucher = 80,

        [Description("品牌充值券")]
        BrandRechargeVoucher = 81,
    }
}
