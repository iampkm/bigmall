using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext;
using Guoc.BigMall.Infrastructure.Extension;
namespace Guoc.BigMall.Domain.Entity
{
    public class Role : BaseEntity
    {
        public Role(string name, string description, int id = 0)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Items = new List<RoleMenu>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual List<RoleMenu> Items { get; private set; }

        public void AssignMenus(string menuIds)
        {
            var arr = menuIds.Split(',').ToIntArray();
            this.Items.Clear();
            foreach (var menuId in arr)
            {
                RoleMenu entity = new RoleMenu()
                {
                    MenuId = menuId,
                    RoleId = this.Id
                };
                this.Items.Add(entity);
            }
        }

        public void LoadMenus(List<RoleMenu> items)
        {
            this.Items = items;
        }

    }
}
