using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
using NPOI.SS.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Reflection;
using System.IO;
namespace Guoc.BigMall.Infrastructure.File
{
    public class ExcelService : IExcel
    {

        public MemoryStream WriteToExcelStream<T>(List<T> list, ExcelVersion version, bool isMerge = false, bool onlyDescription = false)
        {
            try
            {
                IWorkbook workbook = null;
                if (version == ExcelVersion.Above2007)
                    workbook = new XSSFWorkbook();
                else
                    workbook = new HSSFWorkbook();

                List<MergedRegion> regions = new List<MergedRegion>();
                MemoryStream ms = new MemoryStream();
                ISheet sheet = workbook.CreateSheet();

                #region 写入标题行
                T _t = (T)Activator.CreateInstance(typeof(T));
                Type type = typeof(T);
                PropertyInfo[] piList = type.GetProperties();
                for (int i = 0; i < piList.Length - 1; i++)
                {
                    //属性重新排序，父类属性靠前显示
                    for (int j = 0; j < piList.Length - 1 - i; j++)
                    {
                        if (piList[j].DeclaringType.Name == piList[j].ReflectedType.Name && piList[j + 1].DeclaringType.Name != piList[j + 1].ReflectedType.Name)
                        {
                            var temp = piList[j + 1];
                            piList[j + 1] = piList[j];
                            piList[j] = temp;
                        }
                    }
                }
                //建立T的Property与sheet的Columns的对应关系
                Dictionary<int, int> columnIndex = new Dictionary<int, int>();
                IRow headerRow = sheet.CreateRow(0);
                for (int i = 0; i < piList.Length; i++)
                {
                    string headName = piList[i].Name;

                    #region 只展示有Description的属性
                    if (onlyDescription)
                    {
                        //将Description值作为标题名
                        //headName = ((DescriptionAttribute)Attribute.GetCustomAttribute(piList[i], typeof(DescriptionAttribute))).Description;
                        headName = _t.GetDescription(piList[i].Name);
                    }
                    #endregion

                    if (!string.IsNullOrEmpty(headName))
                    {
                        int key = regions.Count;
                        columnIndex.Add(key, i);
                        headerRow.CreateCell(key).SetCellValue(headName);
                        regions.Add(new MergedRegion());
                    }
                }
                #endregion

                #region 写入内容行
                for (int i = 0; i < list.Count; i++)
                {
                    int rowIndex = i + 1;
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    foreach (int j in columnIndex.Keys)
                    {
                        int val = columnIndex[j];
                        string content = string.Empty;
                        try
                        {
                            content = piList[val].GetValue(list[i], null).ToString();
                        }
                        catch { }
                        dataRow.CreateCell(j).SetCellValue(content);

                        #region 合并单元格
                        if (isMerge)
                        {
                            //如果region为空，则初始化
                            if (regions[j].IsInit)
                            {
                                regions[j] = new MergedRegion(content, rowIndex);
                            }
                            //如果region值等于上一行，则region.Length++
                            else if (regions[j].Content.Equals(content))
                            {
                                regions[j].Length++;
                            }
                            else
                            {
                                for (int k = j; k < regions.Count; k++)
                                {
                                    //合并单元格
                                    if (regions[k].Length > 0)
                                    {
                                        sheet.AddMergedRegion(new CellRangeAddress(regions[k].StartIndex, regions[k].StartIndex + regions[k].Length, k, k));
                                    }
                                    //为region重新赋值
                                    regions[k] = k == j ? new MergedRegion(content, rowIndex) : new MergedRegion();
                                }
                            }
                        }
                        #endregion
                    }

                    #region 合并单元格
                    //如果是最后一行，则合并单元格
                    if (isMerge && i == list.Count - 1)
                    {
                        for (int k = 0; k < regions.Count; k++)
                        {
                            if (regions[k].Length > 0)
                            {
                                sheet.AddMergedRegion(new CellRangeAddress(regions[k].StartIndex, regions[k].StartIndex + regions[k].Length, k, k));
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                workbook.Write(ms);
                return ms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    internal class MergedRegion
    {
        #region 属性
        public MergedRegion()
        {
            this.Content = string.Empty;
            this.StartIndex = -1;
            this.Length = -1;
        }
        public MergedRegion(string content, int startIndex)
        {
            this.Content = content;
            this.StartIndex = startIndex;
            this.Length = 0;
        }
        /// <summary>
        /// 数据内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 合并起始位置
        /// </summary>
        public int StartIndex { get; set; }
        /// <summary>
        /// 合并长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 当前是否为初始
        /// </summary>
        public bool IsInit
        {
            get
            {
                return string.IsNullOrEmpty(this.Content) && this.StartIndex == -1 && this.Length == -1;
            }
        }
        #endregion
    }

    /// <summary>
    /// Excel版本
    /// </summary>
    public enum ExcelVersion
    {
        #region 枚举
        /// <summary>
        /// 低于2007版本
        /// </summary>
        Below2007,
        /// <summary>
        /// 高于（包含）2007版本
        /// </summary>
        Above2007,
        #endregion
    }
}
