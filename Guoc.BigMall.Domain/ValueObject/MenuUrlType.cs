using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Guoc.BigMall.Domain.ValueObject
{
    public enum MenuUrlType
    {
        [Description("菜单")]
        /// <summary>
        /// 
        /// </summary>
        MenuLink = 1,
        [Description("按钮")]
        ButtonLink = 2
    }
}
