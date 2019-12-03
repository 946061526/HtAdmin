using Common.Utilities;
using GameBiz.Core;
using HTAdmin.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Controllers
{
    public class LotteryController : BaseController
    {
        // GET: Lottery
        public ActionResult Index()
        {
            return View();
        }
        
        #region 队伍logo

        /// <summary>
        /// 查询首页比赛信息
        /// </summary>
        /// <returns></returns>
        [CheckPermission("BS200")]
        public ActionResult TeamLogoList()
        {
            ViewBag.BS200_XG = CheckRights("BS200_XG");

            ViewBag.TeamID = Request["TeamID"] == null ? string.Empty : Request["TeamID"].ToString();
            ViewBag.TeamName = Request["TeamName"] == null ? string.Empty : Request["TeamName"].ToString();
            ViewBag.HasImg = Request["HasImg"] == null ? "" : Request["HasImg"].ToString();
            ViewBag.PageIndex = Request["PageIndex"] == null ? base.PageIndex : Convert.ToInt32(Request["PageIndex"]);
            ViewBag.PageSize = Request["PageSize"] == null ? base.PageSize : Convert.ToInt32(Request["PageSize"]);
            ViewBag.IndexMatchList = GlobalCache.GameClient.QueryIndexMatchCollection(ViewBag.TeamID, ViewBag.HasImg, ViewBag.PageIndex - 1, ViewBag.PageSize, ViewBag.TeamName);
            
            ViewBag.ImgSite = UploadFileRequestUrl;
            return View();
        }
        /// <summary>
        /// 图标编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult TeamLogoEdit()
        {
            try
            {
                if (Request["Id"] == null)
                    ViewBag.IndexMatchInfo = null;
                var id = Request["Id"];
                ViewBag.IndexMatchInfo = GlobalCache.GameClient.QueryIndexMatchInfo(Convert.ToInt32(id));
                
                ViewBag.ImgSite = UploadFileRequestUrl;
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 图标编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult TeamLogoEditJson()
        {
            try
            {
                var id = PreconditionAssert.IsNotEmptyString(Request["id"], "编号不能为空");
                var imgPath = PreconditionAssert.IsNotEmptyString(Request["imgPath"], "图片url不能为空");
                var result = GlobalCache.GameClient.UpdateIndexMatch(Convert.ToInt32(id), imgPath);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        #endregion

        #region 彩种管理

        [CheckPermission("B103")]
        public ActionResult LotteryStatusManage()
        {
            ViewBag.bcjzzt = CheckRights("BCJZZT100");
            ViewBag.yjtzczzt = CheckRights("YJTZCZZT110");

            ViewBag.Game = GlobalCache.GameIssuseClient.GetLotteryGameList();
            
            ViewBag.ImgSite = UploadFileRequestUrl;
            return View();
        }

        public ActionResult LotteryStatusEdit()
        {
            var GameCode = Request["GameCode"];
            var Game = GlobalCache.GameIssuseClient.GetLotteryGameByGamecode(GameCode, "");
            ViewBag.Game = Game;
            
            ViewBag.ImgSite = UploadFileRequestUrl;
            return View();
        }
        public JsonResult LotteryStatusEditJson()
        {
            try
            {
                var GameCode = Request.Form["GameCode"];
                var PCStatus = int.Parse(Request.Form["PCStatus"]);
                var APPStatus = int.Parse(Request.Form["APPStatus"]);
                var WapStatus = int.Parse(Request.Form["WapStatus"]);
                var ImgPath = Request.Form["ImgPath"];
                var GameAppTxt = Request.Form["GameAppTxt"];
                var Status = int.Parse(Request.Form["Status"]);
                var Sort = int.Parse(Request.Form["Sort"]);
                var reslut = GlobalCache.GameIssuseClient.UpdateLotteryGame(CurrentUser1.UserToken, GameCode, Status, APPStatus, ImgPath, Sort, GameAppTxt, PCStatus, WapStatus);
                return Json(new { IsSuccess = reslut.IsSuccess, Msg = reslut.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }

        }
        public JsonResult LotteryStatusSort()
        {
            try
            {
                var GameCode1 = Request.Form["GameCode1"];
                var Sort1 = int.Parse(Request.Form["Sort1"]);
                var GameCode2 = Request.Form["GameCode2"];
                var Sort2 = int.Parse(Request.Form["Sort2"]);
                var reslut = GlobalCache.GameIssuseClient.UpdateLotteryGameSort(GameCode1, Sort1, GameCode2, Sort2);
                return Json(new { IsSuccess = reslut.IsSuccess, Msg = reslut.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        #endregion

        #region 竞技彩比赛管理

        [CheckPermission("B102")]
        public ActionResult MatchList()
        {
            ViewBag.jybs = CheckRights("JYBS100");

            ViewBag.PageIndex = Request["PageIndex"] == null ? base.PageIndex : Convert.ToInt32(Request["PageIndex"]);
            ViewBag.PageSize = Request["PageSize"] == null ? base.PageSize : Convert.ToInt32(Request["PageSize"]);
            var gameCode = string.IsNullOrEmpty(Request.QueryString["GameCode"]) ? "JCZQ" : Request.QueryString["GameCode"];
            ViewBag.GameCode = gameCode;
            switch (gameCode)
            {
                case "JCZQ":
                    ViewBag.JCZQMatch = GlobalCache.GameIssuseClient.QueryCurrentJCZQMatchInfo(CurrentUser1.UserToken);
                    //ViewBag.JCZQMatch = GlobalCache.GameIssuseClient.QueryCurrentJCZQMatchInfoByPage(0, 20);
                    ViewBag.JCLQMatch = new CoreJCLQMatchInfoCollection();
                    ViewBag.BJDCMatch = new CoreBJDCMatchInfoCollection();
                    break;
                case "JCLQ":
                    ViewBag.JCZQMatch = new CoreJCZQMatchInfoCollection();
                    ViewBag.BJDCMatch = new CoreBJDCMatchInfoCollection();
                    ViewBag.JCLQMatch = GlobalCache.GameIssuseClient.QueryCurrentJCLQMatchInfo(CurrentUser1.UserToken);
                    //ViewBag.JCLQMatch = GlobalCache.GameIssuseClient.QueryCurrentJCLQMatchInfoByPage(ViewBag.PageIndex - 1, ViewBag.PageSize);
                    break;
                case "BJDC":
                    ViewBag.JCZQMatch = new CoreJCZQMatchInfoCollection();
                    ViewBag.JCLQMatch = new CoreJCLQMatchInfoCollection();
                    ViewBag.BJDCMatch = GlobalCache.GameIssuseClient.QueryCurrentBJDCMatchInfo(CurrentUser1.UserToken);
                    //ViewBag.BJDCMatch = GlobalCache.GameIssuseClient.QueryCurrentBJDCMatchInfoByPage(ViewBag.PageIndex - 1, ViewBag.PageSize);
                    break;

            }

            return View();
        }
        /// <summary>
        /// 彩种玩法限制
        /// </summary>
        /// <returns></returns>
        public ActionResult MatchSet()
        {
            ViewBag.GameCode = Request["gameCode"].ToString();
            ViewBag.ID = Request["matchid"].ToString();
            if (ViewBag.GameCode == "JCZQ")
            {
                ViewBag.Match = GlobalCache.GameIssuseClient.GetJCZQMatchByID(ViewBag.ID);
            }
            else if (ViewBag.GameCode == "JCLQ")
            {
                ViewBag.Match = GlobalCache.GameIssuseClient.GetJCLQMatchByID(ViewBag.ID);
            }
            else if (ViewBag.GameCode == "BJDC")
            {
                ViewBag.Match = GlobalCache.GameIssuseClient.GetBJDCMatchByID(ViewBag.ID);
            }
            return View();
        }
        /// <summary>
        /// 彩种玩法限制
        /// </summary>
        /// <returns></returns>
        public JsonResult MatchSetJson()
        {
            try
            {
                var gameCode = Request["GameCode"] == null ? "" : Request["GameCode"].ToString();
                var matchId = Request["matchId"] == null ? "" : Request["matchId"].ToString();
                if (string.IsNullOrEmpty(gameCode) || string.IsNullOrEmpty(matchId))
                {
                    throw new Exception("参数错误");
                }
                var privilegesType = Request["privilegesType"] == null ? "" : Request["privilegesType"].ToString();
                var status = Request["status"] == null ? 1 : Convert.ToInt32(Request["status"]);
                switch (gameCode)
                {
                    case "JCZQ":
                        GlobalCache.GameIssuseClient.UpdateJCZQMatchInfo(matchId, privilegesType, status);
                        break;
                    case "JCLQ":
                        GlobalCache.GameIssuseClient.UpdateJCLQMatchInfo(matchId, privilegesType, status);
                        break;
                    case "BJDC":
                        GlobalCache.GameIssuseClient.UpdateBJDCMatchInfo(matchId, privilegesType, status);
                        break;
                    default:
                        break;
                }

                return Json(new { IsSuccess = true, Msg = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 生成奖期数据

        /// <summary>
        /// 生成奖期数据
        /// </summary>
        public ActionResult PrizePeriodList()
        {
            //ViewBag.MaxIssuse = GlobalCache.GameIssuseClient.QueryMaxGameIssuse();
            ViewBag.MaxIssuse = GlobalCache.GameIssuseClient.QueryGameMaxIssuseInfo();
            return View();
        }
        /// <summary>
        /// 生成奖期数据
        /// </summary>
        public ActionResult PrizePeriodCreate()
        {
            ViewBag.GameCode = Request["GameCode"];
            return View();
        }
        /// <summary>
        /// 生成奖期数据
        /// </summary>
        public JsonResult PrizePeriodCreateJson()
        {
            try
            {
                var gameCode = PreconditionAssert.IsNotEmptyString(Request.Form["gameCode"], "彩种不能为空！");
                var startTime = PreconditionAssert.IsNotEmptyString(Request.Form["startTime"], "开始时间不能为空！");
                var endTime = PreconditionAssert.IsNotEmptyString(Request.Form["endTime"], "停止时间不能为空！");
                if (new string[] { "CQSSC", "JX11X5", "JSKS", "SD11X5", "GD11X5", "GDKLSF", "SDKLPK3" }.Any(t => t == gameCode))//高频彩
                {
                    var openPrizePeriodResult = GlobalCache.GameClient.OpenIssuseBatch_Fast(gameCode, Convert.ToDateTime(startTime), Convert.ToDateTime(endTime));
                    return Json(new { IsSuccess = openPrizePeriodResult.IsSuccess, Msg = openPrizePeriodResult.Message });
                }
                else if (gameCode == "BJPK10")//北京PK10
                {
                    var startIssue = Request["startIssue"] == null ? 0 : Convert.ToInt64(Request["startIssue"]);
                    if (startIssue < 1)
                        throw new Exception("期号不正确");
                    
                    var openPrizePeriodResult = GlobalCache.GameClient.OpenIssuseBatch_Fast_2(gameCode, Convert.ToDateTime(startTime), Convert.ToDateTime(endTime), startIssue);
                    return Json(new { IsSuccess = openPrizePeriodResult.IsSuccess, Msg = openPrizePeriodResult.Message });
                }
                else if (gameCode == "SSQ" || gameCode == "DLT")//低频彩
                {
                    var openPrizePeriodResult = GlobalCache.GameClient.OpenIssuseBatch_Slow(gameCode, Convert.ToInt32(startTime), Convert.ToInt32(endTime));
                    return Json(new { IsSuccess = openPrizePeriodResult.IsSuccess, Msg = openPrizePeriodResult.Message });
                }
                else if (gameCode == "FC3D" || gameCode == "PL3")//每日
                {
                    var openPrizePeriodResult = GlobalCache.GameClient.OpenIssuseBatch_Daily(gameCode, Convert.ToInt32(startTime), Convert.ToInt32(endTime));
                    return Json(new { IsSuccess = openPrizePeriodResult.IsSuccess, Msg = openPrizePeriodResult.Message });
                }
                return Json(new { IsSuccess = false, Msg = "提交失败" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        #endregion
        
    }
}