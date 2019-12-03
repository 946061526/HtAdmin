using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminLoginHistoryConfiguration : EntityTypeConfiguration<AdminLoginHistory>
    {
        public AdminLoginHistoryConfiguration()
        {
            this.ToTable("UserLoginHistory");

            this.HasKey(p => p.Id);
            this.Property(p => p.Id).HasMaxLength(50);
            this.Property(p => p.UserId).IsRequired().HasMaxLength(50);
            this.Property(p => p.Ip).HasMaxLength(128);
            this.Property(p => p.IpArea).HasMaxLength(30);
        }
    }
}
