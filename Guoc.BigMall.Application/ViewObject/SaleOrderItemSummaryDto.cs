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
    public class SaleOrderItemSummaryDto
    {
        [Description("单号")]
        public string SaleOrderCode { get; set; }
        [Description("日期")]
        public DateTime? CreatedOn { get; set; }
        [Description("门店")]
        public string StoreName { get; set; }

        public int SaleOrderId { get; set; }

        public int ProductId { get; set; }
        [Description("商品编码")]
        public string ProductCode { get; set; }
        [Description("商品名称")]
        public string ProductName { get; set; }
        [Description("串码")]
        public string SNCode { get; set; }
        [Description("数量")]
        public int Quantity { get; set; }
        [Description("单价")]
        public decimal RealPrice { get; set; }
        [Description("金额")]
        public decimal Amount
        {
            get
            {
                return this.RealPrice * this.Quantity;
            }
        }
        [Description("税率")]
        public string tax { get; set; }
        [Description("赠品明细")]
        public string GiftItem { get; set; }
        [Description("业务员")]
        public string BusinessUser { get; set; }

        public SaleOrderBillType BillType { get; set; }

        [Description("出库类型")]
        public string BillTypeName
        {
            get
            {
                return this.BillType.Description();
            }
        }
        public string Buyer { get; set; }
        public int StoreId { get; set; }
        public int AuditedBy { get; set; }
        [Description("审核人")]
        public string AuditeUser { get; set; }
        [Description("审核时间")]
        public DateTime? AuditedOn { get; set; }
        public int CreatedBy { get; set; }
        [Description("制单人")]
        public string CreateUser { get; set; }
        [Description("备注")]
        public string Remark { get; set; }



        public string AuditedOnStr
        {
            get
            {
                return (AuditedOn == null ? null : AuditedOn.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        public string CreatedOnStr
        {
            get
            {
                return (CreatedOn == null ? null : CreatedOn.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        public OrderProductType GiftType { get; set; }
        public int ParentProductId { get; set; }
    }
}
