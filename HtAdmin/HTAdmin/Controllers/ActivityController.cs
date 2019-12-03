using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Activity.Core;
using Common.Utilities;
using GameBiz.Core;
using HTAdmin.Models;

namespace HTAdmin.Controllers
{
    public class ActivityController : BaseController
    {
        // GET: Activity
        public ActionResult Index()
        {
            return View();
        }

        #region 活动红包相关

        /// <summary>
        /// 红包模板列表
        /// </summary>
        /// <returns></returns>
        public ActionResult RedBagCouponTemplateManager()
        {
            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Today.AddMonths(-1) : Convert.ToDateTime(Request["startTime"]);
            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Today.AddDays(1).AddSeconds(-1) : Convert.ToDateTime(Request["endTime"]);

            ViewBag.TagList = FundClient.GetRedBagCouponTemplateTags().ReturnValue.Split(',');
            ViewBag.sel_RedBagTemTags = string.IsNullOrWhiteSpace(Request["sel_RedBagTemTags"]) ? "" : Request["sel_RedBagTemTags"].ToString();

            ViewBag.txt_couponName = string.IsNullOrWhiteSpace(Request["txt_couponName"]) ? "" : Request["txt_couponName"].ToString();
            ViewBag.txt_RedBagCopTemId = string.IsNullOrWhiteSpace(Request["txt_RedBagCopTemId"]) ? "" : Request["txt_RedBagCopTemId"].ToString();
            ViewBag.txt_couponMoney = string.IsNullOrWhiteSpace(Request["txt_couponMoney"]) ? "" : Request["txt_couponMoney"].ToString();
            ViewBag.txt_couponFillMoney = string.IsNullOrWhiteSpace(Request["txt_couponFillMoney"]) ? "" : Request["txt_couponFillMoney"].ToString();
            ViewBag.sel_RedBagTemStatus = string.IsNullOrWhiteSpace(Request["sel_RedBagTemStatus"]) ? "" : Request["sel_RedBagTemStatus"].ToString();

            ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request["pageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrWhiteSpace(Request["pageSize"]) ? base.PageSize : Convert.ToInt32(Request["pageSize"]);

            ViewBag.List = this.QueryClient.QueryRedBagCouponTemplateListAdmin(
                string.IsNullOrEmpty(ViewBag.txt_RedBagCopTemId) ? 0 : Convert.ToInt32(ViewBag.txt_RedBagCopTemId),
                ViewBag.txt_couponName,
                string.IsNullOrEmpty(ViewBag.txt_couponMoney) ? 0 : Convert.ToDecimal(ViewBag.txt_couponMoney),
                string.IsNullOrEmpty(ViewBag.txt_couponFillMoney) ? -1 : Convert.ToDecimal(ViewBag.txt_couponFillMoney),
                ViewBag.sel_RedBagTemTags,
                string.IsNullOrEmpty(ViewBag.sel_RedBagTemStatus) ? CouponTemplateStatus.All : ((CouponTemplateStatus)Convert.ToInt32(ViewBag.sel_RedBagTemStatus)),
                Convert.ToDateTime(ViewBag.StartTime),
                Convert.ToDateTime(ViewBag.EndTime),
                Convert.ToInt32(ViewBag.PageIndex) - 1,
                ViewBag.PageSize);
            return View();
        }

        /// <summary>
        /// 获取红包标签
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRedBagTemTags()
        {
            var TagList = FundClient.GetRedBagCouponTemplateTags().ReturnValue.Split(',');
            return Json(new { IsSuccess = true, Data = TagList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增模板页
        /// </summary>
        /// <returns></returns>
        public ActionResult AddNewRedBagCouponTemplate()
        {
            return View();
        }

        /// <summary>
        /// 新增红包模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddNewRedBagConTem()
        {
            try
            {
                var RedBagCouponName = string.IsNullOrWhiteSpace(Request["txt_couponName"]) ? "" : Request["txt_couponName"].ToString(); //红包名称
                var CouponMoney = string.IsNullOrWhiteSpace(Request["txt_couponMoney"]) ? 0M : Convert.ToDecimal(Request["txt_couponMoney"].ToString()); //红包面值
                var couponFillMoney = string.IsNullOrWhiteSpace(Request["txt_couponFillMoney"]) ? 0M : Convert.ToDecimal(Request["txt_couponFillMoney"].ToString()); //红包限制金额
                var gameLimit = string.IsNullOrWhiteSpace(Request["gameLimit"]) ? "" : Request["gameLimit"].ToString(); //彩种限制
                var CouponTag = string.IsNullOrWhiteSpace(Request["txt_RedBagTemTags"]) ? "" : Request["txt_RedBagTemTags"].ToString(); //标签
                var temStatus = string.IsNullOrWhiteSpace(Request["temStatus"]) ? CouponTemplateStatus.Enabled : (CouponTemplateStatus)Convert.ToInt32(Request["temStatus"].ToString()); //红包模板状态
                var EndUseDay = string.IsNullOrWhiteSpace(Request["txt_EndUseDay"]) ? 0 : Convert.ToInt32(Request["txt_EndUseDay"].ToString()); //有效天数
                var EndUseTime = string.IsNullOrWhiteSpace(Request["txt_EndUseTime"]) ? DateTime.Now : Convert.ToDateTime(Request["txt_EndUseTime"].ToString()); //有效时间
                var endUseTimeType = string.IsNullOrWhiteSpace(Request["endUseTimeType"]) ? CouponTemplateEndUseTime.EndUseDay : (CouponTemplateEndUseTime)Convert.ToInt32(Request["endUseTimeType"].ToString()); //有效期选择
                List<string> vs = string.IsNullOrEmpty(gameLimit) ? new List<string>() : gameLimit.Split(',').ToList();

                var redBagCouponTemplate = new RedBagCouponTemplateInfo()
                {
                    CouponMoney = CouponMoney,
                    EndUseDay = EndUseDay,
                    TemplateStatus = temStatus,
                    RedBagCouponName = RedBagCouponName,
                    GameConfigs = vs,
                    FillMoney = couponFillMoney,
                    EndUseTime = EndUseTime,
                    CouponTemplateEndUseTime = endUseTimeType,
                    CouponTag = CouponTag
                };

                var r = this.FundClient.AddNewRedBagCouponTemplateAdmin(redBagCouponTemplate);
                return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新红包优惠券模板状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetRedBagConTemStatus()
        {
            var id = int.Parse(Request["id"]); //红包模板ID
            var temStatus = string.IsNullOrWhiteSpace(Request["txt_temStatus"]) ? CouponTemplateStatus.Enabled : (CouponTemplateStatus)Convert.ToInt32(Request["txt_temStatus"].ToString()); //红包模板状态
            var r = this.FundClient.UpdateRedBagCouponTemplateStatus(id, temStatus);
            return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 红包发放记录
        /// </summary>
        /// <returns></returns>
        public ActionResult RedBagCouponList()
        {
            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Today.AddDays(-3) : Convert.ToDateTime(Request["startTime"]);
            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Today.AddDays(1).AddSeconds(-1) : Convert.ToDateTime(Request["endTime"]);
            ViewBag.txt_couponName = string.IsNullOrWhiteSpace(Request["txt_couponName"]) ? "" : Request["txt_couponName"].ToString();
            ViewBag.sel_RedBagSources = string.IsNullOrWhiteSpace(Request["sel_RedBagSources"]) ? "" : Request["sel_RedBagSources"].ToString();
            ViewBag.sel_RedBagUseStatus = string.IsNullOrWhiteSpace(Request["sel_RedBagUseStatus"]) ? "" : Request["sel_RedBagUseStatus"].ToString();
            ViewBag.txt_UserId = string.IsNullOrWhiteSpace(Request["txt_UserId"]) ? "" : Request["txt_UserId"].ToString();
            ViewBag.txt_CustomId = string.IsNullOrWhiteSpace(Request["txt_CustomId"]) ? "" : Request["txt_CustomId"].ToString();
            ViewBag.txt_RedBagCopTemId = string.IsNullOrWhiteSpace(Request["txt_RedBagCopTemId"]) ? "" : Request["txt_RedBagCopTemId"].ToString();

            ViewBag.RedBagSources = this.FundClient.GetRedBagCouponSources().ReturnValue;
            ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request["pageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrWhiteSpace(Request["pageSize"]) ? base.PageSize : Convert.ToInt32(Request["pageSize"]);

            RedBagCouponInfoCollection list = this.QueryClient.QueryRedBagCouponListAdmin(
                 string.IsNullOrEmpty(ViewBag.txt_RedBagCopTemId) ? 0 : Convert.ToInt32(ViewBag.txt_RedBagCopTemId),
                 ViewBag.txt_couponName,
                 string.IsNullOrEmpty(ViewBag.sel_RedBagSources) ? null : ViewBag.sel_RedBagSources.ToString(),
                 null,
                 string.IsNullOrEmpty(ViewBag.txt_CustomId) ? null : ViewBag.txt_CustomId.ToString(),
                 string.IsNullOrEmpty(ViewBag.txt_UserId) ? null : ViewBag.txt_UserId.ToString(),
                 string.IsNullOrEmpty(ViewBag.sel_RedBagUseStatus) ? CouponStatus.All : ((CouponStatus)Convert.ToInt32(ViewBag.sel_RedBagUseStatus)),
                 Convert.ToDateTime(ViewBag.StartTime),
                 Convert.ToDateTime(ViewBag.EndTime),
                 Convert.ToInt32(ViewBag.PageIndex) - 1,
                 ViewBag.PageSize);

            ViewBag.List = list;
            return View();
        }

        /// <summary>
        /// 手动发放红包
        /// </summary>
        /// <returns></returns>
        public ActionResult AddNewRedBagCoupon()
        {
            ViewBag.CustomId = this.CurUser.UserId;
            ViewBag.List = this.QueryClient.QueryRedBagCouponTemplateListAdmin(
                0,
                "",
                0,
                 -1,
                "",
                CouponTemplateStatus.Enabled,
                Convert.ToDateTime("2000-01-01 00:00"), DateTime.Now,
                0,
                99999999);
            return View();
        }

        public class RedBagRequest
        {
            public int Count { get; set; }
            public string RedBagTmp { get; set; }
        }

        /// <summary>
        /// 给用户发红包
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult AddNewRedBagCouponToUser(string userName, List<RedBagRequest> request)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    return Json(new { IsSuccess = false, Msg = "用户名不能为空" }, JsonRequestBehavior.AllowGet);
                }
                if (request == null || request.Count < 1)
                {
                    return Json(new { IsSuccess = false, Msg = "最少要有一条记录" }, JsonRequestBehavior.AllowGet);
                }
                foreach (var item in request)
                {
                    if (string.IsNullOrEmpty(item.RedBagTmp))
                    {
                        return Json(new { IsSuccess = false, Msg = "红包模板不能为空" }, JsonRequestBehavior.AllowGet);
                    }
                    if (item.Count == 0)
                    {
                        return Json(new { IsSuccess = false, Msg = "红包数量不能为0" }, JsonRequestBehavior.AllowGet);
                    }
                }

                //尝试获取用户
                var userId = this.ExternalClient.GetUserIdByLoginName(userName);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { IsSuccess = false, Msg = "该用户名不存在" }, JsonRequestBehavior.AllowGet);
                }
                foreach (var item in request)
                {
                    for (int i = 0; i < item.Count; i++)
                    {
                        var r = this.FundClient.AddNewRedBagCouponToUserByAuthAdmin(userId, Convert.ToInt32(item.RedBagTmp), "后台手动发放", CouponStatus.Enabled, this.CurUser.UserId);
                    }
                }
                return Json(new { IsSuccess = true, Msg = "发送成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新红包优惠券状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetRedBagCouponStatus()
        {
            var id = int.Parse(Request["id"]); //红包ID
            var temStatus = string.IsNullOrWhiteSpace(Request["txt_cpnStatus"]) ? CouponStatus.Enabled : (CouponStatus)Convert.ToInt32(Request["txt_cpnStatus"].ToString()); //红包模板状态
            var r = this.FundClient.UpdateRedBagCouponStatus(id, temStatus);
            return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新用户赠送管理
        /// </summary>
        /// <returns></returns>
        public ActionResult GiveRegisterManager()
        {
            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Today.AddMonths(-1) : Convert.ToDateTime(Request["startTime"]);
            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Today.AddDays(1).AddSeconds(-1) : Convert.ToDateTime(Request["endTime"]);

            ViewBag.txt_couponName = (string.IsNullOrWhiteSpace(Request["txt_couponName"]) || Request["txt_couponName"] == "") ? string.Empty : Request["txt_couponName"].ToString();
            ViewBag.txt_RedBagCopTemId = (string.IsNullOrWhiteSpace(Request["txt_RedBagCopTemId"]) || Request["txt_RedBagCopTemId"] == "") ? string.Empty : Request["txt_RedBagCopTemId"].ToString();
            ViewBag.txt_couponMoney = (string.IsNullOrWhiteSpace(Request["txt_couponMoney"]) || Request["txt_couponMoney"] == "") ? string.Empty : Request["txt_couponMoney"].ToString();

            ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request["pageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrWhiteSpace(Request["pageSize"]) ? base.PageSize : Convert.ToInt32(Request["pageSize"]);

            ViewBag.List = this.QueryClient.QueryRedBagGiveRegisterList(
                (string.IsNullOrWhiteSpace(ViewBag.txt_RedBagCopTemId) || ViewBag.txt_RedBagCopTemId == "") ? 0 : Convert.ToInt32(ViewBag.txt_RedBagCopTemId),
                (string.IsNullOrWhiteSpace(ViewBag.txt_couponName) || ViewBag.txt_couponName == "") ? string.Empty : ViewBag.txt_couponName,
                Convert.ToDateTime(ViewBag.StartTime),
                Convert.ToDateTime(ViewBag.EndTime),
                (string.IsNullOrWhiteSpace(ViewBag.txt_couponMoney) || ViewBag.txt_couponMoney == "") ? null : Convert.ToDecimal(ViewBag.txt_couponMoney),
               Convert.ToInt32(ViewBag.PageIndex) - 1,
                ViewBag.PageSize);
            return View();
        }

        /// <summary>
        /// 新增新用户赠送
        /// </summary>
        /// <returns></returns>
        public ActionResult AddGiveRegister()
        {
            return View();
        }

        /// <summary>
        /// 删除新用户赠送
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelRedBagGiveRegister()
        {
            var id = int.Parse(Request["id"]); //新用户赠送ID
            var tempId = int.Parse(Request["tempId"]); //新用户赠送ID

            var r = this.FundClient.DelRedBagGiveRegister(new RedBagGiveRegisterInfo() { Id = id, RedBagCouponTemplateInfo = new RedBagCouponTemplateInfo() { RedBagCouponTemplateId = tempId } });
            return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存新用户赠送
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveRedBagGiveRegister()
        {
            var tempId = int.Parse(Request["redBagTmpId"]); //新用户赠送ID

            var r = this.FundClient.AddRedBagGiveRegister(new RedBagGiveRegisterInfo()
            {
                RedBagCouponTemplateInfo = new RedBagCouponTemplateInfo() { RedBagCouponTemplateId = tempId }
            });
            return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region ==活动列表==
        public ActionResult GetActivityInfos(int pageIndex = 1, int pageSize = 20)
        {
            var result = GlobalCache.ActivityClient.GetActivityInfos(pageIndex, pageSize);
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.Result = result;
            return View("ActivityList");
        }
        public ActionResult EditActivityInfo(int id)
        {
            var info = GlobalCache.ActivityClient.GetActivityInfoById(id);
            // （1：红包，2：奖励金，3：加奖）
            if (info.ActivityType == 3)
            {
                ViewBag.AddAwards = GlobalCache.ActivityClient.GetBonusMoneyConfigsByActivityId(info.Id);
                ViewBag.ActivityInfo = info;
                return View("AddAward");
            }

            return View("EditActivityInfo");
        }
        [HttpPost]
        public JsonResult UpdateActivityInfo(ActivityInfoModel model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { status = 0, message = "信息录入有问题" });
                }
                if (model.BonusList == null || model.BonusList.Count == 0)
                {
                    return Json(new { status = 0, message = "一个活动至少需要一个彩种加奖配置" });
                }
                var collection = GlobalCache.ActivityClient.QueryAddBonusMoneyConfig();
                string message = string.Empty;
                foreach (var item in model.BonusList)
                {
                    var info = collection.Where(p => p.GameCode == item.GameCode && p.GameType == item.GameType && item.Id != p.Id).FirstOrDefault();
                    if (info != null)
                    {
                        message += GlobalCache.GetGameCodeName(info.GameCode) + ",";
                    }
                }
                if (!string.IsNullOrWhiteSpace(message))
                {
                    return Json(new { status = 1, message = string.Format("以下几个彩种“{0}”对应的玩法已经存在了，请重新添加", message) });
                }
                GlobalCache.ActivityClient.UpdateActivityInfo(new Activity.Core.ActivityListNewInfo()
                {
                    Id = model.Id,
                    Status = model.Status,
                    ActivityName = model.ActivityName,
                    BeginTime = model.BeginTime,
                    BonusList = GetBonusList(model.BonusList),
                    EndTime = model.EndTime == default(DateTime) ? DateTime.MaxValue : model.EndTime
                });
                return Json(new { status = 1, message = "更新成功" });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = "更新失败，原因：" + ex.Message });
            }
        }

        private List<AddBonusMoneyConfigInfo> GetBonusList(List<AddAwardModel> bonusList)
        {
            List<AddBonusMoneyConfigInfo> list = new List<AddBonusMoneyConfigInfo>();
            foreach (var item in bonusList)
            {
                list.Add(new AddBonusMoneyConfigInfo()
                {
                    Id = item.Id,
                    AddBonusMoneyPercent = item.AddBonusMoneyPercent,
                    AddMoneyWay = item.AddMoneyWay,
                    GameCode = item.GameCode,
                    GameType = item.GameType,
                    MaxAddBonusMoney = item.MaxAddBonusMoney,
                    OrderIndex = item.OrderIndex,
                    PlayType = item.PlayType
                });
            }
            return list;
        }
        [HttpGet]
        public ActionResult AddActivityInfo()
        {
            return View("AddAward");
        }
        [HttpPost]
        public JsonResult CreateActivityInfo(ActivityInfoModel model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { status = 0, message = "信息录入有问题" });
                }
                if (model.BonusList == null || model.BonusList.Count == 0)
                {
                    return Json(new { status = 0, message = "一个活动至少需要一个彩种加奖配置" });
                }
                GlobalCache.ActivityClient.AddActivityInfo(new ActivityListNewInfo()
                {
                    Id = model.Id,
                    Status = model.Status,
                    ActivityName = model.ActivityName,
                    BeginTime = model.BeginTime,
                    BonusList = GetBonusList(model.BonusList),
                    EndTime = model.EndTime == default(DateTime) ? DateTime.MaxValue : model.EndTime,
                    ActivityCategory = model.ActivityCategory,
                    ActivityType = model.ActivityType,
                    CreatorId = CurUser.UserId,
                    TodayCount = 0,
                    TotalCount = 0
                });
                return Json(new { status = 1, message = "添加成功" });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = "添加失败,原因：" + ex.Message });
            }
        }
        public JsonResult EnableActivity(int id)
        {
            try
            {
                var info = GlobalCache.ActivityClient.GetActivityInfoById(id);
                if (info == null)
                {
                    return Json(new { status = 0, message = "活动不存在" });
                }
                if (info.Status != 1)
                {
                    GlobalCache.ActivityClient.EnableActivity(id);
                }
                return Json(new { status = 1, message = "活动启用成功" });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = "活动启用失败,原因：" + ex.Message });
            }
        }
        public JsonResult DisableActivity(int id)
        {
            try
            {
                var info = GlobalCache.ActivityClient.GetActivityInfoById(id);
                if (info == null)
                {
                    return Json(new { status = 0, message = "活动不存在" });
                }
                if (info.Status != 0)
                {
                    GlobalCache.ActivityClient.DisableActivity(id);
                }
                return Json(new { status = 1, message = "活动暂停成功" });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = "活动暂停失败,原因：" + ex.Message });
            }
        }
        public ActionResult AddBonusMoneyConfig(int pageIndex = 1, int pageSize = 20)
        {
            if (!CheckRights("CZJJPZ"))
                throw new Exception("对不起，您的权限不足！");
            ViewBag.CZJJPZ_TJGZ = CheckRights("CZJJPZ_TJGZ");
            ViewBag.CZJJPZ_SCGZ = CheckRights("CZJJPZ_SCGZ");
            var result = GlobalCache.ActivityClient.GetBonusMoneyConfigs(pageIndex, pageSize);
            ViewBag.ConfigListPagination = result;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            return View("BonusMoneyConfig");
        }
        #endregion

        public ActionResult RedBagUseConfig()
        {
            if (!CheckRights("HBSYGZ"))
                throw new Exception("对不起，您的权限不足！");
            ViewBag.HBSYGZ_TJGZ = CheckRights("HBSYGZ_TJGZ");
            ViewBag.HBSYGZ_SCGZ = CheckRights("HBSYGZ_SCGZ");
            ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request["PageIndex"]) ? base.PageIndex : int.Parse(Request["PageIndex"]);
            ViewBag.PageSize = string.IsNullOrWhiteSpace(Request["PageSize"]) ? base.PageSize : int.Parse(Request["PageSize"]);
            ViewBag.List = this.ActivityClient.QueryRedBagUseConfigPage(ViewBag.PageIndex - 1, ViewBag.PageSize);
            return View();
        }

        public ActionResult AddRedBagUseConfig()
        {   
            try
            {
                RadBagConfigInfo info = null;
                if (!string.IsNullOrWhiteSpace(Request["Type"]) && Request["Type"].Equals("edit"))
                {
                    string gameCode = PreconditionAssert.IsNotEmptyString(Request["gameCode"], "彩种不能为空");
                    string id = PreconditionAssert.IsNotEmptyString(Request["id"], "id不能为空");
                    var percent = this.ActivityClient.QueryRedBagUseConfigByGameCode(Request["gameCode"]);
                    if (percent > 0)
                        info = new RadBagConfigInfo { GameCode = gameCode, UsePercent = percent, Id = int.Parse(id) };
                }
                ViewBag.Info = info;
                ViewBag.GameList = this.GameIssuseClient.QueryGameListAdmin();
            }
            catch { }
            return View();
        }

        public JsonResult AddRedBagConfig(FormCollection fc)
        {
            try
            {
                string gameCode = fc["GameCode"];
                decimal percent = decimal.Parse(PreconditionAssert.IsNotEmptyString(fc["UsePercent"], "使用比例不能为空"));
                var r = this.ActivityClient.AddRedBagUseConfig(gameCode, percent);
                return Json(new { IsSuccess = r.IsSuccess, Msg = r.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteRedBagUseConfig()
        {
            try
            {
                var id = int.Parse(Request["id"]);
                var r = this.ActivityClient.DeleteRedBagUseConfig(id);
                return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}