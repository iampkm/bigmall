using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface IRoleFacade
    {
        List<RoleDTO> QueryAll();
        IEnumerable<RoleDTO> GetPageList(Pager page, string name);

        void Delete(string ids);
        void Create(RoleModel model);
        void Edit(RoleModel model);
        bool HavePower(int roleid, string value);
    }
}
