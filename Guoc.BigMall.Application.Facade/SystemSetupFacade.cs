using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
using System.Dynamic;
using Guoc.BigMall.Application.DTO;
namespace Guoc.BigMall.Application.Facade
{
    public class SystemSetupFacade : ISystemSetupFacade
    {
         IDBContext _db;
         public SystemSetupFacade(IDBContext dbContext)
        {
            this._db = dbContext;

        }

         public void Create(SystemSetupModel model)
        {
        
            if (_db.Table<SystemSetup>().Exists(n => n.Name == model.Name.ToLower()))
            {
                throw new FriendlyException(string.Format("字段【{0}】重复",model.Name.ToLower()));
            }
            if (_db.Table<SystemSetup>().Exists(n => n.Description == model.Description))
            {
                throw new FriendlyException(string.Format("描述【{0}】重复！", model.Description));
            }
            var entity = new SystemSetup();
            model.MapTo(entity);
            _db.Insert<SystemSetup>(entity);
            _db.SaveChange();
        }

        public void Edit(SystemSetupModel model)
        {

            if (_db.Table<SystemSetup>().Exists(n => n.Name == model.Name&&n.Id!=model.Id))
            {
                throw new FriendlyException(string.Format("字段【{0}】重复", model.Name.ToLower()));
            }
            if (_db.Table<SystemSetup>().Exists(n => n.Description == model.Description && n.Id != model.Id))
            {
                throw new FriendlyException(string.Format("描述【{0}】重复！", model.Description));
            }
            var entity = new SystemSetup();
            model.MapTo(entity);
            _db.Update<SystemSetup>(entity);
            _db.SaveChange(); 
        }

        public void Delete(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
            {
                throw new FriendlyException("删除参数为空！");
            }
            var arrids = ids.Split(',').ToIntArray();
            foreach (var id in arrids)
            {
                _db.Delete<SystemSetup>(n => n.Id == id);
                _db.SaveChange();
                
            }

        }

        public IEnumerable<SystemSetup> GetList(Pager page, string name, string value)
        {
            IEnumerable<SystemSetup> rows;
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrWhiteSpace(name))
            {
                where += " and name like @Name";
                param.Name = string.Format("%{0}%", name);
            }
            if (!string.IsNullOrWhiteSpace(value))
            {
                where += " and value =@Value";
                param.Value = value;
            }
            string sqltext = @"SELECT * FROM (
                                            SELECT  ROW_NUMBER() OVER(ORDER BY ID  ) AS rows,*	 FROM  dbo.SystemSetup WHERE 1=1  {0}
                                            )t where rows between {1} and {2}" ;

            sqltext=string.Format(sqltext,where,(page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize);
            rows = _db.DataBase.Query<SystemSetup>(sqltext, param);
            string sqlcount = string.Format("   SELECT count(1) FROM  dbo.SystemSetup WHERE 1=1  {0}", where);
            page.Total = _db.DataBase.ExecuteScalar<int>(sqlcount, param);
            return rows;

        }
    }
}
