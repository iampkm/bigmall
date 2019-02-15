using Guoc.BigMall.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
   public interface IAreaFacade
    {
        List<CategoryTreeNode> GetAreaTree(string parentCode = null);
    }
}
