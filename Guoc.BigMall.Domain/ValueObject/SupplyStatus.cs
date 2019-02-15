using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.ValueObject
{
   public enum SupplyStatus
    {
        [Description("初始")]
        Create = 1,
        [Description("供货中")]
        Supplying = 2,
        [Description("不供货")]
        StopSupply = 3
    }
}
