using Bot.ExtendInterface;
using Bot.HsoRequest.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bot.HsoRequest
{
    [Export("HsoRequest", typeof(IMessageRequest))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HsoRequest : IMessageRequest
    {
        string IMessageRequest.DealGroupRequest(JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = jsonGrpMsg.message.Replace("\n", ""); ;
            string strReturn = string.Empty;

            while (Constants.regCQCode.IsMatch(strMessage))
            {
                strMessage = Constants.regCQCode.Replace(strMessage, "");
            }

            strReturn = DealCommon(jsonGrpMsg, strMessage);

            return strReturn;
        }

        string IMessageRequest.DealPrivateRequest(JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = jsonGrpMsg.message.Replace("\n", ""); ;
            string strReturn = string.Empty;

            while (Constants.regCQCode.IsMatch(strMessage))
            {
                strMessage = Constants.regCQCode.Replace(strMessage, "");
            }

            strReturn = DealCommon(jsonGrpMsg, strMessage);

            return strReturn;
        }

        private string DealCommon(JORecvGroupMsg jsonGrpMsg, string strMessage)
        {
            string strReturn = string.Empty;

            if(!strMessage.Contains("查找"))
            {
                strReturn = GetSetu();
            }
            else
            {
                strReturn = GetSetu();
            }


            return strReturn;
        }

        private string GetSetu()
        {
            string strReturn = string.Empty;
            string serverUrl = "https://api.lolicon.app/setu/";

            var reqResponse = (HttpWebRequest)WebRequest.Create(serverUrl);

            SetuResponse setu = null;
            using (WebResponse response = reqResponse.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strJson = sr.ReadToEnd();

                setu = JsonConvert.DeserializeObject<SetuResponse>(strJson); ;
            }


            if (setu.code == 0)
            {
                foreach (SetuData Image in setu.data)
                {
                    //strValue += "[CQ:image,file=" + Image.url + ",cache=1]";
                    strReturn += "作者：" + Image.author + " 标题：" + Image.title + "\n";
                    strReturn += Image.url;
                }
            }

            return strReturn;

        }
    }
}
