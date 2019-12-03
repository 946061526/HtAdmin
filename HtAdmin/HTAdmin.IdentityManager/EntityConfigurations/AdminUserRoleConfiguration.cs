using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminUserRoleConfiguration : EntityTypeConfiguration<IdentityUserRole>
    {
        public AdminUserRoleConfiguration()
        {
            this.ToTable("UserRoles");

            this.HasKey(p => new { p.UserId, p.RoleId });

            this.Property(p => p.RoleId).IsRequired().HasMaxLength(50);
            this.Property(p => p.UserId).IsRequired().HasMaxLength(50);
        }
    }
}
