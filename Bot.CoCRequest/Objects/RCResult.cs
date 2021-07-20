using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.CoCRequest.Objects
{
    public class RCResult
    {
        private string result;
        private bool success;
        private int point;

        public string Result { get => result; set => result = value; }
        public bool Success { get => success; set => success = value; }
        public int Point { get => point; set => point = value; }
    }
}
