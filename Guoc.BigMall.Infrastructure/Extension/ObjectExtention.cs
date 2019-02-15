using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.ComponentModel;

namespace Guoc.BigMall.Infrastructure.Extension
{
    public static class ObjectExtention
    {
        /// <summary>
        /// 获取对象中指定属性名的Description
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static string GetDescription(this object obj, string propertyName)
        {
            Type type = obj.GetType();
            //获取被引用的属性类型
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            if (properties != null)
            {
                //获取被引用类型的特定属性
                PropertyDescriptor property = properties[propertyName];
                if (property != null)
                {
                    //得到所需的属性的属性
                    AttributeCollection attributes = property.Attributes;
                    //得到描述属性的集合
                    DescriptionAttribute descript = (DescriptionAttribute)attributes[typeof(DescriptionAttribute)];
                    //得到引用描述
                    if (!String.IsNullOrEmpty(descript.Description))
                    {
                        return descript.Description;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取枚举值的Description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            Type enumType = value.GetType();
            // 获取枚举常数名称。
            string name = Enum.GetName(enumType, value);
            if (name != null)
            {
                // 获取枚举字段。
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    // 获取描述的属性。
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 泛型对象转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this T t) where T : class
        {
            var list = new List<T>() { t };
            return list.ToDataTable();
        }

        /// <summary>
        /// 泛型对象列表转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list) where T : class
        {
            var type = typeof(T);
            var dt = new DataTable(type.Name);
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (var item in propertyInfos)
            {
                dt.Columns.Add(item.Name);
            }
            //DataTable的Column与Class的Properties对应位置键值对
            //key：泛型类的Properties下标 value：DataTable的Column下标
            Dictionary<int, int> indexMapping = new Dictionary<int, int>();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns[j].ColumnName.Equals(propertyInfos[i].Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        indexMapping.Add(i, j);
                    }
                }
            }
            foreach (var item in list)
            {
                var row = dt.NewRow();
                foreach (var map in indexMapping)
                {
                    row[map.Value] = propertyInfos[map.Key].GetValue(item, null);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static bool IsNull<T>(this T obj)
        {
            return obj == null;
        }

        public static bool NotNull<T>(this T obj)
        {
            return obj != null;
        }

        public static bool In<T>(this T item, params T[] range)
        {
            return range.Contains(item);
        }

        public static bool In<T>(this T item, IEnumerable<T> range)
        {
            return range.Contains(item);
        }

        public static bool NotIn<T>(this T item, params T[] range)
        {
            return !In(item, range);
        }

        public static bool NotIn<T>(this T item, IEnumerable<T> range)
        {
            return !In(item, range);
        }
    }
}
