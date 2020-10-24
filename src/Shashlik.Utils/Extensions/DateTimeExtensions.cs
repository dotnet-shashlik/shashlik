using System;

// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable RedundantIfElseBlock

namespace Shashlik.Utils.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 获取本周第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="whichFirst">哪一天是一周的开始</param>
        /// <returns></returns>
        public static DateTime GetWeekFirstDay(this DateTime dt, DayOfWeek whichFirst = DayOfWeek.Monday)
        {
            return dt.AddDays((int) whichFirst - (int) dt.DayOfWeek);
        }

        /// <summary>
        /// 获取本周的最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="whichFirst">哪一天是一周的开始</param>
        /// <returns></returns>
        public static DateTime GetWeekLastDay(this DateTime dt, DayOfWeek whichFirst = DayOfWeek.Monday)
        {
            return dt.GetWeekFirstDay(whichFirst).AddDays(6);
        }

        /// <summary>
        /// 获取下周的最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="whichFirst">哪一天是一周的开始</param>
        /// <returns></returns>
        public static DateTime GetNextWeekLastDay(this DateTime dt, DayOfWeek whichFirst = DayOfWeek.Monday)
        {
            return dt.GetWeekFirstDay(whichFirst).AddDays(13);
        }

        /// <summary>
        /// 获取下周的第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="whichFirst">哪一天是一周的开始</param>
        /// <returns></returns>
        public static DateTime GetNextWeekFirstDay(this DateTime dt, DayOfWeek whichFirst = DayOfWeek.Monday)
        {
            return dt.GetWeekFirstDay(whichFirst).AddDays(7);
        }

        /// <summary>
        /// 获取本月的第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetMonthFirstDay(this DateTime dt)
        {
            return dt.AddDays(1 - dt.Day).Date;
        }

        /// <summary>
        /// 获取本月的最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetMonthLastDay(this DateTime dt)
        {
            return dt.GetMonthFirstDay().AddMonths(1).AddDays(-1).Date;
        }

        /// <summary>
        /// 获取下一个月的第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetNextMonthFirstDay(this DateTime dt)
        {
            return dt.GetMonthFirstDay().AddMonths(1);
        }

        /// <summary>
        /// 获取下一个月的最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetNextMonthLastDay(this DateTime dt)
        {
            return dt.GetMonthFirstDay().AddMonths(2).AddDays(-1);
        }

        /// <summary>
        /// 获取下一个指定的周几的日期    
        /// 如2016/12/27为周2, 获取下一个周2,2017/1/3,下一个周3就是2016/12/28
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetNextSpecificDayOfWork(this DateTime dt, DayOfWeek dayOfWeek)
        {
            var offset = dayOfWeek - dt.DayOfWeek;
            if (offset > 0)
                return dt.AddDays(offset);
            else
                return dt.AddDays(7 + offset);
        }

        /// <summary>
        /// 获取下一个指定的周几的日期,包含输入的日期
        /// 如2016/12/27为周2, 获取下一个周2,2016/12/27,下一个周3就是2016/12/28
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetCurrentOrNextSpecificDayOfWork(this DateTime dt, DayOfWeek dayOfWeek)
        {
            if (dt.DayOfWeek == dayOfWeek)
                return dt;
            return dt.GetNextSpecificDayOfWork(dayOfWeek);
        }

        /// <summary>
        /// 返回下一个指定的日期
        /// 如2016/12/27 ,获取下一个20号:2017/1/20
        /// 2017/1/31 获取下一个31号:2017/3/31
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dayOfMonth"></param>
        /// <returns></returns>
        public static DateTime GetNextSpecificDayOfMonth(this DateTime dt, int dayOfMonth)
        {
            if (dayOfMonth < 1 || dayOfMonth > 31)
                throw new ArgumentOutOfRangeException(nameof(dayOfMonth));

            if (dayOfMonth <= 28)
                return dt.GetNextMonthFirstDay().AddDays(dayOfMonth);

            int offset = dayOfMonth - dt.Day;

            var d = dt.AddDays(offset);

            if (offset > 0)
            {
                if (d.Month == dt.Month)
                    return d;

                d = new DateTime(dt.Year, dt.Month, 1).AddMonths(1);
                int i = 0;
                while (true)
                {
                    try
                    {
                        var dd = d.AddDays(dayOfMonth).AddMonths(i++);
                        return dd;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                    }
                }
            }
            else
            {
                int i = 1;
                while (true)
                {
                    var next = d.AddMonths(i++);
                    if (next.Day == dayOfMonth)
                        return next;
                }
            }
        }

        /// <summary>
        /// 返回下一个指定的号,包含自身
        /// 如2016/12/27 ,获取下一个20号:2017/1/20
        /// 2017/1/31 获取下一个31号:2017/1/31
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dayOfMonth"></param>
        /// <returns></returns>
        public static DateTime GetCurrentOrNextSpecificDayOfMonth(this DateTime dt, int dayOfMonth)
        {
            if (dt.Day == dayOfMonth)
                return dt;
            return dt.GetNextSpecificDayOfMonth(dayOfMonth);
        }

        public static string ToStringyyyyMMddHHmmss(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToStringyyyyMMddHHmm(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 判断两个日期是否是同一天
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool IsSameDay(this DateTime d1, DateTime d2)
        {
            return d1.Date == d2.Date;
        }

        /// <summary>
        /// 获取1970-1-1 到现在的秒数 使用UTC标准
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static int GetIntDate(this DateTime datetime)
        {
            return (int) new DateTimeOffset(datetime).ToUnixTimeSeconds();
        }

        /// <summary>
        /// 获取1970-1-1 到现在的秒数 使用UTC标准
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static long GetLongDate(this DateTime datetime)
        {
            return new DateTimeOffset(datetime).ToUnixTimeSeconds();
        }

        /// <summary>
        /// 计算生日
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static int GetAge(this DateTime birthday)
        {
            return GetAgeData(birthday).year;
        }

        /// <summary>
        /// 计算年龄,字符串,1岁1月
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static string GetAgeString(this DateTime birthday)
        {
            var res = GetAgeData(birthday);
            if (res.year == 0 && res.month == 0)
                return $"{res.day}天";
            if (res.year == 0)
                return $"{res.month}个月{res.day}天";
            if (res.year < 14)
                return $"{res.year}岁{res.month}个月";

            return $"{res.year}岁";
        }

        /// <summary>
        /// 计算年龄字符串
        /// 默认返回：xx岁xx月xx天
        /// </summary>
        /// <param name="birthday">第1个日期参数</param>
        private static (int year, int month, int day) GetAgeData(DateTime birthday)
        {
            if (birthday > DateTime.Now)
                return (0, 0, 0);

            var now = DateTime.Now;

            //判断时间段是否为正。若为负，调换两个时间点的位置。
            if (DateTime.Compare(birthday, now) > 0)
            {
                var dateTime = birthday;
                birthday = now;
                now = dateTime;
            }

            //定义：年、月、日
            int year, month, day;

            //计算：天
            day = now.Day - birthday.Day;
            if (day < 0)
            {
                day += DateTime.DaysInMonth(birthday.Year, birthday.Month);
                birthday = birthday.AddMonths(1);
            }

            //计算：月
            month = now.Month - birthday.Month;
            if (month < 0)
            {
                month += 12;
                birthday = birthday.AddYears(1);
            }

            //计算：年
            year = now.Year - birthday.Year;

            //返回格式化后的结果
            return (year, month, day);
        }

        /// <summary>
        /// 获取所在季度的第一天
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static DateTime GetQuarterFirstDay(this DateTime today)
        {
            var quarter = today.GetQuarter();
            return new DateTime(today.Year, (quarter - 1) * 3 + 1, 1);
        }

        /// <summary>
        /// 获取所在季度的最后一天
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static DateTime GetQuarterLasyDay(this DateTime today)
        {
            var quarter = today.GetQuarter();
            return new DateTime(today.Year, (quarter - 1) * 3 + 1, 1)
                .AddMonths(3);
        }

        /// <summary>
        /// 获取季度
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static int GetQuarter(this DateTime today)
        {
            return (today.Month - 1) / 3 + 1;
        }

        /// <summary>
        /// int转换为datetime,本地时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime IntToDateTime(this int value)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return startTime.AddSeconds(value).ToLocalTime();
        }

        /// <summary>
        /// int转换为datetime,本地时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime LongToDateTime(this long value)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return startTime.AddSeconds(value).ToLocalTime();
        }
    }
}