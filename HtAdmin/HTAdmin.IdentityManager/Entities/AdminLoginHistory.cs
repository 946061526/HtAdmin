using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminLoginHistory : IComparable<AdminLoginHistory>
    {
        public AdminLoginHistory()
        {
            Id = Guid.NewGuid().ToString();
            CreationDate = DateTime.Now;
        }
        public virtual string Id { get; set; }
        public virtual string Ip { get; set; }
        public virtual string IpArea { get; set; }
        public virtual DateTime LoginTime { get; set; }
        public virtual DateTime? LogoutTime { get; set; }
        public virtual string UserId { get; set; }
        public virtual DateTime CreationDate { get; set; }

        public int CompareTo(AdminLoginHistory other)
        {
            if(other ==null)
            {
                return -1;
            }
            return other.Id.CompareTo(Id);
        }
    }
}
