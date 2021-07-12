using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Eve.Objects.SendMessage
{
    public class Message
    {
        public string action;

        [JsonProperty(PropertyName = "params")]
        public Params joParams;
    }
}
