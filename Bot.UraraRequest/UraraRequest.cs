using Bot.ExtendInterface;
using Bot.UraraRequest.JsonObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bot.UraraRequest
{
    [Export("UraraRequest", typeof(IMessageRequest))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UraraRequest : IMessageRequest
    {
        public List<Urara> lstResult =  new List<Urara>();

        Random rnd = new Random();

        public UraraRequest()
        {
            ReadFile();
        }

        private void ReadFile()
        {
            string strExecutePath = Assembly.GetEntryAssembly().Location;
            string strBaseFolder = Path.GetDirectoryName(strExecutePath);

            string strContents;
            using (var sr = new StreamReader(strBaseFolder + Constants.Path_Result, Encoding.UTF8))
            {
                strContents = sr.ReadToEnd();
            }
            lstResult = JsonConvert.DeserializeObject<List<Urara>>(strContents);
        }

        string IMessageRequest.DealGroupRequest(JORecvGroupMsg jsonGrpMsg)
        {
            if (lstResult == null)
            {
                lstResult = new List<Urara>();
            }

            return ReadAsakusa(jsonGrpMsg);
        }

        string IMessageRequest.DealPrivateRequest(JORecvGroupMsg jsonGrpMsg)
        {
            if (lstResult == null)
            {
                lstResult = new List<Urara>();
            }

            return ReadAsakusa(jsonGrpMsg);
        }


        public string ReadAsakusa(JORecvGroupMsg jsonGrpMsg)
        {
            //请求
            string strReqPath = string.Format("https://www.zgjm.net/chouqian/guanyinlingqian/{0}.html", rnd.Next(101));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {

                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                //JsonTextReader jsonReader = new JsonTextReader(sr);
                string strJson = sr.ReadToEnd();

                strJson = WebUtility.HtmlDecode(strJson);
                //strJson = strJson.Trim('[', ']');
                //XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(strJson);
                HtmlAgilityPack.HtmlDocument xmlDoc = new HtmlAgilityPack.HtmlDocument();

                xmlDoc.LoadHtml(strJson);
                List<HtmlAgilityPack.HtmlNode> lstNode = xmlDoc.DocumentNode.SelectNodes("//article").ToList();
                if (lstNode.Count > 0)
                {
                    string strResult = lstNode[0].InnerText;
                    strResult = strResult.Replace('\t', '\n');
                    strResult = strResult.Substring(0, strResult.IndexOf("〖四句解说〗"));
                    strResult = strResult.Trim('\n', '\r');
                    strResult += "\n\n" + "全文地址：" + strReqPath;

                    return strResult;
                }
            }

            return "";
        }

    }
}
