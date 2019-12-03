using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminRoleConfiguration : EntityTypeConfiguration<AdminRole>
    {
        public AdminRoleConfiguration()
        {
            this.ToTable("Roles");

            this.HasKey(p => p.Id);

            this.Property(p => p.Id).HasMaxLength(50);
            this.Property(p => p.CreatorUserId).HasMaxLength(50);
            this.Property(p => p.UpdatorUserId).HasMaxLength(50);
            this.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(256);

            this.HasMany(p => p.Users).WithRequired().HasForeignKey(p => p.RoleId);
            this.HasMany(p => p.Permissions).WithRequired().HasForeignKey(p => p.RoleId);

        }
    }
}
