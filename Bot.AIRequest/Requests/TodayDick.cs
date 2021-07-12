using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bot.AIRequest.Requests
{
    public static class TodayDick
    {
        public static Dictionary<string, Dictionary<string, string>> dicTodayDick = new Dictionary<string, Dictionary<string, string>>();

        public static void ReadDicks()
        {
            string strExecutePath = Assembly.GetEntryAssembly().Location;
            string strBaseFolder = Path.GetDirectoryName(strExecutePath);

            string strContents;
            using (var sr = new StreamReader(strBaseFolder + Constants.Path_Dick, Encoding.UTF8))
            {
                strContents = sr.ReadToEnd();
            }
            dicTodayDick = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(strContents);
        }
    }
}
