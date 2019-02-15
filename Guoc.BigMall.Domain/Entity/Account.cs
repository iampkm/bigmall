using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Domain;
using Guoc.BigMall.Domain.ValueObject;
using Guoc.BigMall.Infrastructure;
namespace Guoc.BigMall.Domain.Entity
{
    public class Account : BaseEntity
    {
        private const int _FailureTimes = 5;
        private const int _ResetMinute = 15;

        public Account()
        {
            this.Status = AccountStatus.Active;
            this.CreatedOn = DateTime.Now;
            this.LastUpdateDate = DateTime.Now;
        }
        public Account(string userName, string password, string nickName, int roleId, string phone)
            : this()
        {
            this.UserName = userName;
            this.Password = password;
            this.NickName = nickName;
            this.RoleId = roleId;
            this.Phone = phone;
        }

        public string UserName { get; set; }
        /// <summary>
        /// MD5 加密密码
        /// </summary>
        public string Password { get; set; }

        public string NickName { get; set; }

        public int RoleId { get; set; }
        public DateTime CreatedOn { get; private set; }

        public AccountStatus Status { get; private set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastUpdateDate { get; set; }
        /// <summary>
        /// 登录错误次数
        /// </summary>
        public int LoginErrorCount { get; private set; }

        public string Phone { get; set; }
        /// <summary>
        /// 前台网站使用密码，默认和后台一致
        /// </summary>
       //public string WebPassword { get; set; }

        public void Actived()
        {
            this.Status = AccountStatus.Active;
        }
        public void Deleted()
        {
            this.Status = AccountStatus.Deleted;
        }

        public void Disabled()
        {
            this.Status = AccountStatus.Disabled;
        }
        /// <summary>
        /// 加密密码
        /// </summary>
        public void EncryptionPassword()
        {
            MD5 md5Prider = MD5.Create();
            this.Password = md5Prider.GetMd5Hash(this.Password);           
        }
        /// <summary>
        /// 初始化密码时，前后台一致
        /// </summary>
        public void InitEncryptionPassword()
        {
            MD5 md5Prider = MD5.Create();
            this.Password = md5Prider.GetMd5Hash(this.Password);           
           // this.WebPassword = this.Password;           
        }

        public void ResetErrorCount()
        {
            this.LoginErrorCount = 0;
            this.LastUpdateDate = DateTime.Now;
        }

        public void CheckPassword(string password)
        {           
            MD5 md5Prider = MD5.Create();
            if (!md5Prider.VerifyMd5Hash(password, this.Password))
            {
                throw new FriendlyException("账户或密码不正确!"); 
            }
          
        }

        public void CheckAccountState()
        {
            if (this.Status != AccountStatus.Active) { throw new FriendlyException("账户未激活"); }
        }
        /// <summary>
        /// 校验登录
        /// </summary>
        public void CheckLoginFailedTimes()
        {
            if (this.LastUpdateDate.AddMinutes(_ResetMinute) < DateTime.Now)
            {   //距离最后一次登陆错误，超过15分钟，自动解锁
                this.LoginErrorCount = 0;
                this.LastUpdateDate = DateTime.Now;
            }
            if (this.LoginErrorCount >= _FailureTimes)
            {
                throw new FriendlyException(string.Format("您登陆错误次数超过{0}次，请{1}分钟后重试!", _FailureTimes, _ResetMinute));
            }
        }
        /// <summary>
        /// 统计登录错误次数
        /// </summary>
        public bool CountLoginfailedTimes()
        {
            if (this.LoginErrorCount < _FailureTimes)
            {
                this.LoginErrorCount += 1;
                this.LastUpdateDate = DateTime.Now;
                return true;
            }
            return false;
        }

        public void ChangePassword( string oldPassword, string newPassword)
        {
            this.CheckPassword(oldPassword);
           
            this.Password = newPassword;
            this.EncryptionPassword();
            this.LastUpdateDate = DateTime.Now;         
        }

        
    }
}
