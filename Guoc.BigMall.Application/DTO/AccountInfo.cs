using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class AccountInfo
    {
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        /// <summary>
        /// 是否超管。
        /// </summary>
        public bool IsSuperAdmin { get { return this.AccountId == 1; } }

        public string StoreName { get; set; }

        //public int StoreId { get; set; }
        /// <summary>
        /// 逗号分隔门店ID 字符串，为空表示可查看所有
        /// </summary>
        public string CanViewStores
        {
            get
            {
                return string.Join(",", this.StoreArray);
            }
        }

        /// <summary>
        /// 逗号分隔门店ID 字符串，为空表示可查看所有
        /// </summary>
        public int[] StoreArray { get; set; }

        /// <summary>
        /// 是否拥有所有门店的权限。
        /// </summary>
        public bool HaveAllStores { get { return StoreArray.Length == 0; } }

        /// <summary>
        /// 门店编码
        /// </summary>
        public string StoreCode { get; set; }
        /// <summary>
        /// 是否显示门店选择组件
        /// </summary>
        /// <returns></returns>
        public bool ShowSelectStore()
        {
            //if (this.StoreId == 0)
            //{
            //    return true;
            //}
            if (string.IsNullOrEmpty(CanViewStores))
            {
                return true;
            }
            var viewStoresArray = CanViewStores.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (viewStoresArray.Length == 1)
            {
                // 有归属门店，且门店查看权限只有一个门店。
                return false;
            }
            else
            {
                return true;
            }
        }

        public AccountInfo()
        {
            this.StoreArray = new int[0];
        }

        public AccountInfo(int accountId, string nickName)
            : this()
        {
            this.AccountId = accountId;
            this.NickName = nickName;
        }
    }
}
