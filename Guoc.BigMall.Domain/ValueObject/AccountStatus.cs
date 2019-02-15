using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
    public enum AccountStatus
    {
        [Description("激活")]
        /// <summary>
        /// 激活
        /// </summary>
        Active = 1,
        [Description("删除")]
        /// <summary>
        /// 删除
        /// </summary>
        Deleted = 2,
        [Description("禁用")]
        /// <summary>
        /// 禁用
        /// </summary>
        Disabled = 3
    }
}
