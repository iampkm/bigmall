using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Search
{
    public class SearchPurchaseOrder
    {
        public SearchPurchaseOrder()
        {
            this.ShowInlogistics = false;
        }
        public string Code { get; set; }

        public string SupplierId { get; set; }

        public string StoreId { get; set; }

        public string ProductCodeOrBarCode { get; set; }

        /// <summary>
        /// 逗号分隔数字
        /// </summary>
        public string Status { get; set; }

        public string OrderType { get; set; }
        public string Time { get; set; }


        public string NoSupplierCode { get; set; }

        public string BrandId { get; set; }

        /// <summary>
        /// 单据类型： 普通1，存量2，零售 3，多个逗号分隔
        /// </summary>
        public string BillType { get; set; }

        /// <summary>
        ///  是否在物流中显示
        /// </summary>
        public bool ShowInlogistics { get; set; }
        /// <summary>
        /// 配送方式
        /// </summary>
        public int Shipping { get; set; }
        /// <summary>
        /// SAP 单号
        /// </summary>
        public string SapCode { get; set; }

        /// <summary>
        ///  已支付有效订单（状态为 3，4，10 ）
        /// </summary>
        public bool Paid { get; set; }

        public string Creater { get; set; }
        public string categoryId { get; set; }

        public bool? IsPushSap { get; set; }
    }
}
