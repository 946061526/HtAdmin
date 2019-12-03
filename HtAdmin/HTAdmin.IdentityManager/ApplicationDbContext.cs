using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class ApplicationDbContext : IdentityDbContext<AdminUser, AdminRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public override IDbSet<AdminUser> Users { get; set; }
        public override IDbSet<AdminRole> Roles { get; set; }
        public virtual IDbSet<IdentityUserLogin> UserLogins { get; set; }
        public virtual IDbSet<IdentityUserRole> UserRoles { get; set; }
        public virtual IDbSet<IdentityUserClaim> UserClaims { get; set; }
        public virtual IDbSet<AdminPermission> Permissions { get; set; }
        public virtual IDbSet<AdminRolePermission> RolePermissions { get; set; }
        public virtual IDbSet<AdminLoginHistory> LoginHistory { get; set; }
        public virtual IDbSet<AdminOperationLog> OperationLogs { get; set; }
        public virtual IDbSet<AdminMenu> Menus { get; set; }

        public ApplicationDbContext()
            : base("HTAdminIdentity")
        {

        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            modelBuilder.Configurations.Add(new AdminUserConfiguration());
            modelBuilder.Configurations.Add(new AdminRoleConfiguration());
            modelBuilder.Configurations.Add(new AdminUserRoleConfiguration());
            modelBuilder.Configurations.Add(new AdminUserClaimConfiguration());
            modelBuilder.Configurations.Add(new AdminUserLoginConfiguration());
            modelBuilder.Configurations.Add(new AdminPermissionConfiguration());
            modelBuilder.Configurations.Add(new AdminRolePermissionConfiguration());
            modelBuilder.Configurations.Add(new AdminMenuConfiguration());
            modelBuilder.Configurations.Add(new AdminLoginHistoryConfiguration());
            modelBuilder.Configurations.Add(new AdminOperationLogConfiguration());
        }
    }
}
