using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_Bot.JsonEVE
{
    [JsonObject(MemberSerialization.Fields)]
    public class Sovereignty
    {
        public long alliance_id;
        public long corporation_id;
        public long faction_id;
        public long system_id;

        public string alliance_name;
        public string corporation_name;
        public string faction_name;
        public string system_name;
        public string constellation_name;
        public string region_name;
    }
}
