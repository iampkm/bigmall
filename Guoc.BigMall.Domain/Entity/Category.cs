using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class Category : BaseEntity
    {
        public Category() {
            this.Level = 1;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; }
        public int Level { get; set; }
       // public int Status { get; set; }
        public string FullName { get; set; }
       // public string Description { get; set; }
    }
}
