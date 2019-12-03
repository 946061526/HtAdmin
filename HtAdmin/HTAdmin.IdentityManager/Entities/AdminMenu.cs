using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    [Table("Menus")]
    public class AdminMenu
    {
        public AdminMenu()
        {
            IsEnable = true;
        }
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Descrition { get; set; }
        public virtual string Url { get; set; }
        public virtual string IconUrl { get; set; }
        public virtual bool IsEnable { get; set; }
        public virtual string ParentId { get; set; }
        public virtual string PermissionId { get; set; }
        public virtual int Sort { get; set; }
        public virtual DateTime CreationDate { get; set; }
    }
}
