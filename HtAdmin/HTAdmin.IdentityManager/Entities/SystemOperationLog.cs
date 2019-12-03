using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class SystemOperationLog
    {
        public virtual int Id { get; set; }
        public virtual string CreationDate { get; set; }
        public virtual string Name { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string Description { get; set; }
        public virtual string OperationName { get; set; }
    }
}
