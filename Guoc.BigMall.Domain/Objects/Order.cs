using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Objects
{
    public class Order
    {
        /// <summary>
        /// 商玛特采购/调拨单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 抬头文本
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 采购组（调拨单-SM3，采购-SMT传输值）
        /// </summary>
        public string PurchaseGroup { get; set; }

        /// <summary>
        /// 凭证日期（制单日期）
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 商玛特凭证类型（ZP02- 采购退货单、ZUBA-调拨单、ZSM1-采购换货单、ZP01-采购订单）
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// 供应商帐户号
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }


        public List<OrderItem> Items { get; set; }



        /// <summary>
        /// 【接口返回】SAP单据号
        /// </summary>
        public string SapCode { get; set; }

        public Order()
        {
            this.CompanyCode = "6017";
        }
    }

    public class OrderItem
    {
        /// <summary>
        /// 商玛特采购订单行号
        /// </summary>
        public int ItemRow { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 商品类别
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 商品单价（1、当订单类型=ZP01、ZP02、ZSM1时，传含税单价；2、当订单类型=ZUBA时，该字段传0；）
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 调拨发出方仓库或门店
        /// </summary>
        public string FromStoreCode { get; set; }

        /// <summary>
        /// 调入方仓库或门店
        /// </summary>
        public string ToStoreCode { get; set; }

        /// <summary>
        /// 地点
        /// </summary>
        public string StockLocation { get; set; }

        /// <summary>
        /// 计划到货日期
        /// </summary>
        public DateTime? DeliveryDate { get; set; }



        /// <summary>
        /// 【接口返回】SAP行项目号
        /// </summary>
        public string SapRow { get; set; }

        public OrderItem()
        {
            this.StockLocation = "P010";
        }
    }

    public enum OrderType
    {
        /// <summary>
        /// 采购订单
        /// </summary>
        ZP01 = 0,
        /// <summary>
        /// 采购退单
        /// </summary>
        ZP02 = 1,
        /// <summary>
        /// 调拨单
        /// </summary>
        ZUBA = 2,
        /// <summary>
        /// 采购换货单
        /// </summary>
        ZSM1 = 3,
    }
}
