using System;

namespace Matrix.Util.Extentions
{
    /// <summary>
    /// 时间扩展
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// 时间转换成友好显示格式的字符串
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns></returns>
        public static string ToFriendString(this DateTime date)
        {
            var now = DateTime.Now;
            var span = now.Subtract(date);
            if (span.TotalSeconds <= 60)
            {
                return "刚刚";
            }
            else if (span.TotalMinutes <= 30)
            {
                return $"{(int)span.TotalMinutes}分钟前";
            }
            else if (span.TotalMinutes <= 60)
            {
                return $"1小时前";
            }
            else if (span.TotalHours <= 24)
            {
                return $"{(int)span.TotalHours}小时前";
            }
            else if (span.TotalDays <= 30)
            {
                return $"{(int)span.TotalDays}天前";
            }
            else if (span.TotalDays <= 365)
            {
                return $"{((int)span.TotalDays) / 30}个月前";
            }
            return $"{((int)span.TotalDays) / 365}年前";
        }

        /// <summary>
        /// 不同时区的时间转换
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <param name="defaultFromTimeZoneId">当前时间的区域Id,<seealso cref="To(DateTime?, TimeZoneInfo, TimeZoneInfo)"/></param>
        /// <param name="toTimeZoneId">目标时间区域Id</param>
        /// <returns></returns>
        public static DateTime? To(this Nullable<DateTime> dateTime, string defaultFromTimeZoneId, string toTimeZoneId)
        {
            return dateTime.To(TimeZoneInfo.FindSystemTimeZoneById(defaultFromTimeZoneId), TimeZoneInfo.FindSystemTimeZoneById(toTimeZoneId));
        }
        /// <summary>
        /// 不同时区的时间转换
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <param name="defaultFromTimeZoneId">当前时间的区域Id,<seealso cref="To(DateTime?, TimeZoneInfo, TimeZoneInfo)"/></param>
        /// <param name="toTimeZoneId">目标时间区域Id</param>
        /// <returns></returns>
        public static DateTime To(this DateTime dateTime, string defaultFromTimeZoneId, string toTimeZoneId)
        {
            return dateTime.To(TimeZoneInfo.FindSystemTimeZoneById(defaultFromTimeZoneId), TimeZoneInfo.FindSystemTimeZoneById(toTimeZoneId));
        }

        /// <summary>
        /// 将当前时间所处的区域转换称目标时间区域的时间
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <param name="toTimeZone">目标时间区域</param>
        /// <returns></returns>
        public static DateTime? To(this Nullable<DateTime> dateTime, TimeZoneInfo toTimeZone)
        {
            return dateTime.To(TimeZoneInfo.Local, toTimeZone);
        }
        /// <summary>
        /// 将当前时间所处的区域转换称目标时间区域的时间
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <param name="toTimeZone">目标时间区域</param>
        /// <returns></returns>
        public static DateTime To(this DateTime dateTime, TimeZoneInfo toTimeZone)
        {
            return dateTime.To(TimeZoneInfo.Local, toTimeZone);
        }

        /// <summary>
        /// 区域时间的转换
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <param name="fromDefaultTimeZone">默认源时间区域。如果时间有指定区域，则<paramref name="fromDefaultTimeZone"/>无效</param>
        /// <param name="toTimeZone">目标时间区域</param>
        /// <returns></returns>
        public static DateTime? To(this Nullable<DateTime> dateTime, TimeZoneInfo fromDefaultTimeZone, TimeZoneInfo toTimeZone)
        {
            if (!dateTime.HasValue)
                return dateTime;

            return dateTime.Value.To(fromDefaultTimeZone, toTimeZone);
        }

        /// <summary>
        /// 区域时间的转换
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <param name="fromDefaultTimeZone">默认源时间区域。如果时间有指定区域，则<paramref name="fromDefaultTimeZone"/>无效</param>
        /// <param name="toTimeZone">目标时间区域</param>
        /// <returns></returns>
        public static DateTime To(this DateTime dateTime, TimeZoneInfo fromDefaultTimeZone, TimeZoneInfo toTimeZone)
        {
            switch (dateTime.Kind)
            {
                case DateTimeKind.Unspecified:
                    return TimeZoneInfo.ConvertTime(dateTime, fromDefaultTimeZone, toTimeZone);
                case DateTimeKind.Utc:
                    return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc, toTimeZone);
                case DateTimeKind.Local:
                    return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, toTimeZone);
                default:
                    return TimeZoneInfo.ConvertTime(dateTime, fromDefaultTimeZone, toTimeZone);
            }
        }

        /// <summary>
        /// convert a Unix timestamp to DateTime
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if (Math.Floor(Math.Log10(unixTimeStamp) + 1) == 13)
                dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            else
                dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        /// <summary>
        /// 转换成13位（毫秒）时间戳
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static long ToMillisecondsTimestamp(this DateTime date)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, date.Kind);
            return (long)(date.ToUniversalTime() - baseDate).TotalMilliseconds;
        }
        /// <summary>
        /// 转换成10位（秒）时间戳
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns></returns>
        public static long ToSecondsTimestamp(this DateTime date)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, date.Kind);
            return (long)(date.ToUniversalTime() - baseDate).TotalSeconds;
        }
    }

    /// <summary>
    /// 常用的时间区域
    /// </summary>
    public class TimeZones
    {
        /// <summary>
        /// 当前机器时间的区域
        /// </summary>
        public static readonly TimeZoneInfo Local = TimeZoneInfo.Local;
        /// <summary>
        /// UTC 时间区域
        /// </summary>
        public static readonly TimeZoneInfo UTC = TimeZoneInfo.Utc;
        /// <summary>
        /// (UTC+08:00) 中国 北京
        /// </summary>
        public static readonly TimeZoneInfo ChinaStandardTime = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
        /// <summary>
        /// (UTC+03:00) 俄罗斯 莫斯科
        /// </summary>
        public static readonly TimeZoneInfo RussianStandardTime = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        /// <summary>
        /// (UTC-08:00) 美国 太平洋时间
        /// </summary>
        public static readonly TimeZoneInfo PacificStandardTime = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        /// <summary>
        /// (UTC+01:00) 西班牙 布鲁塞尔，哥本哈根，马德里，巴黎
        /// </summary>
        public static readonly TimeZoneInfo RomanceStandardTime = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
        /// <summary>
        /// (UTC-03:00) 巴西 巴西利亚
        /// </summary>
        public static readonly TimeZoneInfo ESouthAmericaStandardTime = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        /// <summary>
        /// (UTC+09:30) 澳大利亚 阿德莱德
        /// </summary>
        public static readonly TimeZoneInfo CenAustraliaStandardTime = TimeZoneInfo.FindSystemTimeZoneById("Cen. Australia Standard Time");
        /// <summary>
        /// (UTC+01:00) 德国 阿姆斯特丹，柏林，伯尔尼，罗马，斯德哥尔摩，维也纳
        /// </summary>
        public static readonly TimeZoneInfo WEuropeStandardTime = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
        /// <summary>
        /// (UTC+05:30) 印度 钦奈，加尔各答，孟买，新德里
        /// </summary>
        public static readonly TimeZoneInfo IndiaStandardTime = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
    }
}
