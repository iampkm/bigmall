using Guoc.BigMall.Domain.ValueObject;
using Guoc.BigMall.Infrastructure.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class BrandRechargeVoucherDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BrandId { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string Categories { get; set; }
        public decimal Amount { get; set; }
        public decimal Reduced { get; set; }
        public decimal Balance { get; set; }
        public float Limit { get; set; }
        public string ExceptProducts { get; set; }
        public RechargeVoucherStatus Status { get; set; }
        public string StatusDesc
        {
            get
            {
                return this.Status.GetDescription();
            }
        }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? AuditOn { get; set; }
        public int? AuditBy { get; set; }
        public string AuditByName { get; set; }
    }
}
