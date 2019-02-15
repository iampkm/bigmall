﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class StoreTreeNode : TreeNode
    {
        public bool IsStore { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
