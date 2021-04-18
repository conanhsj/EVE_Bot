using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using EVE_Bot.JsonEVE;
using Newtonsoft.Json;

namespace EVE_Bot.EVEAPIs
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

        public static Dictionary<string, Price> SearchPriceRegion(string item)
        {
            Dictionary<string, Price> dicResult = new Dictionary<string, Price>();
            
            //请求
            string strReqPath = string.Format("https://www.ceve-market.org/api/market/region/10000002/type/{0}.json", item);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strJson = sr.ReadToEnd();
                Price result = JsonConvert.DeserializeObject<Price>(strJson);

                dicResult.Add("伏尔戈", result);

            }

            strReqPath = string.Format("https://www.ceve-market.org/api/market/region/10000064/type/{0}.json", item);
            request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strJson = sr.ReadToEnd();
                Price result = JsonConvert.DeserializeObject<Price>(strJson);

                dicResult.Add("精华之域", result);

            }

            strReqPath = string.Format("https://www.ceve-market.org/api/market/region/10000042/type/{0}.json", item);
            request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strJson = sr.ReadToEnd();
                Price result = JsonConvert.DeserializeObject<Price>(strJson);

                dicResult.Add("美特伯里斯", result);

            }

            strReqPath = string.Format("https://www.ceve-market.org/api/market/region/10000043/type/{0}.json", item);
            request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strJson = sr.ReadToEnd();
                Price result = JsonConvert.DeserializeObject<Price>(strJson);

                dicResult.Add("多美", result);

            }

            //羊头狗肉
            //strReqPath = string.Format("https://www.ceve-market.org/api/market/region/10000049/type/{0}.json", item);
            strReqPath = string.Format("https://www.ceve-market.org/api/market/region/10000050/type/{0}.json", item);
            request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strJson = sr.ReadToEnd();
                Price result = JsonConvert.DeserializeObject<Price>(strJson);

                dicResult.Add("卡尼迪", result);

            }
            return dicResult;
        }

        public static Wormhole ReadWikiWormhole(string HoleID)
        {
            Wormhole wh = new Wormhole();
            //请求
            string strReqPath = string.Format("http://eve.huijiwiki.com/wiki/{0}", HoleID);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                //JsonTextReader jsonReader = new JsonTextReader(sr);
                string strJson = sr.ReadToEnd();
                strJson = WebUtility.HtmlDecode(strJson);
                //strJson = strJson.Trim('[', ']');
                //XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(strJson);
                HtmlAgilityPack.HtmlDocument xmlDoc = new HtmlAgilityPack.HtmlDocument();

                xmlDoc.LoadHtml(strJson);
                List<HtmlAgilityPack.HtmlNode> lstNode = xmlDoc.DocumentNode.SelectNodes("//div//table").ToList();
                List<HtmlAgilityPack.HtmlNode> lsttd = lstNode[0].SelectNodes("//td").ToList();

                wh.Code = lstNode[0].SelectSingleNode("//th").GetDirectInnerText();
                wh.Come = lsttd[0].InnerText.Trim('\n');
                wh.To = lsttd[1].InnerText.Trim('\n');
                wh.Keep = lsttd[2].InnerText.Trim('\n');
                wh.Pass = lsttd[3].InnerText.Trim('\n');
                wh.Max = lsttd[4].InnerText.Trim('\n');
            }

            return wh;
        }

    }
}
