using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_Bot.JsonObject
{
    public class JOAPI
    {
        public string action;

        [JsonProperty(PropertyName = "params")]
        public JOParams joParams;
    }
}
