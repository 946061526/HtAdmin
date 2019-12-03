using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminOperationLogConfiguration : EntityTypeConfiguration<AdminOperationLog>
    {
        public AdminOperationLogConfiguration()
        {
            this.ToTable("OperationLogs");

            this.HasKey(p => p.Id);
            this.Property(p => p.UserId).HasMaxLength(50);
            this.Property(p => p.OperUserId).IsRequired().HasMaxLength(50);
            this.Property(p => p.OperationName).IsRequired().HasMaxLength(256);
        }
    }
}
