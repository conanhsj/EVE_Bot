using Bot.Eve.Objects.SendMessage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Eve.Helper
{
    public static class TemplateBuilder
    {
        public static string BuildSendMessage(Int64 GroupId, string Message, bool AutoEscape)
        {
            Params jsonParams = new Params();
            jsonParams.group_id = GroupId;
            jsonParams.message = Message;
            jsonParams.auto_escape = AutoEscape;
            Message jsonAPI = new Message();
            jsonAPI.action = "send_group_msg";
            jsonAPI.joParams = jsonParams;
            string strValue = JsonConvert.SerializeObject(jsonAPI);
            return strValue;
        }

        public static string BuildSendMessagePrivate(Int64 userId, string Message, bool AutoEscape)
        {
            Params jsonParams = new Params();
            jsonParams.user_id = userId;
            jsonParams.message = Message;
            jsonParams.auto_escape = AutoEscape;
            Message jsonAPI = new Message();
            jsonAPI.action = "send_private_msg";
            jsonAPI.joParams = jsonParams;
            string strValue = JsonConvert.SerializeObject(jsonAPI);
            return strValue;
        }
    }
}
