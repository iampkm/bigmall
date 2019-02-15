using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
   public class RoleMenu:BaseEntity
    {
       public int RoleId { get; set; }

       public int MenuId { get; set; }
    }
}
