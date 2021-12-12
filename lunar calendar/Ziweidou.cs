using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Media;

namespace lunar_calendar
{
    [Serializable]
    public partial class Ziweidou
    {
        /// <summary>
        /// Get All Ziweidou Number from tblDateAllOptions
        /// </summary>
        /// <param name="dtInputDate"></param>
        /// <returns></returns>
        public DataTable GetPurple(DataTable dtInputDate)
        {
            if (dtInputDate == null) { throw new ArgumentNullException(nameof(dtInputDate)); }

            using DataTable dtOutputData = new("dtOutputData");

            #region Set Columns
            using (DataColumn dcColumn = new())
            {
                dcColumn.ColumnName = "lngDateSN";
                dcColumn.DataType = typeof(int);
                dtOutputData.Columns.Add(dcColumn);
            }
            for (int i = 1; i <= 13; i++)
            {
                dtOutputData.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(CultureInfo.InvariantCulture, "strp{0:d2}", i),
                    DataType = typeof(string)
                });
            }
            #endregion Set Columns
            //int intIndexTemp = 1;
            //int intIndexTotal = dtInputDate.Rows.Count;
            foreach (DataRow drRow in dtInputDate.Rows)
            {
                Ziweidou stuTemp = new()
                {
                    StrWYear = drRow[0].ToString().Substring(0, 4),
                    StrWMonth = drRow[0].ToString().Substring(4, 2),
                    StrWDay = drRow[0].ToString().Substring(6, 2),
                    StrWHour = "YY11",
                    IntGender = Lunar.Gender.MalePlus
                };
                stuTemp.Init();
                DataRow drRrowNew = dtOutputData.NewRow();
                foreach (KeyValuePair<string, string> KeyPair in stuTemp.DicUpdateData)
                {
                    drRrowNew[KeyPair.Key] = KeyPair.Value;
                }
                dtOutputData.Rows.Add(drRrowNew);
            }
            return dtOutputData;
        }


    }

    public partial class Ziweidou
    {
        private static CultureInfo InvariantCulture => CultureInfo.InvariantCulture;

        private Dictionary<string, Dictionary<string, string>> dic12Location;
        private Dictionary<string, Dictionary<string, string>> dic12LocationShow;
        private Dictionary<string, StuStar> dicStar;
        private Dictionary<string, string> dicUpdateData;

        //private Dictionary<string, string> CIDToName { get; set; }  //ID名稱轉換表

        public void Init()
        {
            if (DicWeight == null) DicWeight = new LifeWeight().TblWeight();
            if (DicStarLocation == null) DicStarLocation = TblStarLocation();
            if (DicStarStrength == null) DicStarStrength = TblStarStrongWeak();
            //if (this.CIDToName == null) this.CIDToName = CNameID(1);
            GetLunarDate();          //找陰曆
            Get4Col();              //找四柱
            GetFast4Col();          //找年月日時 干支
            GetFateBodyLocation();  //找命宮及身宮
            DblWeight = GetWeight();//求命重
            GetYY01Master();        //找子年斗君
            Get12LocationXY();      //找12宮
            SetLocation00Show();    //設00宮(顯示)
            SetStar();              //找各星座
            ConvertDicShow();       //轉換顯示模式
            ConvertDicUpdateData(); //轉換成更新資料
        }



        private Dictionary<string, string> TblStarStrongWeak()
        {
            Dictionary<string, string> dicOutput = new();

            using SqlCommand sqlCommand = new()
            {
                Connection = new SqlConnection(new CglDBData().SetDataBase(TargetTable.DataPurple, DatabaseType.Data)),
                CommandText = "SELECT * FROM [tblStarStrongWeak] "
            };
            using DataTable dtDataTable = new CglFunc().GetDataTable(sqlCommand, "Options");
            foreach (DataRow drRow in dtDataTable.Rows)
            {
                dicOutput.Add(drRow["strStarYY"].ToString(), drRow["strStarPY"].ToString());
            }
            return dicOutput;
        }

        private Dictionary<string, string> TblStarLocation()
        {
            Dictionary<string, string> dicOutput = new();

            using SqlCommand sqlCommand = new()
            {
                Connection = new SqlConnection(new CglDBData().SetDataBase(TargetTable.DataPurple, DatabaseType.Data)),
                CommandText = "SELECT * FROM [tblStarLocation] "
            };

            using DataTable dtDataTable = new CglFunc().GetDataTable(sqlCommand, "Options");

            foreach (DataRow drRow in dtDataTable.Rows)
            {
                dicOutput.Add(drRow["strID"].ToString(), drRow["strLocationID"].ToString());
            }
            return dicOutput;
        }



        /// <summary>
        /// 轉換成資料顯示 形式
        /// </summary>
        private void ConvertDicUpdateData()                 //轉換成資料顯示 形式
        {
            DicUpdateData.Add("lngDateSN", string.Format(InvariantCulture, "{0}{1}{2}", StrWYear, StrWMonth, StrWDay));
            int intFateY = Convert.ToInt16(StrFateY1.Substring(2, 2), InvariantCulture);
            int intSTP = 1;
            for (int i = intFateY; i <= intFateY + 11; i++)
            {
                int int12Location = i % 12;
                if (int12Location <= 0)
                    int12Location += 12;
                string[] strStarTemp = new string[Dic12Location[string.Format(InvariantCulture, "{0:d2}", int12Location)].Count];
                int intTemp = 0;
                foreach (var KeyPair in Dic12Location[string.Format(InvariantCulture, "{0:d2}", int12Location)])

                {
                    strStarTemp[intTemp] = KeyPair.Value;
                    intTemp++;
                }
                DicUpdateData.Add(string.Format(InvariantCulture, "strp{0:d2}", intSTP), strStarTemp[2]);
                intSTP++;
            }
            DicUpdateData.Add("strp13", DblWeight.ToString(InvariantCulture));
        }

        /// <summary>
        /// 轉換12宮成顯示形式
        /// </summary>
        private void ConvertDicShow()                       //轉換12宮成顯示形式
        {
            Dictionary<string, string> dicLocationTemp;
            string str12Location, strTemp;
            StuStar stuStarTemp;
            foreach (var KeyPair in DicStar)
            {
                stuStarTemp = DicStar[KeyPair.Key];
                stuStarTemp.StrNameShow = new CglFunc().ConvertNameId(stuStarTemp.StrStarId, 1);
            }
            for (int i = 1; i <= 12; i++)
            {
                str12Location = string.Format(InvariantCulture, "{0:d2}", i);
                foreach (var KeyPair in Dic12Location[str12Location])
                {
                    dicLocationTemp = new Dictionary<string, string>();
                    strTemp = new CglFunc().ConvertNameId(KeyPair.Value, 1);
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
        /// 求命重
        /// </summary>
        /// <returns></returns>
        private double GetWeight()                          //求命重
        {
            if (DicWeight == null) DicWeight = new LifeWeight().TblWeight();
            double dblWeight = DicWeight[string.Format(InvariantCulture, "{0}{1}", StrfYearX1, StrfYearY1)] +
                               DicWeight[string.Format(InvariantCulture, "M{0}", StrCMonth)] +
                               DicWeight[string.Format(InvariantCulture, "D{0}", StrCDay)] +
                               DicWeight[string.Format(InvariantCulture, "{0}", StrHourY1)];
            return dblWeight;
        }

        /// <summary>
        /// 找陽男,陰男,陽女,陰女
        /// </summary>
        /// <returns></returns>
        private string GetGender()                          //找陽男,陰男,陽女,陰女
        {
            string strGender = "";
            switch (IntGender)
            {
                case Lunar.Gender.MalePlus:
                    if (Convert.ToInt16(StrfYearX1.Substring(2, 2), InvariantCulture) % 2 == 1)
                    {
                        IntGender = Lunar.Gender.MalePlus;
                        strGender = "Male+";
                    }
                    else
                    {
                        IntGender = Lunar.Gender.MaleMinus;
                        strGender = "Male-";
                    }
                    break;
                case Lunar.Gender.MaleMinus:
                    IntGender = Lunar.Gender.MaleMinus;
                    strGender = "Male-";
                    break;
                case Lunar.Gender.FemalMinuse:
                    if (Convert.ToInt16(StrfYearX1.Substring(2, 2), InvariantCulture) % 2 == 1)
                    {
                        IntGender = Lunar.Gender.FemalePlus;
                        strGender = "Female+";
                    }
                    else
                    {
                        IntGender = Lunar.Gender.FemalMinuse;
                        strGender = "Female-";
                    }
                    break;
                case Lunar.Gender.FemalePlus:
                    IntGender = Lunar.Gender.FemalePlus;
                    strGender = "Female+";
                    break;
                case Lunar.Gender.None:
                    break;
            }
            return strGender;
        }

        /// <summary>
        /// 轉換陰曆
        /// </summary>
        private void GetLunarDate()                             //找陰曆
        {
            DateTime dtDateTime = new(int.Parse(StrWYear, InvariantCulture), int.Parse(StrWMonth, InvariantCulture), int.Parse(StrWDay, InvariantCulture));
            DowWeekend = dtDateTime.DayOfWeek;
            Dictionary<string, string> dicLubarDate = new Lunar().GetLunarDate(dtDateTime);
            StrCYear = dicLubarDate["StrCYear"];
            StrCMonth = dicLubarDate["StrCMonth"];
            StrCDay = dicLubarDate["StrCDay"];
        }

        /// <summary>
        /// 找四柱
        /// </summary>
        private void Get4Col()                              //找四柱
        {
            //CglDataSet GLDataSet = new CglDataSet(TargetTable.DataOption);

            using SqlCommand sqlCommand = new()
            {
                Connection = new SqlConnection(new CglDBData().SetDataBase(TargetTable.DataPurple, DatabaseType.Data)),
                CommandText = "SELECT * FROM [tblDateAllOptions] WHERE [lngDateSN] = @lngDateSN "
            };
            sqlCommand.Parameters.AddWithValue("lngDateSN", string.Format(InvariantCulture, "{0}{1}{2}", StrWYear, StrWMonth, StrWDay));
            using DataTable dtDataTable = new CglFunc().GetDataTable(sqlCommand, "Options");
            StrYearX1 = dtDataTable.Rows[0]["strYearT1"].ToString();       //年干
            StrYearY1 = dtDataTable.Rows[0]["strYearT2"].ToString();       //年支
            StrMonthX1 = dtDataTable.Rows[0]["strMonthT1"].ToString();     //月干
            StrMonthY1 = dtDataTable.Rows[0]["strMonthT2"].ToString();     //月支
            StrDayX1 = dtDataTable.Rows[0]["strDayT1"].ToString();         //日干
            StrDayY1 = dtDataTable.Rows[0]["strDayT2"].ToString();         //日支
            StrHourY1 = StrWHour;                                         //時支
            int intHour = (Convert.ToInt16(StrDayX1.Substring(2, 2), InvariantCulture) * 2 + Convert.ToInt16(StrHourY1.Substring(2, 2), InvariantCulture) - 2) % 10;
            StrHourX1 = string.Format(InvariantCulture, "XX{0:d2}", intHour);        //時干
        }

        /// <summary>
        /// 找年月日時 干支
        /// </summary>
        private void GetFast4Col()                          //找年月日時 干支
        {
            int intYear = (Convert.ToInt16(StrCYear, InvariantCulture) + 1911) % 10 - 3;
            if (intYear <= 0)
                intYear += 10;
            StrfYearX1 = string.Format(InvariantCulture, "XX{0:d2}", intYear);   //年干
            intYear = (Convert.ToInt16(StrCYear, InvariantCulture) + 1911 - 3) % 12;
            if (intYear == 0)
                intYear += 12;
            StrfYearY1 = string.Format(InvariantCulture, "YY{0:d2}", intYear);   //年支
            int intMonth = (Convert.ToInt16(StrfYearX1.Substring(2, 2), InvariantCulture) * 2 + Convert.ToInt16(StrCMonth, InvariantCulture)) % 10;
            if (intMonth <= 0)
                intMonth += 10;
            StrfMonthX1 = string.Format(InvariantCulture, "XX{0:d2}", intMonth); //月干
            intMonth = (Convert.ToInt16(StrCMonth, InvariantCulture) + 2) % 12;
            if (intMonth <= 0)
                intMonth += 12;
            StrfMonthY1 = string.Format(InvariantCulture, "YY{0:d2}", intMonth); //月支
            StrfDayX1 = StrDayX1;                                          //日干
            StrfDayY1 = StrDayY1;                                          //日支
            StrfHourX1 = StrHourX1;                                   //時干
            StrfHourY1 = StrWHour;                                    //時支
        }

        /// <summary>
        /// 找命宮及身宮
        /// </summary>
        private void GetFateBodyLocation()                  //找命宮及身宮
        {
            int intTemp = 3 - Convert.ToInt16(StrWHour.Substring(2, 2), InvariantCulture) + Convert.ToInt16(StrCMonth, InvariantCulture);
            if (intTemp <= 0)
            { intTemp += 12; }
            if (intTemp > 12)
            { intTemp -= 12; }
            StrFateY1 = string.Format(InvariantCulture, "YY{0:d2}", intTemp);        //命宮地支
            StrFateStar = GetFateStar(StrFateY1);                              //命主
            intTemp = 3 + Convert.ToInt16(StrWHour.Substring(2, 2), InvariantCulture) + Convert.ToInt16(StrCMonth, InvariantCulture) - 2;
            if (intTemp <= 0)
            { intTemp += 12; }
            if (intTemp > 12)
            { intTemp -= 12; }
            StrBodyY1 = string.Format(InvariantCulture, "YY{0:d2}", intTemp);        //身宮地支
            StrBodyStar = GetBodyStar(StrfYearY1);                             //身主
        }

        /// <summary>
        /// 找子年斗君
        /// </summary>
        private void GetYY01Master()                        //找子年斗君
        {
            int intDis = Convert.ToInt16(StrfHourY1.Substring(2, 2), InvariantCulture) - (Convert.ToInt16(StrCMonth, InvariantCulture) - 1);
            if (intDis <= 0)
                intDis += 12;
            StrYY01Master = string.Format(InvariantCulture, "YY{0:d2}", intDis);
        }

        /// <summary>
        /// 找12宮
        /// </summary>
        private void Get12LocationXY()                      //找12宮
        {
            //Dictionary<string, string> CFieldIDToName = new cGLGet().CFieldNameID(1);
            //Dictionary<string, string> CFieldNameToID = new cGLGet().CFieldNameID(2);
            //Dictionary<string, string> CIDToName = new cGLGet().CNameID(1);
            //Dictionary<string, string> CNameToID = new cGLGet().CNameID(2);
            StuStar stuStarTemp;
            Dictionary<string, string> dic12Temp;
            //Dictionary<string, string> dic12TempShow;
            int intYinX = Convert.ToInt16(StrfYearX1.Substring(2, 2), InvariantCulture) % 5;
            if (intYinX <= 0)
                intYinX += 5;
            intYinX = (intYinX * 2 + 1) % 10;                                                       //寅宮天干
            #region 找命宮 天干
            int intFateDistY = Convert.ToInt16(StrFateY1.Substring(2, 2), InvariantCulture) - 3;                 //命宮到寅的距離
            if (intFateDistY < 0)
                intFateDistY += 12;
            int intFateX = (intYinX + intFateDistY) % 10;
            if (intFateX <= 0)
                intFateX += 10;
            StrFateX1 = string.Format(InvariantCulture, "XX{0:d2}", intFateX);                       //命宮天干
            Str60Flower = Get60Flower(string.Format(InvariantCulture, "{0}{1}", StrFateX1, StrFateY1));//找六十花甲
            Str5Element = GetC60To5Element(Str60Flower);                                  //找五形局
            #endregion 找命宮 天干

            #region 定義 十二宮名
            int intLocationX, intLocationY, int12Location;
            string L12XY, L12Name;
            for (int i = 3; i <= 14; i++)                           //寅宮開始
            {
                dic12Temp = new Dictionary<string, string>();
                //dic12TempShow = new Dictionary<string, string>();
                intLocationX = (intYinX + (i - 3)) % 10;            //十二宮 天干
                if (intLocationX <= 0)
                    intLocationX += 10;
                intLocationY = i % 12;                              //十二宮 地支
                if (intLocationY <= 0)
                    intLocationY += 12;
                int12Location = (i - intFateDistY + 10) % 12;       //十二宮 宮名
                if (int12Location <= 0)
                    int12Location += 12;
                L12XY = string.Format(InvariantCulture, "XX{0:d2}YY{1:d2}", intLocationX, intLocationY);
                dic12Temp.Add(L12XY, L12XY); //十二宮 天干地支
                                             //dic12TempShow.Add(L12XY, string.Format(InvariantCulture,"[{0}{1}]",
                                             //    new cGLGet().ConvertIDToName(string.Format(InvariantCulture,"XX{0}", intLocationX.ToString("D2"))),
                                             //    new cGLGet().ConvertIDToName(string.Format(InvariantCulture,"YY{0}", intLocationY.ToString("D2"))))); //十二宮 天干地支
                L12Name = string.Format(InvariantCulture, "LN{0:d2}", int12Location);
                dic12Temp.Add(L12Name, L12Name);  //十二宮 宮名
                                                  //dic12TempShow.Add(L12Name, string.Format(InvariantCulture,"[{0}]",
                                                  //                new cGLGet().ConvertIDToName(string.Format(InvariantCulture,"LN{0}", int12Location.ToString("D2")))));  //十二宮 宮名

                Dic12Location.Add(string.Format(InvariantCulture, "{0:d2}", intLocationY), dic12Temp);
                //this.dic12LocationShow.Add(string.Format(InvariantCulture,"{0}", intLocationY.ToString("D2")), dic12TempShow);
                stuStarTemp = new StuStar()
                {
                    StrLocate = string.Format(InvariantCulture, "YY{0:d2}", intLocationY),
                    StrStarId = dic12Temp[L12Name],
                    //stuStarTemp.strNameShow = dic12TempShow[L12Name];
                    BrushColor = Brushes.LightSalmon
                };
                DicStar.Add(stuStarTemp.StrStarId, stuStarTemp);
            }
            #endregion 定義 十二宮名
        }

        /// <summary>
        /// 設00宮(顯示)
        /// </summary>
        private void SetLocation00Show()                    //設00宮(顯示)
        {
            //Dictionary<string, string> CFieldIDToName = new cGLGet().CFieldNameID(1);
            //Dictionary<string, string> CFieldNameToID = new cGLGet().CFieldNameID(2);          
            //Dictionary<string, string> CNameToID = new cGLGet().CNameID(2);
            Dictionary<string, string> dic12Temp;
            dic12Temp = new Dictionary<string, string>
            {
                { "strGender", string.Format(InvariantCulture,"性別：{0}",new CglFunc().ConvertNameId( GetGender(),1)) },        //性別
                {
                    "strWDate",
                    string.Format(InvariantCulture,"陽曆： {0,4} 年 {1} 月 {2} 日 {3} 時 {4}",
                                                StrWYear,
                                                StrWMonth,
                                                StrWDay,
                                                new CglFunc().ConvertNameId( StrWHour,1),
                                                System.Globalization.DateTimeFormatInfo.CurrentInfo.DayNames[(byte)DowWeekend])
                },
                {
                    "strCDate",
                    string.Format(InvariantCulture,"陰曆： {0,5} 年 {1} 月 {2} 日 {3} 時",
                                                StrCYear,
                                                StrCMonth,
                                                StrCDay,
                                                new CglFunc().ConvertNameId(StrWHour,1))
                },
                { "str5Element", string.Format(InvariantCulture,"五形局： {0}  ", new CglFunc().ConvertNameId(Str60Flower,1)) },
                { "strYY01Star", string.Format(InvariantCulture,"子年斗君： {0}  ", new CglFunc().ConvertNameId(StrYY01Master,1)) },
                {
                    "strFateY1",
                    string.Format(InvariantCulture,"命宮：{0,5}      身宮： {1}",
                                                new CglFunc().ConvertNameId(StrFateY1,1),
                                                new CglFunc().ConvertNameId(StrBodyY1,1))
                },
                {
                    "strFateStar",
                    string.Format(InvariantCulture,"命主：{0,5}      身主： {1}",
                                                new CglFunc().ConvertNameId(StrFateStar,1),
                                                new CglFunc().ConvertNameId(StrBodyStar,1))
                },
                { "strWeight", string.Format(InvariantCulture,"命重：{0}", DblWeight) },
                {
                    "strf8Col",
                    $"干支速算： [{new CglFunc().ConvertNameId(StrfYearX1,1)}{new CglFunc().ConvertNameId(StrfYearY1,1)}] 年" +
                    $" [{new CglFunc().ConvertNameId(StrfMonthX1,1)}{new CglFunc().ConvertNameId(StrfMonthY1,1)}] 月" +
                    $" [{new CglFunc().ConvertNameId(StrfDayX1,1)}{new CglFunc().ConvertNameId(StrfDayY1,1)}] 日" +
                    $" [{new CglFunc().ConvertNameId(StrfHourX1,1)}{new CglFunc().ConvertNameId(StrfHourY1,1)}] 時"
                },
                {
                    "str8Col",
                    $"四　　柱： [{new CglFunc().ConvertNameId(StrYearX1,1)}{new CglFunc().ConvertNameId(StrYearY1,1)}] 年" +
                    $" [{new CglFunc().ConvertNameId(StrMonthX1,1)}{new CglFunc().ConvertNameId(StrMonthY1,1)}] 月" +
                    $" [{new CglFunc().ConvertNameId(StrDayX1,1)}{new CglFunc().ConvertNameId(StrDayY1,1)}] 日" +
                    $" [{new CglFunc().ConvertNameId(StrHourX1,1)}{new CglFunc().ConvertNameId(StrHourY1,1)}] 時"
                }
            };   //00宮
            Dic12LocationShow.Add(key: "00", value: dic12Temp);
        }

        /// <summary>
        /// 設眾星位置
        /// </summary>
        private void SetStar()                              //設眾星位置
        {

            //紫微AY01所在宮位
            SetStarData(strInput01: Str5Element, strInput02: StrCDay, strStarName: "AY01", strStarLevel: "AY01", brushColor: Brushes.Yellow);
            //天機AY02所在宮位
            SetStarData(strInput01: "AY02", strInput02: DicStar["AY01"].StrLocate, strStarName: "AY02", strStarLevel: "AY02", brushColor: Brushes.Yellow);
            //太陽AY03所在宮位
            SetStarData(strInput01: "AY03", strInput02: DicStar["AY01"].StrLocate, strStarName: "AY03", strStarLevel: "AY03", brushColor: Brushes.Yellow);
            //武曲AY04所在宮位
            SetStarData(strInput01: "AY04", strInput02: DicStar["AY01"].StrLocate, strStarName: "AY04", strStarLevel: "AY04", brushColor: Brushes.Yellow);
            //天同AY05所在宮位
            SetStarData(strInput01: "AY05", strInput02: DicStar["AY01"].StrLocate, strStarName: "AY05", strStarLevel: "AY05", brushColor: Brushes.Yellow);
            //廉貞AY06所在宮位
            SetStarData(strInput01: "AY06", strInput02: DicStar["AY01"].StrLocate, strStarName: "AY06", strStarLevel: "AY06", brushColor: Brushes.Yellow);
            //天府BY01所在宮位
            SetStarData(strInput01: "BY01", strInput02: DicStar["AY01"].StrLocate, strStarName: "BY01", strStarLevel: "BY01", brushColor: Brushes.Blue);
            //太陰BY02所在宮位
            SetStarData(strInput01: "BY02", strInput02: DicStar["BY01"].StrLocate, strStarName: "BY02", strStarLevel: "BY02", brushColor: Brushes.Blue);
            //貪狼BY03所在宮位
            SetStarData(strInput01: "BY03", strInput02: DicStar["BY01"].StrLocate, strStarName: "BY03", strStarLevel: "BY03", brushColor: Brushes.Blue);
            //巨門BY04所在宮位
            SetStarData(strInput01: "BY04", strInput02: DicStar["BY01"].StrLocate, strStarName: "BY04", strStarLevel: "BY04", brushColor: Brushes.Blue);
            //天相BY05所在宮位
            SetStarData(strInput01: "BY05", strInput02: DicStar["BY01"].StrLocate, strStarName: "BY05", strStarLevel: "BY05", brushColor: Brushes.Blue);
            //天樑BY06所在宮位
            SetStarData(strInput01: "BY06", strInput02: DicStar["BY01"].StrLocate, strStarName: "BY06", strStarLevel: "BY06", brushColor: Brushes.Blue);
            //七殺BY07所在宮位
            SetStarData(strInput01: "BY07", strInput02: DicStar["BY01"].StrLocate, strStarName: "BY07", strStarLevel: "BY07", brushColor: Brushes.Blue);
            //破軍BY08所在宮位
            SetStarData(strInput01: "BY08", strInput02: DicStar["BY01"].StrLocate, strStarName: "BY08", strStarLevel: "BY08", brushColor: Brushes.Blue);
            //文昌CY01所在宮位
            SetStarData(strInput01: "CY01", strInput02: StrfHourY1, strStarName: "CY01", strStarLevel: "CY01", brushColor: Brushes.LightGreen);
            //文曲CY02所在宮位
            SetStarData(strInput01: "CY02", strInput02: StrfHourY1, strStarName: "CY02", strStarLevel: "CY02", brushColor: Brushes.LightGreen);
            //火星CY03所在宮位
            SetStarData(strInput01: "CY03", strInput02: StrfYearY1 + StrfHourY1, strStarName: "CY03", strStarLevel: "CY03", brushColor: Brushes.LightGreen);
            //鈴星CY04所在宮位
            SetStarData(strInput01: "CY04", strInput02: StrfYearY1 + StrfHourY1, strStarName: "CY04", strStarLevel: "CY04", brushColor: Brushes.LightGreen);
            //地劫CY05所在宮位
            SetStarData("CY05", StrfHourY1, "CY05", "CY05", Brushes.LightGreen);
            //天空CY06所在宮位
            SetStarData("CY06", StrfHourY1, "CY06", "CY06", Brushes.LightGreen);
            //台輔CY07所在宮位
            SetStarData("CY07", StrfHourY1, "CY07", "CY07", Brushes.LightGreen);
            //封誥CY08所在宮位
            SetStarData("CY08", StrfHourY1, "CY08", "CY08", Brushes.LightGreen);
            //左輔DM01所在宮位
            SetStarData("DM01", StrCMonth, "DM01", "DM01", Brushes.LightPink);
            //右弼DM02所在宮位
            SetStarData("DM02", StrCMonth, "DM02", "DM02", Brushes.LightPink);
            //天刑DM03所在宮位
            SetStarData("DM03", StrCMonth, "DM03", "DM03", Brushes.LightPink);
            //天姚DM04所在宮位
            SetStarData("DM04", StrCMonth, "DM04", "DM04", Brushes.LightPink);
            //天馬DM05所在宮位
            SetStarData("DM05", StrCMonth, "DM05", "DM05", Brushes.LightPink);
            //解神DM06所在宮位
            SetStarData("DM06", StrCMonth, "DM06", "DM06", Brushes.LightPink);
            //天巫DM07所在宮位
            SetStarData("DM07", StrCMonth, "DM07", "DM07", Brushes.LightPink);
            //天月DM08所在宮位
            SetStarData("DM08", StrCMonth, "DM08", "DM08", Brushes.LightPink);
            //陰煞DM09所在宮位
            SetStarData("DM09", StrCMonth, "DM09", "DM09", Brushes.LightPink);
            //三台ED01所在宮位
            StuStar stuStarTemp = new();
            int intStarLocation = (Convert.ToInt16(DicStar["DM01"].StrLocate.Substring(2, 2), InvariantCulture) + (Convert.ToInt16(StrCDay, InvariantCulture) - 1)) % 12;
            if (intStarLocation <= 0)
                intStarLocation += 12;
            stuStarTemp.StrLocate = string.Format(CultureInfo.InvariantCulture, "YY{0}", intStarLocation.ToString("D2", CultureInfo.InvariantCulture));
            stuStarTemp.StrStarId = DicStarStrength[string.Format(InvariantCulture, "{0}{1}", "ED01", stuStarTemp.StrLocate)];
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCoral;
            UpdateStarInfo("ED01", "ED01", stuStarTemp);
            //三台ED02所在宮位
            stuStarTemp = new StuStar();
            intStarLocation = (Convert.ToInt16(DicStar["DM02"].StrLocate.Substring(2, 2), CultureInfo.InvariantCulture) - (Convert.ToInt16(StrCDay, CultureInfo.InvariantCulture) - 1)) % 12;
            if (intStarLocation <= 0)
                intStarLocation += 12;
            stuStarTemp.StrLocate = string.Format(InvariantCulture, "YY{0:d2}", intStarLocation);
            stuStarTemp.StrStarId = DicStarStrength[string.Format(InvariantCulture, "{0}{1}", "ED02", stuStarTemp.StrLocate)];
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCoral;
            UpdateStarInfo("ED02", "ED02", stuStarTemp);
            //恩光EC03所在宮位
            stuStarTemp = new StuStar();
            intStarLocation = (Convert.ToInt16(value: DicStar["CY01"].StrLocate.Substring(startIndex: 2, length: 2), InvariantCulture) + (Convert.ToInt16(value: StrCDay, InvariantCulture) - 2)) % 12;
            if (intStarLocation <= 0)
                intStarLocation += 12;
            stuStarTemp.StrLocate = string.Format(InvariantCulture, format: "YY{0:d2}", arg0: intStarLocation);
            stuStarTemp.StrStarId = DicStarStrength[string.Format(InvariantCulture, format: "{0}{1}", arg0: "EC03", arg1: stuStarTemp.StrLocate)];
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCoral;
            UpdateStarInfo(strStarLevel: "EC03", strStarName: "EC03", stuStarInput: stuStarTemp);
            //天貴EC04所在宮位
            stuStarTemp = new StuStar();
            intStarLocation = (Convert.ToInt16(value: DicStar["CY02"].StrLocate.Substring(startIndex: 2, length: 2), CultureInfo.InvariantCulture) + (Convert.ToInt16(value: StrCDay, CultureInfo.InvariantCulture) - 2)) % 12;
            if (intStarLocation <= 0)
                intStarLocation += 12;
            stuStarTemp.StrLocate = string.Format(CultureInfo.InvariantCulture, format: "YY{0:d2}", arg0: intStarLocation);
            stuStarTemp.StrStarId = DicStarStrength[string.Format(CultureInfo.InvariantCulture, format: "{0}{1}", arg0: "EC04", arg1: stuStarTemp.StrLocate)];
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCoral;
            UpdateStarInfo(strStarLevel: "EC04", strStarName: "EC04", stuStarInput: stuStarTemp);
            //祿存FX01所在宮位
            SetStarData("FX01", StrfYearX1, "FX01", "FX01", Brushes.LightCyan);
            //羊刃FX02所在宮位
            SetStarData("FX02", StrfYearX1, "FX02", "FX02", Brushes.LightCyan);
            //陀羅FX03所在宮位
            SetStarData("FX03", StrfYearX1, "FX03", "FX03", Brushes.LightCyan);
            //天魁FX04所在宮位
            SetStarData("FX04", StrfYearX1, "FX04", "FX04", Brushes.LightCyan);
            //天鉞FX05所在宮位
            SetStarData("FX05", StrfYearX1, "FX05", "FX05", Brushes.LightCyan);
            //化祿FX06所在宮位
            string strInput, strOutput;
            strInput = string.Format(CultureInfo.InvariantCulture, "FX06{0}", StrYearX1);
            strOutput = DicStarLocation[strInput];
            stuStarTemp = DicStar[strOutput];
            stuStarTemp.StrStarId += "FX06";
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCyan;
            Dic12Location[stuStarTemp.StrLocate.Substring(2, 2)][strOutput] = stuStarTemp.StrStarId;
            //this.dic12LocationShow[stuStarTemp.strLocate.Substring(2, 2)][strOutput] = stuStarTemp.strNameShow;
            //化權FX07所在宮位
            strInput = string.Format(CultureInfo.InvariantCulture, "FX07{0}", StrYearX1);
            strOutput = DicStarLocation[key: strInput];
            stuStarTemp = DicStar[key: strOutput];
            stuStarTemp.StrStarId += "FX07";
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCyan;
            Dic12Location[key: stuStarTemp.StrLocate.Substring(startIndex: 2, length: 2)][key: strOutput] = stuStarTemp.StrStarId;
            //this.dic12LocationShow[stuStarTemp.strLocate.Substring(2, 2)][strOutput] = stuStarTemp.strNameShow;
            //化科FX08所在宮位
            strInput = string.Format(CultureInfo.InvariantCulture, "FX08{0}", StrYearX1);
            strOutput = DicStarLocation[key: strInput];
            stuStarTemp = DicStar[key: strOutput];
            stuStarTemp.StrStarId += "FX08";
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCyan;
            Dic12Location[key: stuStarTemp.StrLocate.Substring(startIndex: 2, length: 2)][key: strOutput] = stuStarTemp.StrStarId;
            //this.dic12LocationShow[stuStarTemp.strLocate.Substring(2, 2)][strOutput] = stuStarTemp.strNameShow;
            //化忌FX09所在宮位
            strInput = string.Format(CultureInfo.InvariantCulture, "FX09{0}", StrYearX1);
            strOutput = DicStarLocation[key: strInput];
            stuStarTemp = DicStar[key: strOutput];
            stuStarTemp.StrStarId += "FX09";
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightCyan;
            Dic12Location[key: stuStarTemp.StrLocate.Substring(startIndex: 2, length: 2)][key: strOutput] = stuStarTemp.StrStarId;
            //this.dic12LocationShow[stuStarTemp.strLocate.Substring(2, 2)][strOutput] = stuStarTemp.strNameShow;
            //天官FX10所在宮位
            SetStarData(strInput01: "FX10", strInput02: StrfYearX1, strStarName: "FX10", strStarLevel: "FX10", brushColor: Brushes.LightCyan);
            //天福FX11所在宮位
            SetStarData(strInput01: "FX11", strInput02: StrfYearX1, strStarName: "FX11", strStarLevel: "FX11", brushColor: Brushes.LightCyan);
            //博士OB01所在宮位
            stuStarTemp = new StuStar()
            {
                StrLocate = DicStar[key: "FX01"].StrLocate,
                StrStarId = DicStarStrength[key: string.Format(CultureInfo.InvariantCulture, "OB01{0}", stuStarTemp.StrLocate)],
                BrushColor = Brushes.LightGray
            };
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            UpdateStarInfo(strStarLevel: "OB01", strStarName: "OB01", stuStarInput: stuStarTemp);
            //OB02-OB12所在宮位
            int intGenderTemp = 1;
            switch (IntGender)
            {
                case Lunar.Gender.MalePlus:
                    intGenderTemp = 1;
                    break;
                case Lunar.Gender.MaleMinus:
                    intGenderTemp = -1;
                    break;
                case Lunar.Gender.FemalMinuse:
                    intGenderTemp = 1;
                    break;
                case Lunar.Gender.FemalePlus:
                    intGenderTemp = -1;
                    break;
            }
            for (int i = 1; i <= 11; i++)
            {
                int intStarLocationTemp = (Convert.ToInt16(value: DicStar[key: "FX01"].StrLocate.Substring(startIndex: 2, length: 2), CultureInfo.InvariantCulture) + i * intGenderTemp) % 12;
                if (intStarLocationTemp <= 0)
                    intStarLocationTemp += 12;
                string strStartName = string.Format(InvariantCulture, "OB{0:d2}", i + 1);
                stuStarTemp = new StuStar()
                {
                    StrLocate = string.Format(CultureInfo.InvariantCulture, "YY{0:d2}", intStarLocationTemp),
                    StrStarId = DicStarStrength[key: string.Format(CultureInfo.InvariantCulture, "{0}{1}", strStartName, stuStarTemp.StrLocate)],
                    BrushColor = Brushes.LightGray
                };
                //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
                UpdateStarInfo(strStarLevel: strStartName, strStarName: strStartName, stuStarInput: stuStarTemp);
            }
            //天哭FY01所在宮位
            SetStarData(strInput01: "FY01", strInput02: StrfYearY1, strStarName: "FY01", strStarLevel: "FY01", brushColor: Brushes.LightSeaGreen);
            //天虛FY02所在宮位
            SetStarData(strInput01: "FY02", strInput02: StrfYearY1, strStarName: "FY02", strStarLevel: "FY02", brushColor: Brushes.LightSeaGreen);
            //龍池FY03所在宮位
            SetStarData(strInput01: "FY03", strInput02: StrfYearY1, strStarName: "FY03", strStarLevel: "FY03", brushColor: Brushes.LightSeaGreen);
            //鳳閣FY04所在宮位
            SetStarData(strInput01: "FY04", strInput02: StrfYearY1, strStarName: "FY04", strStarLevel: "FY04", brushColor: Brushes.LightSeaGreen);
            //紅鸞FY05所在宮位
            SetStarData(strInput01: "FY05", strInput02: StrfYearY1, strStarName: "FY05", strStarLevel: "FY05", brushColor: Brushes.LightSeaGreen);
            //天喜FY06所在宮位
            SetStarData(strInput01: "FY06", strInput02: StrfYearY1, strStarName: "FY06", strStarLevel: "FY06", brushColor: Brushes.LightSeaGreen);
            //孤辰FY07所在宮位
            SetStarData(strInput01: "FY07", strInput02: StrfYearY1, strStarName: "FY07", strStarLevel: "FY07", brushColor: Brushes.LightSeaGreen);
            //寡宿FY08所在宮位
            SetStarData(strInput01: "FY08", strInput02: StrfYearY1, strStarName: "FY08", strStarLevel: "FY08", brushColor: Brushes.LightSeaGreen);
            //蜚廉FY09所在宮位
            SetStarData(strInput01: "FY09", strInput02: StrfYearY1, strStarName: "FY09", strStarLevel: "FY09", brushColor: Brushes.LightSeaGreen);
            //破碎FY10所在宮位
            SetStarData(strInput01: "FY10", strInput02: StrfYearY1, strStarName: "FY10", strStarLevel: "FY10", brushColor: Brushes.LightSeaGreen);
            //天才FY11所在宮位
            stuStarTemp = new StuStar
            {
                //string sst = DicStarLocation[key:string.Format(CultureInfo.InvariantCulture, "FY11{0}",StrfYearY1)];
                StrLocate = DicStar[key: DicStarLocation[key: string.Format(CultureInfo.InvariantCulture, "FY11{0}", StrfYearY1)]].StrLocate
            };

            stuStarTemp.StrStarId = DicStarStrength[key: string.Format(CultureInfo.InvariantCulture, "FY11{0}", stuStarTemp.StrLocate)];
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            stuStarTemp.BrushColor = Brushes.LightSeaGreen;

            UpdateStarInfo(strStarLevel: "FY11", strStarName: "FY11", stuStarInput: stuStarTemp);

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
            _ = string.Format(CultureInfo.InvariantCulture, "{0}{1}", strInput01, strInput02);
            _ = brushColor;
            StuStar stuStarTemp = new();
            stuStarTemp.StrStarId = DicStarStrength[key: string.Format(provider: CultureInfo.InvariantCulture, format: "{0}{1}", arg0: strStarName, arg1: stuStarTemp.StrLocate)];
            //stuStarTemp.strNameShow = new cGLGet().ConvertIDToName(stuStarTemp.strStarID);
            UpdateStarInfo(strStarLevel: strStarLevel, strStarName: strStarName, stuStarInput: stuStarTemp);
        }

        /// <summary>
        /// 更新星座資料,十二宮資料
        /// </summary>
        /// <param name="strStarLevel">星座位階</param>
        /// <param name="strStarName">星座名稱(strStarID)</param>
        /// <param name="stuStarInput">星座結構(stuStar)</param>
        private void UpdateStarInfo(string strStarLevel, string strStarName, StuStar stuStarInput) // 更新星座資料,十二宮資料
        {
            DicStar.Add(key: strStarName, value: stuStarInput);
            Dic12Location[key: stuStarInput.StrLocate.Substring(startIndex: 2, length: 2)].Add(key: strStarLevel, value: stuStarInput.StrStarId);
        }


        /// <summary>
        /// 找命主
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private static string GetFateStar(string strInput) => new Dictionary<string, string>
            {   { "YY01", "BY03" },{ "YY02", "BY04" },{ "YY03", "FX01" },{ "YY04", "CY02" },
                { "YY05", "AY06" },{ "YY06", "AY04" },{ "YY07", "BY08" },{ "YY08", "AY04" },
                { "YY09", "AY06" },{ "YY10", "CY02" },{ "YY11", "FX01" },{ "YY12", "BY04" }
            }[strInput];

        /// <summary>
        /// 找身主
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private static string GetBodyStar(string strInput) => new Dictionary<string, string>
            {
                { "YY01", "CY03" },{ "YY02", "BY05" },{ "YY03", "BY06" },{ "YY04", "AY05" },
                { "YY05", "CY01" },{ "YY06", "AY02" },{ "YY07", "CY03" },{ "YY08", "BY05" },
                { "YY09", "BY06" },{ "YY10", "AY05" },{ "YY11", "CY01" },{ "YY12", "AY02" }
            }[strInput];

        /// <summary>
        /// 建立六十花甲納音
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private string Get60Flower(string strInput) => new Dictionary<string, string>
            {
            { "XX01YY01", "NF01" },{ "XX02YY02", "NF01" },{ "XX03YY03", "NF02" },{ "XX04YY04", "NF02" },{ "XX05YY05", "NF03" },
            { "XX06YY06", "NF03" },{ "XX07YY07", "NF04" },{ "XX08YY08", "NF04" },{ "XX09YY09", "NF05" },{ "XX10YY10", "NF05" },
            { "XX01YY11", "NF06" },{ "XX02YY12", "NF06" },{ "XX03YY01", "NF07" },{ "XX04YY02", "NF07" },{ "XX05YY03", "NF08" },
            { "XX06YY04", "NF08" },{ "XX07YY05", "NF09" },{ "XX08YY06", "NF09" },{ "XX09YY07", "NF10" },{ "XX10YY08", "NF10" },
            { "XX01YY09", "NF11" },{ "XX02YY10", "NF11" },{ "XX03YY11", "NF12" },{ "XX04YY12", "NF12" },{ "XX05YY01", "NF13" },
            { "XX06YY02", "NF13" },{ "XX07YY03", "NF14" },{ "XX08YY04", "NF14" },{ "XX09YY05", "NF15" },{ "XX10YY06", "NF15" },
            { "XX01YY07", "NF16" },{ "XX02YY08", "NF16" },{ "XX03YY09", "NF17" },{ "XX04YY10", "NF17" },{ "XX05YY11", "NF18" },
            { "XX06YY12", "NF18" },{ "XX07YY01", "NF19" },{ "XX08YY02", "NF19" },{ "XX09YY03", "NF20" },{ "XX10YY04", "NF20" },
            { "XX01YY05", "NF21" },{ "XX02YY06", "NF21" },{ "XX03YY07", "NF22" },{ "XX04YY08", "NF22" },{ "XX05YY09", "NF23" },
            { "XX06YY10", "NF23" },{ "XX07YY11", "NF24" },{ "XX08YY12", "NF24" },{ "XX09YY01", "NF25" },{ "XX10YY02", "NF25" },
            { "XX01YY03", "NF26" },{ "XX02YY04", "NF26" },{ "XX03YY05", "NF27" },{ "XX04YY06", "NF27" },{ "XX05YY07", "NF28" },
            { "XX06YY08", "NF28" },{ "XX07YY09", "NF29" },{ "XX08YY10", "NF29" },{ "XX09YY11", "NF30" },{ "XX10YY12", "NF30" }
            }[strInput];


        /// <summary>
        /// 六十花甲定五形局
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private string GetC60To5Element(String strInput) => new Dictionary<string, string> 
            {
            { "NF01", "NE04" },{ "NF02", "NE06" },{ "NF03", "NE03" },{ "NF04", "NE05" },{ "NF05", "NE04" },
            { "NF06", "NE06" },{ "NF07", "NE02" },{ "NF08", "NE05" },{ "NF09", "NE04" },{ "NF10", "NE03" },
            { "NF11", "NE02" },{ "NF12", "NE05" },{ "NF13", "NE06" },{ "NF14", "NE03" },{ "NF15", "NE02" },
            { "NF16", "NE04" },{ "NF17", "NE06" },{ "NF18", "NE03" },{ "NF19", "NE05" },{ "NF20", "NE04" },
            { "NF21", "NE06" },{ "NF22", "NE02" },{ "NF23", "NE05" },{ "NF24", "NE04" },{ "NF25", "NE03" },
            { "NF26", "NE02" },{ "NF27", "NE05" },{ "NF28", "NE06" },{ "NF29", "NE03" },{ "NF30", "NE02" }
        }[strInput];   

    }
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

}
