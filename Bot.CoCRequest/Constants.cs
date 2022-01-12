using Bot.CoCRequest.Json;
using Bot.CoCRequest.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bot.CoCRequest
{
    public static class Constants
    {
        public static Regex regCQCode = new Regex(@"(?:\[CQ:.*\])");

        private static Random rnd = new Random(DateTime.Now.Millisecond);

        public static string Menu_ChangeName = @"更名";
        public static string Menu_CheckJobs = @"查看职业";
        public static string Menu_SelectJobs = @"选择职业";

        public static string Path_Base = @"CoC7th";
        public static string Path_Characters = @"\Lib\" + Path_Base + @"\Characters.json";

        public static Dictionary<string, string> dicKeywords = new Dictionary<string, string>()
        {
            { "力量","STR" },
            { "体质","CON" },
            { "体型","SIZ" },
            { "敏捷","DEX" },
            { "外貌","APP" },
            { "智力","INT" },
            { "意志","POW" },
            { "教育","EDU" },
            { "幸运","LKY" },
        };




        public static List<SanCheckPattern> lstSanCheck = new List<SanCheckPattern>()
        {
            new SanCheckPattern(){ Event= ""}
        };



        public static void SelectJob(Character Check, Job tar)
        {
            Check.Job = tar.Name;
            if (tar.SkillPointMemo.Contains("＋"))
            {
                string strBaseStatus = tar.SkillPointMemo.Replace("ｘ２", "");
                string[] perp = strBaseStatus.Split('＋');
                string[] AttachStatus = perp[1].Split('或');
                int nMax = 0;
                foreach (string strProp in AttachStatus)
                {
                    nMax = nMax > Constants.GetPropValue(Check, Constants.dicKeywords[strProp]) ? nMax : Constants.GetPropValue(Check, Constants.dicKeywords[strProp]);
                }
                Check.MainSkillPoint = Check.EDU * 2 + nMax * 2;
            }
            else
            {
                Check.MainSkillPoint = Check.EDU * 4;
            }

            List<string> MainSkills = tar.Description.Split('，').ToList();

            List<Skill> Skill = new List<Skill>();
            foreach (string strSkill in MainSkills)
            {
                Skill main = new Skill();
                main.Name = strSkill;
                Skill.Add(main);
            }
            Check.Skills = Skill;

        }

        public static DiceResult RollDices(int Count, int MaxPoint)
        {
            DiceResult result = new DiceResult();
            string strValue = string.Empty;
            int nSum = 0;
            for (int n = 0; n < Count; n++)
            {
                int nResult = (rnd.Next(MaxPoint) + 1);
                strValue += nResult + "+";
                nSum += nResult;
            }

            result.Result = strValue.TrimEnd('+') + " = " + nSum;
            result.Point = nSum;

            return result;
        }

        //ReinforceCheckout
        public static DiceResult RollRC(int Base)
        {
            DiceResult dice = Constants.RollDices(1, 100);
            if (dice.Point > Base)
            {
                dice.Result = "成功,";
                DiceResult buff = Constants.RollDices(1, 10);
                dice.Result += "增加" + buff.Point + "点\n";
                dice.Point = buff.Point;
            }
            else
            {
                dice.Result = "失败\n";
                dice.Point = 0;
            }
            return dice;
        }

        public static void OutputJsonFile(string strFileName, string strContent)
        {
            string strExecutePath = Assembly.GetEntryAssembly().Location;
            string strBaseFolder = Path.GetDirectoryName(strExecutePath);

            if (!Directory.Exists(Path.GetDirectoryName(strBaseFolder + strFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(strBaseFolder + strFileName));
            }
            using (var sw = new StreamWriter(strBaseFolder + strFileName, false, Encoding.Unicode))
            {
                sw.Write(strContent);
            }
            //Environment.
        }

        public static int GetPropValue(object src, string propName)
        {
            object Prop = src.GetType().GetProperty(propName).GetValue(src, null);
            int Value = 0;
            int.TryParse(Prop.ToString(), out Value);

            return Value;
        }

        public static string CheckResult(int BaseStatus, int CheckResult)
        {
            string strResult = string.Empty;

            if (CheckResult == 1)
            {
                strResult = "大成功";
            }
            else if (CheckResult <= (BaseStatus / 5))
            {
                strResult = "极难成功";
            }
            else if (CheckResult <= (BaseStatus / 2))
            {
                strResult = "困难成功";
            }
            else if (CheckResult <= BaseStatus)
            {
                strResult = "检定成功";
            }
            else if (CheckResult >= 95)
            {
                strResult = "大失败";
            }
            else if (CheckResult > BaseStatus)
            {
                strResult = "失败";
            }


            return strResult;
        }

        public static string GetDescription(List<DescriptionStatus> lstDescription, int Score)
        {
            string strReturn = string.Empty;

            lstDescription = lstDescription.OrderBy(X => X.Point).ToList();

            DescriptionStatus lower = null;
            DescriptionStatus higher = null;

            foreach (DescriptionStatus Min in lstDescription)
            {
                if (Min.Point <= Score)
                {
                    lower = Min;
                }
            }

            if (lower == null)
            {
                return "没找到，这东西坏了";
            }
            if (lower.Point == Score)
            {
                return lower.Result;
            }

            foreach (DescriptionStatus Max in lstDescription)
            {
                if (Max.Point > Score)
                {
                    higher = Max;
                    break;
                }
            }
            if (higher == null)
            {
                return "超越" + lstDescription[lstDescription.Count].Result;
            }

            int nAverage = (lower.Point + higher.Point) / 2;

            string strAdverb = string.Empty;
            if (Score > nAverage)
            {
                if (higher.Point - Score > 10)
                {
                    strAdverb = "远没到";
                }
                else if (higher.Point - Score > 5)
                {
                    strAdverb = "弱于";
                }
                else
                {
                    strAdverb = "几乎是";
                }
                return strAdverb + higher.Result.ToString();
            }
            else
            {
                if (Score - lower.Point > 10)
                {
                    strAdverb = "远超过";
                }
                else if (Score - lower.Point > 5)
                {
                    strAdverb = "强于";
                }
                else
                {
                    strAdverb = "几乎是";
                }
                return strAdverb + lower.Result.ToString();
            }
        }
    }
}
