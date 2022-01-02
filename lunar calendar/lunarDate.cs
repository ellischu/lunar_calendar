using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;

namespace lunar_calendar
{
    [Serializable]
    public class LunarDate
    {
        //子　２３～０１點
        //丑　０１～０３點
        //寅　０３～０５點
        //卯　０５～０７點
        //辰　０７～０９點
        //巳　０９～１１點
        //午　１１～１３點
        //未　１３～１５點
        //申　１５～１７點
        //酉　１７～１９點
        //戌　１９～２１點
        //亥　２１～２３點

        /// <summary>
        /// Lunar Year
        /// </summary>
        public int Year { get; }
        /// <summary>
        /// Heavenly Stems of Year
        /// </summary>
        public int YearHS { get; }
        /// <summary>
        /// Earthly Branches of Year
        /// </summary>
        public int YearEB { get; }
        /// <summary>
        /// Heavenly Stems of Year
        /// </summary>
        public string StrYearHS => $"XX{YearHS:00}";
        /// <summary>
        /// Earthly Branches of Year
        /// </summary>
        public string StrYearEB => $"YY{YearEB:00}";

        /// <summary>
        /// Lunar Month
        /// </summary>
        public int Month { get; }
        /// <summary>
        /// Heavenly Stems of Month
        /// </summary>
        public int MonthHS { get; }
        /// <summary>
        /// Earthly Branches of Month
        /// </summary>
        public int MonthEB { get; }
        /// <summary>
        /// Heavenly Stems of Month
        /// </summary>
        public string StrMonthHS => $"XX{MonthHS:00}";
        /// <summary>
        /// Earthly Branches of Month
        /// </summary>
        public string StrMonthEB => $"YY{MonthEB:00}";

        /// <summary>
        /// Lunar Day
        /// </summary>
        public int Day { get; }
        /// <summary>
        /// Heavenly Stems of Day
        /// </summary>
        public int DayHS { get; }
        /// <summary>
        /// Earthly Branches of Day
        /// </summary>
        public int DayEB { get; }
        /// <summary>
        /// Heavenly Stems of Day
        /// </summary>
        public string StrDayHS => $"XX{DayHS:00}";
        /// <summary>
        /// Earthly Branches of Day
        /// </summary>
        public string StrDayEB => $"YY{DayEB:00}";

        /// <summary>
        /// Lunar Hour
        /// </summary>
        public int Hour { get; }
        /// <summary>
        /// Heavenly Stems of Hour
        /// </summary>
        public int HourHS { get; }
        /// <summary>
        /// Earthly Branches of Hour
        /// </summary>
        public int HourEB { get; }
        /// <summary>
        /// Heavenly Stems of Hour
        /// </summary>
        public string StrHourHS => $"XX{HourHS:00}";
        /// <summary>
        /// Earthly Branches of Hour
        /// </summary>
        public string StrHourEB => $"YY{HourEB:00}";

        /// <summary>
        /// Lunar Minute
        /// </summary>
        public int Minute { get; }

        /// <summary>
        /// Lunar Second
        /// </summary>
        public int Second { get; }

        /// <summary>
        /// Day of Year
        /// </summary>
        public int DayOfYear { get; }

        /// <summary>
        /// Day of month
        /// </summary>
        public int DayOfMonth { get; }

        /// <summary>
        /// Day of week
        /// </summary>
        public DayOfWeek DayOfWeek { get; }

        /// <summary>
        /// Is Leap Month
        /// </summary>
        public bool IsLeapMonth { get; }

        public int MonthsInYear { get; }

        public LunarDate(DateTime dateTime)
        {
            TaiwanLunisolarCalendar tlc = new();


            Year = tlc.GetYear(dateTime);
            //年干 (未修正)         
            YearHS = Tools.CheckRange(9 + (Year + 1911 - 1912) % 10, 1, 10);
            //年支 1912 壬子 (未修正) 壬寅
            YearEB = Tools.CheckRange(1 + (Year + 1911 - 1912) % 12, 1, 12);
            DayOfYear = tlc.GetDayOfYear(dateTime);

            int leapMonth = tlc.GetLeapMonth(tlc.GetYear(dateTime));
            Month = leapMonth > 0 && tlc.GetMonth(dateTime) >= leapMonth ? tlc.GetMonth(dateTime) - 1 : tlc.GetMonth(dateTime);
            //月干 (未修正)
            MonthHS = Tools.CheckRange(3 + (YearHS - 1) % 5 * 2 + (Month - 1), 1, 10);
            //月支 (未修正)
            MonthEB = Tools.CheckRange(3 + (Month - 1), 1, 12);

            IsLeapMonth = (Month == leapMonth);
            MonthsInYear = tlc.GetMonthsInYear(Year);
            DayOfMonth = tlc.GetDayOfMonth(dateTime);

            DateTime startDateTime = new(1900, 2, 20, 0, 0, 0);
            // 1900.02.20 00:00:00 甲子日
            int days = (dateTime - startDateTime).Days;
            Day = tlc.GetDayOfMonth(dateTime);
            DayHS = days % 10 + 1;
            DayEB = days % 12 + 1;
            DayOfWeek = tlc.GetDayOfWeek(dateTime);

            Hour = tlc.GetHour(dateTime);
            HourEB = Tools.CheckRange(dateTime.Hour % 2 == 0 ? (dateTime.Hour) / 2 + 1 : (dateTime.Hour + 1) / 2 + 1, 1, 12);

            HourHS = Tools.CheckRange(((DayHS - 1) % 5) * 2 + HourEB, 1, 10);

            Minute = tlc.GetMinute(dateTime);
            Second = tlc.GetSecond(dateTime);
        }
    }

}
