using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class StoreInventoryBatch : BaseEntity
    {
        public StoreInventoryBatch()
        {
            this.History = new List<StoreInventoryHistory>();
        }

        public StoreInventoryBatch(int productId, int storeId, int supplierId, int quantity, decimal contractPrice, decimal price, long batchNo,
                   DateTime? productionDate, int shelfLife, int createdBy, string snCode = null)
            : this()
        {
            this.ProductId = productId;
            this.StoreId = storeId;
            this.SupplierId = supplierId;
            this.Quantity = quantity;
            this.Price = price;
            this.BatchNo = batchNo;
            this.ProductionDate = productionDate;
            this.ShelfLife = shelfLife;
            this.CreatedBy = createdBy;
            this.CreatedOn = DateTime.Now;
            this.ContractPrice = contractPrice;
            this.PurchaseQuantity = quantity; // 入库数量恒定不变 
            this.SNCode = snCode;
        }

        /// <summary>
        /// 商品SKUID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 门店
        /// </summary>
        public int StoreId { get; set; }

        public int SupplierId { get; set; }
        /// <summary>
        /// 批次采购数
        /// </summary>
        public int PurchaseQuantity { get; set; }
        /// <summary>
        /// 销售剩余批次库存数
        /// </summary>
        public int Quantity { get; set; }


        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ProductionDate { get; set; }

        /// <summary>
        /// 保质期：单位天
        /// </summary>
        public int ShelfLife { get; set; }
        /// <summary>
        /// 合同价
        /// </summary>
        public decimal ContractPrice { get; set; }

        /// <summary>
        /// 实际进货价 （赠品价位 0）
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public long BatchNo { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }

        /// <summary>
        ///  并发控制字段
        /// </summary>
        public byte[] RowVersion { get; private set; }

        public string SNCode { get; set; }

        public int LockedQuantity { get; set; }

        public List<StoreInventoryHistory> History { get; set; }

    }
}
