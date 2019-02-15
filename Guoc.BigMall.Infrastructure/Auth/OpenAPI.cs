using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.Auth
{
    /*
     * 开放api接口签名验证
     * 
     * 1.给app分配对应的key、secret

        2.Sign签名，调用API 时需要对请求参数进行签名验证，签名方式如下： 

            a. 按照请求参数名称将所有请求参数按照字母先后顺序排序得到：keyvaluekeyvalue...keyvalue  字符串如：将arong=1,mrong=2,crong=3 排序为：arong=1, crong=3,mrong=2  然后将参数名和参数值进行拼接得到参数字符串：arong1crong3mrong2。 

            b. 将secret加在参数字符串的头部后进行MD5加密 ,加密后的字符串需大写。即得到签名Sign
     * 3 请求的唯一性
     * 为了防止别人重复使用请求参数问题，我们需要保证请求的唯一性，就是对应请求只能使用一次，这样就算别人拿走了请求的完整链接也是无效的。
唯一性的实现：在如上的请求参数中，我们加入时间戳 ：timestamp（yyyyMMddHHmmss），同样，时间戳作为请求参数之一，也加入sign算法中进行加密。
新的api接口：
app调用：
http://api.test.com/getproducts?key=app_key&sign=BCC7C71CF93F9CDBDB88671B701D8A35&timestamp=201603261407&参数1=value1&参数2=value2.......
     */

    /// <summary>
    ///  开发的API接口签名验证
    /// </summary>
    public class OpenAPI
    {

        public static readonly OpenAPI Instance = new OpenAPI();

        
        int _timeout = 20;
        /// <summary>
        /// 默认20秒超时，单位秒
        /// </summary>
        public int RequestTimeOut {
            get {
                return this._timeout;
            }
        }

        public void SetTimeOut(int seconds)
        {
            if (seconds <= 0) {
                throw new FriendlyException("请求超时必须为正数");
            }
            this._timeout = seconds;
        }

        /// <summary>
        /// 验证请求
        /// </summary>
        /// <param name="request">请求参数</param>
        /// <returns></returns>
        public bool Validate(RequestParam request)
        {
            if (null == request)
            {
                throw new FriendlyException("请求参数不能为空");
            }
            // 验证请求参数
            ValidateTimestamp(request.TimeStamp);

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            for (int f = 0; f < request.Params.Count; f++)
            {
                string key = request.Params.Keys[f];
                if (key.ToLower() == "sign") continue;
                parameters.Add(key, request.Params[key]);
            }
            return CreateSign(parameters, request.AppSecret) == request.Sign;//将服务端sign和客户端传过来的sign进行比较
        }

        /*
                 * 上面签名验签使用的是MD5方式，如果要使用RSA的方式用下面这个
                 * 上面使用的是MD5加密，如果使用RSA的方式如下
                */
        //SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
        //for (int f = 0; f < form.Count; f++)
        //{
        //    string key = form.Keys[f];
        //    if (key.ToLower() == "sign") continue;
        //    sParaTemp.Add(key, form[key]);
        //}
        //return RSAFromPkcs8.verify(sParaTemp.ToString(),sign,"存在服务端公钥","utf-8");

        /// <summary>
        /// 创建签名
        /// </summary>
        /// <param name="parameters">请求参数</param>
        /// <param name="secret">key 对应的 secret 密匙</param>
        /// <param name="isAppendSecretAtLast">是否在尾部追加密匙</param>
        /// <returns></returns>
        private string CreateSign(IDictionary<string, string> parameters, string secret, bool isAppendSecretAtLast = false)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder(secret);
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key))// && !string.IsNullOrEmpty(value) 空值也加入计算！！
                {
                    query.Append(key).Append(value);
                }
            }
            if (isAppendSecretAtLast)
            {
                query.Append(secret);
            }

            // 第三步：使用MD5加密
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));

            // 第四步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }

            return result.ToString();
        }
               
        /// <summary>
        ///  timespan 格式：yyyyMMddHHmmss
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private bool ValidateTimestamp(string timestamp)
        {
            //一：检测时间戳
            if (string.IsNullOrEmpty(timestamp) || timestamp.Length != 14)
            {
                throw new FriendlyException("SIGN_ERROR:时间戳格式不正确");
            }
            DateTime dtimestamp;
            try
            {
                dtimestamp = DateTime.ParseExact(timestamp, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                throw new FriendlyException("SIGN_ERROR:时间戳格式不正确", ex);
            }
            //判断签名是否已过期
            if (dtimestamp < DateTime.Now.AddSeconds(-this._timeout))//请求有效时间20秒
            {
                throw new FriendlyException("SIGN_ERROR:当前请求已过期");
            }

            return true;
        }
    }
}
