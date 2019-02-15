using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
namespace Guoc.BigMall.Application.ViewObject
{
    public class PurchaseOrderDetailListDto
    {

        [Description("制单日期")]
        public string CreatedTime
        {
            get
            {
                return CreatedOn.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        [Description("门店名称")]
        public string StoreName { get; set; }
        [Description("门店编码")]
        public string StoreCode { get; set; }
        [Description("商品名称")]
        public string ProductName { get; set; }
        [Description("商品编码")]
        public string ProductCode { get; set; }
        /// <summary>
        /// 预订数量
        /// </summary>
        [Description("数量")]
        public int Quantity { get; set; }
        [Description("单价")]
        public decimal CostPrice { get; set; }

        [Description("合计金额")]
        /// <summary>
        /// 订货金额
        /// </summary>
        public decimal Amount { get; set; }

        public decimal SignalAmount { get { return this.CostPrice * this.Quantity; } }
        [Description("订单状态")]
        public string PurchaseOrderStatus
        {
            get
            {
                return Status.Description();
            }
        }
        [Description("订单号")]
        public string Code { get; set; }
        [Description("SAP单号")]
        public string SapOrderId { get; set; }
        [Description("供应商")]
        public string SupplierName { get; set; }
        [Description("供应商编码")]
        public string SupplierCode { get; set; }
        [Description("品牌")]
        public string BrandName { get; set; }
        [Description("品类")]
        public string CategoryName { get; set; }

        [Description("订单类型")]
        public string BillTypeName
        {
            get
            {
                return BillType.Description();
            }
        }
      
        public int Id { get; set; }

        public string ParentCode { get; set; }

        public string Supplier
        {
            get
            {
                return string.Format("[{0}]{1}", SupplierCode, SupplierName);
            }
        }
        public CBPurchaseOrderStatus Status { get; set; }
        public PurchaseOrderType OrderType { get; set; }
        public string OrderTypeName
        {
            get
            {
                return OrderType.Description();
            }
        }

        public DateTime CreatedOn { get; set; }





        public string CreatedByName { get; set; }
      



        /// <summary>
        /// 供应商备注：可以备注单号，其他信息
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 财务备注
        /// </summary>
        public string FinanceRemark { get; set; }
   
        public PurchaseOrderBillType BillType { get; set; }

     
    
        public int ActualQuantity { get; set; }

        public bool IsPushSap { get; set; }

        public string PushSap
        {
            get { return this.IsPushSap ? "成功" : "未推送"; }
        }

       // #region 明细信息
       // public int ProductId { get; set; }



       // /// <summary>
       // /// 规格
       // /// </summary>
       // public string Specification { get; set; }
       // /// <summary>
       // /// 单位
       // /// </summary>
       // public string Unit { get; set; }


       //// public int ActualShipQuantity { get; set; }
       // public int ActualQuantity { get; set; }

       // /// <summary>
       // /// 库存数量 （采购单：SAP库存，退单：门店库存）
       // /// </summary>
       // public int InventoryQuantity { get; set; }


       // /// <summary>
       // /// 实收金额
       // /// </summary>
       // public decimal ActualAmount { get; set; }
       // /// <summary>
       // /// 实发金额
       // /// </summary>
       //// public decimal ActualShipAmount { get; set; }
       // #endregion
    }
}
