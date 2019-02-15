using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    /// <summary>
    /// 供应商类别
    /// </summary>
    public enum SupplierType
    {
         [Description("经销")]
        /// <summary>
        /// 经销
        /// </summary>
        SellBySelf = 1,
         [Description("代销")]
        /// <summary>
        /// 代销
        /// </summary>
        SellByProxy = 2,
         [Description("联营")]
        /// <summary>
        /// 联营
        /// </summary>
        SellJoin = 3
    }
}
