using Common.Utilities;
using GameBiz.Core;
using HTAdmin.Filters;
using HTAdmin.IdentityManager;
using HTAdmin.Models;
using HTAdmin.Models.Finance;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace HTAdmin.Controllers
{
    public class FinanceController : BaseController
    {

        #region 充值

        /// <summary>
        /// 充值列表
        /// </summary>
        /// <returns></returns>
        [CheckPermission("C102")]
        public ActionResult RechargeList()
        {
            bool wccz = false;
            bool zwsb = false;
            bool czdcsj = false;
            bool czmxckyhxq = false;
            if (CheckRights("WCCZ100"))
                wccz = true;
            if (CheckRights("ZWSB110"))
                zwsb = true;
            if (CheckRights("CZDCSJ120"))
                czdcsj = true;
            if (CheckRights("CZMXCKYHXQ130"))
                czmxckyhxq = true;
            ViewBag.wccz = wccz;
            ViewBag.zwsb = zwsb;
            ViewBag.czdcsj = czdcsj;
            ViewBag.czmxckyhxq = czmxckyhxq;
            ViewBag.CZMX_CZBS = CheckRights("CZMX_CZBS");
            ViewBag.CZMX_QQCZJE = CheckRights("CZMX_QQCZJE");
            ViewBag.CZMX_WCCZJQ = CheckRights("CZMX_WCCZJQ");

            ViewBag.UserID = string.IsNullOrWhiteSpace(Request["UserID"]) ? "" : Request["UserID"].Trim();
            ViewBag.OrderId = string.IsNullOrEmpty(Request["OrderId"]) ? "" : Request["OrderId"].Trim();
            ViewBag.Status = Request["Status"] == null ? "" : Request["Status"].ToString();
            ViewBag.SchemeSource = Request["SchemeSource"] == null ? "" : Request["SchemeSource"].ToString();
            ViewBag.AgentType = Request["AgentType"] == null ? "" : Request["AgentType"].ToString();
            ViewBag.StartTime = string.IsNullOrEmpty(Request["StartTime"]) ? DateTime.Today.AddDays(-7) : Convert.ToDateTime(Request["StartTime"]);
            ViewBag.EndTime = string.IsNullOrEmpty(Request["EndTime"]) ? DateTime.Today.AddDays(1).AddSeconds(-1) : Convert.ToDateTime(Request["EndTime"]);
            ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request.QueryString["PageIndex"]) ? base.PageIndex : int.Parse(Request.QueryString["PageIndex"]);
            ViewBag.PageSize = string.IsNullOrWhiteSpace(Request.QueryString["PageSize"]) ? base.PageSize : int.Parse(Request.QueryString["PageSize"]);
            ViewBag.AgentIds = string.IsNullOrWhiteSpace(Request.QueryString["AgentIds"]) ? "" : Request.QueryString["AgentIds"];
            ViewBag.List = base.QueryClient.QueryFillMoneyList(ViewBag.UserID,
                ViewBag.AgentType,
                ViewBag.Status,
                ViewBag.SchemeSource,
                ViewBag.StartTime, ViewBag.EndTime,
                Convert.ToInt32(ViewBag.PageIndex) - 1, ViewBag.PageSize, ViewBag.OrderId, ViewBag.AgentIds,
                CurrentUser1.UserToken);

            return View();
        }

        /// <summary>
        /// 手工完成充值
        /// </summary>
        [HttpPost]
        public JsonResult CompleteRecharge()
        {
            try
            {
                string orderId = PreconditionAssert.IsNotEmptyString(Request["orderId"], "订单ID不能为空");
                if (!string.IsNullOrWhiteSpace(Request["sta"]) && Request["sta"] == "Fail")
                {
                    var result = base.FundClient.ManualCompleteFillMoneyOrder(orderId, GameBiz.Core.FillMoneyStatus.Failed, CurrentUser1.UserToken);

                    AdminOperLogManager operLog = new AdminOperLogManager();
                    operLog.AddOperationLog(new AdminOperationLog()
                    {
                        CreationDate = DateTime.Now,
                        Description = string.Format("操作员【{0}】充值手工置为失败【{1}】，订单号【{2}】", User.Identity.Name, result.IsSuccess ? "成功" : "失败", orderId),
                        OperationName = "手工充值",
                        OperUserId = CurUser.UserId,
                        UserId = ""
                    });
                    return Json(new { IsSuccess = result.IsSuccess, Msg = (result.IsSuccess ? "手工置为失败成功" : "手工置为失败失败") });
                }
                else
                {
                    var result = base.FundClient.ManualCompleteFillMoneyOrder(orderId, GameBiz.Core.FillMoneyStatus.Success, CurrentUser1.UserToken);

                    AdminOperLogManager operLog = new AdminOperLogManager();
                    operLog.AddOperationLog(new AdminOperationLog()
                    {
                        CreationDate = DateTime.Now,
                        Description = string.Format("操作员【{0}】充值手工置为成功【{1}】，订单号【{2}】", User.Identity.Name, result.IsSuccess ? "成功" : "失败", orderId),
                        OperationName = "手工充值",
                        OperUserId = CurUser.UserId,
                        UserId = ""
                    });
                    return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        ///// <summary>
        ///// 充值列表查询
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public JsonResult RechargeListQuery(RechargeListQueryModel model)
        //{
        //    LayuiGridResult result = new LayuiGridResult() { code = 0 };
        //    string userID = model.UserID ?? "";
        //    string orderId = model.OrderId ?? "";
        //    string agentType = model.AgentType ?? "";
        //    string status = model.Status ?? "";
        //    string source = model.SchemeSource ?? "";
        //    string agentIds = model.AgentIds ?? "";
        //    DateTime sTime = model.StartTime == null ? DateTime.Today.AddMonths(-1) : Convert.ToDateTime(model.StartTime);
        //    DateTime eTime = model.EndTime == null ? DateTime.Today : Convert.ToDateTime(model.EndTime);

        //    var res = GlobalCache.QueryClient.QueryFillMoneyList(userID, agentType, status, source, sTime, eTime, model.PageIndex - 1, model.PageSize, orderId, agentIds, CurrentUser1.UserToken) as GameBiz.Core.FillMoneyQueryInfoCollection;

        //    ViewBag.FillDetail = base.QueryClient.QueryFillMoneyList(userID, agentType, status, source, sTime, eTime, model.PageIndex - 1, model.PageSize, orderId, agentIds, CurrentUser1.UserToken);

        //    if (res != null)
        //    {
        //        result.count = res.TotalCount;
        //        int i = (model.PageIndex - 1) * model.PageSize;
        //        result.data = res.FillMoneyList?.Select(a =>
        //        {
        //            i++;
        //            return new
        //            {
        //                Id = i,
        //                a.OuterFlowId,
        //                a.ResponseTime,
        //                a.ResponseMoney,
        //                a.ResponseMessage,
        //                a.ResponseCode,
        //                a.ResponseBy,
        //                a.Status,
        //                a.ShowUrl,
        //                a.NotifyUrl,
        //                a.ReturnUrl,
        //                a.RequestTime,
        //                a.PayMoney,
        //                a.RequestMoney,
        //                a.RequestExtensionInfo,
        //                a.RequestBy,
        //                a.DeliveryAddress,
        //                a.IsNeedDelivery,
        //                a.GoodsDescription,
        //                a.GoodsType,
        //                a.GoodsName,
        //                a.UserComeFrom,
        //                a.UserDisplayName,
        //                a.UserId,
        //                a.FillMoneyAgent,
        //                a.OrderId,
        //                a.SchemeSource,
        //                a.AgentId
        //            };
        //        }).ToList();
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        #region 提现
        /// <summary>
        /// 提现管理列表
        /// </summary>
        [CheckPermission("C103")]
        public ActionResult WithdrawList()
        {
            try
            {
                bool cktxxq = false;
                bool txmxdcsj = false;
                if (CheckRights("CKTXXQ100"))
                    cktxxq = true;
                if (CheckRights("TXMXDCSJ110"))
                    txmxdcsj = true;
                ViewBag.cktxxq = cktxxq;
                ViewBag.txmxdcsj = txmxdcsj;
                //详情权限
                ViewBag.AllowDetail = CheckRights("C103");

                ViewBag.StartTime = string.IsNullOrEmpty(Request["StartTime"]) ? DateTime.Today.AddDays(-7) : Convert.ToDateTime(Request["StartTime"]);
                ViewBag.EndTime = string.IsNullOrEmpty(Request["EndTime"]) ? DateTime.Today.AddDays(1).AddSeconds(-1) : Convert.ToDateTime(Request["EndTime"]);
                ViewBag.UserID = string.IsNullOrWhiteSpace(Request["UserID"]) ? "" : Request["UserID"].Trim();
                ViewBag.UserName = string.IsNullOrWhiteSpace(Request["UserName"]) ? "" : Request["UserName"].Trim();
                ViewBag.BankCode = string.IsNullOrWhiteSpace(Request["BankCode"]) ? "" : Request["BankCode"].Trim();
                ViewBag.BankCard = string.IsNullOrWhiteSpace(Request["BankCard"]) ? "" : Request["BankCard"].Trim();
                ViewBag.RealName = string.IsNullOrWhiteSpace(Request["RealName"]) ? "" : Request["RealName"].Trim();
                ViewBag.MoneyMin = string.IsNullOrWhiteSpace(Request["MoneyMin"]) ? -1M : Convert.ToDecimal(Request["MoneyMin"]);
                ViewBag.MoneyMax = string.IsNullOrWhiteSpace(Request["MoneyMax"]) ? -1M : Convert.ToDecimal(Request["MoneyMax"]);
                WithdrawAgentType? agent = null;
                ViewBag.Agent = agent;
                WithdrawStatus? status = null;
                if (!string.IsNullOrEmpty(Request["Status"]))
                {
                    var intStatus = int.Parse(Request["Status"]);
                    status = (WithdrawStatus)intStatus;
                }
                ViewBag.Status = status;
                ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request.QueryString["PageIndex"]) ? base.PageIndex : int.Parse(Request.QueryString["PageIndex"]);
                ViewBag.PageSize = string.IsNullOrWhiteSpace(Request.QueryString["PageSize"]) ? base.PageSize : int.Parse(Request.QueryString["PageSize"]);

                ViewBag.List = GlobalCache.ExternalClient.QueryWithdrawListV13(ViewBag.StartTime, ViewBag.EndTime, ViewBag.UserID, ViewBag.UserName
                    , ViewBag.BankCode, ViewBag.BankCard, ViewBag.RealName
                    , ViewBag.MoneyMin, ViewBag.MoneyMax, ViewBag.Status, ViewBag.Agent, Convert.ToInt32(ViewBag.PageIndex) - 1, ViewBag.PageSize);

                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [CheckPermission("C104")]
        /// <summary>
        /// 提现订单详情
        /// </summary>
        public ActionResult WithdrawDetail()
        {
            try
            {
                //bool txqxdcsj = false;
                //bool txckyhxq = false;
                //bool txckddxq = false;
                //bool txcl = false;
                //if (CheckRights("TXQXDCSJ100"))
                //    txqxdcsj = true;
                //if (CheckRights("TXCKYHXQ110"))
                //    txckyhxq = true;
                //if (CheckRights("TXCKDDXQ120"))
                //    txckddxq = true;
                //if (CheckRights("TXCL130"))
                //    txcl = true;
                //ViewBag.txqxdcsj = txqxdcsj;
                //ViewBag.txckyhxq = txckyhxq;
                //ViewBag.txckddxq = txckddxq;
                //ViewBag.txcl = txcl;
                ViewBag.OrderId = string.IsNullOrWhiteSpace(Request["OrderId"]) ? "" : Request["OrderId"];
                //string userId = string.IsNullOrWhiteSpace(Request["UserId"]) ? "" : Request["UserId"];
                if (!string.IsNullOrEmpty(ViewBag.OrderId))
                {
                    ViewBag.WithdrawInfo = base.FundClient.GetWithdrawById(ViewBag.OrderId, CurrentUser1.UserToken);
                    //if (ViewBag.WithdrawInfo != null && (ViewBag.WithdrawInfo.Status == WithdrawStatus.Requesting || ViewBag.WithdrawInfo.Status == WithdrawStatus.Request))
                    //{
                    //    ViewBag.RemarkList = GetRemarkInfoList();
                    //}
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 用户充值总额
        /// </summary>
        /// <returns></returns>
        public ContentResult QueryUserTotalRecharge()
        {
            try
            {
                var userId = Request["userId"];
                var money = FundClient.QueryUserTotalFillMoney(userId);
                return Content(money.ToString("c"));
            }
            catch (Exception)
            {
                return Content("0");
            }
        }
        /// <summary>
        /// 用户提现总额
        /// </summary>
        /// <returns></returns>
        public ContentResult QueryUserTotalWithdraw()
        {
            try
            {
                var userId = Request["userId"];
                var money = FundClient.QueryUserTotalWithdrawMoney(userId);
                return Content(money.ToString("c"));
            }
            catch (Exception)
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 人工打款
        /// </summary>
        /// <returns></returns>
        public JsonResult CompleteWithdrawALL()
        {
            try
            {
                string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
                string[] arrs = orderIds.Split(',');
                if (arrs.Length == 0)
                {
                    return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
                }
                string msg = string.Empty;
                foreach (var item in arrs)
                {
                    var result = base.FundClient.CompleteWithdraw(item, "接受提现，已打款", CurrentUser1.UserToken);
                    //提款成功发送短信给用户
                    if (result.IsSuccess)
                    {
                        try
                        {
                            var with = base.FundClient.GetWithdrawById(item, CurrentUser1.UserToken);
                            var mobile = with.RequesterMobile;
                            var content = "尊敬的 " + with.RequesterRealName + " 您好，您申请【" + with.RequestMoney.ToString("f") + "元】已处理，请注意查收。";
                            var returned = SendMessage(mobile, content);

                            //推送消息
                            var pushTitle = "提现申请审核通过";
                            var pushContent = "您的" + with.RequestMoney.ToString("f") + "元的提现已提交到银行处理，敬请留意银行账户余额。";
                            base.GameClient.BackGroundDoPush(with.RequesterUserKey, PushType.Unicast, AfterOpenType.GoActivity,
                                pushTitle, pushContent, string.Empty, "Push_Finance_Withdraw", string.Empty);
                        }
                        catch
                        {

                        }
                    }
                }
                return Json(new { IsSuccess = true, Msg = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 拒绝提现
        /// </summary>
        /// <returns></returns>
        public JsonResult RefusedWithdrawALL()
        {
            try
            {
                string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
                string message = PreconditionAssert.IsNotEmptyString(Request["message"], "备注不能为空");
                string[] arrs = orderIds.Split(',');
                if (arrs.Length == 0)
                {
                    return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
                }
                foreach (var item in arrs)
                {
                    var result = base.FundClient.RefusedWithdraw(item, message, CurrentUser1.UserToken);
                    //推送消息
                    var with = base.FundClient.GetWithdrawById(item, CurrentUser1.UserToken);
                    var pushTitle = "提现申请审核拒绝";
                    var pushContent = "您的" + with.RequestMoney.ToString("f") + "元的提现申请被拒绝，点击查看详情。";
                    base.GameClient.BackGroundDoPush(with.RequesterUserKey, PushType.Unicast, AfterOpenType.GoActivity,
                        pushTitle, pushContent, string.Empty, "Push_Finance_Withdraw", string.Empty);
                }
                return Json(new { IsSuccess = true, Msg = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        //private List<RemarkInfo> GetRemarkInfoList()
        //{
        //    var result = new List<RemarkInfo>();
        //    var errorList = new List<string>();
        //    errorList.Add("银行卡账户与户名不符或不存在，请仔细核对银行卡后联系在线客服处理！");
        //    errorList.Add("打款用户不存在，已经多次尝试失败，请查看银行卡是否正确，联系客服！");
        //    errorList.Add("您本次提款含充值账户资金，需扣除手续费，如继续申请请联系客服确认！");
        //    errorList.Add("您本次提款含充值账户资金，需扣除手续费，如确认提款请再次重新申请！");
        //    errorList.Add("您的银行受限或已进黑名单，导致三方打款失败，建议重新注册其他实名！");
        //    errorList.Add("您本次提款已经达到当日限额，再次提款需过零点以后，或联系客服换卡！");
        //    errorList.Add("您所使用的银行卡为二类三类账户，无法正常接收打款，请联系客服更换！");
        //    errorList.Add("您所使用的银行卡状态异常，或已经超出当日限额，请联系客服更换处理！");
        //    errorList.Add("请不要重复失败提款，您的银行信息有误，请及时联系在线客服处理谢谢！");
        //    errorList.Add("重复申请提款已取消，如继续请重新提交申请！");
        //    errorList.Add("受理用户取消提现确认！");
        //    errorList.Add("同一身份注册多个账号！");
        //    errorList.Add("活动赠送现金仅供购彩使用，不予直接提现！");
        //    errorList.Add("您的账户存在异常行为，为了保障您的资金安全，请联系在线客服并提供手持身份证正面和反面照片！");
        //    for (int i = 0; i < errorList.Count; i++)
        //    {
        //        var item = errorList[i];
        //        result.Add(new RemarkInfo
        //        {
        //            Index = i,
        //            Message = item,
        //            Value = i == 0,
        //        });
        //    }
        //    return result;
        //}

        #region 顺利付

        public ulong SLF_PAY_APP_ID
        {
            get
            {
                ulong defalutValue = 11006;
                try
                {
                    var v = ConfigurationManager.AppSettings["slf_appid"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return ulong.Parse(v);
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        public string SLF_PAY_privateKey
        {
            get
            {
                string defalutValue = "";
                try
                {
                    var v = ConfigurationManager.AppSettings["slf_privateKey"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        /// <summary>
        /// 顺利付
        /// </summary>
        /// <returns></returns>
        public JsonResult CompleteWithdrawALL_SLF()
        {
            try
            {
                string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
                string[] arrs = orderIds.Split(',');
                if (arrs.Length == 0)
                {
                    return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
                }
                string msg = "已提交\r\n";
                foreach (var item in arrs)
                {
                    var with = base.FundClient.GetWithdrawById(item, CurrentUser1.UserToken);
                    string channelid = Common.Pay.shunlifu.SLF_PAY.GetInstance().GetBankNo(with.BankName);
                    ushort channel = 0;
                    if (channelid == "0")
                    {
                        msg += string.Format("该订单提交失败,原因是未找到银行对应编号,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
                        continue;
                    }
                    channel = ushort.Parse(channelid);
                    var slf_pay = Common.Pay.shunlifu.SLF_PAY.GetInstance();
                    slf_pay.AppId = this.SLF_PAY_APP_ID;
                    slf_pay.PrivateKey = this.SLF_PAY_privateKey;
                    slf_pay.gateway_url = ConfigurationManager.AppSettings["gateway_url"].ToString();
                    var pay_msg = slf_pay.ChargePay(1, channel, item, with.ResponseMoney.Value, "127.0.0.1", "", "代付", "代付", CurrentUser1.UserId, with.RequesterRealName, with.BankCardNumber);
                    string[] arr = pay_msg.Split('|');
                    if (arr[0] == "true")
                    {
                        if (arr[1] == "0")
                        {
                            var result = base.FundClient.ReuqestWithdraw(item, CurrentUser1.UserToken, this.SLF_PAY_APP_ID.ToString());
                        }
                        else
                        {
                            msg += string.Format("该订单提交失败,原因是银行返回失败,请手动处理这一笔,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
                            continue;
                        }

                        //var result = base.FundClient.CompleteWithdraw(item, "接受提现，已打款", CurrentUser.UserToken);
                        //提款成功发送短信给用户
                        //if (result.IsSuccess)
                        //{
                        //    try
                        //    {
                        //        var mobile = with.RequesterMobile;
                        //        var content = "尊敬的 " + with.RequesterRealName + " 您好，您申请【" + with.ResponseMoney.Value.ToString("f") + "元】已处理，请注意查收。";
                        //        var returned = SendMessage(mobile, content);
                        //    }
                        //    catch { }
                        //}
                    }
                    else
                    {
                        msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, arr[1]);
                    }
                }
                return Json(new { IsSuccess = true, Msg = msg });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        #endregion

        #region "艾付代付"

        string af_merchant_no = System.Configuration.ConfigurationManager.AppSettings["af_merchant_no"].ToString() == "" ? "144801008923" : System.Configuration.ConfigurationManager.AppSettings["af_merchant_no"].ToString();
        string af_key = System.Configuration.ConfigurationManager.AppSettings["af_key"].ToString() == "" ? "560d1319-fff4-11e7-87a9-8d889c40f997" : System.Configuration.ConfigurationManager.AppSettings["af_key"].ToString();
        string af_url = System.Configuration.ConfigurationManager.AppSettings["af_url"].ToString() == "" ? "http://pay.ifeepay.com/withdraw/singleWithdraw" : System.Configuration.ConfigurationManager.AppSettings["af_url"].ToString();
        /// <summary>
        /// 代付支付密码998909
        /// </summary>
        string af_pay_pwd = System.Configuration.ConfigurationManager.AppSettings["af_pay_pwd"].ToString() == "" ? "B73DEFC24E4EE5388C95975DC0014E8436443066304B" : System.Configuration.ConfigurationManager.AppSettings["af_pay_pwd"].ToString();
        /// <summary>
        /// 提交艾付代付打款
        /// </summary>
        /// <returns></returns>
        public JsonResult CompleteWithdrawALL_AF()
        {
            try
            {
                string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
                string[] arrs = orderIds.Split(',');
                if (arrs.Length == 0)
                {
                    return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
                }
                string msg = "已提交\r\n";
                String TempRSAStr = string.Empty;//明细串
                foreach (var item in arrs)
                {
                    var with = base.FundClient.GetWithdrawById(item, CurrentUser1.UserToken);
                    //检查是否支持该银行
                    string bankCode = Common.Pay.af.afPay.checkBankName(with.BankName);
                    if (bankCode == "0")
                    {
                        msg += string.Format("该订单提交失败,原因是艾付暂不支持该银行打款,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
                        continue;
                    }
                    //MD5签名串
                    string src = string.Format(@"merchant_no={0}&order_no={1}&card_no={2}&account_name={3}&bank_branch={4}&cnaps_no={5}&bank_code={6}&bank_name={7}&amount={8}&pay_pwd={9}",
                        this.af_merchant_no, with.OrderId, with.BankCardNumber, Common.Pay.af.afUtil.EncodeBase64(Encoding.UTF8, with.RequesterRealName), "", "", bankCode, Common.Pay.af.afUtil.EncodeBase64(Encoding.UTF8, with.BankName), with.ResponseMoney.Value.ToString("f2"), this.af_pay_pwd);
                    src += "&key=" + af_key;
                    string sign = Common.Pay.af.afUtil.MD5(src, Encoding.UTF8);
                    src += "&sign=" + sign;

                    string responseStr = Common.Pay.af.afUtil.HttpPost(af_url + "?" + src.ToString(), "", Encoding.UTF8);
                    var json = Newtonsoft.Json.Linq.JObject.Parse(responseStr);
                    //代付申请成功
                    if (json["result_code"].ToString() == "000000")
                    {
                        var result = base.FundClient.ReuqestWithdraw(item, CurrentUser1.UserToken, this.af_merchant_no.ToString());
                    }
                    else
                    {
                        msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, json["result_msg"].ToString());
                    }
                }
                return Json(new { IsSuccess = true, Msg = msg });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        #endregion

        #region "新付代付"

        string xinfu_merchNo = System.Configuration.ConfigurationManager.AppSettings["xinfu_merchNo"].ToString() == "" ? "20180126001676" : System.Configuration.ConfigurationManager.AppSettings["xinfu_merchNo"].ToString();
        /// <summary>
        /// 新付代付
        /// </summary>
        /// <returns></returns>
        public JsonResult CompleteWithdrawALL_XF()
        {
            try
            {
                string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");

                string xinfu_key = System.Configuration.ConfigurationManager.AppSettings["xinfu_key"].ToString() == "" ? "b69c16776291781f9fbf9036f6dd5621" : System.Configuration.ConfigurationManager.AppSettings["xinfu_key"].ToString();
                string xinfu_Url = System.Configuration.ConfigurationManager.AppSettings["xinfu_Url"].ToString() == "" ? "http://trade.7xinpay.com/cgi-bin/netpayment/pay_gate.cgi" : System.Configuration.ConfigurationManager.AppSettings["xinfu_Url"].ToString();
                string xinfu_NoticeUrl = System.Configuration.ConfigurationManager.AppSettings["xinfu_NoticeUrl"].ToString() == "" ? "http://paytz.198w.com/XFDFNotifyUrl" : System.Configuration.ConfigurationManager.AppSettings["xinfu_NoticeUrl"].ToString();
                string[] arrs = orderIds.Split(',');
                if (arrs.Length == 0)
                {
                    return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
                }
                string msg = "已提交\r\n";
                String TempRSAStr = string.Empty;//明细串
                foreach (var item in arrs)
                {
                    var with = base.FundClient.GetWithdrawById(item, CurrentUser1.UserToken);
                    //检查是否支持该银行
                    string bankCode = Common.Pay.xingfu.xinfupay.checkBankName(with.BankName);
                    if (bankCode == "0")
                    {
                        msg += string.Format("该订单提交失败,原因是新付暂不支持该银行打款,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
                        continue;
                    }
                    //支行名字最少4位 且满足比如:xx支行
                    if (with.BankSubName.ToString().Length < 4 || with.BankSubName.LastIndexOf("支行") < 0)
                    {
                        with.BankSubName = with.BankSubName + "支行";
                    }
                    //MD5签名串
                    string src = string.Format(@"apiName=SINGLE_ENTRUST_SETT&apiVersion=1.0.0.2&platformID={0}&merchNo={1}&orderNo={2}&tradeDate={3}&merchUrl={4}&merchParam={5}&bankAccNo={6}&bankAccName={7}&bankCode={8}&bankName={9}&province={10}&city={11}&branchBankNo={12}&Amt={13}&tradeSummary={14}",
                        xinfu_merchNo, xinfu_merchNo, with.OrderId, DateTime.Now.ToString("yyyyMMdd"), xinfu_NoticeUrl, CurrentUser1.UserId, with.BankCardNumber, with.RequesterRealName, bankCode, with.BankSubName, with.ProvinceName, with.CityName, "", with.ResponseMoney.Value.ToString("f2"), "");

                    //src += xinfu_key;
                    string sign = Common.Pay.HttpHelp.MD5(src + xinfu_key, Encoding.UTF8);
                    src += "&signMsg=" + sign;

                    string responseStr = Common.Pay.HttpHelp.HttpPost(xinfu_Url, src);
                    var xml = System.Xml.Linq.XDocument.Parse(responseStr);
                    var xml_root = xml.Root;
                    var xml_root_respData = xml_root.Element("respData");
                    var respCode = xml_root_respData.Element("respCode").Value;
                    //代付申请成功
                    if (respCode == "00")
                    {
                        var result = base.FundClient.ReuqestWithdraw(item, CurrentUser1.UserToken, xinfu_merchNo.ToString());
                    }
                    else
                    {
                        msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, xml_root_respData.Element("respDesc").Value);
                    }
                }
                return Json(new { IsSuccess = true, Msg = msg });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        #endregion

        #endregion
    }

    public class RemarkInfo
    {
        public int Index { get; set; }
        public bool Value { get; set; }
        public string Message { get; set; }
    }
}