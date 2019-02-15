using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface ISystemSetupFacade
    {
       void Create(SystemSetupModel model);
       void Edit(SystemSetupModel model);

        void Delete(string ids);

        IEnumerable<SystemSetup> GetList(Pager page, string name,string value);
    }
}
