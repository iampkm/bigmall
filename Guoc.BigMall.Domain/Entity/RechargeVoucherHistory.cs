using Guoc.BigMall.Domain.ValueObject;
using System;

namespace Guoc.BigMall.Domain.Entity
{
    public class RechargeVoucherHistory : BaseEntity
    {
        public RechargeVoucherType VoucherType { get; set; }
        public int StoreId { get; set; }
        public string BillCode { get; set; }
        public BillIdentity BillType { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string SNCode { get; set; }
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public int Quantity { get; set; }
        public decimal BalanceBeforeChange { get; set; }
        public decimal ChangeAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
