using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DerbyRequest
{
    public static class Constants
    {
        private static Random rnd = new Random(DateTime.Now.Millisecond);
        public static string Path_Base = @"Derby";
        public static string Path_AiSyo = @"\Lib\" + Path_Base + @"\AiSyo.png";

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
