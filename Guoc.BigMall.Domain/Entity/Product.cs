using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class Product : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Spec { get; set; }
        public string Unit { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string SecondSpec { get; set; }
        public bool HasSNCode { get; set; }
    }
}
