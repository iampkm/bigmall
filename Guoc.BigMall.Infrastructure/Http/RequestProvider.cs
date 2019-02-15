using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Guoc.BigMall.Infrastructure.Http
{
    public class RequestProvider
    {
        public string Url { get; set; }

        public string UrlPreview
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Url)) return null;
                string urlParams = GetUrlParamsString();
                if (!string.IsNullOrEmpty(urlParams)) return string.Format("{0}{1}{2}", Url.Trim(), (Url.Contains("?") ? (Url.TrimEnd().EndsWith("?") ? "" : "&") : "?"), urlParams);
                return Url.Trim();
            }
        }

        public int Timeout { get; set; }

        public RequestMethod Method { get; set; }

        private string _ContentType;
        public string ContentType { get { return _ContentType ?? "text/xml"; } set { _ContentType = value; } }

        private Encoding _DataEncoding;
        public Encoding DataEncoding { get { return _DataEncoding ?? Encoding.UTF8; } set { _DataEncoding = value; } }

        public NameValueCollection HeadersParams { get; set; }
        public IDictionary<string, string> UrlParams { get; set; }

        private RequestProvider() { UrlParams = new Dictionary<string, string>(); HeadersParams = new NameValueCollection(); }
        public static RequestProvider Provide(string url, RequestMethod method = RequestMethod.Get, string contentType = "text/xml", Encoding dataEncoding = null, int timeout = 100000, IDictionary<string, string> urlParams = null, NameValueCollection headersParams = null)
        {
            RequestProvider request = new RequestProvider()
            {
                Url = url,
                Method = method,
                Timeout = timeout,
                ContentType = contentType,
                DataEncoding = dataEncoding ?? Encoding.UTF8,
            };
            if (urlParams != null) request.UrlParams = urlParams;
            if (headersParams != null) request.HeadersParams = headersParams;
            return request;
        }

        public void ClearParameters()
        {
            if (HeadersParams != null) HeadersParams.Clear();
            if (UrlParams != null) UrlParams.Clear();
        }

        public string GetUrlParamsString()
        {
            if (UrlParams == null || UrlParams.Count == 0) return string.Empty;
            StringBuilder parametersStr = new StringBuilder();
            foreach (var parameter in UrlParams) if (!string.IsNullOrEmpty(parameter.Key)) parametersStr.AppendFormat("{0}={1}&", parameter.Key, parameter.Value);
            if (parametersStr.Length > 0) parametersStr.Length -= 1;
            return parametersStr.ToString();
        }

        public bool SendRequest(byte[] postData, out string responseString)
        {
            string url = UrlPreview;
            responseString = string.Empty;
            if (string.IsNullOrEmpty(url))
            {
                responseString = "请求地址无效。";
                return false;
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = Enum.GetName(typeof(RequestMethod), Method).ToUpper();
            //request.ServicePoint.Expect100Continue = false;
            //request.KeepAlive = true;
            request.Timeout = Timeout;

            //request.ContentLength = 0;
            if (Method == RequestMethod.Post)
            {
                if (postData != null && postData.Length > 0)
                {
                    request.ContentType = ContentType;
                    //request.ContentLength = postData.Length;
                    Stream requestStream;
                    try
                    {
                        requestStream = request.GetRequestStream();
                    }
                    catch (WebException ex)
                    {
                        responseString = string.Format("无法连接服务器。{0}", ex.Message);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        responseString = string.Format("无法连接服务器。{0}", ex.Message);
                        return false;
                    }
                    requestStream.Write(postData, 0, postData.Length);
                    requestStream.Close();
                }
                else request.ContentLength = 0;
            }

            if (HeadersParams != null && HeadersParams.Count > 0) request.Headers.Add(HeadersParams);

            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, (string.IsNullOrEmpty(response.CharacterSet) ? DataEncoding : Encoding.GetEncoding(response.CharacterSet)));
                responseString = reader.ReadToEnd();

                reader.Close();
                responseStream.Close();
                response.Close();
                return true;
            }
            catch (WebException ex)
            {
                try { using (var reader = new StreamReader(ex.Response.GetResponseStream()))responseString = reader.ReadToEnd(); }
                catch { }
                responseString = string.Format("请求异常：{0}\r\n{1}", ex.Message, responseString);
                return false;
            }
            catch (Exception ex)
            {
                responseString = string.Format("请求异常：{0}", ex.Message);
                return false;
            }
        }

        public bool SendRequest(out string responseString)
        {
            return SendRequest((byte[])null, out  responseString);
        }

        public bool SendRequest(string data, out string responseString)
        {
            return SendRequest(data == null ? null : DataEncoding.GetBytes(data), out  responseString);
        }
        public static ParameterData GetRequestParameter()
        {
            NameValueCollection collection;
            var requestType = HttpContext.Current.Request.RequestType;
            var isHttpGet = string.Equals(requestType, "GET", StringComparison.InvariantCultureIgnoreCase);
            if (isHttpGet)
                collection = HttpContext.Current.Request.QueryString;
            else
                collection = HttpContext.Current.Request.Form;

            return GetRequestParameter(collection, isHttpGet);
        }

        /// <summary>
        /// 获取当前Http请求参数
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="isHttpGet"></param>
        /// <returns></returns>
        public static ParameterData GetRequestParameter(NameValueCollection collection, bool isHttpGet)
        {
            var data = new ParameterData();
            foreach (var key in collection.AllKeys)
            {
                var value = collection[key];
                data.SetValue(key, isHttpGet ? value : HttpUtility.UrlDecode(value));
            }
            return data;
        }

    }
}
