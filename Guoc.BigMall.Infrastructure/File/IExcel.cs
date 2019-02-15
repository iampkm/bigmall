using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.File
{
   public interface IExcel
    {
       MemoryStream WriteToExcelStream<T>(List<T> list, ExcelVersion version, bool isMerge = false, bool onlyDescription = false);
    }
}
