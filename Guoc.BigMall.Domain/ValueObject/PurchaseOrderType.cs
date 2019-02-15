using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum PurchaseOrderType
    {
        /// <summary>
        /// 采购单
        /// </summary>
        [Description("采购单")]
        PurchaseOrder = 1,
        /// <summary>
        /// 采购退单
        /// </summary>
        [Description("采购退单")]
        PurchaseReturn = 2,

        /// <summary>
        /// 采购换机单
        /// </summary>
        [Description("采购换机单")]
        PurchaseChange = 3,
    }
}
