using External.Core.Login;
using HTAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Filters
{
    /// <summary>
    /// 检查用户是否拥有执行权限
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CheckPermissionAttribute : FilterAttribute, IActionFilter
    {
        /// <summary>
        /// 功能列表Id
        /// </summary>
        public string FunctionId { get; set; }
        #region ===构造函数===
        public CheckPermissionAttribute(string functionId)
        {
            this.FunctionId = functionId;
        }
        public CheckPermissionAttribute()
        {

        }
        #endregion
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var currentUser = filterContext.HttpContext.Session["CurUser"];
            if (currentUser != null)
            {
                var user = currentUser as LoginInfoModel;
                if (!user.Permissions.Contains(this.FunctionId))
                {
                    filterContext.Result = new HttpUnauthorizedResult("对不起，您的权限不足！");
                }
            }
            else
            {
                filterContext.Result = new RedirectResult("/Account/Login");
            }
        }
    }
}