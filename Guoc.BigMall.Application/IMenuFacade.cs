using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface IMenuFacade
    {
        void Create(MenuModel model);
        void Edit(MenuModel model);

        void Delete(string ids);

        IEnumerable<Menu> GetList(Pager page, string name);

        IList<Menu> LoadMenuTree();
        IList<Menu> LoadMenuTree(int roleId);

        IEnumerable<Menu> LoadMenu(int roleId);

        IList<TreeNode> LoadMenuTreeNode();
    }
}
