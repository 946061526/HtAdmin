using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminRolePermission
    {
        public virtual string RoleId { get; set; }
        public virtual string PermissionId { get; set; }
        public virtual DateTime CreationDate { get; set; }
    }
}
