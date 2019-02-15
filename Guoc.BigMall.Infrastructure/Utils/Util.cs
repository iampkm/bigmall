using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.Utils
{
    public class Util
    {
        public static int TrimIntNull(Object obj)
        {
            if (obj is System.DBNull || obj == null || string.IsNullOrEmpty(obj.ToString()))//启动报错,所以...(by Yung)
            {
                return 0;
            }
            else
            {
                return Int32.Parse(obj.ToString());
            }
        }
        public static decimal TrimDecimalNull(Object obj)
        {
            if (obj is System.DBNull || obj == null || string.IsNullOrEmpty(obj.ToString())|| !IsPositiveNumber(obj.ToString()))
            {
                return 0M;
            }
            else
            {
                return decimal.Parse(obj.ToString());
            }
        }
        public static bool IsPositiveNumber(String strNumber)
        {
            Regex objNotPositivePattern = new Regex("[^0-9.]");
            Regex objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            return !objNotPositivePattern.IsMatch(strNumber) &&
                   objPositivePattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber);
        }

      
    }
}
