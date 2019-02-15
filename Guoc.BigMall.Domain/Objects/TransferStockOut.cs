using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Objects
{
    public class TransferStockOut
    {
        /// <summary>
        /// 出库单号
        /// </summary>
        public string StockOutCode { get; set; }

        /// <summary>
        /// 出库日期
        /// </summary>
        public DateTime StockOutDate { get; set; }

        /// <summary>
        /// 商玛特调拨单号
        /// </summary>
        public string TransferCode { get; set; }

        /// <summary>
        /// SAP调拨单号
        /// </summary>
        public string SapTransferCode { get; set; }

        /// <summary>
        /// 调拨明细
        /// </summary>
        public List<TransferStockOutItem> Items { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// 【接口返回】SAP交货单号（SAP的出库单号）
        /// </summary>
        public string SapStockOutCode { get; set; }
    }

    public class TransferStockOutItem
    {
        /// <summary>
        /// 出库单行项目号
        /// </summary>
        public int StockOutRow { get; set; }

        /// <summary>
        /// SAP调拨单行项目号
        /// </summary>
        public string SapTransferRow { get; set; }

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
        /// 调出方地点
        /// </summary>
        public string StockLocation { get; set; }

        /// <summary>
        /// 调出方库位
        /// </summary>
        public string FromStoreCode { get; set; }



        /// <summary>
        /// 【接口返回】SAP交货单行项目号（SAP出库单行号）
        /// </summary>
        public string SapStockOutRow { get; set; }

        public TransferStockOutItem()
        {
            this.StockLocation = "P010";
        }
    }
}
