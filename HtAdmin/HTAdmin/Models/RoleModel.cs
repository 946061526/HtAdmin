using HTAdmin.IdentityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class RoleModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public RoleType RoleType { get; set; }
        public List<string> Permissions { get; set; }
    }
}