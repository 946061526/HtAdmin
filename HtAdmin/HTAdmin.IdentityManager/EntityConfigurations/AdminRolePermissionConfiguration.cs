using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminRolePermissionConfiguration : EntityTypeConfiguration<AdminRolePermission>
    {
        public AdminRolePermissionConfiguration()
        {
            this.ToTable("RolePermissions");
            this.HasKey(p => new { p.RoleId,p.PermissionId});

            this.Property(p => p.RoleId).HasMaxLength(50);
            this.Property(p => p.PermissionId).HasMaxLength(128);
        }
    }
}
