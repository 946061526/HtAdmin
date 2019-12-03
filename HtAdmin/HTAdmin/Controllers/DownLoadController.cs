using HTAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Controllers
{
    public class DownLoadController : BaseController
    {
        // GET: DownLoad
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult UploadPic()
        {
            var pic = Request.Files["uploadDemo"];

            return Json(new { success = true, msg = "成功" });
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetVersionUpdateInfo(FormCollection fc)
        {
            var pageIndex = string.IsNullOrEmpty(Request["page"]) ? base.PageIndex : int.Parse(Request["page"]) - 1;
            var pageSize = string.IsNullOrEmpty(Request["limit"]) ? base.PageSize : int.Parse(Request["limit"]);
            //var list =  this.ExternalClient.QueryVersionUpdateByPage()

            LayuiGridResult ret = new LayuiGridResult();
            ret.code = 0;
            ret.count = 0; //list.TotalCount;
            ret.data = "";// list.RecordList;
            return Json(ret);

        }
    }
}