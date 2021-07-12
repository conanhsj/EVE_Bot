using Bot.ExtendInterface;
using EVE_Bot.Helper;
using EVE_Bot.JsonEVE;
using EVE_Bot.JsonObject;
using EVE_Bot.JsonSetting;
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

        //各Q群设置
        public static List<JsonGroup> lstGroupSetting = JsonConvert.DeserializeObject<List<JsonGroup>>(FilesHelper.ReadJsonFile(@"Trigger\GroupSetting"));
        public static List<string> lstWarningWord = JsonConvert.DeserializeObject<List<string>>(FilesHelper.ReadJsonFile(@"Trigger\WarningWord"));
        public static List<string> lstDirtyWord = JsonConvert.DeserializeObject<List<string>>(FilesHelper.ReadJsonFile(@"Trigger\DirtyWord"));
        public static List<string> lstPrattle = JsonConvert.DeserializeObject<List<string>>(FilesHelper.ReadJsonFile(@"Trigger\Prattle"));
        public static List<string> lstFlatter = JsonConvert.DeserializeObject<List<string>>(FilesHelper.ReadJsonFile(@"Trigger\Flatter"));
        public static Dictionary<long, string> dicMemo = JsonConvert.DeserializeObject<Dictionary<long, string>>(FilesHelper.ReadJsonFile(@"Trigger\Memo"));
        public static Dictionary<string, string> dicAnswer = JsonConvert.DeserializeObject<Dictionary<string, string>>(FilesHelper.ReadJsonFile(@"Trigger\Answer"));

        public static Dictionary<string, List<string>> dicRequest = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(FilesHelper.ReadJsonFile(@"Request"));
        public static string DealAtRequest(ClientWebSocket ws, JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;//"[CQ:at,qq=" + jsonGrpMsg.user_id + "]";
            string strRequest = Commons.RemoveCQCode(jsonGrpMsg.message).Trim();
            if (strRequest.StartsWith("加入脏话："))
            {
                string strKeyWord = strRequest.Substring(5);

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

                if (!lstWarningWord.Contains(strKeyWord))
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
                FilesHelper.OutputJsonFile("DirtyWord", JsonConvert.SerializeObject(lstDirtyWord, Formatting.Indented));
                strMessage += "变的更干净了";
            }
            else if (strRequest.StartsWith("删除敏感词："))
            {
                string strKeyWord = strRequest.Substring(6);
                if (lstWarningWord.Contains(strKeyWord))
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
            else if (strRequest.StartsWith("加入情话："))
            {
                string strKeyWord = strRequest.Substring(5);
                if (!lstPrattle.Contains(strKeyWord))
                {
                    lstPrattle.Add(strKeyWord);
                }
                else
                {
                    strMessage += "刚刚教过了";
                    return strMessage;
                }
                FilesHelper.OutputJsonFile("Prattle", JsonConvert.SerializeObject(lstPrattle, Formatting.Indented));
                strMessage += "会更努力的夸你的";
            }
            else if (strRequest.StartsWith("加入彩虹屁："))
            {
                string strKeyWord = strRequest.Substring(6);
                if (!lstFlatter.Contains(strKeyWord))
                {
                    lstFlatter.Add(strKeyWord);
                }
                else
                {
                    strMessage += "刚刚教过了";
                    return strMessage;
                }
                FilesHelper.OutputJsonFile(@"Trigger\Flatter", JsonConvert.SerializeObject(lstFlatter, Formatting.Indented));
                strMessage += "自己都觉得恶心了";
            }
            else if (strRequest.StartsWith("加入专有问答："))
            {
                string strKeyWord = strRequest.Substring(7).Trim();

                int nIndexComma = strKeyWord.IndexOf("：");
                if (nIndexComma > 0)
                {
                    string strKey = strKeyWord.Substring(0, nIndexComma);
                    string strValue = strKeyWord.Substring(nIndexComma + 1);
                    dicAnswer.Add(strKey, strValue);
                    FilesHelper.OutputJsonFile(@"Trigger\Answer", JsonConvert.SerializeObject(dicAnswer, Formatting.Indented));
                    strMessage += "添加专有问答成功。";
                }
                else
                {
                    strMessage += "你这个教的不规范，我听不懂。";
                }
            }
            else if (strRequest.StartsWith("删除专有问答："))
            {
                string strKeyWord = strRequest.Substring(7).Trim();

                if (dicAnswer.ContainsKey(strKeyWord))
                {
                    dicAnswer.Remove(strKeyWord);
                    FilesHelper.OutputJsonFile(@"Trigger\Answer", JsonConvert.SerializeObject(dicAnswer, Formatting.Indented));
                    strMessage += "删除专有问答成功。";
                }
                else
                {
                    strMessage += "没人教过这个东西。";
                }
            }
            else if (strRequest.StartsWith("色图功能：开启"))
            {
                JsonGroup group = lstGroupSetting.Find(obj => obj.group_id == jsonGrpMsg.group_id);
                group.SetuOpen = true;
                FilesHelper.OutputJsonFile(@"Trigger\GroupSetting", JsonConvert.SerializeObject(MoodRequest.lstGroupSetting, Formatting.Indented));
                strMessage += "又要给你们找色图去了";
            }
            else if (strRequest.StartsWith("色图功能：关闭"))
            {
                JsonGroup group = lstGroupSetting.Find(obj => obj.group_id == jsonGrpMsg.group_id);
                group.SetuOpen = false;
                FilesHelper.OutputJsonFile(@"Trigger\GroupSetting", JsonConvert.SerializeObject(MoodRequest.lstGroupSetting, Formatting.Indented));
                strMessage += "是你们自己决定戒色的";
            }
            else if (strRequest.Contains("帮助"))
            {
                string strKeyWord = strRequest.Substring(strRequest.IndexOf(' ') + 1).Trim();
                if (strRequest.IndexOf(' ') == -1)
                {
                    strMessage += "目前支持的触发方式有：\n";
                    strMessage += "　　！ => 有关EVE的各种查询功能\n";
                    strMessage += "　　@我  => 有关触发词的设置功能\n";
                    strMessage += "　　Roll  => 简单的掷骰子功能\n";
                    strMessage += "　　涩图  => 第三方提供的P站图片功能\n";
                }
            }
            else if (strRequest.StartsWith("备忘："))
            {
                string strKeyWord = strRequest.Substring(3).Trim();
                if (!string.IsNullOrEmpty(strRequest))
                {
                    if (dicMemo.ContainsKey(jsonGrpMsg.user_id))
                    {
                        string strMemo = dicMemo[jsonGrpMsg.user_id];
                        strMessage += "明明刚才说" + strMemo + "的，";
                        dicMemo.Remove(jsonGrpMsg.user_id);
                    }
                    dicMemo.Add(jsonGrpMsg.user_id, strRequest);
                    FilesHelper.OutputJsonFile(@"Trigger\Memo", JsonConvert.SerializeObject(dicMemo, Formatting.Indented));
                    strMessage += "行吧，我记下来了。";
                }
                else
                {
                    if (dicMemo.ContainsKey(jsonGrpMsg.user_id))
                    {
                        string strMemo = dicMemo[jsonGrpMsg.user_id];
                        strMessage += "你之前记录的内容是：\"" + strMemo + "\"来着";
                        dicMemo.Remove(jsonGrpMsg.user_id);
                        FilesHelper.OutputJsonFile(@"Trigger\Memo", JsonConvert.SerializeObject(dicMemo, Formatting.Indented));
                    }
                }
            }
            else if (strRequest.StartsWith("留言："))
            {
                if (dicRequest == null)
                {
                    dicRequest = new Dictionary<string, List<string>>();
                }
                string strKeyWord = strRequest.Substring(3).Trim();
                if (dicRequest.ContainsKey(jsonGrpMsg.user_id.ToString() + "||" + jsonGrpMsg.sender.nickname + "||" + jsonGrpMsg.sender.card))
                {
                    dicRequest[jsonGrpMsg.user_id.ToString() + "||" + jsonGrpMsg.sender.nickname + "||" + jsonGrpMsg.sender.card].Add(strKeyWord);
                }
                else
                {
                    List<string> lstRequest = new List<string>();
                    lstRequest.Add(strKeyWord);
                    dicRequest.Add(jsonGrpMsg.user_id.ToString() + "||" + jsonGrpMsg.sender.nickname + "||" + jsonGrpMsg.sender.card, lstRequest);
                }
                FilesHelper.OutputJsonFile("Request", JsonConvert.SerializeObject(dicRequest, Formatting.Indented));
                strMessage += "好的，记录下来了。可能会看到的。";
            }
            else
            {
                strMessage += "目前可以支持的设置内容有：\n";
                strMessage += "　　加入敏感词：\n";
                strMessage += "　　加入脏话：\n";
                strMessage += "　　删除敏感词：\n";
                strMessage += "　　删除脏话：\n";
                strMessage += "　　色图功能：开启\n";
                strMessage += "　　色图功能：关闭\n";
                strMessage += "　　备忘：\n";
                strMessage += "　　留言：\n";
                strMessage += "　　帮助\n";
            }

            return strMessage;
        }

    }
}
