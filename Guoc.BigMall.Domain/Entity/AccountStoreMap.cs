using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Schema;

namespace Guoc.BigMall.Domain.Entity
{
    [Table("account_store_map")]
    public class AccountStoreMap : BaseEntity
    {
        public int StoreId { get; set; }
        public int AccountId { get; set; }
    }
}
