using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using EVE_Bot.JsonEVE;
using Newtonsoft.Json;

namespace EVE_Bot.EVEAPIs
{
    public static class CEVESwaggerInterface
    {
        public static List<Sovereignty> SovereigntyMap()
        {
            List<Sovereignty> lstResult = new List<Sovereignty>();

            //请求
            string strReqPath = string.Format("https://esi.evepc.163.com/latest/sovereignty/map/?datasource=serenity");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "GET";
            using (WebResponse response = request.GetResponse())
            {

                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strJson = sr.ReadToEnd();
                lstResult = JsonConvert.DeserializeObject<List<Sovereignty>>(strJson);

            }
            return lstResult;
        }
    }
}
