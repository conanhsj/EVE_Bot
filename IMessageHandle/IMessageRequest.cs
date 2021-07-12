using EVE_Bot.JsonObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoneBot.IMessageHandle
{
    public interface IMessageRequest
    {
        string DealGroupRequest(JORecvGroupMsg jsonGrpMsg);

        string DealPrivateRequest(JORecvGroupMsg jsonGrpMsg);
    }
}
