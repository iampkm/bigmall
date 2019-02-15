using System;
using System.Collections.Generic;
using System.Linq;

using Guoc.BigMall.Domain.ValueObject;

namespace Guoc.BigMall.Application.DTO
{
    public class SaleOrderDto
    {
        public SaleOrderDto()
        {
            this.Items = new List<SaleOrderItemDto>();
            this.CreatedOn = DateTime.Now;
            this.UpdatedOn = DateTime.Now;
            this.OrderLevel = (int)SaleOrderLevel.General;
        }

        public int Id { get; set; }
        /// <summary>
        /// 订单编号 ： 单据类型 2 + 账号4位+ （日期20160101） 3位+ 下单的秒数（86400，5位）+ 2位随机
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 良品仓门店代码
        /// </summary>
        public string StoreCode { get; set; }
        /// <summary>
        /// 赠品仓
        /// </summary>
        public int StoreIdGift { get; set; }
        /// <summary>
        /// 赠品仓门店代码
        /// </summary>
        public string StoreCodeGift { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }

        public string CreatedByName { get; set; }
        /// <summary>
        /// Pos 机ID
        /// </summary>
        public int PosId { get; set; }
        /// <summary>
        /// 订单类型：销售单1，销售退单2
        /// </summary>
        public int OrderType { get; set; }
        public int BillType { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public int? PaymentWay { get; set; }
        /// <summary>
        /// 退款账户
        /// </summary>
        public string RefundAccount { get; set; }
        /// <summary>
        /// 支付日期
        /// </summary>
        public DateTime? PaidDate { get; set; }

        public int Hour { get; set; }

        /// <summary>
        /// 订单金额 = 实际价格RealAmount * 数量
        /// </summary> 
        public decimal OrderAmount { get; set; }
        /// <summary>
        /// 现金支付金额
        /// </summary>
        public decimal PayAmount { get; set; }
        /// <summary>
        /// 刷卡支付，微信支付，阿里支付等在线支付金额
        /// </summary>
        public decimal OnlinePayAmount { get; set; }
        /// <summary>
        /// 销售单状态
        /// </summary>
        public SaleOrderStatus Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

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
        /// <summary>
        /// 班次代码
        /// </summary>
        public string WorkScheduleCode { get; set; }

        public int OrderLevel { get; set; }

        public string AreaId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Buyer { get; set; }
        public int RoStatus { get; set; }
        public string SapCode { get; set; }
        public string SourceSaleOrderCode { get; set; }
        public string SourceSapCode { get; set; }
        /// <summary>
        /// 父单号 P+单号
        /// </summary>
        public string ParentCode { get; set; }
        public virtual List<SaleOrderItemDto> Items { get; set; }

        public string Remark { get; set; }

        public string CustomerCode { get; set; }
        public void SetItems(List<SaleOrderItemDto> saleOrderItems)
        {
            foreach (var orderItem in saleOrderItems)
                this.SetItem(orderItem);
        }

        public void SetItem(SaleOrderItemDto orderItem)
        {
            //if (this.Items.Any(x => x.ProductId == orderItem.ProductId && x.ParentProductId == null) == false)
            //    this.Items.Add(orderItem);

            //if (this.Items.Any(x => x.ProductId == orderItem.ProductId && x.ParentProductId == orderItem.ParentProductId) == false)
            //    this.Items.Add(orderItem);

            if (this.Items.Exists(x => x.ProductId == orderItem.ProductId && x.GiftType == orderItem.GiftType && x.ParentProductId == orderItem.ParentProductId) == false)
                this.Items.Add(orderItem);
        }
        public void SetOrderAmount()
        {
            foreach (var saleOrderItem in Items)
            {
                OrderAmount += saleOrderItem.Quantity * saleOrderItem.RealPrice;
            }
        }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string SNCode { get; set; }
    }
}