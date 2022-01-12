using Bot.CoCRequest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.CoCRequest.Json
{
    //Investigator 
    public class Character
    {
        private long groupid;
        private long userid;
        private string nickName;
        private string sex;
        private int age;
        //基础属性
        private int sTR, cON, sIZ, dEX, aPP, iNT, pOW, eDU, lKY;
        private string createLog = string.Empty;

        //衍生属性
        private int sAN, mP, hP, dB, pHY;
        private int mOV;

        private string job;

        private List<Skill> skills;

        //本职技能点
        private int mainSkillPoint;
        //技能点
        private int interestPoint;
        //当前状态
        private string status;

        private string impression, belief, important, place, treasure, special;

        public Character(int nAge)
        {

            age = nAge;
            DiceResult dice = Constants.RollDices(3, 6);
            CreateLog += "力量的结果为：" + dice.Result + "\n";
            STR = dice.Point * 5;

            dice = Constants.RollDices(3, 6);
            CreateLog += "体质的结果为：" + dice.Result + "\n";
            CON = dice.Point * 5;

            dice = Constants.RollDices(2, 6);
            CreateLog += "体型的结果为：" + dice.Result + "\n";
            SIZ = (dice.Point + 6) * 5;

            dice = Constants.RollDices(3, 6);
            CreateLog += "敏捷的结果为：" + dice.Result + "\n";
            DEX = dice.Point * 5;

            dice = Constants.RollDices(3, 6);
            CreateLog += "外貌的结果为：" + dice.Result + "\n";
            APP = dice.Point * 5;

            dice = Constants.RollDices(2, 6);
            CreateLog += "智力的结果为：" + dice.Result + "\n";
            INT = (dice.Point + 6) * 5;

            dice = Constants.RollDices(3, 6);
            CreateLog += "意志的结果为：" + dice.Result + "\n";
            POW = dice.Point * 5;

            dice = Constants.RollDices(2, 6);
            CreateLog += "教育的结果为：" + dice.Result + "\n";
            EDU = (dice.Point + 6) * 5;

            dice = Constants.RollDices(3, 6);
            CreateLog += "幸运的结果为：" + dice.Result + "\n";
            LKY = dice.Point * 5;

            AgeFix(nAge);

            if (cON < 0)
            {
                return;
            }

            //衍生属性
            sAN = POW;
            mP = POW / 5;
            hP = (sIZ + cON) / 10;
            MovFix(nAge);

            dice = Constants.RollDices(1, ConstInfo.lstJobs.Count);
            job = ConstInfo.lstJobs[dice.Point - 1].Name;
            Constants.SelectJob(this, ConstInfo.lstJobs[dice.Point - 1]);

            interestPoint = iNT * 2;

            impression = "无";
            belief = ConstInfo.lstBelief[Constants.RollDices(1, ConstInfo.lstBelief.Count).Point - 1];
            important = ConstInfo.lstVIP[Constants.RollDices(1, ConstInfo.lstVIP.Count).Point - 1] + "因为"+ ConstInfo.lstReason[Constants.RollDices(1, ConstInfo.lstReason.Count).Point - 1];
            place = ConstInfo.lstPlace[Constants.RollDices(1, ConstInfo.lstPlace.Count).Point - 1];
            treasure = ConstInfo.lstTreasure[Constants.RollDices(1, ConstInfo.lstTreasure.Count).Point - 1];
            special = ConstInfo.lstSpecial[Constants.RollDices(1, ConstInfo.lstSpecial.Count).Point - 1];
        }

        //移动力调整
        private void MovFix(int nAge)
        {
            if (dEX < sIZ && sTR < sIZ)
            {
                mOV = 7;
            }
            else if (dEX <= sIZ || sTR <= sIZ)
            {
                mOV = 8;
            }
            else
            {
                mOV = 9;
            }

            if (nAge >= 40 && nAge < 50)
            {
                mOV -= 1;
            }
            else if (nAge >= 50 && nAge < 60)
            {
                mOV -= 2;
            }
            else if (nAge >= 60 && nAge < 70)
            {
                mOV -= 3;
            }
            else if (nAge >= 70 && nAge < 80)
            {
                mOV -= 4;
            }
            else if (nAge >= 80 && nAge < 90)
            {
                mOV -= 5;
            }

            return;
        }

        //年龄调整
        private void AgeFix(int nAge)
        {
            DiceResult dice;
            if (nAge >= 15 && nAge < 20)
            {
                dice = Constants.RollDices(1, 5);
                CreateLog += "力量惩罚结果为：" + dice.Point.ToString() + " 体型惩罚：" + (5 - dice.Point).ToString() + "\n";
                STR = STR - dice.Point;
                SIZ = SIZ - (5 - dice.Point);
                CreateLog += "教育惩罚：5 点\n";
                EDU = EDU - 5;
                dice = Constants.RollDices(3, 6);
                CreateLog += "额外幸运的结果为：" + dice.Result;
                if (dice.Point * 5 > LKY)
                {
                    CreateLog += "成功\n";
                    LKY = dice.Point * 5;
                }
                else
                {
                    CreateLog += "失败\n";
                }
            }
            else if (nAge >= 20 && nAge < 40)
            {
                EduRefCheck();
                if (eDU > 100)
                {
                    eDU = 99;
                }
            }
            else if (nAge >= 40 && nAge < 50)
            {
                EduRefCheck();
                EduRefCheck();
                if (eDU > 100)
                {
                    eDU = 99;
                }
                SenilityDebuff(5);
                CreateLog += "外貌惩罚5点";
                APP = APP - 5;
            }
            else if (nAge >= 50 && nAge < 60)
            {
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                SenilityDebuff(10);
                CreateLog += "外貌惩罚10点";
                APP = APP - 10;
            }
            else if (nAge >= 60 && nAge < 70)
            {
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                SenilityDebuff(20);
                CreateLog += "外貌惩罚15点";
                APP = APP - 15;
            }
            else if (nAge >= 70 && nAge < 80)
            {
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                SenilityDebuff(40);
                CreateLog += "外貌惩罚20点";
                APP = APP - 20;
            }
            else if (nAge >= 80 && nAge < 90)
            {
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                SenilityDebuff(80);
                CreateLog += "外貌惩罚25点";
                APP = APP - 25;
            }

            return;
        }

        //衰老惩罚
        private DiceResult SenilityDebuff(int nBaseDebuff)
        {
            DiceResult dice = Constants.RollDices(1, nBaseDebuff);
            if (dice.Point < sTR)
            {
                CreateLog += "力量衰老" + dice.Point + "点\n";
                sTR = sTR - dice.Point;
                nBaseDebuff = nBaseDebuff - dice.Point;
            }
            else
            {
                CreateLog += "力量衰老" + sTR + "点\n";
                nBaseDebuff = nBaseDebuff - sTR;
                sTR = 0;
            }
            if (nBaseDebuff > 0)
            {
                dice = Constants.RollDices(1, nBaseDebuff);
                if (dice.Point < dEX)
                {
                    CreateLog += "敏捷衰老" + dice.Point + "点\n";
                    dEX = dEX - dice.Point;
                    nBaseDebuff = nBaseDebuff - dice.Point;
                }
                else
                {
                    CreateLog += "敏捷衰老" + dEX + "点\n";
                    nBaseDebuff = nBaseDebuff - dEX;
                    dEX = 0;
                }
            }
            if (nBaseDebuff > 0)
            {
                if (nBaseDebuff <= cON)
                {
                    CreateLog += "体质衰老" + nBaseDebuff + "点\n";
                    cON = cON - nBaseDebuff;
                }
                else
                {
                    CreateLog += "体质衰老" + (cON - 1) + "点\n";
                    nBaseDebuff = nBaseDebuff - (cON - 1);
                    cON = 1;
                    if (nBaseDebuff <= sTR)
                    {
                        CreateLog += "力量追加衰老" + nBaseDebuff + "点\n";
                        sTR = sTR - nBaseDebuff;
                        nBaseDebuff = 0;
                    }
                    else
                    {
                        CreateLog += "力量追加衰老" + sTR + "点\n";
                        nBaseDebuff = nBaseDebuff - sTR;
                        sTR = 0;
                    }
                    if (nBaseDebuff != 0 && nBaseDebuff <= dEX)
                    {
                        CreateLog += "敏捷追加衰老" + nBaseDebuff + "点\n";
                        dEX = dEX - nBaseDebuff;
                        nBaseDebuff = 0;
                    }
                    else
                    {
                        CreateLog += "敏捷追加衰老" + dEX + "点\n";
                        nBaseDebuff = nBaseDebuff - dEX;
                        dEX = 0;
                    }
                    if (nBaseDebuff > 0)
                    {
                        CreateLog += "体质追加衰老" + nBaseDebuff + "点\n";
                        cON = cON - nBaseDebuff;
                        CreateLog += "你因衰老而死了\n";
                    }
                }
            }

            return dice;
        }

        //教育增强检定
        private void EduRefCheck()
        {
            DiceResult dice = Constants.RollDices(1, 100);
            CreateLog += "教育增强检定";
            if (dice.Point > eDU)
            {
                CreateLog += "成功,";
                dice = Constants.RollDices(1, 10);
                CreateLog += "教育增加" + dice.Point + "点\n";
                eDU += dice.Point;
            }
            else
            {
                CreateLog += "失败\n";
            }
            return;
        }


        public long Groupid { get => groupid; set => groupid = value; }
        public long Userid { get => userid; set => userid = value; }
        public string NickName { get => nickName; set => nickName = value; }
        public int Age { get => age; set => age = value; }
        public int STR { get => sTR; set => sTR = value; }
        public int CON { get => cON; set => cON = value; }
        public int SIZ { get => sIZ; set => sIZ = value; }
        public int DEX { get => dEX; set => dEX = value; }
        public int APP { get => aPP; set => aPP = value; }
        public int INT { get => iNT; set => iNT = value; }
        public int POW { get => pOW; set => pOW = value; }
        public int EDU { get => eDU; set => eDU = value; }
        public int LKY { get => lKY; set => lKY = value; }
        public string CreateLog { get => createLog; set => createLog = value; }
        public string Sex { get => sex; set => sex = value; }
        public int SAN { get => sAN; set => sAN = value; }
        public int MP { get => mP; set => mP = value; }
        public int HP { get => hP; set => hP = value; }
        public int DB { get => dB; set => dB = value; }
        public int PHY { get => pHY; set => pHY = value; }
        public int MOV { get => mOV; set => mOV = value; }
        public string Status { get => status; set => status = value; }
        public int InterestPoint { get => interestPoint; set => interestPoint = value; }
        public string Job { get => job; set => job = value; }
        public List<Skill> Skills { get => skills; set => skills = value; }
        public int MainSkillPoint { get => mainSkillPoint; set => mainSkillPoint = value; }
        public string Impression { get => impression; set => impression = value; }
        public string Belief { get => belief; set => belief = value; }
        public string Important { get => important; set => important = value; }
        public string Place { get => place; set => place = value; }
        public string Treasure { get => treasure; set => treasure = value; }
        public string Special { get => special; set => special = value; }

        public override string ToString()
        {
            string strValue = string.Empty;

            try
            {

                strValue += "名称：" + NickName + " 年龄：" + Age.ToString() + " 性别：" + Sex + "\n";
                strValue += " ————基础属性————\n";
                strValue += "力量：" + STR.ToString() + " " + Constants.GetDescription(ConstInfo.lstSTR, sTR) + "\n";
                strValue += "体质：" + CON.ToString() + " " + Constants.GetDescription(ConstInfo.lstCON, cON) + "\n";
                strValue += "体型：" + SIZ.ToString() + " " + Constants.GetDescription(ConstInfo.lstSIZ, sIZ) + "\n";
                strValue += "敏捷：" + DEX.ToString() + " " + Constants.GetDescription(ConstInfo.lstDEX, dEX) + "\n";
                strValue += "外貌：" + APP.ToString() + " " + Constants.GetDescription(ConstInfo.lstAPP, APP) + "\n";
                strValue += "智力：" + INT.ToString() + " " + Constants.GetDescription(ConstInfo.lstINT, INT) + "\n";
                strValue += "意志：" + POW.ToString() + " " + Constants.GetDescription(ConstInfo.lstPOW, pOW) + "\n";
                strValue += "教育：" + EDU.ToString() + " " + Constants.GetDescription(ConstInfo.lstEDU, eDU) + "\n";
                strValue += "幸运：" + LKY.ToString() + "\n";
                if (cON > 0)
                {
                    strValue += " ————衍生属性————\n";
                    strValue += "生命：" + HP.ToString() + " 魔力：" + MP.ToString() + " 理智：" + SAN.ToString() + "移动：" + MOV.ToString() + "\n";
                }
                else
                {
                    return strValue;
                }

                if (!string.IsNullOrEmpty(Job))
                {
                    strValue += " ————职业属性————\n";
                    strValue += "职业：" + Job.ToString() + " 本职点：" + mainSkillPoint.ToString() + " 兴趣点：" + interestPoint.ToString() + "\n";
                    foreach (Skill item in skills)
                    {
                        strValue += "技能名：" + item.Name + " 基础点数：" + item.BasePoint.ToString() + "\n";
                    }
                }
                if(impression != null)
                {
                    strValue += " ————调查员背景————\n";
                    strValue += "形象描述：" + Impression.ToString() + "\n";
                    strValue += "思想/信念：" + Belief.ToString() + "\n";
                    strValue += "重要之人：" + Important.ToString() + "\n";
                    strValue += "意义非凡之地：" + Place.ToString() + "\n";
                    strValue += "宝贵之物：" + Treasure.ToString() + "\n";
                    strValue += "特质：" + Special.ToString() + "\n";
                }

            }
            catch (Exception ex)
            {
                strValue += ex.ToString();
            }


            return strValue;
        }


    }
}
