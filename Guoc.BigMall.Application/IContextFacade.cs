using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Domain.Entity;
using System.Collections.Generic;

namespace Guoc.BigMall.Application
{
    public interface IContextFacade
    {
        // AccountIdentity CurrentAccount { get; }
        AccountInfo CurrentAccount { get; }

        List<int> GetAccountStoreIds(int accountId);
        List<StoreAccountMapping> LoadStoreAccountMappingCache();
        void RemoveStoreAccountMappingCache();
    }

    //public class AccountIdentity
    //{
    //    public string AccountId { get; private set; }
    //    public string AccountName { get; private set; }

    //    public AccountIdentity(string accountId, string accountName)
    //    {
    //        AccountId = accountId;
    //        AccountName = accountName;
    //    }
    //}
}
