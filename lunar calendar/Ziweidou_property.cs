using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace lunar_calendar
{
    [Serializable]
    /// <summary>
    /// 星座結構(stuStar)
    /// </summary>
    public struct StuStar
    {
        public string StrLocate { get; set; }       //所在宮名
        public string StrStarId { get; set; }       //星ID
        public string StrNameShow { get; set; }     //星名顯示
        public Brush BrushColor { get; set; }       //背景顏色

    }
    public partial class Ziweidou
    {
        /// <summary>
        /// 命重表
        /// </summary>
        private Dictionary<string, double> DicWeight { get; set; }  //命重表

        /// <summary>
        /// 星座位宮表
        /// </summary>
        private Dictionary<string, string> DicStarLocation { get; set; }  //星座位宮表

        /// <summary>
        /// 星座強弱表
        /// </summary>
        private Dictionary<string, string> DicStarStrength { get; set; }  //星座強弱表

        /// <summary>
        /// 眾星
        /// </summary>
        private Dictionary<string, StuStar> DicStar
        {
            get { if (dicStar == null) { dicStar = new Dictionary<string, StuStar>(); } return dicStar; }
        }

        /// <summary>
        /// 12 宮位
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> Dic12Location
        {
            get { if (dic12Location == null) { dic12Location = new Dictionary<string, Dictionary<string, string>>(); } return dic12Location; }
        }

        /// <summary>
        /// 12 宮位(顯示用)
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> Dic12LocationShow
        {
            get { if (dic12LocationShow == null) { dic12LocationShow = new Dictionary<string, Dictionary<string, string>>(); } return dic12LocationShow; }
        }

        /// <summary>
        /// 更新模式 資料
        /// </summary>
        private Dictionary<string, string> DicUpdateData
        {
            get { if (dicUpdateData == null) { dicUpdateData = new Dictionary<string, string>(); } return dicUpdateData; }
        } //更新模式 資料

        /// <summary>
        /// 性別
        /// </summary>
        private Lunar.Gender IntGender { get; set; }       //性別

        #region 西元日期

        /// <summary>
        /// 西元年
        /// </summary>
        private string StrWYear { get; set; }        //西元年

        /// <summary>
        /// 西元月
        /// </summary>
        private string StrWMonth { get; set; }       //西元月

        /// <summary>
        /// 西元日
        /// </summary>
        /// 
        private string StrWDay { get; set; }         //西元日

        #endregion 西元日期

        #region 陰曆日期 

        /// <summary>
        /// 陰曆年
        /// </summary>
        private string StrCYear { get; set; }        //陰曆年

        /// <summary>
        /// 陰曆月
        /// </summary>
        private string StrCMonth { get; set; }       //陰曆月

        /// <summary>
        /// 陰曆日
        /// </summary>
        private string StrCDay { get; set; }         //陰曆日

        #endregion 陰曆日期 

        /// <summary>
        /// 時
        /// </summary>
        private string StrWHour { get; set; }        //時

        /// <summary>
        /// 星期
        /// </summary>
        private DayOfWeek DowWeekend { get; set; }   //星期

        #region 四柱

        /// <summary>
        /// 四柱年干
        /// </summary>
        public string StrYearX1 { get; set; }       //四柱年干

        /// <summary>
        /// 四柱年支
        /// </summary>
        public string StrYearY1 { get; set; }       //四柱年支

        /// <summary>
        /// 四柱月干
        /// </summary>
        public string StrMonthX1 { get; set; }      //四柱月干

        /// <summary>
        /// 四柱月支
        /// </summary>
        public string StrMonthY1 { get; set; }      //四柱月支

        /// <summary>
        /// 四柱日干
        /// </summary>
        public string StrDayX1 { get; set; }        //四柱日干

        /// <summary>
        /// 四柱日支
        /// </summary>
        public string StrDayY1 { get; set; }        //四柱日支

        /// <summary>
        /// 四柱時干
        /// </summary>
        public string StrHourX1 { get; set; }       //四柱時干

        /// <summary>
        /// 四柱時支
        /// </summary>
        public string StrHourY1 { get; set; }       //四柱時支

        #endregion 四柱

        /// <summary>
        /// 速算年干
        /// </summary>
        private string StrfYearX1 { get; set; }      //速算年干

        /// <summary>
        /// 速算年支
        /// </summary>
        private string StrfYearY1 { get; set; }      //速算年支

        /// <summary>
        /// 速算月干
        /// </summary>
        private string StrfMonthX1 { get; set; }     //速算月干

        /// <summary>
        /// 速算月支
        /// </summary>
        private string StrfMonthY1 { get; set; }     //速算月支

        /// <summary>
        /// 速算日干
        /// </summary>
        private string StrfDayX1 { get; set; }       //速算日干

        /// <summary>
        /// 速算日支
        /// </summary>
        private string StrfDayY1 { get; set; }       //速算日支

        /// <summary>
        /// 速算時干
        /// </summary>
        private string StrfHourX1 { get; set; }      //速算時干

        /// <summary>
        /// 速算時支
        /// </summary>
        private string StrfHourY1 { get; set; }      //速算時支

        /// <summary>
        /// 命宮天干
        /// </summary>
        private string StrFateX1 { get; set; }       //命宮天干

        /// <summary>
        /// 命宮地支
        /// </summary>
        private string StrFateY1 { get; set; }       //命宮地支

        /// <summary>
        /// 命主
        /// </summary>
        private string StrFateStar { get; set; }     //命主

        /// <summary>
        /// 身宮地支
        /// </summary>
        private string StrBodyY1 { get; set; }       //身宮地支

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

    }
}
