using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lunar_calendar
{

    //九宮飛星
    // KF01 1白  KF02 2黑     KF03 3碧	
    // KF04 4綠  KF05 5黃     KF06 6白
    // KF07 7赤  KF08 8白     KF09 9紫

    //國12月(冬至) 第1個甲子(1白)||(6白) 順算
    //國02月(雨水) 第1個甲子(7赤)||(3碧) 順算
    //國04月(穀雨) 第1個甲子(4綠)||(9紫) 順算

    //國06月(夏至) 第1個甲子(9紫)||(4綠) 逆算
    //國08月(處暑) 第1個甲子(3碧)||(7赤) 逆算
    //國10月(霜降) 第1個甲子(6白)||(1白) 逆算


    public class LunarJiugongFlyStar
    {
        public int Year { get; set; }
        public string StrYear => $"KF{Year:00}";

        public int Month { get; set; }
        public string StrMonth => $"KF{Month:00}";

        public int Day { get; set; }
        public string StrDay => $"KF{Day:00}";

        public int Hour { get; set; }
        public string StrHour => $"KF{Hour:00}";


        public LunarJiugongFlyStar(DateTime dateTime)
        {
            Year = GetYear(dateTime);
            Month = GetMonth(dateTime);
            Day = GetDay(dateTime);
            Hour = GetHour(dateTime);
        }

        private int GetHour(DateTime dateTime)
        {
            int hour = 1;
            LunarDate lunarDate = new(dateTime);
            if (new int[] { 1, 4, 7, 10 }.Contains(lunarDate.DayEB))
            {
                hour = Tools.CheckRange(9 - (lunarDate.HourEB - 1), 1, 9);
            }
            if (new int[] { 3, 6, 9, 12 }.Contains(lunarDate.DayEB))
            {
                hour = Tools.CheckRange(6 - (lunarDate.HourEB - 1), 1, 9);
            }
            if (new int[] { 2, 5, 8, 11 }.Contains(lunarDate.DayEB))
            {
                hour = Tools.CheckRange(3 - (lunarDate.HourEB - 1), 1, 9);
            }
            return hour;
        }

        private int GetDay(DateTime dateTime)
        {
            Dictionary<int, DateTime> DicSolarTermsQi = GetdicSolarTermsQi(dateTime);
            //冬至(WS) Winter Solstice 00 (Year -1)

            DateTime WinterSolstice00 = DicSolarTermsQi[0];
            //雨水(RW) The Rains
            DateTime theRains = DicSolarTermsQi[1];
            //穀雨(GR) Grain Rain
            DateTime GrainRain = DicSolarTermsQi[2];

            //夏至(SS) Summer Solstice
            DateTime SummerSolstice = DicSolarTermsQi[3];
            //處暑(LH) The Limite of Heat(Stopping of Heat)
            DateTime TheLimiteOfHeat = DicSolarTermsQi[4];
            //霜降(FD) Frost's Descent(First Frost)
            DateTime FrostDescent = DicSolarTermsQi[5];

            //冬至(WS) Winter Solstice
            DateTime WinterSolstice = DicSolarTermsQi[6];

            int Star = 1;
            // month : date < SummerSolstice(6) Ascending

            //month 1,2 
            if ((dateTime.Date - theRains.Date).Days < 0)
            {
                Star = GetJFStar(dateTime, WinterSolstice00, 1, 6, SortOrder.Ascending);
            }
            //month 2 theRains             
            if ((dateTime.Date - theRains.Date).Days == 0)
            {
                Star = (dateTime - theRains).TotalMilliseconds < 0 ?
                            GetJFStar(dateTime, WinterSolstice00, 1, 6, SortOrder.Ascending) :
                            GetJFStar(dateTime, theRains, 7, 3, SortOrder.Ascending);
            }
            //month 2,3,4
            if ((dateTime.Date - theRains.Date).Days > 0 && (dateTime.Date - GrainRain.Date).Days < 0)
            {
                Star = GetJFStar(dateTime, theRains, 7, 3, SortOrder.Ascending);
            }
            //month 4 GrainRain
            if ((dateTime.Date - GrainRain.Date).Days == 0)
            {
                Star = (dateTime - GrainRain).TotalMilliseconds < 0 ?
                            GetJFStar(dateTime, theRains, 7, 3, SortOrder.Ascending) :
                            GetJFStar(dateTime, GrainRain, 4, 9, SortOrder.Ascending);
            }
            //month 4,5,6
            if ((dateTime.Date - GrainRain.Date).Days > 0 && (dateTime.Date - SummerSolstice.Date).Days < 0)
            {
                Star = GetJFStar(dateTime, GrainRain, 4, 9, SortOrder.Ascending);
            }
            //month 6 SummerSolstice
            if ((dateTime.Date - SummerSolstice.Date).Days == 0)
            {
                Star = (dateTime - SummerSolstice).TotalMilliseconds < 0 ?
                            GetJFStar(dateTime, GrainRain, 4, 9, SortOrder.Ascending) :
                            GetJFStar(dateTime, SummerSolstice, 9, 4, SortOrder.Descending);
            }
            // month : Winter Solstice(12) < date >= SummerSolstice(6) Descending 
            //month 6,7,8
            if ((dateTime.Date - SummerSolstice.Date).Days > 0 && (dateTime.Date - TheLimiteOfHeat.Date).Days < 0)
            {
                Star = GetJFStar(dateTime, SummerSolstice, 9, 4, SortOrder.Descending);
            }
            //month 8 TheLimiteOfHeat
            if ((dateTime.Date - TheLimiteOfHeat.Date).Days == 0)
            {
                Star = (dateTime - TheLimiteOfHeat).TotalMilliseconds < 0 ?
                            GetJFStar(dateTime, SummerSolstice, 9, 4, SortOrder.Descending) :
                            GetJFStar(dateTime, TheLimiteOfHeat, 3, 7, SortOrder.Descending);
            }
            //month 8,9,10
            if ((dateTime.Date - TheLimiteOfHeat.Date).Days > 0 && (dateTime.Date - FrostDescent.Date).Days < 0)
            {
                Star = GetJFStar(dateTime, TheLimiteOfHeat, 3, 7, SortOrder.Descending);
            }
            //month 10 FrostDescent
            if ((dateTime.Date - FrostDescent.Date).Days == 0)
            {
                Star = (dateTime - FrostDescent).TotalMilliseconds < 0 ?
                            GetJFStar(dateTime, TheLimiteOfHeat, 3, 7, SortOrder.Descending) :
                            GetJFStar(dateTime, FrostDescent, 6, 1, SortOrder.Descending);
            }
            //month 10,11,12
            if ((dateTime.Date - FrostDescent.Date).Days > 0 && (dateTime.Date - WinterSolstice.Date).Days < 0)
            {
                Star = GetJFStar(dateTime, FrostDescent, 6, 1, SortOrder.Descending);
            }
            //month 12 WinterSolstice
            if ((dateTime.Date - WinterSolstice.Date).Days == 0)
            {
                Star = (dateTime - WinterSolstice).TotalMilliseconds < 0 ?
                            GetJFStar(dateTime, FrostDescent, 6, 1, SortOrder.Descending) :
                            GetJFStar(dateTime, WinterSolstice, 1, 6, SortOrder.Ascending);
            }
            // month : date >= Winter Solstice(12) Ascending 
            if ((dateTime.Date - WinterSolstice.Date).Days > 0)
            {
                //month 12
                Star = GetJFStar(dateTime, WinterSolstice, 1, 6, SortOrder.Ascending);
            }
            return Star;
        }

        private int GetMonth(DateTime dateTime)
        {
            int month = 1;
            LunarDate lunarDate = new(dateTime);
            Lunar8Characters lunar8Characters = new(dateTime);
            if (new int[] { 1, 4, 7, 10 }.Contains(lunar8Characters.YearEB))
            {
                month = Tools.CheckRange(8 - (lunarDate.Month - 1), 1, 9);
            }
            if (new int[] { 3, 6, 9, 12 }.Contains(lunar8Characters.YearEB))
            {
                month = Tools.CheckRange(2 - (lunarDate.Month - 1), 1, 9);
            }
            if (new int[] { 2, 5, 8, 11 }.Contains(lunar8Characters.YearEB))
            {
                month = Tools.CheckRange(5 - (lunarDate.Month - 1), 1, 9);
            }
            return month;
        }

        private int GetYear(DateTime dateTime)
        {
            return Tools.CheckRange(7 - (dateTime.Year - 1912), 1, 9);
        }

        private static Dictionary<int, DateTime> GetdicSolarTermsQi(DateTime dateTime)
        {
            Dictionary<int, DateTime> result = new();
            for (int month = 1; month < 14; month += 2)
            {
                DateTime SolarTermsQi = new Lunar24SolarTerms((month - 1) / 2 == 0 ? dateTime.AddYears(-1) : dateTime).Half24SolarQi[Tools.CheckRange(((month - 1) / 2) * 2, 1, 12)];
                result[(month - 1) / 2] = SolarTermsQi;
            }
            return result;
        }

        private static int GetJFStar(DateTime dateTime, DateTime SolarTerms, int Start1, int Start2, SortOrder sortOrder)
        {
            //算出甲子日
            int solarDayHSEB = Get60Flower(new Lunar8Characters(SolarTerms).DayHS, new Lunar8Characters(SolarTerms).DayEB);
            DateTime XY01 = SolarTerms.AddDays(solarDayHSEB == 1 ? 0 : 61 - solarDayHSEB); ;
            //甲子日前一天
            DateTime XY60 = XY01.AddDays(-1);
            if ((dateTime.Date - XY01.Date).Days >= 0)
            {
                return sortOrder == SortOrder.Ascending ? Tools.CheckRange(Start1 + (dateTime.Date - XY01.Date).Days, 1, 9) :
                                                          Tools.CheckRange(Start1 - (dateTime.Date - XY01.Date).Days, 1, 9);
            }
            else
            {
                return sortOrder == SortOrder.Ascending ? Tools.CheckRange(Start2 - (XY60.Date - dateTime.Date).Days, 1, 9) :
                                                          Tools.CheckRange(Start2 + (XY60.Date - dateTime.Date).Days, 1, 9);
            }
        }

        private static int Get60Flower(int HS, int EB)
        {
            int int60Flowers;
            if (EB > HS)
            {
                int60Flowers = (6 - (EB - HS) / 2) * 10 + HS;
            }
            else
            {
                int60Flowers = (HS - EB) / 2 * 10 + HS;
            }

            return int60Flowers;
        }

    }
}
