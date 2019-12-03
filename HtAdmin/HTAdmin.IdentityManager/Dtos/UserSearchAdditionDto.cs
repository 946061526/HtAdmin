using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager.Dtos
{
    public class UserSearchAdditionDto
    {
        public string RoleId { get; set; }
        public string Mobile { get; set; }
        public bool? IsEnable { get; set; }
        public string RealName { get; set; }
        public DateTime? LoginTime { get; set; }
        public string LoginIp { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
