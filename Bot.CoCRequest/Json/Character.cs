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
        }

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

        private void AgeFix(int nAge)
        {
            DiceResult dice;
            if (nAge >= 15 && nAge < 20)
            {
                dice = Constants.RollDices(1, 5);
                CreateLog += "力量惩罚结果为：" + dice.Point.ToString() + " 体型惩罚：" + (5 - dice.Point).ToString() + "\n";
                STR = STR - dice.Point;
                SIZ = SIZ - (5 - dice.Point);
                CreateLog += "教育惩罚：5 点";
                EDU = EDU - 5;
                dice = Constants.RollDices(3, 6);
                CreateLog += "额外幸运的结果为：" + dice.Result;
                if (dice.Point > LKY)
                {
                    CreateLog += "成功\n";
                    LKY = dice.Point;
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
                APP = APP - 5;
            }
            else if (nAge >= 50 && nAge < 60)
            {
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                SenilityDebuff(10);
                APP = APP - 10;
            }
            else if (nAge >= 60 && nAge < 70)
            {
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                SenilityDebuff(20);
                APP = APP - 15;
            }
            else if (nAge >= 70 && nAge < 80)
            {
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                SenilityDebuff(40);
                APP = APP - 20;
            }
            else if (nAge >= 80 && nAge < 90)
            {
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                EduRefCheck();
                SenilityDebuff(80);
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
                sTR = 0;
                nBaseDebuff = nBaseDebuff - sTR;
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
                    dEX = 0;
                    nBaseDebuff = nBaseDebuff - dEX;
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
                    if (nBaseDebuff <= sTR)
                    {
                        CreateLog += "力量追加衰老" + nBaseDebuff + "点\n";
                        sTR = sTR - nBaseDebuff;
                    }
                    else
                    {
                        CreateLog += "力量追加衰老" + sTR + "点\n";
                        sTR = 0;
                        nBaseDebuff = nBaseDebuff - sTR;
                    }
                    if (nBaseDebuff <= dEX)
                    {
                        CreateLog += "敏捷追加衰老" + nBaseDebuff + "点\n";
                        dEX = dEX - nBaseDebuff;
                    }
                    else
                    {
                        CreateLog += "敏捷追加衰老" + dEX + "点\n";
                        dEX = 0;
                        nBaseDebuff = nBaseDebuff - dEX;
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

        public override string ToString()
        {
            string strValue = string.Empty;

            strValue += "名称：" + NickName + " 年龄：" + Age.ToString() + " 性别：" + Sex + "\n";
            strValue += " ————基础属性————\n";
            strValue += "力量：" + STR.ToString() + " 体质：" + CON.ToString() + " 体型：" + SIZ.ToString() + "\n";
            strValue += "敏捷：" + DEX.ToString() + " 外貌：" + APP.ToString() + " 智力：" + INT.ToString() + "\n";
            strValue += "意志：" + POW.ToString() + " 教育：" + EDU.ToString() + " 幸运：" + LKY.ToString() + "\n";
            if (cON > 0)
            {
                strValue += " ————衍生属性————\n";
                strValue += "生命：" + HP.ToString() + " 魔力：" + MP.ToString() + " 理智：" + SAN.ToString() + "\n";
                strValue += "移动：" + MOV.ToString() + "\n";
            }

            return strValue;
        }
    }
}
