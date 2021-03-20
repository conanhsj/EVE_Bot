using EVE_Bot.Helper;
using EVE_Bot.JsonObject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EVE_Bot.AILogic
{
    public class AIRequest
    {
        public static List<string> lstAdjs = new List<string>();
        public static List<string> lstPronoun = new List<string>();

        public static List<string> lstMe = new List<string>() { "猫娘", "这猫" };
        public static List<string> lstPrep = new List<string>() { "的", "是", "了" };

        public static List<string> lstSelf = new List<string>();
        public static List<string> lstIs = new List<string>();
        public static List<string> lstDid = new List<string>();
        public static List<string> lstBelong = new List<string>();

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

            if (lstMe.Where(Callme => { return strInput.Contains(Callme); }).ToList().Count > 0)
            {
                //单纯叫名字
                if (lstPrep.Where(Callme => { return strInput.Contains(Callme); }).ToList().Count <= 0)
                {
                    strMessage += "叫我吗？";
                }
                lstSelf.Add(strInput);
                FilesHelper.OutputJsonFile("Talking\\Self", JsonConvert.SerializeObject(lstSelf, Formatting.Indented));
            }
            //常见语法1 
            else if (regAdj.IsMatch(strInput))
            {
                string strAdjs = regAdj.Match(strInput).Value;
                strMessage += "哦？这么" + strAdjs.Trim('太', '了');
                if (!lstAdjs.Contains(strAdjs))
                {
                    lstAdjs.Add(strAdjs);
                    FilesHelper.OutputJsonFile("Talking\\Adjs", JsonConvert.SerializeObject(lstAdjs, Formatting.Indented));
                }
            }
            else if (strInput.Contains("是"))
            {
                lstIs.Add(strInput);
                FilesHelper.OutputJsonFile("Talking\\Is", JsonConvert.SerializeObject(lstIs, Formatting.Indented));
            }
            else if (strInput.Contains("了"))
            {
                lstDid.Add(strInput);
                FilesHelper.OutputJsonFile("Talking\\Did", JsonConvert.SerializeObject(lstDid, Formatting.Indented));
            }
            else if (strInput.Contains("的"))
            {
                lstBelong.Add(strInput);
                FilesHelper.OutputJsonFile("Talking\\Belong", JsonConvert.SerializeObject(lstBelong, Formatting.Indented));
            }



            return strMessage;
        }
    }
}
