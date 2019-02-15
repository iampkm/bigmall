using System;
using System.Collections.Generic;
using System.Linq;

using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Domain.ValueObject;

namespace Guoc.BigMall.Application.DTO
{
    public class SaleOrderModel
    {
        public SaleOrderModel()
        {
            this.Items = new List<SaleOrderItemModel>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
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
        public int RoStatus { get; set; }
        public string RoStatusName
        {
            get
            {
                return this.OrderType == Guoc.BigMall.Domain.ValueObject.OrderType.Return.Value() ? ((ReturnSaleOrderStatus)this.RoStatus).Description() : "--";
            }
        }
        public string RefundAccount { get; set; }
        public SaleOrderStatus Status { get; set; }
        public string SNCode { get; set; }
        public string FJCode { get; set; }
        public string SourceSapCode { get; set; }
        public string SapCode { get; set; }
        public string StatusName
        {
            get
            {
                return this.OrderType == Guoc.BigMall.Domain.ValueObject.OrderType.Order.Value() ? Status.Description() : "--";
            }
        }
        public decimal OrderAmount { get; set; }
        public decimal PayAmount { get; set; }
        public decimal OnlinePayAmount { get; set; }
        public int? PaymentWay { get; set; }
        public DateTime? PaidDate { get; set; }
        public int? Hour { get; set; }
        public int CreatedBy { get; set; }
        public String CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnTime
        {
            get
            {
                return CreatedOn.HasValue ? CreatedOn.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;
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
        public string AuditedRemark { get; set; }
        public string WorkScheduleCode { get; set; }
        public int? OrderLevel { get; set; }
        public string AreaId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Buyer { get; set; }
        public string Remark { get; set; }
        public string SourceSaleOrderCode { get; set; }

        public string UpdatedByName { get; set; }
        public string CustomerCode { get; set; }
        /// <summary>
        /// 商品json
        /// </summary>
        //public string ItemsJson { get; set; }

        public List<SaleOrderItemModel> Items { get; set; }

        public decimal TotalAmount
        {
            get
            {
                return Items.Sum(n => n.RealPrice * n.Quantity);
            }
        }

        public decimal TotalQuantity
        {
            get
            {
                return Items.Sum(n => n.Quantity);
            }
        }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        /// <summary>
        ///  设置默认出入库时的默认实收，发数
        /// </summary>
        public void SetDefaultActualQuantity()
        {
            this.Items.ForEach(item => item.ActualQuantity = item.Quantity);
        }

    }
}