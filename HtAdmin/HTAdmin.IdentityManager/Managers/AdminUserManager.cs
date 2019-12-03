using HTAdmin.IdentityManager.Dtos;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminUserManager : UserManager<AdminUser, string>
    {
        public AdminUserManager(IUserStore<AdminUser, string> store)
            : base(store)
        {
        }
        public List<string> GetPermissionsByUserId(string userId)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var roleIds = ctx.UserRoles.Where(p => p.UserId == userId).Select(p => p.RoleId).ToList();
                int type = (int)RoleType.SuperAdmin;
                int count = ctx.Roles.Where(p => p.Type == RoleType.SuperAdmin && roleIds.Contains(p.Id)).Count();
                if (count > 0)
                {
                    return ctx.Permissions.Select(p => p.Id).ToList();
                }
                return ctx.RolePermissions.Where(p => roleIds.Contains(p.RoleId)).Select(p => p.PermissionId).ToList();
            }
        }

        public int AddUserAndUserRole(AdminUser user, string roleId)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var tran = ctx.Database.BeginTransaction();
                try
                {
                    ctx.Users.Add(user);
                    ctx.UserRoles.Add(new IdentityUserRole() { RoleId = roleId, UserId = user.Id });
                    int count = ctx.SaveChanges();
                    tran.Commit();
                    return count;
                }
                catch
                {
                    tran.Rollback();
                    return 0;
                }

            }
        }

        public int UpdateUserAndUserRole(AdminUserDto user)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var en = ctx.Users.FirstOrDefault(p => p.Id == user.Id);
                if (en != null)
                {
                    en.Name = user.Name;
                    en.PhoneNumber = user.Mobile;
                    en.PasswordHash = user.Password;
                    en.IsEnable = user.IsEnable;

                    var role = ctx.UserRoles.FirstOrDefault(p => p.UserId == user.Id);
                    role.RoleId = user.RoleIds.FirstOrDefault();
                    return ctx.SaveChanges();
                }
                else
                {
                    return 0;
                }
            }
        }

        public AdminUserDto GetAdminUserInfo(string userId)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var user = ctx.Users.Include(p => p.Roles).Where(p => p.Id == userId).FirstOrDefault();
                if (user != null)
                {
                    return new AdminUserDto()
                    {
                        Id = user.Id,
                        IsEnable = user.IsEnable,
                        Mobile = user.PhoneNumber,
                        Name = user.Name,
                        Password = user.PasswordHash,
                        RoleIds = user.Roles?.Select(p => p.RoleId).ToList()
                    };
                }
                return null;
            }
        }

        public PaginationResult<UserSearchListDto> GetAdminUsers(UserSearchAdditionDto addition)
        {
            PaginationResult<UserSearchListDto> result = new PaginationResult<UserSearchListDto>();
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var roleQuery = from r in ctx.Roles
                                join ur in ctx.UserRoles on r.Id equals ur.RoleId
                                where (string.IsNullOrEmpty(addition.RoleId) || r.Id == addition.RoleId)
                                select new
                                {
                                    r.Name,
                                    r.Id,
                                    UserId = ur == null ? "" : ur.UserId
                                };

                var query = from u in ctx.Users
                            join r in roleQuery on u.Id equals r.UserId
                            where (addition.IsEnable.HasValue == false || u.IsEnable == addition.IsEnable)
                            select new UserRoleDto
                            {
                                Id = u.Id,
                                Name = u.Name,
                                PhoneNumber = u.PhoneNumber,
                                UserName = u.UserName,
                                IsEnable = u.IsEnable,
                                CreationDate = u.CreationDate,
                                RoleName = r == null ? string.Empty : r.Name,
                                RoleId = r == null ? string.Empty : r.Id
                            };

                var loginQuery = ctx.LoginHistory.Where(p => query.Select(uid => uid.Id)
                                              .Contains(p.UserId))
                                              .GroupBy(p => p.UserId)
                                              .Select(p => p.Max(m => m.Id));

                var searchQuery = from u in query
                                  join uls in ctx.LoginHistory.Where(p => loginQuery.Contains(p.Id)) on u.Id equals uls.UserId
                                  into loginHistory
                                  from ul in loginHistory.DefaultIfEmpty()
                                  where (string.IsNullOrEmpty(addition.LoginIp) || (ul != null && ul.Ip == addition.LoginIp))
                                  && (string.IsNullOrEmpty(addition.Mobile) || u.PhoneNumber == addition.Mobile)
                                  && (string.IsNullOrEmpty(addition.RealName) || u.Name.Contains(addition.RealName))
                                  select new UserSearchListDto
                                  {
                                      UserId = u.Id,
                                      DisplayName = u.UserName,
                                      IpDisplayName = ul == null ? "" : ul.IpArea,
                                      LoginIp = ul == null ? "" : ul.Ip,
                                      IsEnable = u.IsEnable,
                                      LoginTime = ul == null ? DateTime.Now : ul.LoginTime,
                                      Mobile = u.PhoneNumber,
                                      RealName = u.Name,
                                      RoleName = u.RoleName,
                                      LoginName = u.UserName,
                                      CreationDate = u.CreationDate
                                  };

                result.TotalCount = searchQuery.Count();
                result.Data = searchQuery.OrderByDescending(p => p.CreationDate).Skip((addition.PageIndex - 1) * addition.PageSize)
                    .Take(addition.PageSize).ToList();
            };
            return result;
        }

        public int EnableUser(string userId, bool enabled)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var user = ctx.Users.FirstOrDefault(p => p.Id == userId);
                if (user != null)
                {
                    user.IsEnable = enabled;
                }
                else
                {
                    return 0;
                }
                return ctx.SaveChanges();
            }
        }

        public static AdminUserManager Create(IdentityFactoryOptions<AdminUserManager> options, IOwinContext context)
        {
            var manager = new AdminUserManager(new AdminUserStore(context.Get<ApplicationDbContext>()));
            // 配置用户名的验证逻辑
            manager.UserValidator = new UserValidator<AdminUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // 配置密码的验证逻辑
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // 配置用户锁定默认值
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // 注册双重身份验证提供程序。此应用程序使用手机和电子邮件作为接收用于验证用户的代码的一个步骤
            // 你可以编写自己的提供程序并将其插入到此处。
            manager.RegisterTwoFactorProvider("电话代码", new PhoneNumberTokenProvider<AdminUser>
            {
                MessageFormat = "你的安全代码是 {0}"
            });
            manager.RegisterTwoFactorProvider("电子邮件代码", new EmailTokenProvider<AdminUser>
            {
                Subject = "安全代码",
                BodyFormat = "你的安全代码是 {0}"
            });

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<AdminUser>(dataProtectionProvider.Create("HTAmin.Identity"));
            }
            return manager;
        }

        public int DeleteUser(string userId)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var user = ctx.Users.FirstOrDefault(p => p.Id == userId);
                ctx.Users.Remove(user);
                return ctx.SaveChanges();
            }
        }

        public List<AdminUser> GetAllAdminUsers()
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var users = ctx.Users.ToList();
                return users;
            }
        }
    }
}
