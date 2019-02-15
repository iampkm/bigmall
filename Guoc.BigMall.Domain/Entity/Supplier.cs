using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBS.Domain.ValueObject;
using Guoc.BigMall.Domain.ValueObject;
namespace Guoc.BigMall.Domain.Entity
{
    /// <summary>
    /// 供应商
    /// </summary>
   public class Supplier:BaseEntity
    {

       public Supplier() {
           this.CreatedOn = DateTime.Now;
           this.UpdatedOn = DateTime.Now;
       }
       /// <summary>
       /// 供应商编码： 经销方式 1+ 区域 11+ 公司检测拼音字母 3位 共6位
       /// </summary>
       public string Code { get; set; }  

       public string Name { get; set; }
       /// <summary>
       /// 简称
       /// </summary>
       public string ShortName { get; set; }

        public SupplierType Type { get; set; }
       /// <summary>
       /// 联系人
       /// </summary>
       public string Contact { get; set; }
        public string QQ { get; set; }

        public string Address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
       /// <summary>
       /// 开户行
       /// </summary>
       public string Bank { get; set; }
       /// <summary>
       /// 税号
       /// </summary>
       public string TaxNo { get; set; }
       /// <summary>
       /// 开户行账号
       /// </summary>
       public string BankAccount { get; set; }
       /// <summary>
       /// 开户名
       /// </summary>
       public string BankAccountName { get; set; }

       /// <summary>
       /// 执照编号
       /// </summary>
       public string LicenseNo { get; set; }
       public DateTime CreatedOn { get; set; }

       public int CreatedBy { get; set; }

       public DateTime UpdatedOn { get; set; }

       public int UpdatedBy { get; set; }

       // qq ,address
    }
}
