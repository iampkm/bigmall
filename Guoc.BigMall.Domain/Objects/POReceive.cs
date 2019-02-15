using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Objects
{
    public class POReceive
    {
        /// <summary>
        /// SAP采购订单号
        /// </summary>
        public string SapPOCode { get; set; }

        /// <summary>
        /// 商玛特采购订单号
        /// </summary>
        public string POCode { get; set; }

        /// <summary>
        /// 商玛特收货单号（入库单号）
        /// </summary>
        public string InStockCode { get; set; }

        /// <summary>
        /// 商玛特收货单日期
        /// </summary>
        public DateTime InStockDate { get; set; }

        /// <summary>
        /// 抬头文本
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 采购订单号
        /// </summary>
        public List<POReceiveItem> Items { get; set; }


        /// <summary>
        /// 【接口返回】SAP商品凭证（SAP收货单号）
        /// </summary>
        public string SapInStockCode { get; set; }
    }

    public class POReceiveItem
    {
        /// <summary>
        /// SAP采购订单行项目号
        /// </summary>
        public string SapPORow { get; set; }

        /// <summary>
        /// 商玛特收货凭证行项目号（入库单id）
        /// </summary>
        public int InStockRow { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 商品串码
        /// </summary>
        public string SNCodes { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        public string StoreCode { get; set; }

        /// <summary>
        /// 地点
        /// </summary>
        public string StockLocation { get; set; }


        /// <summary>
        /// 【接口返回】SAP行项目（SAP收货单行号）
        /// </summary>
        public string SapInStockRow { get; set; }

        public POReceiveItem()
        {
            this.StockLocation = "P010";
        }
    }
}
