using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Dapper.DBContext;

namespace Guoc.BigMall.Application.Facade
{
    public class SupplierFacade : ISupplierFacade
    {

        IDBContext _db;
        public SupplierFacade(IDBContext db)
        {
            this._db = db;
        }

        public IEnumerable<SupplierDto> GetPageList(Pager page, string name, string code)
        {
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrEmpty(name))
            {
                where += "and t0.Name like @Name ";
                param.Name = string.Format("%{0}%", name.Trim());
            }
            if (!string.IsNullOrEmpty(code))
            {
                where += "and t0.Code like @Code ";
                param.Code = code.Trim() + "%";
            }
            string sqlCount = @"SELECT  count(1)  
                            FROM dbo.supplier  t0
                            where 1=1 {0}";
            sqlCount = string.Format(sqlCount, where);


            page.Total = _db.DataBase.ExecuteScalar<int>(sqlCount, param);
            if (page.Total < (page.PageIndex - 1) * page.PageSize)
            {
                page.PageIndex = 1;
            }

            string sql = @"select * from (select ROW_NUMBER() over(order by t0.id desc) as rownum, t0.Id,t0.Name,t0.Code,t0.Phone,t0.Address 
               from supplier t0 where 1=1 {0} ) as T where rownum between {1} and {2}";

            sql = string.Format(sql, where, (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize);
            var rows = _db.Table<SupplierDto>().Query(sql, param);
            return rows;
        }


        public SupplierDto GetProductSupplier(string productId)
        {
            throw new NotImplementedException();
        }
    }
}
