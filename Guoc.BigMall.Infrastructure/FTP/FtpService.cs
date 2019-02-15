using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.FTP;
using System.Configuration;
using System.IO;
using Guoc.BigMall.Infrastructure.Utils;
using System.Drawing;
namespace Guoc.BigMall.Infrastructure.FTP
{
   public class FtpService :IFtpService
    {
        FtpClient _ftpClient;
        string _server = "";
        string _account = "";
        string _password = "";
        string _pictureService = "";
        CutAndUploadImage _imageTool;
        public FtpService()
        {
            _server = ConfigurationManager.AppSettings["FTPServer"].ToString();
            _account = ConfigurationManager.AppSettings["FTPAccount"].ToString();
            _password = ConfigurationManager.AppSettings["FTPPassword"].ToString();
            _pictureService = ConfigurationManager.AppSettings["FTPServerImg"].ToString();
            _ftpClient = new FtpClient(_server, _account, _password);
           // _imageTool = new CutAndUploadImage();
        }


        public bool Upload(MemoryStream memoryStream, string remotePath)
        {
            return _ftpClient.Upload(memoryStream, remotePath);
        }

        public bool DeleteFile(string remotePath)
        {
            return _ftpClient.DeleteFile(remotePath);
        }

        public string[] GetFiles(string dirName = "")
        {
            return _ftpClient.GetFiles(dirName);
        }

        public bool CreateDirectory(string dirName)
        {
            return _ftpClient.CreateDirectory(dirName);
        }


        public string PictureService
        {
            get { return _pictureService; }
        }


        public byte[] CutPicture(MemoryStream memoryStream, int width, int height)
        {

            Image image = Image.FromStream(memoryStream);
            byte[] imageBytes800 = _imageTool.Cut(image, width, height);
            return imageBytes800;
        }
    }
}
