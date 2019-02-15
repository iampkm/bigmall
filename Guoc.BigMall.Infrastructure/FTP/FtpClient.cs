using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.FTP
{
    public class FtpClient
    {
        /// <summary>
        /// ftp根目录
        /// </summary>
        private string _baseUri;
        /// <summary>
        /// ftp工作目录
        /// </summary>
        private string _workUri;
        /// <summary>
        /// ftp登录账号
        /// </summary>
        private string _account;
        /// <summary>
        /// ftp登录密码
        /// </summary>
        private string _password;
        /// <summary>
        /// 是否开启PASV模式
        /// </summary>
        private bool _usePassive;

        /// <summary>
        /// 实例化FTP客户端
        /// </summary>
        /// <param name="server">FTP服务器</param>
        /// <param name="account">FTP登录账号</param>
        /// <param name="password">FTP登录密码</param>
        /// <param name="usePassive">是否开启PASV模式</param>
        public FtpClient(string server, string account, string password, bool usePassive = false)
        {
            this._baseUri = "ftp://" + server + "/";
            this._workUri = this._baseUri;
            this._account = account;
            this._password = password;
            this._usePassive = usePassive;
        }

        /// <summary>
        /// 设置工作目录
        /// </summary>
        /// <param name="dirName">ftp目录</param>
        public void SetWorkDirectory(string dirName = "")
        {
            dirName = dirName.Replace(@"\", "/").TrimEnd('/');
            if (!string.IsNullOrWhiteSpace(dirName))
                this.CreateDirectory(dirName);

            dirName = dirName + (dirName.Length > 0 ? "/" : "");
            this._workUri = this._baseUri + dirName;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="ms">文件流</param>
        /// <param name="remotePath">ftp文件路径</param>
        /// <returns></returns>
        public bool Upload(MemoryStream ms, string remotePath)
        {
            try
            {
                var request = this.CreateFtpWebRequest(WebRequestMethods.Ftp.UploadFile, remotePath);
                using (Stream stream = request.GetRequestStream())
                {
                    byte[] bytes = ms.ToArray();
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                    ms.Close();
                }
                return true;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="bytes">文件字节</param>
        /// <param name="remotePath">ftp文件路径</param>
        /// <returns></returns>
        public bool Upload(byte[] bytes, string remotePath)
        {
            try
            {
                var request = this.CreateFtpWebRequest(WebRequestMethods.Ftp.UploadFile, remotePath);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }
                return true;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">本地文件路径</param>
        /// <param name="remotePath">ftp文件路径</param>
        /// <returns></returns>
        public bool Upload(string filePath, string remotePath)
        {
            try
            {
                var bytes = System.IO.File.ReadAllBytes(filePath);
                var request = this.CreateFtpWebRequest(WebRequestMethods.Ftp.UploadFile, remotePath);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }
                return true;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 下载ftp文件
        /// </summary>
        /// <param name="remotePath">ftp文件路径</param>
        /// <param name="localPath">存放的本地文件路径</param>
        /// <returns></returns>
        public bool Download(string remotePath, string localPath)
        {
            var bytes = this.Download(remotePath);
            if (bytes == null || bytes.Length == 0)
                return false;

            System.IO.File.WriteAllBytes(localPath, bytes);
            return true;
        }

        /// <summary>
        /// 下载ftp文件
        /// </summary>
        /// <param name="remotePath">ftp文件路径</param>
        /// <returns></returns>
        public byte[] Download(string remotePath)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                var request = this.CreateFtpWebRequest(WebRequestMethods.Ftp.DownloadFile, remotePath);
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream reader = response.GetResponseStream())
                    {
                        byte[] buffer = new byte[128];
                        int count = 0; //实际读取的字节数
                        while ((count = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, count);
                        }
                        ms.Dispose();
                    }
                }
                return ms.ToArray();
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="remotePath">ftp文件路径</param>
        /// <returns></returns>
        public bool DeleteFile(string remotePath)
        {
            try
            {
                var request = this.CreateFtpWebRequest(WebRequestMethods.Ftp.DeleteFile, remotePath);
                using (var response = request.GetResponse()) { }
                
                return true;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建ftp文件夹
        /// </summary>
        /// <param name="dirName">文件夹名称</param>
        /// <returns></returns>
        public bool CreateDirectory(string dirName)
        {
            try
            {
                var request = this.CreateFtpWebRequest(WebRequestMethods.Ftp.MakeDirectory, dirName);
                using (var response = request.GetResponse()) { }
                return true;
            }
            catch (WebException ex)
            {
                var response = (System.Net.FtpWebResponse)ex.Response;
                if (Regex.IsMatch(response.StatusDescription, "directory already exists", RegexOptions.IgnoreCase))
                    return true;

                return false;
            }
        }

        /// <summary>
        /// 判断ftp文件夹是否存在
        /// </summary>
        /// <param name="dirName">文件夹名称</param>
        /// <returns></returns>
        public bool CheckDirectory(string dirName)
        {
            try
            {
                var request = this.CreateFtpWebRequest(WebRequestMethods.Ftp.ListDirectoryDetails, dirName);
                using (var response = request.GetResponse()) { }
                return true;
            }
            catch (WebException ex)
            {
                var response = (System.Net.FtpWebResponse)ex.Response;
                if (Regex.IsMatch(response.StatusDescription, "directory already exists", RegexOptions.IgnoreCase))
                    return true;

                return false;
            }
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="remotePath">ftp目录</param>
        /// <returns></returns>
        public bool DeleteDirectory(string dirName)
        {
            try
            {
                var request = this.CreateFtpWebRequest(WebRequestMethods.Ftp.RemoveDirectory, dirName);
                using (var response = request.GetResponse()) { }
                return true;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取ftp目录中的文件
        /// 备注：慎用！！！！数量越多，执行越慢
        /// </summary>
        /// <param name="dirName">ftp目录</param>
        /// <returns></returns>
        public string[] GetFiles(string dirName = "")
        {
            var list = this.GetDetails(dirName);
            var fileList = list.Where(o => o.Type == FtpDetailType.File).Select(o => o.Name);
            return fileList.ToArray();
        }

        /// <summary>
        /// 获取ftp目录中的文件夹
        /// 备注：慎用！！！！数量越多，执行越慢
        /// </summary>
        /// <param name="dirName">ftp目录</param>
        /// <returns></returns>
        public string[] GetDirectories(string dirName = "")
        {
            var list = this.GetDetails(dirName);
            var fileList = list.Where(o => o.Type == FtpDetailType.Directory).Select(o => o.Name);
            return fileList.ToArray();
        }

        /// <summary>
        /// 获取ftp目录中的文件和文件夹
        /// 备注：慎用！！！！数量越多，执行越慢，大约每个文件或文件夹消耗1毫秒
        /// </summary>
        /// <param name="dirName">ftp目录</param>
        /// <returns></returns>
        public FtpDetailInfo[] GetDetails(string dirName = "")
        {
            var list = new List<FtpDetailInfo>();
            try
            {
                var request = this.CreateFtpWebRequest(WebRequestMethods.Ftp.ListDirectoryDetails, dirName);
                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var line = reader.ReadLine();
                    while (line != null)
                    {
                        var detail = this.ResolveDetail(line);
                        if (detail != null)
                            list.Add(detail);

                        line = reader.ReadLine();
                    }
                }
                response.Dispose();
            }
            catch (WebException ex)
            {
                throw ex;
            }
            return list.ToArray();
        }

        /// <summary>
        /// 解析ftp目录中的文件和文件夹
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        private FtpDetailInfo ResolveDetail(string desc)
        {
            if (string.IsNullOrWhiteSpace(desc))
                return null;

            #region 解析类型
            var info = new FtpDetailInfo();
            var list = Regex.Split(desc, "ftp", RegexOptions.IgnoreCase);
            var typeStr = list[0].ToLower();
            if (typeStr.IndexOf("drwxr-xr-x") == 0)
                info.Type = FtpDetailType.Directory;
            else if (typeStr.IndexOf("-rw-r--r--") == 0)
                info.Type = FtpDetailType.File;
            #endregion

            #region 解析大小
            var array = Regex.Split(list[2].TrimStart(' '), @"\s");
            info.Size = Convert.ToInt32(array[0]);
            #endregion

            #region 解析名称
            int index = 4;
            var tempList = new List<string>();
            for (int i = 0; i < array.Length; i++)
            {
                if (i < index)
                {
                    if (array[i] == "")
                        index++;
                }
                else
                {
                    tempList.Add(array[i]);
                }
            }
            info.Name = string.Join(" ", tempList);
            #endregion

            return info;
        }

        /// <summary>
        /// 创建ftp文件传输请求
        /// </summary>
        /// <param name="ftpMethod">ftp方式</param>
        /// <param name="remotePath">ftp路径</param>
        /// <returns></returns>
        private FtpWebRequest CreateFtpWebRequest(string ftpMethod, string remotePath = null)
        {
            if (remotePath != null)
                remotePath = Regex.Replace(remotePath.Trim('\\').Trim('/').Replace(@"\", "/"), "/+", "/");

            var request = (FtpWebRequest)WebRequest.Create(this._workUri + remotePath);
            request.Credentials = new NetworkCredential(this._account, this._password);
            request.Method = ftpMethod;
            request.UsePassive = this._usePassive;
            request.UseBinary = true;
            request.Timeout = 1800000;
            return request;
        }
    }

    /// <summary>
    /// ftp子项
    /// </summary>
    public class FtpDetailInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public FtpDetailType Type { get; set; }
    }

    /// <summary>
    /// ftp子项类型
    /// </summary>
    public enum FtpDetailType
    {
        /// <summary>
        /// 文件
        /// </summary>
        File = 0,
        /// <summary>
        /// 文件夹
        /// </summary>
        Directory = 1,
    }
}
