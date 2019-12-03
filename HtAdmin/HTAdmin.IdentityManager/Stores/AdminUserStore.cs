using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminUserStore : UserStore<AdminUser, AdminRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public AdminUserStore(ApplicationDbContext context) : base(context)
        {
        }
    }
}
