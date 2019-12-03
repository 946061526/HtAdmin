using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminUserLoginConfiguration : EntityTypeConfiguration<IdentityUserLogin>
    {
        public AdminUserLoginConfiguration()
        {
            this.ToTable("UserLogins");

            this.HasKey(p => new { p.LoginProvider, p.ProviderKey, p.UserId });

            this.Property(p => p.UserId).IsRequired().HasMaxLength(50);
        }
    }
}
