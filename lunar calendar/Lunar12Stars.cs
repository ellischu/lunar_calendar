using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lunar_calendar
{
    public class Lunar12Stars
    {
        //IS01建	IS02除	IS03滿	IS04平	IS05定	IS06執
        //IS07破	IS08危	IS09成	IS10收	IS11開	IS12閉

        //風水學術語：
        //正(寅)月建寅   二(卯)月建卯     三(辰)月建辰     四(巳)月建巳     五(午)月建午     六(未)月建未，
        //七(申)月建申   八(酉)月建酉     九(戌)月建戌     十(亥)月建亥     十一(子)月建子   十二(丑)月建丑。

        public int Star { get; set; }
        public string StrStar => $"IS{Star:00}";

        public Lunar12Stars(DateTime Today)
        {

            DateTime SolarDate = new Lunar24SolarTerms(Today).Half24SolarTerms[Today.Month];
            TimeSpan checkMonth = Today - SolarDate; // 節交界 修正月干支
            if (checkMonth.TotalMilliseconds > 0)
            {
                Lunar8Characters lunarToday = new(Today);
                Star = Tools.CheckRange(lunarToday.DayEB > lunarToday.MonthEB ?
                               13 - (lunarToday.MonthEB - lunarToday.DayEB) :
                               1 - (lunarToday.MonthEB - lunarToday.DayEB), 1, 12);
            }
            //else if (checkMonth == 0)
            //{
            //    //Lunar8Characters lunarToday = new(Today);
            //    if (SolarDate.Hour >= 12)
            //    {
            //        Star = new Lunar12Stars(Today.AddDays(+1)).Star;
            //    }
            //    else
            //    {
            //        Star = new Lunar12Stars(Today.AddDays(-1)).Star;
            //    }
            //}
            else if (checkMonth.TotalMilliseconds <= 0)
            {
                SolarDate = new Lunar24SolarTerms(Today.AddDays(-15)).Half24SolarTerms[Today.AddDays(-15).Month];
                Lunar8Characters lunarToday = new(Today);
                Lunar8Characters lunarSolarDate = new(SolarDate);
                int BaseStar = Tools.CheckRange(lunarSolarDate.DayEB > lunarToday.MonthEB ?
                               13 - (lunarToday.MonthEB - lunarSolarDate.DayEB) :
                               1 - (lunarToday.MonthEB - lunarSolarDate.DayEB), 1, 12);
                Star = Tools.CheckRange(BaseStar + (DateTime.DaysInMonth(SolarDate.Year, SolarDate.Month) - SolarDate.Day) + Today.Day, 1, 12);
            }
        }
    }
}
