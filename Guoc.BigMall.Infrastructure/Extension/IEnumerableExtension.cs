using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.Extension
{
    public static class IEnumerableExtension
    {
        public static List<TResult> ToList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, bool skipNull = false, bool removeNullResult = false)
        {
            if (source == null) return null;

            List<TResult> container = new List<TResult>();
            foreach (var item in source)
            {
                if (item == null) continue;
                var result = selector.Invoke(item);
                if (result == null && removeNullResult) continue;
                container.Add(result);
            }

            return container;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || source.Count() == 0;
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> source)
        {
            return !source.IsEmpty();
        }

        /// <summary>
        /// 根据索引片段分组。
        /// </summary>
        /// <param name="subsectionSize">每个片段的索引个数</param>
        public static IEnumerable<IGrouping<int, T>> GroupByIndexSubsection<T>(this IEnumerable<T> source, int subsectionSize)
        {
            if (subsectionSize <= 0) throw new ArgumentException("指定片段的索引个数必须大于0", "subsectionSize");
            var groupCount = (source.Count() + subsectionSize - 1) / subsectionSize;
            var grouped = new List<IGrouping<int, T>>();
            for (int i = 0; i < groupCount; i++)
            {
                var groupCategoryIds = source.Skip(subsectionSize * i).Take(subsectionSize);
                grouped.AddRange(groupCategoryIds.GroupBy(k => i));
            }
            return grouped;
        }
    }
}
