using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class StoreInventory : BaseEntity
    {
        public StoreInventory()
        {
            this.IsQuit = false;
            this.Status = StoreInventoryStatus.Normal;
        }
        public StoreInventory(int storeId, int productId, int quantity, decimal avgCostPrice)
            : this()
        {
            this.StoreId = storeId;
            this.ProductId = productId;
            this.Quantity = quantity;
            this.AvgCostPrice = avgCostPrice;
            this.Status = StoreInventoryStatus.Normal;
        }
        /// <summary>
        /// 商品SKUID
        /// </summary>
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        /// <summary>
        /// 可售总数量（此字段无实际用处）
        /// </summary>
        public int SaleQuantity { get; set; }
        /// <summary>
        /// 预订总数量
        /// </summary>
        public int OrderQuantity { get; set; }
        /// <summary>
        /// 库存总数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 移动加权平均成本
        /// </summary>
        public decimal AvgCostPrice { get; set; }
        /// <summary>
        /// 预警数量
        /// </summary>
        public int WarnQuantity { get; set; }
        /// <summary>
        /// 是否退市（例如不再进货销售）
        /// </summary>
        public bool IsQuit { get; set; }
        /// <summary>
        /// 库存状态
        /// </summary>
        public StoreInventoryStatus Status { get; set; }
        /// <summary>
        /// 最后一次进价，如果是赠品，价格为0 的，不更新此价格
        /// </summary>
        public decimal LastCostPrice { get; set; }
        /// <summary>
        /// 门店售价
        /// </summary>
        public decimal StoreSalePrice { get; set; }

        /// <summary>
        /// 退货锁定库存
        /// </summary>
        public int LockedQuantity { get; set; }

        /// <summary>
        ///  并发控制字段
        /// </summary>
        public byte[] RowVersion { get; private set; }

        /// <summary>
        /// 实际可退库存
        /// </summary>
        /// <returns></returns>
        public int GetActualRefundQuantity()
        {
            var quantity = this.Quantity - this.LockedQuantity > 0 ? this.Quantity - this.LockedQuantity : 0;
            return quantity;
        }
    }
}
