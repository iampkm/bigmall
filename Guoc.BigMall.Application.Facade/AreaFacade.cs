using Dapper.DBContext;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Facade
{
    public class AreaFacade:IAreaFacade
    {
        IDBContext _db;
        /// <summary>
        /// 000000
        /// </summary>
        string _RootId = "000000";
        public AreaFacade(IDBContext dbContext)
        {
            _db = dbContext;
        }
       
        public List<CategoryTreeNode> GetAreaTree(string parentCode = null)
        {
           
            var sql = "select * from Area ";
            parentCode = string.IsNullOrEmpty(parentCode) ? _RootId : parentCode;
            if (parentCode != _RootId)
            {
                parentCode = parentCode.TrimEnd('0');
                sql += string.Format("where Id like '{0}%'", parentCode);
            }
            var areas = _db.DataBase.Query<Area>(sql, null).ToList();
            return this.CreateTree(areas, null);
        }

        private List<CategoryTreeNode> CreateTree(List<Area> areas, CategoryTreeNode parentNode)
        {
            string parentCode;
            List<TreeNode> childrens;
            if (parentNode == null)
            {
                parentCode = _RootId;
                childrens = new List<TreeNode>();
            }
            else
            {
                parentCode = parentNode.Code;
                childrens = parentNode.Children;
            }

            var data = areas.Where(c => c.ParentId == parentCode);

            foreach (var item in data)
            {
                var node = new CategoryTreeNode()
                {
                    Key = item.Id,
                    Label = string.Format("[{0}] {1}", item.Id, item.Name),
                    Code = item.Id,
                    Level = item.Level,
                    ParentCode = item.ParentId,
                };
                this.CreateTree(areas, node);
                childrens.Add(node);
            }

            return childrens.Cast<CategoryTreeNode>().ToList();
        }
    }
}
