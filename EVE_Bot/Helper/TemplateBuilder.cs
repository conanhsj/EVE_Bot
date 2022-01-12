using EVE_Bot.JsonObject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_Bot.Helper
{
    public static class TemplateBuilder
    {
        public static string BuildSendMessage(Int64 GroupId, string Message, bool AutoEscape)
        {
            JOParams jsonParams = new JOParams();
            jsonParams.group_id = GroupId;
            jsonParams.message = Message;
            jsonParams.auto_escape = AutoEscape;
            JOAPI jsonAPI = new JOAPI();
            jsonAPI.action = "send_group_msg";
            jsonAPI.joParams = jsonParams;
            string strValue = JsonConvert.SerializeObject(jsonAPI);
            return strValue;
        }

        public static string ActionGetUserList(Int64 GroupId, string Message, bool AutoEscape)
        {
            JOParams jsonParams = new JOParams();
            jsonParams.group_id = GroupId;
            jsonParams.message = Message;
            jsonParams.auto_escape = AutoEscape;
            JOAPI jsonAPI = new JOAPI();
            jsonAPI.action = "get_group_member_list";
            jsonAPI.joParams = jsonParams;
            string strValue = JsonConvert.SerializeObject(jsonAPI);
            return strValue;
        }

        public static string BuildSendMessagePrivate(Int64 userId, string Message, bool AutoEscape)
        {
            JOParams jsonParams = new JOParams();
            jsonParams.user_id = userId;
            jsonParams.message = Message;
            jsonParams.auto_escape = AutoEscape;
            JOAPI jsonAPI = new JOAPI();
            jsonAPI.action = "send_private_msg";
            jsonAPI.joParams = jsonParams;
            string strValue = JsonConvert.SerializeObject(jsonAPI);
            return strValue;
        }
    }
}
