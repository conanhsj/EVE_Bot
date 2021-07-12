using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DerbyRequest.JsonDerby
{
    public class Player
    {
        public string name;
        public string grass;
        public string dirt;
        public string shortDistance;
        public string mile;
        public string mediumDistance;
        public string longDistance;
        public string escape;
        public string leading;
        public string insert;
        public string tracking;
        public string speedGrow;
        public string staminaGrow;
        public string powerGrow;
        public string gutsGrow;
        public string wisdomGrow;
        public List<string> skillList;
        public List<string> eventList;
        public string id;
        public string rare;
        public Dictionary<string, RaceListItem> raceList;
        public string gwId;
        public List<string> uniqueSkillList;
        public List<string> initialSkillList;
        public List<string> awakeningSkillList;
        public string imgUrl;
        public string charaName;
        public int db_id;
        public int default_rarity;
    }
}
