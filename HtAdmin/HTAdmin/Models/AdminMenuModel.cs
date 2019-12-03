using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class AdminMenuModel
    {
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Descrition { get; set; }
        public virtual string Url { get; set; }
        public virtual string IconUrl { get; set; }
        public virtual bool IsEnable { get; set; }
        public virtual string ParentId { get; set; }
        public virtual string PermissionId { get; set; }
        public virtual int Sort { get; set; }
        public virtual DateTime CreationDate { get; set; }
    }
}