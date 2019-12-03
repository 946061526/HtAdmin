using GameBiz.Core;
using HTAdmin.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Controllers
{
    public class OrderController : BaseController
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 订单列表
        /// </summary>
        /// <returns></returns>
        [CheckPermission("JY100")]
        public ActionResult OrderList()
        {
            try
            {
                ViewBag.ckddxq = CheckRights("CKDDXQ100");
                //ViewBag.ckhyxq = CheckRights("CKHYXQ110");
                //ViewBag.ckqtdd = CheckRights("CKQTDD120");
                //ViewBag.dcsj = CheckRights("DCSJ130");

                //ViewBag.CKDDRS = CheckRights("CKDDRS");
                //ViewBag.CKDDTS = CheckRights("CKDDTS");
                //ViewBag.CKSJTZ = CheckRights("CKSJTZ");
                //ViewBag.CKSQJE = CheckRights("CKSQJE");
                //ViewBag.CKSHJE = CheckRights("CKSHJE");
                //ViewBag.CKJJE = CheckRights("CKJJE");
                //ViewBag.CKDDTZJE = CheckRights("CKDDTZJE");
                //ViewBag.CKDDRBJE = CheckRights("CKDDRBJE");

                ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request["pageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]);
                ViewBag.PageSize = string.IsNullOrWhiteSpace(Request["pageSize"]) ? base.PageSize : Convert.ToInt32(Request["pageSize"]);
                ViewBag.UserID = string.IsNullOrWhiteSpace(Request["UserID"]) ? "" : Request["UserID"];
                ViewBag.UserName = string.IsNullOrWhiteSpace(Request["UserName"]) ? "" : Request["UserName"];
                ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["StartTime"]) ? DateTime.Today : Convert.ToDateTime(Request["StartTime"]);
                ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["EndTime"]) ? DateTime.Today.AddDays(1).AddSeconds(-1) : Convert.ToDateTime(Request["EndTime"]);
                ViewBag.IssueNo = string.IsNullOrWhiteSpace(Request["IssueNo"]) ? "" : Request["IssueNo"];
                ViewBag.GameCode = string.IsNullOrWhiteSpace(Request["GameCode"]) ? "" : Request["GameCode"];
                ViewBag.IsAppend = string.IsNullOrWhiteSpace(Request["IsAppend"]) ? -1 : Convert.ToInt32(Request["IsAppend"]);

                //进度
                ProgressStatus? progressStatus = null;
                if (!string.IsNullOrWhiteSpace(Request["ProgressStatus"]))
                {
                    progressStatus = (ProgressStatus)Convert.ToInt32(Request["ProgressStatus"]);
                }
                ViewBag.ProgressStatus = progressStatus;

                var orderList = GlobalCache.QueryClient.QueryOrderList_Ht(ViewBag.UserID, ViewBag.UserName, ViewBag.StartTime, ViewBag.EndTime, ViewBag.GameCode, "", ViewBag.IssueNo, ViewBag.IsAppend, ViewBag.ProgressStatus, ViewBag.PageIndex - 1, ViewBag.PageSize);
                ViewBag.List = orderList;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderDetail()
        {
            return View();
        }
    }
}