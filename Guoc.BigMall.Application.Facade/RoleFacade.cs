using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.Service;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Facade
{
    public class RoleFacade : IRoleFacade
    {
        IDBContext _db;
        RoleService _service;
        public RoleFacade(IDBContext db)
        {
            this._db = db;
            _service = new RoleService(this._db);
        }

        public List<RoleDTO> QueryAll()
        {
            return _db.Table<Role>().Where(r => r.Id > 1).ToList().MapToList<RoleDTO>();
        }

        public IEnumerable<RoleDTO> GetPageList(Pager page, string name)
        {
            IEnumerable<RoleDTO> rows;
            dynamic param = new ExpandoObject();
            string where = " and  t.Id>1"; //不加载系统超管角色
            if (!string.IsNullOrEmpty(name))
            {
                where += "and t.Name like @Name ";
                param.Name = string.Format("%{0}%", name.Trim());
            }


            string sqlCount = @"SELECT  count(*)  
                            FROM dbo.Role  t
                            where 1=1 {0}";
            sqlCount = string.Format(sqlCount, where);
            // page.Total = this._query.Context.ExecuteScalar<int>(sqlCount, param);
            page.Total = _db.DataBase.ExecuteScalar<int>(sqlCount, param);
            if (page.Total < (page.PageIndex - 1) * page.PageSize)
            {
                page.PageIndex = 1;
            }
            string sql = @" SELECT* FROM (
                            SELECT *,ROW_NUMBER()     OVER(ORDER BY ID DESC) AS  ROWS	
                            FROM dbo.Role t WHERE 1=1  {0} 
                            )   t0    WHERE   ROWS BETWEEN {1} AND {2} ";

            sql = string.Format(sql, where, (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize);
            //rows = this._query.FindAll<RoleDTO>(sql, param);
            rows = _db.Table<RoleDTO>().Query(sql, param);
            return rows;
        }

        public void Delete(string ids)
        {
            _service.Delete(ids);
            _db.SaveChange();
        }

        public void Create(RoleModel model)
        {
            Role entity = new Role(model.Name, model.Description);
            entity.AssignMenus(model.MenuIds);
            _service.Create(entity); // 框架自动实现 子外键关联对象添加
            _db.Insert(entity);
            _db.SaveChange();
        }

        public void Edit(RoleModel model)
        {
            Role entity = new Role(model.Name, model.Description, model.Id);
            _service.Update(entity);
            // 权限
            entity.AssignMenus(model.MenuIds);
            if (_db.Table<RoleMenu>().Exists(n => n.RoleId == model.Id))
            {
                _db.Delete<RoleMenu>(n => n.RoleId == model.Id);
            }
            _db.Insert<RoleMenu>(entity.Items.ToArray());
            _db.SaveChange();
        }


        public bool HavePower(int roleId, string url)
        {
            var sql = "";
            //超管角色（菜单存在即可）
            if (roleId == 1)
            {
                sql = @"
                    SELECT TOP 1
                            1
                    FROM    Menu m
                    WHERE   m.ParentId != 0
                            AND Url = @Url";
                return _db.DataBase.ExecuteScalar<bool>(sql, new { RoleId = roleId, Url = url });
            }
            //普通角色（必须分配了菜单权限才行）
            sql = @"
                SELECT TOP 1
                        1
                FROM    Menu m
                        INNER JOIN RoleMenu r ON m.Id = r.MenuId
                WHERE   m.ParentId != 0
                        AND r.RoleId = @RoleId
                        AND Url = @Url";
            return _db.DataBase.ExecuteScalar<bool>(sql, new { RoleId = roleId, Url = url });
        }
    }
}
