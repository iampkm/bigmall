using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure;
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
            //var sql = string.Format(@"
            //    WITH parent AS
            //    (
            //     SELECT * FROM Category WHERE ParentCode {0}
            //     UNION ALL
            //     SELECT c.* FROM Category c,parent p WHERE c.ParentCode=p.Code
            //    )
            //    SELECT * FROM parent", parentCode == null ? "IS NULL" : "= @ParentCode");

            var sql = "select * from Category ";
            if (!string.IsNullOrEmpty(parentCode))
            {
                sql += string.Format(" where Code like '{0}%'", parentCode);
            }
            var categories = _db.DataBase.Query<Category>(sql, null).ToList();
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
                    Name = category.Name
                };
                this.CreateTree(categories, node);
                childrens.Add(node);
            }

            return childrens.Cast<CategoryTreeNode>().ToList();
        }


        public string Create(string parentCode, string name)
        {
            // 分类 2位为一节：  10  0101  0102  010101

            if (_db.Table<Category>().Exists(n => n.Name == name))
            {
                throw new Exception(string.Format("分类{0}已经存在", name));
            }
            Category model = new Category();
            model.ParentCode = parentCode;
            model.Name = name;
            if (!string.IsNullOrEmpty(parentCode))
            {
                model.Level = parentCode.Length / 2 + 1;
            }
            List<Category> nodes = new List<Category>();
            if (model.Level == 1)
            {
                model.FullName = model.Name;
                nodes = _db.Table<Category>().Where(n => n.Level == 1).OrderBy(n => n.Code).ToList();
            }
            else
            {
                var parentModel = _db.Table<Category>().FirstOrDefault(n => n.Code == parentCode);
                model.FullName = string.Format("{0}-{1}", parentModel.FullName, model.Name);
                var parentIdValue = parentCode + "%";
                var nodeLevel = model.Level;
                nodes = _db.Table<Category>().Where(n => n.Code.Like(parentIdValue) && n.Level == nodeLevel).ToList();
            }

            if (model.Level == 1)
            {
                if (nodes.Count == 0)
                {
                    model.Code = "10";
                }
                else
                {
                    var maxNumber = nodes.Max(n => Convert.ToInt32(n.Code)) + 1;
                    var startNumber = 9;
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        var currentId = int.Parse(node.Code);
                        // ID 从11 开始，
                        if (startNumber + i + 1 != currentId)
                        {
                            //如果有空缺，取空缺Number
                            maxNumber = startNumber + i + 1;
                            break;
                        }
                    }
                    model.Code = maxNumber.ToString();
                }

            }
            else
            {
                if (nodes.Count == 0)
                {
                    model.Code = parentCode + "01";
                }
                else
                {
                    // 默认最大值加1
                    var maxNumber = nodes.Max(n => Convert.ToInt32(n.Code.Substring(n.Code.Length - 2, 2))) + 1;
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        //取最后两位
                        var currentId = int.Parse(node.Code.Substring(node.Code.Length - 2, 2));
                        if (i + 1 != currentId)
                        {
                            //如果有空缺，取空缺Number
                            maxNumber = i + 1;
                            break;
                        }
                    }
                    model.Code = parentCode + maxNumber.ToString().PadLeft(2, '0');
                }
            }

            _db.Insert(model);
            _db.SaveChange();
            return model.Code;
        }

        public void Update(string code, string name)
        {
            if (_db.Table<Category>().Exists(n => n.Name == name && n.Code != code))
            {
                throw new Exception("名称重复!");
            }
            var model = _db.Table<Category>().FirstOrDefault(n => n.Code == code);
            model.FullName = model.FullName.Replace(model.Name, name);
            model.Name = name;
            _db.Update(model);
            _db.SaveChange();
        }

        public void Delete(string code)
        { 
            var entity = _db.Table<Category>().FirstOrDefault(n => n.Code == code);
            if (entity==null)
            {
                throw new FriendlyException(string.Format("品类[{0}] 不存在!",code));
            }
            //var hasProduct = _db.Table<Product>().Exists(n => n.CategoryId==entity.Id);
            //if (hasProduct) throw new Exception("该品类及子类下有商品，不能删除");

            _db.Delete<Category>(n => n.Code.Like(code + "%"));
            _db.SaveChange();
        }
    }
}
