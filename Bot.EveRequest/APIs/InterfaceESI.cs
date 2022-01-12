using Bot.EveRequest.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bot.EveRequest.APIs
{
    public static class InterfaceESI
    {
        public static List<SystemKills> Universe_SystemKills()
        {
            List<SystemKills> lstSystemKills = new List<SystemKills>();
            //请求
            string strReqPath = string.Format("https://esi.evepc.163.com/latest/universe/system_kills/?datasource=serenity");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                //JsonTextReader jsonReader = new JsonTextReader(sr);
                string strJson = sr.ReadToEnd();
                lstSystemKills = JsonConvert.DeserializeObject<List<SystemKills>>(strJson);
            }

            return lstSystemKills;
        }

    }
}
