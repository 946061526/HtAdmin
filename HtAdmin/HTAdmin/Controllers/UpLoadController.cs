using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Net;
using System.Text;
using System.Configuration;
using System.Data;

namespace HTAdmin.Controllers
{
    public class UpLoadController : BaseController
    {

        [HttpPost]
        public ContentResult UpLoadImage()
        {
            try
            {
                var file = Request.Files["imgFile"];
                string nameImg = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string upLoadFile = ConfigurationManager.AppSettings["UpLoadFile"].ToString();
                string upLoadPostPath = ConfigurationManager.AppSettings["UpLoadPostPath"].ToString();
                string siteName = ConfigurationManager.AppSettings["WebSiteEName"].ToString();
                nameImg += file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                string url = string.Format(@"{0}/{1}{2}", UploadFileRequestUrl, siteName, Path.Combine(upLoadFile, nameImg));

                upLoadFile = "/" + ConfigurationManager.AppSettings["WebSiteEName"].ToString() + upLoadFile;

                string postUrl = string.Format("{0}{1}?filename={2}&upLoadFile={3}", resourceSitePostUrl, upLoadPostPath, nameImg, upLoadFile);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
                request.Method = "POST";
                request.AllowAutoRedirect = false;
                request.ContentType = "multipart/form-data";
                byte[] bytes = new byte[file.InputStream.Length];
                file.InputStream.Read(bytes, 0, (int)file.InputStream.Length);
                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse respon = (HttpWebResponse)request.GetResponse();
                Hashtable hash = new Hashtable();
                hash["error"] = 0;
                hash["url"] = url;
                return Content(System.Web.Helpers.Json.Encode(hash), "text/html; charset=UTF-8");
            }
            catch (Exception ex)
            {
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("UploadFiles", "-Admin_Upload", ex);
                throw ex;
            }
        }

        public string UpLoadFile(string fromFilePath, string toFilePath, string fileName)
        {
            fromFilePath = "/Xml/test.xml";
            toFilePath = "/UpLoad/uploadimages/";
            try
            {
                FileInfo fi = new FileInfo(fromFilePath);
                FileStream fs = fi.OpenRead();
                string resourceSiteUrl = ConfigurationManager.AppSettings["ResourceSiteUrl"].ToString();

                string postUrl = resourceSiteUrl + "/UpLoad/upload_json.aspx" + "?filename=" + fileName + "&upLoadFile=" + toFilePath;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
                request.Method = "POST";
                request.AllowAutoRedirect = false;
                request.ContentType = "multipart/form-data";
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);

                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse respon = (HttpWebResponse)request.GetResponse();
                Hashtable hash = new Hashtable();
                hash["error"] = 0;
                hash["url"] = toFilePath + fileName;

                //return Content(System.Web.Helpers.Json.Encode(hash), "text/html; charset=UTF-8");
                return "";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 文件上传 zwc.20180910
        /// </summary>
        /// <param name="type">1.Android 2.IOS</param>
        /// <param name="ver">版本号</param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult UploadFileNew()
        {
            Hashtable hash = new Hashtable();
            hash["code"] = 0;
            try
            {
                var file = Request.Files["uploadFile"];
                if (file == null)
                    throw new Exception("file is null");

                string type = Request["type"] == null ? "" : Request["type"].ToString();//1.Android 2.IOS
                string ver = Request["ver"] == null ? "" : Request["ver"].ToString();//版本号
                string newFileName = "";
                if (!string.IsNullOrWhiteSpace(type))
                    newFileName += type + "_";
                if (!string.IsNullOrWhiteSpace(ver))
                    newFileName += ver + "_";

                newFileName += DateTime.Now.ToString("yyMMddHHmmss");
                newFileName += file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();

                string SavePath = string.Format("/{0}/{1}/", WebSiteEName, FileSavePath); //比如：/HtAdmin/Upload/files/
                string fullPath = string.Format("{0}{1}{2}", UploadFileRequestUrl, SavePath, newFileName);//比如：http:img.cai.com/HtAdmin/Upload/files/newFileName

                //上传接口
                string postUrl = string.Format("{0}/{1}?filename={2}&savepath={3}", UploadFileRequestUrl, UploadFileAction, newFileName, SavePath);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
                request.Method = "POST";
                request.AllowAutoRedirect = false;
                request.ContentType = "multipart/form-data";
                byte[] bytes = new byte[file.InputStream.Length];
                file.InputStream.Read(bytes, 0, (int)file.InputStream.Length);
                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse respon = (HttpWebResponse)request.GetResponse();
                if (respon.StatusCode == HttpStatusCode.OK)
                {
                    hash["name"] = newFileName;
                    hash["fullpath"] = fullPath;
                }
                else
                {
                    hash["code"] = -1;
                    hash["msg"] = "error";
                }
            }
            catch (Exception ex)
            {
                hash["code"] = -1;
                hash["msg"] = ex.Message;
            }
            return Content(System.Web.Helpers.Json.Encode(hash), "text/html; charset=UTF-8");
        }

        /// <summary>
        /// 图片上传，保存url不带域，比如只保存到数据库:/HtAdmin/Upload/files/xxx.jpg
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ContentResult UploadImageNew()
        {
            Hashtable hash = new Hashtable();
            hash["code"] = 0;
            try
            {
                var file = Request.Files["imgFile"];
                if (file == null)
                    throw new Exception("file is null");

                string newFileName = "";

                newFileName += DateTime.Now.ToString("yyMMddHHmmss");
                newFileName += file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();

                string SavePath = string.Format("/{0}/{1}/", WebSiteEName, FileSavePath); //比如：/HtAdmin/Upload/files/
                string Path = string.Format("{0}{1}", SavePath, newFileName);//比如：/HtAdmin/Upload/files/newFileName
                string fullPath = string.Format("{0}{1}{2}", UploadFileRequestUrl, SavePath, newFileName);//比如：http://img.cai.com/HtAdmin/Upload/files/newFileName

                //上传接口
                string postUrl = string.Format("{0}/{1}?filename={2}&savepath={3}", UploadFileRequestUrl, UploadFileAction, newFileName, SavePath);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
                request.Method = "POST";
                request.AllowAutoRedirect = false;
                request.ContentType = "multipart/form-data";
                byte[] bytes = new byte[file.InputStream.Length];
                file.InputStream.Read(bytes, 0, (int)file.InputStream.Length);
                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse respon = (HttpWebResponse)request.GetResponse();
                if (respon.StatusCode == HttpStatusCode.OK)
                {
                    hash["name"] = newFileName;
                    hash["path"] = Path;
                    hash["fullpath"] = fullPath;
                }
                else
                {
                    hash["code"] = -1;
                    hash["msg"] = "error";
                }
            }
            catch (Exception ex)
            {
                hash["code"] = -1;
                hash["msg"] = ex.Message;
            }
            return Content(System.Web.Helpers.Json.Encode(hash), "text/html; charset=UTF-8");
        }
    }
}
