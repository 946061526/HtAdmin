using HTAdmin.IdentityManager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class SystemUserModel
    {
        [Required(ErrorMessage = "用户名为必填项")]
        public string RealName { get; set; }
        [Required(ErrorMessage = "手机号码为必填项")]
        public string Mobile { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool Enabled { get; set; }
        [Required]
        public string RoleId { get; set; }
        public string UserId { get; set; }

        public RoleType RoleType { get; set; }
    }
}