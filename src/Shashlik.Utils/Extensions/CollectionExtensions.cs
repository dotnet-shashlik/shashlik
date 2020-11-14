using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;


namespace Shashlik.Utils.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// 计算索引号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetIndex<T>(this IEnumerable<T> source, T value)
        {
            var i = 0;
            foreach (var item in source)
            {
                if (item.Equals(value))
                    return i;
                i++;
            }

            return -1;
        }

        /// <summary>
        /// 集合拼接为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> source, string separator)
        {
            if (separator is null)
            {
                throw new ArgumentNullException(nameof(separator));
            }

            return string.Join(separator, source);
        }

        /// <summary>
        /// 循环集合元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static void ForEachItem<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// 转换只读集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IReadOnlyList<T> ToReadOnly<T>(this IEnumerable<T> list)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return list.ToImmutableList();
        }

        /// <summary>
        /// 判断集合是否为null或者空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list is null || !list.Any();
        }

        /// <summary>
        /// 判断集合是否存在重复元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="keyComparer"></param>
        /// <returns></returns>
        public static bool HasRepeat<T>(this IEnumerable<T> list, IEqualityComparer<T> keyComparer = null)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (keyComparer is null)
                return list.GroupBy(r => r).Any(g => g.Count() > 1);
            else
                return list.GroupBy(r => r, keyComparer).Any(g => g.Count() > 1);
        }

        /// <summary>
        /// 判断集合是否存在重复的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TP"></typeparam>
        /// <param name="list"></param>
        /// <param name="selectProperty"></param>
        /// <param name="keyComparer"></param>
        /// <returns></returns>
        public static bool HasRepeat<T, TP>(this IEnumerable<T> list, Func<T, TP> selectProperty,
            IEqualityComparer<TP> keyComparer = null)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (selectProperty is null)
            {
                throw new ArgumentNullException(nameof(selectProperty));
            }

            if (keyComparer is null)
                return list.GroupBy(selectProperty).Any(g => g.Count() > 1);
            else
                return list.GroupBy(selectProperty, keyComparer).Any(g => g.Count() > 1);
        }

        /// <summary>
        /// where id
        /// </summary>
        /// <param name="list"></param>
        /// <param name="condition">条件值</param>
        /// <param name="where">where</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> list, bool condition, Func<T, bool> where)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (where is null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            if (condition)
                return list.Where(where);
            return list;
        }

        /// <summary>
        /// where if
        /// </summary>
        /// <param name="list"></param>
        /// <param name="condition">条件之</param>
        /// <param name="where">where</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> list, bool condition, Expression<Func<T, bool>> where)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (where is null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            if (condition)
                return list.Where(where);
            return list;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="query">查询源</param>
        /// <param name="pageIndex">当前页,索引从1开始</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        public static IQueryable<T> DoPage<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return
                query
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="query">查询源</param>
        /// <param name="pageIndex">当前页,索引从1开始</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        public static IEnumerable<T> DoPage<T>(this IEnumerable<T> query, int pageIndex, int pageSize)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return
                query
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize);
        }

        /// <summary>
        /// 获取字典值,没有则返回默认值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        {
            if (key is null)
                return default;
            if (dic.TryGetValue(key, out TValue value))
                return value;
            return default;
        }

        /// <summary>
        /// 是否为空行
        /// </summary>
        /// <param name="row"></param>
        /// <param name="whiteSpaceIs">空白字符串算不算空</param>
        /// <returns></returns>
        public static bool IsEmptyRow(this DataRow row, bool whiteSpaceIs = true)
        {
            for (int j = 0; j < row.Table.Columns.Count; j++)
            {
                var value = row[j]?.ToString();
                if (whiteSpaceIs)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                        return false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 合并两个字典，并覆盖相同键名的值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> to, IDictionary<TKey, TValue> from)
        {
            if (from is null) return;
            foreach (var kv in from)
            {
                if (to.ContainsKey(kv.Key))
                {
                    to[kv.Key] = kv.Value;
                }
                else
                {
                    to.Add(kv.Key, kv.Value);
                }
            }
        }

        /// <summary>
        /// 列表转DataTable,将类型的<typeparamref name="T"/>属性作为列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T))
                ;

            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }

                table.Rows.Add(values);
            }

            return table;
        }
    }
}