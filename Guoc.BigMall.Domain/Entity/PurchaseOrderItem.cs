using Guoc.BigMall.Infrastructure.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    [Serializable]
    public class PurchaseOrderItem :CloneBase<PurchaseOrderItem>
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        /// <summary>
        /// 商品
        /// </summary>
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        /// <summary>
        /// 成本价/供应价
        /// </summary>
        public decimal CostPrice { get; set; }

        /// <summary>
        /// 预订数量/申请退货数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 实际收货数量（仓库：实收退货；门店：实收发货）
        /// </summary>
        public int ActualQuantity { get; set; }
        /// <summary>
        /// 实际发货数量（仓库：实发；门店：实退）
        /// </summary>
        //public int ActualShipQuantity { get; set; }
        /// <summary>
        /// SAP 订单号
        /// </summary>
        //public string SapOrderId { get; set; }
        /// <summary>
        /// SAP 订单行号
        /// </summary>
        public string SapRow { get; set; }
        /// <summary>
        /// Sap 商品编码
        /// </summary>
       // public string SapProductCode { get; set; }

        /// <summary>
        /// 入库批次号
        /// </summary>
        public long BatchNo { get; set; }
        /// <summary>
        /// 串码
        /// </summary>
        public string SNCodes { get; set; }
        public int SNQuantity { get; set; }
        public bool IsSnCode { get; set; }
        public string Unit { get; set; }
        //public PurchaseOrderItem Clone()
        //{
        //    MemoryStream stream = new MemoryStream();
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    formatter.Serialize(stream, this);
        //    stream.Position = 0;
        //    return formatter.Deserialize(stream) as PurchaseOrderItem;
        //}
    }
}
