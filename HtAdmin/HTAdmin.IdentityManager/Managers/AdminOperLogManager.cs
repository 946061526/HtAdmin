using HTAdmin.IdentityManager.Dtos;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminOperLogManager:IDisposable
    {
        public void AddOperationLog(AdminOperationLog log)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                ctx.OperationLogs.Add(log);
                ctx.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operUserId">操作者的用户Id</param>
        /// <param name="operName">操作的名称，一般是对应的菜单</param>
        /// <param name="description">才做描述</param>
        /// <param name="userId">可空，被操作者的userId</param>
        public void AddOperationLog(string operUserId, string operName, string description, string userId = "")
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                AdminOperationLog log = new AdminOperationLog()
                {
                    CreationDate = DateTime.Now,
                    Description = description,
                    OperationName = operName,
                    OperUserId = operUserId,
                    UserId = userId
                };
                ctx.OperationLogs.Add(log);
                ctx.SaveChanges();
            }
        }

        public static AdminOperLogManager Create(IdentityFactoryOptions<AdminOperLogManager> options, IOwinContext context)
        {
            var manager = new AdminOperLogManager();
            return manager;
        }

        public PaginationResult<SystemOperationLog> GetAdminOperationLogList(DateTime? startTime,DateTime? endTime,string Operator,string desc,string view, int pageIndex, int pageSize)
        {
            PaginationResult<SystemOperationLog> res = new PaginationResult<SystemOperationLog>();
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var query = (from log in ctx.OperationLogs
                             join ue in ctx.Users on log.OperUserId equals ue.Id
                             join ur in ctx.UserRoles on ue.Id equals ur.UserId
                             join ro in ctx.Roles on ur.RoleId equals ro.Id
                             where (startTime == null || log.CreationDate >= startTime ) && (endTime == null || log.CreationDate <= endTime) 
                             && (Operator == null || log.OperUserId == Operator)
                             && (desc == null || log.Description.Contains(desc))
                             && (view == null || log.OperationName.Contains(view))
                             orderby ue.CreationDate descending
                             select new
                             {
                                 log.CreationDate,
                                 ue.Name,
                                 RoleName = ro.Name,
                                 log.Description,
                                 log.OperationName
                             }).Skip(pageIndex * pageSize).Take(pageSize);
                if (query != null)
                {
                    int i = 1;
                    List<SystemOperationLog> list = new List<SystemOperationLog>();
                    foreach (var item in query)
                    {
                        
                        list.Add(new SystemOperationLog
                        {
                            Id = i++,
                            CreationDate = item.CreationDate.ToString("yyyy-MM-dd HH:ss:mm"),
                            Description = item.Description,
                            Name = item.Name,
                            OperationName = item.OperationName,
                            RoleName = item.RoleName,
                        });
                    }
                    res.TotalCount = query.Count();
                    res.Data = list;
                }
                return res;
            }
        }

        public void Dispose()
        {

        }
    }
}
