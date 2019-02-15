using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext;
using Guoc.BigMall.Domain.ValueObject;
namespace Guoc.BigMall.Domain.Entity
{
    public class Menu : BaseEntity
    {
        public Menu() { }
        public Menu(string name, string url, string icon,int parentId = 0, int displayOrder = 0,int urlType = (int)MenuUrlType.MenuLink,int id = 0)
        {   
            this.Name = name;
            this.Url = url;
            this.Icon = icon;
            this.ParentId = parentId;
            this.Id = id;
            this.DisplayOrder = displayOrder;
            this.UrlType = urlType;
    }       
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string Icon { get; private set; }
        public int ParentId { get; private set; }
        public int DisplayOrder { get; private set; }
        public int UrlType { get; private set; }

        public List<Menu> Children { get; set; }



    }
}
