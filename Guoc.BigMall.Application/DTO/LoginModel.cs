using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Guoc.BigMall.Application.DTO
{
   public class LoginModel
   {
       [Required(ErrorMessage="用户名不能为空!")]
       public string UserName { get; set; }
       [Required(ErrorMessage = "密码不能为空!")]
       public string Password { get; set; }

       public bool RememberMe { get; set; }
       public string IpAddress { get; set; }

       public string ReturnUrl { get; set; }
      
    }
}
