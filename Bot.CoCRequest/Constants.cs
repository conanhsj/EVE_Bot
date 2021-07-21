using Bot.CoCRequest.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bot.CoCRequest
{
    public static class Constants
    {
        private static Random rnd = new Random(DateTime.Now.Millisecond);
        public static string Path_Base = @"CoC7th";
        public static string Path_Characters = @"\Lib\" + Path_Base + @"\Characters.json";

        public static Dictionary<string, string> dicKeywords = new Dictionary<string, string>()
        {
            { "力量","STR" },
            { "体质","CON" },
            { "体型","SIZ" },
            { "敏捷","DEX" },
            { "外貌","APP" },
            { "智力","INT" },
            { "意志","POW" },
            { "教育","EDU" },
            { "幸运","LKY" },
        };



        public static DiceResult RollDices(int Count, int MaxPoint)
        {
            DiceResult result = new DiceResult();
            string strValue = string.Empty;
            int nSum = 0;
            for (int n = 0; n < Count; n++)
            {
                int nResult = (rnd.Next(MaxPoint) + 1);
                strValue += nResult + "+";
                nSum += nResult;
            }

            result.Result = strValue.TrimEnd('+') + " = " + nSum;
            result.Point = nSum;

            return result;
        }

        //ReinforceCheckout
        public static DiceResult RollRC(int Base)
        {
            DiceResult dice = Constants.RollDices(1, 100);
            if (dice.Point > Base)
            {
                dice.Result = "成功,";
                DiceResult buff = Constants.RollDices(1, 10);
                dice.Result += "增加" + buff.Point + "点\n";
                dice.Point = buff.Point;
            }
            else
            {
                dice.Result = "失败\n";
                dice.Point = 0;
            }
            return dice;
        }

        public static void OutputJsonFile(string strFileName, string strContent)
        {
            string strExecutePath = Assembly.GetEntryAssembly().Location;
            string strBaseFolder = Path.GetDirectoryName(strExecutePath);

            if (!Directory.Exists(Path.GetDirectoryName(strBaseFolder + strFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(strBaseFolder + strFileName));
            }
            using (var sw = new StreamWriter(strBaseFolder + strFileName, false, Encoding.Unicode))
            {
                sw.Write(strContent);
            }
            //Environment.
        }

        public static int GetPropValue(object src, string propName)
        {
            object Prop = src.GetType().GetProperty(propName).GetValue(src, null);
            int Value = 0;
            int.TryParse(Prop.ToString(), out Value);

            return Value;
        }

        public static string CheckResult(int BaseStatus, int CheckResult)
        {
            string strResult = string.Empty;

            if (CheckResult == 1)
            {
                strResult = "大成功";
            }
            else if (CheckResult <= (BaseStatus / 5))
            {
                strResult = "极难成功";
            }
            else if (CheckResult <= (BaseStatus / 2))
            {
                strResult = "困难成功";
            }
            else if (CheckResult <= BaseStatus)
            {
                strResult = "检定成功";
            }
            else if (CheckResult > BaseStatus)
            {
                strResult = "检定失败";
            }

            return strResult;
        }
    }
}
