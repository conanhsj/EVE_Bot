using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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


                    if (Cached.sell.min == 0 || 
                       (result.sell.min != 0 && Cached.sell.min > result.sell.min))
                    {
                        Cached.sell = result.sell;
                        Cached.sell.place = "皮";
                    }
                    if (Cached.buy.max == 0 || Cached.buy.max < result.buy.max)
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

        public static List<long> SearchNameToId(string strName)
        {
            SearchResult SearchIDResult = new SearchResult();

            //请求
            string strReqPath = string.Format("https://esi.evepc.163.com/latest/universe/ids/?datasource=serenity&language=zh");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "POST";

            request.KeepAlive = false;
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            byte[] postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new List<string>() { strName }));   //使用utf-8格式组装post参数
            Stream reqStream = request.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);

            using (WebResponse response = request.GetResponse())
            {

                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strJson = sr.ReadToEnd();
                SearchIDResult = JsonConvert.DeserializeObject<SearchResult>(strJson);
            }

            return SearchIDResult.characters.Select(Value => Value.id).ToList();
        }

        public static List<long> SearchCharacter(string strUserName)
        {
            SearchResult SearchIDResult = new SearchResult();
            Random random = new Random();

            //请求
            string strReqPath = string.Format("https://esi.evepc.163.com/latest/search/?categories=character&datasource=serenity&language=zh&search={0}&strict=false", strUserName);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReqPath);
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                //JsonTextReader jsonReader = new JsonTextReader(sr);
                string strJson = sr.ReadToEnd();
                SearchIDResult = JsonConvert.DeserializeObject<SearchResult>(strJson);
            }

            return SearchIDResult.character;
        }
        public static List<Character> SearchCharacterData(List<long> lstSearchValue)
        {
            HttpWebRequest request = null;
            List<Character> lstCharacter = new List<Character>();

            if (lstSearchValue.Count > 0)
            {
                foreach (long charaID in lstSearchValue)
                {
                    //请求
                    string strCharacterRequest = string.Format("https://esi.evepc.163.com/latest/characters/{0}/?datasource=serenity", charaID);
                    request = (HttpWebRequest)WebRequest.Create(strCharacterRequest);
                    request.Method = "GET";

                    using (WebResponse response = request.GetResponse())
                    {

                        Stream stream = response.GetResponseStream();
                        StreamReader sr = new StreamReader(stream);
                        string strJson = sr.ReadToEnd();
                        Character chara = JsonConvert.DeserializeObject<Character>(strJson);
                        chara.character_id = charaID;
                        lstCharacter.Add(chara);
                    }
                }

                foreach (Character IDResult in lstCharacter)
                {
                    //请求
                    string strCharacterRequest = string.Format("https://esi.evepc.163.com/latest/corporations/{0}/?datasource=serenity", IDResult.corporation_id);
                    request = (HttpWebRequest)WebRequest.Create(strCharacterRequest);
                    request.Method = "GET";

                    using (WebResponse response = request.GetResponse())
                    {

                        Stream stream = response.GetResponseStream();
                        StreamReader sr = new StreamReader(stream);
                        string strJson = sr.ReadToEnd();
                        Corporation corp = JsonConvert.DeserializeObject<Corporation>(strJson);
                        IDResult.corporation_name = corp.name + "[" + corp.ticker + "]";
                    }
                }

                foreach (Character IDResult in lstCharacter)
                {
                    if (IDResult.alliance_id == 0)
                    {
                        continue;
                    }
                    //请求
                    string strCharacterRequest = string.Format("https://esi.evepc.163.com/latest/alliances/{0}/?datasource=serenity", IDResult.alliance_id);
                    request = (HttpWebRequest)WebRequest.Create(strCharacterRequest);
                    request.Method = "GET";

                    using (WebResponse response = request.GetResponse())
                    {

                        Stream stream = response.GetResponseStream();
                        StreamReader sr = new StreamReader(stream);
                        string strJson = sr.ReadToEnd();
                        Alliances alli = JsonConvert.DeserializeObject<Alliances>(strJson);
                        IDResult.alliance_name = alli.name + "[" + alli.ticker + "]";
                    }
                }

            }
            return lstCharacter;
        }


    }
}
