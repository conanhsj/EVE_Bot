using Bot.EveRequest.APIs;
using Bot.EveRequest.Json;
using Bot.ExtendInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bot.EveRequest
{
    [Export("EveRequest", typeof(IMessageRequest))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EveRequest : IMessageRequest
    {
        private static List<WormholeSystem> lstWormholeSystem = null;
        private static List<WormholeReward> lstRewards = null;
        private static List<Solar> lstSolar = null;
        private static List<SystemKills> lstSystemKills = null;
        private static DateTime lastCache = DateTime.MinValue;


        public EveRequest()
        {
            ReadWormholeSystem();
            ReadRewards();
            ReadSolar();
        }
        private void ReadWormholeSystem()
        {
            string strContents = Constants.CheckAndReadPath(Constants.Path_Wormhole);
            if (string.IsNullOrEmpty(strContents))
            {
                lstWormholeSystem = new List<WormholeSystem>();
                return;
            }
            lstWormholeSystem = JsonConvert.DeserializeObject<List<WormholeSystem>>(strContents);
        }
        private void ReadRewards()
        {
            string strContents = Constants.CheckAndReadPath(Constants.Path_Rewards);
            if (string.IsNullOrEmpty(strContents))
            {
                lstRewards = new List<WormholeReward>();
                return;
            }
            lstRewards = JsonConvert.DeserializeObject<List<WormholeReward>>(strContents);
        }
        private void ReadSolar()
        {
            string strContents = Constants.CheckAndReadPath(Constants.Path_Solar);
            if (string.IsNullOrEmpty(strContents))
            {
                lstSolar = new List<Solar>();
                return;
            }
            lstSolar = JsonConvert.DeserializeObject<List<Solar>>(strContents);
        }

        string IMessageRequest.DealGroupRequest(JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = jsonGrpMsg.message.Replace("\n", ""); ;
            string strReturn = string.Empty;

            while (Constants.regCQCode.IsMatch(strMessage))
            {
                strMessage = Constants.regCQCode.Replace(strMessage, "");
            }

            strReturn = DealSearchCommon(jsonGrpMsg, strMessage);

            return strReturn;
        }
        private string DealSearchCommon(JORecvGroupMsg jsonGrpMsg, string strMessage)
        {
            string strReturn = string.Empty;

            if (strMessage.IndexOf("虫洞") > 0)
            {
                strReturn = SearchWormhole(jsonGrpMsg, strMessage);
            }
            else if (strMessage.IndexOf(Constants.Menu_AddReward) > 0)
            {
                strReturn = AddReward(jsonGrpMsg, strMessage);
            }
            else if (strMessage.IndexOf(Constants.Menu_DelReward) > 0)
            {
                strReturn = DelReward(jsonGrpMsg, strMessage);
            }
            else if (strMessage.IndexOf(Constants.Menu_SystemKills) > 0)
            {
                strReturn = SystemKills(jsonGrpMsg, strMessage);
            }
            else
            {
                strReturn += "目前的可用命令有：虫洞,";
                strReturn += Constants.Menu_AddReward + ",";
                strReturn += Constants.Menu_DelReward + ",";
                strReturn += Constants.Menu_SystemKills;
            }
            return strReturn;
        }
        private static string SystemKills(JORecvGroupMsg jsonGrpMsg, string strMessage)
        {
            //掐头检索词
            strMessage = strMessage.Substring(strMessage.IndexOf(Constants.Menu_SystemKills) + Constants.Menu_SystemKills.Length).Trim();
            //切分变量
            string[] strArgs = strMessage.Split(' ');
            //初始化返回值
            string strReturn = string.Empty;

            if ((DateTime.Now - lastCache).TotalMinutes > 15)
            {
                strReturn += "之前的数据有点旧了，正在重新统计\n";
                lstSystemKills = InterfaceESI.Universe_SystemKills();
                lastCache = DateTime.Now;
            }
            else
            {
                strReturn += "前回统计时间为：" + lastCache.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
            }

            if (strArgs.Length >= 1 && !string.IsNullOrEmpty(strArgs[0]))
            {
                if (strArgs[0].ToUpper() == "NPC")
                {
                    List<SystemKills> lstSort = lstSystemKills.OrderBy(item => item.npc_kills).Reverse().ToList();
                    for (int i = 0; i < 10; i++)
                    {
                        Solar solar = lstSolar.Find(item => item.system_id == lstSort[i].system_id);
                        strReturn += "第" + (i + 1) + "位," + solar.system_name + " 击杀数：" + lstSort[i].npc_kills + "\n";
                    }
                }
                else if (strArgs[0].ToUpper() == "船只")
                {
                    List<SystemKills> lstSort = lstSystemKills.OrderBy(item => item.ship_kills).Reverse().ToList();
                    for (int i = 0; i < 10; i++)
                    {
                        Solar solar = lstSolar.Find(item => item.system_id == lstSort[i].system_id);
                        strReturn += "第" + (i + 1) + "位," + solar.system_name + " 击杀数：" + lstSort[i].ship_kills + "\n";
                    }
                }
                else if (strArgs[0].ToUpper() == "太空舱")
                {
                    List<SystemKills> lstSort = lstSystemKills.OrderBy(item => item.pod_kills).Reverse().ToList();
                    for (int i = 0; i < 10; i++)
                    {
                        Solar solar = lstSolar.Find(item => item.system_id == lstSort[i].system_id);
                        strReturn += "第" + (i + 1) + "位," + solar.system_name + " 击杀数：" + lstSort[i].pod_kills + "\n";
                    }
                }
                else
                {
                    Solar target = lstSolar.Find(item => item.system_name.ToUpper() == strArgs[0].ToUpper());
                    if (target != null)
                    {
                        SystemKills record = lstSystemKills.Find(item => item.system_id == target.system_id);
                        if(record != null)
                        {
                            strReturn += "在" + target.system_name + "(" + target.system_id + ")最近的击杀数为";
                            strReturn += " NPC:" + record.npc_kills + " 船只:" + record.ship_kills + " 太空舱:" + record.pod_kills + "\n";
                        }
                        else
                        {
                            strReturn += "未找到记录.";
                        }

                    }
                    else
                    {
                        List<Solar> lstResult = lstSolar.FindAll(item => item.system_name.ToUpper().Contains(strArgs[0].ToUpper()));
                        if (lstResult.Count < 20)
                        {
                            strReturn += "找到" + lstResult.Count + "个星系,请说清楚一点：\n";
                            foreach (Solar System in lstResult)
                            {
                                strReturn += System.system_name + ",";
                            }
                            strReturn = strReturn.TrimEnd(',');
                        }
                        else
                        {
                            strReturn += "有" + lstResult.Count + "个结果,你莫不是在消遣洒家？";
                        }
                    }


                }
            }
            else
            {
                strReturn += "范例 「" + Constants.Menu_SystemKills + " NPC/船只/太空舱/星系名」";
            }

            return strReturn;
        }
        private static string SearchSolar(JORecvGroupMsg jsonGrpMsg, string strMessage)
        {
            //掐头检索词
            strMessage = strMessage.Substring(strMessage.IndexOf(Constants.Menu_AddReward) + Constants.Menu_AddReward.Length).Trim();
            //切分变量
            string[] strArgs = strMessage.Split(' ');
            //初始化返回值
            string strReturn = string.Empty;


            //List<Solar> lstResult = lstSolar.FindAll(solar => solar.system_name.Contains(strRequest));

            //if (lstResult.Count > 0)
            //{
            //    strMessage += "找到" + lstResult.Count + "个结果\n";
            //    foreach (Solar Solar in lstResult)
            //    {
            //        strMessage += Solar.system_name + "(" + Solar.system_id + ") << " + Solar.constellation_name + "(" + Solar.constellation_id + ") << " + Solar.region_name + "(" + Solar.region_id + ")\n";
            //    }
            //    strMessage.Trim('\n');
            //}
            //else
            //{
            //    strMessage += "没找到啊";
            //}
            return strMessage;
        }


        private string DelReward(JORecvGroupMsg jsonGrpMsg, string strMessage)
        {
            //掐头检索词
            strMessage = strMessage.Substring(strMessage.IndexOf(Constants.Menu_DelReward) + Constants.Menu_DelReward.Length).Trim();
            //切分变量
            string[] strArgs = strMessage.Split(' ');
            //初始化返回值
            string strReturn = string.Empty;

            if (strArgs.Length >= 1 && !string.IsNullOrEmpty(strArgs[0]))
            {
                List<WormholeSystem> lstResult = lstWormholeSystem.FindAll(wh => wh.Name.Contains(strArgs[0].ToUpper()));
                if (lstResult.Count == 1)
                {
                    int nInt = lstRewards.RemoveAll(X => X.Name.Contains(strArgs[0].ToUpper()));

                    Constants.OutputJsonFile(Constants.Path_Rewards, JsonConvert.SerializeObject(lstRewards, Formatting.Indented));
                    strReturn += "删掉了" + nInt + "条求助记录。";
                }
                else
                {
                    if (lstResult.Count < 10)
                    {
                        strReturn += "找到" + lstResult.Count + "个星系,请说清楚一点：\n";
                        foreach (WormholeSystem System in lstResult)
                        {
                            strReturn += System.Name + ",";
                        }
                        strReturn = strReturn.TrimEnd(',');
                    }
                    else
                    {
                        strReturn += "有" + lstResult.Count + "个结果,你莫不是在消遣洒家？";
                    }
                }
            }
            else
            {
                strReturn += "请输入虫洞星系的名称，范例 「" + Constants.Menu_DelReward + " 虫洞名」";
            }
            return strReturn;
        }
        private string AddReward(JORecvGroupMsg jsonGrpMsg, string strMessage)
        {
            //掐头检索词
            strMessage = strMessage.Substring(strMessage.IndexOf(Constants.Menu_AddReward) + Constants.Menu_AddReward.Length).Trim();
            //切分变量
            string[] strArgs = strMessage.Split(' ');
            //初始化返回值
            string strReturn = string.Empty;

            if (strArgs.Length >= 1 && !string.IsNullOrEmpty(strArgs[0]))
            {
                List<WormholeSystem> lstResult = lstWormholeSystem.FindAll(wh => wh.Name.Contains(strArgs[0].ToUpper()));
                if (lstResult.Count == 1)
                {
                    WormholeReward newReward = new WormholeReward();
                    newReward.Name = lstResult[0].Name;
                    newReward.Memo = strArgs.Length > 1 ? strMessage.Substring(strMessage.IndexOf(strArgs[0]) + strArgs[0].Length + 1) : "此人没有留下内容，请联系QQ:" + jsonGrpMsg.sender.user_id.ToString();
                    newReward.Date = DateTime.Now;
                    newReward.senderQQ = jsonGrpMsg.sender.user_id;

                    lstRewards.Add(newReward);
                    Constants.OutputJsonFile(Constants.Path_Rewards, JsonConvert.SerializeObject(lstRewards, Formatting.Indented));
                    strReturn += "追加了一条求助记录";
                }
                else
                {
                    if (lstResult.Count < 10)
                    {
                        strReturn += "找到" + lstResult.Count + "个星系,请说清楚一点：\n";
                        foreach (WormholeSystem System in lstResult)
                        {
                            strReturn += System.Name + ",";
                        }
                        strReturn = strReturn.TrimEnd(',');
                    }
                    else
                    {
                        strReturn += "有" + lstResult.Count + "个结果,你莫不是在消遣洒家？";
                    }
                }
            }
            else
            {
                strReturn += "请输入虫洞星系的名称，范例 「" + Constants.Menu_AddReward + " 虫洞名 联系QQ/游戏内名称/奖赏内容等」";
            }

            return strReturn;
        }
        private string SearchWormhole(JORecvGroupMsg jsonGrpMsg, string strMessage)
        {
            //掐头检索词
            strMessage = strMessage.Substring(strMessage.IndexOf("虫洞") + 2).Trim();
            //切分变量
            string[] strArgs = strMessage.Split(' ');
            //初始化返回值
            string strReturn = string.Empty;

            List<WormholeSystem> lstResult = lstWormholeSystem.FindAll(wh => wh.Name.Contains(strMessage.ToUpper()));

            strReturn += "找到" + lstResult.Count + "个结果";

            if (lstResult.Count > 0 && lstResult.Count < 20)
            {
                strReturn += "\n";
                ReadRewards();
                foreach (WormholeSystem WH in lstResult)
                {
                    strReturn += WH.Name + " 等级：" + WH.Class + " 天象：" + (WH.Effects == string.Empty ? "None" : WH.Effects) + " 永联：" + WH.Statics + "\n";

                    List<WormholeReward> rewards = lstRewards.FindAll(X => X.Name.Contains(strMessage.ToUpper()));
                    if (rewards.Count > 0)
                    {
                        strReturn += "好像有人在找这个洞：\n";
                        foreach (WormholeReward reward in rewards)
                        {
                            strReturn += ((int)(DateTime.Now - reward.Date).TotalDays).ToString() + "天前 " + reward.Memo + "\n";
                        }
                    }
                }
                strReturn.Trim('\n');
            }
            return strReturn;
        }

        string IMessageRequest.DealPrivateRequest(JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = jsonGrpMsg.message;
            string strReturn = string.Empty;

            while (Constants.regCQCode.IsMatch(strMessage))
            {
                strMessage = Constants.regCQCode.Replace(strMessage, "");
            }

            strReturn = DealSearchCommon(jsonGrpMsg, strMessage);

            return strReturn;
        }
    }
}
