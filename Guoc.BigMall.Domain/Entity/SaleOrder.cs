using System;
using System.Collections.Generic;
using System.Linq;

using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Domain;
using Guoc.BigMall.Domain.ValueObject;
using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Utils;

namespace Guoc.BigMall.Domain.Entity
{
    public class SaleOrder : BaseEntity
    {
        public SaleOrder()
        {
            this.Items = new List<SaleOrderItem>();
            this.CreatedOn = DateTime.Now;
            this.UpdatedOn = DateTime.Now;
            this.OrderLevel = (int)SaleOrderLevel.General;
            this.BillType = SaleOrderBillType.SaleOrder;
        }
        /// <summary>
        /// 订单编号 ： 单据类型 2 + 账号4位+ （日期20160101） 3位+ 下单的秒数（86400，5位）+ 2位随机
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 良品仓门店
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 良品仓门店代码
        /// </summary>
        public string StoreCode { get; set; }
        /// <summary>
        /// 赠品仓门店
        /// </summary>
        public int StoreIdGift { get; set; }
        /// <summary>
        /// 赠品仓门店代码
        /// </summary>
        public string StoreCodeGift { get; set; }

        /// <summary>
        /// Pos 机ID
        /// </summary>
        public int PosId { get; set; }
        /// <summary>
        /// 订单类型：销售单1，销售退单2
        /// </summary>
        public int OrderType { get; set; }
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
        /// <summary>
        ///  原销售单号： 退单时填写
        /// </summary>
        public string SourceSaleOrderCode { get; set; }
        /// <summary>
        /// 原SAP 单号： 退单时填写
        /// </summary>
        public string SourceSapCode { get; set; }
        /// <summary>
        /// 父单号 P+单号
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        ///  SAP 订单号
        /// </summary>
        public string SapCode { get; set; }


        public virtual List<SaleOrderItem> Items { get; set; }

        public string Remark { get; set; }

        public int? AuditedBy { get; set; }
        public DateTime? AuditedOn { get; set; }
        public string AuditedRemark { get; set; }
        public bool IsPushSap { get; set; }
        public SaleOrderBillType BillType { get; set; }
        /// <summary>
        ///  SAP 客户编码- 一个客户编码对应商码特多个StoreCode
        /// </summary>
        public string CustomerCode { get; set; }
        public string GetBillIdentityDescription()
        {
            return this.OrderType == (int)Domain.ValueObject.OrderType.Order ? BillIdentity.SaleOrder.Description() : BillIdentity.SaleRefund.Description();
        }

        public BillIdentity GetBillIdentity()
        {
            if (this.OrderType == ValueObject.OrderType.Order.Value() && BillType == SaleOrderBillType.SaleOrder)
            {
                return BillIdentity.SaleOrder;
            }
            if (this.OrderType == ValueObject.OrderType.Return.Value() && BillType == SaleOrderBillType.SaleOrder)
            {
                return BillIdentity.SaleRefund;
            }
            if (this.OrderType == ValueObject.OrderType.Order.Value() && BillType == SaleOrderBillType.BatchOrder)
            {
                return BillIdentity.BatchOrder;
            }
            if (this.OrderType == ValueObject.OrderType.Return.Value() && BillType == SaleOrderBillType.BatchOrder)
            {
                return BillIdentity.BatchRefund;
            }
            if (this.OrderType == ValueObject.OrderType.Order.Value() && BillType == SaleOrderBillType.PreSaleOrder)
            {
                return BillIdentity.PreSaleOrder;
            }
            if (this.OrderType == ValueObject.OrderType.Return.Value() && BillType == SaleOrderBillType.PreSaleOrder)
            {
                return BillIdentity.PreSaleRefund;
            }
            if (this.OrderType == ValueObject.OrderType.Order.Value() && BillType == SaleOrderBillType.ExchangeOrder)
            {
                return BillIdentity.ExchangeOrder;
            }
            throw new FriendlyException("单据类型异常");
            // return this.OrderType == (int)Domain.ValueObject.OrderType.Order ? BillIdentity.SaleOrder : BillIdentity.SaleRefund;
        }

        public string GenderateNewCode(BillIdentity billIdentity)
        {
            string createdBy = this.CreatedBy > 9999 ? this.CreatedBy.ToString().Substring(0, 4) : this.CreatedBy.ToString().PadLeft(4, '0'); // 4位
            var dayDiff = (this.CreatedOn.Date - new DateTime(2016, 1, 1)).Days;  // 5位
            // 天数不足4位补足4位
            var dayDiffString = dayDiff.ToString().Length > 4 ? dayDiff.ToString() : dayDiff.ToString().PadLeft(4, '0');
            var date = this.CreatedOn;
            var ts = date - Convert.ToDateTime(date.ToShortDateString());
            var seconds = Math.Truncate(ts.TotalSeconds).ToString().PadLeft(6, '0');  // 5位
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            var rdNumber = rd.Next(0, 100);
            var rdNumberString = rdNumber > 9 ? rdNumber.ToString() : "0" + rdNumber.ToString();
            return string.Format("{0}{1}{2}{3}{4}", billIdentity.Value(), createdBy, dayDiffString, seconds, rdNumberString);
        }

        public void AuditdRo(int updateBy)
        {
            if (this.RoStatus == (int)SaleOrderStatus.WaitAudit)
            {
                this.RoStatus = (int)SaleOrderStatus.Audited;
                this.UpdatedBy = updateBy;
                this.UpdatedOn = DateTime.Now;
            }
            else
            {
                throw new Exception("订单不是待审核状态，不能审核");
            }
        }

        public void SetItems(List<SaleOrderItem> saleOrderItems)
        {
            foreach (var orderItem in saleOrderItems)
                this.SetItem(orderItem);

            //this.SetOrderAmount();
        }

        public void SetItem(SaleOrderItem orderItem)
        {
            orderItem.SaleOrderId = this.Id;

            //if (this.Items.Any(x => x.ProductId == orderItem.ProductId && x.ParentProductId == null) == false)
            //    this.Items.Add(orderItem);

            //if (this.Items.Any(x => x.ProductId == orderItem.ProductId && x.ParentProductId == orderItem.ParentProductId) == false)
            //    this.Items.Add(orderItem);

            if (this.Items.Exists(x => x.ProductId == orderItem.ProductId && x.GiftType == orderItem.GiftType && x.ParentProductId == orderItem.ParentProductId) == false)
                this.Items.Add(orderItem);

            this.SetOrderAmount();
        }

        public void InitStatus()
        {
            if (this.OrderType == (int)Domain.ValueObject.OrderType.Order)
                this.Status = SaleOrderStatus.WaitAudit;
            else
                this.RoStatus = (int)ReturnSaleOrderStatus.WaitAudit;
        }

        public void SetOrderAmount()
        {
            this.OrderAmount = 0;

            foreach (var saleOrderItem in Items)
            {
                OrderAmount += saleOrderItem.Quantity * saleOrderItem.RealPrice;
            }
        }
        /// <summary>
        /// ZSM5-商玛特批发销售、ZSM6-商玛特批发退货
        /// </summary>
        /// <param name="sapOrderType"></param>
        public void SetSapBatchOrderType(string sapOrderType)
        {
            if (sapOrderType == "ZSM5")
            {
                this.OrderType = 1;
                this.BillType = SaleOrderBillType.BatchOrder;
                this.Status = SaleOrderStatus.WaitOutStock;  // 待出库
                this.RoStatus = 0;
            }
            else if (sapOrderType == "ZSM6")
            {
                this.OrderType = 2;
                this.BillType = SaleOrderBillType.BatchOrder;
                this.RoStatus = ReturnSaleOrderStatus.WaitInStock.Value();//  待入库
                this.Status = SaleOrderStatus.Create;
            }
            else
            {
                throw new FriendlyException(string.Format("SAP单据类型：{0} 有错!", sapOrderType));
            }
        }

        /// <summary>
        /// 关闭预销售单
        /// </summary>
        public void ClosePreSaleOrder()
        {
            this.Status = SaleOrderStatus.Cancel;
            this.UpdatedBy = 1;
            this.UpdatedOn = DateTime.Now;

        }

        public void AuditedOrder(string remark, int currentAccountId)
        {
            Ensure.EqualThan(this.Status, SaleOrderStatus.WaitAudit, "订单号：{0} 当前不是待审核状态。".FormatWith(this.Code));

            this.Status = SaleOrderStatus.WaitOutStock;
            this.AuditedRemark = remark;
            this.AuditedOn = DateTime.Now;
            this.AuditedBy = currentAccountId;
        }

        /// <summary>
        ///  获取商玛特订单类型 L-零售销售 、T-零售退货、H-零售换货、Y-预销售
        /// </summary>
        /// <returns></returns>
        public string GetSAPOrderType()
        {
            if (this.OrderType == ValueObject.OrderType.Order.Value() && this.BillType == SaleOrderBillType.SaleOrder)
            {
                return "ZSM1";
            }
            if (this.OrderType == ValueObject.OrderType.Return.Value() && this.BillType == SaleOrderBillType.SaleOrder)
            {
                return "ZSM2";
            }
            if (this.OrderType == ValueObject.OrderType.Order.Value() && this.BillType == SaleOrderBillType.ExchangeOrder)
            {
                return "ZSM4";
            }
            if (this.OrderType == ValueObject.OrderType.Order.Value() && this.BillType == SaleOrderBillType.PreSaleOrder)
            {
                return "ZSM3";
            }
            return "";
        }

        /// <summary>
        /// //分销渠道 10 零售、20 团购
        /// </summary>
        /// <returns></returns>
        public string GetSapSaleMethod()
        {
            return this.BillType == SaleOrderBillType.BatchOrder ? "20" : "10";
        }

        /// <summary>
        ///   当商玛特订单类型= T、H 换货时，需用退货标识区分退货行项目，退货标识“X”
        /// </summary>
        /// <returns></returns>
        public string GetSAPReturnFlag()
        {
            return (this.GetSAPOrderType() == "T" || this.GetSAPOrderType() == "H") ? "X" : "";
        }

        /// <summary>
        /// 是否已发货。
        /// </summary>
        public bool IsShipped()
        {
            return this.Status.In(SaleOrderStatus.OutStock, SaleOrderStatus.Convert);
        }

        /// <summary>
        /// 是否退货。
        /// </summary>
        /// <returns></returns>
        public bool IsReturned()
        {
            return this.RoStatus != 0;
        }
    }
}