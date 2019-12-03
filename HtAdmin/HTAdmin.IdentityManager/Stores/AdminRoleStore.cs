using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminRoleStore : RoleStore<AdminRole, string, IdentityUserRole>
    {
        public AdminRoleStore(ApplicationDbContext context) : base(context)
        {

        }
    }
   
}
