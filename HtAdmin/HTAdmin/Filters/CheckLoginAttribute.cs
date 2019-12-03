using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class CheckLoginAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["CurrentUser"] == null )
            {
                //string urlHost = ConfigurationManager.AppSettings["ThisLocal_Url"]; //由于反向代理的原因，不能使用HttpContext.Current.Request.Url;
                string urlRote = filterContext.HttpContext.Request.RawUrl.ToString();
                filterContext.Result = new RedirectResult(string.Format("/Account/Login?returnUrl={0}", urlRote));//filterContext.HttpContext.Server.UrlEncode(urlHost + 
            }
        }
    }
}