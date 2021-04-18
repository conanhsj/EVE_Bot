﻿using EVE_Bot.Interface;
using EVE_Bot.JsonGame;
using EVE_Bot.JsonObject;
using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVE_Bot.Helper;
using Newtonsoft.Json;

namespace EVE_Bot.AILogic
{
    [Export("WolfRequest", typeof(IMessageRequest))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WolfRequest : IMessageRequest
    {

        private Dictionary<long, List<WolfChara>> dicGames = JsonConvert.DeserializeObject<Dictionary<long, List<WolfChara>>>(FilesHelper.ReadJsonFile("Game\\Wolf"));

        string IMessageRequest.DealGroupRequest(JORecvGroupMsg jsonGrpMsg)
        {
            string strValue = string.Empty;
            string strMessage = jsonGrpMsg.message;

            if (strMessage.StartsWith("狼人杀"))
            {
                strMessage = strMessage.Substring(3).Trim();

                if (!dicGames.ContainsKey(jsonGrpMsg.group_id) && strMessage.StartsWith("创建游戏"))
                {
                    strValue = CreateGame(jsonGrpMsg, strValue);
                }
                else if (!dicGames.ContainsKey(jsonGrpMsg.group_id))
                {
                    strValue += "当前群内没有正在进行的进度 可用命令：\n  创建游戏\n  加入游戏\n  开始游戏\n  结束游戏\n  帮助";
                }
                else if (strMessage.StartsWith("帮助"))
                {
                    List<WolfChara> lstGame = dicGames[jsonGrpMsg.group_id];
                    strValue += "在本局游戏中：\n";
                    foreach (WolfChara chara in lstGame)
                    {
                        strValue += chara.user_name + "[" + chara.status + "],";
                    }
                    strValue = strValue.TrimEnd(',');
                }
                else if (strMessage.StartsWith("结束游戏"))
                {
                    strValue = EndGame(jsonGrpMsg, strValue);
                }
                else if (strMessage.StartsWith("加入游戏"))
                {
                    strValue = JoinGame(jsonGrpMsg, strValue);
                }
                else if (strMessage.StartsWith("开始游戏"))
                {
                    strValue = "后边的功能以后再来探索吧~";
                }
                else
                {

                }
            }

            return strValue;
        }

        string IMessageRequest.DealPrivateRequest(JORecvGroupMsg jsonGrpMsg)
        {
            throw new NotImplementedException();
        }

        private string CreateGame(JORecvGroupMsg jsonGrpMsg, string strValue)
        {
            WolfChara wcCreator = new WolfChara();
            wcCreator.user_id = jsonGrpMsg.user_id;
            wcCreator.user_name = jsonGrpMsg.sender.card;
            wcCreator.status = "存活";
            wcCreator.position = "创建者";
            List<WolfChara> lstGame = new List<WolfChara>();
            lstGame.Add(wcCreator);
            dicGames.Add(jsonGrpMsg.group_id, lstGame);
            strValue += jsonGrpMsg.sender.card + "支起了桌子";
            FilesHelper.OutputJsonFile("Game\\Wolf", JsonConvert.SerializeObject(dicGames, Formatting.Indented));
            return strValue;
        }

        private string JoinGame(JORecvGroupMsg jsonGrpMsg, string strValue)
        {
            List<WolfChara> lstGame = dicGames[jsonGrpMsg.group_id];
            if (lstGame.Where(player => player.user_id == jsonGrpMsg.user_id).ToList().Count > 0)
            {
                strValue += "别瞎添乱，老实坐着";
                return strValue;
            }
            WolfChara wcCreator = new WolfChara();
            wcCreator.user_id = jsonGrpMsg.user_id;
            wcCreator.user_name = jsonGrpMsg.sender.card;
            wcCreator.status = "存活";
            wcCreator.position = "参与者";
            lstGame.Add(wcCreator);
            strValue += jsonGrpMsg.sender.card + "坐到了" + lstGame[lstGame.Count - 2].user_name + "旁边，现在有" + lstGame.Count + "个人了";

            FilesHelper.OutputJsonFile("Game\\Wolf", JsonConvert.SerializeObject(dicGames, Formatting.Indented));
            return strValue;
        }

        private string EndGame(JORecvGroupMsg jsonGrpMsg, string strValue)
        {
            List<WolfChara> lstGame = dicGames[jsonGrpMsg.group_id];
            WolfChara wcChara = lstGame.Find(gamer => gamer.user_id == jsonGrpMsg.user_id);

            if (lstGame.Count < 4)
            {
                dicGames.Remove(jsonGrpMsg.group_id);
                strValue += jsonGrpMsg.sender.card + "把桌子收起来了";
            }
            else if (wcChara == null)
            {
                if (lstGame[0].position != "创建者")
                {
                    strValue += "已经开始的对决只能由局内人来结束";
                }
                else
                {
                    dicGames.Remove(jsonGrpMsg.group_id);
                    strValue += jsonGrpMsg.sender.card + "不希望看到" + lstGame[0].user_name + "摸鱼行为";
                }
            }
            else if (wcChara.position != "创建者" && wcChara.position != "参与者")
            {
                dicGames.Remove(jsonGrpMsg.group_id);
                strValue += wcChara.user_name + "愤怒的踢翻了桌子";
            }
            else
            {
                dicGames.Remove(jsonGrpMsg.group_id);
                strValue += "觉得组不起来的游戏的" + wcChara.user_name + "把桌子收拾起来了";
            }
            FilesHelper.OutputJsonFile("Game\\Wolf", JsonConvert.SerializeObject(dicGames, Formatting.Indented));
            return strValue;
        }


    }
}
