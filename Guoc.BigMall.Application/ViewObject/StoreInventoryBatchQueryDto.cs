using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.ViewObject
{
    public class StoreInventoryBatchQueryDto
    {
        /// <summary>
        /// 商品SKUID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 门店
        /// </summary>
        public int StoreId { get; set; }
        public string StoreName { get; set; }

        public int SupplierId { get; set; }
        /// <summary>
        /// 库存数
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 采购数
        /// </summary>
        public int PurchaseQuantity { get; set; }
        /// <summary>
        /// 锁定数
        /// </summary>
        public int LockedQuantity { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ProductionDate { get; set; }

        /// <summary>
        /// 保质期：单位天
        /// </summary>
        public int ShelfLife { get; set; }
        /// <summary>
        /// 批次价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string BarCode { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        public string SupplierName { get; set; }
        public string SNCode { get; set; }
        public string CreatedTime
        {
            get
            {
                return CreatedOn.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public int InventoryDay
        {
            get
            {
                return (DateTime.Now - CreatedOn).Days;
            }
        }
    }
}
