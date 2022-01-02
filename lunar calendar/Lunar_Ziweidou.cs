using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Media;

namespace lunar_calendar
{

    //紫微星系 ID 對照表
    //LN01 本命宮      LN02 父母宮        LN03 福德宮        LN04 田宅宮
    //LN05 官錄宮      LN06 奴僕宮        LN07 遷移宮        LN08 疾厄宮
    //LN09 財帛宮      LN10 子女宮        LN11 夫妻宮        LN12 兄弟宮
    //XX01-XX10 天干  YY01-YY12 地支

    //AY01-AY06 紫微諸星(五形,日)
    //AY01紫微(含廟旺)       AY02天機(含廟旺)     AY03太陽(含廟旺)     AY04武曲(含廟旺)
    //AY05天同(含廟旺)       AY06廉貞(含廟旺)     

    //BY01-BY08 天府諸星(紫微)
    //BY01天府(含廟旺)       BY02太陰(含廟旺)     BY03貪狼(含廟旺)     BY04巨門(含廟旺)
    //BY05天相(含廟旺)       BY06天樑(含廟旺)     BY07七殺(含廟旺)     BY08破軍(含廟旺)

    //CY01-CY08 時系諸星(年干,時支)
    //CY01文昌(含廟旺)       CY02文曲(含廟旺)     CY03火星(含廟旺)     CY04鈴星(含廟旺)
    //CY05地劫(含廟旺)       CY06天空(含廟旺)     CY07台輔(含廟旺)     CY08封誥(含廟旺)

    //DM01-DM09 月系諸星(月)
    //DM01左輔(含廟旺)       DM02右弼(含廟旺)     DM03天刑(含廟旺)     DM04天姚(含廟旺)
    //DM05天馬(含廟旺)       DM06解神(含廟旺)     DM07天巫(含廟旺)     DM08天月(含廟旺)
    //DM09陰煞(含廟旺)

    //EC01-EC04 日系諸星
    //ED01三台(含廟旺)       ED02八座(含廟旺)     EC03恩光(含廟旺)     EC04天貴(含廟旺)

    //FX01-FX11 干系諸星(年干)
    //FX01祿存(含廟旺)       FX02羊刃(含廟旺)     FX03陀羅(含廟旺)     FX04天魁(含廟旺)
    //FX05天鉞(含廟旺)       FX06化祿(含廟旺)     FX07化權(含廟旺)     FX08化科(含廟旺)
    //FX09化忌(含廟旺)       FX10天官(含廟旺)     FX11天福(含廟旺)

    //FY01-FY11 支系諸星(年支)
    //FY01天哭(含廟旺)       FY02天虛(含廟旺)     FY03龍池(含廟旺)     FY04鳳閣(含廟旺)
    //FY05紅鸞(含廟旺)       FY06天喜(含廟旺)     FY07孤辰(含廟旺)     FY08寡宿(含廟旺)
    //FY09蜚廉(含廟旺)       FY10破碎(含廟旺)     FY11天才(含廟旺)     FY12天壽(含廟旺)

    //LN01-LN12 十二宮名
    //LN01 本命宮            LN02 父母宮          LN03 福德宮          LN04 田宅宮
    //LN05 官錄宮            LN06 奴僕宮          LN07 遷移宮          LN08 疾厄宮
    //LN09 財帛宮            LN10 子女宮          LN11 夫妻宮          LN12 兄弟宮

    //ME01-ME12 長生十二神(五形)
    //NF01-NF30 六十花甲(命宮干支)
    //NE02-NE06 五形局(六十花甲)

    //OB01-OB12 博士十二神(祿存)
    //OB01博士(含廟旺)       OB02力士(含廟旺)     OB03青龍(含廟旺)     OB04小耗(含廟旺)
    //OB05將軍(含廟旺)       OB06奏書(含廟旺)     OB07飛廉(含廟旺)     OB08喜神(含廟旺)
    //OB09病符(含廟旺)       OB10大耗(含廟旺)     OB11伏兵(含廟旺)     OB12官府(含廟旺)

    //QR01-QR12 流年將前諸星
    //RR01-RR12 流年歲前諸星
    //SY01      旬空
    //SY02      截空
    //SY03      天傷
    //SY04      天使
    //PY01-PY07 十二宮廟旺地利平閑陷

    [Serializable]
    public class Lunar_Ziweidou
    {
        private Dictionary<string, Dictionary<string, string>> dic12Location;
        private Dictionary<string, StuStar> dicStar;
        private Dictionary<string, Dictionary<string, string>> dic12LocationShow;
        private Dictionary<string, string> dicUpdateData;

        /// <summary>
        /// 眾星
        /// </summary>
        private Dictionary<string, StuStar> DicStar
        {
            get { if (dicStar == null) { dicStar = new Dictionary<string, StuStar>(); } return dicStar; }
            set => dicStar = value;
        }

        /// <summary>
        /// 12 宮位
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> Dic12Location
        {
            get { if (dic12Location == null) { dic12Location = new Dictionary<string, Dictionary<string, string>>(); } return dic12Location; }

            set => dic12Location = value;
        }


        /// <summary>
        /// 12 宮位(顯示用)
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> Dic12LocationShow
        {

            get { if (dic12LocationShow == null) { dic12LocationShow = new Dictionary<string, Dictionary<string, string>>(); } return dic12LocationShow; }
            set => dic12LocationShow = value;
        }


        /// <summary>
        /// 更新模式 資料
        /// </summary>
        public Dictionary<string, string> UpdateData => DicUpdateData;


        private Dictionary<string, string> DicUpdateData
        {
            get { if (dicUpdateData == null) { dicUpdateData = new Dictionary<string, string>(); } return dicUpdateData; }
            set => dicUpdateData = value;
        }

        #region 四柱

        /// <summary>
        /// 四柱年干
        /// </summary>
        private string StrFPYearHS { get; set; }       //四柱年干

        /// <summary>
        /// 四柱年支
        /// </summary>
        private string StrFPYearEB { get; set; }       //四柱年支

        /// <summary>
        /// 四柱月干
        /// </summary>
        private string StrFPMonthHS { get; set; }      //四柱月干

        /// <summary>
        /// 四柱月支
        /// </summary>
        private string StrFPMonthEB { get; set; }      //四柱月支

        /// <summary>
        /// 四柱日干
        /// </summary>
        private string StrFPDayHS { get; set; }        //四柱日干

        /// <summary>
        /// 四柱日支
        /// </summary>
        private string StrFPDayEB { get; set; }        //四柱日支

        /// <summary>
        /// 四柱時干
        /// </summary>
        private string StrFPHourHS { get; set; }       //四柱時干

        /// <summary>
        /// 四柱時支
        /// </summary>
        private string StrFPHourEB { get; set; }       //四柱時支

        #endregion 四柱

        #region 陰曆日期 

        /// <summary>
        /// 陰曆年
        /// </summary>
        private string StrLunarYear { get; set; }        //陰曆年

        /// <summary>
        /// 陰曆月
        /// </summary>
        private string StrLunarMonth { get; set; }       //陰曆月

        /// <summary>
        /// 陰曆日
        /// </summary>
        private string StrLunarDay { get; set; }         //陰曆日

        #endregion 陰曆日期 

        #region 速算日期 
        /// <summary>
        /// 速算年干
        /// </summary>
        private string StrfYearHS { get; set; }      //速算年干

        /// <summary>
        /// 速算年支
        /// </summary>
        private string StrfYearEB { get; set; }      //速算年支

        /// <summary>
        /// 速算月干
        /// </summary>
        private string StrfMonthHS { get; set; }     //速算月干

        /// <summary>
        /// 速算月支
        /// </summary>
        private string StrfMonthEB { get; set; }     //速算月支

        /// <summary>
        /// 速算日干
        /// </summary>
        private string StrfDayHS { get; set; }       //速算日干

        /// <summary>
        /// 速算日支
        /// </summary>
        private string StrfDayEB { get; set; }       //速算日支

        /// <summary>
        /// 速算時干
        /// </summary>
        private string StrfHourHS { get; set; }      //速算時干

        /// <summary>
        /// 速算時支
        /// </summary>
        private string StrfHourEB { get; set; }      //速算時支

        #endregion 速算日期 

        /// <summary>
        /// 性別
        /// </summary>
        private Gender IntGender { get; set; }       //性別

        /// <summary>
        /// 命宮天干
        /// </summary>
        private string StrFateHS { get; set; }       //命宮天干

        /// <summary>
        /// 命宮地支
        /// </summary>
        private string StrFateEB { get; set; }       //命宮地支

        /// <summary>
        /// 命主
        /// </summary>
        private string StrFateStar { get; set; }     //命主

        /// <summary>
        /// 身宮地支
        /// </summary>
        private string StrBodyEB { get; set; }       //身宮地支

        /// <summary>
        /// 身主
        /// </summary>
        private string StrBodyStar { get; set; }     //身主

        /// <summary>
        /// 子年斗君
        /// </summary>
        private string StrYY01Master { get; set; }   //子年斗君    

        /// <summary>
        /// 六十花甲
        /// </summary>
        private string Str60Flower { get; set; }     //六十花甲

        /// <summary>
        /// 五形局
        /// </summary>
        private string Str5Element { get; set; }     //五形局

        /// <summary>
        /// 命重
        /// </summary>
        private double DblWeight { get; set; }       //命重

        public Lunar_Ziweidou(DateTime dateTime, Gender gender = Gender.MalePlus)
        {
            IntGender = gender;

            //找陰曆
            LunarDate lunarDate = new(dateTime);
            StrLunarYear = lunarDate.Year.ToString();
            StrLunarMonth = lunarDate.Month.ToString();
            StrLunarDay = lunarDate.Day.ToString();

            //求命重
            DblWeight = new LunarWeight(dateTime).Weight;

            //找四柱
            Lunar8Characters eightCharacters = new(dateTime);
            StrFPYearHS = eightCharacters.StrYearHS;        //年干
            StrFPYearEB = eightCharacters.StrYearEB;        //年支
            StrFPMonthHS = eightCharacters.StrMonthHS;      //月干
            StrFPMonthEB = eightCharacters.StrMonthEB;      //月支
            StrFPDayHS = eightCharacters.StrDayHS;          //日干
            StrFPDayEB = eightCharacters.StrDayEB;          //日支
            StrFPHourHS = eightCharacters.StrHourHS;        //時干
            StrFPHourEB = eightCharacters.StrHourEB;        //時支

            //找年月日時 干支
            StrfYearHS = lunarDate.StrYearHS;
            StrfYearEB = lunarDate.StrYearEB;
            StrfMonthHS = lunarDate.StrMonthHS;
            StrfMonthEB = lunarDate.StrMonthEB;
            StrfDayHS = lunarDate.StrDayHS;
            StrfDayEB = lunarDate.StrDayEB;
            StrfHourHS = lunarDate.StrHourHS;
            StrfHourEB = lunarDate.StrHourEB;

            //找命宮及身宮
            StrFateEB = $"YY{Tools.CheckRange(3 + lunarDate.Month - lunarDate.HourEB, 1, 12):00}";          //命宮地支
            StrFateStar = GetFateStar(StrFateEB);                                                           //命主

            StrBodyEB = $"YY{Tools.CheckRange(3 + lunarDate.Month + lunarDate.HourEB - 2, 1, 12):00}";      //身宮地支
            StrBodyStar = GetBodyStar(StrfYearEB);                                                          //身主

            //找子年斗君
            StrYY01Master = $"YY{Tools.CheckRange(lunarDate.HourEB - (lunarDate.Month - 1), 1, 12):00}";

            //找12宮
            Get12LocationXY();

            //設00宮(顯示)
            SetLocation00Show(dateTime);

            //找各星座
            SetStar();

            //轉換顯示模式
            ConvertDicShow();

            //轉換成更新資料
            ConvertDicUpdateData(dateTime);
        }

        /// <summary>
        /// 轉換成資料顯示 形式
        /// </summary>
        private void ConvertDicUpdateData(DateTime dateTime)    //轉換成資料顯示 形式
        {
            DicUpdateData.Add("lngDateSN", $"{dateTime.Year}{dateTime.Month:00}{dateTime.Day:00}");
            int intFateY = int.Parse(StrFateEB.Substring(2, 2));
            int intSTP = 1;
            for (int i = intFateY; i <= intFateY + 11; i++)
            {
                int int12Location = Tools.CheckRange(i % 12, 1, 12);
                string[] strStarTemp = new string[Dic12Location[$"{int12Location:00}"].Count];
                int intTemp = 0;
                foreach (var KeyPair in Dic12Location[$"{int12Location:00}"])

                {
                    strStarTemp[intTemp] = KeyPair.Value;
                    intTemp++;
                }
                DicUpdateData.Add($"strp{intSTP:00}", strStarTemp[2]);
                intSTP++;
            }
            DicUpdateData.Add("strp13", DblWeight.ToString());
        }

        /// <summary>
        /// 轉換12宮成顯示形式
        /// </summary>
        private void ConvertDicShow()                           //轉換12宮成顯示形式
        {
            Dictionary<string, string> dicLocationTemp;
            string str12Location, strTemp;
            StuStar stuStarTemp;
            foreach (var KeyPair in DicStar)
            {
                stuStarTemp = DicStar[KeyPair.Key];
                stuStarTemp.StrNameShow = Tools.ConvertNameId(stuStarTemp.StrStarId, ConverOption.IDtoName);
            }
            for (int i = 1; i <= 12; i++)
            {
                str12Location = $"{i:00}";
                foreach (var KeyPair in Dic12Location[str12Location])
                {
                    dicLocationTemp = new Dictionary<string, string>();
                    strTemp = Tools.ConvertNameId(KeyPair.Value, ConverOption.IDtoName);
                    if (Dic12LocationShow.ContainsKey(str12Location))
                    {
                        Dic12LocationShow[str12Location].Add(KeyPair.Key, strTemp);
                    }
                    else
                    {
                        dicLocationTemp.Add(KeyPair.Key, strTemp);
                        Dic12LocationShow.Add(str12Location, dicLocationTemp);
                    }
                }
            }
        }

        /// <summary>
        /// 找陽男,陰男,陽女,陰女
        /// </summary>
        /// <returns></returns>
        private string GetGender()                              //找陽男,陰男,陽女,陰女
        {
            string strGender = "";
            switch (IntGender)
            {
                case Gender.MalePlus:
                    if (int.Parse(StrfYearHS.Substring(2, 2)) % 2 == 1)
                    {
                        IntGender = Gender.MalePlus;
                        strGender = "Male+";
                    }
                    else
                    {
                        IntGender = Gender.MaleMinus;
                        strGender = "Male-";
                    }
                    break;
                case Gender.MaleMinus:
                    IntGender = Gender.MaleMinus;
                    strGender = "Male-";
                    break;
                case Gender.FemalMinuse:
                    if (int.Parse(StrfYearHS.Substring(2, 2)) % 2 == 1)
                    {
                        IntGender = Gender.FemalePlus;
                        strGender = "Female+";
                    }
                    else
                    {
                        IntGender = Gender.FemalMinuse;
                        strGender = "Female-";
                    }
                    break;
                case Gender.FemalePlus:
                    IntGender = Gender.FemalePlus;
                    strGender = "Female+";
                    break;
                case Gender.None:
                    break;
            }
            return strGender;
        }

        /// <summary>
        /// 找12宮
        /// </summary>
        private void Get12LocationXY()                          //找12宮
        {
            int intYinX = Tools.CheckRange(int.Parse(StrfYearHS.Substring(2, 2)) % 5, 1, 5);
            intYinX = (intYinX * 2 + 1) % 10;                           //寅宮天干
            #region 找命宮 天干
            int intFateDistY = Convert.ToInt16(StrFateEB.Substring(2, 2)) - 3;   //命宮到寅的距離
            if (intFateDistY < 0) intFateDistY += 12;
            int intFateX = Tools.CheckRange((intYinX + intFateDistY) % 10, 1, 10);
            StrFateHS = $"XX{intFateX:00}";   //命宮天干
            Str60Flower = new Lunar60Flower(intFateX, intFateDistY).StrFlower; //找六十花甲
            Str5Element = new Lunar60Flower(intFateX, intFateDistY).StrElement5;//找五形局
            #endregion 找命宮 天干

            #region 定義 十二宮名
            int intLocationX, intLocationY, int12Location;
            string L12XY, L12Name;
            for (int i = 3; i <= 14; i++)                                                       //寅宮開始
            {
                Dictionary<string, string> dic12Temp = new();
                intLocationX = Tools.CheckRange((intYinX + (i - 3)) % 10, 1, 10);               //十二宮 天干
                intLocationY = Tools.CheckRange(i % 12, 1, 12);                                 //十二宮 地支
                int12Location = Tools.CheckRange((i - intFateDistY + 10) % 12, 1, 12);            //十二宮 宮名
                L12XY = $"XX{intLocationX:00}YY{intLocationY:00}";
                dic12Temp.Add(L12XY, L12XY);
                L12Name = $"LN{int12Location:00}";
                dic12Temp.Add(L12Name, L12Name);

                Dic12Location.Add($"{intLocationY:00}", dic12Temp);
                StuStar stuStarTemp = new()
                {
                    StrLocate = $"YY{intLocationY:00}",
                    StrStarId = dic12Temp[L12Name],
                    BrushColor = Brushes.LightSalmon
                };
                DicStar.Add(stuStarTemp.StrStarId, stuStarTemp);
            }
            #endregion 定義 十二宮名
        }

        /// <summary>
        /// 設00宮(顯示)
        /// </summary>
        private void SetLocation00Show(DateTime dateTime)       //設00宮(顯示)
        {
            Dictionary<string, string> dic12Temp;
            dic12Temp = new Dictionary<string, string>
            {
                { "strGender", $"性別：{Tools.ConvertNameId(GetGender(), ConverOption.IDtoName)}" },        //性別
                {
                    "strWDate",
                    $"陽曆： {dateTime.Year,4} 年 {dateTime.Month} 月 {dateTime.Day} 日 {dateTime.Hour} 時 {DateTimeFormatInfo.CurrentInfo.DayNames[(byte)dateTime.DayOfWeek]}"
                },
                {
                    "strCDate",
                    $"陰曆： {StrLunarYear,5} 年 {StrLunarMonth} 月 {StrLunarDay} 日 {Tools.ConvertNameId(StrFPHourEB, ConverOption.IDtoName)} 時"
                },
                { "str5Element", $"五形局： {Tools.ConvertNameId(Str60Flower, ConverOption.IDtoName)}  " },
                { "strYY01Star", $"子年斗君： {Tools.ConvertNameId(StrYY01Master, ConverOption.IDtoName)}  " },
                {
                    "strFateY1", $"命宮：{Tools.ConvertNameId(StrFateEB, ConverOption.IDtoName),5}      身宮： {Tools.ConvertNameId(StrBodyEB, ConverOption.IDtoName)}"
                },
                {
                    "strFateStar", $"命主：{Tools.ConvertNameId(StrFateStar,ConverOption.IDtoName),5}      身主： {Tools.ConvertNameId(StrBodyStar, ConverOption.IDtoName)}"
                },
                { "strWeight", $"命重：{DblWeight}" },
                {
                    "strf8Col",
                    $"干支速算： [{Tools.ConvertNameId(StrfYearHS, ConverOption.IDtoName)}{Tools.ConvertNameId(StrfYearEB, ConverOption.IDtoName)}] 年" +
                    $" [{Tools.ConvertNameId(StrfMonthHS, ConverOption.IDtoName)}{Tools.ConvertNameId(StrfMonthEB, ConverOption.IDtoName)}] 月" +
                    $" [{Tools.ConvertNameId(StrfDayHS, ConverOption.IDtoName)}{Tools.ConvertNameId(StrfDayEB, ConverOption.IDtoName)}] 日" +
                    $" [{Tools.ConvertNameId(StrfHourHS, ConverOption.IDtoName)}{Tools.ConvertNameId(StrfHourEB, ConverOption.IDtoName)}] 時"
                },
                {
                    "str8Col",
                    $"四　　柱： [{Tools.ConvertNameId(StrFPYearHS, ConverOption.IDtoName)}{Tools.ConvertNameId(StrFPYearEB, ConverOption.IDtoName)}] 年" +
                    $" [{Tools.ConvertNameId(StrFPMonthHS, ConverOption.IDtoName)}{Tools.ConvertNameId(StrFPMonthEB, ConverOption.IDtoName)}] 月" +
                    $" [{Tools.ConvertNameId(StrFPDayHS, ConverOption.IDtoName)}{Tools.ConvertNameId(StrFPDayEB, ConverOption.IDtoName)}] 日" +
                    $" [{Tools.ConvertNameId(StrFPHourHS, ConverOption.IDtoName)}{Tools.ConvertNameId(StrFPHourEB, ConverOption.IDtoName)}] 時"
                }
            };   //00宮
            Dic12LocationShow.Add("00", dic12Temp);
        }

        /// <summary>
        /// 設眾星位置
        /// </summary>
        private void SetStar()                                  //設眾星位置
        {
            //紫微AY01所在宮位
            SetStarData(Str5Element, StrLunarDay, "AY01", "AY01", Brushes.Yellow);
            //天機AY02所在宮位
            SetStarData("AY02", DicStar["AY01"].StrLocate, "AY02", "AY02", Brushes.Yellow);
            //太陽AY03所在宮位
            SetStarData("AY03", DicStar["AY01"].StrLocate, "AY03", "AY03", Brushes.Yellow);
            //武曲AY04所在宮位
            SetStarData("AY04", DicStar["AY01"].StrLocate, "AY04", "AY04", Brushes.Yellow);
            //天同AY05所在宮位
            SetStarData("AY05", DicStar["AY01"].StrLocate, "AY05", "AY05", Brushes.Yellow);
            //廉貞AY06所在宮位
            SetStarData("AY06", DicStar["AY01"].StrLocate, "AY06", "AY06", Brushes.Yellow);
            //天府BY01所在宮位
            SetStarData("BY01", DicStar["AY01"].StrLocate, "BY01", "BY01", Brushes.Blue);
            //太陰BY02所在宮位
            SetStarData("BY02", DicStar["BY01"].StrLocate, "BY02", "BY02", Brushes.Blue);
            //貪狼BY03所在宮位
            SetStarData("BY03", DicStar["BY01"].StrLocate, "BY03", "BY03", Brushes.Blue);
            //巨門BY04所在宮位
            SetStarData("BY04", DicStar["BY01"].StrLocate, "BY04", "BY04", Brushes.Blue);
            //天相BY05所在宮位
            SetStarData("BY05", DicStar["BY01"].StrLocate, "BY05", "BY05", Brushes.Blue);
            //天樑BY06所在宮位
            SetStarData("BY06", DicStar["BY01"].StrLocate, "BY06", "BY06", Brushes.Blue);
            //七殺BY07所在宮位
            SetStarData("BY07", DicStar["BY01"].StrLocate, "BY07", "BY07", Brushes.Blue);
            //破軍BY08所在宮位
            SetStarData("BY08", DicStar["BY01"].StrLocate, "BY08", "BY08", Brushes.Blue);
            //文昌CY01所在宮位
            SetStarData("CY01", StrfHourEB, "CY01", "CY01", Brushes.LightGreen);
            //文曲CY02所在宮位
            SetStarData("CY02", StrfHourEB, "CY02", "CY02", Brushes.LightGreen);
            //火星CY03所在宮位
            SetStarData("CY03", StrfYearEB + StrfHourEB, "CY03", "CY03", Brushes.LightGreen);
            //鈴星CY04所在宮位
            SetStarData("CY04", StrfYearEB + StrfHourEB, "CY04", "CY04", Brushes.LightGreen);
            //地劫CY05所在宮位
            SetStarData("CY05", StrfHourEB, "CY05", "CY05", Brushes.LightGreen);
            //天空CY06所在宮位
            SetStarData("CY06", StrfHourEB, "CY06", "CY06", Brushes.LightGreen);
            //台輔CY07所在宮位
            SetStarData("CY07", StrfHourEB, "CY07", "CY07", Brushes.LightGreen);
            //封誥CY08所在宮位
            SetStarData("CY08", StrfHourEB, "CY08", "CY08", Brushes.LightGreen);
            //左輔DM01所在宮位
            SetStarData("DM01", StrLunarMonth, "DM01", "DM01", Brushes.LightPink);
            //右弼DM02所在宮位
            SetStarData("DM02", StrLunarMonth, "DM02", "DM02", Brushes.LightPink);
            //天刑DM03所在宮位
            SetStarData("DM03", StrLunarMonth, "DM03", "DM03", Brushes.LightPink);
            //天姚DM04所在宮位
            SetStarData("DM04", StrLunarMonth, "DM04", "DM04", Brushes.LightPink);
            //天馬DM05所在宮位
            SetStarData("DM05", StrLunarMonth, "DM05", "DM05", Brushes.LightPink);
            //解神DM06所在宮位
            SetStarData("DM06", StrLunarMonth, "DM06", "DM06", Brushes.LightPink);
            //天巫DM07所在宮位
            SetStarData("DM07", StrLunarMonth, "DM07", "DM07", Brushes.LightPink);
            //天月DM08所在宮位
            SetStarData("DM08", StrLunarMonth, "DM08", "DM08", Brushes.LightPink);
            //陰煞DM09所在宮位
            SetStarData("DM09", StrLunarMonth, "DM09", "DM09", Brushes.LightPink);
            //三台ED01所在宮位
            StuStar stuStarTemp = new();
            int intStarLocation = Tools.CheckRange((int.Parse(DicStar["DM01"].StrLocate.Substring(2, 2)) + (int.Parse(StrLunarDay) - 1)) % 12, 1, 12);
            stuStarTemp.StrLocate = $"YY{intStarLocation:00}";
            stuStarTemp.StrStarId = GetStarStrongWeak($"{"ED01"}{stuStarTemp.StrLocate}");
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCoral;
            UpdateStarInfo("ED01", "ED01", stuStarTemp);

            //三台ED02所在宮位
            stuStarTemp = new StuStar();
            intStarLocation = (int.Parse(DicStar["DM02"].StrLocate.Substring(2, 2)) - (int.Parse(StrLunarDay) - 1)) % 12;
            intStarLocation = Tools.CheckRange(intStarLocation, 1, 12);
            stuStarTemp.StrLocate = $"YY{intStarLocation:00}";
            stuStarTemp.StrStarId = GetStarStrongWeak($"{"ED02"}{stuStarTemp.StrLocate}");
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCoral;
            UpdateStarInfo("ED02", "ED02", stuStarTemp);

            //恩光EC03所在宮位
            stuStarTemp = new StuStar();
            intStarLocation = (int.Parse(DicStar["CY01"].StrLocate.Substring(startIndex: 2, length: 2)) + (int.Parse(StrLunarDay) - 2)) % 12;
            intStarLocation = Tools.CheckRange(intStarLocation, 1, 12);
            stuStarTemp.StrLocate = $"YY{intStarLocation:00}";
            stuStarTemp.StrStarId = GetStarStrongWeak($"{"EC03"}{stuStarTemp.StrLocate}");
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCoral;
            UpdateStarInfo("EC03", "EC03", stuStarTemp);

            //天貴EC04所在宮位
            stuStarTemp = new StuStar();
            intStarLocation = (int.Parse(DicStar["CY02"].StrLocate.Substring(startIndex: 2, length: 2)) + (int.Parse(StrLunarDay) - 2)) % 12;
            intStarLocation = Tools.CheckRange(intStarLocation, 1, 12);
            stuStarTemp.StrLocate = $"YY{intStarLocation:00}";
            stuStarTemp.StrStarId = GetStarStrongWeak($"{"EC04"}{stuStarTemp.StrLocate}");
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCoral;
            UpdateStarInfo("EC04", "EC04", stuStarTemp);
            //祿存FX01所在宮位
            SetStarData("FX01", StrfYearHS, "FX01", "FX01", Brushes.LightCyan);
            //羊刃FX02所在宮位
            SetStarData("FX02", StrfYearHS, "FX02", "FX02", Brushes.LightCyan);
            //陀羅FX03所在宮位
            SetStarData("FX03", StrfYearHS, "FX03", "FX03", Brushes.LightCyan);
            //天魁FX04所在宮位
            SetStarData("FX04", StrfYearHS, "FX04", "FX04", Brushes.LightCyan);
            //天鉞FX05所在宮位
            SetStarData("FX05", StrfYearHS, "FX05", "FX05", Brushes.LightCyan);

            //化祿FX06所在宮位
            string strInput, strOutput;
            strInput = $"FX06{StrFPYearHS}";
            strOutput = GetStarLocation(strInput);
            stuStarTemp = DicStar[strOutput];
            stuStarTemp.StrStarId += "FX06";
            stuStarTemp.BrushColor = Brushes.LightCyan;
            Dic12Location[stuStarTemp.StrLocate.Substring(2, 2)][strOutput] = stuStarTemp.StrStarId;

            //化權FX07所在宮位
            strInput = $"FX07{StrFPYearHS}";
            strOutput = GetStarLocation(strInput);
            stuStarTemp = DicStar[strOutput];
            stuStarTemp.StrStarId += "FX07";
            stuStarTemp.BrushColor = Brushes.LightCyan;
            Dic12Location[stuStarTemp.StrLocate.Substring(2, 2)][strOutput] = stuStarTemp.StrStarId;

            //化科FX08所在宮位
            strInput = $"FX08{StrFPYearHS}";
            strOutput = GetStarLocation(strInput);
            stuStarTemp = DicStar[key: strOutput];
            stuStarTemp.StrStarId += "FX08";
            stuStarTemp.BrushColor = Brushes.LightCyan;
            Dic12Location[stuStarTemp.StrLocate.Substring(2, 2)][strOutput] = stuStarTemp.StrStarId;

            //化忌FX09所在宮位
            strInput = $"FX09{StrFPYearHS}";
            strOutput = GetStarLocation(strInput);
            stuStarTemp = DicStar[strOutput];
            stuStarTemp.StrStarId += "FX09";
            stuStarTemp.BrushColor = Brushes.LightCyan;
            Dic12Location[stuStarTemp.StrLocate.Substring(2, 2)][strOutput] = stuStarTemp.StrStarId;

            //天官FX10所在宮位
            SetStarData("FX10", StrfYearHS, "FX10", "FX10", Brushes.LightCyan);

            //天福FX11所在宮位
            SetStarData("FX11", StrfYearHS, "FX11", "FX11", Brushes.LightCyan);

            //博士OB01所在宮位
            stuStarTemp = new StuStar()
            {
                StrLocate = DicStar["FX01"].StrLocate,
                StrStarId = GetStarStrongWeak($"OB01{stuStarTemp.StrLocate}"),
                BrushColor = Brushes.LightGray
            };
            UpdateStarInfo("OB01", "OB01", stuStarTemp);

            //OB02-OB12所在宮位
            int intGenderTemp = 1;
            switch (IntGender)
            {
                case Gender.MalePlus:
                    intGenderTemp = 1;
                    break;
                case Gender.MaleMinus:
                    intGenderTemp = -1;
                    break;
                case Gender.FemalMinuse:
                    intGenderTemp = 1;
                    break;
                case Gender.FemalePlus:
                    intGenderTemp = -1;
                    break;
                case Gender.None:
                    break;
                default:
                    break;
            }
            for (int i = 1; i <= 11; i++)
            {
                string strStartName = $"OB{i + 1:00}";
                int intStarLocationTemp = Tools.CheckRange((int.Parse(DicStar["FX01"].StrLocate.Substring(2, 2)) + i * intGenderTemp) % 12, 1, 12);
                stuStarTemp = new StuStar()
                {
                    StrLocate = $"YY{intStarLocationTemp:00}",
                    StrStarId = GetStarStrongWeak($"{strStartName}{stuStarTemp.StrLocate}"),
                    BrushColor = Brushes.LightGray
                };
                UpdateStarInfo(strStartName, strStartName, stuStarTemp);
            }

            //天哭FY01所在宮位
            SetStarData("FY01", StrfYearEB, "FY01", "FY01", Brushes.LightSeaGreen);
            //天虛FY02所在宮位
            SetStarData("FY02", StrfYearEB, "FY02", "FY02", Brushes.LightSeaGreen);
            //龍池FY03所在宮位
            SetStarData("FY03", StrfYearEB, "FY03", "FY03", Brushes.LightSeaGreen);
            //鳳閣FY04所在宮位
            SetStarData("FY04", StrfYearEB, "FY04", "FY04", Brushes.LightSeaGreen);
            //紅鸞FY05所在宮位
            SetStarData("FY05", StrfYearEB, "FY05", "FY05", Brushes.LightSeaGreen);
            //天喜FY06所在宮位
            SetStarData("FY06", StrfYearEB, "FY06", "FY06", Brushes.LightSeaGreen);
            //孤辰FY07所在宮位
            SetStarData("FY07", StrfYearEB, "FY07", "FY07", Brushes.LightSeaGreen);
            //寡宿FY08所在宮位
            SetStarData("FY08", StrfYearEB, "FY08", "FY08", Brushes.LightSeaGreen);
            //蜚廉FY09所在宮位
            SetStarData("FY09", StrfYearEB, "FY09", "FY09", Brushes.LightSeaGreen);
            //破碎FY10所在宮位
            SetStarData("FY10", StrfYearEB, "FY10", "FY10", Brushes.LightSeaGreen);
            //天才FY11所在宮位
            stuStarTemp = new StuStar
            {
                StrLocate = DicStar[GetStarLocation($"FY11{StrfYearEB}")].StrLocate
            };

            stuStarTemp.StrStarId = GetStarStrongWeak($"FY11{stuStarTemp.StrLocate}");
            stuStarTemp.BrushColor = Brushes.LightSeaGreen;

            UpdateStarInfo("FY11", "FY11", stuStarTemp);

        }

        /// <summary>
        /// 設定各星座值
        /// </summary>
        /// <param name="strInput01">strID argument1</param>
        /// <param name="strInput02">strID argument2</param>
        /// <param name="strStarName">星座名稱(strStarID)</param>
        /// <param name="strStarLevel">星座位階</param>
        private void SetStarData(string strInput01, string strInput02, string strStarName, string strStarLevel, Brush brushColor) // 設定各星座值
        {
            string strInput = string.Format("{0}{1}", strInput01, strInput02);
            StuStar stuStarTemp = new()
            {
                StrLocate = GetStarLocation(strInput),
                BrushColor = brushColor
            };
            stuStarTemp.StrStarId = GetStarStrongWeak($"{strStarName}{stuStarTemp.StrLocate}");
            //            stuStarTemp.StrStarId = DicStarStrongWeak[key: string.Format(provider: CultureInfo.InvariantCulture, format: "{0}{1}", arg0: strStarName, arg1: stuStarTemp.StrLocate)];
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            UpdateStarInfo(strStarLevel, strStarName, stuStarTemp);
        }

        /// <summary>
        /// 更新星座資料,十二宮資料
        /// </summary>
        /// <param name="strStarLevel">星座位階</param>
        /// <param name="strStarName">星座名稱(strStarID)</param>
        /// <param name="stuStarInput">星座結構(stuStar)</param>
        private void UpdateStarInfo(string strStarLevel, string strStarName, StuStar stuStarInput) // 更新星座資料,十二宮資料
        {
            DicStar.Add(strStarName, stuStarInput);
            Dic12Location[stuStarInput.StrLocate.Substring(2, 2)].Add(strStarLevel, stuStarInput.StrStarId);
        }


        /// <summary>
        /// 找命主
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private static string GetFateStar(string strInput)
        {
            Dictionary<string, string> result = new()
            {
                { "YY01", "BY03" },
                { "YY02", "BY04" },
                { "YY03", "FX01" },
                { "YY04", "CY02" },
                { "YY05", "AY06" },
                { "YY06", "AY04" },
                { "YY07", "BY08" },
                { "YY08", "AY04" },
                { "YY09", "AY06" },
                { "YY10", "CY02" },
                { "YY11", "FX01" },
                { "YY12", "BY04" }
            };
            return result.ContainsKey(strInput) ? result[strInput] : strInput;
        }

        /// <summary>
        /// 找身主
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private static string GetBodyStar(string strInput)
        {
            Dictionary<string, string> result = new()
            {
                { "YY01", "CY03" },
                { "YY02", "BY05" },
                { "YY03", "BY06" },
                { "YY04", "AY05" },
                { "YY05", "CY01" },
                { "YY06", "AY02" },
                { "YY07", "CY03" },
                { "YY08", "BY05" },
                { "YY09", "BY06" },
                { "YY10", "AY05" },
                { "YY11", "CY01" },
                { "YY12", "AY02" }
            };
            return result.ContainsKey(strInput) ? result[strInput] : strInput;
        }

        private static string GetStarStrongWeak(string strStarYY)
        {
            Dictionary<string, string> dicOutput = new();

            using DataTable dtDataTable = Tools.ResourceToDataTable(Properties.Resources.StarStrongWeak);
            foreach (DataRow drRow in dtDataTable.Rows)
            {
                dicOutput.Add(drRow["strStarYY"].ToString(), drRow["strStarPY"].ToString());
            }

            return dicOutput.ContainsKey(strStarYY) ? dicOutput[strStarYY] : strStarYY;
        }

        private static string GetStarLocation(string strID)
        {
            Dictionary<string, string> dicOutput = new();

            using DataTable dtDataTable = Tools.ResourceToDataTable(Properties.Resources.StarLocation);
            foreach (DataRow drRow in dtDataTable.Rows)
            {
                dicOutput.Add(drRow["strID"].ToString(), drRow["strLocationID"].ToString());
            }

            return dicOutput.ContainsKey(strID) ? dicOutput[strID] : strID;
        }

    }

    [Serializable]
    /// <summary>
    /// 星座結構(stuStar)
    /// </summary>
    internal struct StuStar
    {
        internal string StrLocate { get; set; }       //所在宮名
        internal string StrStarId { get; set; }       //星ID
        internal string StrNameShow { get; set; }     //星名顯示
        internal Brush BrushColor { get; set; }       //背景顏色

    }

}
