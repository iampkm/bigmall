using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.FTP
{
    public interface IFtpService
    {
        bool Upload(MemoryStream memoryStream, string remotePath);
        bool DeleteFile(string remotePath);
        string[] GetFiles(string dirName = "");
        bool CreateDirectory(string dirName);

        string PictureService { get; }

        /// <summary>
        /// 等比例裁剪图片
        /// </summary>
        /// <param name="MemoryStream">图片内存流</param>
        /// <param name="width">裁剪后宽</param>
        /// <param name="height">裁剪后高</param>
        /// <returns></returns>
        byte[] CutPicture(MemoryStream memoryStream, int width, int height);
    }
}
