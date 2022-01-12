using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bot.EveRequest
{
    public static class Constants
    {
        public static Regex regCQCode = new Regex(@"(?:\[CQ:.*\])");

        public static string Menu_AddReward = @"设置悬赏";
        public static string Menu_DelReward = @"取消悬赏";
        public static string Menu_SystemKills = @"击杀排行";

        public static string Link_CheckSum = @"https://eve-static-data-export.s3-eu-west-1.amazonaws.com/tranquility/checksum";

        public static string Path_Base = @"EVE";
        public static string Path_Wormhole = @"\Lib\" + Path_Base + @"\Wormhole.json";
        public static string Path_Rewards = @"\Lib\" + Path_Base + @"\Rewards.json";
        public static string Path_Solar = @"\Lib\" + Path_Base + @"\UniverseSystem.json";
        public static string Path_SystemKills = @"\Lib\" + Path_Base + @"\SystemKills.json";


        public static string Path_BluePrint = @"\Lib\" + Path_Base + @"\BluePrint.json";
        public static string Path_typeIDs = @"\Lib\" + Path_Base + @"\typeIDs.yaml";
        public static string Path_Universe = @"\Lib\" + Path_Base + @"\UniverseSystem.json";


        public static string CheckAndReadPath(string strPath)
        {
            string strExecutePath = Assembly.GetEntryAssembly().Location;
            string strBaseFolder = Path.GetDirectoryName(strExecutePath);

            if (!Directory.Exists(Path.GetDirectoryName(strBaseFolder + strPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(strBaseFolder + strPath));
            }
            if (!File.Exists(strBaseFolder + strPath))
            {
                File.Create(strBaseFolder + strPath).Close();
            }
            string strContents;
            using (var sr = new StreamReader(strBaseFolder + strPath, Encoding.UTF8))
            {
                strContents = sr.ReadToEnd();
            }

            return strContents;
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
