using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminRole : IdentityRole
    {
        public AdminRole()
        {
            CreationDate = DateTime.Now;
            UpdateDate = DateTime.Now;
        }
        public virtual RoleType Type { get; set; }
        public virtual ICollection<AdminRolePermission> Permissions { get; set; }

        public virtual string UpdatorUserId { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual string CreatorUserId { get; set; }
        public virtual DateTime CreationDate { get; set; }
    }
}
