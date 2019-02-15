using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface ISupplierFacade
    {
        IEnumerable<SupplierDto> GetPageList(Pager page, string name, string code);

        SupplierDto GetProductSupplier(string productId);
    }
}
