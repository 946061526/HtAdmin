using HTAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Controllers
{
    public class MessageController : BaseController
    {

        #region 手机短信

        /// <summary>
        /// 短信发送记录
        /// </summary>
        /// <returns></returns>
        public ActionResult SmsMessageList()
        {
            ViewBag.UserId = Request["userId"] ?? "";
            ViewBag.MobileNumber = Request["mobileNumber"] ?? "";
            ViewBag.Status = Request["status"] ?? "";
            ViewBag.Type = Request["type"] ?? "1";

            ViewBag.StartTime = string.IsNullOrEmpty(Request["startTime"]) ? DateTime.Today : Convert.ToDateTime(Request["startTime"]);
            ViewBag.EndTime = string.IsNullOrEmpty(Request["endTime"]) ? DateTime.Today.AddDays(1).AddSeconds(-1) : Convert.ToDateTime(Request["endTime"]);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);

            ViewBag.List = base.QueryClient.QueryMobileMessageInfoList(
                ViewBag.UserId
                , ViewBag.MobileNumber
                , ViewBag.StartTime
                , ViewBag.EndTime
                , ViewBag.Status
                , ViewBag.Type
                , Convert.ToInt32(ViewBag.PageIndex) - 1
                , ViewBag.PageSize);

            return View();
        }

        /// <summary>
        /// 短信重发
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendSmsAgain()
        {
            var userId = Request["userId"];
            var mobileNumber = Request["mobileNumber"];
            var content = Request["content"];
            return SendSmsMsg(userId, mobileNumber, content);
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private JsonResult SendSmsMsg(string userId, string mobile, string content)
        {
            try
            {
                var result = base.QueryClient.SendSMS(mobile, content, userId);

                //记录日志
                OperLogManager.AddOperationLog(this.CurUser.UserId, "发短信", string.Format("操作员【{0}】接收手机号【{1}】发送内容【{2}】", this.CurUser.UserId, Request["mobileNumber"], content), Request["userId"]);

                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <returns></returns>
        public ActionResult SmsMessageSend()
        {
            if (!CheckRights("U105"))
                throw new Exception("对不起，您的权限不足！");
            //bool plfsdx = false;
            //if (CheckRights("PLFSDX100"))
            //    plfsdx = true;
            //ViewBag.plfsdx = plfsdx;
            //ViewBag.FSZWYH = CheckRights("FSZWYH");
            return View();
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SmsMessageSendJson()
        {
            var mobile = Request["mobile"];
            var content = Request["content"];
            return SendSmsMsg("", mobile, content);
        }

        /// <summary>
        /// 查询最后一条未验证短信验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult SmsMessageUnValidateQuery()
        {
            return View();
        }

        /// <summary>
        /// 查询最后一条未验证短信验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SmsMessageUnValidateQueryJson()
        {
            try
            {
                var mobile = Request["mobile"] ?? "";
                string res = base.QueryClient.QueryLastOneUnValidateSms(mobile);
                return Json(new { IsSuccess = true, Data = res });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 语音信息
        /// </summary>
        /// <returns></returns>
        public ActionResult VoiceMessageList()
        {
            ViewBag.UserId = Request["userId"];
            ViewBag.MobileNumber = Request["mobileNumber"];
            ViewBag.Status = Request["status"]; //发送状态 0:发送成功，-1:发送失败
            ViewBag.SendCode = Request["sendCode"]; //接通状态 接通状态的code,外呼成功为1，外呼失败为0 

            ViewBag.StartTime = string.IsNullOrEmpty(Request["startTime"]) ? DateTime.Today : Convert.ToDateTime(Request["startTime"]);
            ViewBag.EndTime = string.IsNullOrEmpty(Request["endTime"]) ? DateTime.Today.AddDays(1).AddSeconds(-1) : Convert.ToDateTime(Request["endTime"]);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);

            ViewBag.List = base.QueryClient.QueryVoiceSendRecordList(
                ViewBag.UserId
                , ViewBag.MobileNumber
                , ViewBag.StartTime
                , ViewBag.EndTime
                , ViewBag.Status
                , ViewBag.SendCode
                , Convert.ToInt32(ViewBag.PageIndex) - 1
                , ViewBag.PageSize);

            // ExternalClient.AddSysOperationLog(Request["userId"], this.CurrentUser.UserId, "查询语音发送记录", string.Format("操作员【{0}】查询手机号【{1}】的查询语音发送记录", this.CurrentUser.UserId, Request["mobileNumber"]));

            return View();
        }
        [HttpPost]
        public JsonResult SendVoiceMessage()
        {
            try
            {
                var userId = Request["userId"];
                var mobileNumber = Request["mobileNumber"];
                var content = Request["content"];
                var result = base.QueryClient.SendVoice(mobileNumber, content, userId);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        #endregion

        #region 消息管理

        /// <summary>
        /// 消息管理列表
        /// </summary>
        /// <returns></returns>
        public ActionResult MessageList()
        {
            ViewBag.Title = Request["Title"] ?? "";
            ViewBag.IsMsg = Request["IsMsg"] ?? "-1";
            ViewBag.IsPush = Request["IsPush"] ?? "-1";

            ViewBag.StartTime = null;
            if (Request["StartTime"] != null && !string.IsNullOrWhiteSpace(Request["StartTime"].ToString()))
                ViewBag.StartTime = Convert.ToDateTime(Request["StartTime"]);
            ViewBag.EndTime = null;
            if (Request["EndTime"] != null && !string.IsNullOrWhiteSpace(Request["EndTime"].ToString()))
                ViewBag.EndTime = Convert.ToDateTime(Request["EndTime"]);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);

            //var endTime = Utility.GetDateTime24(Convert.ToDateTime(ViewBag.EndTime));

            ViewBag.List = base.ExternalClient.QueryMessageInfoList(
                ViewBag.Title
                , ViewBag.StartTime
                , ViewBag.EndTime
                , -1
                , Convert.ToInt32(ViewBag.IsPush)
                , Convert.ToInt32(ViewBag.IsMsg)
                , Convert.ToInt32(ViewBag.PageIndex) - 1
                , ViewBag.PageSize);

            return View();
        }

        //public JsonResult GetMessageList()
        //{
        //    LayuiGridResult result = new LayuiGridResult() { code = 0 };
        //    try
        //    {
        //        var Title = Request["Title"] ?? "";
        //        var IsMsg = Request["IsMsg"] ?? "-1";
        //        var IsPush = Request["IsPush"] ?? "-1";

        //        var StartTime = string.IsNullOrEmpty(Request["StartTime"]) ? DateTime.Today : Convert.ToDateTime(Request["StartTime"]);
        //        var EndTime = string.IsNullOrEmpty(Request["EndTime"]) ? DateTime.Today.AddDays(1).AddSeconds(-1) : Convert.ToDateTime(Request["EndTime"]);
        //        var PageIndex = string.IsNullOrEmpty(Request["PageIndex"]) ? base.PageIndex : int.Parse(Request["PageIndex"]);
        //        var PageSize = string.IsNullOrEmpty(Request["PageSize"]) ? base.PageSize : int.Parse(Request["PageSize"]);

        //        var list = base.ExternalClient.QueryMessageInfoList(Title, StartTime, EndTime, -1, Convert.ToInt32(IsPush), Convert.ToInt32(IsMsg), Convert.ToInt32(PageIndex) - 1, PageSize);

        //        result.count = list.TotalCount;
        //        int i = ((int)PageIndex - 1) * (int)PageSize + 1;
        //        if (list.TotalCount > 0)
        //        {
        //            result.data = list.DataList.Select(item =>
        //            {
        //                return new
        //                {
        //                    Index = i++,
        //                    CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm"),
        //                    Title = item.Title.Length > 20 ? item.Title.Substring(0, 19) + "..." : item.Title,
        //                    Sender = item.Sender,
        //                    IsMsg = item.IsMsg,
        //                    IsPush = item.IsPush,
        //                    Type = item.Type,
        //                    PushCount = item.PushCount,
        //                    PushSuccessCount = item.PushSuccessCount,
        //                    PushOpenCount = item.PushOpenCount
        //                };
        //            }).ToArray();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.code = 1;
        //        result.msg = ex.Message;
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);

        //}

        /// <summary>
        /// 发送消息、推送
        /// </summary>
        /// <returns></returns>
        public ActionResult MessageAdd()
        {
            return View();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MessageAddJson()
        {
            try
            {
                string ActivityTitle = Request["Title"] ?? "";
                string ActivitySummary = Request["Content"] ?? "";
                string LinkUrl = Request["LinkUrl"] ?? "";
                string ImageUrl = Request["ImgUrl"] ?? "";
                int AppJumpType = LinkUrl == "" ? 0 : 1;
                int IsPush = Request["IsPush"] == null ? 0 : Convert.ToInt32(Request["IsPush"]);
                string Sender = CurUser.UserName;
                string AppLinkUrl = "";
                //int ActivityStatu = 1;
                DateTime BeginTime = DateTime.Today;
                DateTime EndTime = DateTime.MaxValue;
                DateTime CreateTime = DateTime.Now;
                DateTime AppPushTime = DateTime.Now;
                int ActivitySort = 1;

                var result = ExternalClient.AddActivityMessage(ActivityTitle, ActivitySummary, LinkUrl, ImageUrl, AppJumpType, AppLinkUrl, External.Core.ActivityStatus.Enable, BeginTime, EndTime, CreateTime, AppPushTime, ActivitySort, IsPush, Sender);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 推送记录
        /// </summary>
        /// <returns></returns>
        public ActionResult PushList()
        {
            //if (!CheckRights("H137"))
            //    throw new Exception("对不起，您的权限不足！");

            ViewBag.Title = Request["Title"] ?? "";
            ViewBag.IsMsg = (Request["IsMsg"] == null || string.IsNullOrWhiteSpace(Request["IsMsg"].ToString())) ? -1 : Convert.ToInt32(Request["IsMsg"]);
            ViewBag.StartTime = null;
            if (Request["StartTime"] != null && !string.IsNullOrWhiteSpace(Request["StartTime"].ToString()))
                ViewBag.StartTime = Convert.ToDateTime(Request["StartTime"]);
            ViewBag.EndTime = null;
            if (Request["EndTime"] != null && !string.IsNullOrWhiteSpace(Request["EndTime"].ToString()))
                ViewBag.EndTime = Convert.ToDateTime(Request["EndTime"]);

            GameBiz.Core.PushType? pushType = null;
            if (Request["PushType"] != null && !string.IsNullOrWhiteSpace(Request["PushType"].ToString()))
                pushType = (GameBiz.Core.PushType)(Convert.ToInt32(Request["PushType"].ToString()));

            ViewBag.PushType = pushType;
            ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request["PageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["PageIndex"]);
            ViewBag.PageSize = string.IsNullOrWhiteSpace(Request["PageSize"]) ? base.PageSize : Convert.ToInt32(Request["PageSize"]);

            ViewBag.List = base.QueryClient.QueryPushRecordList((ViewBag.PageIndex - 1), ViewBag.PageSize, ViewBag.StartTime, ViewBag.EndTime, ViewBag.Title, ViewBag.IsMsg, ViewBag.PushType);
            return View();
        }

        /// <summary>
        /// 发送推送
        /// </summary>
        /// <returns></returns>
        public ActionResult MessagePush()
        {
            return View();
        }
        /// <summary>
        /// 发送推送
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MessagePushJson()
        {
            return PushMessageToUsers();
        }
        /// <summary>
        /// 发送推送
        /// </summary>
        /// <returns></returns>
        private JsonResult PushMessageToUsers()
        {
            try
            {
                GameBiz.Core.PushRecordInfo info = new GameBiz.Core.PushRecordInfo();
                info.Title = Request["PushTitle"] ?? "";
                info.Content = Request["PushContent"] ?? "";
                info.Url = Request["PushLinkUrl"] ?? "";
                info.Activity = Request["Activity"] ?? "";
                string AfterOpenType = Request["AfterOpenType"] ?? "1";
                switch (AfterOpenType)
                {
                    case "1": info.AfterOpenType = GameBiz.Core.AfterOpenType.GoUrl; break;
                    case "2": info.AfterOpenType = GameBiz.Core.AfterOpenType.GoActivity; break;
                    case "3": info.AfterOpenType = GameBiz.Core.AfterOpenType.GoCustom; break;
                    case "4": info.AfterOpenType = GameBiz.Core.AfterOpenType.GoIndexPage; break;
                    default: info.AfterOpenType = GameBiz.Core.AfterOpenType.GoApp; break;
                }
                info.PushType = GameBiz.Core.PushType.Broadcast;
                info.IsMsg = Request["IsMsg"] == null ? 0 : Convert.ToInt32(Request["IsMsg"]);
                info.Sender = CurUser.UserName;

                var result = base.GameClient.PushMessageToUsers(info.PushType, info.AfterOpenType, info.Title, info.Content, info.Url, info.Activity, info.Custom, info.IsMsg, info.Sender);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        #endregion
    }
}