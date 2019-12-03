using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminPermissionManager : IDisposable
    {
        public List<AdminPermission> GetAllPermissions()
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                return ctx.Permissions.OrderBy(p => p.Sort).ToList();
            }
        }
        public static AdminPermissionManager Create(IdentityFactoryOptions<AdminPermissionManager> options, IOwinContext context)
        {
            var manager = new AdminPermissionManager();
            return manager;
        }
        public void Dispose()
        {

        }
    }
}
