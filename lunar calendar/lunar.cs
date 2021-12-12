using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lunar_calendar
{
    [Serializable]
    public class Lunar
    {

        private const int sqlCommandTimeout = 1000;

        internal static int SqlCommandTimeout => sqlCommandTimeout;

        /// <summary>
        ///  MalePlus = 陽男, FemalMinuse = 陰女, MaleMinus = 陰男, FemalePlus = 陽女
        /// </summary>
        internal enum Gender : int
        {
            None = 0, MalePlus = 1, FemalMinuse = 2, MaleMinus = 3, FemalePlus = 4
        }

        /// <summary>
        /// Get lunae date
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>Dictionary(): StrCYear,StrCMonth,StrCDay</returns>
        public Dictionary<string, string> GetLunarDate(DateTime dateTime)
        {
            DateTime dtDateTime = new(dateTime.Year, dateTime.Month, dateTime.Day);
            //DowWeekend = dtDateTime.DayOfWeek;
            TaiwanLunisolarCalendar tlc = new();
            int leapMouth = tlc.GetLeapMonth(tlc.GetYear(dtDateTime));
            int LuniMouth = tlc.GetMonth(dtDateTime);
            if (leapMouth > 0)
            {
                if (LuniMouth == leapMouth)
                {
                    LuniMouth = leapMouth - 1;
                }
                else if (LuniMouth > leapMouth)
                {
                    LuniMouth -= 1;
                }
            }
            return new Dictionary<string, string>() {
                { "StrCYear", tlc.GetYear(dtDateTime).ToString(CultureInfo.InvariantCulture) },
                { "StrCMonth", LuniMouth.ToString("d2", CultureInfo.InvariantCulture) },
                { "StrCDay", tlc.GetDayOfMonth(dtDateTime).ToString("d2", CultureInfo.InvariantCulture) }
            };
        }


    }


}
