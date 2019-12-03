using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace WebHelper
{
    /// <summary>
    /// 继承cshtml的HTML方法
    /// </summary>
    public class HelperPage : System.Web.WebPages.HelperPage
    {
        // Workaround - exposes the MVC HtmlHelper instead of the normal helper
        public static new HtmlHelper Html
        {
            get { return ((System.Web.Mvc.WebViewPage)WebPageContext.Current.Page).Html; }
        }
    }
}