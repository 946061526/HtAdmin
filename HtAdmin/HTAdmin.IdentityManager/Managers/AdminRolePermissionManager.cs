using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminRolePermissionManager : IDisposable
    {
        public AdminRolePermissionManager()
        {

        }

        public int DeleteAndAddRolePermissions(string roleId, IList<AdminRolePermission> list)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var p = new SqlParameter("RoleId", roleId);
                int count = ctx.Database.ExecuteSqlCommand("DELETE FROM dbo.RolePermissions WHERE RoleId=@RoleId", p);
                foreach (var item in list)
                {
                    ctx.RolePermissions.Add(item);
                }
                return ctx.SaveChanges();
            }
        }

        public static AdminRolePermissionManager Create(IdentityFactoryOptions<AdminRolePermissionManager> options, IOwinContext context)
        {
            var manager = new AdminRolePermissionManager();
            return manager;
        }

        public void Dispose()
        {

        }
    }
}
