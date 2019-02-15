using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class StoreInventoryHistory : BaseEntity
    {
        public StoreInventoryHistory() { }
        public StoreInventoryHistory(int productId, int storeId, int quantity, int changeQuantity, decimal price, long batchNo,
            int billId, string billCode, BillIdentity billType, int createdBy, int supplierId, string snCode)
        {
            this.ProductId = productId;
            this.StoreId = storeId;
            this.Quantity = quantity;
            this.ChangeQuantity = changeQuantity;
            this.Price = price;
            this.BatchNo = batchNo;
            this.BillId = billId;
            this.BillCode = billCode;
            this.BillType = billType;
            this.CreatedBy = createdBy;
            this.CreatedOn = DateTime.Now;
            this.SupplierId = supplierId;
            this.SNCode = snCode;
        }

        public StoreInventoryHistory(int storeInventoryBatchId, int productId, int storeId, int quantity, int changeQuantity, decimal price, long batchNo,
            int billId, string billCode, BillIdentity billType, int createdBy, DateTime createdOn, int supplierId, decimal realPrice = 0m, string snCode = null, decimal categoryPreferential = 0, decimal brandPreferential = 0)
        {
            this.StoreInventoryBatchId = storeInventoryBatchId;
            this.ProductId = productId;
            this.StoreId = storeId;
            this.Quantity = quantity;
            this.ChangeQuantity = changeQuantity;
            this.Price = price;
            this.BatchNo = batchNo;
            this.BillId = billId;
            this.BillCode = billCode;
            this.BillType = billType;
            this.CreatedBy = createdBy;
            this.CreatedOn = createdOn;
            this.SupplierId = supplierId;
            this.RealPrice = realPrice;
            this.SNCode = snCode;
            this.CategoryPreferential = categoryPreferential;
            this.BrandPreferential = brandPreferential;
        }

        /// <summary>
        /// 商品SKUID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 门店
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 库存数
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 变动数
        /// </summary>
        public int ChangeQuantity { get; set; }
        /// <summary>
        /// 批次价格（成本价）
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public long BatchNo { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        public int BillId { get; set; }
        /// <summary>
        /// 单据编码
        /// </summary>
        public string BillCode { get; set; }
        public BillIdentity BillType { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public int SupplierId { get; set; }
        /// <summary>
        /// 售价
        /// </summary>
        public decimal RealPrice { get; set; }
        /// <summary>
        /// 当前批次的品类优惠总额
        /// </summary>
        public decimal CategoryPreferential { get; set; }
        /// <summary>
        /// 当前批次的品牌优惠总额
        /// </summary>
        public decimal BrandPreferential { get; set; }
        public string SNCode { get; set; }
        public int StoreInventoryBatchId { get; set; }
    }
}
