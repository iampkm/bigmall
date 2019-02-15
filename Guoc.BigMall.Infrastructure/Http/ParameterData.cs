using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Guoc.BigMall.Infrastructure.Http
{
    public class ParameterData
    {
        private SortedDictionary<string, string> _data = new SortedDictionary<string, string>();

        /// <summary>
        /// 由url参数格式转化
        /// </summary>
        /// <param name="url"></param>
        /// <param name="urlDecode">是否将参数url解码</param>
        public void FromUrl(string url, bool urlDecode = true)
        {
            string[] param = url.Trim('&').Split('&');
            foreach (var item in param)
            {
                string[] data = item.Split('=');
                this._data[data[0]] = urlDecode ? System.Web.HttpUtility.UrlDecode(data[1]) : data[1];
            }
        }

        /// <summary>
        /// 转化成url参数格式
        /// </summary>
        /// <param name="urlEncode">是否将参数url编码</param>
        /// <returns></returns>
        public string ToUrl(bool urlEncode = true)
        {
            string buff = "";
            foreach (var item in this._data)
            {
                var value = urlEncode ? System.Web.HttpUtility.UrlEncode(item.Value) : item.Value;
                buff += item.Key + "=" + value + "&";
            }
            buff = buff.Trim('&');
            return buff;
        }

        /// <summary>
        /// 由xml格式转化
        /// </summary>
        /// <param name="xml"></param>
        public void FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return;

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            foreach (XmlNode child in doc.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.XmlDeclaration)
                    continue;

                foreach (XmlNode node in child.ChildNodes)
                {
                    if (node.NodeType != XmlNodeType.Element)
                        continue;

                    var element = (XmlElement)node;
                    this._data[element.Name] = element.InnerText;
                }
            }
        }

        /// <summary>
        /// 转化成xml格式
        /// </summary>
        /// <param name="character">是否是字符类型</param>
        /// <returns></returns>
        public string ToXml(bool character = true)
        {
            string xml = "<xml>";
            foreach (var pair in this._data)
            {
                xml += "<" + pair.Key + ">";
                xml += character ? ("<![CDATA[" + pair.Value + "]]>") : pair.Value;
                xml += "</" + pair.Key + ">";
            }
            xml += "</xml>";
            return xml;
        }

        /// <summary>
        /// 是否有值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasValue(string key)
        {
            string value = null;
            this._data.TryGetValue(key, out value);
            if (null != value)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 设置键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            this._data[key] = value;
        }

        /// <summary>
        /// 获取键的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            string value = null;
            this._data.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// 获取所有键值对
        /// </summary>
        /// <returns></returns>
        public SortedDictionary<string, string> GetValues()
        {
            return this._data;
        }

        /// <summary>
        /// 获取所有键列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetKeyList()
        {
            return this._data.Keys.ToList();
        }

        /// <summary>
        /// 获取所有值列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetValueList()
        {
            return this._data.Values.ToList();
        }
    }
}
