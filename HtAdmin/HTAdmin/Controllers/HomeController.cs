using Common.Net;
using External.Core.AdminMenu;
using HTAdmin.IdentityManager;
using HTAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Redirect("/account/login");
            }
            ViewBag.UserName = CurUser.UserName;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        #region 菜单
        public PartialViewResult MenuLeft()
        {
            try
            {
                var menu = GetMenuCollection();
                ViewBag.User = CurUser;
                return PartialView("MenuLeft", menu);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<AdminMenuModel> GetMenuCollection()
        {
            if (Session["MyMenuCollection"] == null)
            {
                var collection = new AdminMenuManager().GetMenusByUserId(CurUser.UserId);// GlobalCache.ExternalClient.QueryMyMenuCollection(CurrentUser.UserToken);
                Session["MyMenuCollection"] = collection.Select(p =>
                {
                    return new AdminMenuModel()
                    {
                        Id = p.Id,
                        Url = p.Url,
                        Sort = p.Sort,
                        Descrition = p.Descrition,
                        IconUrl = p.IconUrl,
                        IsEnable = p.IsEnable,
                        Name = p.Name,
                        ParentId = p.ParentId,
                        PermissionId = p.PermissionId
                    };
                }).ToList();
            }
            return Session["MyMenuCollection"] as List<AdminMenuModel>;
        }

        #endregion
    }
}