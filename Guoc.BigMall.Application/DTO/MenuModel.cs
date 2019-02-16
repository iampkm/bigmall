using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class MenuModel
    {
        public Dictionary<int, string> ButtonRole = new Dictionary<int, string>();
        
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int DisplayOrder { get; set; }
        public int UrlType { get; set; }

       

    }
  
}
