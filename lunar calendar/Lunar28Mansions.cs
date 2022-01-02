using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lunar_calendar
{
    //HS01角 HS02亢   HS03氐   HS04房   HS05心   HS06尾   HS07箕   東方稱青龍：角木蛟 亢金龍 氐土貉 房日兔 心月狐 尾火虎 箕水豹
    //HS08斗 HS09牛   HS10女   HS11虛   HS12危   HS13室   HS14壁   北方稱玄武：鬥木獬 牛金牛 女土蝠 虛日鼠 危月燕 室火豬 壁水獝
    //HS15奎 HS16婁   HS17胃   HS18昂   HS19畢   HS20觜   HS21參   西方稱白虎：奎木狼 婁金狗 胃土雉 昴日雞 畢月烏 觜火猴 參水猿
    //HS22井 HS23鬼   HS24柳   HS25星   HS26張   HS27翼   HS28軫   南方稱朱雀：井木犴 鬼金羊 柳土獐 星日馬 張月鹿 翼火蛇 軫水蚓

    public class Lunar28Mansions
    {
        public int YearZodiac { get; set; }
        public int MonthZodiac { get; set; }
        public int DayZodiac { get; set; }
        public int HourZodiac { get; set; }

        public string StrYearZodiac => $"HS{YearZodiac:00}";
        public string StrMonthZodiac => $"HS{MonthZodiac:00}";
        public string StrDayZodiac => $"HS{DayZodiac:00}";
        public string StrHourZodiac => $"HS{HourZodiac:00}";


        public Lunar28Mansions(DateTime dateTime)
        {
            DateTime startDateTime = new(1912, 2, 18, 0, 0, 0);
            TimeSpan timeSpan = dateTime - startDateTime;

            LunarDate startlunarDate = new(startDateTime);
            LunarDate lunarDate = new(dateTime);

            int Years = lunarDate.Year - startlunarDate.Year;

            bool checkMonth = DateTime.Compare(dateTime, new Lunar24SolarTerms(dateTime).Half24SolarTerms[dateTime.Month]) >= 0; // 節交界 修正月干支
            int Months = (12 - startDateTime.Month) + (dateTime.Year - startDateTime.Year - 1) * 12 + dateTime.Month;
            if (checkMonth && lunarDate.Month != Tools.CheckRange((dateTime.Month - 1), 1, 12)) { Months += 1; }
            if (!checkMonth && lunarDate.Month == Tools.CheckRange((dateTime.Month - 1), 1, 12)) { Months -= 1; }

            int Days = timeSpan.Days;
            int Hours = Days * 24 + timeSpan.Hours;

            YearZodiac = Tools.CheckRange(Years % 28 + 23, 1, 28);
            MonthZodiac = Tools.CheckRange(Months % 28 + 17, 1, 28);
            DayZodiac = Tools.CheckRange(Days % 28 + 11, 1, 28);
            HourZodiac = Tools.CheckRange((Hours / 2) % 28 - 17, 1, 28);
        }
    }
}

