using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DerbyRequest.JsonDerby
{
    public class Event
    {
        public string name;
        public List<JArray> choiceList;
        public string id;
        public string pid;
    }
}
