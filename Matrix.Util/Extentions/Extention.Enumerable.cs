using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Matrix.Util.Extentions
{
    /// <summary>
    /// 扩展集合功能
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// 将字符串加在一起
        /// </summary>
        /// <param name="source"></param>
        /// <param name="splitor">分隔符</param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> source, string splitor)
        {
            if (source == null) return string.Empty;

            return string.Join(splitor, source);
        }
        /// <summary>
        /// 将数据源映射成字符串数据源后加在一起
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="splitor">分隔符</param>
        /// <param name="selector">映射器</param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> source, string splitor, Func<T, string> selector)
        {
            if (source == null) return string.Empty;

            return source.Select(selector).Join(splitor);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="splitor"></param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> source, string splitor)
        {
            if (source == null) return string.Empty;

            return source.Where(x => x != null).Select(x => x?.ToString() ?? string.Empty).Join(splitor);
        }
        /// <summary>
        /// 为集合添加元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static IEnumerable<T> Plus<T>(this IEnumerable<T> source, T add)
        {
            foreach (var element in source)
            {
                yield return element;
            }
            yield return add;
        }
        /// <summary>
        /// 将元素添加到集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static IEnumerable<T> Plus<T>(this T source, IEnumerable<T> add)
        {
            yield return source;
            foreach (var element in add)
            {
                yield return element;
            }
        }

        /// <summary>
        /// 对source的每一个元素执行指定的动作action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null || action == null)
                return;

            foreach (var element in source)
            {
                action(element);
            }
        }

        /// <summary>
        /// 给IEnumerable拓展ForEach方法
        /// </summary>
        /// <typeparam name="T">模型类</typeparam>
        /// <param name="iEnumberable">数据源</param>
        /// <param name="func">方法</param>
        public static void ForEach<T>(this IEnumerable<T> iEnumberable, Action<T, int> func)
        {
            var array = iEnumberable.ToArray();
            for (int i = 0; i < array.Count(); i++)
            {
                func(array[i], i);
            }
        }

        /// <summary>
        /// 判断序列是否包含任何元素，如果序列为空，则返回False
        /// </summary>
        /// <typeparam name="T">序列类型</typeparam>
        /// <param name="source">序列</param>
        /// <returns></returns>
        public static bool AnyOne<T>(this IEnumerable<T> source)
        {
            return source != null ? source.Any() : false;
        }

        /// <summary>
        /// WhereIf[在condition为true的情况下应用Where表达式]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> expression)
        {
            return condition ? source.Where(expression) : source;
        }

        public static IEnumerable<TFirst> Except<TFirst, TSecond, TCompared>(
       this IEnumerable<TFirst> first,
       IEnumerable<TSecond> second,
       Func<TFirst, TCompared> firstSelect,
       Func<TSecond, TCompared> secondSelect)
        {
            return Except(first, second, firstSelect, secondSelect, EqualityComparer<TCompared>.Default);
        }

        public static IEnumerable<TFirst> Except<TFirst, TSecond, TCompared>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TCompared> firstSelect,
            Func<TSecond, TCompared> secondSelect,
            IEqualityComparer<TCompared> comparer)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");
            return ExceptIterator<TFirst, TSecond, TCompared>(first, second, firstSelect, secondSelect, comparer);
        }

        private static IEnumerable<TFirst> ExceptIterator<TFirst, TSecond, TCompared>(
            IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TCompared> firstSelect,
            Func<TSecond, TCompared> secondSelect,
            IEqualityComparer<TCompared> comparer)
        {
            HashSet<TCompared> set = new HashSet<TCompared>(second.Select(secondSelect), comparer);
            foreach (TFirst tSource1 in first)
                if (set.Add(firstSelect(tSource1)))
                    yield return tSource1;
        }
    }
}
