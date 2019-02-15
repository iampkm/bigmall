using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
using System.ComponentModel;
namespace Guoc.BigMall.Application.ViewObject
{
    /// <summary>
    ///  销售综合查询列表明细
    /// </summary>
    public class SaleOrderListDetailDto
    {
        public int Id { get; set; }
        [Description("单号")]
        public string Code { get; set; }
        public int StoreId { get; set; }
        [Description("销售门店")]
        public string StoreName { get; set; }
        /// <summary>
        /// 赠品仓
        /// </summary>
        public int StoreIdGift { get; set; }
        public int? PosId { get; set; }
        public int? OrderType { get; set; }
        public SaleOrderBillType BillType { get; set; }
        public string OrderTypeName
        {
            get
            {
                var billType = this.BillType.Description();
                return "{0}{1}".FormatWith(billType.Substring(0, billType.Length - 1), ((OrderType)this.OrderType).Description());
            }
        }
        public ReturnSaleOrderStatus RoStatus { get; set; }
        [Description("退单状态")]
        public string RoStatusName
        {
            get
            {
                return this.OrderType == Guoc.BigMall.Domain.ValueObject.OrderType.Return.Value() ? RoStatus.Description() : "--";
            }
        }
        public string RefundAccount { get; set; }
        public SaleOrderStatus Status { get; set; }
        [Description("SAP单号")]
        public string SapCode { get; set; }
        [Description("订单状态")]
        public string StatusName
        {
            get
            {
                return this.OrderType == Guoc.BigMall.Domain.ValueObject.OrderType.Order.Value() ? Status.Description() : RoStatus.Description();
            }
        }
        public decimal OrderAmount { get; set; }
        public decimal PayAmount { get; set; }
        public decimal OnlinePayAmount { get; set; }
        public int? PaymentWay { get; set; }
        public DateTime? PaidDate { get; set; }
        [Description("销售时间")]
        public string PaidDateTime
        {
            get
            {
                return this.PaidDate.HasValue ? this.PaidDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            }
        }
        public int? Hour { get; set; }
        public int CreatedBy { get; set; }
        [Description("制单人")]
        public String CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        [Description("制单时间")]
        public string CreatedOnTime
        {
            get
            {
                return CreatedOn.HasValue ? CreatedOn.Value.ToString("yyyy-MM-dd") : string.Empty;
            }
        }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? AuditedBy { get; set; }

        public string AuditedByName { get; set; }
        public DateTime? AuditedOn { get; set; }

        public string AuditedOnTime
        {
            get
            {
                return AuditedOn.HasValue ? AuditedOn.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;
            }
        }
        [Description("审核意见")]
        public string AuditedRemark { get; set; }
        public string WorkScheduleCode { get; set; }
        public int? OrderLevel { get; set; }
        public string AreaId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Buyer { get; set; }
        [Description("备注")]
        public string Remark { get; set; }
        public string SourceSaleOrderCode { get; set; }

        public string UpdatedByName { get; set; }

        // 明细
        [Description("商品编码")]
        public string ProductCode { get; set; }
        [Description("商品名称")]
        public string ProductName { get; set; }
        [Description("品类名称")]
        public string CategoryName { get; set; }
        [Description("品牌名称")]
        public string BrandName { get; set; }
        [Description("规格")]
        public string Specification { get; set; }
        [Description("串码")]
        public string SNCode { get; set; }

        [Description("售价")]
        public decimal RealPrice { get; set; }

        public decimal MinSalePrice { get; set; }

        [Description("数量")]
        public int Quantity { get; set; }

        [Description("金额")]
        public decimal Amount
        {
            get
            {
                return RealPrice * Quantity;
            }
        }

        [Description("富基单号")]
        public string FJCode { get; set; }

        /// <summary>
        ///  实发实收数，批发出入库使用
        /// </summary>
        [Description("实发/收数(批发)")]
        public int ActualQuantity { get; set; }
        /// <summary>
        ///  实发出入库金额
        /// </summary>
        [Description("实际金额(批发)")]
        public decimal ActualAmount
        {
            get
            {
                return RealPrice * ActualQuantity;
            }
        }

        public bool IsPushSap { get; set; }

        public string PushSap
        {
            get { return this.IsPushSap ? "成功" : "未推送"; }
        }

    }
}
