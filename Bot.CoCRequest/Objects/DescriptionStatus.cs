using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.CoCRequest.Objects
{
    public class DescriptionStatus
    {
        private int point;
        private string result;

        public int Point { get => point; set => point = value; }
        public string Result { get => result; set => result = value; }
    }
}
