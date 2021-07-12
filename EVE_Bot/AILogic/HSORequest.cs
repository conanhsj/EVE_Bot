using EVE_Bot.JsonSetu;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EVE_Bot.AILogic
{
    public class HSORequest
    {
        public static JOResponse GetSetu()
        {
             string serverUrl = "https://api.lolicon.app/setu/";

            var reqResponse = (HttpWebRequest)WebRequest.Create(serverUrl);

            using (WebResponse response = reqResponse.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strJson = sr.ReadToEnd();

                return JsonConvert.DeserializeObject<JOResponse>(strJson); ;
            }
        }
    }
}
