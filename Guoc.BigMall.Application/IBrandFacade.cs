using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface IBrandFacade
    {
        IEnumerable<Brand> GetList(Pager page, BrandSearch search);

        void Create(BrandModel model);
        void Update(BrandModel model);
        void Delete(string ids);
    }
}
