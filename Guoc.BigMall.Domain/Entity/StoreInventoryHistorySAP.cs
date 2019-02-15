using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class StoreInventoryHistorySAP : BaseEntity
    {

        public StoreInventoryHistorySAP() { }
        public StoreInventoryHistorySAP(string code, StoreInventoryHistorySapType type, int productId, string productCode, int storeId, string storeCode,
            int quantity, string snCodes, string billCode, BillIdentity billType, int createdBy, string sapCode = null, string sapRow = null, int billItemId = 0, string BillSapRow = null, string BillSapCode = null)
        {
            this.Code = code;
            this.Type = type;
            this.ProductId = productId;
            this.ProductCode = productCode;
            this.StoreId = storeId;
            this.StoreCode = storeCode;
            this.Quantity = quantity;
            this.SNCodes = snCodes;
            this.BillCode = billCode;
            this.BillType = billType;
            this.CreatedBy = createdBy;
            this.CreatedOn = DateTime.Now;
            this.SAPCode = sapCode;
            this.SAPRow = sapRow;
            this.BillItemId = BillItemId;
            this.BillSapRow = BillSapRow;
            this.BillSapCode = BillSapCode;

        }
        public string Code { get; set; }
        public StoreInventoryHistorySapType Type { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public int Quantity { get; set; }
        public string SNCodes { get; set; }
        public string BillCode { get; set; }
        public BillIdentity BillType { get; set; }
        public string SAPCode { get; set; }
        public string SAPRow { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string BillSapRow { get; set; }
        public string BillSapCode { get; set; }

        /// <summary>
        /// 原单据明细行Id
        /// </summary>
        public int BillItemId { get; set; }
        /// <summary>
        ///  单位
        /// </summary>
        public string Unit { get; set; }
    }
}
