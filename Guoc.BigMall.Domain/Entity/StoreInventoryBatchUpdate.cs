using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class StoreInventoryBatchUpdate:BaseEntity
    {
        /// <summary>
        /// 兼容旧代码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        public StoreInventoryBatchUpdate(int id, int quantity)
        {
            this.Id = id;
            this.Quantity = quantity;
        }
        public StoreInventoryBatchUpdate(int id, int quantity, int lockedQuantity)
        {
            this.Id = id;
            this.Quantity = quantity;
            this.LockedQuantity = lockedQuantity;
        }
        public int Quantity { get; set; }

        /// <summary>
        /// 已退数（单据已占用可退数）（退货专用）
        /// </summary>
        public int LockedQuantity { get; set; }
    }
}
