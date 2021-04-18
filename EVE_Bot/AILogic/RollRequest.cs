using EVE_Bot.Interface;
using EVE_Bot.JsonGame;
using EVE_Bot.JsonObject;
using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVE_Bot.Helper;
using Newtonsoft.Json;

namespace EVE_Bot.AILogic
{
    [Export("RollRequest", typeof(IMessageRequest))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RollRequest : IMessageRequest
    {

        Random rnd = new Random();
        string IMessageRequest.DealGroupRequest(JORecvGroupMsg jsonGrpMsg)
        {
            string strValue = string.Empty;
            string strMessage = jsonGrpMsg.message;

            if (strMessage.StartsWith("Roll"))
            {
                strMessage = strMessage.Substring(4).Trim();
                string[] Args = strMessage.Split(' ');
                if (Args.Length > 0)
                {
                    string strRange = Args[0];
                    string strReason = strMessage.Substring(Args[0].Length).Trim();
                    if (strRange.Contains("D"))
                    {
                        string[] strDice = strRange.Split('D');

                        int nDice = 0;
                        int nRange = 0;

                        if (int.TryParse(strDice[0], out nDice) && int.TryParse(strDice[1], out nRange))
                        {
                            strValue += jsonGrpMsg.sender.card + "的骰子结果为：";
                            int nSum = 0;
                            for (int n = 0; n < nDice; n++)
                            {
                                int nResult = (rnd.Next(nRange) + 1);
                                strValue += nResult + "+";
                                nSum += nResult;
                            }
                            if (nSum > 0)
                            {
                                strValue = strValue.Trim('+') + " = " + nSum;
                            }
                        }
                        else
                        {
                            strValue += "请输入骰子的最大值，或按照 2D6的规则输入。而不是" + Args[0];
                        }
                    }
                    else
                    {
                        int nRange = 0;
                        if (int.TryParse(Args[0], out nRange))
                        {
                            strValue += jsonGrpMsg.sender.card + "的骰子结果为：" + (rnd.Next(nRange) + 1);
                        }
                        else
                        {
                            strValue += "请输入骰子的最大值，或按照 2D6的规则输入。而不是" + Args[0];
                        }
                    }
                    if (Args.Length > 1)
                    {
                        strValue += "\n" + strReason;
                    }
                }
                else
                {
                    strValue += jsonGrpMsg.sender.card + "的骰子结果为：" + (rnd.Next(6) + 1);
                }
            }

            return strValue;
        }

        string IMessageRequest.DealPrivateRequest(JORecvGroupMsg jsonGrpMsg)
        {
            throw new NotImplementedException();
        }

    }
}
