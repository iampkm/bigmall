using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class BillSequence : BaseEntity
    {
        public BillSequence()
        {
            this.GuidCode = Guid.NewGuid().ToString().Replace("-", "");
        }
        public string GuidCode { get; set; }
    }
}
