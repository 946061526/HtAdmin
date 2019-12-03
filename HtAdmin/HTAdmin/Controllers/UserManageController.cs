using Common.Utilities;
using External.Core;
using External.Core.Login;
using GameBiz.Core;
using HTAdmin.Filters;
using HTAdmin.IdentityManager;
using HTAdmin.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Controllers
{
    public class UserManageController : BaseController
    {
        private AdminUserManager _userManager;
        public AdminUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AdminUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #region ==会员管理==
        [CheckPermission("U101")]
        public ActionResult MemberManage()
        {
            try
            {
                bool dcsj = false;
                bool hyglckyhxq = false;
                if (CheckRights("DCSJ100"))
                    dcsj = true;
                if (CheckRights("HYGLCKYHXQ110"))
                    hyglckyhxq = true;
                ViewBag.Dcsj = dcsj;
                ViewBag.hyglckyhxq = hyglckyhxq;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.UsersSummary = new UserQueryInfoCollection();
                throw ex;
            }
        }
        [CheckPermission("U101")]
        public JsonResult GetUserList(UserQueryConditionModel model)
        {
            UserQueryCondition condition = new UserQueryCondition()
            {
                UserId = model.UserId,
                IsEnable = model.IsEnable,
                Mobile = model.Mobile,
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
                DisplayName = model.DisplayName
            };
            if (!string.IsNullOrWhiteSpace(model.RegistrationTime))
            {
                var arr = model.RegistrationTime.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length == 1)
                {
                    if (arr[0] == "0")
                    {
                        // 今天
                        condition.StartRegistrationTime = DateTime.Now;
                    }
                    else
                    {
                        // 90天以上
                        condition.EndRegistrationTime = DateTime.Now.AddDays(-int.Parse(arr[0]));
                    }
                }
                else if (arr.Length == 2)
                {
                    condition.StartRegistrationTime = DateTime.Now.AddDays(-int.Parse(arr[0]));
                    condition.EndRegistrationTime = DateTime.Now.AddDays(-int.Parse(arr[1]));
                }
            }
            if (!string.IsNullOrWhiteSpace(model.PurchaseCount))
            {
                var arr = model.PurchaseCount.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length == 1)
                {
                    if (arr[0] == "10")
                    {
                        // 10次以上
                        condition.EndPurchaseCount = int.Parse(arr[0]);

                    }
                    else
                    {
                        // 单个多少次
                        condition.StartPurchaseCount = int.Parse(arr[0]);
                    }
                }
                else if (arr.Length == 2)
                {
                    condition.StartPurchaseCount = int.Parse(arr[0]);
                    condition.EndPurchaseCount = int.Parse(arr[1]);
                }
            }
            if (!string.IsNullOrWhiteSpace(model.TotalLotteryMoney))
            {
                var arr = model.TotalLotteryMoney.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length == 1)
                {
                    condition.EndTotalLotteryMoney = int.Parse(arr[0]);
                }
                else if (arr.Length == 2)
                {
                    condition.StartTotalLotteryMoney = int.Parse(arr[0]);
                    condition.EndTotalLotteryMoney = int.Parse(arr[1]);
                }
            }


            var list = GlobalCache.ExternalClient.GetUserListForAdmin(condition);
            LayuiGridResult ret = new LayuiGridResult();
            ret.code = 0;
            ret.count = list.TotalCount;
            if (list.Users != null && list.Users.Count > 0)
            {
                ret.data = list.Users.Select(p =>
                {
                    return new
                    {
                        RowId = p.RowId,
                        DisplayName = p.DisplayName,
                        IsEnable = p.IsEnable,
                        LastLotteryTime = p.LastLotteryTime,
                        Mobile = p.Mobile,
                        OrderCount = p.OrderCount,
                        RegTime = p.RegTime,
                        TotalAssets = p.TotalAssets.ToString("c"),
                        TotalLotteryMoney = p.TotalLotteryMoney.ToString("c"),
                        UserId = p.UserId,
                        UserType = p.UserType
                    };
                });
            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public ViewResult GetUserDetailInfo(string userId)
        {
            ViewBag.UserDetailInfo = GlobalCache.ExternalClient.GetUserDetailInfo(userId);
            var cardInfo = GlobalCache.FundClient.QueryBankCardByUserId(userId, null);
            if (cardInfo != null)
            {
                ViewBag.IsBindBank = true;
            }
            else
            {
                ViewBag.IsBindBank = false;
            }
            ViewBag.UserId = userId;
            return View("UserDetailInfo");
        }

        public JsonResult GetUserLoginHistory(string userId, int pageIndex = 1, int pageSize = 10)
        {

            LayuiGridResult result = new LayuiGridResult()
            {
                code = 0
            };
            try
            {
                int count = 0;
                var p = new AdminLoginHistoryManager().GetUserLoginHistory(userId, pageIndex, pageSize, out count);
                result.data = p.Select(n => new
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    LoginIp = n.Ip,
                    IpDisplayName = n.IpArea,
                    LoginTime = n.LoginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                }).ToArray(); ;
                result.count = count;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.msg = "服务端出错";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLotteryRecord(string userId, int pageIndex = 1, int pageSize = 10)
        {
            LayuiGridResult result = new LayuiGridResult()
            {
                code = 0,
            };
            try
            {
                var p = GlobalCache.QueryClient.GetUserOrderBasicInfos(userId, pageIndex, pageSize);
                if (p != null && p.Data != null)
                {
                    result.data = p.Data.Select(n => new
                    {
                        RowId = n.RowId,
                        SchemeId = n.SchemeId,
                        UserId = n.UserId,
                        BetTime = n.BetTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        SchemeType = n.SchemeType,
                        GameCode = n.GameCode,
                        GameName = n.GameName,
                        TotalMoney = n.TotalMoney.ToString("c"),
                        RedBagMoney = n.RedBagMoney.ToString("c"),
                        RedBagCouponMoney = n.RedBagCouponMoney.ToString("c"),
                    }).ToArray();
                }
                result.count = p.TotalCount;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.msg = "服务端出错";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRedBagRecord(string userId, int pageIndex = 1, int pageSize = 10)
        {

            LayuiGridResult result = new LayuiGridResult()
            {
                code = 0,
            };
            try
            {
                var p = GlobalCache.FundClient.GetRedBagCouponByUserId(userId);
                result.data = p.RedBagCouponMoreInfoList.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                    .Select(n =>
                    new
                    {
                        RedBagCouponId = n.RedBagCouponId,
                        RedBagCouponName = n.RedBagCouponName,
                        ExpiredTime = n.ExpiredTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        FillMoney = n.FillMoney.ToString("c"),
                        CouponUseGames = n.CouponUseGames,
                        CouponStatus = n.CouponStatus,
                        CouponMoney = n.CouponMoney.ToString("c"),
                        CouponUseGameDes = n.CouponUseGameDes
                    }).ToArray();
                result.count = p.TotalCount;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.msg = "服务端出错" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult EnableUser(string userId, string isEnabled)
        {
            try
            {
                string msg = "启用成功";
                if (isEnabled == "0")
                {
                    UserManager.EnableUser(userId,false);
                    //GlobalCache.ExternalClient.DisableUser(userId, CurrentUser.UserToken);
                    msg = "禁用成功";
                }
                else
                {
                    UserManager.EnableUser(userId, true);
                }
                return Json(new { status = 1, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = "操作失败" + ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ResetMobile(string userId, string mobile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Json(new { status = 0, message = "用户Id不可以为空" });
                }
                if (string.IsNullOrWhiteSpace(mobile))
                {
                    return Json(new { status = 0, message = "手机号不可以为空" });
                }
                if (!Regex.IsMatch(mobile, "^1[1-9]\\d{9}$"))
                {
                    return Json(new { status = 0, message = "手机号格式不正确" });
                }
                var exist = GlobalCache.ExternalClient.CheckMobileIsBind(mobile);
                if (exist)
                {
                    return Json(new { status = 0, message = "该手机已被绑定" });
                }
                var result = GlobalCache.ExternalClient.UpdateMobileAuthen(userId, mobile, CurUser.UserId);
                OperLogManager.AddOperationLog(this.CurUser.UserId, "手机认证", string.Format("操作员【{0}】对用户【{1}】手机认证，手机号【{2}】", this.CurUser.UserId, userId, mobile));
                return Json(new { status = 1, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ResetPayPassword(string userId, string payPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Json(new { status = 0, message = "用户Id不可以为空" });
                }
                if (string.IsNullOrWhiteSpace(payPassword))
                {
                    return Json(new { status = 0, message = "支付密码不可以为空" });
                }
                var result = GlobalCache.ExternalClient.ResetUserBalancePwdAdmin(userId, payPassword);
                OperLogManager.AddOperationLog(this.CurUser.UserId, "重置支付密码", string.Format("操作员【{0}】对用户【{1}】的支付密码进行重置", this.CurUser.UserId, userId), userId);
                return Json(new { status = 1, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ResetLoginPassword(string userId, string loginPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Json(new { status = 0, message = "用户Id不可以为空" });
                }
                if (string.IsNullOrWhiteSpace(loginPassword))
                {
                    return Json(new { status = 0, message = "登录密码不可以为空" });
                }
                var result = GlobalCache.ExternalClient.ResetUserLoginPwdAdmin(userId, loginPassword);
                OperLogManager.AddOperationLog(this.CurUser.UserId, "重置登录密码", string.Format("操作员【{0}】对用户【{1}】的登录密码进行重置", this.CurUser.UserId, userId), userId);
                return Json(new { status = 1, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult FillMoneyForUser(string userId, decimal totalMoney, string accountType, string description)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Json(new { status = 0, message = "用户Id不可以为空" });
                }
                if (totalMoney < 0)
                {
                    return Json(new { status = 0, message = "充值金额不可以小于0" });
                }
                AccountType accType;
                if (Enum.TryParse(accountType, out accType) && (accType != AccountType.FillMoney || accType != AccountType.RedBag || accType != AccountType.Commission || accType != AccountType.Bonus))
                {
                    var result = GlobalCache.FundClient.ManualAddMoneyAdmin("", totalMoney, accType, userId, description, CurUser.UserId);
                    OperLogManager.AddOperationLog(this.CurUser.UserId, "手工打款", string.Format("操作员【{0}】对用户【{1}】打款【{2}】元", this.CurUser.UserId, userId, totalMoney), userId);
                    return Json(new { status = 1, message = result.Message });
                }
                else
                {
                    return Json(new { status = 0, message = "充值账户类型不正确" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ChargeForUser(string userId, decimal totalMoney, string accountType, string description)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Json(new { status = 0, message = "用户Id不可以为空" });
                }
                if (totalMoney < 0)
                {
                    return Json(new { status = 0, message = "扣款金额不可以小于0" });
                }
                AccountType accType;
                if (Enum.TryParse(accountType, out accType) && (accType != AccountType.FillMoney || accType != AccountType.RedBag || accType != AccountType.Commission || accType != AccountType.Bonus))
                {

                    var result = GlobalCache.FundClient.ManualDeductMoneyAdmin("", totalMoney, accType, userId, description, CurUser.UserId);
                    OperLogManager.AddOperationLog(this.CurUser.UserId, "手工扣款", string.Format("操作员【{0}】对用户【{1}】扣款【{2}】元", this.CurUser.UserId, userId, totalMoney), userId);
                    return Json(new { status = 1, message = result.Message });
                }
                else
                {
                    return Json(new { status = 0, message = "扣款账户类型不正确" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ReauthenticateUser(string userId, string realName, string cardNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Json(new { status = 0, message = "UserId不可以为空" });
                }
                if (string.IsNullOrWhiteSpace(realName))
                {
                    return Json(new { status = 0, message = "姓名不可以为空" });
                }
                if (string.IsNullOrWhiteSpace(cardNumber) ||
                    !Regex.IsMatch(cardNumber, @"^(^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$)|(^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])((\d{4})|\d{3}[Xx])$)$"))
                {
                    return Json(new { status = 0, message = "身份证号不可以为空，并且长度不可以小于16个字符" });
                }
                string msg = "成功添加";
                bool exist = GlobalCache.ExternalClient.CheckIsAuthenticatedUserRealName(userId, null);
                if (!exist)
                {
                    GlobalCache.ExternalClient.AuthenticateUserRealNameAdmin(userId, new External.Core.Authentication.UserRealNameInfo()
                    {
                        AuthFrom = "INNER",
                        CardType = "0",
                        RealName = realName,
                        CreateBy = CurUser.UserId,
                        CreateTime = DateTime.Now,
                        UpdateBy = CurUser.UserId,
                        UpdateTime = DateTime.Now,
                        UserId = userId,
                        IdCardNumber = cardNumber
                    }, SchemeSource.Web, CurUser.UserId);
                    OperLogManager.AddOperationLog(this.CurUser.UserId, "添加实名认证", string.Format("操作员【{0}】添加用户【{1}】实名认证，真实姓名【{2}】，身份证号【{3}】", this.CurUser.UserId, userId, realName, cardNumber), userId);
                }
                else
                {
                    msg = "成功修改";
                    var result = GlobalCache.ExternalClient.UpdateRealNameAuthenticationAdmin(userId, realName, cardNumber, CurUser.UserId);
                    OperLogManager.AddOperationLog(this.CurUser.UserId, "更新实名认证", string.Format("操作员【{0}】更新用户【{1}】实名认证，真实姓名【{2}】，身份证号【{3}】", this.CurUser.UserId, userId, realName, cardNumber), userId);
                }
                return Json(new { status = 1, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateBankInfoForUser(string userId, string bankCode, string bankSubName, string cardNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Json(new { status = 0, message = "用户Id不可以为空" });
                }
                if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 16)
                {
                    return Json(new { status = 0, message = "银行卡不可以为空，并且长度不可以小于16个字符" });
                }

                var bank = GlobalCache.ExternalClient.QueryBankManagement(bankCode);
                if (bank == null || bank.TotalCount == 0)
                {
                    return Json(new { status = 0, message = "输入的银行暂不支持" });
                }
                BankCardInfo info = GlobalCache.FundClient.QueryBankCardByUserId(userId,null);
                var user = GlobalCache.ExternalClient.GetUserDetailInfo(userId);
                if (info == null)
                {
                    GlobalCache.FundClient.AddBankCardAdmin(new BankCardInfo()
                    {
                        BankCardNumber = cardNumber,
                        BankSubName = bankSubName,
                        BankName = bank.HelpList[0].bank_name,
                        BankCode = bankCode,
                        UserId = userId,
                        RealName = user.RealName,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        CityName = "",
                        ProvinceName = ""
                    });
                    OperLogManager.AddOperationLog(this.CurUser.UserId, "添加银行卡", string.Format("操作员【{0}】给用户【{1}】添加银行卡，卡号【{2}】 ", this.CurUser.UserName, userId, cardNumber), userId);
                    return Json(new { status = 1, message = "成功添加银行卡" });
                }
                else
                {
                    info.BankName = bank.HelpList[0].bank_name;
                    info.BankSubName = bankSubName;
                    info.BankCardNumber = cardNumber;
                    var result = base.FundClient.UpdateBankCardAdmin(info);
                    OperLogManager.AddOperationLog(this.CurUser.UserId, "更新银行卡", string.Format("操作员【{0}】给用户【{1}】更新银行卡，卡号【{2}】 ", this.CurUser.UserName, userId, cardNumber), userId);
                    return Json(new { status = 1, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }
        public ActionResult ModifyBankInfo(string userId)
        {
            ViewBag.UserId = userId;
            ViewBag.BankInfos = GlobalCache.ExternalClient.QueryBankManagement();
            return View();
        }
        #endregion

        #region 推送发送记录
        /// <summary>
        /// 推送记录
        /// </summary>
        /// <returns></returns>
        public ActionResult PushRecordLog()
        {
            //if (!CheckRights("H137"))
            //    throw new Exception("对不起，您的权限不足！");
            ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request["pageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrWhiteSpace(Request["pageSize"]) ? base.PageSize : Convert.ToInt32(Request["pageSize"]);

            ViewBag.List = base.QueryClient.QueryPushRecordList((ViewBag.PageIndex - 1), ViewBag.PageSize);
            return View();
        }

        /// <summary>
        /// 查询推送记录
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult QueryPushRecordLog()
        {
            var pageIndex = string.IsNullOrEmpty(Request["page"]) ? base.PageIndex : int.Parse(Request["page"]) - 1;
            var pageSize = string.IsNullOrEmpty(Request["limit"]) ? base.PageSize : int.Parse(Request["limit"]);
            var list = base.QueryClient.QueryPushRecordList(pageIndex, pageSize);
            LayuiGridResult ret = new LayuiGridResult();
            ret.code = 0;
            ret.count = list.TotalCount;
            ret.data = list.RecordList;
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 手动推送
        /// <summary>
        /// 手动推送
        /// </summary>
        /// <returns></returns>
        public ActionResult ManualPush()
        {
            //if (!CheckRights("H138"))
            //    throw new Exception("对不起，您的权限不足！");
            return View();
        }

        /// <summary>
        /// 手动推送
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ActionResult PushSubmit(PushRecordInfo info)
        {
            try
            {
                //if (!CheckRights("H138"))
                //    throw new Exception("对不起，您的权限不足！");
                if (info == null)
                {
                    return Content("保存失败");
                }

                var res = base.GameClient.BackGroundDoPush(info.UserId, info.PushType, info.AfterOpenType, info.Title, info.Content, info.Url, info.Activity, info.Custom);
                if (res.IsSuccess)
                {
                    return RedirectToAction("PushRecordLog");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 代理相关
        /// <summary>
        /// 添加代理
        /// </summary>
        [CheckPermission("U106")]
        public ActionResult AgentManage()
        {
            return View();
        }

        public JsonResult GetAgentDetailInfos(AgentDetailModel model)
        {
            LayuiGridResult result = new LayuiGridResult() { code = 0 };
            var list = ExternalClient.QueryAgentDetailsList(model.UserId, model.DisplayName, model.Mobile, model.ParentAgentId, model.IsAgent, model.AgentGrade, model.PageIndex - 1, model.PageSize);
            result = new LayuiGridResult { data = list.List, msg = "成功", count = list.TotalCount };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AgentRebateInfo(string userId) {
           ViewBag.List = ExternalClient.QueryUserRebate(userId); 
            return View();
        }

        /// <summary>
        /// 是否注销经销商
        /// </summary>
        /// <returns></returns>
        public JsonResult IsResAgent()
        {
            try
            {
                var userId = PreconditionAssert.IsNotEmptyString(Request["userId"], "用户编号不能为空");
                var status = PreconditionAssert.IsNotEmptyString(Request["status"], "状态不能为空");
                var result = ExternalClient.IsOffUserAgent(userId, status.Contains("0"));
                if (result.ReturnValue.Equals("true"))
                    return Json(new CommonResult { code = (int)ResultCodeEnum.OK, message = result.Message });
                else
                    return Json(new CommonResult { code = (int)ResultCodeEnum.APIError, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new CommonResult { code = (int)ResultCodeEnum.APIError, message = ex.Message });
            }
        }
        
        /// <summary>
        /// 添加代理
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAgent()
        {
            return View();
        }

        /// <summary>
        /// 提交添加代理
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoAgent()
        {
            try
            {
                var txt_userId = Request.Form["txt_userId"].ToString();
                var txt_oCAgentCategory = (OCAgentCategory)Convert.ToInt32(Request.Form["txt_oCAgentCategory"]);
                var txt_superiorUserId = string.Empty;
                if (txt_oCAgentCategory != OCAgentCategory.Company)
                {
                    txt_superiorUserId = Request.Form["txt_superiorUserId"].ToString();
                }
                var result = base.ExternalClient.AddOCAgent(txt_oCAgentCategory, txt_superiorUserId, txt_userId, CPSMode.PayRebate, "");
                if (result.IsSuccess)
                {
                    //清理redis 缓存
                    base.ExternalClient.ManualClearUserBindCache(txt_userId);
                    //成生缓存
                    base.ExternalClient.ManualBuildUserBindCache(txt_userId);
                }
                return Json(new CommonResult { code = (int)ResultCodeEnum.OK, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new CommonResult { code = (int)ResultCodeEnum.APIError, message = ex.Message });
            }
        }

        public ActionResult Agent_PayDetail()
        {
            ViewBag.GameList = this.GameIssuseClient.QueryGameListAdmin();
            return View();
        }

        public JsonResult GetAgentPayDetailsInfos(AgentPayDetailsModel model)
        {
            try
            {
                LayuiGridResult result = new LayuiGridResult() { code = 0 };
                var list = ExternalClient.NewQueryOCAgentPayDetailList(model.fromDate, model.toDate, model.userId, model.displayName,
                    model.gameCode, model.startBuyMoney, model.endBuyMoney, model.startRabateMoney, model.endRabateMoney, model.pageIndex - 1, model.pageSize);
                if (list != null && list.DetailList.Count > 0)
                {
                    var objList = new List<object>();
                    foreach (var item in list.DetailList)
                    {
                        objList.Add(new 
                        {
                            NumIndex = list.DetailList.IndexOf(item) + ((model.pageIndex-1)*10) + 1,
                            CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            OrderUser = item.OrderUser,
                            OrderUserDisplayName = item.OrderUserDisplayName,
                            TotalMoney = item.TotalMoney.ToString("N2"),
                            PayMoney = item.PayMoney.ToString("N2"),
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            SchemeId = item.SchemeId,
                        });
                    }
                    
                    result.count = list.TotalCount;
                    result.data = objList;
                    result.msg = "成功";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.msg = "没有查到结果";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new LayuiGridResult { code = (int)ResultCodeEnum.APIError, msg = "查询失败" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}