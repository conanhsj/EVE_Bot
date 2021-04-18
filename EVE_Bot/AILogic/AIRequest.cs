using EVE_Bot.Helper;
using EVE_Bot.JsonObject;
using EVE_Bot.JsonSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EVE_Bot.AILogic
{
    public class AIRequest
    {
        public static List<JsonWords> lstWords = JsonConvert.DeserializeObject<List<JsonWords>>(FilesHelper.ReadJsonFile("Talking\\Words"));
        public static List<string> lstPronoun = new List<string>();

        public static List<string> lstMe = new List<string>() { "猫娘", "这猫", "机器人" };
        public static List<string> lstPrep = new List<string>() { "的", "是", "了" };
        public static List<string> lstAnswer = new List<string>() { "哦？这么", "是么？有多", "那还真是" };


        public static List<string> lstSelf = new List<string>();
        public static List<string> lstIs = new List<string>();
        public static List<string> lstHave = new List<string>();
        public static List<string> lstDid = new List<string>();
        public static List<string> lstBelong = new List<string>();



        public static Regex regAdj = new Regex("太(\\w*?)了");
        public static Regex regCQCode = new Regex(@"(?:\[CQ:.*\])");

        public static string DealOtherRequest(JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;
            string strInput = jsonGrpMsg.message;
            while (regCQCode.IsMatch(strInput))
            {
                strInput = regCQCode.Replace(strInput, "");
            }

            bool bAnswer = true;
            if (lstMe.Where(Callme => { return strInput.Contains(Callme); }).ToList().Count > 0)
            {
                //单纯叫名字
                if (lstPrep.Where(Callme => { return strInput.Contains(Callme); }).ToList().Count <= 0)
                {
                    strMessage += "叫我吗？";
                }
                lstSelf.Add(strInput);
                FilesHelper.OutputJsonFile("Talking\\Self", JsonConvert.SerializeObject(lstSelf, Formatting.Indented));
            }
            else
            {
                bAnswer = Commons.rnd.Next() % 100 < 10;
            }
            //常见语法1 
            if (regAdj.IsMatch(strInput))
            {
                string strAdjs = regAdj.Match(strInput).Value;
                strAdjs = strAdjs.Trim('太', '了');
                if (bAnswer)
                {
                    strMessage += lstAnswer[Commons.rnd.Next() % lstAnswer.Count] + strAdjs;
                }
                GetNewWord(strAdjs);
            }
            else if (strInput.Contains("是"))
            {

                lstIs.Add(strInput);
                FilesHelper.OutputJsonFile("Talking\\Is", JsonConvert.SerializeObject(lstIs, Formatting.Indented));
            }
            else if (strInput.Contains("有没有"))
            {
                string strOther = strInput.Substring(strInput.IndexOf("有没有") + 3);

                if (bAnswer)
                {
                    string strURL = @"https://www.baidu.com/s?wd=" + WebUtility.UrlEncode(strOther);
                    strMessage += "没有，建议百度一下\n" + strURL;
                }
                lstHave.Add(strInput);
                FilesHelper.OutputJsonFile("Talking\\Have", JsonConvert.SerializeObject(lstHave, Formatting.Indented));
            }
            else if (strInput.Contains("了"))
            {
                lstDid.Add(strInput);
                FilesHelper.OutputJsonFile("Talking\\Did", JsonConvert.SerializeObject(lstDid, Formatting.Indented));
            }
            else if (strInput.Contains("的"))
            {
                lstBelong.Add(strInput);
                FilesHelper.OutputJsonFile("Talking\\Belong", JsonConvert.SerializeObject(lstBelong, Formatting.Indented));
            }
            return strMessage;
        }

        private static void GetNewWord(string strAdjs)
        {
            if (lstWords.FindAll(Words => Words.Word == strAdjs).Count <= 0)
            {
                JsonWords words = new JsonWords();
                words.Type = "Adjs";
                words.Word = strAdjs;
                lstWords.Add(words);
                FilesHelper.OutputJsonFile("Talking\\Words", JsonConvert.SerializeObject(lstWords, Formatting.Indented));
            }
        }
    }
}
