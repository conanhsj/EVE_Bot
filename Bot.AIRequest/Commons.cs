
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bot.AIRequest
{
    public static class Commons
    {
        public static Random rnd = new Random(100);

        public static Regex regAdj = new Regex("太(\\w*?)了");
        public static Regex regCQCode = new Regex(@"(?:\[CQ:.*\])");

        public static string RemoveCQCode(string strMessage)
        {
            while (regCQCode.IsMatch(strMessage))
            {
                strMessage = regCQCode.Replace(strMessage, "");
            }
            return strMessage;
        }
    }
}
