using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager
{
    public class AdminSignInManager : SignInManager<AdminUser, string>
    {
        public AdminSignInManager(AdminUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AdminUser user)
        {
            return user.GenerateUserIdentityAsync((AdminUserManager)UserManager);
        }

        public static AdminSignInManager Create(IdentityFactoryOptions<AdminSignInManager> options, IOwinContext context)
        {
            return new AdminSignInManager(context.GetUserManager<AdminUserManager>(), context.Authentication);
        }
    }
}
