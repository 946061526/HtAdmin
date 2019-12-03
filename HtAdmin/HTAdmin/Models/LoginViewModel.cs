using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "登录账号不能为空")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "登录密码不能为空")]
        public string Password { get; set; }
        [Required(ErrorMessage = "验证码不能为空")]
        public string VerifyCode { get; set; }
        public bool RememberMe { get; set; }
    }
}