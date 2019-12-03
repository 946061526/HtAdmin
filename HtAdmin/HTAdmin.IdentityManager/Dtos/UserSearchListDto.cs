using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager.Dtos
{
    public class UserSearchListDto
    {
        public string LoginName { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string DisplayName { get; set; }
        public string Mobile { get; set; }
        public bool IsEnable { get; set; }
        public long RowId { get; set; }
        public string RealName { get; set; }
        public DateTime? LoginTime { get; set; }
        public string LoginIp { get; set; }
        public string IpDisplayName { get; set; }
        public string Password { get; set; }

        public DateTime? CreationDate { get; set; }
    }
}
