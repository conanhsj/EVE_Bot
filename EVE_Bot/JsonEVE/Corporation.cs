    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_Bot.JsonEVE
{
    public class Corporation
    {
        public long alliance_id;
        public long ceo_id;
        public long creator_id;
        public DateTime date_founded;
        public string description;
        public long home_station_id;
        public int member_count;
        public string name;
        public long shares;
        public double tax_rate;
        public string ticker;
        public string url;
        public bool war_eligible;
    }
}
