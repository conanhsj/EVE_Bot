using Bot.AIRequest.JsonObjects;
using Bot.AIRequest.Requests;
using Bot.ExtendInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Bot.AIRequest
{
    [Export("AIRequest", typeof(IMessageRequest))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AIRequest : IMessageRequest
    {

        public static List<Words> lstWords = new List<Words>();
        public static List<WaitAnswer> lstWaitAnswer = new List<WaitAnswer>();

        private System.Timers.Timer timerThinking = new System.Timers.Timer();

        private static string strThinkResult = string.Empty;

        public AIRequest()
        {
            CreateScheduleTimer();

            ReadWords();
        }

        private static void ReadWords()
        {
            string strExecutePath = Assembly.GetEntryAssembly().Location;
            string strBaseFolder = Path.GetDirectoryName(strExecutePath);

            string strContents;
            using (var sr = new StreamReader(strBaseFolder + Constants.Path_Words, Encoding.UTF8))
            {
                strContents = sr.ReadToEnd();
            }
            lstWords = JsonConvert.DeserializeObject<List<Words>>(strContents);
        }

        private void CreateScheduleTimer()
        {
            timerThinking.Elapsed += DoThinking;
            timerThinking.Interval = 60 * 1000;
            timerThinking.AutoReset = true;
            timerThinking.Start();
        }

        private void DoThinking(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Second != 0)
            {
                timerThinking.Interval = (60 - DateTime.Now.Second) * 1000;
            }
            else
            {
                timerThinking.Interval = 60 * 1000;
            }

            ReadWords();

            List<Words> lstParts = lstWords.FindAll(Words => Words.Type == "Adjs");

            List<string> lstDirty = lstWords.FindAll(Words => Words.Means == "脏话").Select(Value => Value.Word).ToList();

            List<Words> lstFilter = new List<Words>();

            foreach (Words Adjs in lstParts)
            {
                if (lstDirty.Where(dirty => { return Adjs.Word.Contains(dirty); }).ToList().Count > 0)
                {
                    lstFilter.Add(Adjs);
                }
            }


            strThinkResult = "";
            if (lstFilter.Count > 0)
            {
                string strValue = string.Empty;
                foreach (Words wrds in lstFilter)
                {
                    strValue += wrds.Word + ",";
                }
                strThinkResult += "脏形容词：" + strValue;
            }

            strThinkResult = "形容词:" + lstParts.Count + " 脏话：" + lstDirty.Count + "\n" + strThinkResult;


        }

        string IMessageRequest.DealGroupRequest(JORecvGroupMsg jsonGrpMsg)
        {
            return DealMessage(jsonGrpMsg);
        }

        string IMessageRequest.DealPrivateRequest(JORecvGroupMsg jsonGrpMsg)
        {
            return DealMessage(jsonGrpMsg);
        }

        private string DealMessage(JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;
            string strInput = jsonGrpMsg.message;

            bool bIsAtMe = false;

            while (Commons.regCQCode.IsMatch(strInput))
            {
                if (jsonGrpMsg.message.Contains("[CQ:at,qq=" + jsonGrpMsg.self_id.ToString()))
                {
                    bIsAtMe = true;
                }
                if (jsonGrpMsg.message.Contains("[CQ:json,data=" + jsonGrpMsg.self_id.ToString()))
                {
                    bIsAtMe = true;
                }
                if (jsonGrpMsg.message.Contains("[CQ:image,file=" + jsonGrpMsg.self_id.ToString()))
                {
                    bIsAtMe = true;
                }
                strInput = Commons.regCQCode.Replace(strInput, "");
            }

            if (strInput.Contains("改为"))
            {
                string[] value = strInput.Split(' ');
                if (value.Length == 3 && value[1] == "改为")
                {
                    Words wd = lstWords.Find(X => X.Word == value[0]);
                    if (wd != null)
                    {
                        wd.Word = value[2];
                        OutputWords(JsonConvert.SerializeObject(lstWords, Formatting.Indented));
                        strMessage += "好的，改完了";
                    }
                    else
                    {
                        strMessage += "没找到词啊？";
                    }
                }
            }

            if (!string.IsNullOrEmpty(strThinkResult))
            {
                strMessage += strThinkResult + "\n";
                strThinkResult = string.Empty;
            }

            if (strInput.StartsWith("长度统计"))
            {
                TodayDick.ReadDicks();
                foreach (string strKey in TodayDick.dicTodayDick.Keys)
                {
                    strMessage += strKey + "的长度统计为\n";
                    Dictionary<string, string> dicDick = TodayDick.dicTodayDick[strKey];
                    foreach (string strTar in dicDick.Keys)
                    {
                        strMessage += strTar + "今日的长度为" + dicDick[strTar] + "\n";
                    }
                }
            }
            else
            {
                strMessage += lstWords[Commons.rnd.Next(lstWords.Count)].Word + " 是正确的么？";
            }


            return strMessage;

        }


        public static void OutputWords(string strContent)
        {
            string strExecutePath = Assembly.GetEntryAssembly().Location;
            string strBaseFolder = Path.GetDirectoryName(strExecutePath);
            string strBasePath = strBaseFolder + Constants.Path_Words;
            if (!Directory.Exists(Path.GetDirectoryName(strBasePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(strBasePath));
            }
            using (var sw = new StreamWriter(strBasePath, false, Encoding.Unicode))
            {
                sw.Write(strContent);
            }
            //Environment.
        }
    }
}
