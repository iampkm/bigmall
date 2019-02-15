using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Domain.ValueObject;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class TransferOrderDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string SapCode { get; set; }
        public TransferType Type { get; set; }
        public string TypeDesc { get { return this.Type.Description(); } }
        public int FromStoreId { get; set; }
        public string FromStoreCode { get; set; }
        public string FromStoreName { get; set; }
        public int ToStoreId { get; set; }
        public string ToStoreCode { get; set; }
        public string ToStoreName { get; set; }
        public long? BatchNo { get; set; }
        public TransferStatus Status { get; set; }
        public string StatusDesc { get { return this.Status.Description(); } }
        public string Remark { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime? AuditOn { get; set; }
        public int AuditBy { get; set; }
        public string AuditByName { get; set; }
        public string AuditRemark { get; set; }

        public bool IsPushSap { get; set; }

        public string PushSap
        {
            get { return this.IsPushSap ? "成功" : "未推送"; }
        }
        public List<TransferOrderItemDto> Items { get; set; }
    }
}
