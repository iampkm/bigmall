using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext;
using Guoc.BigMall.Domain.ValueObject;
using EBS.Domain.ValueObject;
namespace Guoc.BigMall.Domain.Entity
{
    public class AccountLoginHistory : BaseEntity
    {
       public AccountLoginHistory(int accountId,string userName, string ipAddress,LoginStatus status = LoginStatus.Login)
       {
           this.AccountId = accountId;
           this.UserName = userName;
           this.IPAddress = ipAddress;
           this.LoginStatus = status;
           this.CreatedOn = DateTime.Now;
       }
       public int AccountId { get; private set; }

       public string UserName { get; private set; }

       public DateTime CreatedOn { get; private set; }

       public string IPAddress { get; private set; }

       public LoginStatus LoginStatus { get; private set; }

     
    }
}
