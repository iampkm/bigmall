using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
namespace Guoc.BigMall.Application.ViewObject
{
    public class StoreInventoryHistoryQueryDto
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

        /// <summary>
        /// 库存数
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 变动数
        /// </summary>
        public int ChangeQuantity { get; set; }
        /// <summary>
        /// 剩余库存
        /// </summary>
        public int CurrentQuantity
        {
            get
            {
                return Quantity + ChangeQuantity;
            }
        }
        /// <summary>
        /// 批次价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        public int BillId { get; set; }
        /// <summary>
        /// 单据编码
        /// </summary>
        public string BillCode { get; set; }
        public BillIdentity BillType { get; set; }

        public string BillTypeName
        {
            get
            {
                return BillType.Description();
            }
        }
        public DateTime CreatedOn { get; set; }
        public string CreateTime
        {
            get
            {
                return CreatedOn.ToString("yyyy-MM-dd HH:mm");
            }
        }
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
    }
}
