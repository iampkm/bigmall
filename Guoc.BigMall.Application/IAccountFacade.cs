using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
namespace Guoc.BigMall.Application
{
    public interface IAccountFacade
    {
        AccountEditModel GetAccountById(int id);

        AccountInfo Login(LoginModel model);

        void Create(AccountCreateModel model);

        void Edit(AccountEditModel model);
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newPassword"></param>
        void ChangePassword(int id, string oldPassword, string newPassword);
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        void ResetPassword(int[] ids);
        /// <summary>
        /// 激活账户
        /// </summary>
        void ActiveAccount(int[] ids);
        /// <summary>
        /// 禁用账户
        /// </summary>
        void DisabledAccount(int[] ids);
        void DeleteAccount(int id);

        IEnumerable<AccountDto> GetPageList(Pager page, int? id, string userName, string nickName, int storeId);


    }
}
