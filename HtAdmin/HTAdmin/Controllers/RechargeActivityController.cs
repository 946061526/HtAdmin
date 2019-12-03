using HTAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Controllers
{
    public class RechargeActivityController : BaseController
    {
        // GET: RechargeActivity
        public ActionResult Index()
        {
            ViewBag.UpdateConfig = CheckRights("CZZSPZ_UDPATE");
            ViewBag.AddConfig = CheckRights("CZZSPZ_ADD");
            ViewBag.DeleteConfig = CheckRights("CZZSPZ_DELETE");
            return View();
        }

        public JsonResult GetRechargeConfigs(int PageIndex, int PageSize)
        {
            LayuiGridResult r = new LayuiGridResult()
            {
                code = 0
            };
            var list = GlobalCache.ActivityClient.GetRechargeConfigs(new Activity.Core.RechargeConfigCondition
            {
                PageIndex = PageIndex,
                PageSize = PageSize
            });
            r.count = list.TotalCount;
            r.data = list.Data;
            return Json(r);
        }

        public ActionResult OperateConfig(int configid)
        {
            bool edit = false;
            if (configid > 0)
            {
                ViewBag.Config = GlobalCache.ActivityClient.GetRechargeConfigById(configid);
                edit = true;
            }

            return View();
        }
    }
}