using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class StoreInventoryUpdate:BaseEntity
    {
        /// <summary>
        /// 增减数库存数 
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 可售库存
        /// </summary>
        public int SaleQuantity { get; set; }
        /// <summary>
        /// 订购库存
        /// </summary>
        public int OrderQuantity { get; set; }
        /// <summary>
        /// 库存商品移动加权平均价
        /// </summary>
        public decimal AvgCostPrice { get; set; }

        /// <summary>
        /// 最后一次进价
        /// </summary>
        public decimal LastCostPrice { get; set; }
    }
}
