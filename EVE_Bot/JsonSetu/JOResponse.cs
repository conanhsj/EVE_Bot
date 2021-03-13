using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_Bot.JsonSetu
{
    public class JOResponse
    {
        public int code;
        public string message;
        public int count;
        public List<JOData> data;
    }
}
