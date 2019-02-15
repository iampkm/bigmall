using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Infrastructure.Extension;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Facade
{
    public class BrandFacade : IBrandFacade
    {
        IDBContext _db;
        public BrandFacade(IDBContext dbContext)
        {
            this._db = dbContext;

        }

        public IEnumerable<Brand> GetList(Pager page, BrandSearch search)
        {
            IEnumerable<Brand> rows;
            dynamic param = new ExpandoObject();
            string where = "";
            if (search.Code.NotNullOrEmpty())
            {
                where += " and  Code like @Code";
                param.code = string.Format("%{0}%", search.Code);
            }

            if (search.Name.NotNullOrEmpty())
            {
                where += " and Name like @Name";
                param.name = string.Format("%{0}%", search.Name);
            }

            if (search.Ids.NotNullOrEmpty())
            {
                where += " and Id in @Ids";
                param.Ids = search.Ids.Split(',').ToIntArray();
            }

            StringBuilder str = new StringBuilder();
            str.AppendFormat(" SELECT *, ROW_NUMBER() OVER (ORDER BY id) AS rows  FROM dbo.Brand WHERE 1=1 {0} ", where);
            if (page.IsPaging)
                str.AppendFormat("ORDER BY Id ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", (page.PageIndex - 1) * page.PageSize, page.PageSize);

            rows = _db.DataBase.Query<Brand>(str.ToString(), param);
            var sqlcount = string.Format(" SELECT count(1) FROM dbo.Brand WHERE 1=1 {0} ", where);
            page.Total = this._db.DataBase.ExecuteScalar<int>(sqlcount, param);

            return rows;
        }
    }
}
