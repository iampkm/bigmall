using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Objects
{
    public class TransferStockIn
    {
        /// <summary>
        /// SAP交货单号（SAP的出库单号）
        /// </summary>
        public string SapStockOutCode { get; set; }

        /// <summary>
        /// 商玛特收货凭证号（入库单号）
        /// </summary>
        public string StockInCode { get; set; }

        /// <summary>
        /// 商玛特凭证日期（入库日期）
        /// </summary>
        public DateTime StockInDate { get; set; }

        /// <summary>
        /// 商玛特调拨单号
        /// </summary>
        public string TransferCode { get; set; }

        /// <summary>
        /// 调拨明细
        /// </summary>
        public List<TransferStockInItem> Items { get; set; }



        /// <summary>
        /// 【接口返回】SAP商品凭证（SAP的入库单号）
        /// </summary>
        public string SapStockInCode { get; set; }
    }

    public class TransferStockInItem
    {
        /// <summary>
        /// SAP交货单行项目号（SAP出库单行号）
        /// </summary>
        public string SapStockOutRow { get; set; }

        /// <summary>
        /// 商玛特收货凭证行项目号（入库单行项目号）
        /// </summary>
        public int StockInRow { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 商品串码,以逗号分割
        /// </summary>
        public string SNCodes { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 调入地点
        /// </summary>
        public string StockLocation { get; set; }

        /// <summary>
        /// 调入库位
        /// </summary>
        public string ToStoreCode { get; set; }



        /// <summary>
        /// 【接口返回】SAP商品凭证行项目（SAP入库单行号）
        /// </summary>
        public string SapStockInRow { get; set; }

        public TransferStockInItem()
        {
            this.StockLocation = "P010";
        }
    }
}
