using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.ExtendInterface
{
    public class JORecvGroupMsg
    {
        public Int64 time;
        public Int64 self_id;
        public string post_type;
        public string message_type;
        public string sub_type;
        public string message_id;
        public Int64 group_id;
        public Int64 user_id;
        public string anonymous;
        public string message;
        public string raw_message;
        public string font;
        public JOSender sender;

    }
}
