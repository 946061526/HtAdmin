using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminRoleManager : RoleManager<AdminRole, string>
    {
        public AdminRoleManager(IRoleStore<AdminRole, string> store) : base(store)
        {

        }

        public int GetRoleCountByRoleName(string roleName)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                return ctx.Roles.Where(p=>p.Name == roleName).Count();
            }
        }
        public List<AdminRole> GetAllRoles()
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                return ctx.Roles.ToList();
            }
        }
        public int DeleteRole(string roleId)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var role = ctx.Roles.Include(p => p.Users).Include(p => p.Permissions).FirstOrDefault(p => p.Id == roleId);
                if (role != null)
                {
                    ctx.Roles.Remove(role);
                    return ctx.SaveChanges();
                }
                return 0;
            }
        }

        public int UpdateRole(AdminRole entity)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                ctx.Roles.Attach(entity);
                return ctx.SaveChanges();
            }
        }

        public void AddRole(AdminRole entity)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                ctx.Roles.Add(entity);
                ctx.SaveChanges();
            }
        }

        public List<AdminRole> GetAdminRolesByPagination(int pageIndex, int pageSize, out int count)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                count = ctx.Roles.Count();
                var list = ctx.Roles.Include(p => p.Permissions).OrderByDescending(p => p.UpdateDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            }
        }

        public List<AdminRole> GetAdminRoleByRoleId(string roleId)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                return ctx.Roles.Include(p=>p.Permissions).Where(p => p.Id == roleId).ToList();
            }
        }

        public static AdminRoleManager Create(IdentityFactoryOptions<AdminRoleManager> options, IOwinContext context)
        {
            var manager = new AdminRoleManager(new AdminRoleStore(context.Get<ApplicationDbContext>()));
            return manager;
        }
    }
}
