using EVE_Bot.Classes;
using EVE_Bot.Helper;
using EVE_Bot.JsonEVE;
using EVE_Bot.JsonObject;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EVE_Bot.AILogic
{
    public static class EVERequest
    {
        private static List<BluePrint> lstBluePrint = JsonConvert.DeserializeObject<List<BluePrint>>(FilesHelper.ReadJsonFile("BluePrint"));
        private static List<Item> lstItem = JsonConvert.DeserializeObject<List<Item>>(FilesHelper.ReadJsonFile("ItemID"));

        private static CancellationToken cancelState = new CancellationToken();
        public static async Task<string> DealAtRequest(ClientWebSocket ws, JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;//"[CQ:at,qq=" + jsonGrpMsg.user_id + "]";

            Match match = Regex.Match(jsonGrpMsg.message, "(?:\\[*\\]).*");

            if (match.Success)
            {
                string strRequest = match.Value.Trim(']', ' ');
                if (strRequest.StartsWith("查询蓝图"))
                {
                    strMessage = SearchBluePrint(strMessage, strRequest);
                }
                else if (strRequest.StartsWith("查询价格"))
                {
                    strMessage = SearchPrice(strMessage, strRequest);
                }
                else
                {
                    strMessage += "哈？";
                }
            }
            else
            {
                strMessage += "听不懂";
            }

            return strMessage;

        }
        private static string SearchPrice(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询价格"字眼
            strKeyWord = strRequest.Substring(4).Trim();
            List<Item> lstSearch = lstItem.FindAll(Item => Item.Name.Contains(strKeyWord) && !Item.Name.Contains("蓝图"));

            //二次筛选
            if (lstSearch.Count > 20)
            {
                lstSearch = lstSearch.FindAll(Item => !Item.Name.Contains("涂装"));
            }
            //提取ID列表
            List<string> lstTypeID = lstSearch.Select(obj => { return obj.TypeID; }).ToList();



            if (lstTypeID.Count == 0)
            {
                strMessage += "没找到物品";
            }
            else if (lstTypeID.Count < 20)
            {
                //查询价格
                Dictionary<string, Price> dicResult = CEVEMarket.SearchPriceJson(lstTypeID);
                Dictionary<string, Price> dicNameResult = new Dictionary<string, Price>();
                foreach (string strKey in dicResult.Keys)
                {
                    if (dicResult[strKey].sell.min == 0 && dicResult[strKey].buy.max == 0)
                    {
                        continue;
                    }
                    //输出模板
                    Item target = lstSearch.Find(Item => Item.TypeID == strKey);
                    dicNameResult.Add(target.Name, dicResult[strKey]);
                }

                //寻找最长名字
                int nMaxLength = dicNameResult.Keys.Max(obj => { return obj.Length; });
                foreach (string strKey in dicNameResult.Keys)
                {
                    string strSell = Commons.FormatISK(dicNameResult[strKey].sell.min.ToString("0.00")) + "[" + dicNameResult[strKey].sell.place + "]";
                    string strBuy = Commons.FormatISK(dicNameResult[strKey].buy.max.ToString("0.00")) + "[" + dicNameResult[strKey].buy.place + "]";
                    strMessage += strKey.PadRight(nMaxLength + 2, '　') + "\t 出售：" + strSell.PadLeft(7, ' ') + "\t 收购：" + strBuy.PadLeft(7, ' ') + "\n";
                }
                if (string.IsNullOrEmpty(strMessage))
                {
                    strMessage += "冇得买啦";
                }
                strMessage = strMessage.TrimEnd('\n');
            }
            else
            {
                strMessage += "太多了，不想写";
            }
            return strMessage;
        }



        private static string SearchBluePrint(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询蓝图"字眼
            strRequest = strRequest.Substring(4).Trim();
            int nIndex = strRequest.IndexOf("%");
            double dMaterRate = 0;
            if (nIndex > 0)
            {
                try
                {
                    string strRate = strRequest.Substring(strRequest.IndexOf(" ", nIndex - 4), nIndex - strRequest.IndexOf(" ", nIndex - 4)).Trim();
                    dMaterRate = Commons.ReadDouble(strRate) / 100;
                }
                catch (Exception ex)
                {
                    throw new Exception("这材料效率写错了吧！？");
                }
                strKeyWord = strRequest.Substring(0, strRequest.IndexOf(" ")).Trim();
            }
            else
            {
                strKeyWord = strRequest;
            }

            //查找和计算
            BluePrint bluePrint = lstBluePrint.Find(Item => Item.BPName == strKeyWord || Item.ProductName == strKeyWord);

            if (bluePrint != null)
            {
                if (bluePrint.BPName.Contains("反应配方") && dMaterRate != 0)
                {
                    throw new Exception("反应配方没有材料效率哒喵！");
                }

                strMessage += "生产 " + bluePrint.ProductQty + " 个 " + bluePrint.ProductName + " 需要以下材料：\n";
                foreach (BluePrintMtls Mtls in bluePrint.Materials)
                {
                    string strQty = Mtls.Qty == 1 ? "1" : (Mtls.Qty * (1 - dMaterRate)).ToString("0.0");
                    strMessage += "  " + Mtls.Name + ":" + strQty + " 个\n";
                }
                strMessage = strMessage.TrimEnd('\n');
            }
            else
            {
                List<BluePrint> lstResult = lstBluePrint.FindAll(Item => Item.BPName.Contains(strKeyWord) || Item.ProductName.Contains(strKeyWord));
                if (lstResult.Count > 20)
                {
                    throw new Exception("实在太多了算不过来！");
                }
                strMessage += "你要找的是不是这些？\n";
                foreach (BluePrint bp in lstResult)
                {
                    strMessage += bp.ProductName + "\n";
                }
                strMessage += "请使用 查询蓝图 名称 材料效率% 来叫我";
            }
            return strMessage;
        }
    }
}
