using Guoc.BigMall.Domain.ValueObject;
using Guoc.BigMall.Infrastructure.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Guoc.BigMall.Application.DTO
{
    public class TransferOrderDetailDto
    {
        public int Id { get; set; }
        [Description("调拨单号")]
        public string Code { get; set; }
        [Description("Sap调拨单号")]
        public string SapCode { get; set; }
        public TransferType Type { get; set; }
        [Description("类型")]
        public string TypeDesc { get { return this.Type.Description(); } }
        public int FromStoreId { get; set; }
        [Description("调出门店编码")]
        public string FromStoreCode { get; set; }
        [Description("调出门店名称")]
        public string FromStoreName { get; set; }
        public int ToStoreId { get; set; }
        [Description("调入门店编码")]
        public string ToStoreCode { get; set; }
        [Description("调入门店名称")]
        public string ToStoreName { get; set; }
        public long? BatchNo { get; set; }
        public TransferStatus Status { get; set; }
        [Description("状态")]
        public string StatusDesc { get { return this.Status.Description(); } }
        [Description("备注")]
        public string Remark { get; set; }
        [Description("制单时间")]
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        [Description("制单人")]
        public string CreatedByName { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime? AuditOn { get; set; }
        public int AuditBy { get; set; }
        [Description("审核人")]
        public string AuditByName { get; set; }
        [Description("审核意见")]
        public string AuditRemark { get; set; }

        public bool IsPushSap { get; set; }

        public string PushSap
        {
            get { return this.IsPushSap ? "成功" : "未推送"; }
        }


        [Description("商品编码")]
        public string ProductCode { get; set; }
        [Description("商品名称")]
        public string ProductName { get; set; }
        [Description("型号")]
        public string Spec { get; set; }
        [Description("调拨数")]
        public int Quantity { get; set; }
        [Description("实发数")]
        public int ActualShipmentQuantity { get; set; }
        [Description("实收数")]
        public int ActualReceivedQuantity { get; set; }
    }
}
