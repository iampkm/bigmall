using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class TreeNode
    {
        public string Key { get; set; }

        public string Label { get; set; }

        public List<TreeNode> Children { get; set; }

        public TreeNode()
        {
            this.Children = new List<TreeNode>();
        }
    }
}
