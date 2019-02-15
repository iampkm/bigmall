using Guoc.BigMall.Infrastructure.DataAnnotations;
using Guoc.BigMall.Infrastructure.DataAnnotations.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class AccountCreateModel
    {
        [Required(ErrorMessage = "工号不能为空。")]
        [MaxLength(20, ErrorMessage = "工号长度不能超过20。")]
        public string UserName { get; set; }

        [StringLength(6, ErrorMessage = "密码长度必须是6位。")]
        public string Password { get; set; }

        [Required(ErrorMessage = "姓名不能为空。")]
        [MaxLength(20, ErrorMessage = "姓名长度不能超过20。")]
        public string NickName { get; set; }

        [Phone(ErrorMessage = "电话号码格式错误。")]
        public string Phone { get; set; }

        [ElmRequired(RuleTrigger.Change, "必须指定一个角色。")]
        public int RoleId { get; set; }

        //public int StoreId { get; set; }

        [Required(ErrorMessage = "门店权限不能为空。")]
        public int[] CanViewStores { get; set; }
    }

    public class AccountEditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "工号不能为空。")]
        [MaxLength(20, ErrorMessage = "工号长度不能超过20。")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "姓名不能为空。")]
        [MaxLength(20, ErrorMessage = "姓名长度不能超过20。")]
        public string NickName { get; set; }

        [Phone(ErrorMessage = "电话号码格式错误。")]
        public string Phone { get; set; }

        [ElmRequired(RuleTrigger.Change, "必须指定一个角色。")]
        public int RoleId { get; set; }

        //public int StoreId { get; set; }

        [Required(ErrorMessage = "门店权限不能为空。")]
        public int[] CanViewStores { get; set; }
    }
}
