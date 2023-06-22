using System;

namespace Assets.Scripts.Common
{
    public static class DateTimeExtension
    {
        private static readonly DateTime UNIX_EPOCH = DateTime.Parse("1970-01-01 00:00:00");
        private static readonly TimeSpan OFFSET = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);

        public static DateTime UnixTimestamp2LocalTime(long seconds)
        {
            return UNIX_EPOCH.AddSeconds(seconds) + OFFSET;
        }

        public static long ToUnixTimestamp(this DateTime date)
        {
            double nowTicks = (date.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
            return (long)nowTicks;
        }

        public static DateTime GetDate(this DateTime date, DayOfWeek dayOfWeek)
        {
            int diff = dayOfWeek - date.DayOfWeek;
            if (diff > 0) diff -= 7;
            return date.AddDays(diff);
        }

        public static int DaysInMonth(this DateTime dt)
        {
            return DateTime.DaysInMonth(dt.Year, dt.Month);
        }

        public static DateTime BeginOfMonth(this DateTime dt)
        {
            return dt.AddDays((dt.Day - 1) * -1);
        }

        public static int MonthDiff(this DateTime dt, DateTime target)
        {
            var dtFrom = DateTime.MinValue;
            var dtTo = DateTime.MaxValue;

            if (dt < target)
            {
                dtFrom = dt;
                dtTo = target;
            }
            else
            {
                dtFrom = target;
                dtTo = dt;
            }
            return GetElapsedMonths(dtFrom, dtTo);
        }

        private static int GetElapsedMonths(DateTime baseDate, DateTime target)
        {
            var elapsedMonths = (target.Year - baseDate.Year) * 12 + (target.Month - baseDate.Month);
            
            // [1]baseDayの日部分がdayの日部分以上の場合は、その月を満了しているとみなす.
            //    (例:1月30日→3月30日以降の場合は満(3-1)ヶ月)
            // [2]baseDayの日部分がdayの表す月の末日以降の場合は、その月を満了しているとみなす.
            //    (例:1月30日→2月28日(平年2月末日)/2月29日(閏年2月末日)以降の場合は満(2-1)ヶ月)
            if (baseDate.Day <= target.Day || (target.Day == DateTime.DaysInMonth(target.Year, target.Month) && target.Day <= baseDate.Day))
            {
                return elapsedMonths;
            }
            else
            {
                return elapsedMonths - 1;
            }
        }
    }
}
