using Guoc.BigMall.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface ICategoryFacade
    {
        List<CategoryTreeNode> GetCategoryTree(string parentCode = null);
        string Create(string parentCode, string name);
        void Update(string code, string name);
        void Delete(string code);
    }
}
