using HTAdmin.IdentityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class SysOperationLogInfoModel
    {
        public virtual DateTime? StartCreationDate { get; set; }
        public virtual DateTime? EndCreationDate { get; set; }
        public virtual string Name { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string Description { get; set; }
        public virtual string OperationName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}