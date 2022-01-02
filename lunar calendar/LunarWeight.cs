using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;
using System.Xml.Linq;

namespace lunar_calendar
{
    public class LunarWeight
    {
        public double Weight { get; private set; }
        public double YearWeight { get; private set; }
        public double MonthWeight { get; private set; }
        public double DayWeight { get; private set; }
        public double HourWeight { get; private set; }


        public LunarWeight(DateTime dateTime)
        {
            LunarDate lunarDate = new(dateTime);
            YearWeight = GetYearWeight(lunarDate);
            MonthWeight = new double[] { 0.6, 0.7, 1.8, 0.9, 0.5, 1.6, 0.9, 1.5, 1.8, 0.8, 0.9, 0.5 }[lunarDate.Month - 1];
            DayWeight = new double[] { 0.5, 1.0, 0.8, 1.5, 1.6, 1.5, 0.8, 1.6, 0.8, 1.6, 0.9, 1.7, 0.8, 1.7, 1.0, 0.8, 0.9, 1.8, 0.5, 1.5, 1.0, 0.9, 0.8, 0.9, 1.5, 1.8, 0.7, 0.8, 1.6, 0.6 }[lunarDate.Day - 1];
            HourWeight = new double[] { 1.6, 0.6, 0.7, 1.0, 0.9, 1.6, 1.0, 0.8, 0.8, 0.9, 0.6, 0.6 }[lunarDate.HourEB - 1];

            Weight = Math.Round(YearWeight + MonthWeight + DayWeight + HourWeight, 1);
        }

        private double GetYearWeight(LunarDate lunarDate)
        {
            double result = 1;
            switch (lunarDate.YearHS)
            {
                case 1:
                    result = new double[] { 1.2, 1.2, 0.8, 1.5, 0.5, 1.5 }[(lunarDate.YearEB - 1) / 2];
                    break;
                case 2:
                    result = new double[] { 0.9, 0.8, 0.7, 0.6, 1.5, 0.9 }[(lunarDate.YearEB - 1) / 2];
                    break;
                case 3:
                    result = new double[] { 1.6, 0.6, 0.8, 1.3, 0.5, 0.6 }[(lunarDate.YearEB - 1) / 2];
                    break;
                case 4:
                    result = new double[] { 0.8, 0.7, 0.6, 0.5, 1.4, 1.6 }[(lunarDate.YearEB - 1) / 2];
                    break;
                case 5:
                    result = new double[] { 1.5, 0.8, 1.2, 1.9, 1.4, 1.4 }[(lunarDate.YearEB - 1) / 2];
                    break;
                case 6:
                    result = new double[] { 0.7, 1.9, 0.5, 0.6, 0.5, 0.9 }[(lunarDate.YearEB - 1) / 2];
                    break;
                case 7:
                    result = new double[] { 0.7, 0.9, 1.2, 0.9, 0.8, 0.9 }[(lunarDate.YearEB - 1) / 2];
                    break;
                case 8:
                    result = new double[] { 0.7, 1.2, 0.6, 0.8, 1.6, 1.7 }[(lunarDate.YearEB - 1) / 2];
                    break;
                case 9:
                    result = new double[] { 0.5, 0.9, 1, 0.8, 0.7, 1 }[(lunarDate.YearEB - 1) / 2];
                    break;
                case 10:
                    result = new double[] { 0.7, 1.2, 0.7, 0.7, 0.8, 0.7 }[(lunarDate.YearEB - 1) / 2];
                    break;
            }
            return result;
        }


        /// <summary>
        /// 獲取命重表
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<string, double> ToDicWeight()      //獲取命重表 
        {
            using DataTable dtDataTable = Tools.ResourceToDataTable(Properties.Resources.LifeWeight);

            Dictionary<string, double> dicOutput = new();

            foreach (DataRow drRow in dtDataTable.Rows)
            {
                dicOutput.Add(drRow["strInput"].ToString(), double.Parse(drRow["shtWeight"].ToString(), CultureInfo.InvariantCulture));
            }
            return dicOutput;
        }
    }
}
