using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminUserConfiguration : EntityTypeConfiguration<AdminUser>
    {
        public AdminUserConfiguration()
        {
            this.ToTable("Users");

            this.HasMany(p => p.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
            this.HasMany(p => p.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
            this.HasMany(p => p.Logins).WithRequired().HasForeignKey(ul => ul.UserId);

            this.HasKey(p => p.Id);

            this.Property(p => p.Id).HasMaxLength(50);

            this.Property(p => p.Name).IsRequired().HasMaxLength(50);

            this.Property(p => p.Email).HasMaxLength(128);

            this.Property(p => p.PhoneNumber).HasMaxLength(30);

            this.Property(p => p.UserName).HasMaxLength(50);
            this.Property(p => p.CreatorUserId).HasMaxLength(50);
            this.Property(p => p.UpdatorUserId).HasMaxLength(50);
            this.Property(p => p.PasswordHash).IsRequired().HasMaxLength(256);

            this.Property(p => p.LockoutEndDateUtc).HasColumnType("datetime");
            this.Property(p => p.CreationDate).HasColumnType("datetime");
            this.Property(p => p.UpdateDate).HasColumnType("datetime");
        }
    }
}
