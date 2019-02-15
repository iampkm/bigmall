using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Application;
using System.Dynamic;
namespace Guoc.BigMall.Application.Facade.Service
{
    public class MenuFacade : IMenuFacade
    {
        IDBContext _db;
        public MenuFacade(IDBContext dbContext)
        {
            this._db = dbContext;

        }


         public void Create(MenuModel model)
         {

             if (_db.Table<Menu>().Exists(m => m.Name == model.Name))
             {
                 throw new FriendlyException(string.Format("{0}已经存在", model.Name));
             }
             if (model.UrlType == (int)MenuUrlType.MenuLink && model.Url == "#")
             {

             }
             else if ( _db.Table<Menu>().Exists(m => m.Url == model.Url ))
             {
                 throw new FriendlyException(string.Format("已经存在“{0}”链接", model.Url));
             }
             Menu menu = new Menu(model.Name, model.Url, model.Icon, model.ParentId, model.DisplayOrder,model.UrlType);
             _db.Insert(menu);
             _db.SaveChange();
         }


         public void Edit(MenuModel model)
         {
            
             if (_db.Table<Menu>().Exists(n=>n.Name==model.Name&&n.Id!=model.Id))
             {
                 throw new FriendlyException(string.Format("{0}已经存在", model.Name));
             }
             if (model.UrlType == (int)MenuUrlType.MenuLink && model.Url == "#")
             {

             }
             else if ( _db.Table<Menu>().Exists(n => n.Url == model.Url &&  n.Id != model.Id))
             {
                 throw new FriendlyException(string.Format("已经存在“{0}”链接", model.Url));
             }
             
             //}
             Menu menu = new Menu(model.Name, model.Url, model.Icon, model.ParentId, model.DisplayOrder, model.UrlType, model.Id);
             _db.Update(menu);
             _db.SaveChange();
         }
        
        public void Delete(string ids)
        {

            if (string.IsNullOrEmpty(ids))
            {
                throw new FriendlyException("id 参数为空");
            }
            var arrIds = ids.Split(',').ToIntArray();
            foreach (var id in arrIds)
            {

                _db.Delete<Menu>(n=>n.Id == id);
               // _db.Delete<Menu>()

                // _db.Delete<Menu>()

            }
            // _db.Delete<Menu>(arrIds);
            _db.SaveChange();

            // _menuService.Delete(ids);

            
         }
         public IEnumerable<Menu> GetList(Pager page, string name)
         {
             IEnumerable<Menu> rows;
             dynamic param = new ExpandoObject();
             string where = "";
             if (!string.IsNullOrEmpty(name))
             {
                 where += "and Name like @Name ";
                 param.Name = string.Format("%{0}%", name);
             }
             StringBuilder sb = new StringBuilder();
             sb.Append(" SELECT * FROM ( ");
             sb.Append(string.Format(@"SELECT *,ROW_NUMBER( )  OVER( ORDER BY id ) rows FROM dbo.Menu    WHERE 1=1 {0}", where));


             sb.Append(string.Format(" ) as T where rows between {0} and {1}", (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize));
            rows = this._db.DataBase.Query<Menu>(sb.ToString(), param);
            //超级管理员superman不显示
            string sqlCount = @"Select count(*) from Menu   where 1=1 {0}";
            sqlCount = string.Format(sqlCount, where);
            page.Total = this._db.DataBase.ExecuteScalar<int>(sqlCount, param);
            return rows;
        }


        public IList<Menu> LoadMenuTree()
        {
            // 根据当前角色对应权限菜单
            IEnumerable<Menu> rows = _db.DataBase.Query<Menu>("select * from menu", null);
            // 转化为树形结构
            List<Menu> oneMenus = rows.Where(n => n.ParentId == 0).OrderBy(n => n.DisplayOrder).ToList();
            List<Menu> tree = new List<Menu>();
            foreach (var item in oneMenus)
            {
                tree.Add(item);
                LoadChildren(tree, item, rows);
            }
            return tree;
        }
        public IList<Menu> LoadMenuTree(int roleId)
        {
            // 根据当前角色对应权限菜单
            IEnumerable<Menu> rows = LoadMenu(roleId);
            // 转化为树形结构
            List<Menu> oneMenus = rows.Where(n => n.ParentId == 0).OrderBy(n => n.DisplayOrder).ToList();
            List<Menu> tree = new List<Menu>();
            foreach (var item in oneMenus)
            {
                tree.Add(item);
                LoadChildren(tree, item, rows);
            }
            return tree;
        }

        private void LoadChildren(List<Menu> tree, Menu parent, IEnumerable<Menu> data)
        {
            var childrenMenus = data.Where(n => n.ParentId == parent.Id).OrderBy(n => n.DisplayOrder).ToList();
            foreach (var child in childrenMenus)
            {
                tree.Add(child);
                LoadChildren(tree, child, data);
            }
        }


        public IEnumerable<Menu> LoadMenu(int roleId)
        {
            string sql = @"select m.* from menu m inner JOIN rolemenu r on m.Id =  r.MenuId
where r.RoleId = @RoleId";
            var rows = _db.DataBase.Query<Menu>(sql, new { RoleId = roleId });
            return rows;
        }

        public IList<TreeNode> LoadMenuTreeNode()
        {
            // 根据当前角色对应权限菜单
            List<Menu> rows = _db.DataBase.Query<Menu>("select * from menu", null).ToList();
            // 转化为树形结构
            //List<Menu> oneMenus = rows.Where(n => n.ParentId == 0).OrderBy(n => n.DisplayOrder).ToList();

            var treeNodes = CreateMenuNode(rows);

            return treeNodes;
        }

        private static List<TreeNode> CreateMenuNode(List<Menu> menus, TreeNode parentNode = null)
        {
            int parentId = 0;
            List<TreeNode> nodeContainer;
            if (parentNode == null)
            {
                nodeContainer = new List<TreeNode>();
            }
            else
            {
                parentId = int.Parse(parentNode.Key);
                nodeContainer = parentNode.Children;
            }

            var childMenus = menus.Where(m => m.ParentId == parentId).OrderBy(m => m.DisplayOrder).ToList();

            foreach (var menu in childMenus)
            {
                TreeNode treeNode = new TreeNode()
                {
                    Key = menu.Id.ToString(),
                    Label = menu.Name,
                };
                nodeContainer.Add(treeNode);
                CreateMenuNode(menus, treeNode);

                //foreach (var item in oneMenus)
                //{
                //    treeNode.Id = item.Id;
                //    treeNode.Label = item.Name;
                //    treeNodeList.Add(treeNode);
                //    LoadChildrenNote(treeNodeList, item, rows);
                //}
            }
            return nodeContainer;
        }

    }
}
