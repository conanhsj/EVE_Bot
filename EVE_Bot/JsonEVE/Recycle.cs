using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_Bot.JsonEVE
{
    public class Recycle
    {
        public int TypeID;
        public string Name;
        public List<RecycleMtls> Materials = new List<RecycleMtls>();
    }
}
