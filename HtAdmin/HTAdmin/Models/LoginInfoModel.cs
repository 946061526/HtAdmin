using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class LoginInfoModel
    {
        public string UserName { get; set; }
        public List<string> Permissions { get; set; }
        public string UserId { get; set; }
    }
}