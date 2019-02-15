using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum PurchaseOrderBillType
    {
        /// <summary>
        /// 门店采购单
        /// </summary>
        [Description("门店采购单")]
        StoreOrder = 1,
        /// <summary>
        /// 总仓采购单
        /// </summary>
        [Description("总仓采购单")]
        StockOrder = 2,
        

    }
}
