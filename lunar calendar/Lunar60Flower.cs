using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lunar_calendar
{
    public class Lunar60Flower
    {
        // 甲子乙丑 海中金 白 此年生人，是海中金命，屬相是白鼠和白牛
        // 丙寅丁卯 爐中火 紅 此年生人，是爐中火命，屬相是紅虎和紅兔
        // 戊辰已巳 大林木 青 此年生人，是大林木命，屬相是青龍和青蛇
        // 庚午辛未 路旁土 黃 此年生人，是路旁土命，屬相是黃馬和黃羊
        // 壬申癸酉 劍鋒金 白 此年生人，是劍鋒金命，屬相是白猴和白雞
        // 甲戌乙亥 山頭火 紅 此年生人，是山頭火命，屬相是紅狗和紅豬
        // 丙子丁丑 澗下水 黑 此年生人，是澗下水命，屬相是黑鼠和黑牛
        // 戊寅已卯 城牆土 黃 此年生人，是城牆土命，屬相是黃虎和黃兔
        // 庚辰辛巳 白蠟金 白 此年生人，是白蠟金命，屬相是白龍和白蛇
        // 壬午癸未 楊柳木 青 此年生人，是楊柳木命，屬相是青馬和青羊
        // 甲申乙酉 泉中水 黑 此年生人，是泉中水命，屬相是黑猴和黑雞
        // 丙戌丁亥 屋上土 黃 此年生人，是屋上土命，屬相是黃狗和黃豬
        // 戊子已丑 霹雷火 紅 此年生人，是霹靂火命，屬相是紅鼠和紅牛
        // 庚寅辛卯 松柏木 青 此年生人，是松柏木命，屬相是青虎和青兔
        // 壬辰癸巳 長流水 黑 此年生人，是長流水命，屬相是黑龍和黑蛇
        // 甲午乙未 沙中金 白 此年生人，是沙中金命，屬相是白馬和白羊
        // 丙申丁酉 山下火 紅 此年生人，是山下火命，屬相是紅猴和紅雞
        // 戊戌已亥 平地木 青 此年生人，是平地木命，屬相是青狗和青豬
        // 庚子辛丑 壁上土 黃 此年生人，是壁上土命，屬相是黃鼠和黃牛
        // 壬寅癸卯 金箔金 白 此年生人，是金箔金命，屬相是白虎和白兔
        // 甲辰乙巳 佛燈火 紅 此年生人，是佛燈火命，屬相是紅龍和紅蛇
        // 丙午丁未 天河水 黑 此年生人，是天河水命，屬相是黑馬和黑羊
        // 戊申已酉 大驛木 青 此年生人，是大驛木命，屬相是青猴和青雞
        // 庚戌辛亥 釵釧金 白 此年生人，是釵釧金命，屬相是白狗和白豬
        // 壬子癸丑 桑松木 青 此年生人，是桑松木命，屬相是青鼠和青牛
        // 甲寅乙卯 大溪水 黑 此年生人，是大溪水命，屬相是黑虎和黑兔
        // 丙辰丁巳 沙中土 黃 此年生人，是沙中土命，屬相是黃龍和黃蛇
        // 戊午已未 天上火 紅 此年生人，是天上火命，屬相是紅馬和紅羊
        // 庚申辛酉 石榴木 青 此年生人，是石榴木命，屬相是青猴和青雞
        // 壬戌癸亥 大海水 黑 此年生人，是大海水命，屬相是黑狗和黑豬
        /// <summary>
        /// 六十花甲 01-30
        /// </summary>
        public int Flower { get; private set; }
        /// <summary>
        /// 六十花甲 NF01-NF30
        /// </summary>
        public string StrFlower => $"NF{Flower:00}";

        public int Element5 { get; private set; }

        public string StrElement5 => $"NE{Element5:00}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="HS">天干</param>
        /// <param name="EB">地支</param>
        public Lunar60Flower(int HS, int EB)
        {
            Flower = Get60Flower(HS, EB);
            Element5 = C60To5Element(Flower);
        }

        private static int Get60Flower(int HS, int EB)
        {
            int result = 0;
            switch (HS)
            {
                case 1:
                    result = new int[] { 1, 26, 21, 16, 11, 6 }[(EB - 1) / 2];
                    break;
                case 2:
                    result = new int[] { 1, 26, 21, 16, 11, 6 }[(EB - 1) / 2];
                    break;
                case 3:
                    result = new int[] { 7, 2, 27, 22, 17, 12 }[(EB - 1) / 2];
                    break;
                case 4:
                    result = new int[] { 7, 2, 27, 22, 17, 12 }[(EB - 1) / 2];
                    break;
                case 5:
                    result = new int[] { 13, 8, 3, 28, 23, 18 }[(EB - 1) / 2];
                    break;
                case 6:
                    result = new int[] { 13, 8, 3, 28, 23, 18 }[(EB - 1) / 2];
                    break;
                case 7:
                    result = new int[] { 19, 14, 9, 4, 29, 24 }[(EB - 1) / 2];
                    break;
                case 8:
                    result = new int[] { 19, 14, 9, 4, 29, 24 }[(EB - 1) / 2];
                    break;
                case 9:
                    result = new int[] { 25, 20, 15, 10, 5, 30 }[(EB - 1) / 2];
                    break;
                case 10:
                    result = new int[] { 25, 20, 15, 10, 5, 30 }[(EB - 1) / 2];
                    break;
            }
            return result;
        }

        private static int C60To5Element(int Flower)
        {
            int result = 0;
            if (new int[] { 7, 11, 15, 22, 26, 30 }.Contains(Flower)) result = 2;
            if (new int[] { 3, 10, 14, 18, 25, 29 }.Contains(Flower)) result = 3;
            if (new int[] { 1, 5, 9, 16, 20, 24 }.Contains(Flower)) result = 4;
            if (new int[] { 4, 8, 12, 19, 23, 27 }.Contains(Flower)) result = 5;
            if (new int[] { 2, 6, 13, 17, 21, 28 }.Contains(Flower)) result = 6;
            return result;
        }

    }
}
