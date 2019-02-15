using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace Guoc.BigMall.Infrastructure
{
   public class Configer
    {
        /// <summary>
        /// 主数据库连接串
        /// </summary>
        public static string DBName
        {
            get
            {
                return ConfigurationManager.AppSettings["dbName"];              
            }
        }

        /// <summary>
        /// SAP接口访问开关，关闭时，使用模拟接口，测试专用
        /// </summary>
        public static bool Sap_Mock
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["Sap_Mock"]);
            }
        }
    }
}
