using EVE_Bot.EVEAPIs;
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

        public static string DealSearchRequest(ClientWebSocket ws, JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;//"[CQ:at,qq=" + jsonGrpMsg.user_id + "]";
            string strRequest = jsonGrpMsg.message.Trim('!', '！', ' ');
            if (strRequest.StartsWith("查询蓝图"))
            {
                strMessage = SearchBluePrint(strMessage, strRequest);
            }
            else if (strRequest.StartsWith("查询价格"))
            {
                strMessage = SearchPrice(strMessage, strRequest);
            }
            else if (strRequest.StartsWith("查询广域"))
            {
                strMessage = SearchRange(strMessage, strRequest);
            }
            else
            {
                strMessage += "这边没有您需要的服务，请回吧";
            }
            return strMessage;
        }

        private static string SearchRange(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询价格"字眼
            strKeyWord = strRequest.Substring(4).Trim();
            if (strKeyWord.Length < 3)
            {
                throw new Exception("太短了，下次再长点。");
            }
            List<Item> lstSearch = lstItem.FindAll(Item => Item.Name == strKeyWord);

            //二次筛选
            if (lstSearch.Count != 1)
            {
                strMessage += "本功能仅支持单商品查询，请通过查询价格或蓝图确认物品名称";
                return strMessage;
            }
            //提取ID列表
            List<string> lstTypeID = lstSearch.Select(obj => { return obj.TypeID; }).ToList();

            if (lstTypeID.Count == 0)
            {
                strMessage += "没找到物品ID";
            }
            else if (lstTypeID.Count == 1)
            {
                //查询价格
                Dictionary<string, Price> dicResult = CEVEMarket.SearchPriceRegion(lstTypeID[0]);



                strMessage += strKeyWord + "在以下星域的价格为：\n";
                foreach (string strKey in dicResult.Keys)
                {
                    string strSell = dicResult[strKey].sell.min == 0 ? "无货" : Commons.FormatISK(dicResult[strKey].sell.min.ToString("0.00")).TrimEnd('0');
                    string strBuy = dicResult[strKey].buy.max == 0 ? "无货" : Commons.FormatISK(dicResult[strKey].buy.max.ToString("0.00")).TrimEnd('0');
                    strMessage += strKey.PadRight(6, '　') + "出售：" + strSell.TrimEnd('.').PadLeft(7, ' ') + " 收购：" + strBuy.TrimEnd('.').PadLeft(7, ' ') + "\n";
                }
                if (string.IsNullOrEmpty(strMessage))
                {
                    strMessage += "冇得买啦";
                }
                strMessage = strMessage.TrimEnd('\n');
            }

            return strMessage;
        }

        private static string SearchPrice(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询价格"字眼
            strKeyWord = strRequest.Substring(4).Trim();
            if (strKeyWord.Length < 3)
            {
                throw new Exception("太短了，下次再长点。");
            }

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
            else if (lstTypeID.Count < 40)
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
                    strMessage += strKey.PadRight(nMaxLength + 2, '　') + "\n出售：" + strSell.PadLeft(7, ' ') + " 收购：" + strBuy.PadLeft(7, ' ') + "\n";
                }
                if (string.IsNullOrEmpty(strMessage))
                {
                    strMessage += "冇得买啦";
                }
                strMessage = strMessage.TrimEnd('\n');
            }
            else
            {
                strMessage += "你要找的是不是里的某个？\n";
                foreach (Item obj in lstSearch)
                {

                    strMessage += obj.Name + "\n";
                }
                strMessage = strMessage.TrimEnd('\n');
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
                    string strRate = strRequest.Substring(strRequest.IndexOf(" ", nIndex - 3), nIndex - strRequest.IndexOf(" ", nIndex - 3)).Trim();
                    dMaterRate = Commons.ReadDouble(strRate) / 100;
                }
                catch (Exception ex)
                {
                    throw new Exception("这材料效率写错了吧！？");
                }
                strKeyWord = strRequest.Substring(0, strRequest.IndexOf(" ", strRequest.IndexOf(" ", nIndex - 3))).Trim();
            }
            else
            {
                strKeyWord = strRequest;
            }
            if (strKeyWord.Length < 3)
            {
                throw new Exception("太短了，下次再长点。");
            }
            //查找和计算
            BluePrint bluePrint = lstBluePrint.Find(Item => Item.BPName == strKeyWord || Item.ProductName == strKeyWord);

            if (bluePrint != null)
            {
                if (bluePrint.BPName.Contains("反应配方") && dMaterRate != 0)
                {
                    throw new Exception("反应配方没有材料效率哒喵！");
                }

                //价格查询
                List<string> lstSearch = new List<string>();
                lstSearch.Add(bluePrint.ProductID.ToString());
                foreach (BluePrintMtls Mtls in bluePrint.Materials)
                {
                    lstSearch.Add(Mtls.TypeID.ToString());
                }
                Dictionary<string, Price> dicResult = CEVEMarket.SearchPriceJson(lstSearch);

                strMessage += "生产 " + bluePrint.ProductQty + " 个 " + bluePrint.ProductName + " 需要以下材料：\n";

                double dBuy = 0;
                double dSell = 0;
                foreach (BluePrintMtls Mtls in bluePrint.Materials)
                {

                    double dQty = Mtls.Qty == 1 ? 1 : (Mtls.Qty * (1 - dMaterRate));

                    //收购价格
                    double dMtlsBuy = dicResult[Mtls.TypeID.ToString()].buy.max * Math.Ceiling(dQty);
                    string strBuyISK = string.Empty;
                    if (dMtlsBuy == 0)
                    {
                        strBuyISK = "这个没价格";
                    }
                    else
                    {
                        dBuy += dMtlsBuy;
                        strBuyISK = Commons.FormatISK(dMtlsBuy.ToString("0.00")) + "[" + dicResult[Mtls.TypeID.ToString()].buy.place + "]";
                    }
                    //卖单价格
                    double dMtlsSell = dicResult[Mtls.TypeID.ToString()].sell.min * Math.Ceiling(dQty);
                    string strSellISK = string.Empty;
                    if (dMtlsSell == 0)
                    {
                        strSellISK = "这个没人卖";
                    }
                    else
                    {
                        dSell += dMtlsSell;
                        strSellISK = Commons.FormatISK(dMtlsSell.ToString("0.00")) + "[" + dicResult[Mtls.TypeID.ToString()].sell.place + "]";
                    }
                    strMessage += "  " + Mtls.Name + "： " + dQty.ToString("0.0") + "个,买： " + strSellISK + ",收： " + strBuyISK + "\n";
                }
                strMessage += "=====================总计=====================\n";
                string strItemSell = string.Empty;
                if (dicResult[bluePrint.ProductID.ToString()].sell.min == 0)
                {
                    strItemSell = "这个买不到";
                }
                else
                {
                    strItemSell = Commons.FormatISK(dicResult[bluePrint.ProductID.ToString()].sell.min.ToString("0.00")) + "[" + dicResult[bluePrint.ProductID.ToString()].sell.place + "]";
                }

                //产出复数
                string strComment = string.Empty;
                if (bluePrint.ProductQty > 1)
                {
                    dSell = dSell / bluePrint.ProductQty;
                    dBuy = dBuy / bluePrint.ProductQty;
                    strComment = "(平均每个)";
                }

                strMessage += bluePrint.ProductName + "直接买：" + strItemSell + " 材料采购：" + Commons.FormatISK(dSell.ToString("0.00")) + " 材料收购：" + Commons.FormatISK(dBuy.ToString("0.00")) + "\n" + strComment;
                strMessage = strMessage.TrimEnd('\n');
            }
            else
            {
                List<BluePrint> lstResult = lstBluePrint.FindAll(Item => Item.BPName.Contains(strKeyWord) || Item.ProductName.Contains(strKeyWord));
                if (lstResult.Count > 100)
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
