using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Facade
{
    public class CategoryFacade : ICategoryFacade
    {
        IDBContext _db;
        public CategoryFacade(IDBContext dbContext)
        {
            _db = dbContext;
        }

        public List<CategoryTreeNode> GetCategoryTree(string parentCode = null)
        {
            var sql = string.Format(@"
                WITH parent AS
                (
	                SELECT * FROM Category WHERE ParentCode {0}
	                UNION ALL
	                SELECT c.* FROM Category c,parent p WHERE c.ParentCode=p.Code
                )
                SELECT * FROM parent", parentCode == null ? "IS NULL" : "= @ParentCode");
            var categories = _db.DataBase.Query<Category>(sql, new { ParentCode = parentCode }).ToList();
            return this.CreateTree(categories, null);
        }

        private List<CategoryTreeNode> CreateTree(List<Category> categories, CategoryTreeNode parentNode)
        {
            string parentCode;
            List<TreeNode> childrens;
            if (parentNode == null)
            {
                parentCode = null;
                childrens = new List<TreeNode>();
            }
            else
            {
                parentCode = parentNode.Code;
                childrens = parentNode.Children;
            }

            var data = categories.Where(c => c.ParentCode == parentCode);

            foreach (var category in data)
            {
                var node = new CategoryTreeNode()
                {
                    Key = category.Id.ToString(),
                    Label = string.Format("[{0}] {1}", category.Code, category.Name),
                    Code = category.Code,
                    Level = category.Level,
                    ParentCode = category.ParentCode,
                };
                this.CreateTree(categories, node);
                childrens.Add(node);
            }

            return childrens.Cast<CategoryTreeNode>().ToList();
        }
    }
}
