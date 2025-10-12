using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace PortoWeb.Helpers
{
    public static class DateTimeExtensions
    {
        public static string ToPersianDate(this DateTime dateTime)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            int year = persianCalendar.GetYear(dateTime);
            int month = persianCalendar.GetMonth(dateTime);
            int day = persianCalendar.GetDayOfMonth(dateTime);

            return $"{year}/{month:00}/{day:00}";
        }
    }
}
