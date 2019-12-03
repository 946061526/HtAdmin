using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminMenuManager : IDisposable
    {
        public List<AdminMenu> GetAllMenus()
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                return ctx.Menus.ToList();
            }
        }
        public List<AdminMenu> GetMenusByUserId(string userId)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var query = from ur in ctx.UserRoles.Where(p => p.UserId == userId)
                            join r in ctx.Roles on ur.RoleId equals r.Id
                            select r.Id;

                var pers = from p in ctx.RolePermissions
                           where query.Contains(p.RoleId)
                           select p.PermissionId;

                var list = from m in ctx.Menus
                           where pers.Contains(m.PermissionId)
                           orderby m.Sort
                           select m;
                return list.ToList();
            }
        }
        public static AdminMenuManager Create(IdentityFactoryOptions<AdminMenuManager> options, IOwinContext context)
        {
            var manager = new AdminMenuManager();
            return manager;
        }
        public void Dispose()
        {

        }
    }
}
