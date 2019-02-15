using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;

namespace Guoc.BigMall.Application
{
    public interface IStoreFacade
    {
        void Create(StoreModel model);
        void Edit(StoreModel model);
        void Delete(string ids);
        //void EditLicense(int id, string license);
        List<StoreTreeNode> LoadStore(string canViewStores, string name = "", string code = "", string groupId = "");
        StoreDto LoadStore(int storeId);
        List<StoreDto> GetPageList(Pager page, StoreSearch searchArgs);

        StoreModel GetById(int storeId);

        List<string> GetStoreTags();

        
    }
}
