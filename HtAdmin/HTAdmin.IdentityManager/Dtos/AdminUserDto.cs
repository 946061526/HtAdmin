using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager.Dtos
{
    public class AdminUserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public List<string> RoleIds { get; set; }
        public string Password { get; set; }
        public bool IsEnable { get; set; }
    }
}
