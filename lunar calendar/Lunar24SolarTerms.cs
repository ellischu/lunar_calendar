using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace lunar_calendar
{
    //*********************************************************************************
    // 節氣無任何確定規律,所以只好建立表格對應
    //**********************************************************************************}
    // 立春(BS) Beginning of Spring                   雨水(RW) The Rains
    // 驚蟄(WI) The Waking of Insects                 春分(SE) The Spring Equinox(Vernal Equinox)
    // 清明(PB) Pure Brighness(Qingming Festival)     穀雨(GR) Grain Rain
    // 立夏(BU) Beginning of Summer                   小滿(GF) Grain Full(Grain Buds)
    // 芒種(GE) Grain in Ear                          夏至(SS) Summer Solstice
    // 小暑(SH) Slight Heat(Lesser Heat)              大暑(GH) Great Heat
    // 立秋(GA) Beginning of Autumn                   處暑(LH) The Limite of Heat(Stopping of Heat)
    // 白露(WD) White Dew                             秋分(AE) Autumn Equinox
    // 寒露(CD) Cold Dew                              霜降(FD) Frost's Descent(First Frost)
    // 立冬(BW) Beginning of Winter                   小雪(SN) Slight Snow
    // 大雪(GN) Great Snow(Heavy Snow)                冬至(WS) Winter Solstice
    // 小寒(SC) Slight Cold(Lesser Cold)              大寒(GC) Great Cold


    // web crawler for
    // Twenty four solar terms
    // https://hl.zdic.net/msjr/jqlist/1999.html (1999-2049)
    // https://www.ximizi.net/24jieqi_1.php?year=2100 (1800-2100)

    public class Lunar24SolarTerms
    {
        public DataTable Dt24SolarTerms => GetDt24SolarTerms(InputDateTime);

        public Dictionary<int, DateTime> Half24SolarTerms => GetHalf24SolarTerms(InputDateTime, SolarTerms.Terms);
        public Dictionary<int, DateTime> Half24SolarQi => GetHalf24SolarTerms(InputDateTime, SolarTerms.Qi);

        public DataTable DtHalf24SolarTerms => GetDtHalf24SolarTerms(InputDateTime, SolarTerms.Terms);
        public DataTable DtHalf24SolarQi => GetDtHalf24SolarTerms(InputDateTime, SolarTerms.Qi);

        private DateTime InputDateTime { get; set; }

        public Lunar24SolarTerms(DateTime dateTime)
        {
            InputDateTime = dateTime;
        }

        private static DataTable GetDt24SolarTerms(DateTime dateTime)
        {
            return Tools.ResourceToDataTable(Properties.Resources._24solarterms).Select(string.Format("Year = {0} ", dateTime.Year)).CopyToDataTable();
        }

        private Dictionary<int, DateTime> GetHalf24SolarTerms(DateTime dateTime, SolarTerms solarTerms)
        {
            DataTable datatable = GetDt24SolarTerms(dateTime).Select("", "SolarDate ASC").CopyToDataTable();
            Dictionary<int, DateTime> result = new();
            int SolarIndex = 1;
            foreach (DataRow row in datatable.Rows)
            {
                if (SolarIndex % 2 == (solarTerms == SolarTerms.Terms ? 1 : 0))
                {
                    result.Add(((DateTime)row["SolarDate"]).Month, (DateTime)row["SolarDate"]);
                }
                SolarIndex++;
            }
            return result;
        }

        private DataTable GetDtHalf24SolarTerms(DateTime dateTimr, SolarTerms solarTerms)
        {
            DataTable datatable = GetDt24SolarTerms(dateTimr).Select("", "SolarDate ASC").CopyToDataTable();
            DataTable result = datatable.Clone();
            int SolarIndex = 1;
            foreach (DataRow row in datatable.Rows)
            {
                if (SolarIndex % 2 == (solarTerms == SolarTerms.Terms ? 1 : 0))
                {
                    result.ImportRow(row);
                }
                SolarIndex++;
            }
            return result;
        }

        public static string Get24DayMessage(string str24Day)
        {
            string strMsg = str24Day switch
            {
                "立春" => "開始進入春天，萬物復蘇。",
                "雨水" => "這時春風遍吹，天氣漸暖，冰雪溶化，空氣濕潤，雨水增多。",
                "驚蟄" => "天氣轉暖，春雷震響，蟄伏在泥土裏的各種冬眠動物蘇醒過來及開始活動，所以叫驚蟄。大部分地區進入春耕。",
                "春分" => "這一天南北兩半球晝夜相等。大部分地區越冬作物進入春季生長階段。",
                "清明" => "天氣晴朗溫暖，草木始發新枝芽，萬物開始生長，農民忙於春耕春種。人們在門口插上楊柳條，到郊外踏青，以及祭掃墳墓。",
                "穀雨" => "天氣較暖，雨量增加，是北方春耕作物播種的好季節，因為有雨水滋潤大地。",
                "立夏" => "夏天開始，雨水增多，農作物生長漸旺，田間工作日益繁忙。",
                "小滿" => "大麥、冬小麥等夏收作物，已經結果、籽粒飽滿，但尚未成熟，所以叫小滿。",
                "芒種" => "芒種表明小麥等有芒作物成熟，宜開始秋播，如晚穀、黍、稷等。長江中下游地區將進入黃梅季節，連綿陰雨。",
                "夏至" => "陽光直射北回歸線，白天最長。從這一天起，進入炎熱季節，萬物生長最旺盛，雜草害蟲也迅速滋長。",
                "小暑" => "正值初伏前後，天氣很熱但尚未酷熱，忙於夏秋作物的工作。",
                "大暑" => "正值中伏前後，一年最炎熱時期，喜溫作物迅速生長；雨水甚多。",
                "立秋" => "秋天開始，氣溫逐漸下降；中部地區早稻收割，晚稻開始移栽。",
                "處暑" => "氣候變涼的象徵，表示暑天終止，夏季火熱已經到了盡頭。",
                "白露" => "天氣轉涼，地面水汽結露。",
                "秋分" => "陽光直射赤道，晝夜幾乎相等。北方秋收秋種。",
                "寒露" => "天氣轉涼，露水日多。",
                "霜降" => "天氣已冷，開始有霜凍，所以叫霜降。南方仍可秋收秋種。",
                "立冬" => "冬季開始，一年的田間操作結束，作物收割之後要收藏起來。",
                "小雪" => "氣溫下降，黃河流域開始降雪；北方已進入封凍季節。",
                "大雪" => "黃河流域一帶漸有積雪；而北方已是萬里冰封。",
                "冬至" => "這一天，陽光幾乎直射南回歸線，北半球白晝最短，黑夜最長。",
                "小寒" => "開始進入寒冷季節。冷氣積久而寒，大部分地區進入嚴寒時期。",
                "大寒" => "大寒就是天氣寒冷到了極點的意思。大寒前後是一年中最冷的時候。",
                _ => "",
            };
            return strMsg;
        }

        /// <summary>
        /// Craw 24 Solar Terms from web 
        /// </summary>
        /// <param name="intYear"></param>
        /// <returns>DataTable</returns>
        public DataTable Craw24SolarTerms(int intYear)
        {
            string strInputhttp = string.Format("https://www.ximizi.net/24jieqi_1.php?year={0}", intYear);
            string strResult = Tools.GetPage(new Uri(strInputhttp, UriKind.Absolute));
            DataTable dt24SolarTerms = Get24SolarTerms(intYear, strResult);

            return dt24SolarTerms;
        }

        private DataTable Get24SolarTerms(int intYear, string strResult)
        {
            DataTable dt24SolarTerms = new();
            dt24SolarTerms.Columns.Add(new DataColumn() { ColumnName = "ID", DataType = typeof(Int64), AllowDBNull = false, Unique = true });
            dt24SolarTerms.Columns.Add(new DataColumn() { ColumnName = "Year", DataType = typeof(Int16), AllowDBNull = true });
            dt24SolarTerms.Columns.Add(new DataColumn() { ColumnName = "SolarID", DataType = typeof(string), AllowDBNull = true });
            dt24SolarTerms.Columns.Add(new DataColumn() { ColumnName = "SolarName", DataType = typeof(string), AllowDBNull = true });
            dt24SolarTerms.Columns.Add(new DataColumn() { ColumnName = "SolarDate", DataType = typeof(DateTime), AllowDBNull = true });

            Dictionary<string, string> str24solarterms = new();
            str24solarterms.Add("小寒", "SC"); str24solarterms.Add("大寒", "GC");
            str24solarterms.Add("立春", "BS"); str24solarterms.Add("雨水", "RW");
            str24solarterms.Add("驚蟄", "WI"); str24solarterms.Add("春分", "SE");
            str24solarterms.Add("清明", "PB"); str24solarterms.Add("穀雨", "GR");
            str24solarterms.Add("立夏", "BU"); str24solarterms.Add("小滿", "GF");
            str24solarterms.Add("芒種", "GE"); str24solarterms.Add("夏至", "SS");
            str24solarterms.Add("小暑", "SH"); str24solarterms.Add("大暑", "GH");
            str24solarterms.Add("立秋", "GA"); str24solarterms.Add("處暑", "LH");
            str24solarterms.Add("白露", "WD"); str24solarterms.Add("秋分", "AE");
            str24solarterms.Add("寒露", "CD"); str24solarterms.Add("霜降", "FD");
            str24solarterms.Add("立冬", "BW"); str24solarterms.Add("小雪", "SN");
            str24solarterms.Add("大雪", "GN"); str24solarterms.Add("冬至", "WS");

            int index = 1;
            foreach (KeyValuePair<string, string> kvp in str24solarterms)
            {
                strResult = strResult.Substring(strResult.IndexOf(string.Format("<strong>{0}</strong>", kvp.Key), StringComparison.Ordinal));
                strResult = strResult.Substring(strResult.IndexOf("公元", StringComparison.Ordinal));
                string strDateTime = strResult.Substring(0, 19);
                int year = int.Parse(strDateTime.Substring(2, 4));
                int month = int.Parse(strDateTime.Substring(7, 2));
                int day = int.Parse(strDateTime.Substring(10, 2));
                int hour = int.Parse(strDateTime.Substring(14, 2));
                int minute = int.Parse(strDateTime.Substring(17, 2));
                DateTime solarDateTime = new(year, month, day, hour, minute, 0);
                DataRow drdataRow = dt24SolarTerms.NewRow();
                drdataRow["ID"] = string.Format("{0:0000}{1:00}", intYear, index);
                drdataRow["Year"] = intYear;
                drdataRow["SolarID"] = kvp.Value;
                drdataRow["SolarName"] = kvp.Key;
                drdataRow["SolarDate"] = solarDateTime;
                dt24SolarTerms.Rows.Add(drdataRow);
                index++;
            }
            return dt24SolarTerms;
        }
    }
}
