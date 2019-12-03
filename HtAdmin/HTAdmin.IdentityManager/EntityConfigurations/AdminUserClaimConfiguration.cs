using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminUserClaimConfiguration : EntityTypeConfiguration<IdentityUserClaim>
    {
        public AdminUserClaimConfiguration()
        {
            this.ToTable("UserClaims");

            this.HasKey(p => p.Id);

            this.Property(p => p.UserId).IsRequired().HasMaxLength(50);
            this.Property(p => p.ClaimType).HasMaxLength(256);
            this.Property(p => p.ClaimValue).HasMaxLength(1024);
        }
    }
}
