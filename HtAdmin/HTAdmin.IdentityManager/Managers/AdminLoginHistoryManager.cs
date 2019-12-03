using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminLoginHistoryManager : IDisposable
    {
        public void AddLoginHistory(AdminLoginHistory entity)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                ctx.LoginHistory.Add(entity);
                ctx.SaveChanges();
            }
        }

        public static AdminLoginHistoryManager Create(IdentityFactoryOptions<AdminLoginHistoryManager> options, IOwinContext context)
        {
            var manager = new AdminLoginHistoryManager();
            return manager;
        }

        public void Dispose()
        {

        }

        public List<AdminLoginHistory> GetUserLoginHistory(string userId, int pageIndex, int pageSize, out int count)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var query = ctx.LoginHistory.Where(p => p.UserId == userId);
                count = query.Count();
                var list = query
                    .OrderByDescending(p => p.CreationDate)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize).ToList();
                return list;
            }
        }
    }
}
