using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.FTP;
using System.Threading;

namespace Guoc.BigMall.Infrastructure.Utils
{
    public class CutAndUploadImage
    {
        string _server;
        string _account;
        string _password;
        public CutAndUploadImage(string ftpService,string ftpAccount,string ftpPassword)
        {
            this._server = ftpService;
            this._account = ftpAccount;
            this._password = ftpPassword;
        }       
        public byte[] Cut(Image image, int width, int height)
        {
            Bitmap m_hovertreeBmp = new Bitmap(width, height);
            Graphics m_HvtGr = Graphics.FromImage(m_hovertreeBmp);
            m_HvtGr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            m_HvtGr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            m_HvtGr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            Rectangle rectDestination = new Rectangle(0, 0, width, height);
            int m_width, m_height;
            if (image.Width * height > image.Height * width)
            {
                m_height = image.Height;
                m_width = (image.Height * width) / height;
            }
            else
            {
                m_width = image.Width;
                m_height = (image.Width * height) / width;
            }
            m_HvtGr.DrawImage(image, rectDestination, 0, 0, m_width, m_height, GraphicsUnit.Pixel);

            ImageConverter imgconv = new ImageConverter();
            byte[] imageBytes = (byte[])imgconv.ConvertTo(m_hovertreeBmp, typeof(byte[]));

            return imageBytes;
        }
        public int UploadMainImage(byte[] imageBytes, string code, string imgNameFormat) 
        {
            var ftp800 = new FtpClient(_server, _account, _password);
            var ftp450 = new FtpClient(_server, _account, _password);
            var ftp200 = new FtpClient(_server, _account, _password);
            var ftp80 = new FtpClient(_server, _account, _password);
            var mainName800 = @"XZT\Product\" + code + @"\" + @"800\";
            var mainName450 = @"XZT\Product\" + code + @"\" + @"450\";
            var mainName200 = @"XZT\Product\" + code + @"\" + @"200\";
            var mainName80 = @"XZT\Product\" + code + @"\" + @"80\";
            //var fileName = @"XZT\Product\" + code + @"\";
            try
            {
                imgNameFormat = (Int32.Parse(imgNameFormat.Substring(0, imgNameFormat.IndexOf(".")))).ToString() + ".jpg";
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms);
                byte[] imageBytes800 = Cut(image, 800, 800);
                MemoryStream ms800 = new MemoryStream(imageBytes800, 0, imageBytes800.Length);
                byte[] imageBytes450 = Cut(image, 450, 450);
                MemoryStream ms450 = new MemoryStream(imageBytes450, 0, imageBytes450.Length);
                byte[] imageBytes200 = Cut(image, 200, 200);
                MemoryStream ms200 = new MemoryStream(imageBytes200, 0, imageBytes200.Length);
                byte[] imageBytes80 = Cut(image, 80, 80);
                MemoryStream ms80 = new MemoryStream(imageBytes80, 0, imageBytes80.Length);
                ms800.Write(imageBytes800, 0, imageBytes800.Length);
                ms450.Write(imageBytes450, 0, imageBytes450.Length);
                ms200.Write(imageBytes200, 0, imageBytes200.Length);
                ms80.Write(imageBytes80, 0, imageBytes80.Length);

                //if (!ftp800.CheckDirectory(fileName)) 
                //{
                    ftp800.CreateDirectory(mainName800);
                    ftp450.CreateDirectory(mainName450);
                    ftp200.CreateDirectory(mainName200);
                    ftp80.CreateDirectory(mainName80);                
                //}

                var remotePath800 = Path.Combine(mainName800, imgNameFormat);
                var remotePath450 = Path.Combine(mainName450, imgNameFormat);
                var remotePath200 = Path.Combine(mainName200, imgNameFormat);
                var remotePath80 = Path.Combine(mainName80, imgNameFormat);
                ftp800.Upload(ms800, remotePath800);
                ftp450.Upload(ms450, remotePath450);
                ftp200.Upload(ms200, remotePath200);
                ftp80.Upload(ms80, remotePath80);
                //_productService.CreatePicture(imgNameFormat, name[i], id);
                ms.Close();
                ms.Dispose();
                ms800.Close();
                ms800.Dispose();
                ms450.Close();
                ms450.Dispose();
                ms200.Close();
                ms200.Dispose();
                ms80.Close();
                ms80.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            int count = ftp800.GetFiles(mainName800).Length;
            return count;
        }
        public int UploadDetailImage(byte[] imageBytes, string code, string imgNameFormat)
        {
            var ftp = new FtpClient(_server, _account, _password);
            var detailName = @"XZT\Product\" + code + @"\" + @"Detail\";
            //var fileName = @"XZT\Product\" + code + @"\";
            try
            {
                imgNameFormat = (Int32.Parse(imgNameFormat.Substring(0, imgNameFormat.IndexOf(".")))).ToString() + ".jpg";
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                //if(!ftp.CheckDirectory(fileName))
                    ftp.CreateDirectory(detailName);
                var remotePath = Path.Combine(detailName, imgNameFormat);
                ftp.Upload(ms, remotePath);
                ms.Close();
                ms.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            int count = ftp.GetFiles(detailName).Length;
            return count;
        }

        public int UploadFile(byte[] fileBytes, string code, string fileNameFormat,string filePath)
        {
            var ftpClient = new FtpClient(_server, _account, _password);
            if (string.IsNullOrEmpty(filePath)) return 0;
            try
            {
                ftpClient.CreateDirectory(filePath);
                var remotePath = Path.Combine(filePath, fileNameFormat);
                MemoryStream ms = new MemoryStream(fileBytes, 0, fileBytes.Length);
                ftpClient.Upload(ms, remotePath);
                ms.Close();
                ms.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            int count = ftpClient.GetFiles(filePath).Length;
            return count;
        }
        public List<int> GetMainImage(string code) 
        {
            var ftp = new FtpClient(_server, _account, _password);
            var mainName = @"XZT\Product\" + code + @"\" + @"800\";
            string[] images = ftp.GetFiles(mainName);
            List<int> newImages = new List<int>();
            foreach (string image in images)
            {
                int imageName = Int32.Parse(image.Substring(0, image.IndexOf(".")));
                newImages.Add(imageName);
            }
            newImages.Sort();

            return newImages;
        }
        public List<int> GetFiles(string code,string filePath)
        {
            var ftp = new FtpClient(_server, _account, _password);
            //var filePath = @"XZT\Product\" + code + @"\" + @"Video\Main\";
            string[] files = ftp.GetFiles(filePath);
            List<int> newImages = new List<int>();
            foreach (string image in files)
            {
                int fileName = Int32.Parse(image.Substring(0, image.IndexOf(".")));
                newImages.Add(fileName);
            }
            newImages.Sort();

            return newImages;
        }
        public int GetFilesCount(string filePath)
        {
            var ftpClient = new FtpClient(_server, _account, _password);
            int count = ftpClient.GetFiles(filePath).Length;
            return count;
        }
        public int DeleteMainImage(string code, string imgNameFormat)
        {
            var ftp = new FtpClient(_server, _account, _password);
            var mainName800 = @"XZT\Product\" + code + @"\" + @"800\";
            var remotePath800 = Path.Combine(mainName800, imgNameFormat);
            ftp.DeleteFile(remotePath800);

            var mainName450 = @"XZT\Product\" + code + @"\" + @"450\";
            var remotePath450 = Path.Combine(mainName450, imgNameFormat);
            ftp.DeleteFile(remotePath450);

            var mainName200 = @"XZT\Product\" + code + @"\" + @"200\";
            var remotePath200 = Path.Combine(mainName200, imgNameFormat);
            ftp.DeleteFile(remotePath200);

            var mainName80 = @"XZT\Product\" + code + @"\" + @"80\";
            var remotePath80 = Path.Combine(mainName80, imgNameFormat);
            ftp.DeleteFile(remotePath80);

            int count = ftp.GetFiles(mainName800).Length;
            return count;
        }
        public int DeleteFtpFile(string code, string filePath, IList<string> fileNameList)
        {
            try
            {
                var ftp = new FtpClient(_server, _account, _password);
                int oldCount = ftp.GetFiles(filePath).Length;
                int counter = 0;
                int timer = 0;
                foreach (string fileName in fileNameList)
                {
                    var remotePath = Path.Combine(filePath, fileName);
                    ftp.DeleteFile(remotePath);
                    counter++;
                }
                int count = ftp.GetFiles(filePath).Length;
                while (oldCount - counter < count)
                {
                    Thread.Sleep(500);
                    count = ftp.GetFiles(filePath).Length;
                    timer = timer + 500;                    
                    if (timer >= 5500)
                    {
                        count = oldCount - counter;
                        break;
                    }
                }
                return count;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public List<int> GetDetailImage(string code)
        {
            var ftp = new FtpClient(_server, _account, _password);
            var detailName = @"XZT\Product\" + code + @"\" + @"Detail\";
            string[] images = ftp.GetFiles(detailName);
            List<int> newImages = new List<int>();
            foreach(string image in images)
            {
                int imageName = Int32.Parse(image.Substring(0, image.IndexOf(".")));
                newImages.Add(imageName);
            }
            newImages.Sort();

            return newImages;
        }
        public int DeleteDetailImage(string code, string imgNameFormat)
        {
            var ftp = new FtpClient(_server, _account, _password);
            var detailName = @"XZT\Product\" + code + @"\" + @"Detail\";
            var remotePath = Path.Combine(detailName, imgNameFormat);
            ftp.DeleteFile(remotePath);

            int count = ftp.GetFiles(detailName).Length;
            return count;
        }
        public int DeleteDetailImages(string code, string imageNames)
        {
            var ftp = new FtpClient(_server, _account, _password);
            var detailName = @"XZT\Product\" + code + @"\" + @"Detail\";
            var imgNames = imageNames.Split(',');
            foreach (string imgName in imgNames)
            {
                var remotePath = Path.Combine(detailName, imgName);
                ftp.DeleteFile(remotePath);
            }
            int count = ftp.GetFiles(detailName).Length;
            return count;
        }

        /// <summary>
        /// <param name="imageBytes">图片字节数组</param>
        /// <param name="url">图片上传地址</param>
        /// <param name="imgNameFormat">图片名字</param>
        /// </summary>
        public void UploadImage(byte[] imageBytes, string url, string imgNameFormat)
        {
            var ftp = new FtpClient(_server, _account, _password);
            try
            {
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                //if(!ftp.CheckDirectory(fileName))
                ftp.CreateDirectory(url);
                var remotePath = Path.Combine(url, imgNameFormat);
                ftp.Upload(ms, remotePath);
                ms.Close();
                ms.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
