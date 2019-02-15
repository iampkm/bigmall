using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.ViewObject
{
   public class Pager
    {
       public Pager()
       {
           this.SumColumns = new List<SumColumn>();
       }
       /// <summary>
       /// 当前页索引
       /// </summary>
       public int PageIndex { get; set; }
       /// <summary>
       /// 每页显示行数
       /// </summary>
       public int PageSize { get; set; }
       /// <summary>
       /// 总记录数
       /// </summary>
       public int Total { get; set; }
       /// <summary>
       /// 是否分页
       /// </summary>
       public bool IsPaging { get; set; }

       public List<SumColumn> SumColumns { get; set; }
       /// <summary>
       /// 是否导出excel
       /// </summary>
       public bool toExcel { get; set; }
    }

    /// <summary>
    /// 求和列
    /// </summary>
   public class SumColumn
   {
       public SumColumn(string column, string value)
       {
           this.Column = column;
           this.Value = value;
       }
       /// <summary>
       /// 求和列名: 列名需要与Vue.js表格　中　的显示列名一致　
       /// </summary>
       public string Column { get; set; }

       /// <summary>
       /// 求和值
       /// </summary>
       public string Value { get; set; }
   }
}
