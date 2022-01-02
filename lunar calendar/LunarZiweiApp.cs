using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lunar_calendar
{
    public partial class LunarZiwei
    {
        /// <summary>
        /// 計算命宮地支
        /// </summary>
        /// <param name="lunarMonth">農曆月</param>
        /// <param name="lunarDate.HourEB">農曆時支</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private int GetFateEB(LunarDate lunarDate)
        {
            int First = Tools.CheckRange(3 + lunarDate.Month - 1, 1, 12);
            int Distance = lunarDate.HourEB - 1;
            return Tools.CheckRange(First - Distance, 1, 12);
        }

        /// <summary>
        /// 計算身宮地支
        /// </summary>
        /// <param name="lunarMonth">農曆月</param>
        /// <param name="lunarDate.HourEB">農曆時支</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private int GetBodyEB(LunarDate lunarDate)
        {
            int First = Tools.CheckRange(3 + lunarDate.Month - 1, 1, 12);
            int Distance = lunarDate.HourEB - 1;
            return Tools.CheckRange(First + Distance , 1, 12);
        }

        /// <summary>
        /// 計算命宮地支
        /// </summary>
        /// <param name="yearHS">農曆年干</param>
        /// <param name="fateEB">命宮地支</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private int GetFateHS(LunarDate lunarDate, int fateEB)
        {
            int First = Tools.CheckRange(3 + ((lunarDate.YearHS - 1) % 5) * 2, 1, 10);
            int distance = fateEB >= 3 ? fateEB - 3 : 12 + fateEB - 3;
            return Tools.CheckRange(First + distance, 1, 10);
        }

        /// <summary>
        /// MalePlus = 陽男, FemalMinuse = 陰女, MaleMinus = 陰男, FemalePlus = 陽女
        /// </summary>
        /// <param name="gender">性別</param>
        /// <param name="yearEB">農曆年支</param>
        /// <returns></returns>
        private Gender CheckGender(Gender gender, int yearEB)
        {
            switch (gender)
            {
                case Gender.MaleMinus:
                case Gender.MalePlus:
                    if (yearEB % 2 == 1) { return Gender.MalePlus; } else { return Gender.MaleMinus; }
                case Gender.FemalePlus:
                case Gender.FemalMinuse:
                    if (yearEB % 2 == 1) { return Gender.FemalePlus; } else { return Gender.FemalMinuse; }
                case Gender.None:
                    break;
            }
            return gender;
        }

        /// <summary>
        /// 定12宮
        /// </summary>
        /// <param name="fateHS">命宮干</param>
        /// <param name="fateEB">命宮支</param>
        /// <returns></returns>
        private Dictionary<string, List<string>> SetDicLNStart(int fateHS, int fateEB)
        {
            Dictionary<string, List<string>> dicLNStart = new();
            for (int index = 1; index <= 12; index++)
            {
                List<string> list = new();
                list.Add($"{Tools.CheckRange(fateHS + index - 1, 1, 10):00}{Tools.CheckRange(fateEB + index - 1, 1, 12):00}");
                dicLNStart.Add($"LN{index:00}", list);
            }
            return dicLNStart;
        }
        /// <summary>
        /// 設諸星
        /// </summary>
        /// <param name="startEB">星地支</param>
        /// <param name="startName">星名</param>
        private void SetStart(int startEB, string startName)
        {
            foreach (var item in dicLNStart)
            {
                if (int.Parse(item.Value[0].Substring(2, 2)) == startEB)
                {
                    item.Value.Add(startName);
                }
            }
        }

        private void SortStarOrder()
        {
            for (int Ln = 1; Ln <= 12; Ln++)
            {
                DataTable dtStar = new();
                dtStar.Columns.Add(new DataColumn() { ColumnName = "StarName", DataType = typeof(string) });
                dtStar.Columns.Add(new DataColumn() { ColumnName = "Order", DataType = typeof(int) });
                List<string> listLNStart = dicLNStart[$"LN{Ln:00}"];
                for (int index = 1; index < listLNStart.Count; index++)
                {
                    DataRow drStar = dtStar.NewRow();
                    drStar["StarName"] = listLNStart[index];
                    drStar["Order"] = GetOrder(listLNStart[index]);
                    dtStar.Rows.Add(drStar);
                }
                List<string> newList = new();
                newList.Add(listLNStart[0]);
                foreach (DataRow dr in dtStar.Select(string.Empty, "Order ASC ,StarName ASC").CopyToDataTable().Rows)
                {
                    newList.Add(dr["StarName"].ToString());
                }
                dicLNStart[$"LN{Ln:00}"] = newList;
            }
        }

        private int GetOrder(string star)
        {
            int result = 6;
            if (new string[] { "AY01", "AY02", "AY03", "AY04", "AY05", "AY06", "BY01", "BY02", "BY03", "BY04", "BY05", "BY06", "BY07", "BY08", "CY01", "CY02", "CY03", "CY04", "DM01", "DM02", "DM03", "DM04", "FX01", "FX02", "FX03", "FX04", "FX05", "FX06", "FX07", "FX08", "FX09" }.Contains(star)) { result = 1; }
            if (new string[] { "CY05", "CY06", "CY07", "CY08", "DM05", "DM06", "DM07", "DM08", "DM09", "EC01", "EC02", "EC03", "EC04", "FX10", "FX11", "FY01", "FY02", "FY03", "FY04", "FY05", "FY06", "FY07", "FY08", "FY09", "FY10", "FY11", "FY12" }.Contains(star)) { result = 2; }
            if (new string[] { "ME01", "ME02", "ME03", "ME04", "ME05", "ME06", "ME07", "ME08", "ME09", "ME10", "ME11", "ME12" }.Contains(star)) { result = 3; }
            if (new string[] { "QY01", "QY08", "QY10", "RY01", "RY02", "RY03", "RY05" }.Contains(star)) { result = 4; }
            if (new string[] { "QY02", "QY03", "QY04", "QY05", "QY06", "QY07", "QY09", "QY11", "QY12", "RY04", "RY06", "RY07", "RY08", "RY09", "RY10", "RY11", "RY12" }.Contains(star)) { result = 5; }
            return result;
        }

        /// <summary>
        /// 紫微地支 AY01
        /// </summary>
        /// <param name="str5Element">五形局</param>
        /// <param name="lunarDay">農曆日</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private int GetZiweiEB(string str5Element, LunarDate lunarDate)
        {
            int result = 1;
            switch (str5Element)
            {
                case "NE02":
                    result = new int[] { 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 1, 1, 2, 2, 3, 3, 4, 4, 5 }[lunarDate.Day - 1];
                    break;
                case "NE03":
                    result = new int[] { 5, 2, 3, 6, 3, 4, 7, 4, 5, 8, 5, 6, 9, 6, 7, 10, 7, 8, 11, 8, 9, 12, 9, 10, 1, 10, 11, 2, 11, 12 }[lunarDate.Day - 1];
                    break;
                case "NE04":
                    result = new int[] { 12, 5, 2, 3, 1, 6, 3, 4, 2, 7, 4, 5, 3, 8, 5, 6, 4, 9, 6, 7, 5, 10, 7, 8, 6, 11, 8, 9, 7, 12 }[lunarDate.Day - 1];
                    break;
                case "NE05":
                    result = new int[] { 7, 12, 5, 2, 3, 8, 1, 6, 3, 4, 9, 2, 7, 4, 5, 10, 3, 8, 5, 6, 11, 4, 9, 6, 7, 12, 5, 10, 7, 8 }[lunarDate.Day - 1];
                    break;
                case "NE06":
                    result = new int[] { 10, 7, 12, 5, 2, 3, 11, 8, 1, 6, 3, 4, 12, 9, 2, 7, 4, 5, 1, 10, 3, 8, 5, 6, 2, 11, 4, 9, 6, 7 }[lunarDate.Day - 1];
                    break;
            }
            return result;
        }
        /// <summary>
        /// 安紫微諸星 AY
        /// </summary>
        /// <param name="ziweiEB">紫微地支</param>
        /// <param name="index">序號 2:天機,3:太陽,4:武曲,5:天同,6:廉貞</param>
        /// <returns></returns>
        private int GetAY(int ziweiEB, int index)
        {
            int result = 1;
            switch (index)
            {
                case 2:
                    result = Tools.CheckRange(12 + (ziweiEB - 1), 1, 12);
                    break;
                case 3:
                    result = Tools.CheckRange(10 + (ziweiEB - 1), 1, 12);
                    break;
                case 4:
                    result = Tools.CheckRange(9 + (ziweiEB - 1), 1, 12);
                    break;
                case 5:
                    result = Tools.CheckRange(8 + (ziweiEB - 1), 1, 12);
                    break;
                case 6:
                    result = Tools.CheckRange(5 + (ziweiEB - 1), 1, 12);
                    break;
            }
            return result;
            //return new int[][] { new int[] { 12, 10, 9, 8, 5 }, new int[] { 1, 11, 10, 9, 6 }, new int[] { 2, 12, 11, 10, 7 },
            //                     new int[] { 3, 1, 12, 11, 8 }, new int[] { 4, 2, 1, 12, 9 } , new int[] { 5, 3, 2, 1, 10 },
            //                     new int[] { 6, 4, 3, 2, 11 } , new int[] { 7, 5, 4, 3, 12 } , new int[] { 8, 6, 5, 4, 1 } ,
            //                     new int[] { 9, 7, 6, 5, 2 }  , new int[] { 10, 8, 7, 6, 3 } , new int[] { 11, 9, 8, 7, 4 }}
            //                     [ziweiEB - 1][index - 2];
        }

        /// <summary>
        /// 天府地支 BY01
        /// </summary>
        /// <param name="ziweiEB">紫微地支</param>
        /// <returns></returns>
        private int GetBY01EB(int ziweiEB)
        {
            return Tools.CheckRange(5 - (ziweiEB - 1), 1, 12);
        }
        /// <summary>
        /// 安天府諸星 BY
        /// </summary>
        /// <param name="by01EB">天府地支</param>
        /// <param name="index">序號 2:太陰,3:貪狼,4:巨門,5:天相,6:天梁,7:七殺,8:破軍</param>
        /// <returns></returns>
        private int GetBY(int by01EB, int index)
        {
            return index != 8 ? Tools.CheckRange(by01EB + index - 1, 1, 12) :
                                Tools.CheckRange(10 + by01EB, 1, 12);
        }

        /// <summary>
        /// 安時系諸星 CY
        /// </summary>
        /// <param name="lunarDate">農曆 年地支 , 時地支</param>
        /// <param name="index">序號 1:文昌 02:文曲 3:火星 4:鈴星 5:地劫 6：天空 7:台輔 8:封誥</param>
        /// <returns></returns>
        private int GetCY(LunarDate lunarDate, int index)
        {
            int result = 1;
            switch (index)
            {
                case 1:
                    result = Tools.CheckRange(11 - (lunarDate.HourEB - 1), 1, 12);
                    break;
                case 2:
                    result = Tools.CheckRange(5 + (lunarDate.HourEB - 1), 1, 12);
                    break;
                case 3:
                    if (new int[] { 3, 7, 11 }.Contains(lunarDate.YearEB))
                    {
                        result = Tools.CheckRange(2 + (lunarDate.HourEB - 1), 1, 12);
                    }
                    if (new int[] { 9, 1, 5 }.Contains(lunarDate.YearEB))
                    {
                        result = Tools.CheckRange(3 + (lunarDate.HourEB - 1), 1, 12);
                    }
                    if (new int[] { 6, 10, 2 }.Contains(lunarDate.YearEB))
                    {
                        result = Tools.CheckRange(4 + (lunarDate.HourEB - 1), 1, 12);
                    }
                    if (new int[] { 12, 4, 8 }.Contains(lunarDate.YearEB))
                    {
                        result = Tools.CheckRange(10 + (lunarDate.HourEB - 1), 1, 12);
                    }
                    break;
                case 4:
                    if (new int[] { 3, 7, 11 }.Contains(lunarDate.YearEB))
                    {
                        result = Tools.CheckRange(4 + (lunarDate.HourEB - 1), 1, 12);
                    }
                    if (new int[] { 9, 1, 5 }.Contains(lunarDate.YearEB))
                    {
                        result = Tools.CheckRange(11 + (lunarDate.HourEB - 1), 1, 12);
                    }
                    if (new int[] { 12, 4, 8 }.Contains(lunarDate.YearEB))
                    {
                        result = Tools.CheckRange(11 + (lunarDate.HourEB - 1), 1, 12);
                    }
                    if (new int[] { 6, 10, 2 }.Contains(lunarDate.YearEB))
                    {
                        result = Tools.CheckRange(11 + (lunarDate.HourEB - 1), 1, 12);
                    }
                    break;
                case 5:
                    result = Tools.CheckRange(12 + (lunarDate.HourEB - 1), 1, 12);
                    break;
                case 6:
                    result = Tools.CheckRange(12 - (lunarDate.HourEB - 1), 1, 12);
                    break;
                case 7:
                    result = Tools.CheckRange(7 + (lunarDate.HourEB - 1), 1, 12);
                    break;
                case 8:
                    result = Tools.CheckRange(3 + (lunarDate.HourEB - 1), 1, 12);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 安月系諸星 DM
        /// </summary>
        /// <param name="lunarDate">農曆年</param>
        /// <param name="index">序號 1:左輔 2:右弼 3:天刑 4:天姚 5:天馬 6:解神 7:天巫 8:天月 9:陰煞</param>
        /// <returns></returns>
        private int GetDM(LunarDate lunarDate, int index)
        {
            int result = 1;
            switch (index)
            {
                case 1:
                    result = Tools.CheckRange(5 + (lunarDate.Month - 1), 1, 12);
                    break;
                case 2:
                    result = Tools.CheckRange(11 - (lunarDate.Month - 1), 1, 12);
                    break;
                case 3:
                    result = Tools.CheckRange(10 + (lunarDate.Month - 1), 1, 12);
                    break;
                case 4:
                    result = Tools.CheckRange(2 + (lunarDate.Month - 1), 1, 12);
                    break;
                case 5:
                    result = new int[] { 9, 6, 3, 12, 9, 6, 3, 12, 9, 6, 3, 12 }[lunarDate.Month - 1];
                    break;
                case 6:
                    result = new int[] { 9, 9, 11, 11, 1, 1, 3, 3, 5, 5, 7, 7 }[lunarDate.Month - 1];
                    break;
                case 7:
                    result = new int[] { 6, 9, 3, 12, 6, 9, 3, 12, 6, 9, 3, 12 }[lunarDate.Month - 1];
                    break;
                case 8:
                    result = new int[] { 11, 6, 5, 3, 8, 4, 12, 8, 3, 7, 11, 3 }[lunarDate.Month - 1];
                    break;
                case 9:
                    result = new int[] { 3, 1, 11, 9, 7, 5, 3, 1, 11, 9, 7, 5 }[lunarDate.Month - 1];
                    break;
            }
            return result;
        }

        /// <summary>
        /// 安日系諸星 EC
        /// </summary>
        /// <param name="lunarDate">農曆日</param>
        /// <param name="index">序號 1:三台 2:八座 3:恩光 4:天貴</param>
        /// <returns></returns>
        private int GetEC(LunarDate lunarDate, int index)
        {
            int result = 1;
            switch (index)
            {
                case 1:
                    result = Tools.CheckRange(GetDM(lunarDate, 1) + (lunarDate.Day - 1), 1, 12);
                    break;
                case 2:
                    result = Tools.CheckRange(GetDM(lunarDate, 2) - (lunarDate.Day - 1), 1, 12);
                    break;
                case 3:
                    result = Tools.CheckRange(GetCY(lunarDate, 1) + (lunarDate.Day - 2), 1, 12);
                    break;
                case 4:
                    result = Tools.CheckRange(GetCY(lunarDate, 2) + (lunarDate.Day - 2), 1, 12);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 安干系諸星 FX
        /// </summary>
        /// <param name="lunarDate">農曆年干</param>
        /// <param name="index">序號 1:祿存 2:羊刃 3:陀羅 4:天魁 5:天鉞 6:化祿 7:化權 8:化科 9:化忌 10:天官 11:天福</param>
        /// <returns></returns>
        private int GetFX(LunarDate lunarDate, int index)
        {
            int result = 1;
            string star = string.Empty;
            switch (index)
            {
                case 1:
                    result = new int[] { 3, 4, 6, 7, 6, 7, 9, 10, 12, 1 }[lunarDate.YearHS - 1];
                    break;
                case 2:
                    result = new int[] { 4, 5, 7, 8, 7, 8, 10, 11, 1, 2 }[lunarDate.YearHS - 1];
                    break;
                case 3:
                    result = new int[] { 2, 3, 5, 6, 5, 6, 8, 9, 11, 12 }[lunarDate.YearHS - 1];
                    break;
                case 4:
                    result = new int[] { 2, 1, 12, 12, 2, 1, 2, 7, 4, 4 }[lunarDate.YearHS - 1];
                    break;
                case 5:
                    result = new int[] { 8, 9, 10, 10, 8, 9, 8, 3, 6, 6 }[lunarDate.YearHS - 1];
                    break;
                case 6:
                    star = new string[] { "AY06", "AY02", "AY05", "BY02", "BY03", "AY04", "AY03", "BY04", "BY06", "BY08" }[lunarDate.YearHS - 1];
                    break;
                case 7:
                    star = new string[] { "BY08", "BY06", "AY02", "AY05", "BY02", "BY03", "AY04", "AY02", "AY01", "BY04" }[lunarDate.YearHS - 1];
                    break;
                case 8:
                    star = new string[] { "AY04", "AY01", "CY01", "AY02", "DM02", "BY06", "BY02", "CY02", "DM01", "BY02" }[lunarDate.YearHS - 1];
                    break;
                case 9:
                    star = new string[] { "AY03", "BY02", "AY06", "BY04", "AY02", "CY01", "AY05", "CY02", "AY04", "BY03" }[lunarDate.YearHS - 1];
                    break;
                case 10:
                    result = new int[] { 8, 5, 6, 3, 4, 10, 12, 10, 11, 7 }[lunarDate.YearHS - 1];
                    break;
                case 11:
                    result = new int[] { 10, 9, 1, 12, 4, 3, 7, 6, 7, 6 }[lunarDate.YearHS - 1];
                    break;
            }
            if (new int[] { 6, 7, 8, 9 }.Contains(index))
            {
                switch (star.Substring(0, 2))
                {
                    case "AY":
                        result = int.Parse(star.Substring(2, 2)) == 1 ? ZiweiEB : GetAY(ZiweiEB, int.Parse(star.Substring(2, 2)));
                        break;
                    case "BY":
                        result = int.Parse(star.Substring(2, 2)) == 1 ? BY01EB : GetBY(BY01EB, int.Parse(star.Substring(2, 2)));
                        break;
                    case "CY":
                        result = GetCY(lunarDate, int.Parse(star.Substring(2, 2)));
                        break;
                    case "DM":
                        result = GetDM(lunarDate, int.Parse(star.Substring(2, 2)));
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 安支系諸星 FY
        /// </summary>
        /// <param name="lunarDate">農曆年支</param>
        /// <param name="index">序號 1:天哭 2:天虛 3:龍池 4:鳳閣 5:紅鸞 6:天喜 7:孤辰 8:寡宿 9:蜚廉 10:破碎 11:天才 12:天壽</param>
        /// <returns></returns>
        private int GetFY(LunarDate lunarDate, int index)
        {
            int result = 1;
            switch (index)
            {
                case 1:
                    result = Tools.CheckRange(7 - (lunarDate.YearEB - 1), 1, 12);
                    break;
                case 2:
                    result = Tools.CheckRange(7 + (lunarDate.YearEB - 1), 1, 12);
                    break;
                case 3:
                    result = Tools.CheckRange(5 + (lunarDate.YearEB - 1), 1, 12);
                    break;
                case 4:
                    result = Tools.CheckRange(11 - (lunarDate.YearEB - 1), 1, 12);
                    break;
                case 5:
                    result = Tools.CheckRange(4 - (lunarDate.YearEB - 1), 1, 12);
                    break;
                case 6:
                    result = Tools.CheckRange(10 - (lunarDate.YearEB - 1), 1, 12);
                    break;
                case 7:
                    result = new int[] { 3, 3, 6, 6, 6, 9, 9, 9, 12, 12, 12, 3 }[lunarDate.YearEB - 1];
                    break;
                case 8:
                    result = new int[] { 11, 11, 2, 2, 2, 5, 5, 5, 8, 8, 8, 11 }[lunarDate.YearEB - 1];
                    break;
                case 9:
                    result = new int[] { 9, 10, 11, 6, 7, 8, 3, 4, 5, 12, 1, 2 }[lunarDate.YearEB - 1];
                    break;
                case 10:
                    result = new int[] { 6, 2, 10, 6, 2, 10, 6, 2, 10, 6, 2, 10 }[lunarDate.YearEB - 1];
                    break;
                case 11:
                    result = Tools.CheckRange(FateEB + (lunarDate.YearEB - 1), 1, 12);
                    break;
                case 12:
                    result = Tools.CheckRange(BodyEB + (lunarDate.YearEB - 1), 1, 12);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 長生十二神(五形)
        /// </summary>
        /// <param name="str5Element">五形局</param>
        /// <param name="gender">性別</param>
        /// <param name="index">序號 1:長生 2:沐浴 3:冠帶 4:臨官 5:帝旺 6:衰 7:病 8:死 9:墓 10:絕 11:胎 12:養</param>
        /// <returns></returns>
        private int GetME(int str5Element, Gender gender, int index)
        {
            return str5Element switch
            {
                2 => Tools.CheckRange(new Gender[] { Gender.MalePlus, Gender.FemalMinuse }.Contains(gender) ? 9 + (index - 1) : 9 - (index - 1), 1, 12),
                3 => Tools.CheckRange(new Gender[] { Gender.MalePlus, Gender.FemalMinuse }.Contains(gender) ? 12 + (index - 1) : 12 - (index - 1), 1, 12),
                4 => Tools.CheckRange(new Gender[] { Gender.MalePlus, Gender.FemalMinuse }.Contains(gender) ? 6 + (index - 1) : 6 - (index - 1), 1, 12),
                5 => Tools.CheckRange(new Gender[] { Gender.MalePlus, Gender.FemalMinuse }.Contains(gender) ? 9 + (index - 1) : 9 - (index - 1), 1, 12),
                _ => Tools.CheckRange(new Gender[] { Gender.MalePlus, Gender.FemalMinuse }.Contains(gender) ? 3 + (index - 1) : 3 - (index - 1), 1, 12),
            };
        }

        /// <summary>
        /// 流年歲前諸星
        /// </summary>
        /// <param name="lunardate">農曆年支</param>
        /// <param name="index">序號 1:歲建 2:晦氣 3:喪門 4:貫索 5:官符 6:小秏 7:大秏 8:龍德 9:白虎 10:天德 11:弔客 12:病符</param>
        /// <returns></returns>
        private int GetQY(LunarDate lunardate, int index)
        {
            return Tools.CheckRange(lunardate.YearEB + (index - 1), 1, 12);
        }

        /// <summary>
        /// 流年將前諸星
        /// </summary>
        /// <param name="lunardate">農曆年支</param>
        /// <param name="index">序號 1:將星 2:攀鞍 3:歲驛 4:息神 5:華蓋 6:劫煞 7:災煞 8:天煞 9:指背 10:咸池 11:月煞 12:亡神</param>
        /// <returns></returns>
        private int GetRY(LunarDate lunardate, int index)
        {
            int result = 0;
            if (new int[] { 3, 7, 11 }.Contains(lunardate.YearEB)) { result = 7 + (index - 1); }
            if (new int[] { 9, 1, 5 }.Contains(lunardate.YearEB)) { result = 1 + (index - 1); }
            if (new int[] { 6, 10, 2 }.Contains(lunardate.YearEB)) { result = 10 + (index - 1); }
            if (new int[] { 12, 4, 8 }.Contains(lunardate.YearEB)) { result = 4 + (index - 1); }
            return Tools.CheckRange(result, 1, 12);
        }
    }
}
