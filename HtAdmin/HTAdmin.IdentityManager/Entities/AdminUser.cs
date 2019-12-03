using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    [Table("Users")]
    public class AdminUser : IdentityUser
    {
        public AdminUser()
        {
            IsEnable = true;
            UpdateDate = DateTime.Now;
            CreationDate = DateTime.Now;
        }
        public virtual RoleType Type { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsEnable { get; set; }

        public virtual string UpdatorUserId { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual string CreatorUserId { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AdminUser,string> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }
}
