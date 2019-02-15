using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class PurchaseOrderItemModel
    {
        public int ProductId { get; set; }

        /// <summary>
        /// 成本价/供应价
        /// </summary>
        public decimal CostPrice { get; set; }

        /// <summary>
        /// 预订数量/申请退货数量
        /// </summary>
        public int Quantity { get; set; }
        public string ProductCode { get; set; }

       

      


    }
}
