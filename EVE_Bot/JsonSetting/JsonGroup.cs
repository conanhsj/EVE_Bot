using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_Bot.JsonSetting
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonGroup
    {
        [JsonProperty]
        public long group_id;
        [JsonProperty]
        public int CoolDownTime;
        [JsonIgnore]
        public long last_time;
        [JsonIgnore]
        public string RepeatCheck;

    }
}
