using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
namespace Guoc.BigMall.Application.ViewObject
{
    public class AccountDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }
        public string CreatedOn { get; set; }

        public string StoreCodes { get; set; }

        public AccountStatus Status { get; set; }
        public string Phone { get; set; }

        public string StatusName
        {
            get
            {
                return Status.Description();
            }
        }
        /// <summary>
        /// 登录错误次数
        /// </summary>
        public int LoginErrorCount { get; set; }
        /// <summary>
        ///  关联门店分组名
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        ///  分拣标记
        /// </summary>
       // public string PickingGroupName { get; set; }
    }
}
