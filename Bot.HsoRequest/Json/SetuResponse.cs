using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.HsoRequest.Json
{
    public class SetuResponse
    {
        public int code;
        public string message;
        public int count;
        public List<SetuData> data;
    }
}
