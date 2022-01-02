using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lunar_calendar
{
    // PG01 乾   PG02 兌  PG03 離  PG04 震
    // PG05 巽   PG06 坎  PG07 艮  PG08 坤
    // 冬至以後 順掛 ,夏至以後 逆掛

    public class Lunar8Trigrams
    {
        public int Trigram { get; set; }

        public string StrTrigram => $"PG{Trigram:00}";

        public Lunar8Trigrams(DateTime dateTime)
        {
            //Lunar24SolarTerms lunar24SolarTerms = new(dateTime);
            Dictionary<int, DateTime> lunar24SolarTermsQi = new Lunar24SolarTerms(dateTime).Half24SolarQi;
            //Summer Solstice
            DateTime SummerSolstice = lunar24SolarTermsQi[6];
            //Winter Solstice
            DateTime WinterSolstice = lunar24SolarTermsQi[12];
            if (dateTime.Date >= WinterSolstice.Date)
            {
                Trigram = Tools.CheckRange(1 + (dateTime.Date - WinterSolstice.Date).Days, 1, 8);
            }
            else if (dateTime.Date >= SummerSolstice.Date && dateTime.Date < WinterSolstice.Date)
            {
                Trigram = Tools.CheckRange(8 - (dateTime.Date - SummerSolstice.Date).Days, 1, 8);
            }
            else
            {
                WinterSolstice = new Lunar24SolarTerms(dateTime.AddYears(-1)).Half24SolarQi[12];
                Trigram = Tools.CheckRange(1 + (dateTime.Date - WinterSolstice.Date).Days, 1, 8);
            }
        }
    }
}
