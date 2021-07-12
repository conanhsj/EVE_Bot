using Bot.ExtendInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.EveRequest
{
    [Export("EveRequest", typeof(IMessageRequest))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SearchRequest : IMessageRequest
    {

        public SearchRequest()
        {

        }

        string IMessageRequest.DealGroupRequest(JORecvGroupMsg jsonGrpMsg)
        {
            string strValue = string.Empty;
            string strMessage = jsonGrpMsg.message.ToUpper();
            string[] Args = strMessage.Split(' ');
            if (Args.Length > 1)
            {

            }
            else
            {

            }

            return strValue;
        }

        string IMessageRequest.DealPrivateRequest(JORecvGroupMsg jsonGrpMsg)
        {
            string strValue = string.Empty;
            string strMessage = jsonGrpMsg.message.ToUpper();
            string[] Args = strMessage.Split(' ');

            return strValue;
        }
    }
}
