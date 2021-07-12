using Bot.DerbyRequest.JsonDerby;
using Bot.ExtendInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DerbyRequest
{
    [Export("DerbyRequest", typeof(IMessageRequest))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DerbyRequest : IMessageRequest
    {
        private static string strExecutePath = Assembly.GetEntryAssembly().Location;
        private static string strBaseFolder = Path.GetDirectoryName(strExecutePath);

        private static db DerbyDB = ReadDerbyDB();
        private static db ReadDerbyDB()
        {
            string strContents;
            using (var sr = new StreamReader(strBaseFolder + @"\Lib\Derby\db.json", Encoding.UTF8))
            {
                strContents = sr.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<db>(strContents);
        }
        private static Dictionary<string, string> dicTransCN = ReadTransCN();
        private static Dictionary<string, string> ReadTransCN()
        {
            string strContents;
            using (var sr = new StreamReader(strBaseFolder + @"\Lib\Derby\cn.json", Encoding.UTF8))
            {
                strContents = sr.ReadToEnd();
            }
            JObject jObject = JsonConvert.DeserializeObject<JObject>(strContents);

            Dictionary<string, string> dicTran = new Dictionary<string, string>();
            foreach (JToken jToken in jObject.Children().ToList())
            {
                JProperty jProperty = jToken.ToObject<JProperty>();
                dicTran.Add(jProperty.Name, jProperty.Value.ToString());
                if (!dicTran.ContainsKey(jProperty.Value.ToString()))
                {
                    dicTran.Add(jProperty.Value.ToString(), jProperty.Name);
                }
            }
            return dicTran;
        }
        private static Dictionary<long, string> dicCharator = ReadUserData();
        private static Dictionary<long, string> ReadUserData()
        {
            string strContents;
            string strFinalName = strBaseFolder + @"\Lib\Derby\userData.json";
            if (!File.Exists(strFinalName))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(strFinalName));
                File.Create(strFinalName).Close();
            }
            using (var sr = new StreamReader(strFinalName, Encoding.UTF8))
            {
                strContents = sr.ReadToEnd();
            }
            Dictionary<long, string> result = JsonConvert.DeserializeObject<Dictionary<long, string>>(strContents);
            if (result == null)
            {
                result = new Dictionary<long, string>();
            }
            return result;
        }

        private static Dictionary<string, List<Prize>> dicPrize = ReadPrizeData();
        private static Dictionary<string, List<Prize>> ReadPrizeData()
        {
            string strContents;
            string strFinalName = strBaseFolder + @"\Lib\Derby\prize.json";
            if (!File.Exists(strFinalName))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(strFinalName));
                File.Create(strFinalName).Close();
            }
            using (var sr = new StreamReader(strFinalName, Encoding.UTF8))
            {
                strContents = sr.ReadToEnd();
            }
            Dictionary<string, List<Prize>> result = JsonConvert.DeserializeObject<Dictionary<string, List<Prize>>>(strContents);
            if (result == null)
            {
                result = new Dictionary<string, List<Prize>>();
            }
            return result;
        }

        private List<string> lstType = new List<string>() { "Pre-OP", "OP", "G3", "G2", "G1" };
        private Dictionary<string, string> dicDistance = new Dictionary<string, string>() { { "短", "短距離" }, { "英", "マイル" }, { "中", "中距離" }, { "长", "長距離" } };
        string IMessageRequest.DealGroupRequest(JORecvGroupMsg jsonGrpMsg)
        {
            return DealString(jsonGrpMsg);
        }

        string IMessageRequest.DealPrivateRequest(JORecvGroupMsg jsonGrpMsg)
        {
            return DealString(jsonGrpMsg);
        }

        private string DealString(JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;
            string strRequest = jsonGrpMsg.message.Substring(3).Trim(' ');

            if (strRequest.StartsWith("赛场"))
            {
                strRequest = strRequest.Substring(2).Trim(' ');
                return SearchRace(strRequest);
            }
            else if (strRequest.StartsWith("事件"))
            {
                strRequest = strRequest.Substring(2).Trim(' ');
                return SearchEvent(strRequest, jsonGrpMsg);
            }
            else if (strRequest.StartsWith("绑定角色"))
            {
                strRequest = strRequest.Substring(4).Trim(' ');
                return SearchPlayer(strRequest, jsonGrpMsg);
            }
            else if (strRequest.StartsWith("筛选"))
            {
                strRequest = strRequest.Substring(2).Trim(' ');
                try
                {
                    strMessage = SearchDerby(strRequest, jsonGrpMsg);
                }
                catch
                {

                }


                return strMessage;
            }
            else if (strRequest.StartsWith("科学养马"))
            {
                strRequest = strRequest.Substring(4).Trim(' ');
                return SearchCareer(strRequest, jsonGrpMsg);
            }
            else
            {
                strMessage += "本功能目前支持以下操作：\n";
                strMessage += " 搜索赛场，例句：赛马娘 赛场 2年 G1 长\n";
                strMessage += " 查看具体赛场，例句：赛马娘 赛场 天皇赏\n";
                strMessage += " 查询事件 例句：赛马娘 事件 宝塚記念の後に \n";
                strMessage += " 绑定角色 例句：赛马娘 绑定角色 米浴\n";
                strMessage += " 赛程计算 例句：赛马娘 科学养马 (0-71)\n";
                strMessage += " 筛选马娘 例句：赛马娘 筛选 距离：长 科目：耐 \n";
                strMessage += "注1：部分事件需要绑定角色后才可搜索。\n";
                strMessage += "注2：附带数字可显示20日内的详细计划。\n";
                return strMessage;
            }
        }

        private string SearchCareer(string strRequest, JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;
            Player player = null;
            if (dicCharator.ContainsKey(jsonGrpMsg.user_id))
            {
                strMessage += "你当前绑定的角色是：" + dicCharator[jsonGrpMsg.user_id] + "\n";
                player = DerbyDB.players.Find(derby => derby.charaName == dicCharator[jsonGrpMsg.user_id]);
            }
            else
            {
                strMessage += "你当前没有绑定角色\n";
                return strMessage;
            }

            //必赛日程
            Dictionary<int, RaceListItem> dicCareer = new Dictionary<int, RaceListItem>();
            foreach (string DateNum in player.raceList.Keys)
            {
                Race race = DerbyDB.races.Find(item => item.id == player.raceList[DateNum].id);
                player.raceList[DateNum].race = race;
                dicCareer.Add(int.Parse(DateNum), player.raceList[DateNum]);
            }

            //选赛日程1
            Dictionary<int, List<RaceListItem>> dicDetailPrize = new Dictionary<int, List<RaceListItem>>();
            foreach (string strKey in dicPrize.Keys)
            {
                foreach (Prize prize in dicPrize[strKey])
                {
                    Race race = DerbyDB.races.Find(prizeRace => prizeRace.id == prize.id);
                    RaceListItem item = new RaceListItem();
                    item.id = race.id;
                    item.race = race;
                    item.goal = strKey;
                    if (dicDetailPrize.ContainsKey(race.dateNum))
                    {
                        RaceListItem raceArrange = dicDetailPrize[race.dateNum].Find(raceDiff => raceDiff.id == item.id);
                        if (raceArrange != null)
                        {
                            raceArrange.goal += "|" + item.goal;
                        }
                        else
                        {
                            dicDetailPrize[race.dateNum].Add(item);
                        }
                    }
                    else
                    {
                        List<RaceListItem> lstItem = new List<RaceListItem>();
                        lstItem.Add(item);
                        dicDetailPrize.Add(race.dateNum, lstItem);
                    }
                }
            }
            dicDetailPrize = dicDetailPrize.OrderBy(KeyPair => KeyPair.Key).ToDictionary(KeyPare => KeyPare.Key, KeyPare => KeyPare.Value);

            //杯赛自身冲突
            //[トリプルティアラ]和[クラシック三冠]完全冲突 全看 2年目 10月後半;
            //[春秋マイル]如果不能调节则和[秋シニア三冠]冲突;
            //[春秋マイル]34,45,58,69 マイル
            //3年目 3月後半的大阪杯[春シニア三冠]和高松宮記念[春秋スプリント]冲突
            //[春シニア三冠]中距离 [春秋スプリント]短距离 53
            List<string> lstPriceConflict = new List<string>();
            foreach (int nKey in dicDetailPrize.Keys)
            {
                if (dicDetailPrize[nKey].Count > 1)
                {
                    string strConflict = dicDetailPrize[nKey][0].race.date + "的";
                    foreach (RaceListItem item in dicDetailPrize[nKey])
                    {
                        strConflict += item.race.name + "[" + item.goal + "]" + "和";
                    }
                    strConflict = strConflict.TrimEnd('和') + "冲突";
                    lstPriceConflict.Add(strConflict);
                }
            }

            List<string> lstPriceBadReason = new List<string>();
            List<string> lstPriceGoodReason = new List<string>();
            List<string> lstPriceBad = new List<string>();
            List<RaceListItem> lstRaceFix = new List<RaceListItem>();

            //必赛冲突检测
            foreach (int nKey in dicCareer.Keys)
            {
                if (dicDetailPrize.ContainsKey(nKey))
                {
                    foreach (RaceListItem raceConflict in dicDetailPrize[nKey])
                    {
                        if (raceConflict.id != dicCareer[nKey].id)
                        {
                            lstPriceBadReason.Add(dicCareer[nKey].race.date + "的" + raceConflict.race.name + "由于目标" + dicCareer[nKey].race.name + dicCareer[nKey].goal + "冲突，无法达成" + raceConflict.goal);

                            bool bFixAkiThree = false;
                            if (raceConflict.goal.Contains("秋シニア三冠"))
                            {
                                Race race = DerbyDB.races.Find(item => item.id != raceConflict.id && raceConflict.race.name == item.name);
                                if (!dicCareer.ContainsKey(race.dateNum) || dicCareer[race.dateNum].id == race.id)
                                {
                                    lstPriceBadReason.Add(dicCareer[nKey].race.date + "的秋シニア三冠可以改为参加" + race.date + "的" + race.name + "来完成");
                                    RaceListItem fixRace = new RaceListItem();
                                    fixRace.id = race.id;
                                    fixRace.goal = raceConflict.goal + "*";
                                    fixRace.race = race;
                                    lstRaceFix.Add(fixRace);
                                    bFixAkiThree = true;
                                }
                            }

                            //添加冲突日程
                            if (raceConflict.goal.IndexOf("|") > 0)
                            {
                                string[] prizes = raceConflict.goal.Split('|');
                                foreach (string strPrize in prizes)
                                {
                                    if (!lstPriceBad.Contains(strPrize))
                                    {
                                        if (strPrize == "秋シニア三冠" && bFixAkiThree)
                                        {
                                            continue;
                                        }
                                        lstPriceBad.Add(strPrize);
                                    }
                                }
                            }
                            else
                            {
                                if (!lstPriceBad.Contains(raceConflict.goal))
                                {
                                    if (raceConflict.goal == "秋シニア三冠" && bFixAkiThree)
                                    {
                                        continue;
                                    }
                                    lstPriceBad.Add(raceConflict.goal);
                                }
                            }
                        }
                        else
                        {
                            lstPriceGoodReason.Add(dicCareer[nKey].race.date + "的" + dicCareer[nKey].race.name + "可以完成" + raceConflict.goal);
                        }
                    }
                }
            }

            dicDetailPrize.Clear();
            foreach (string strKey in dicPrize.Keys)
            {
                if (lstPriceBad.Contains(strKey))
                {
                    continue;
                }

                if (!lstPriceBad.Contains("春秋スプリント") && !lstPriceBad.Contains("春シニア三冠"))
                {
                    if (GetRankNum(player.mediumDistance) > GetRankNum(player.shortDistance))
                    {
                        if (strKey == "春秋スプリント")
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (strKey == "春シニア三冠")
                        {
                            continue;
                        }
                    }

                }

                if (!lstPriceBad.Contains("秋シニア三冠") && !lstPriceBad.Contains("春秋マイル"))
                {
                    if (GetRankNum(player.mile) > GetRankNum(player.longDistance))
                    {
                        if (strKey == "秋シニア三冠")
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (strKey == "春秋マイル")
                        {
                            continue;
                        }
                    }
                }

                if (!lstPriceBad.Contains("春秋ダート"))
                {
                    if (GetRankNum(player.dirt) < 6)
                    {
                        if (strKey == "春秋ダート")
                        {
                            continue;
                        }
                    }
                }


                foreach (Prize prize in dicPrize[strKey])
                {
                    Race race = DerbyDB.races.Find(prizeRace => prizeRace.id == prize.id);
                    RaceListItem item = new RaceListItem();
                    item.id = race.id;
                    item.race = race;
                    item.goal = strKey;
                    RaceListItem fix = lstRaceFix.Find(fixRace => fixRace.race.name == item.race.name && fixRace.id != item.race.id);
                    if (fix != null)
                    {
                        PushItemInDicList(dicDetailPrize, fix);
                        continue;
                    }
                    else
                    {
                        PushItemInDicList(dicDetailPrize, item);
                    }

                }
            }
            dicDetailPrize = dicDetailPrize.OrderBy(KeyPair => KeyPair.Key).ToDictionary(KeyPare => KeyPare.Key, KeyPare => KeyPare.Value);

            Dictionary<int, RaceListItem> dicFinalRacePlan = new Dictionary<int, RaceListItem>();
            for (int n = 0; n < 72; n++)
            {
                RaceListItem raceDay = new RaceListItem();
                if (player.raceList.ContainsKey(n.ToString()))
                {
                    raceDay = player.raceList[n.ToString()];
                }
                if (dicDetailPrize.ContainsKey(n))
                {
                    if (raceDay.id != null)
                    {
                        if (raceDay.id != dicDetailPrize[n][0].id)
                        {
                            throw new Exception("算出错了。");
                        }
                    }
                    else
                    {
                        raceDay = dicDetailPrize[n][0];
                    }
                }
                if (string.IsNullOrEmpty(raceDay.id))
                {
                    raceDay.goal = "空白";
                }
                dicFinalRacePlan.Add(n, raceDay);
            }

            bool nStart = false;
            foreach (int DateNum in dicFinalRacePlan.Keys)
            {
                RaceListItem race = dicFinalRacePlan[DateNum];
                if (string.IsNullOrEmpty(race.id))
                {
                    if (!nStart)
                    {
                        continue;
                    }
                    strMessage += ((DateNum / 24) + 1) + "年目 " + (((DateNum % 24) / 2) + 1) + "月" + ((((DateNum % 24) % 2) == 0) ? "前半" : "後半") + "/" + race.goal + "\n";
                }
                else
                {
                    nStart = true;
                    strMessage += race.race.date + "/" + race.race.name + "/" + race.race.distanceType + "/" + race.race.grade + "/" + race.race.ground + "/" + race.goal + "\n";
                }

            }
            return strMessage;
        }

        private static void PushItemInDicList(Dictionary<int, List<RaceListItem>> dicDetailPrize, RaceListItem fix)
        {
            if (dicDetailPrize.ContainsKey(fix.race.dateNum))
            {
                RaceListItem raceArrange = dicDetailPrize[fix.race.dateNum].Find(raceDiff => raceDiff.id == fix.race.id);
                if (raceArrange != null)
                {
                    raceArrange.goal += "|" + fix.goal;
                }
                else
                {
                    dicDetailPrize[fix.race.dateNum].Add(fix);
                }
            }
            else
            {
                List<RaceListItem> lstItem = new List<RaceListItem>();
                lstItem.Add(fix);
                dicDetailPrize.Add(fix.race.dateNum, lstItem);
            }
        }

        private int GetRankNum(string strRank)
        {
            switch (strRank)
            {
                case "S":
                    return 8;
                case "A":
                    return 7;
                case "B":
                    return 6;
                case "C":
                    return 5;
                case "D":
                    return 4;
                case "E":
                    return 3;
                case "F":
                    return 2;
                case "G":
                    return 1;
                default:
                    return 0;
            }
        }

        private string SearchPlayer(string strRequest, JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;
            if (string.IsNullOrEmpty(strRequest))
            {
                return "请不要说怪话，好吗";
            }

            List<string> Args = strRequest.Split(' ').ToList();
            List<Player> lstPlayer = DerbyDB.players;
            string strName = strRequest;
            if (Args.Count == 1 && dicTransCN.ContainsKey(Args[0]))
            {
                if (dicTransCN[Args[0]].Length > Args[0].Length)
                {
                    strName = dicTransCN[Args[0]];
                }

            }
            Player target = lstPlayer.Find(derby => derby.charaName.Contains(strName));
            if (target != null)
            {
                if (dicCharator.ContainsKey(jsonGrpMsg.user_id))
                {
                    dicCharator.Remove(jsonGrpMsg.user_id);
                }
                dicCharator.Add(jsonGrpMsg.user_id, target.charaName);
                strMessage += "将君の愛馬设定为：" + target.charaName + " 了";
                OutputJsonFile("userData", JsonConvert.SerializeObject(dicCharator, Formatting.Indented));
            }
            else
            {
                strMessage += "没有找到你说的名字，请从以下名称中选择：\n";
                foreach (Player chara in lstPlayer)
                {
                    strMessage += "　" + chara.charaName + " / " + dicTransCN[chara.charaName] + "\n";
                }
                strMessage.TrimEnd('\n');
            }

            return strMessage;
        }

        private string SearchDerby(string strRequest, JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;
            if (string.IsNullOrEmpty(strRequest))
            {
                return "请输入筛选内容";
            }

            List<string> Args = strRequest.Split(' ').ToList();
            List<Player> lstPlayer = DerbyDB.players;

            List<Player> result = new List<Player>(lstPlayer);
            if (Args.Count > 0)
            {
                foreach (string Condition in Args)
                {
                    string strKey = Condition.Split('：')[0];
                    string strValue = Condition.Split('：')[1];
                    if (strKey == "场地")
                    {
                        if (strValue == "草")
                        {
                            result = result.FindAll(X =>
                            {
                                return RankToInt(X.grass) > RankToInt("C");
                            });
                        }
                        if (strValue == "泥")
                        {
                            result = result.FindAll(X =>
                            {
                                return RankToInt(X.grass) > RankToInt("C");
                            });
                        }
                    }
                    if (strKey == "科目")
                    {
                        if (strValue == "速")
                        {
                            result = result.FindAll(X =>
                            {
                                return X.speedGrow.Trim('+') != "0%";
                            });
                        }
                        if (strValue == "耐")
                        {
                            result = result.FindAll(X =>
                            {
                                return X.staminaGrow.Trim('+') != "0%";
                            });
                        }
                        if (strValue == "力")
                        {
                            result = result.FindAll(X =>
                            {
                                return X.powerGrow.Trim('+') != "0%";
                            });
                        }
                        if (strValue == "根")
                        {
                            result = result.FindAll(X =>
                            {
                                return X.gutsGrow.Trim('+') != "0%";
                            });
                        }
                        if (strValue == "智")
                        {
                            result = result.FindAll(X =>
                            {
                                return X.wisdomGrow.Trim('+') != "0%";
                            });
                        }
                    }
                }
            }

            if (result.Count > 0)
            {
                foreach (Player Derby in result)
                {
                    strMessage += dicTransCN[Derby.charaName] + " 场地 草：" + Derby.grass + " 泥：" + Derby.dirt + "\n";
                    strMessage += "科目 速：" + Derby.speedGrow + " 耐：" + Derby.staminaGrow + " 力：" + Derby.powerGrow + " 根：" + Derby.gutsGrow + " 智：" + Derby.wisdomGrow + "\n";
                    strMessage += " ========== \n";
                }
            }
            else
            {
                strMessage += "规范用语：\n";
                strMessage += "场地：草 泥\n";
                strMessage += "距离：短 英 中 长\n";
                strMessage += "适性：逃 先 差 追\n";
                strMessage += "科目：速 耐 力 根 智\n";
            }

            return strMessage;
        }

        private string SearchEvent(string strRequest, JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;
            if (string.IsNullOrEmpty(strRequest))
            {
                return "请不要说怪话，好吗";
            }

            List<string> Args = strRequest.Split(' ').ToList();
            List<Event> lstEvent = DerbyDB.events;

            if (Args.Count == 1)
            {
                Player player = null;
                if (dicCharator.ContainsKey(jsonGrpMsg.user_id))
                {
                    strMessage += "你当前绑定的角色是：" + dicCharator[jsonGrpMsg.user_id] + "\n";
                    player = DerbyDB.players.Find(derby => derby.charaName == dicCharator[jsonGrpMsg.user_id]);
                }
                else
                {
                    strMessage += "你当前没有绑定角色\n";
                }
                List<Event> events = lstEvent.FindAll(eve => eve.name != null && eve.name.Contains(Args[0]));

                Event target = events[0];
                if (player != null && events.Count > 30)
                {
                    target = events.Find(eve => player.eventList.Contains(eve.id));
                }

                foreach (JArray choice in target.choiceList)
                {
                    List<JToken> lstResult = choice[1].ToList();
                    string strResult = string.Empty;
                    foreach (JToken item in lstResult)
                    {
                        strResult += item.ToString() + "/";
                    }
                    strResult = strResult.Trim('/');
                    strMessage += choice[0] + "：" + strResult + "\n";
                }
            }
            strMessage = strMessage.TrimEnd('\n');
            return strMessage;
        }

        private string SearchRace(string strRequest)
        {
            string strMessage = string.Empty;
            if (string.IsNullOrEmpty(strRequest))
            {
                return "请不要说怪话，好吗";
            }

            List<string> Args = strRequest.Split(' ').ToList();
            List<Race> lstRaces = DerbyDB.races;

            if (Args.Count == 1 && lstRaces.FindAll(race => race.name.Contains(Args[0])).Count <= 2)
            {
                lstRaces = lstRaces.FindAll(race => race.name.Contains(Args[0]));
                foreach (Race race in lstRaces)
                {
                    strMessage += race.date + "/" + race.name + "/" + race.grade + "/" + race.ground + "/" + race.distanceType + "\n";
                }
                return strMessage;
            }

            //年度
            string strYear = Args.Find(strArgs => strArgs.Contains("年"));
            if (!string.IsNullOrEmpty(strYear))
            {
                lstRaces = lstRaces.FindAll(race => race.date.StartsWith(strYear));
            }
            //G1,G2,G3
            string strType = Args.Find(strArgs => lstType.Contains(strArgs));
            if (!string.IsNullOrEmpty(strType))
            {
                lstRaces = lstRaces.FindAll(race => race.grade == strType);
            }
            string strDistance = Args.Find(strArgs => dicDistance.Keys.Contains(strArgs));
            if (!string.IsNullOrEmpty(strDistance))
            {
                string strKeyCondition = dicDistance[strDistance];
                lstRaces = lstRaces.FindAll(race => race.distanceType == strKeyCondition);
            }

            foreach (Race race in lstRaces)
            {
                strMessage += race.date + "/" + race.name + "/" + race.grade + "/" + race.ground + "/" + race.distanceType + "\n";
            }
            strMessage.TrimEnd('\n');
            return strMessage;
        }



        public int RankToInt(string str)
        {
            int nValue = 0;
            switch (str.ToUpper())
            {
                case "A":
                    nValue = 7;
                    break;
                case "B":
                    nValue = 6;
                    break;
                case "C":
                    nValue = 5;
                    break;
                case "D":
                    nValue = 4;
                    break;
                case "E":
                    nValue = 3;
                    break;
                case "F":
                    nValue = 2;
                    break;
                case "G":
                    nValue = 1;
                    break;
            }

            return nValue;
        }

        public static void OutputJsonFile(string strFileName, string strContent)
        {
            string strBasePath = strBaseFolder + @"\Lib\Derby\" + strFileName + ".json";
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
