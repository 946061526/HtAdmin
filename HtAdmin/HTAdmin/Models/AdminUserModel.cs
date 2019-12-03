using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class AdminUserModel
    {
        public string UserId{get;set;}
        public string Name { get; set; }

        public string UserName { get; set; }

        public string Mobile { get; set; }
        public string RoleId { get; set; }
        public string Password { get; set; }
        public bool IsEnable { get; set; }

    }
}