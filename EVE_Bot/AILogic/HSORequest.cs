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
            string serverUrl;

            Random randSource = new Random();
            if (randSource.Next(1, 100) > 50)
            {
                serverUrl = "https://api.lolicon.app/setu/";
                //apiKey = hso.LoliconApiKey ?? string.Empty;
            }
            else
            {
                serverUrl = "https://api.yukari.one/setu/";
                //apiKey = hso.YukariApiKey ?? string.Empty;
            }

            var reqResponse = (HttpWebRequest)WebRequest.Create(serverUrl);
            //reqResponse.
            //{
            //    Timeout = 3000,
            //    Params = new Dictionary<string, string>
            //        {
            //            {"apikey", apiKey}
            //        },
            //    isCheckSSLCert = hso.CheckSSLCert
            //});
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
