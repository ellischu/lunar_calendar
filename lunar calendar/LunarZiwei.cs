using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lunar_calendar
{
    //紫微星系 ID 對照表


    //SY01      旬空
    //SY02      截空
    //SY03      天傷
    //SY04      天使
    //PY01-PY07 十二宮廟旺地利平閑陷

    public partial class LunarZiwei
    {
        private Dictionary<string, List<string>> dicLNStart { get; set; }

        /// <summary>
        /// 農曆
        /// </summary>
        public LunarDate Lunardate { get; private set; }
        /// <summary>
        /// 性別 (陽男,陰女,陰男,陽女)
        /// </summary>
        public Gender IGender { get; private set; }
        /// <summary>
        /// 八字
        /// </summary>
        public Lunar8Characters EightCharacters { get; set; }
        /// <summary>
        /// 12宮
        /// </summary>
        public Dictionary<string, List<string>> DicLNStart => dicLNStart;

        /// <summary>
        /// 命宮天干
        /// </summary>
        private int FateHS { get; set; }
        /// <summary>
        /// 命宮地支
        /// </summary>
        private int FateEB { get; set; }

        /// <summary>
        /// 身宮地支
        /// </summary>
        private int BodyEB { get; set; }

        /// <summary>
        /// 五形局
        /// </summary>
        private string Str5Element { get; set; }

        /// <summary>
        /// 紫微地支
        /// </summary>
        private int ZiweiEB { get; set; }
        /// <summary>
        /// 天府地支
        /// </summary>
        private int BY01EB { get; set; }

        public LunarZiwei(DateTime dateTime, Gender Gender = Gender.MalePlus)
        {
            //農曆
            //XX01-XX10 天干  YY01-YY12 地支
            Lunardate = new LunarDate(dateTime);
            //性別
            IGender = CheckGender(Gender, Lunardate.YearEB);
            //八字
            EightCharacters = new Lunar8Characters(dateTime);
            //命宮地支
            FateEB = GetFateEB(Lunardate);
            //命宮天干
            FateHS = GetFateHS(Lunardate, FateEB);
            //身宮地支
            BodyEB = GetBodyEB(Lunardate);
            //定五形局


            //NE02-NE06 五形局(六十花甲)
            //Str5Element = $"NE{GetNE(EightCharacters.YearHS, FateEB):00}";
            Str5Element = new Lunar60Flower(FateHS, FateEB).StrElement5;

            //------------------------------------------------------------------------
            //設12宮 LN01- LN12
            //LN01 本命宮      LN02 父母宮        LN03 福德宮        LN04 田宅宮
            //LN05 官錄宮      LN06 奴僕宮        LN07 遷移宮        LN08 疾厄宮
            //LN09 財帛宮      LN10 子女宮        LN11 夫妻宮        LN12 兄弟宮
            dicLNStart = SetDicLNStart(FateHS, FateEB);
            //------------------------------------------------------------------------
            //安紫微諸星 AY 甲級
            //AY01-AY06 紫微諸星(五形,日)
            //AY01紫微(含廟旺)       AY02天機(含廟旺)     AY03太陽(含廟旺)     AY04武曲(含廟旺)
            //AY05天同(含廟旺)       AY06廉貞(含廟旺)
            ZiweiEB = GetZiweiEB(Str5Element, Lunardate);
            SetStart(ZiweiEB, "AY01");
            for (int index = 2; index <= 6; index++)
            {
                SetStart(GetAY(ZiweiEB, index), $"AY{index:00}");
            }
            //------------------------------------------------------------------------
            //安天府諸星 BY 甲級
            //BY01-BY08 天府諸星(紫微)
            //BY01天府(含廟旺)       BY02太陰(含廟旺)     BY03貪狼(含廟旺)     BY04巨門(含廟旺)
            //BY05天相(含廟旺)       BY06天樑(含廟旺)     BY07七殺(含廟旺)     BY08破軍(含廟旺)
            BY01EB = GetBY01EB(ZiweiEB);
            SetStart(BY01EB, "BY01");
            for (int index = 2; index <= 8; index++)
            {
                SetStart(GetBY(BY01EB, index), $"BY{index:00}");
            }
            //------------------------------------------------------------------------
            //安時系諸星 CY
            //CY01-CY08 時系諸星(年干,時支)
            //CY01文昌(含廟旺)甲       CY02文曲(含廟旺)甲     CY03火星(含廟旺)甲     CY04鈴星(含廟旺)甲
            //CY05地劫(含廟旺)乙       CY06天空(含廟旺)乙     CY07台輔(含廟旺)乙     CY08封誥(含廟旺)乙
            for (int index = 1; index <= 8; index++)
            {
                SetStart(GetCY(Lunardate, index), $"CY{index:00}");
            }
            //------------------------------------------------------------------------
            //安月系諸星 DM
            //DM01-DM09 月系諸星(月)
            //DM01左輔(含廟旺)甲       DM02右弼(含廟旺)甲     DM03天刑(含廟旺)甲     DM04天姚(含廟旺)甲
            //DM05天馬(含廟旺)乙       DM06解神(含廟旺)乙     DM07天巫(含廟旺)乙     DM08天月(含廟旺)乙
            //DM09陰煞(含廟旺)乙
            for (int index = 1; index <= 9; index++)
            {
                SetStart(GetDM(Lunardate, index), $"DM{index:00}");
            }
            //------------------------------------------------------------------------
            //安日系諸星 EC
            //EC01-EC04 日系諸星
            //EC01三台(含廟旺)乙       EC02八座(含廟旺)乙     EC03恩光(含廟旺)乙     EC04天貴(含廟旺)乙
            for (int index = 1; index <= 4; index++)
            {
                SetStart(GetEC(Lunardate, index), $"EC{index:00}");
            }

            //------------------------------------------------------------------------
            //FX01-FX11 干系諸星(年干)
            //FX01祿存(含廟旺)甲       FX02羊刃(含廟旺)甲     FX03陀羅(含廟旺)甲     FX04天魁(含廟旺)甲
            //FX05天鉞(含廟旺)甲       FX06化祿(含廟旺)甲     FX07化權(含廟旺)甲     FX08化科(含廟旺)甲
            //FX09化忌(含廟旺)甲       FX10天官(含廟旺)乙     FX11天福(含廟旺)乙
            for (int index = 1; index <= 11; index++)
            {
                SetStart(GetFX(Lunardate, index), $"FX{index:00}");
            }
            //------------------------------------------------------------------------
            //OB01-OB12 博士十二神(祿存)
            //OB01博士(含廟旺)       OB02力士(含廟旺)     OB03青龍(含廟旺)     OB04小耗(含廟旺)
            //OB05將軍(含廟旺)       OB06奏書(含廟旺)     OB07飛廉(含廟旺)     OB08喜神(含廟旺)
            //OB09病符(含廟旺)       OB10大耗(含廟旺)     OB11伏兵(含廟旺)     OB12官府(含廟旺)
            for (int index = 1; index <= 12; index++)
            {
                switch (IGender)
                {
                    case Gender.MalePlus:
                    case Gender.FemalMinuse:
                        SetStart(GetFX(Lunardate, 1) + (index - 1), $"OB{index:00}");
                        break;
                    case Gender.MaleMinus:
                    case Gender.FemalePlus:
                        SetStart(GetFX(Lunardate, 1) - (index - 1), $"OB{index:00}");
                        break;
                }
            }
            //------------------------------------------------------------------------
            //FY01-FY11 支系諸星(年支)
            //FY01天哭(含廟旺)乙       FY02天虛(含廟旺)乙     FY03龍池(含廟旺)乙     FY04鳳閣(含廟旺)乙
            //FY05紅鸞(含廟旺)乙       FY06天喜(含廟旺)乙     FY07孤辰(含廟旺)乙     FY08寡宿(含廟旺)乙
            //FY09蜚廉(含廟旺)乙       FY10破碎(含廟旺)乙     FY11天才(含廟旺)乙     FY12天壽(含廟旺)乙
            for (int index = 1; index <= 12; index++)
            {
                SetStart(GetFY(Lunardate, index), $"FY{index:00}");
            }
            //------------------------------------------------------------------------
            //ME01-ME12 長生十二神(五形)(丙)
            //ME01長生(丙)  ME02沐浴(丙)  ME03冠帶(丙)  ME04臨官(丙)  ME05帝旺(丙)  ME06衰(丙) 
            //ME07病(丙)    ME08死(丙)    ME09墓(丙)    ME10絕(丙)    ME11胎(丙)    ME12養(丙)
            for (int index = 1; index <= 12; index++)
            {
                SetStart(GetME(int.Parse(Str5Element.Substring(2, 2)), IGender, index), $"ME{index:00}");
            }
            //------------------------------------------------------------------------
            //QY01-QY12 流年歲前諸星
            //QY01歲建(丁) QY02晦氣(戊) QY03喪門(戊) QY04貫索(戊) QY05官符(戊) QY06小秏(戊)
            //QY07大秏(戊) QY08龍德(丁) QY09白虎(戊) QY10天德(丁) QY11弔客(戊) QY12病符(戊)
            for (int index = 1; index <= 12; index++)
            {
                SetStart(GetQY(Lunardate, index), $"QY{index:00}");
            }
            //------------------------------------------------------------------------
            //RY01-RY12 流年將前諸星
            //RY01將星(丁) RY02攀鞍(丁) RY03歲驛(丁) RY04息神(戊) RY05華蓋(丁) RY06劫煞(戊)
            //RY07災煞(戊) RY08天煞(戊) RY09指背(戊) RY10咸池(戊) RY11月煞(戊) RY12亡神(戊)
            for (int index = 1; index <= 12; index++)
            {
                SetStart(GetRY(Lunardate, index), $"RY{index:00}");
            }

            SortStarOrder();
        }

        private int GetNE(int yearHS, int fateEB)
        {
            int result = 0;
            switch ((yearHS - 1) % 5)
            {
                case 0:
                    result = new int[] { 2, 6, 3, 5, 4, 6 }[(fateEB - 1) / 2];
                    break;
                case 1:
                    result = new int[] { 6, 5, 4, 3, 2, 5 }[(fateEB - 1) / 2];
                    break;
                case 2:
                    result = new int[] { 5, 3, 2, 4, 6, 3 }[(fateEB - 1) / 2];
                    break;
                case 3:
                    result = new int[] { 3, 4, 6, 2, 5, 4 }[(fateEB - 1) / 2];
                    break;
                case 4:
                    result = new int[] { 4, 2, 5, 6, 3, 2 }[(fateEB - 1) / 2];
                    break;
            }

            return result;
        }
    }
}
