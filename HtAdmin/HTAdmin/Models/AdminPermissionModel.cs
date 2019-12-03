using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class AdminPermissionModel
    {
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Descrition { get; set; }
        public virtual bool IsEnable { get; set; }
        public virtual string ParentId { get; set; }
        public virtual string ParentPath { get; set; }
    }
}