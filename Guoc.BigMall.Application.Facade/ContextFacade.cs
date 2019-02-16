using Guoc.BigMall.Infrastructure.Caching;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using Guoc.BigMall.Infrastructure;

namespace Guoc.BigMall.Application.Facade
{
    public class ContextFacade : IContextFacade
    {
        IDBContext _db;
        ICacheManager _cache;
        private const string STORE_ACCOUNT_MAPPING_CACHE_KEY = "StoreAccountMapping";
        public ContextFacade(IDBContext dbContext, ICacheManager cache)
        {
            this._db = dbContext;
            this._cache = cache;
        }

        public AccountInfo CurrentAccount
        {
            get
            {
                var user = HttpContext.Current.User;
                if (user != null &&
                    user.Identity != null &&
                    user.Identity.IsAuthenticated &&
                    user.Identity is FormsIdentity)
                {
                    // var accountId = ((FormsIdentity)user.Identity).Ticket.UserData;
                    var accountInfo = ((FormsIdentity)user.Identity).Ticket.UserData;
                    var account = JsonConvert.DeserializeObject<AccountInfo>(accountInfo);
                    account.StoreArray = this.GetAccountStoreIds(account.AccountId).ToArray();
                    // var accountName = user.Identity.Name;
                    //  return new AccountIdentity(accountId, accountName);
                    return account;
                }
                throw new Exception("账号已过期，请注销重新登录");
            }
        }

        public List<int> GetAccountStoreIds(int accountId)
        {
            var allMapping = LoadStoreAccountMappingCache();
            return allMapping.Where(m => m.AccountId == accountId).Select(m => m.StoreId).ToList();
        }

        public List<AccountStoreMap> LoadStoreAccountMappingCache()
        {
            var allMapping = _cache.Get<List<AccountStoreMap>>(STORE_ACCOUNT_MAPPING_CACHE_KEY, 120, () =>
            {
                var storeMapping = _db.Table<AccountStoreMap>().ToList();
                return storeMapping;
            });

            if (allMapping == null)
                throw new FriendlyException("门店数据获取失败。");

            return allMapping;
        }

        public void RemoveStoreAccountMappingCache()
        {
            _cache.Remove(STORE_ACCOUNT_MAPPING_CACHE_KEY);
        }
    }
}