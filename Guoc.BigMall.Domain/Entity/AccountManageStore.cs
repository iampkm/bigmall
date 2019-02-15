using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
   public class AccountManageStore:BaseEntity
    {
          public int AccountId { get; set; }

          /// <summary>
          ///  所有门店：store-all, 标签门店:store-tag, 门店： store-id, 归属门店: belong-to-store
          /// </summary>
          public string ManageType { get; set; }

          public int ManageValue { get; set; }

          public string Remark { get; set; }
    }
}
