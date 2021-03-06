using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using EVE_Bot.JsonEVE;
using Newtonsoft.Json;

namespace EVE_Bot.Classes
{
    public static class CEVEMarket
    {
        public static List<Price> lstPriceCache = new List<Price>();

        public static Dictionary<string, Price> SearchPriceJson(List<string> lstItem)
        {
            Dictionary<string, Price> dicResult = new Dictionary<string, Price>();

            foreach (string Item in lstItem)
            {
                Price Cached = lstPriceCache.Find(X => X.TypeId == Item);
                if (Cached != null)
                {
                    //检查缓存
                    if ((DateTime.Now - Cached.CachedTime).TotalMinutes < 20)
                    {
                        dicResult.Add(Item, Cached);
                        continue;
                    }
                    else
                    {
                        lstPriceCache.Remove(Cached);
                        Cached = null;
                    }
        
                }
                //请求
                string strReqPath = string.Format("https://www.ceve-market.org/api/market/region/10000002/system/30000142/type/{0}.json", Item);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReqPath);
                request.Method = "GET";

                using (WebResponse response = request.GetResponse())
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    string strJson = sr.ReadToEnd();
                    Price result = JsonConvert.DeserializeObject<Price>(strJson);
            
                    Cached = result;
                   
                }

                strReqPath = string.Format("https://www.ceve-market.org/api/market/region/10000002/system/30000144/type/{0}.json", Item);
                request = (HttpWebRequest)WebRequest.Create(strReqPath);
                request.Method = "GET";

                using (WebResponse response = request.GetResponse())
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    string strJson = sr.ReadToEnd();
                    Price result = JsonConvert.DeserializeObject<Price>(strJson);


                    if (result.sell.min != 0 && Cached.sell.min > result.sell.min)
                    {
                        Cached.sell = result.sell;
                        Cached.sell.place = "皮";
                    }
                    if (Cached.buy.min == 0 || Cached.buy.max < result.buy.max)
                    {
                        Cached.buy = result.buy;
                        Cached.buy.place = "皮";
                    }
                }

                Cached.TypeId = Item;
                Cached.CachedTime = DateTime.Now;
                dicResult.Add(Item, Cached);
                lstPriceCache.Add(Cached);
            }
            return dicResult;
        }

    }
}
