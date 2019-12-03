using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager.Dtos
{
    public class UserRoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public bool IsEnable { get; set; }
        public DateTime CreationDate { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
    }
}
