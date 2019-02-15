using Guoc.BigMall.Infrastructure.Utils;
using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;

namespace Guoc.BigMall.Domain.Entity
{
    public class BrandRechargeVoucher : BaseEntity
    {
        public string Code { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BrandId { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public decimal Amount { get; set; }
        public decimal Reduced { get; set; }
        public decimal Balance { get; set; }
        public decimal Limit { get; set; }
        public RechargeVoucherStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? AuditOn { get; set; }
        public int? AuditBy { get; set; }
        public string AuditByName { get; set; }

        public byte[] RowVersion { get; private set; }


        /// <summary>
        /// 参与的类别
        /// </summary>
        public List<ParticipantCategory> ParticipantCategories { get; set; }
        /// <summary>
        /// 排除的商品
        /// </summary>
        public List<ExceptProduct> ExceptProducts { get; set; }

        public BrandRechargeVoucher()
        {
            this.ParticipantCategories = new List<ParticipantCategory>();
            this.ExceptProducts = new List<ExceptProduct>();
        }

        public void PassAudit(int auditBy, string auditByName)
        {
            Ensure.EqualThan(this.Status, RechargeVoucherStatus.WaitAudit, "非待审状态无法审核。");

            this.AuditBy = auditBy;
            this.AuditByName = auditByName;
            this.AuditOn = DateTime.Now;
            this.Status = RechargeVoucherStatus.Normal;
        }

        public void RejectAudit(int auditBy, string auditByName)
        {
            Ensure.EqualThan(this.Status, RechargeVoucherStatus.WaitAudit, "非待审状态无法审核。");

            this.AuditBy = auditBy;
            this.AuditByName = auditByName;
            this.AuditOn = DateTime.Now;
            this.Status = RechargeVoucherStatus.Rejected;
        }

        public void Abort()
        {
            Ensure.EqualThan(this.Status, RechargeVoucherStatus.Normal, "非可用状态无法中止。");
            this.Status = RechargeVoucherStatus.Aborted;
        }
    }
}
