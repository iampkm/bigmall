using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using Newtonsoft;
using Newtonsoft.Json.Linq;
namespace Guoc.BigMall.Infrastructure.Extension
{
    public static class StringExtension
    {
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// 把excel格式数据转换成 string,decimal 字典
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, decimal> ToDecimalDic(this string value)
        {
            Dictionary<string, decimal> dicProductPrice = new Dictionary<string, decimal>(1000);
            string[] productIdArray = value.Trim('\n').Split('\n');
            foreach (var item in productIdArray)
            {
                if (item.Contains("\t"))
                {
                    string[] parentIDAndQuantity = item.Split('\t');
                    if (!dicProductPrice.ContainsKey(parentIDAndQuantity[0].Trim()))
                    {
                        decimal quantity = 0m;
                        decimal.TryParse(parentIDAndQuantity[1], out quantity);
                        dicProductPrice.Add(parentIDAndQuantity[0].Trim(), quantity);
                    }
                }
                else
                {
                    if (!dicProductPrice.ContainsKey(item.Trim()))
                    {
                        dicProductPrice.Add(item.Trim(), 0);
                    }
                }
            }

            return dicProductPrice;
        }
        /// <summary>
        /// 把excel格式数据转换成 string,int 字典
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, int> ToIntDic(this string value)
        {
            Dictionary<string, int> dicProductPrice = new Dictionary<string, int>(1000);
            string[] productIdArray = value.Trim('\n').Split('\n');
            foreach (var item in productIdArray)
            {
                if (item.Contains("\t"))
                {
                    string[] parentIDAndQuantity = item.Split('\t');
                    if (!dicProductPrice.ContainsKey(parentIDAndQuantity[0].Trim()))
                    {
                        int quantity = 0;
                        int.TryParse(parentIDAndQuantity[1], out quantity);
                        dicProductPrice.Add(parentIDAndQuantity[0].Trim(), quantity);
                    }
                }
                else
                {
                    if (!dicProductPrice.ContainsKey(item.Trim()))
                    {
                        dicProductPrice.Add(item.Trim(), 0);
                    }
                }
            }

            return dicProductPrice;
        }

        public static bool IsNumeric(this string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        /// <summary>
        /// 创建二维码/条形码
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string CreateBarCodeUri(this string content, string type)
        {
            // LogWriter.WriteLog("content:" + content + " type:" + type);
            try
            {
                Bitmap bitmap = null;
                string enCodeString = "http://m.sjgo365.com/{0}.html";
                switch (type)
                {
                    case "barCode":
                        enCodeString = content;
                        BarCodeHelper barCodeHelper = new BarCodeHelper();
                        bitmap = barCodeHelper.GetCodeImage(content, BarCodeHelper.Encode.Code128A);
                        break;
                    case "QRCode":
                        QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                        qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//二维码编码方式
                        qrCodeEncoder.QRCodeScale = 2;//每个小方格的宽度
                        qrCodeEncoder.QRCodeVersion = 5;//二维码版本号
                        qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;//纠错码等级
                        bitmap = qrCodeEncoder.Encode(string.Format(enCodeString, content), Encoding.UTF8);
                        break;
                }
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                string strUrl = "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());
                bitmap.Dispose();
                ms.Dispose();
                return strUrl;
            }
            catch (Exception ex)
            {
                // LogWriter.WriteLog("异常信息：" + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 支付方式 生成签名
        /// </summary>
        /// <param name="data">生成签名的数据(json字符串)</param>
        /// <returns></returns>
        public static string SignEncrypt(this string data, string signKey)
        {
            JObject json = JObject.Parse(data);

            //遍历json的节点，并获取节点的值，并根据属性排升序
            var dict = new SortedDictionary<string, string>();
            var properties = json.Properties().ToList();
            foreach (var item in properties)
            {
                dict.Add(item.Name, item.Value.ToString());
            }

            //组装被加密的字符串
            string signString = string.Empty;
            foreach (var item in dict)
            {
                signString += string.Format("{0}={1}&", item.Key, item.Value);
            }
            signString += signKey;

            //对数据进行MD5加密
            string sign = MD5Encrypt(signString);
            return sign;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="strText">加密内容</param>
        /// <param name="toLower">是否转换成小写</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static string MD5Encrypt(string strText, bool toLower = false, Encoding encoding = null)
        {
            string code = string.Empty;
            if (encoding == null)
                encoding = Encoding.UTF8;

            var md5 = System.Security.Cryptography.MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择
            byte[] s = md5.ComputeHash(encoding.GetBytes(strText));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                if (toLower)
                    code += s[i].ToString("x2");
                else
                    code += s[i].ToString("X2");
            }
            return code;
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool NotNullOrEmpty(this string value)
        {
            return !IsNullOrEmpty(value);
        }
    }
}
