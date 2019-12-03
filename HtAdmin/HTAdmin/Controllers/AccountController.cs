using Common.Net;
using Common.Utilities;
using External.Core.Login;
using HTAdmin.Filters;
using HTAdmin.IdentityManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using HTAdmin.Models;

namespace HTAdmin.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private AdminSignInManager _signInManager;
        private AdminUserManager _userManager;
        public AdminSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<AdminSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public AdminUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AdminUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        public AccountController()
        {
        }

        public AccountController(AdminUserManager userManager, AdminSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (Request["returnUrl"] != null)
                    return Redirect(Request["returnUrl"]);
                else {
                    return Redirect("/account/login");
                }
            }
            ViewBag.ReturnUrl = Request["returnUrl"] ?? "";
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> LoginFunction(LoginViewModel model, string returnUrl)
        {
            CommonResult r = new CommonResult();
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.Select(p => p.Errors);
                    var list = errors.Select(p =>
                    {
                        return string.Join(",", p.Select(e => e.ErrorMessage));
                    });
                    r.code = (int)ResultCodeEnum.VerifyError;
                    r.message = string.Join(",", list);
                    return Json(r);
                }

                string value = GetCookieValue("ValidateCodeCount");
                int i = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(value);
                if (i >= 3)
                {
                    throw new Exception("验证码连续输错3次,请一个小时后在登录！");
                }
                if (Session["ValidateCode"] == null ||Session["ValidateCode"].ToString() != model.VerifyCode)
                {
                    ModCookies("ValidateCodeCount", (++i).ToString());
                    throw new Exception("验证码输入错误！");
                }
                else
                {
                    DelCookeis("ValidateCodeCount");
                }

                // 这不会计入到为执行帐户锁定而统计的登录失败次数中
                // 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        try
                        {
                            var ipAddr = IpManager.IPAddress;
                            var user = UserManager.FindByName(model.UserName);
                            Task.Factory.StartNew(() =>
                            {
                                var ip = IpManager.GetIpDisplayname_Amap(ipAddr,null);
                                new AdminLoginHistoryManager().AddLoginHistory(new AdminLoginHistory()
                                {
                                    CreationDate = DateTime.Now,
                                    Ip = ipAddr,
                                    IpArea = ip == null ? "Unknown" : ip.ToString(),
                                    LoginTime = DateTime.Now,
                                    UserId = user.Id
                                });
                            });
                        }
                        catch
                        {

                        }
                        return Json(r);
                    case SignInStatus.LockedOut:
                        r.code = (int)ResultCodeEnum.UserLockout;
                        r.message = "账户被锁";
                        return Json(r);
                    case SignInStatus.RequiresVerification:
                        r.code = (int)ResultCodeEnum.VerifyError;
                        r.message = "账户要求验证";
                        return Json(r);
                    case SignInStatus.Failure:
                    default:
                        r.code = (int)ResultCodeEnum.SystemError;
                        r.message = "账号或密码不对";
                        return Json(r);
                }
            }
            catch (Exception ex)
            {
                r.code = (int)ResultCodeEnum.SystemError;
                r.message = ex.Message;
                return Json(r);
            }
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session["CurUser"] = null;
            return Redirect("/account/login?returnUrl=/");
        }
        //[AllowAnonymous]
        //public async Task<ActionResult> Register()
        //{
        //    try
        //    {
        //        var user = new AdminUser
        //        {
        //            UserName = "13616953637",
        //            Email = "13616953637@qianbaiwan.com",
        //            UpdatorUserId = "",
        //            CreatorUserId = "",
        //            PhoneNumber = "13616953637",
        //            Name = "Luffy",
        //            Type = RoleType.SuperAdmin,
        //            IsEnable = true,
        //            PasswordHash = new PasswordHasher().HashPassword("admin123"),
        //            CreationDate = DateTime.Now,
        //            UpdateDate = DateTime.Now
        //        };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //            // 有关如何启用帐户确认和密码重置的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkID=320771
        //            // 发送包含此链接的电子邮件
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "确认你的帐户", "请通过单击 <a href=\"" + callbackUrl + "\">這裏</a>来确认你的帐户");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        // 如果我们进行到这一步时某个地方出错，则重新显示表单
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>  
        /// 添加一个Cookie  
        /// </summary>  
        /// <param name="cookiename">cookie名</param>  
        /// <param name="cookievalue">cookie值</param>  
        /// <param name="expires">过期时间 DateTime</param>  
        private void SetCookie(string cookiename, string cookievalue, DateTime expires)
        {
            HttpCookie cookie = new HttpCookie(cookiename)
            {
                Value = cookievalue,
                Expires = expires
            };
            Response.Cookies.Add(cookie);
        }

        /// <summary>  
        /// 获取指定Cookie值  
        /// </summary>  
        /// <param name="cookiename">cookiename</param>  
        /// <returns></returns>  
        public string GetCookieValue(string cookiename)
        {
            string str = string.Empty;
            try
            {
                HttpCookie cookie = Request.Cookies[cookiename];
                if (cookie != null)
                {
                    str = cookie.Value;
                }
                return str;
            }
            catch
            {

                return str;
            }
        }

        ///<summary>
        /// 修改cookies
        ///</summary>
        public void ModCookies(string cookiename, string value)
        {
            HttpCookie cookies = Request.Cookies[cookiename];
            if (cookies == null)
            {
                SetCookie(cookiename, value, DateTime.Now.AddHours(1));
            }
            else
            {
                cookies.Value = value;
                cookies.Expires = DateTime.Now.AddHours(1);
                Response.Cookies.Add(cookies);
            }

        }

        ///<summary>
        /// 删除cookies
        ///</summary>
        public void DelCookeis(string cookiename)
        {
            if (Request.Cookies[cookiename] != null)
            {
                HttpCookie cookies = new HttpCookie(cookiename);
                cookies.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookies);
                Request.Cookies.Remove(cookiename);
            }
        }
        [AllowAnonymous]
        public ValidateCodeGenerator CreateValidateCode()
        {
            var num = 0;
            string randomText = SelectRandomNumber(5, out num);
            Session["ValidateCode"] = num;
            ValidateCodeGenerator vlimg = new ValidateCodeGenerator()
            {
                BackGroundColor = Color.FromKnownColor(KnownColor.LightGray),
                RandomWord = randomText,
                ImageHeight = 25,
                ImageWidth = 100,
                fontSize = 14,
            };
            return vlimg;
        }

        //选择随机数字
        private string SelectRandomNumber(int numberOfChars, out int num)
        {
            num = 0;
            StringBuilder randomBuilder = new StringBuilder();
            Random randomSeed = new Random();
            char[] columns = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int incr = 0; incr < numberOfChars; incr++)
            {
                if (incr == 0 || incr == 2)
                {
                    var randomNum = columns[randomSeed.Next(10)].ToString();
                    randomBuilder.Append(randomNum);//取26个字符里的任意一个
                    num += int.Parse(randomNum);
                }
                if (incr == 1)
                {
                    randomBuilder.Append("+").ToString();
                }
                if (incr == 3)
                {
                    randomBuilder.Append("=").ToString();
                }
                if (incr == 4)
                {
                    randomBuilder.Append("?").ToString();
                }
            }
            return randomBuilder.ToString();
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            ViewBag.UserId = User.Identity.GetUserId();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            CommonResult r = new CommonResult();
            try
            {
                PreconditionAssert.IsNotEmptyString(userId, "UserId不能为空");
                PreconditionAssert.IsNotEmptyString(oldPassword, "旧密码不能为空");
                PreconditionAssert.IsNotEmptyString(newPassword, "新密码不能为空");

                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), oldPassword, newPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return Json(r);
                }
                else
                {
                    r.code = (int)ResultCodeEnum.PasswordError;
                    r.message = string.Join(",", result.Errors);
                }

                return Json(r);
            }
            catch (Exception ex)
            {
                r.code = (int)ResultCodeEnum.SystemError;
                r.message = ex.Message;
                return Json(r);
            }
        }
    }
}