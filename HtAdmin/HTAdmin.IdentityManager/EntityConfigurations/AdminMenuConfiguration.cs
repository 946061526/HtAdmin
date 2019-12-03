using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminMenuConfiguration : EntityTypeConfiguration<AdminMenu>
    {
        public AdminMenuConfiguration()
        {
            this.ToTable("Menus");

            this.HasKey(p => p.Id);
            this.Property(p => p.Id).HasMaxLength(128);

            this.Property(p => p.Name).IsRequired().HasMaxLength(128);
            this.Property(p => p.Descrition).HasMaxLength(256);
            this.Property(p => p.Url).HasMaxLength(256);
            this.Property(p => p.IconUrl).HasMaxLength(256);

            this.Property(p=>p.ParentId).HasMaxLength(128);
            this.Property(p => p.PermissionId).HasMaxLength(128);
        }
    }
}
