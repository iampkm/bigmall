using System.Configuration;

namespace Guoc.BigMall.Application.Configuration
{
    public class SystemConfig
    {
        /// <summary>
        /// 单据单明细项串码个数上限。
        /// </summary>
        public static int ItemMaxSNCodeQuantity
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["ItemMaxSNCodeQuantity"]);
            }
        }
    }
}
