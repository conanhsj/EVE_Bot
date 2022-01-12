using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bot.EveRequest.APIs
{
    public class InterfaceSDE
    {
        public string GetVersion()
        {
            string strVersion = string.Empty;

            //请求
            string strReqPath = string.Format(Constants.Link_CheckSum);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "GET";
            using (WebResponse response = request.GetResponse())
            {

                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strJson = sr.ReadToEnd();

            }


            return strVersion;
        }
    }
}
