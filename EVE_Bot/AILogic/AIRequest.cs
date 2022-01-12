using Bot.ExtendInterface;
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
        public static List<string> lstMe = new List<string>() { "猫娘", "这猫", "AI" };
        public static List<string> lstPrep = new List<string>() { "的", "是", "了" };
        public static List<string> lstAnswer = new List<string>() { "哦？这么", "是么？有多", "那还真是" };

        public static List<string> lstSelf = new List<string>();
        public static List<string> lstIs = new List<string>();
        public static List<string> lstHave = new List<string>();
        public static List<string> lstDid = new List<string>();
        public static List<string> lstBelong = new List<string>();
        public static List<string> lstSearchTrigger = new List<string>() { "有没有人知道", "百度一下", "有没有谁知道" };
        public static Dictionary<string, Dictionary<string, string>> dicTodayDick = new Dictionary<string, Dictionary<string, string>>();
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

            //触发几率
            bool bAnswer = true;
            if (lstMe.Where(Callme => { return strInput.StartsWith(Callme); }).ToList().Count > 0)
            {
                strMessage += "叫我吗？\n";
                lstSelf.Add(strInput);
                FilesHelper.OutputJsonFile("Talking\\Self", JsonConvert.SerializeObject(lstSelf, Formatting.Indented));
            }
            else
            {
                bAnswer = Commons.rnd.Next() % 100 < 10;
            }

            List<JsonWords> lstParts = lstWords.FindAll(Words => Words.Type == "Adjs").Where(Keys => { return strInput.ToUpper().Contains(Keys.Word); }).ToList();

            //常见语法1 
            if (regAdj.IsMatch(strInput))
            {
                string strAdjs = regAdj.Match(strInput).Value;
                strAdjs = strAdjs.Trim('太', '了');
                if (strAdjs.Length == 0 || strAdjs.Length > 5)
                {
                    return "";
                }
                if (bAnswer)
                {
                    strMessage += lstAnswer[Commons.rnd.Next() % lstAnswer.Count] + strAdjs;
                }
                GetNewWord(strAdjs);
            }
            else if (jsonGrpMsg.sender.user_id == 691854365 && strInput.Contains("今日的丁丁长度是"))
            {
                int QQNumber = jsonGrpMsg.message.IndexOf("[CQ:at,qq=");
                string strTar = jsonGrpMsg.message.Substring(QQNumber + 10);
                string strName = strTar.Substring(strTar.IndexOf('@') + 1);
                strName = strName.Substring(0, strName.IndexOf(']')).Trim();

                string strDick = strInput.Substring(strInput.IndexOf("今日的丁丁长度是") + 8);

                string strToday = DateTime.Now.ToString("yyyy-MM-dd");
                if (dicTodayDick.ContainsKey(strToday))
                {
                    Dictionary<string, string> dicDick = dicTodayDick[strToday];
                    if (!dicDick.ContainsKey(strName))
                    {
                        dicDick.Add(strName, strDick);
                    }
                    FilesHelper.OutputJsonFile("Talking\\Dick", JsonConvert.SerializeObject(dicTodayDick, Formatting.Indented));
                }
                else
                {
                    if (dicTodayDick.Keys.Count > 0)
                    {
                        List<string> lstKey = dicTodayDick.Keys.ToList();

                        foreach (string strKey in lstKey)
                        {
                            dicTodayDick.Remove(strKey);
                        }
                    }
                    Dictionary<string, string> dicDick = new Dictionary<string, string>();
                    dicDick.Add(strName, strDick);
                    dicTodayDick.Add(strToday, dicDick);
                    FilesHelper.OutputJsonFile("Talking\\Dick", JsonConvert.SerializeObject(dicTodayDick, Formatting.Indented));
                }
            }
            //else if (strInput.Contains("是"))
            //{
            //    lstIs.Add(strInput);
            //    FilesHelper.OutputJsonFile("Talking\\Is", JsonConvert.SerializeObject(lstIs, Formatting.Indented));
            //}
            //else if (lstSearchTrigger.Where(Keys => { return strInput.ToUpper().Contains(Keys); }).ToList().Count > 0)
            //{
            //    string strKey = lstSearchTrigger.Where(Keys => { return strInput.ToUpper().Contains(Keys); }).First();
            //    string strOther = strInput.Substring(strInput.IndexOf(strKey) + strKey.Length);
            //    if (bAnswer)
            //    {
            //        string strURL = @"https://www.baidu.com/s?wd=" + WebUtility.UrlEncode(strOther);
            //        strMessage += "自己去百度找吧。\n" + strURL;
            //    }
            //    lstHave.Add(strInput);
            //    FilesHelper.OutputJsonFile("Talking\\Have", JsonConvert.SerializeObject(lstHave, Formatting.Indented));
            //}
            //else if (strInput.Contains("了"))
            //{
            //    lstDid.Add(strInput);
            //    FilesHelper.OutputJsonFile("Talking\\Did", JsonConvert.SerializeObject(lstDid, Formatting.Indented));
            //}
            //else if (strInput.Contains("的"))
            //{
            //    lstBelong.Add(strInput);
            //    FilesHelper.OutputJsonFile("Talking\\Belong", JsonConvert.SerializeObject(lstBelong, Formatting.Indented));
            //}
            //else if (lstParts.Count > 0)
            //{
            //    if (bAnswer)
            //    {
            //        strMessage += "那也太" + lstParts[0].Word + "了";
            //    }
            //}
            else if (strInput.ToUpper().StartsWith(".JITA") || strInput.ToUpper().StartsWith("JITA"))
            {
                if (bAnswer)
                {
                    strMessage += "成天就知道jita，就不能问问我嘛！";
                }
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
