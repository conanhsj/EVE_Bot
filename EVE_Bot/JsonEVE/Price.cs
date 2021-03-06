using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_Bot.JsonEVE
{
    public class Price
    {
        public string TypeId;
        public string Name;
        public PriceInfo all;
        public PriceInfo buy;
        public PriceInfo sell;
        public DateTime CachedTime;

    }
}
