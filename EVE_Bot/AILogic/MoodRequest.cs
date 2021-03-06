using EVE_Bot.Classes;
using EVE_Bot.Helper;
using EVE_Bot.JsonEVE;
using EVE_Bot.JsonObject;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EVE_Bot.AILogic
{
    public static class MoodRequest
    {
        public static List<string> lstWarningWord = JsonConvert.DeserializeObject<List<string>>(FilesHelper.ReadJsonFile("WarningWord"));
        public static List<string> lstDirtyWord = JsonConvert.DeserializeObject<List<string>>(FilesHelper.ReadJsonFile("DirtyWord"));


        public static string DealAtRequest(ClientWebSocket ws, JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;//"[CQ:at,qq=" + jsonGrpMsg.user_id + "]";
            Match match = Regex.Match(jsonGrpMsg.message, "(?:\\[*\\]).*");

            if (match.Success)
            {
                string strRequest = match.Value.Trim(']', ' ');
                if (strRequest.StartsWith("加入脏话："))
                {
                    string strKeyWord = strRequest.Substring(5);
                    if (strKeyWord.Contains("[CQ:"))
                    {
                        strMessage += "暂时不可以";
                        return strMessage;
                    }
                    if (!lstDirtyWord.Contains(strKeyWord))
                    {
                        lstDirtyWord.Add(strKeyWord);
                    }
                    else
                    {
                        strMessage += "已经会了，火星人";
                        return strMessage;
                    }
                 
                    FilesHelper.OutputJsonFile("DirtyWord", JsonConvert.SerializeObject(lstDirtyWord, Formatting.Indented));
                    strMessage += "变的更脏了";
                }
                else if (strRequest.StartsWith("加入敏感词："))
                {
                    string strKeyWord = strRequest.Substring(6);
                    if (strKeyWord.Contains("[CQ:"))
                    {
                        strMessage += "暂时不可以";
                        return strMessage;
                    }
                    if (!lstDirtyWord.Contains(strKeyWord))
                    {
                        lstWarningWord.Add(strKeyWord);
                    }
                    else
                    {
                        strMessage += "已经会了，火星人";
                        return strMessage;
                    }
                    FilesHelper.OutputJsonFile("WarningWord", JsonConvert.SerializeObject(lstWarningWord, Formatting.Indented));
                    strMessage += "变的更敏感了";
                }
                else if (strRequest.StartsWith("删除脏话："))
                {
                    string strKeyWord = strRequest.Substring(5);
                    if (lstDirtyWord.Contains(strKeyWord))
                    {
                        lstDirtyWord.Remove(strKeyWord);
                    }
                    else
                    {
                        strMessage += "我可没那么脏";
                        return strMessage;
                    }
                    FilesHelper.OutputJsonFile("WarningWord", JsonConvert.SerializeObject(lstWarningWord, Formatting.Indented));
                    strMessage += "变的更干净了";
                }
                else if (strRequest.StartsWith("删除敏感词："))
                {
                    string strKeyWord = strRequest.Substring(6);
                    if (lstDirtyWord.Contains(strKeyWord))
                    {
                        lstWarningWord.Remove(strKeyWord);
                    }
                    else
                    {
                        strMessage += "我可没那么敏感";
                        return strMessage;
                    }
                    FilesHelper.OutputJsonFile("WarningWord", JsonConvert.SerializeObject(lstWarningWord, Formatting.Indented));
                    strMessage += "变的更正经了";
                }
            }
            else
            {
                strMessage += "听不懂";
            }

            return strMessage;
        }

    }
}
