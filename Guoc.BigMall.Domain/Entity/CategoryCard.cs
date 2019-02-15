using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class CategoryCard : BaseEntity
    {
        public int StoreId { get; set; }
        public string CardNumber { get; set; }
    }
}
