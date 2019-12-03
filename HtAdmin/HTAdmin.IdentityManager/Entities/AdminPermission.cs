using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminPermission
    {
        public AdminPermission()
        {
            Sort = 90;
        }
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Descrition { get; set; }
        public virtual bool IsEnable { get; set; }
        public virtual string ParentId { get; set; }
        public virtual string ParentPath { get; set; }
        public virtual ICollection<AdminRolePermission> Roles { get; set; }
        public virtual int Sort { get; set; }
        public virtual DateTime CreationDate { get; set; }
    }
}
