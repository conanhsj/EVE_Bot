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
    }
}
