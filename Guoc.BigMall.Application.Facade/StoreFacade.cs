using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Infrastructure.Extension;
using System.Dynamic;
using Guoc.BigMall.Application.ViewObject;

namespace Guoc.BigMall.Application.Facade
{
    public class StoreFacade : IStoreFacade
    {
        IDBContext _db;
        public StoreFacade(IDBContext dbContext)
        {
            _db = dbContext;
        }

        public void Create(StoreModel model)
        {
            var entity = model.MapTo<Store>();
            if (_db.Table<Store>().Exists(n => n.Name == entity.Name))
            {
                throw new Exception("名称重复!");
            }
            // 生成门店编码           
            var firstAreaId = entity.AreaId.Substring(0, 2);
            var result = _db.Table<Store>().Where(n => n.AreaId.Like(firstAreaId + "%")).ToList();
            var maxAreaIdNumber = 0;
            if (result.Count() > 0)
            {
                maxAreaIdNumber = result.Max(n => n.Number);
            }
            entity.GenerateNewCode(maxAreaIdNumber);
            _db.Insert(entity);
            _db.SaveChange();
        }

        public void Edit(StoreModel model)
        {
            Store entity = _db.Table<Store>().FirstOrDefault(s => s.Id == model.Id);
            var oldAreaId = entity.AreaId;
            model.MapTo(entity);
            Update(entity, oldAreaId);
            _db.SaveChange();
        }

        private void Update(Store model, string oldAreaId)
        {
            if (_db.Table<Store>().Exists(n => n.Name == model.Name && n.Id != model.Id))
            {
                throw new Exception("名称重复!");
            }
            if (model.AreaId != oldAreaId)
            {
                //如果区域发生改变，重新生成新的 code
                var firstAreaId = model.AreaId.Substring(0, 2);
                var result = _db.Table<Store>().Where(n => n.AreaId.Like(firstAreaId + "%")).ToList();
                var maxAreaIdNumber = 0;
                if (result.Count() > 0)
                {
                    maxAreaIdNumber = result.Max(n => n.Number);
                }
                model.GenerateNewCode(maxAreaIdNumber);
            }
            _db.Update(model);
        }

        public void Delete(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                throw new Exception("id 参数为空");
            }
            var arrIds = ids.Split(',').ToIntArray();
            _db.Delete<Store>(s => s.Id.In(arrIds));
            _db.SaveChange();
            //删除权限
        }

        //public void EditLicense(int id, string license)
        //{
        //    if (string.IsNullOrEmpty(license)) throw new Exception("门店授权码不能为空");
        //    var model = _db.Table<Store>().FirstOrDefault(s => s.Id == id);
        //    model.LicenseCode = license;
        //    model.EncryptionLicense();
        //    _db.Update(model);
        //    _db.SaveChange();

        //}

        public List<StoreTreeNode> LoadStore(string canViewStores, string name = "", string code = "", string groupId = "")
        {
            // 查询一级区域
            var areaRows = _db.Table<Area>().Where(n => n.Level == 1).ToList();
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrEmpty(canViewStores))
            {
                where += "and t0.Id in @CanViewStore ";
                param.CanViewStore = canViewStores.Split(',').ToIntArray();
            }
            if (!string.IsNullOrEmpty(name))
            {
                where += "and t0.Name like @StoreName ";
                param.StoreName = string.Format("%{0}%", name);
            }
            if (!string.IsNullOrEmpty(code))
            {
                where += "and t0.code like @StoreCode ";
                param.StoreCode = string.Format("%{0}%", code);
            }
            if (!string.IsNullOrEmpty(groupId))
            {
                where += "and g.Id like @GroupId ";
                param.GroupId = string.Format("{0}%", groupId);
            }
            //            string sql = @"select t0.Id,t0.Code,t0.Name,t0.AreaId  from Store  t0 
            //left join StoreGroupMapping m on t0.Id = m.StoreId
            //left join StoreGroup g on g.Id = m.StoreGroupId where 1=1 {0}";

            string sql = @"select t0.Id,t0.Code,t0.Name,t0.AreaId  from Store  t0  where 1=1 {0}";
            sql = string.Format(sql, where);
            List<Store> stores = _db.DataBase.Query<Store>(sql, param);
            // 组装ztree树形结构
            List<StoreTreeNode> treeNodes = new List<StoreTreeNode>();
            foreach (var area in areaRows)
            {
                // 找当前区域门店
                var areaID = area.Id.Substring(0, 2);
                var areaStores = stores.Where(n => n.AreaId.IndexOf(areaID) == 0);
                //只加载有门店的区域
                if (!areaStores.Any()) { continue; }

                var areaNode = new StoreTreeNode()
                {
                    Key = "area_" + area.Id,//避免在Tree上与门店Id冲突
                    Label = area.Name,
                    Code = area.FullName,
                    Name = area.Name
                };

                foreach (var store in areaStores)
                {
                    var storeNode = new StoreTreeNode()
                    {
                        Key = store.Id.ToString(),
                        Code = store.Code,
                        Name = store.Name,
                        Label = string.Format("[{0}]{1}", store.Code, store.Name),
                        IsStore = true,
                    };
                    areaNode.Children.Add(storeNode);
                }

                if ((string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name)) || areaNode.Children.Count > 0)
                {
                    treeNodes.Add(areaNode);
                }
            }
            return treeNodes;
        }

        public StoreDto LoadStore(int storeId)
        {
            var store = _db.Table<Store>().FirstOrDefault(x => x.Id == storeId);
            if (store == null) return null;
            var dto = store.MapTo<StoreDto>();
            return dto;
        }

        public StoreModel GetById(int storeId)
        {
            var store = _db.Table<Store>().FirstOrDefault(x => x.Id == storeId);
            if (store == null) return null;
            var model = store.MapTo<StoreModel>();
            return model;
        }

        public List<string> GetStoreTags()
        {
            var sql = "select Tag from store group by Tag";
            var rows = this._db.DataBase.Query<string>(sql, null).ToList();
            return rows;
        }

        public List<StoreDto> GetPageList(Pager page, StoreSearch searchArgs)
        {
            List<StoreDto> rows;
            dynamic param = new ExpandoObject();
            string where = "";

            if (!string.IsNullOrEmpty(searchArgs.Code))
            {
                where += "AND Code LIKE @Code ";
                param.Code = "%{0}%".FormatWith(searchArgs.Code);
            }

            if (!string.IsNullOrEmpty(searchArgs.Name))
            {
                where += "AND Name LIKE @Name ";
                param.Name = "%{0}%".FormatWith(searchArgs.Name);
            }
            if (!string.IsNullOrEmpty(searchArgs.Tag))
            {
                where += "AND Tag LIKE @Tag ";
                param.Tag = "%{0}%".FormatWith(searchArgs.Tag);
            }

            if (searchArgs.StoreIds.NotNullOrEmpty())
            {
                where += "AND Id IN @StoreIds ";
                param.StoreIds = searchArgs.StoreIds.Split(',');
            }

            var fields = "Id, Code, Name, Address, Phone, CreatedOn, CreatedBy,Tag";
            var dataSql = "SELECT {0} FROM Store WHERE 1=1 " + where;
            var pageSql = dataSql.FormatWith(fields) + " ORDER BY Id OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY".FormatWith((page.PageIndex - 1) * page.PageSize, page.PageSize);
            var countSql = dataSql.FormatWith("COUNT(1)");
            rows = this._db.DataBase.Query<StoreDto>(pageSql, param) as List<StoreDto>;
            page.Total = this._db.DataBase.ExecuteScalar<int>(countSql, param);
            return rows;
        }
    }
}
