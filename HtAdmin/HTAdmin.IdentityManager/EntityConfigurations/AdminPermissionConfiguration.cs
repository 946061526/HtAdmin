using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminPermissionConfiguration : EntityTypeConfiguration<AdminPermission>
    {
        public AdminPermissionConfiguration()
        {
            this.ToTable("Permissions");

            this.HasKey(p => p.Id);
            this.Property(p => p.Id).HasMaxLength(128);

            this.Property(p => p.Name).IsRequired().HasMaxLength(128);
            this.Property(p => p.Descrition).HasMaxLength(256);
            this.Property(p=>p.ParentId).HasMaxLength(128);
            this.Property(p => p.ParentPath).HasMaxLength(256);

            this.HasMany(p => p.Roles).WithRequired().HasForeignKey(p => p.PermissionId);
        }
    }
}
