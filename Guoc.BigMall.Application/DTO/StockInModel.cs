using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class StockInModel
    {
        /// <summary>
        /// 执行入库操作的单据ID（例如：调拨入库，传递调拨单ID）
        /// </summary>
        public int StockInBillId { get; set; }
        /// <summary>
        /// 【退货入库时传递】执行出库操作的单据ID（例如：调拨出库，传递调拨单ID）
        /// </summary>
        public int StockOutBillId { get; set; }
        /// <summary>
        /// 执行入库操作的单据编码
        /// </summary>
        public string StockInBillCode { get; set; }
        /// <summary>
        /// 执行入库操作的单据类型，（例如：调拨入库，传递 BillIdentity.TransferOrder）
        /// </summary>
        public BillIdentity StockInBillType { get; set; }
        /// <summary>
        /// 【退货入库时传递】执行出库操作的单据类型，（例如：调拨出库，传递 BillIdentity.TransferOrder）
        /// </summary>
        public BillIdentity StockOutBillType { get; set; }
        /// <summary>
        /// 执行入库操作的门店ID（退货入库时，该门店亦是出库时的门店）
        /// </summary>
        public int StoreId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// 入库类型（正常入库、退货入库）
        /// </summary>
        public InStockType InStockType { get; set; }
        public List<StockInItemModel> Items { get; set; }

        public StockInModel()
        {
            this.Items = new List<StockInItemModel>();
        }
    }

    /// <summary>
    /// 正常入库：传递所有字段
    /// 退货入库：传递 ProductId、Quantity、SNCode
    /// </summary>
    public class StockInItemModel
    {
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public int Quantity { get; set; }
        public decimal ContractPrice { get; set; }
        public decimal CostPrice { get; set; }
        public DateTime? ProductionDate { get; set; }
        public int ShelfLife { get; set; }
        //public DateTime CreatedOn { get; set; }
        //public int CreatedBy { get; set; }
        //public List<SNCodeModel> SNCodes { get; set; }
        public string SNCode { get; set; }

        //品类优惠总额
        public decimal CategoryPreferential { get; set; }
        //品牌优惠总额
        public decimal BrandPreferential { get; set; }

        public StockInItemModel()
        {
            //this.SNCodes = new List<SNCodeModel>();
        }

        public StockInItemModel(int productId, int quantity, string snCode)
        {
            this.ProductId = productId;
            this.Quantity = quantity;
            this.SNCode = snCode;
        }
    }
}
