using EVE_Bot.Helper;
using EVE_Bot.Interface;
using EVE_Bot.JsonGame;
using EVE_Bot.JsonObject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EVE_Bot.AILogic
{
    [Export("UraraRequest", typeof(IMessageRequest))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UraraRequest : IMessageRequest
    {
        public List<Urara> lstResult = JsonConvert.DeserializeObject<List<Urara>>(FilesHelper.ReadJsonFile("Urara\\Result"));

        Random rnd = new Random();
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
            throw new NotImplementedException();
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
                    strResult = strResult.Trim('\n', '\r');
                    strResult += "\n\n" + "全文地址：" + strReqPath;

                    return strResult;
                }
            }

            return "";
        }

    }
}
