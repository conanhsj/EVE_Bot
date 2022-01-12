using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bot.HsoRequest
{
    public static class Constants
    {
        public static Regex regCQCode = new Regex(@"(?:\[CQ:.*\])");

    }
}
