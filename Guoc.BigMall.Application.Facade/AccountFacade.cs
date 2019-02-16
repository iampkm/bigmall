using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Caching;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
namespace Guoc.BigMall.Application.Facade
{
    public class AccountFacade : IAccountFacade
    {
        IDBContext _db;
        IContextFacade _context;

        public AccountFacade(IDBContext dbContext, IContextFacade context)
        {
            this._db = dbContext;
            this._context = context;
        }

        public AccountEditModel GetAccountById(int id)
        {
            var account = _db.Table<Account>().FirstOrDefault(a => a.Id == id).MapTo<AccountEditModel>();
            //account.CanViewStores = _db.Table<StoreAccountMapping>().Where(m => m.AccountId == account.Id).Select(m => m.StoreId).ToList().ToArray();
            account.CanViewStores = _db.Table<AccountStoreMap>().Where(m => m.AccountId == account.Id).ToList().Select(m => m.StoreId).ToArray();
            return account;
        }

        public AccountInfo Login(LoginModel model)
        {
            var account = _db.Table<Account>().FirstOrDefault(n => n.UserName == model.UserName);

            if (account == null) throw new FriendlyException("用户名或密码错误!");

            account.CheckAccountState();
            account.CheckLoginFailedTimes();

            try
            {
                account.CheckPassword(model.Password);
                this._db.Update(account);
                this._db.Insert(new AccountLoginHistory(account.Id, account.UserName, model.IpAddress));
                this._db.SaveChange();
            }
            catch (FriendlyException ex)
            {
                if (account.CountLoginfailedTimes())
                {
                    this._db.Update(account);
                    this._db.SaveChange();
                }
                throw new FriendlyException(ex.Message);
            }

            var role = _db.Table<Role>().FirstOrDefault(n => n.Id == account.RoleId);
            var storeName = string.Empty;
            var storeCode = string.Empty;
            var storeMapping = _db.Table<AccountStoreMap>().Where(m => m.AccountId == account.Id).ToList().FirstOrDefault();
            if (storeMapping != null)
            {
                var store = _db.Table<Store>().FirstOrDefault(n => n.Id == storeMapping.StoreId);
                storeName = store.Name;
                storeCode = store.Code;
            }

            return new AccountInfo()
            {
                AccountId = account.Id,
                UserName = account.UserName,
                RoleId = account.RoleId,
                NickName = account.NickName,
                RoleName = role.Name,
                StoreName = storeName,
                StoreCode = storeCode
            };

        }

        public void Create(AccountCreateModel model)
        {
            Account entity = new Account(model.UserName, model.Password, model.NickName, model.RoleId, model.Phone);
            if (this._db.Table<Account>().Exists(a => a.UserName == model.UserName))
            {
                throw new Exception(string.Format("您输入的工号{0}重复,请重新输入!", model.UserName));
            }

            //检查账户可查看门店权限属性
            //entity.CanViewStores = model.CanViewStores;
            //entity.CheckCanViewStore();

            //加密密码
            entity.Password = "123456";
            entity.InitEncryptionPassword();
            //entity.UserName= _accountService.GenerateNewAccount();

            _db.Insert(entity);
            _db.SaveChange();

            SetStoreMapping(entity.Id, model.CanViewStores);
        }

        public void Edit(AccountEditModel model)
        {
            Account entity = _db.Table<Account>().FirstOrDefault(n => n.Id == model.Id);
            if (entity == null) throw new FriendlyException("账户不存在!");
            model.MapTo(entity);
            entity.LastUpdateDate = DateTime.Now;
            //entity.CheckCanViewStore();
            SetStoreMapping(entity.Id, model.CanViewStores);
            _db.Update(entity);
            _db.SaveChange();
        }

        private void SetStoreMapping(int accountId, int[] canViewStores)
        {
            //删除的时候需要查询数据是否存在
            var oldEntitys = _db.Table<AccountStoreMap>().Where(n => n.AccountId == accountId).ToList().ToArray();
            _db.Delete(oldEntitys);
            if (canViewStores.IsNotEmpty())
            {
                //var storeIds = canViewStores.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToIntArray();
                if (canViewStores.Any(storeId => _db.Table<Store>().Exists(s => s.Id == storeId) == false))
                {
                    throw new FriendlyException("门店不存在!");
                }
                var newMapping = canViewStores.Select(storeId => new AccountStoreMap()
                    {
                        AccountId = accountId,
                        StoreId = storeId,
                    }).ToArray();
                _db.Insert(newMapping);
            }
            _db.SaveChange();

            //刷新门店账户关系缓存
            _context.RemoveStoreAccountMappingCache();
        }

        public void ChangePassword(int id, string oldPassword, string newPassword)
        {
            Account entity = _db.Table<Account>().FirstOrDefault(n => n.Id == id);
            if (entity == null) throw new FriendlyException("账户不存在!");
            entity.ChangePassword(oldPassword, newPassword);
            _db.Update(entity);
            _db.SaveChange();
        }

        public void ResetPassword(int[] ids)
        {
            var entities = _db.Table<Account>().Where(n => n.Id.In(ids)).ToList();
            entities.ForEach(entity =>
            {
                if (entity == null) throw new FriendlyException("账号不存在");
                entity.Password = "123456";
                entity.EncryptionPassword();
                entity.LastUpdateDate = DateTime.Now;
                _db.Update(entity);
            });
            _db.SaveChange();
        }

        public void ActiveAccount(int[] ids)
        {
            var entities = _db.Table<Account>().Where(n => n.Id.In(ids)).ToList();
            entities.ForEach(entity =>
            {
                entity.Actived();
                _db.Update(entity);
            });
            _db.SaveChange();
        }

        public void DisabledAccount(int[] ids)
        {
            var entities = _db.Table<Account>().Where(n => n.Id.In(ids)).ToList();
            entities.ForEach(entity =>
            {
                entity.Disabled();
                _db.Update(entity);
            });
            _db.SaveChange();
        }

        public void DeleteAccount(int id)
        {
            var entity = _db.Table<Account>().FirstOrDefault(n => n.Id == id); ;
            entity.Deleted();
            _db.Update(entity);
            _db.SaveChange();
        }

        #region select method

        public IEnumerable<AccountDto> GetPageList(Pager page, int? id, string userName, string nickName, int storeId)
        {
            IEnumerable<AccountDto> rows;
            dynamic param = new ExpandoObject();
            string where = "";

            if (!string.IsNullOrEmpty(nickName))
            {
                where += "AND t0.NickName LIKE @NickName ";
                param.NickName = string.Format("%{0}%", nickName);
            }

            if (!string.IsNullOrEmpty(userName))
            {
                where += "AND t0.UserName LIKE @UserName ";
                param.UserName = string.Format("%{0}%", userName);
            }

            var storeFilter = "";
            if (storeId >= 0)
            {
                where += "AND t2.StoreId = @StoreId ";
                storeFilter = " LEFT JOIN AccountStoreMap t2 ON t0.Id=t2.AccountId ";
                param.StoreId = storeId;
            }

            //StringBuilder sb = new StringBuilder();
            //sb.Append(" SELECT * FROM ( ");
            //sb.AppendFormat(@"
            //    SELECT  ROW_NUMBER() OVER ( ORDER BY t0.Id DESC ) AS rownum ,
            //            t0.Id ,
            //            t0.UserName ,
            //            t0.NickName ,
            //            t0.Status ,
            //            t0.CreatedOn ,
            //            t0.LastUpdateDate ,
            //            t0.LoginErrorCount ,
            //            t1.Name AS RoleName ,
            //            STUFF((SELECT ','+s.Code FROM StoreAccountMapping m,Store s WHERE m.StoreId=s.Id AND m.AccountId=t0.Id FOR XML PATH('')),1,1,'') AS StoreCodes ,
            //            t0.Phone
            //    FROM    Account t0
            //            INNER JOIN Role t1 ON t0.RoleId = t1.Id
            //            {0}
            //    WHERE   t0.Id > 1 {1}", storeFilter, where);
            //sb.Append(string.Format(" ) AS T WHERE rownum BETWEEN {0} AND {1}", (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize));

            string sql = @"Select t0.`Id`,t0.`UserName`,t0.`NickName`,t0.`Status`,t0.`CreatedOn`,t0.`LastUpdateDate`,t0.`LoginErrorCount`,t1.Name as RoleName,
(select s.Name from account_store_map m left join store s on m.storeId=s.id where m.AccountId=t0.Id LIMIT 1 ) as StoreName 
from Account t0 inner join Role t1 on t0.RoleId=t1.Id 
where t0.Id>1 {0} Order By  t0.Id desc LIMIT {1},{2}";
            sql = string.Format(sql, where, (page.PageIndex - 1) * page.PageSize, page.PageSize);
            rows = this._db.DataBase.Query<AccountDto>(sql, param);
            //超级管理员superman不显示
            string sqlCount = string.Format("SELECT COUNT(1) FROM Account t0 INNER JOIN Role t1 ON t0.RoleId=t1.Id {0} WHERE t0.Id>1 {1}", storeFilter, where);
            page.Total = this._db.DataBase.ExecuteScalar<int>(sqlCount, param);
            return rows;
        }

        #endregion

    }
}
