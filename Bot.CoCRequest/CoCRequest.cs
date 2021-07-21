using Bot.CoCRequest.Json;
using Bot.CoCRequest.Objects;
using Bot.ExtendInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bot.CoCRequest
{
    [Export("CoCRequest", typeof(IMessageRequest))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CoCRequest : IMessageRequest
    {
        Random rnd = new Random(DateTime.Now.Millisecond);
        Regex regCQCode = new Regex(@"(?:\[CQ:.*\])");

        List<Character> lstChara = null;
        public CoCRequest()
        {
            ReadChara();
        }

        private void ReadChara()
        {
            string strExecutePath = Assembly.GetEntryAssembly().Location;
            string strBaseFolder = Path.GetDirectoryName(strExecutePath);

            if (!Directory.Exists(Path.GetDirectoryName(strBaseFolder + Constants.Path_Characters)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(strBaseFolder + Constants.Path_Characters));
            }
            if (!File.Exists(strBaseFolder + Constants.Path_Characters))
            {
                File.Create(strBaseFolder + Constants.Path_Characters).Close();
            }
            string strContents;
            using (var sr = new StreamReader(strBaseFolder + Constants.Path_Characters, Encoding.UTF8))
            {
                strContents = sr.ReadToEnd();
            }
            if (string.IsNullOrEmpty(strContents))
            {
                lstChara = new List<Character>();
                return;
            }
            lstChara = JsonConvert.DeserializeObject<List<Character>>(strContents);
        }

        string IMessageRequest.DealGroupRequest(JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = jsonGrpMsg.message;
            string strReturn = string.Empty;

            while (regCQCode.IsMatch(strMessage))
            {
                strMessage = regCQCode.Replace(strMessage, "");
            }

            if (strMessage.IndexOf("车卡") > 0)
            {
                strReturn = CreateInvestigator(jsonGrpMsg, strMessage);
            }
            else if (strMessage.IndexOf("查询") > 0)
            {
                strReturn = CheckCharacter(jsonGrpMsg, strMessage);
            }
            else if (strMessage.IndexOf("理智增强") > 0)
            {
                strReturn = SanRC(jsonGrpMsg, strMessage);
            }
            else if (strMessage.IndexOf("检定") > 0)
            {
                strReturn = Checkout(jsonGrpMsg, strMessage);
            }
            else
            {
                strReturn = "目前可用的功能有：\n";
                strReturn += "车卡，查询，检定，理智增强\n";
            }


            return strReturn;
        }

        private string Checkout(JORecvGroupMsg jsonGrpMsg, string strMessage)
        {
            strMessage = strMessage.Substring(strMessage.IndexOf("检定") + 2).Trim();
            string[] strArgs = strMessage.Split(' ');
            string strReturn = string.Empty;

            Character Check = lstChara.Find(Player => Player.Groupid == jsonGrpMsg.group_id && Player.Userid == jsonGrpMsg.sender.user_id);
            if (Check == null)
            {
                strReturn = "您在本群还没有建立角色，请使用「CoC 车卡」来创建角色";
                return strReturn;
            }

            DiceResult dice;
            string strProp = string.Empty;
            int nStatus = 0;
            if (strArgs.Length >= 1 && !string.IsNullOrEmpty(strArgs[0]))
            {
                if (Constants.dicKeywords.ContainsKey(strArgs[0]))
                {
                    strProp = strArgs[0];
                    nStatus = Constants.GetPropValue(Check, Constants.dicKeywords[strProp]);
                    dice = Constants.RollDices(1, 100);
                    strReturn += "你的" + strProp + "有：" + nStatus + " 检定点数为：" + dice.Point + "\n";
                    strReturn += Constants.CheckResult(nStatus, dice.Point);
                }
                else
                {
                    foreach (string Key in Constants.dicKeywords.Keys)
                    {
                        strProp += Key + ",";
                    }
                    strReturn += "目前可以检定的属性有：" + strProp.TrimEnd(',');
                }
            }
            else
            {
                //随机抽一个
                dice = Constants.RollDices(1, Constants.dicKeywords.Count);
                strProp = Constants.dicKeywords.Keys.ToList()[dice.Point];
                strReturn += "你没说要检定的内容，那就测个" + strProp + "吧\n";
                //扔个属性
                nStatus = Constants.GetPropValue(Check, Constants.dicKeywords[strProp]);
                dice = Constants.RollDices(1, 100);
                strReturn += "你的" + strProp + "有：" + nStatus + " 检定点数为：" + dice.Point + "\n";
                strReturn += Constants.CheckResult(nStatus, dice.Point);
            }
            return strReturn;
        }
        private string CheckCharacter(JORecvGroupMsg jsonGrpMsg, string strMessage)
        {
            strMessage = strMessage.Substring(strMessage.IndexOf("查询") + 2).Trim();
            string[] strArgs = strMessage.Split(' ');
            string strReturn = string.Empty;

            Character Check = lstChara.Find(Player => Player.Groupid == jsonGrpMsg.group_id && Player.Userid == jsonGrpMsg.sender.user_id);
            if (Check == null)
            {
                strReturn = "您在本群还没有建立角色，请使用「CoC 车卡」来创建角色";
                return strReturn;
            }

            return Check.ToString();
        }
        private string SanRC(JORecvGroupMsg jsonGrpMsg, string strMessage)
        {
            strMessage = strMessage.Substring(strMessage.IndexOf("理智增强") + 4).Trim();
            string[] strArgs = strMessage.Split(' ');
            string strReturn = string.Empty;

            Character Check = lstChara.Find(Player => Player.Groupid == jsonGrpMsg.group_id && Player.Userid == jsonGrpMsg.sender.user_id);
            if (Check == null)
            {
                strReturn = "您在本群还没有建立角色，请使用「CoC 车卡」来创建角色";
                return strReturn;
            }

            DiceResult dice = Constants.RollRC(Check.SAN);
            Check.SAN += dice.Point;
            Constants.OutputJsonFile(Constants.Path_Characters, JsonConvert.SerializeObject(lstChara, Formatting.Indented));

            strReturn = dice.Result + "当前理智为：" + Check.SAN;
            return strReturn;
        }
        private string CreateInvestigator(JORecvGroupMsg jsonGrpMsg, string strMessage)
        {
            strMessage = strMessage.Substring(strMessage.IndexOf("车卡") + 2).Trim();
            string[] strArgs = strMessage.Split(' ');
            string strReturn = string.Empty;

            Character Check = lstChara.Find(Player => Player.Groupid == jsonGrpMsg.group_id && Player.Userid == jsonGrpMsg.sender.user_id);
            if (Check != null)
            {
                lstChara.Remove(Check);
                strReturn += "之前的角色我就删啦！\n";
            }


            int nAge = 0;
            if (strArgs.Length > 0)
            {
                if (!int.TryParse(strArgs[0].TrimEnd('岁'), out nAge))
                {
                    nAge = (rnd.Next(75) + 14);
                    strReturn += "不输入年龄我就随便写啦，" + nAge.ToString() + "岁\n";
                }
                else
                {
                    if (nAge >= 90 || nAge < 15)
                    {
                        nAge = (Math.Abs(nAge - 15) % 75) + 15;
                        strReturn += "你被强制轮回了，被改为了" + nAge.ToString() + "岁\n";
                    }
                }
            }

            Character Chara = new Character(nAge);
            Chara.Groupid = jsonGrpMsg.group_id;
            Chara.Userid = jsonGrpMsg.sender.user_id;
            Chara.Sex = jsonGrpMsg.sender.sex;
            if (jsonGrpMsg.group_id != 0)
            {
                Chara.NickName = jsonGrpMsg.sender.card == "" ? jsonGrpMsg.sender.nickname : jsonGrpMsg.sender.card;
            }
            else
            {
                Chara.NickName = jsonGrpMsg.sender.nickname;
            }

            lstChara.Add(Chara);

            Constants.OutputJsonFile(Constants.Path_Characters, JsonConvert.SerializeObject(lstChara, Formatting.Indented));
            strReturn += Chara.CreateLog + Chara.ToString();
            return strReturn;
        }




        string IMessageRequest.DealPrivateRequest(JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = jsonGrpMsg.message;
            string strReturn = string.Empty;

            while (regCQCode.IsMatch(strMessage))
            {
                strMessage = regCQCode.Replace(strMessage, "");
            }

            if (strMessage.IndexOf("检定") > 0)
            {
                strReturn = Checkout(jsonGrpMsg, strMessage);
            }
            else
            {
                strReturn = "目前可用的功能有：\n";
                strReturn += "检定\n";
            }


            return strReturn;
        }


    }
}
