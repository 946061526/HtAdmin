using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public enum RoleType
    {
        /// <summary>
        /// 系统管理员
        /// </summary>
        SystemAdmin = 0,
        /// <summary>
        /// 超级管理员拥有所有权限
        /// </summary>
        SuperAdmin = 100
    }
}
