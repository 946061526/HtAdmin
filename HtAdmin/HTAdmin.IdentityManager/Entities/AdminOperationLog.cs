using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    [Table("OperationLogs")]
    public class AdminOperationLog
    {
        public virtual long Id { get; set; }
        public virtual string OperationName { get; set; }
        public virtual string Description { get; set; }
        public virtual string UserId { get; set; }
        public virtual string OperUserId { get; set; }
        public virtual DateTime CreationDate { get; set; }
    }
}
