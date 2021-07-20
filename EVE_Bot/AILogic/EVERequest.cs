using Bot.ExtendInterface;
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
        private static List<BluePrint> lstBluePrint = JsonConvert.DeserializeObject<List<BluePrint>>(FilesHelper.ReadJsonFile(@"EVE\BluePrint"));
        private static List<Item> lstItem = JsonConvert.DeserializeObject<List<Item>>(FilesHelper.ReadJsonFile(@"EVE\ItemID"));
        private static List<Recycle> lstRecycle = JsonConvert.DeserializeObject<List<Recycle>>(FilesHelper.ReadJsonFile(@"EVE\Materials"));
        private static List<WormholeSystem> lstWormholeSystem = JsonConvert.DeserializeObject<List<WormholeSystem>>(FilesHelper.ReadJsonFile(@"EVE\Wormhole"));
        private static List<Wormhole> lstWormhole = JsonConvert.DeserializeObject<List<Wormhole>>(FilesHelper.ReadJsonFile(@"EVE\Hole"));
        private static List<Solar> lstSolar = JsonConvert.DeserializeObject<List<Solar>>(FilesHelper.ReadJsonFile(@"EVE\UniverseSystem"));

        public static string DealSearchRequest(ClientWebSocket ws, JORecvGroupMsg jsonGrpMsg)
        {
            string strMessage = string.Empty;//"[CQ:at,qq=" + jsonGrpMsg.user_id + "]";
            string strRequest = jsonGrpMsg.message.Trim('!', '！', ' ');
            if (strRequest.StartsWith("蓝图"))
            {
                strMessage = SearchBluePrint(strMessage, strRequest);
            }
            else if (strRequest.StartsWith("价格"))
            {
                strMessage = SearchPrice(strMessage, strRequest);
            }
            else if (strRequest.StartsWith("广域"))
            {
                strMessage = SearchRange(strMessage, strRequest);
            }
            else if (strRequest.StartsWith("材料"))
            {
                strMessage = SearchMaterial(strMessage, strRequest);
            }
            else if (strRequest.StartsWith("提炼"))
            {
                strMessage = SearchRecycle(strMessage, strRequest);
            }
            else if (strRequest.StartsWith("虫洞"))
            {
                strMessage = SearchWormhole(strMessage, strRequest);
            }
            else if (strRequest.StartsWith("洞口"))
            {
                strMessage = SearchHole(strMessage, strRequest);
            }
            else if (strRequest.StartsWith("角色"))
            {
                strMessage = SearchCharacter(strMessage, strRequest);
            }
            else if (strRequest.StartsWith("星系"))
            {
                strMessage = SearchSolar(strMessage, strRequest);
            }
            else if (strRequest.StartsWith("帮助"))
            {
                strMessage += "可用命令：\n";
                strMessage += "!蓝图 查询详细制造消耗\n";
                strMessage += "!价格 查询具体名称或吉他皮米价格\n";
                strMessage += "!广域 查询具体物品的多星域价格\n";
                strMessage += "!材料 查询物品的使用用途\n";
                strMessage += "!虫洞 查询对应编号的虫洞信息\n";
                strMessage += "!洞口 查询对应编号的洞口信息\n";
                strMessage += "!角色 查询人物的公开信息\n";
                strMessage += "!星系 查询星系相关的各种内容（制作中）\n";
                strMessage += "!提炼 查询化矿或碎铁产物";
            }
            else
            {
                strMessage += "[CQ:at,qq=" + jsonGrpMsg.sender.user_id + "]？有问题查帮助啊";
            }
            return strMessage;
        }

        private static string SearchSolar(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询提炼"字眼
            strRequest = strRequest.Substring(2).Trim();


            List<Solar> lstResult = lstSolar.FindAll(solar => solar.system_name.Contains(strRequest));

            if (lstResult.Count > 0)
            {
                strMessage += "找到" + lstResult.Count + "个结果\n";
                foreach (Solar Solar in lstResult)
                {
                    strMessage += Solar.system_name + "(" + Solar.system_id + ") << " + Solar.constellation_name + "(" + Solar.constellation_id + ") << " + Solar.region_name + "(" + Solar.region_id + ")\n";
                }
                strMessage.Trim('\n');
            }
            else
            {
                strMessage += "没找到啊";
            }
            return strMessage;
        }

        private static string SearchRecycle(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询提炼"字眼
            strRequest = strRequest.Substring(2).Trim();
            int nIndex = strRequest.IndexOf("%");
            double dMaterRate = 1;
            if (nIndex > 0)
            {
                try
                {
                    string strRate = strRequest.Substring(strRequest.IndexOf(" ", nIndex - 5), nIndex - strRequest.IndexOf(" ", nIndex - 5)).Trim();
                    dMaterRate = Commons.ReadDouble(strRate) / 100;
                }
                catch (Exception ex)
                {
                    throw new Exception("这材料效率写错了吧！？");
                }
                strKeyWord = strRequest.Substring(0, strRequest.IndexOf(" ", strRequest.IndexOf(" ", nIndex - 5))).Trim();
            }
            else
            {
                strKeyWord = strRequest;
            }

            //查找和计算
            List<Recycle> recItem = lstRecycle.FindAll(Item => Item.Name == strKeyWord);

            //二次筛选
            if (recItem.Count != 1)
            {
                strMessage += "本功能仅支持单商品查询，请通过查询价格或蓝图确认物品名称";
                return strMessage;
            }


            //价格查询
            List<string> lstSearch = new List<string>();
            lstSearch.Add(recItem[0].TypeID.ToString());
            foreach (RecycleMtls Mtls in recItem[0].Materials)
            {
                lstSearch.Add(Mtls.TypeID.ToString());
            }

            Dictionary<string, Price> dicResult = CEVEMarket.SearchPriceJson(lstSearch);

            string strMaterRate = dMaterRate.ToString("00.0%");

            strMessage += "在 " + strMaterRate + " 转化率下提炼 " + recItem[0].Name + " 可得到以下材料：\n";

            double dBuy = 0;
            double dSell = 0;
            foreach (RecycleMtls Mtls in recItem[0].Materials)
            {

                double dQty = Math.Floor(Mtls.Quantity * dMaterRate);

                if (dQty == 0)
                {
                    strMessage += "  " + Mtls.Name + "被损耗吃掉啦";
                    continue;
                }

                //收购价格
                double dMtlsBuy = dicResult[Mtls.TypeID.ToString()].buy.max * Math.Ceiling(dQty);
                string strBuyISK = string.Empty;
                if (dMtlsBuy == 0)
                {
                    strBuyISK = "这个没人收";
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
                strMessage += "  " + Mtls.Name + "：" + dQty.ToString("0") + "个,挂单出约： " + strSellISK + ",卖收单约： " + strBuyISK + "\n";
            }
            strMessage += "=====================总计=====================\n";
            string strItemSell = string.Empty;
            if (dicResult[recItem[0].TypeID.ToString()].sell.min == 0)
            {
                strItemSell = "这个没人卖";
            }
            else
            {
                strItemSell = Commons.FormatISK(dicResult[recItem[0].TypeID.ToString()].sell.min.ToString("0.00")) + "[" + dicResult[recItem[0].TypeID.ToString()].sell.place + "]";
            }

            string strItemBuy = string.Empty;
            if (dicResult[recItem[0].TypeID.ToString()].buy.max == 0)
            {
                strItemBuy = "这个没人收";
            }
            else
            {
                strItemBuy = Commons.FormatISK(dicResult[recItem[0].TypeID.ToString()].buy.max.ToString("0.00")) + "[" + dicResult[recItem[0].TypeID.ToString()].buy.place + "]";
            }
            strMessage += recItem[0].Name + "卖单：" + strItemSell + " 收单：" + strItemBuy + " \n";
            strMessage += "  提炼后挂单出售：" + Commons.FormatISK(dSell.ToString("0.00")) + " 提炼后直接出售：" + Commons.FormatISK(dBuy.ToString("0.00")) + "\n";
            return strMessage;
        }

        private static string SearchMaterial(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询蓝图"字眼
            strRequest = strRequest.Substring(2).Trim();
            List<Item> lstSearch = lstItem.FindAll(Item => Item.Name == strRequest);
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
                //查找和计算
                List<BluePrint> bluePrint = lstBluePrint.FindAll(Item => Item.Materials.FindAll(Mater => Mater.TypeID.ToString() == lstTypeID[0]).Count > 0);
                strMessage += "共查询到" + bluePrint.Count + "种消耗方式\n";
                if (bluePrint.Count > 30)
                {
                    strMessage += "由于数量太多，所以仅显示前30种\n";
                    for (int n = 0; n < 30; n++)
                    {
                        strMessage += bluePrint[n].ProductName + "\n";
                    }
                }
                else
                {
                    foreach (BluePrint bp in bluePrint)
                    {
                        strMessage += bp.ProductName + "\n";
                    }
                }
            }

            return strMessage;
        }

        private static string SearchRange(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询价格"字眼
            strKeyWord = strRequest.Substring(2).Trim();
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
            strKeyWord = strRequest.Substring(2).Trim();

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
            strRequest = strRequest.Substring(2).Trim();
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
            if (strKeyWord.Length < 2)
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

                int nMaxLength = bluePrint.Materials.Max(obj => { return obj.Name.Length; });

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
                    strMessage += "  " + Mtls.Name.PadRight(nMaxLength, '　') + "： " + dQty.ToString("0.0") + "个,买： " + strSellISK + ",收： " + strBuyISK + "\n";
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

        private static string SearchWormhole(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询蓝图"字眼
            strRequest = strRequest.Substring(2).Trim();

            List<WormholeSystem> lstResult = lstWormholeSystem.FindAll(wh => wh.Name.Contains(strRequest.ToUpper()));
            strMessage += "找到" + lstResult.Count + "个结果";
            if (lstResult.Count > 0 && lstResult.Count < 20)
            {
                strMessage += "\n";
                foreach (WormholeSystem WH in lstResult)
                {
                    strMessage += WH.Name + " 等级：" + WH.Class + " 天象：" + (WH.Effects == string.Empty ? "None" : WH.Effects) + " 永联：" + WH.Statics + "\n";
                }
                strMessage.Trim('\n');
            }
            return strMessage;
        }

        private static string SearchHole(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询蓝图"字眼
            strRequest = strRequest.Substring(2).Trim().ToUpper();
            if (lstWormhole == null)
            {
                lstWormhole = new List<Wormhole>();
            }

            List<Wormhole> lstResult = lstWormhole.FindAll(wh => wh.Code == strRequest);

            if (lstResult.Count > 0)
            {
                foreach (Wormhole WH in lstResult)
                {
                    strMessage += "洞口编号：" + WH.Code + "\n";
                    strMessage += "出现于：" + WH.Come + "\n";
                    strMessage += "联通至：" + WH.To + "\n";
                    strMessage += "持续时间：" + WH.Keep + "\n";
                    strMessage += "单次通过质量：" + WH.Pass + "\n";
                    strMessage += "总共通过质量：" + WH.Max + "\n";

                    strMessage = FixHoleChange(strMessage, WH);
                }
                strMessage.Trim('\n');
            }
            else
            {
                if (strRequest.Length != 4)
                {
                    strMessage += "俺寻思你这编号写错了呐！";
                    return strMessage;
                }
                try
                {
                    Wormhole WH = CEVEMarket.ReadWikiWormhole(strRequest);
                    strMessage += "洞口编号：" + WH.Code + "\n";
                    strMessage += "出现于：" + WH.Come + "\n";
                    strMessage += "联通至：" + WH.To + "\n";
                    strMessage += "持续时间：" + WH.Keep + "\n";
                    strMessage += "单次通过质量：" + WH.Pass + "\n";
                    strMessage += "总共通过质量：" + WH.Max;
                    strMessage = FixHoleChange(strMessage, WH);
                    lstWormhole.Add(WH);
                    FilesHelper.OutputJsonFile(@"EVE\Hole", JsonConvert.SerializeObject(lstWormhole, Formatting.Indented));
                }
                catch
                {
                    return "Wiki娘说她找不到！";
                }

            }
            return strMessage;
        }

        private static string SearchCharacter(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询角色"字眼
            strRequest = strRequest.Substring(2).Trim();

            if (strRequest.Length < 3)
            {
                strRequest = "*" + strRequest + "*";
                strMessage += "查询内容过短，以下显示查询\"" + strRequest + "\"的结果";
            }

            //查找和计算
            List<long> lstResult = CEVEMarket.SearchCharacter(strRequest);

            strMessage += "找到" + lstResult.Count + "个结果\n";
            if (lstResult.Count > 20)
            {
                strMessage += "结果数量过多，改为精确查找\n";
                lstResult = CEVEMarket.SearchNameToId(strRequest);
            }

            if (lstResult.Count > 0)
            {
                List<Character> lstCharacter = CEVEMarket.SearchCharacterData(lstResult);
                foreach (Character chara in lstCharacter)
                {
                    strMessage += "角色名：" + chara.name + "\n";
                    strMessage += "角色ID：" + chara.character_id + "\n";
                    strMessage += "军团名称：" + chara.corporation_name + "\n";
                    strMessage += "军团ID：" + chara.corporation_id + "\n";
                    if (chara.alliance_id != 0)
                    {
                        strMessage += "联盟名称：" + chara.alliance_name + "\n";
                        strMessage += "联盟ID：" + chara.alliance_id + "\n";
                    }
                    strMessage += "出生日期：" + chara.birthday.ToString("yyyy-MM-dd HH:mm") + "\n";
                    int nDays = (int)(DateTime.Now - chara.birthday).TotalDays;
                    strMessage += "年龄：" + nDays + "天\n";
                    strMessage += "头像：[CQ:image,file=" + string.Format("https://image.evepc.163.com/Character/{0}_256.jpg", chara.character_id) + ",cache=1]\n";
                    strMessage += "=====================================\n";
                }
                strMessage = strMessage.Trim('\n', '=');
            }
            else
            {
                strMessage += "没找到";
            }
            return strMessage;
        }

        private static string FixHoleChange(string strMessage, Wormhole WH)
        {
            bool bHasChanged = false;
            if (WH.Pass.StartsWith("20,000,000kg"))
            {
                WH.Pass = WH.Pass.Replace("20,000,000kg", "62,000,000kg");
                strMessage += "\n这个在2021年YC123.3的更新中改成62,000,000kg啦";
                bHasChanged = true;
            }
            if (WH.Pass.StartsWith("300,000,000kg"))
            {
                WH.Pass = WH.Pass.Replace("300,000,000kg", "375,000,000kg");
                strMessage += "\n这个在2021年YC123.3的更新中改成375,000,000kg啦";
                bHasChanged = true;
            }
            if (WH.Pass.StartsWith("1,350,000,000kg"))
            {
                WH.Pass = WH.Pass.Replace("1,350,000,000kg", "2,000,000,000kg");
                strMessage += "\n这个在2021年YC123.3的更新中改成2,000,000,000kg啦";
                bHasChanged = true;
            }
            if (WH.Pass.StartsWith("1,800,000,000kg"))
            {
                WH.Pass = WH.Pass.Replace("1,800,000,000kg", "2,000,000,000kg");
                strMessage += "\n这个在2021年YC123.3的更新中改成2,000,000,000kg啦";
                bHasChanged = true;
            }
            if (bHasChanged)
            {
                FilesHelper.OutputJsonFile("Hole", JsonConvert.SerializeObject(lstWormhole, Formatting.Indented));
            }
            return strMessage;
        }


        private static string SearchUniverse(string strMessage, string strRequest)
        {
            string strKeyWord = string.Empty;
            //去掉最前边的"查询XX"字眼
            strKeyWord = strRequest.Substring(2).Trim();

            List<Sovereignty> joSov = CEVESwaggerInterface.SovereigntyMap();
            FilesHelper.OutputJsonFile("Sovereignty\\Map", JsonConvert.SerializeObject(joSov, Formatting.Indented));

            return strMessage;
        }

    }
}
