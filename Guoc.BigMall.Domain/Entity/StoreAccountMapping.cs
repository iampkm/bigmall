using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class StoreAccountMapping : BaseEntity
    {
        public int StoreId { get; set; }
        public int AccountId { get; set; }
    }
}
