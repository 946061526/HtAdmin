using Common.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report

        #region 收支统计

        /// <summary>
        /// 收支统计表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult IncomeExpensesTotal()
        {
            var list = GlobalCache.ExternalClient.QueryIncomeExpensesTotalDs(1, "");
            ViewBag.List = list;

            var GameList = GlobalCache.GameIssuseClient.QueryAllGameList();
            ViewBag.GameList = GameList;

            return View();
        }

        /// <summary>
        /// 收支走势图数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetIncomeExpensesTotalJson()
        {
            var dimType = Request.Form["dimType"] == null ? "" : Request.Form["dimType"].ToString();
            var gameCode = Request.Form["gameCode"] == null ? "" : Request.Form["gameCode"].ToString();
            if (string.IsNullOrEmpty(dimType))
            {
                return Json(new { IsSuccess = false, Msg = "参数错误" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var list = GlobalCache.ExternalClient.QueryIncomeExpensesTotalDs(2, gameCode, Convert.ToInt32(dimType));
                var dataList = Json(list.DataList);
                var dataChart = Json(list.DataChart);

                return Json(new { IsSuccess = true, Msg = "查询成功", DataList = dataList, DataChart = dataChart }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region 首页

        public ActionResult Index()
        {
            var list = GlobalCache.ExternalClient.QueryIndexData(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
            ViewBag.List = list;
            return View();
        }

        /// <summary>
        /// 购彩用户分布
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetIndexProvinceUsersList()
        {
            return GetIndexDataList(2);
        }
        /// <summary>
        /// 彩种销售排行
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetIndexSaleTop10List()
        {
            return GetIndexDataList(3);
        }
        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="bizType">1.汇总 2.购彩用户分布 3.彩种销售排行</param>
        /// <returns></returns>
        private JsonResult GetIndexDataList(int bizType)
        {
            var dimType = Request.Form["dimType"] == null ? 1 : Convert.ToInt32(Request.Form["dimType"]);
            var date = GetDate(dimType);
            DateTime beginDate = date.Item1;
            DateTime endDate = date.Item2;
            var dataList = Json("{DataList:null}");
            try
            {

                if (bizType == 2)
                {
                    var list = GlobalCache.ExternalClient.QueryIndexProvinceUsers(beginDate, endDate).DataList;

                    #region test
                    //var list = new List<External.Core.HTReport.IndexProvinceUsers>();
                    //if (dimType == 1)
                    //{
                    //    list = new List<External.Core.HTReport.IndexProvinceUsers>() {
                    //        new External.Core.HTReport.IndexProvinceUsers(){name="西藏",value=1000},
                    //        new External.Core.HTReport.IndexProvinceUsers(){name="青海",value=2000},
                    //        new External.Core.HTReport.IndexProvinceUsers(){name="贵州",value=30000},
                    //        new External.Core.HTReport.IndexProvinceUsers(){name="山西",value=4000},
                    //        new External.Core.HTReport.IndexProvinceUsers(){name="上海",value=50000},
                    //    };
                    //}
                    //else
                    //{
                    //    list = new List<External.Core.HTReport.IndexProvinceUsers>() {
                    //        new External.Core.HTReport.IndexProvinceUsers(){name="辽宁",value=1000},
                    //        new External.Core.HTReport.IndexProvinceUsers(){name="北京",value=20000},
                    //        new External.Core.HTReport.IndexProvinceUsers(){name="浙江",value=30000},
                    //        new External.Core.HTReport.IndexProvinceUsers(){name="广东",value=40000},
                    //        new External.Core.HTReport.IndexProvinceUsers(){name="澳门",value=65000},
                    //    };
                    //}
                    #endregion

                    dataList = Json(list);
                }
                else if (bizType == 3)
                {
                    var list = GlobalCache.ExternalClient.QueryIndexSaleTop10(beginDate, endDate).DataList;
                    foreach (var item in list)
                    {
                        item.GameName = GetGameName(item.GameCode);
                    }

                    dataList = Json(list);
                }
                return Json(new { IsSuccess = true, Msg = "查询成功", DataList = dataList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 根据日期维度返回开始日期、结束日期
        /// </summary>
        /// <param name="dimType">日期维度: 1.今天, 2.昨天, 3.近7天, 4.近30天</param>
        /// <returns></returns>
        private Tuple<DateTime, DateTime> GetDate(int dimType)
        {
            DateTime sDate = DateTime.Today;
            DateTime eDate = DateTime.Today.AddDays(1);

            switch (dimType)
            {
                case 2://昨天
                    sDate = DateTime.Today.AddDays(-1);
                    eDate = DateTime.Today;
                    break;
                case 3://近7天
                    sDate = DateTime.Today.AddDays(-7);
                    eDate = DateTime.Today;
                    break;
                case 4://近30天
                    sDate = DateTime.Today.AddDays(-30);
                    eDate = DateTime.Today;
                    break;
            }
            return new Tuple<DateTime, DateTime>(sDate.Date, eDate.Date);
        }
        /// <summary>
        /// 根据彩种编码（玩法类型）获取彩种（玩法）名称
        /// </summary>
        /// <param name="gamecode">彩种编码</param>
        /// <returns>彩种（玩法）名称</returns>
        private static string GetGameName(string gamecode)
        {
            //根据彩种编号获取彩种名称
            switch (gamecode.ToUpper())
            {
                case "CQSSC": return "高频彩-重庆时时彩";
                case "GD11X5": return "高频彩-广东11选5";
                case "GDKLSF": return "高频彩-广东快乐十分";
                case "GXKLSF": return "高频彩-广西快乐十分";
                case "JSKS": return "高频彩-江苏快3";
                case "JX11X5": return "高频彩-江西11选5";
                case "JXSSC": return "高频彩-江西时时彩";
                case "SD11X5": return "高频彩-山东11选5";
                case "SDQYH": return "高频彩-山东群英会";
                case "CQ11X5": return "高频彩-重庆11选5";
                case "CQKLSF": return "高频彩-重庆快乐十分";
                case "DF6J1": return "高频彩-东方6+1";
                case "HBK3": return "高频彩-湖北快3";
                case "HC1": return "高频彩-好彩一";
                case "HD15X5": return "高频彩-华东15选5";
                case "HNKLSF": return "高频彩-湖南快乐十分";
                case "JLK3": return "高频彩-新快3";
                case "JSK3": return "高频彩-江苏快3";
                case "LN11X5": return "高频彩-辽宁11选5";
                case "SDKLPK3": return "高频彩-山东快乐扑克3";
                case "BJPK10": return "高频彩-北京赛车pk10";

                case "BJDC": return "竞技彩-北京单场";
                case "JCZQ": return "竞技彩-竞彩足球";
                case "JCLQ": return "竞技彩-竞彩篮球";
                case "CTZQ": return "竞技彩-传统足球";

                case "DLT": return "数字彩-大乐透";
                case "FC3D": return "数字彩-福彩3D";
                case "PL5": return "数字彩-排列五";
                case "SSQ": return "数字彩-双色球";
                case "PL3": return "数字彩-排列三";
                case "QLC": return "数字彩-七乐彩";
                case "QXC": return "数字彩-七星彩";
                default: return "";
            }
        }
        #endregion

    }
}