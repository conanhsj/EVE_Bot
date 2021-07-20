using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.CoCRequest.Objects
{
    public class DiceResult
    {

        private string result;
        private int point;

        public int Point { get => point; set => point = value; }
        public string Result { get => result; set => result = value; }
    }
}
