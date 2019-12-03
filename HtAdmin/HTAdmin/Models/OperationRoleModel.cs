using HTAdmin.IdentityManager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class OperationRoleModel
    {
        [Required]
        public string RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }
        [Required]
        public RoleType RoleType { get; set; }
        [Required]
        public List<string> FunctionIds { get; set; }

    }
}