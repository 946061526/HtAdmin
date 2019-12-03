using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchBiz.Core;
using Common.JSON;
using System.Configuration;
using System.IO;
using Common.Lottery.OpenDataGetters;
using System.Text;
using Common.Log;

namespace HTAdmin.jsonManager
{
    /// <summary>
    /// 订单日志记录器
    /// </summary>
    [Serializable]
    public class OrderLogWriter : ILogWriter
    {
        public OrderLogWriter(HttpServerUtilityBase service)
        {
            Service = service;
        }

        #region 属性
        /// <summary>
        /// Service请求接口
        /// </summary>
        public static HttpServerUtilityBase Service
        {
            get;
            set;
        }
        #endregion

        #region 功能函数
        /// <summary>
        /// 读取物理文件路径
        /// </summary>
        /// <param name="fileName">文件物理地址</param>
        /// <returns>文件内容</returns>
        public static string ReadFileString(string fileName)
        {
            using (var sr = new StreamReader(fileName))
            {
                return sr.ReadToEnd();
            }
        }
        #endregion

        #region 文件路径

        /// <summary>
        /// 存储用户订单信息文件地址
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderId">订单号</param>
        /// <returns>信息文件地址</returns>
        private static string CreateFileName(string userId, string orderId)
        {
            var basedir = Service.MapPath("~/CacheData/Users/");
            return Path.Combine(basedir, userId.Substring(0, 3), userId, "Orders", DateTime.Today.ToString("yyyy-MM"), DateTime.Today.ToString("dd"), orderId.Split('|')[0] + ".log");
        }

        /// <summary>
        /// 上传订单文件路径
        /// </summary>
        /// <param name="guid">guid编号</param>
        /// <returns>文件路径</returns>
        private static string UploadFileName(string guid)
        {
            var basedir = Service.MapPath("~/OrderData/UploadOrder/");
            return Path.Combine(basedir, DateTime.Today.ToString("yyyy-MM"), DateTime.Today.ToString("dd"), guid + ".inf");
        }

        /// <summary>
        /// 过滤选号路径
        /// </summary>
        /// <param name="guid">guid编号</param>
        /// <param name="file">文件名称</param>
        /// <returns>文件路径</returns>
        private static string FilterFileName(string guid, string file)
        {
            var basedir = Service.MapPath("~/OrderData/FilterOrder/");
            return Path.Combine(basedir, DateTime.Today.ToString("yyyy-MM"), DateTime.Today.ToString("dd"), guid, file + ".txt");
        }
        #endregion

        #region 将消息添加到日志文件
        //记录投注信息日志
        private void AppendTextToFile(string text, string userId, string orderId)
        {
            string FileName = CreateFileName(userId, orderId);
            string path = Path.GetDirectoryName(FileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamWriter sw = System.IO.File.AppendText(FileName))
            {
                sw.WriteLine(text);
                sw.Flush();
            }
        }

        //记录上传订单信息
        private void AppendTextToFile(string text, string guid)
        {
            string FileName = UploadFileName(guid);
            string path = Path.GetDirectoryName(FileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamWriter sw = System.IO.File.AppendText(FileName))
            {
                sw.WriteLine(text);
                sw.Flush();
            }
        }

        //记录过滤号码信息
        private void AppendTextToFile_Filter(string text, string guid, string file)
        {
            string FileName = FilterFileName(guid, file);
            string path = Path.GetDirectoryName(FileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamWriter sw = System.IO.File.AppendText(FileName))
            {
                sw.WriteLine(text);
                sw.Flush();
            }
        }

        #endregion

        #region 订单信息日志记录，订单上传信息记录

        private static object lockObj = new object();

        //记录订单日志
        public void Write(string userId, string orderId, LogType logType, string logMsg, string detail)
        {
            lock (lockObj)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("    [{0}][{1:yyyy-MM-dd HH:mm:ss}] {2}", logType, DateTime.Now, logMsg);
                sb.AppendLine();
                sb.AppendLine();
                if (!string.IsNullOrEmpty(detail))
                {
                    sb.AppendLine("    " + detail);
                }
                AppendTextToFile(sb.ToString(), userId, orderId);
            }
        }
        public void Write(string userId, string orderId, Exception exception)
        {
            lock (lockObj)
            {
                if (exception == null)
                {
                    return;
                }
                Write(userId, orderId, LogType.Error, exception.Message, exception.ToString());
            }
        }

        /// <summary>
        /// 记录上传订单信息
        /// </summary>
        /// <param name="orderText">上传的号码信息</param>
        public string Write(string codeInfo)
        {
            var guid = Guid.NewGuid().ToString();
            lock (lockObj)
            {
                if (!string.IsNullOrEmpty(codeInfo))
                {
                    AppendTextToFile(codeInfo, guid);
                }
            }
            return UploadFileName(guid); ;
        }

        #endregion

        #region 过滤选号信息记录
        /// <summary>
        /// 记录过滤号码信息
        /// </summary>
        /// <param name="anteList">过滤号码的文本信息</param>
        public string Write(string orderText, string guid, string category = "after")
        {
            var file = "OrderInfo_AfterFilter";
            switch (category.ToLower())
            {
                case "before":
                    file = "OrderInfo_BeforeFilter";
                    break;
                case "after":
                    file = "OrderInfo_AfterFilter";
                    break;
                case "detail":
                    file = "OrderInfo_DetailFilter";
                    break;
                case "filter":
                    file = "OrderInfo_FilterList";
                    break;
            }
            lock (lockObj)
            {
                if (!string.IsNullOrEmpty(orderText))
                {
                    AppendTextToFile_Filter(orderText, guid, file);
                }
            }
            return FilterFileName(guid, file);
        }

        private static Dictionary<string, string> _filterList = new Dictionary<string, string>();
        /// <summary>
        /// 更新过滤KEY列表信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void UpdateFilterList(string key, string value = "")
        {
            try
            {
                if (_filterList.ContainsKey(key))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        _filterList.Remove(key);
                    }
                    else
                    {
                        _filterList[key] = value;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _filterList.Add(key, value);
                    }
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// 获取过滤选号GUID级目录路径
        /// </summary>
        /// <param name="fullPath">过滤选号全路径</param>
        /// <returns></returns>
        public string GetFilterBaseFilePath(string fullPath)
        {
            try
            {
                var fullPathArr = fullPath.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                return fullPath.EndsWith(".txt") ? string.Join("\\", fullPathArr.Take(fullPathArr.Length - 1)) : fullPath;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 根据缓存值获取过滤文件路径 - 用于提交投注
        /// </summary>
        /// <param name="key">过滤KEY</param>
        /// <param name="guid">过滤GUID</param>
        /// <param name="category">过滤文件类型</param>
        /// <returns></returns>
        public string GetFilterFilePath(string key, string guid, string category = "after")
        {
            if (!_filterList.ContainsKey(key))
            {
                throw new Exception("您还未进行过滤，请先选号过滤。");
            }
            var val = _filterList[key];
            if (string.IsNullOrEmpty(val) || !val.Contains("|"))
            {
                throw new Exception("您的过滤已过期或已被覆盖，请重新过滤。");
            }
            var paths = val.Split('|');
            if (guid != paths[0])
            {
                throw new Exception("您的过滤已过期或已被覆盖，请重新过滤。");
            }

            var file = "OrderInfo_AfterFilter";
            switch (category.ToLower())
            {
                case "before":
                    file = "OrderInfo_BeforeFilter";
                    break;
                case "after":
                    file = "OrderInfo_AfterFilter";
                    break;
                case "detail":
                    file = "OrderInfo_DetailFilter";
                    break;
            }
            return FilterFileName(guid, file);
        }
        #endregion
    }

    /// <summary>
    /// 删除文件和目录
    /// </summary> 
    public class Cleaner
    {
        /// <summary>
        /// 删除指定目录以及该目录下所有文件
        /// </summary>
        /// <param name="dir">欲删除文件或者目录的路径</param> 
        public static void Clean(string dir)
        {
            CleanFiles(dir);//第一次删除文件  
            CleanFiles(dir);//第二次删除目录 
        }
        private static void CleanFiles(string dir)
        {
            if (!Directory.Exists(dir))
            {
                File.Delete(dir);
                return;
            }
            else
            {
                string[] dirs = Directory.GetDirectories(dir);
                string[] files = Directory.GetFiles(dir);
                if (0 != dirs.Length)
                {
                    foreach (string subDir in dirs)
                    {
                        if (null == Directory.GetFiles(subDir))
                        {
                            Directory.Delete(subDir);
                            return;
                        }
                        else CleanFiles(subDir);
                    }
                }
                if (0 != files.Length)
                {
                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }
                }
                else Directory.Delete(dir);
            }
        }
    }
}