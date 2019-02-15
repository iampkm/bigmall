using Guoc.BigMall.Infrastructure.Utils;
using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class TransferOrder : BaseEntity
    {
        public TransferOrder()
        {
            this.Items = new List<TransferOrderItem>();
        }
        public string Code { get; set; }
        public string SapCode { get; set; }
        public TransferType Type { get; set; }
        public int FromStoreId { get; set; }
        public string FromStoreCode { get; set; }
        public string FromStoreName { get; set; }
        public string ToStoreCode { get; set; }
        public string ToStoreName { get; set; }
        public int ToStoreId { get; set; }
        public long? BatchNo { get; set; }
        public TransferStatus Status { get; set; }
        public string Remark { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime? AuditOn { get; set; }
        public int? AuditBy { get; set; }
        public string AuditByName { get; set; }
        public string AuditRemark { get; set; }
        /// <summary>
        /// 调拨单当前状态是否推送给了SAP
        /// </summary>
        public bool IsPushSap { get; set; }

        public List<TransferOrderItem> Items { get; set; }

        public void Edit(int updatedBy, string updatedByName, DateTime updatedOn)
        {
            this.UpdatedBy = updatedBy;
            this.UpdatedByName = updatedByName;
            this.UpdatedOn = updatedOn;
        }

        /// <summary>
        /// 门店制单时设置调拨单是向总仓申请调拨还是向门店申请调拨（根据调出门店判断）。
        /// </summary>
        public void SetType(Store fromStore)
        {
            Ensure.EqualThan(fromStore.Id, this.FromStoreId, "传入调出门店与调拨单调出门店不一致。");
            this.Type = fromStore.IsMainStore() ? TransferType.StoreApply : TransferType.StoreTransfer;
        }

        public void SetStore(Store fromStore, Store toStore)
        {
            Ensure.NotNull(fromStore, "找不到对应的调出门店。");
            Ensure.NotNull(toStore, "找不到对应的调入门店。");
            Ensure.NotEqualThan(fromStore.Id, toStore.Id, "相同门店不能做调入调出。");
            this.FromStoreId = fromStore.Id;
            this.FromStoreCode = fromStore.Code;
            this.FromStoreName = fromStore.Name;
            this.ToStoreId = toStore.Id;
            this.ToStoreCode = toStore.Code;
            this.ToStoreName = toStore.Name;
        }

        public void ApplyAudit(int updatedBy, string updatedByName, DateTime updatedOn)
        {
            Ensure.EqualThan(this.Type, TransferType.StoreApply, "门店向总仓申请的调拨单才能申请审核。");
            Ensure.EqualThan(this.Status, TransferStatus.Initial, "初始状态的调拨单才能申请审核。");
            this.Edit(updatedBy, updatedByName, updatedOn);
            this.Status = TransferStatus.WaitAudit;
        }

        public void PassAudit(int updatedBy, string updatedByName, DateTime updatedOn, string auditRemark)
        {
            Ensure.EqualThan(this.Type, TransferType.StoreApply, "门店向总仓申请的调拨单才能执行审核操作。");
            Ensure.EqualThan(this.Status, TransferStatus.WaitAudit, "待审状态的调拨单才能执行审核操作。");
            this.Edit(updatedBy, updatedByName, updatedOn);
            this.Status = TransferStatus.Audited;
            this.AuditOn = updatedOn;
            this.AuditBy = updatedBy;
            this.AuditByName = updatedByName;
            this.AuditRemark = auditRemark;
            this.IsPushSap = false;
        }

        public void RejectAudit(int updatedBy, string updatedByName, DateTime updatedOn, string auditRemark)
        {
            Ensure.EqualThan(this.Type, TransferType.StoreApply, "门店向总仓申请的调拨单才能执行审核操作。");
            Ensure.EqualThan(this.Status, TransferStatus.WaitAudit, "待审状态的调拨单才能执行审核操作。");
            this.Edit(updatedBy, updatedByName, updatedOn);
            this.Status = TransferStatus.Reject;
            this.AuditOn = updatedOn;
            this.AuditBy = updatedBy;
            this.AuditByName = updatedByName;
            this.AuditRemark = auditRemark;
        }

        //public void Cancel(int updatedBy, string updatedByName, DateTime updatedOn)
        //{
        //    Ensure.EqualThan(this.Type, TransferType.StoreApply, "门店向总仓申请的调拨单才能执行审核操作。");
        //    Ensure.In(this.Status, new[] { TransferStatus.Initial, TransferStatus.WaitAudit }, "初始或待审状态的调拨单才能执行作废操作。");
        //    this.Edit(updatedBy, updatedByName, updatedOn);
        //    this.Status = TransferStatus.Canceled;
        //}

        public void OutStock(int updatedBy, string updatedByName, DateTime updatedOn)
        {
            Ensure.EqualThan(this.Status, TransferStatus.WaitShipping, "待发货状态的调拨单才能执行出库操作。");
            this.Edit(updatedBy, updatedByName, updatedOn);
            this.Status = TransferStatus.Shipped;
            this.IsPushSap = false;
        }

        public void InStock(int updatedBy, string updatedByName, DateTime updatedOn)
        {
            Ensure.EqualThan(this.Status, TransferStatus.WaitReceiving, "待收货状态的调拨单才能执行入库操作。");
            this.Edit(updatedBy, updatedByName, updatedOn);
            this.Status = TransferStatus.Received;
            this.IsPushSap = false;
        }

    }
}
