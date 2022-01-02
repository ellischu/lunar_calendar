using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lunar_calendar
{
    [Serializable]
    /// <summary>
    ///  MalePlus = 陽男, FemalMinuse = 陰女, MaleMinus = 陰男, FemalePlus = 陽女
    /// </summary>
    public enum Gender : int
    {
        None = 0, MalePlus = 1, FemalMinuse = 2, MaleMinus = 3, FemalePlus = 4
    }

    public enum ConverOption
    {
        IDtoName = 1, NametoID = 2
    }

    /// <summary>
    /// Terms : 節 , Qi : 氣
    /// </summary>
    public enum SolarTerms
    {
        Terms = 1, Qi = 2
    }

}
