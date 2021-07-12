using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DerbyRequest.JsonDerby
{
    [JsonObject(MemberSerialization.Fields)]
    public class Race
    {
        public string name;
        public string date;
        public int dateNum;
        public string uniqueName;
        [JsonProperty("class")]
        public string raceclass;
        public string grade;
        public string place;
        public string ground;
        public int distance;
        public string distanceType;
        public string direction;
        public string side;
        public string id;
 
    }
}
