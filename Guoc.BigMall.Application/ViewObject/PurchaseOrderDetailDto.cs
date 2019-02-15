using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
namespace Guoc.BigMall.Application.ViewObject
{
    /// <summary>
    /// 采购单明细dto
    /// </summary>
    public class PurchaseOrderDetailDto
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string ParentCode { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }

        public int SupplierId { get; set; }
        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public CBPurchaseOrderStatus Status { get; set; }

        public string PurchaseOrderStatus
        {
            get
            {
                return Status.Description();
            }
        }
        public DateTime CreatedOn { get; set; }

        public string CreatedTime
        {
            get
            {
                return CreatedOn.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        //public OrderType OrderType { get; set; }
        //public string OrderTypeName
        //{
        //    get
        //    {
        //        return OrderType.Description();
        //    }
        //}
        public string SapOrderId { get; set; }

        public string CreatedByName { get; set; }
        ///// <summary>
        ///// 支付方式
        ///// </summary>
        //public PaymentWay PaymentWay { get; set; }
        //public ShippingMethod Shipping { get; set; }

        /// <summary>
        /// 供应商备注：可以备注单号，其他信息
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 财务备注
        /// </summary>
        public string FinanceRemark { get; set; }

        public PurchaseOrderBillType BillType { get; set; }

        /// <summary>
        /// 门店详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        public string Buyer { get; set; }


        #region 明细信息
        public int ProductId { get; set; }
        /// <summary>
        /// 重百编码
        /// </summary>
        public string SapProductCode { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        public decimal CostPrice { get; set; }

        /// <summary>
        /// 预订数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 库存数量 （采购单：SAP库存，退单：门店库存）
        /// </summary>
        public int InventoryQuantity { get; set; }
        #endregion
    }
}
