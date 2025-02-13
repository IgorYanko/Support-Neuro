using System;
using System.Collections.Generic;

namespace NeuroApp.Classes
{
    public static class BusinessDayCalculator
    {
        private static readonly Dictionary<int, HashSet<DateTime>> HolidayCache = new();

        private static HashSet<DateTime> GetHolidays(int year)
        {
            if (HolidayCache.TryGetValue(year, out var cachedHolidays))
                return cachedHolidays;

            var holidays = new HashSet<DateTime>
            {
                new DateTime(year, 1, 1),
                new DateTime(year, 4, 21),
                new DateTime(year, 5, 1),
                new DateTime(year, 9, 7),
                new DateTime(year, 10, 12),
                new DateTime(year, 11, 2),
                new DateTime(year, 11, 15),
                new DateTime(year, 11, 20),
                new DateTime(year, 12, 25)
            };

            DateTime easter = CalculateEasterSunday(year);
            holidays.Add(easter.AddDays(-47));
            holidays.Add(easter.AddDays(-2));
            holidays.Add(easter.AddDays(60));
            holidays.Add(new DateTime(year, 5, 22));

            HolidayCache[year] = holidays;
            return holidays;
        }

        private static DateTime CalculateEasterSunday(int year)
        {
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = ((h + l - 7 * m + 114) % 31) + 1;
            return new DateTime(year, month, day);
        }

        public static int CalculateBusinessDays(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return 0;

            int businessDays = 0;
            var holidays = new HashSet<DateTime>();

            for (int year = startDate.Year; year <= endDate.Year; year++)
            {
                holidays.UnionWith(GetHolidays(year));
            }

            DateTime date = startDate;
            while (date <= endDate)
            {
                if (date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday && !holidays.Contains(date.Date))
                {
                    businessDays++;
                }
                date = date.AddDays(1);
            }

            return businessDays;
        }

        public static DateTime AddBusinessDays(DateTime startDate, int businessDaysToAdd)
        {
            DateTime currentDate = startDate;
            int addedDays = 0;
            HashSet<DateTime> holidays = new HashSet<DateTime>();
            int currentYear = startDate.Year;

            holidays.UnionWith(GetHolidays(currentYear));

            while (addedDays < businessDaysToAdd)
            {
                currentDate = currentDate.AddDays(1);

                if (currentDate.Year != currentYear)
                {
                    currentYear = currentDate.Year;
                    holidays.UnionWith(GetHolidays(currentYear));
                }

                if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                if (holidays.Contains(currentDate.Date)) continue;

                addedDays++;
            }

            return currentDate;
        }

        public static DateTime CalculateDeadline(DateTime startDate)
        {
            int businessDaysToAdd = 10;
            return AddBusinessDays(startDate, businessDaysToAdd);
        }

        public static DateTime CalculateExpirationDate(DateTime startDate)
        {
            int businessDaysToAdd = 20;
            return AddBusinessDays(startDate,businessDaysToAdd);
        }
    }
}
