using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.ExtendInterface
{
    public interface IMessageRequest
    {
        string DealGroupRequest(JORecvGroupMsg jsonGrpMsg);

        string DealPrivateRequest(JORecvGroupMsg jsonGrpMsg);
    }
}
