using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class CategoryTreeNode : TreeNode
    {
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public int Level { get; set; }
    }
}
