using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace lunar_calendar
{
    //天干順序：甲、乙、丙、丁、戊、己、庚、辛、壬、癸。
    //地支順序：子（鼠）、丑（牛）、寅（虎）、卯（兔）、辰（龍）、巳（蛇）、
    //          午（馬）、未（羊）、申（猴）、酉（雞）、戌（狗）、亥（豬）。
    //
    //一，起年柱。
    //第一個規律是：凡是
    //              尾數是4的都是甲天干；尾數是5的都是乙天干；尾數是6的都是丙天干；尾數是7的都是丁天干；尾數是8的都是戊天干；
    //              尾數是9的都是己天干；尾數是0的都是庚天干；尾數是1的都是辛天干；尾數是2的都是壬天干；尾數是3的都是癸天干。
    //
    //第二個規律：凡是
    //            2000年以前，所有後兩位能被12整除的年份地支都是子，也就是鼠年，知道了這一點，
    //            我們就知道了1900至2000年任何一年的地支，因為我們只需要找到離相應年份最近的子年，
    //            然後根據地支順序就可以得出了。
    //            舉例說明：1980年，離1980年最近的後兩位能被12整除的年份是1984，那麼根據地支順序逆序推出，
    //            子（1984）、亥（1983）、戌（1982）、酉（1981）、申（1980），
    //            1980年尾數是0所以天干為庚，由上得出1980為庚申年；
    //            再比如1999年，離1999年最近的後兩位能被12整除的年份是1996，那麼根據地支順序推出，
    //            子（1996）、丑（1997）、寅（1998）、卯（1999），
    //            1999年尾數是9所以天干為己，由上得出1999為己卯年。
    //第三個規律：凡是
    //            2000年以後，所有後兩位能被12整除的年份地支都是辰，也就是龍年，知道了這一點，
    //            我們就知道了2000至2100年任何一年的地支，因為我們只需要找到離相應年份最近的辰年，
    //            然後根據地支順序就可以得出了。
    //            舉例說明：2017年，離2017年最近的後兩位能被12整除的年份是2012，那麼根據地支順序推出，
    //            辰（2012）、巳（2013）、午（2014）、未（2015）、申（2016）、酉（2017），
    //            2017年尾數是7所以天干為丁，由上得出2017為丁酉年。
    //
    //二，起月柱。
    //起月柱的難點主要在地支上，月柱的地支簡稱月支。
    //首先，在甲子歷中，月地支不是從子開始而是從寅開始，簡稱正月建寅，
    //年與年的界限是立春，而月與月的界限是交節，交節中的節就是二十四節氣中的節，
    //二十四節氣包括了節和氣，其中
    //節為：立春、驚蟄、清明、立夏、芒種、小暑、立秋、白露、寒露、立冬、大雪、小寒。
    //氣為：雨水、春分、穀雨、小滿、夏至、大暑、處暑、秋分、霜降、小雪、冬至、大寒。
    //推算月柱地支，要以節來推算，不能用氣來推算。
    //
    //既然是以節為界，那麼我們舉個例子，如果正好是芒種出生，那麼到底算上一個月還是算下一個月呢？
    //要判斷準確就要通過萬年曆查交節時辰。舉例來說，通過萬年曆查詢1995年芒種是陽曆6月6日，交節時間是11：42：28，
    //在這個時間以前出生的都算上一個月的也就是辛巳月，這個時間以後的算壬午月。
    //確定了月柱的地支，我們再來看看月干，和月干相比，月支是更為固定的，
    //因為正月建寅，十二個月正好對應十二個地支，而天干則有十個，十個天干輪配十二地支，每六年一個輪迴。形成了以下規律：
    //
    //甲己之年丙作首，乙庚之歲戊為頭，丙辛必定尋庚起，丁壬壬位順行流，更有戊癸何方覓，甲寅之上好追求。
    //
    //口訣的意思是：
    //甲己之年正月起丙寅，
    //乙庚之年正月起戊寅，
    //丙辛之年正月起庚寅，
    //丁壬之年正月起壬寅，
    //戊癸之年正月起甲寅。
    //
    //有些朋友可能對閏月的問題有些疑問，但是閏月是陰曆里的，和甲子歷沒什麼關係，
    //甲子歷中月的分界無需注意閏月，只需注意「節」就可以了。
    //
    //三，起日柱。
    //起日柱可通過萬年曆確定，不過要注意時辰的劃分，尤其是子時，子時是（晚上：11:00至次日1:00），
    //如果你是凌晨0:15分出生，那麼應該算上一日的子時還是下一日的子時呢？
    //可以明確的回答你，是下一日的子時，因為子時分為早子時和晚子時，如果是0點以後出生就是第二日的早子時，
    //如果是0點以前，就是第一日的晚子時。
    //
    //四，起時柱。
    //起時柱的難點在於起時干，時支都是固定的，時支如下：
    //23時——次日01時為子時；
    //01時——03時為丑時；
    //03時——05時為寅時；
    //05時——07時為卯時；
    //07時——09時為辰時；
    //09時——11時為巳時；
    //11時——13時為午時；
    //13時——15時為未時；
    //15時——17時為申時；
    //17時——19時為酉時；
    //19時——21時為戌時；
    //21時——23時為亥時。
    //
    //關於子時的內容需要注意，早子時和晚子時，不過這部分內容在日柱中已經介紹，在此就不多做贅述了。
    //時干需要通過日干來確定，有一個口訣：
    //
    //甲己還生甲，乙庚丙作初，丙辛從戊起，丁壬庚子居，戊癸何方發，壬子是真途。
    //
    //甲己日起自甲子時，
    //乙庚日起自丙子時，
    //丙辛日起自戊子時，
    //丁壬日起自庚子時，
    //戊癸日起自壬子時，
    //都是依次順推。
    //
    //說到時柱的確定，還要注意夏令時的影響。在資源短缺、爭取日照的年代，很多國家和地區採用過夏令時，
    //即比標準時間快了一個小時，一個小時等於半個時辰，所以如果是夏令時的話，要回撥一個小時，才能算出準確的時柱。
    //對於時柱還有些朋友有著疑問，如果我是在國外出生的，和北京時間不統一怎麼辦？
    //這個問題很好解決，在哪裡出生，就用哪裡的時間，因為時間不過是反映了地球和太陽的相對位置，
    //你所在的位置不同，時間自然就不同了。


    public class Lunar8Characters
    {
        // Heavenly Stems and Earthly Branches

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

        public int Hour { get; }
        public int Minute { get; }
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

        public Lunar8Characters(DateTime dateTime)
        {
            Lunar24SolarTerms lunar_24SolarTerms = new(dateTime);
            // 立春交界 修正年干支
            DateTime SolarDayYear = lunar_24SolarTerms.Half24SolarTerms[2];
            bool checkYear = DateTime.Compare(dateTime, SolarDayYear) >= 0;
            LunarDate lunarYear = checkYear ? new(lunar_24SolarTerms.Half24SolarQi[2]) :
                                              new(new Lunar24SolarTerms(dateTime.AddYears(-1)).Half24SolarQi[12]);

            Year = lunarYear.Year;
            //年干 (修正)
            YearHS = lunarYear.YearHS;
            //年支 (修正)
            YearEB = lunarYear.YearEB;

            //if (checkYear && LunarYear + 1911 != intEYear) { Year += 1; YearHS += 1; YearEB += 1; }
            //if (!checkYear && LunarYear + 1911 == intEYear) { Year -= 1; YearHS -= 1; YearEB -= 1; }
            //YearHS = Tools.CheckRange(YearHS, 1, 10); YearEB = Tools.CheckRange(YearEB, 1, 12);

            // 節交界 修正月干支
            DateTime SolarDayMonth = lunar_24SolarTerms.Half24SolarTerms[dateTime.Month];
            bool checkMonth = DateTime.Compare(dateTime, SolarDayMonth) >= 0;
            LunarDate LunarMonth = checkMonth ? new(lunar_24SolarTerms.Half24SolarQi[dateTime.Month]) :
                                                new(new Lunar24SolarTerms(dateTime.AddDays(-15)).Half24SolarQi[dateTime.AddDays(-15).Month]);
            //if (!checkMonth)
            //{
            //    LunarMonth = dateTime.Month != 1 ? new(lunar_24SolarTerms.Half24SolarQi[dateTime.Month - 1]) :
            //                                      new(new Lunar24SolarTerms(dateTime.AddYears(-1)).Half24SolarQi[12]);
            //}

            Month = LunarMonth.Month;
            MonthHS = LunarMonth.MonthHS;
            MonthEB = LunarMonth.MonthEB;

            //bool checkMonth01 = Tools.CheckRange(dateTime.Month - 1, 1, 12) != lunarDate.Month;
            //Month = checkMonth && checkMonth01 ? lunarDate.Month + 1 : lunarDate.Month;
            //月干 (修正)
            //MonthHS = Tools.CheckRange(checkMonth && checkMonth01 ? lunarDate.MonthHS + 1 : lunarDate.MonthHS, 1, 10);
            //月支 (修正)
            //MonthEB = Tools.CheckRange(checkMonth && checkMonth01 ? lunarDate.MonthEB + 1 : lunarDate.MonthEB, 1, 12);
            //if (checkMonth && LunarMonth != Tools.CheckRange((intEMonth - 1), 1, 12)) { Month += 1; MonthHS += 1; MonthEB += 1; }
            //if (!checkMonth && LunarMonth == Tools.CheckRange((intEMonth - 1), 1, 12)) { Month -= 1; MonthHS -= 1; MonthEB -= 1; }
            //Month = Tools.CheckRange(Month, 1, 12); MonthHS = Tools.CheckRange(MonthHS, 1, 10); MonthEB = Tools.CheckRange(MonthEB, 1, 12);
            LunarDate lunarDate = new(dateTime);
            Day = lunarDate.Day;
            DayHS = lunarDate.DayHS;
            DayEB = lunarDate.DayEB;

            Hour = lunarDate.Hour;
            HourHS = lunarDate.HourHS;
            HourEB = lunarDate.HourEB;

            Minute = lunarDate.Minute;
        }

    }

}
