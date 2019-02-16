using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Schema;
namespace Guoc.BigMall.Domain.Entity
{
    [Table("site_item")]
  public  class SiteItem:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SiteId { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string DispalyOrder { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
