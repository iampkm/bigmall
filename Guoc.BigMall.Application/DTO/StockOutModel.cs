using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class StockOutModel
    {
        public int BillId { get; set; }
        public string BillCode { get; set; }
        public BillIdentity BillType { get; set; }
        public int StoreId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<StockOutItemModel> Items { get; set; }

        public StockOutModel()
        {
            this.Items = new List<StockOutItemModel>();
        }
    }

    /// <summary>
    /// 对于有串码的商品，出入库使用串码上单独的 Price ，
    /// 对于无串码的商品，出入库使用Item上统一的 Price 。
    /// </summary>
    public class StockOutItemModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// 入库传入成本价
        /// </summary>
        public decimal CostPrice { get; set; }
        /// <summary>
        /// 销售出库传入销售价
        /// </summary>
        public decimal SalePrice { get; set; }
        public List<SNCodeModel> SNCodes { get; set; }

        //品类优惠总额
        public decimal CategoryPreferential { get; set; }
        //品牌优惠总额
        public decimal BrandPreferential { get; set; }

        public StockOutItemModel()
        {
            this.SNCodes = new List<SNCodeModel>();
        }

        public StockOutItemModel(int productId, int quantity, decimal costPrice = 0, decimal salePrice = 0)
            : this()
        {
            this.ProductId = productId;
            this.Quantity = quantity;
            this.CostPrice = costPrice;
            this.SalePrice = salePrice;
        }
    }

    public class SNCodeModel
    {
        public string SNCode { get; set; }
        /// <summary>
        /// 入库传入成本价
        /// </summary>
        public decimal CostPrice { get; set; }
        /// <summary>
        /// 销售出库传入销售价
        /// </summary>
        public decimal SalePrice { get; set; }

        public SNCodeModel(string snCode, decimal costPrice = 0, decimal salePrice = 0)
        {
            this.SNCode = snCode;
            this.CostPrice = costPrice;
            this.SalePrice = salePrice;
        }
    }

    public class StoreInventoryResult
    {
        /// <summary>
        /// 出库单/入库单编码
        /// </summary>
        public string HistoryCode { get; set; }
        public int BillId { get; set; }
        public string BillCode { get; set; }
        public BillIdentity BillType { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<StoreInventoryItemResult> Items { get; set; }

        public StoreInventoryHistorySapType StoreInventoryChangeType { get; set; }

        public StoreInventoryResult()
        {
            this.Items = new List<StoreInventoryItemResult>();
        }
    }

    public class StoreInventoryItemResult
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get { return BatchNos.Sum(b => b.Quantity); } }
        public List<ChangedBatch> BatchNos { get; set; }
        public List<string> SNCodes { get; set; }

        public StoreInventoryItemResult(int productId, string productCode)
        {
            this.ProductId = productId;
            this.ProductCode = productCode;
            this.BatchNos = new List<ChangedBatch>();
            this.SNCodes = new List<string>();
        }
    }

    public class ChangedBatch
    {
        public long BatchNo { get; set; }
        public int Quantity { get; set; }

        public ChangedBatch(long batchNo, int quantity)
        {
            this.BatchNo = batchNo;
            this.Quantity = quantity;
        }
    }
}
