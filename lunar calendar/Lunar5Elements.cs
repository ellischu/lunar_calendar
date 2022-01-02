using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lunar_calendar
{
    public class Lunar5Elements
    {
        // NE02     水二局     JE05    水
        // NE03     木三局     JE01    木
        // NE04     金四局     JE04    金
        // NE05     土五局	   JE03    土
        // NE06     火六局     JE02    火

        public string StrYear { get; }
        public int Year => int.Parse(StrYear.Substring(2));

        public string StrMonth { get; }
        public int Month => int.Parse(StrMonth.Substring(2));

        public string StrDay { get; }
        public int Day => int.Parse(StrDay.Substring(2));

        public string StrHour { get; }
        public int Hour => int.Parse(StrHour.Substring(2));

        public Lunar5Elements(DateTime dateTime)
        {
            Lunar8Characters eightCharacters = new(dateTime);
            StrYear = Convert(new Lunar60Flower(eightCharacters.YearHS, eightCharacters.YearEB).StrElement5);
            StrMonth = Convert(new Lunar60Flower(eightCharacters.MonthHS, eightCharacters.MonthEB).StrElement5);
            StrDay = Convert(new Lunar60Flower(eightCharacters.DayHS, eightCharacters.DayEB).StrElement5);
            StrHour = Convert(new Lunar60Flower(eightCharacters.HourHS, eightCharacters.HourEB).StrElement5);
        }



        internal string Convert(string StrInput)
        {
            return new Dictionary<string, string>()
            {
                {"NE02", "JE05"}, {"NE03", "JE01"}, {"NE04", "JE04"}, {"NE05", "JE03"}, {"NE06", "JE02"}
            }[StrInput];
        }

    }
}
