using EVE_Bot.AILogic;
using EVE_Bot.Helper;
using EVE_Bot.Interface;
using EVE_Bot.JsonEVE;
using EVE_Bot.JsonObject;
using EVE_Bot.JsonSetting;
using EVE_Bot.JsonSetu;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVE_Bot
{
    public partial class frmMain : Form
    {
        private ClientWebSocket ws = new ClientWebSocket();
        private CancellationToken cancelState = new CancellationToken();
        private BackgroundWorker bgw = new BackgroundWorker();

        public frmMain()
        {
            InitializeComponent();
        }
        #region
        private Random rnd = new Random(100);

        //各Q群设置
        private static List<JsonGroup> lstGroupSetting = JsonConvert.DeserializeObject<List<JsonGroup>>(FilesHelper.ReadJsonFile("GroupSetting"));

        //ESI保存容器
        private static Dictionary<long, ESIAccessKey> dicESIContainer = JsonConvert.DeserializeObject<Dictionary<long, ESIAccessKey>>(FilesHelper.ReadJsonFile("ESISetting"));
        //Q群冷却计时器
        private Dictionary<Int64, System.Timers.Timer> dicGroupRepeat = new Dictionary<long, System.Timers.Timer>();

        //模块化
        private Dictionary<string, string> dicModuleConfig = new Dictionary<string, string>() { { "狼人杀", "WolfRequest" } };
        //模块化
        private Dictionary<string, Lazy<IMessageRequest>> dicRequestModule = new Dictionary<string, Lazy<IMessageRequest>>();
        private Dictionary<string, IMessageRequest> dicModule = new Dictionary<string, IMessageRequest>();
        #endregion


        //初期后台加载
        private void frmMain_Load(object sender, EventArgs e)
        {

            //ESIAccessKey accessKey = new ESIAccessKey();
            //accessKey.CharacterID = 90076612;
            //accessKey.AccessKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkpXVC1TaWduYXR1cmUtS2V5IiwidHlwIjoiSldUIn0.eyJzY3AiOiJlc2ktaW5kdXN0cnkucmVhZF9jb3Jwb3JhdGlvbl9qb2JzLnYxIiwianRpIjoiNzY2ZGMyNDEtM2U0Ny00NGNjLTgzNTktZjliN2JkNDM0NDk5Iiwia2lkIjoiSldULVNpZ25hdHVyZS1LZXkiLCJzdWIiOiJDSEFSQUNURVI6RVZFOjkwMDc2NjEyIiwiYXpwIjoiYmM5MGFhNDk2YTQwNDcyNGE5M2Y0MWI0ZjRlOTc3NjEiLCJ0ZW5hbnQiOiJzZXJlbml0eSIsInRpZXIiOiJsaXZlIiwicmVnaW9uIjoiY2hpbmEiLCJuYW1lIjoi57uv6Iie5LmL5aScIiwib3duZXIiOiJpa1V6SmFZVnFFeG5UUWFLMjBaN2l0MUlJeEU9IiwiZXhwIjoxNjE1MTA3NjczLCJpc3MiOiJsb2dpbi5ldmVwYy4xNjMuY29tIn0.m7hiBElEoGXumNTyFDxPiX4qnNi5QE-mCbMeHnVTWSk3PlIj8EqJyjniPOpf9LLaPQXUdZYK9NW7tOkr06mAt9ZDLkdEO29DDzaLaYBMllINmSdH4K9USLP6PyOl-TDaE60vUT1TUMZdwkxD9o86qcWaAJ0SpgWjb68pevVI7S5BqBbl_gyEve_nNCwVymZqIwcVJzd9rliSBKYyAFCXPp5iH9iXhIiLZkgPiJ3y32X48doOdAmbtBMwlfSnz2VZ3duKK5eaIrQR3uo1cssdcPAea6fs2k0Leti2xUD2UAfCSs5PPYpN2H10zUN73VcXZjg6qQy4Bel7-IPfNvGQbQ";
            //dicESIContainer.Add(173965593, accessKey);


            //FilesHelper.OutputJsonFile("ESISetting", JsonConvert.SerializeObject(dicESIContainer, Formatting.Indented));

            //アセンブリからクラスを取得
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(catalog, true);

            foreach (ComposablePartDefinition Parts in catalog.Parts)
            {

                foreach (ExportDefinition Module in Parts.ExportDefinitions)
                {
                    List<Lazy<IMessageRequest>> lstModule = container.GetExports<IMessageRequest>(Module.ContractName).ToList();
                    if (lstModule.Count != 1)
                    {
                        MessageBox.Show("Warning");
                    }
                    dicRequestModule.Add(Module.ContractName, lstModule[0]);
                }
            }
            //var provider = container.GetExport<IMessageRequest>(ConfigurationProvider.LogProviderName).Value;


            bgw.DoWork += Bgw_DoWork;
            bgw.ProgressChanged += Bgw_ProgressChanged;
            bgw.RunWorkerAsync();

        }

        //后台进程
        private async void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            await ws.ConnectAsync(new Uri("ws://127.0.0.1:5700"), cancelState);
            //string strValue = BuildSendMessage(443029533,this.rtbInput.Text);
            //strValue = Regex.Replace(strValue, "\"(\\w+)\":", "$1:");
            //await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strValue)), WebSocketMessageType.Text, true, cancelState);

            while (true)
            {
                byte[] byteCache = new byte[10000];
                ArraySegment<byte> Result = new ArraySegment<byte>(byteCache);
                await ws.ReceiveAsync(Result, CancellationToken.None);//接受数据
                var str = Encoding.UTF8.GetString(Result.ToArray(), 0, Result.Count);
                JORecvGroupMsg jsonGrpMsg = null;
                try
                {

                    jsonGrpMsg = JsonConvert.DeserializeObject<JORecvGroupMsg>(str);

                    if (bgw.CancellationPending)
                    {
                        MessageBox.Show("停止!");
                        break;
                    }
                    if (jsonGrpMsg.post_type == "meta_event")
                    {

                        continue;
                    }
                    if (jsonGrpMsg.message_type == "group")
                    {
                        JsonGroup group = lstGroupSetting.Find(obj => obj.group_id == jsonGrpMsg.group_id);
                        if (group == null)
                        {
                            group = new JsonGroup();
                            group.group_id = jsonGrpMsg.group_id;
                            group.CoolDownTime = 3600;
                            group.last_time = jsonGrpMsg.time;
                            lstGroupSetting.Add(group);
                            FilesHelper.OutputJsonFile("GroupSetting", JsonConvert.SerializeObject(lstGroupSetting, Formatting.Indented));
                        }
                        await DealWithMessage(ws, jsonGrpMsg);
                    }
                    else if (jsonGrpMsg.message_type == "private")
                    {
                        await DealWithMessagePrivateAsync(ws, jsonGrpMsg);
                    }

                }
                catch (Exception ex)
                {
                    string strError = TemplateBuilder.BuildSendMessage(jsonGrpMsg.group_id, "出错啦：" + ex.Message, false);
                    await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strError)), WebSocketMessageType.Text, true, cancelState);
                }

            }
        }

        private async Task DealWithMessagePrivateAsync(ClientWebSocket ws, JORecvGroupMsg jsonGrpMsg)
        {
            string strValue = string.Empty;
            strValue += rnd.Next() % 100 < 10 ? "喵" : "";

            string strMessage = TemplateBuilder.BuildSendMessagePrivate(jsonGrpMsg.user_id, strValue, false);
            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strMessage)), WebSocketMessageType.Text, true, cancelState);
        }

        //后台相应
        private void Bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        //发布提醒
        private async void btnAnnounce_Click(object sender, EventArgs e)
        {

            if (ws.State != WebSocketState.Open)
            {
                bgw.RunWorkerAsync();
            }
            Thread.Sleep(5000);

            foreach (JsonGroup group in lstGroupSetting)
            {
                string strBasePath = Application.StartupPath + @"\image\Back\";
                List<string> lstImage = Directory.EnumerateFiles(strBasePath).ToList();
                string strPathImage = lstImage[rnd.Next() % lstImage.Count].Replace("[", "&#91;").Replace("]", "&#93;");

                await SendMessage(ws, group.group_id, DateTime.Now.Ticks, "[CQ:image,file=" + strPathImage + "]");
                //+"我又回来啦！"
            }
        }

        //主要入口
        private async Task DealWithMessage(ClientWebSocket ws, JORecvGroupMsg jsonGrpMsg)
        {

            if (jsonGrpMsg.message == null)
            {
                return;
            }
            string strValue = string.Empty;
            JsonGroup group = lstGroupSetting.Find(obj => obj.group_id == jsonGrpMsg.group_id);
            if (!string.IsNullOrEmpty(group.RepeatCheck) && group.RepeatCheck == Commons.RemoveCQCode(jsonGrpMsg.message))
            {
                strValue = "不许学我说话";
                await SendMessage(ws, jsonGrpMsg.group_id, jsonGrpMsg.time, strValue);
                return;
            }

            //情话→彩虹屁
            if (MoodRequest.lstPrattle.Where(Prattle => { return jsonGrpMsg.message.Contains(Prattle); }).ToList().Count > 0)
            {
                string strFlatter = MoodRequest.lstFlatter[rnd.Next() % MoodRequest.lstFlatter.Count];
                strValue += strFlatter;
            }
            //敏感词→脏话
            if (MoodRequest.lstWarningWord.Where(Dirty => { return jsonGrpMsg.message.Contains(Dirty); }).ToList().Count > 0)
            {
                string strDirty = MoodRequest.lstDirtyWord[rnd.Next() % MoodRequest.lstDirtyWord.Count];
                strValue += strDirty;
            }

            if (jsonGrpMsg.message.Contains("[CQ:at,qq=" + jsonGrpMsg.self_id.ToString()))
            {
                strValue = MoodRequest.DealAtRequest(ws, jsonGrpMsg);
            }
            else if (jsonGrpMsg.message.StartsWith("创建赛马"))
            {
                Thread.Sleep(1000);
                List<string> lstHorse = new List<string>();
                lstHorse.Add("东海帝皇");
                lstHorse.Add("目白麦昆");
                lstHorse.Add("特别周");
                lstHorse.Add("米浴");
                strValue = "加入赛马 " + lstHorse[rnd.Next() % lstHorse.Count];
            }
            else if (dicModuleConfig.Keys.Where(Keys => { return jsonGrpMsg.message.StartsWith(Keys); }).ToList().Count > 0)
            {
                string strKey = dicModuleConfig.Keys.Where(Keys => { return jsonGrpMsg.message.StartsWith(Keys); }).First();
                Lazy<IMessageRequest> objModuleLazy = dicRequestModule[dicModuleConfig[strKey]];

                if (!objModuleLazy.IsValueCreated)
                {
                    IMessageRequest objModule = objModuleLazy.Value;
                    dicModule.Add(strKey, objModule);
                    strValue += "那个功能还没睡醒，等我叫一下试试";
                }
                else
                {
                    strValue += dicModule[strKey].DealRequest(jsonGrpMsg);
                }
            }
            else if (jsonGrpMsg.message.EndsWith("色图") || jsonGrpMsg.message.Contains("涩图"))
            {
                if (jsonGrpMsg.time - group.last_time < 60)
                {
                    string strBasePath = Application.StartupPath + @"\image\CoolDown\";
                    List<string> lstImage = Directory.EnumerateFiles(strBasePath).ToList();
                    //string strPathImage = WebUtility.HtmlEncode(lstImage[rnd.Next() % lstImage.Count]);
                    string strPathImage = lstImage[rnd.Next() % lstImage.Count].Replace("[", "&#91;").Replace("]", "&#93;");
                    strValue = "[CQ:image,file=" + strPathImage + "]";
                }
                else
                {
                    group.last_time = jsonGrpMsg.time;
                    JOResponse jOResponse = HSORequest.GetSetu();
                    if (jOResponse.code == 0)
                    {
                        foreach (JsonSetu.JOData Image in jOResponse.data)
                        {
                            strValue += "[CQ:image,file=" + Image.url + ",cache=1]";
                        }
                    }
                    else
                    {
                        strValue = "失败了，先等一会";
                    }
                }
            }
            //本业
            else if (jsonGrpMsg.message.StartsWith("!") || jsonGrpMsg.message.StartsWith("！"))
            {
                strValue = EVERequest.DealSearchRequest(ws, jsonGrpMsg);

                if (strValue.Length > 550)
                {
                    string strBasePath = Commons.DrawKyalImage(strValue);
                    //string strPathImage = WebUtility.HtmlEncode(lstImage[rnd.Next() % lstImage.Count]);
                    string strPathImage = strBasePath.Replace("[", "&#91;").Replace("]", "&#93;");
                    strValue = "[CQ:image,file=" + strPathImage + "]";
                }
                await SendMessage(ws, jsonGrpMsg.group_id, jsonGrpMsg.time, strValue);
                return;
            }
            //研究
            else
            {
                strValue += AIRequest.DealOtherRequest(jsonGrpMsg);
            }
            if (jsonGrpMsg.message.Contains("喵") && !jsonGrpMsg.message.Contains("喵子"))
            {
                //int nIndex = jsonGrpMsg.message.IndexOf("喵");
                //string strLast = jsonGrpMsg.message.Substring(nIndex + 1);
                string strLast = rnd.Next() % 10 > 6 ? "喵？" : "喵！";
                strValue += jsonGrpMsg.message + strLast;
            }

            if (!string.IsNullOrEmpty(strValue))
            {
                strValue += rnd.Next() % 100 < 10 ? "喵" : "";
                await SendMessage(ws, jsonGrpMsg.group_id, jsonGrpMsg.time, strValue);
            }
        }

        //发送消息功能重构
        private async Task SendMessage(ClientWebSocket ws, long group_id, long time, string strValue)
        {
            string strMessage = TemplateBuilder.BuildSendMessage(group_id, strValue, false);
            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strMessage)), WebSocketMessageType.Text, true, cancelState);
            JsonGroup group = lstGroupSetting.Find(obj => obj.group_id == group_id);
            if (group != null)
            {
                group.RepeatCheck = Commons.RemoveCQCode(strValue);
            }

            if (dicGroupRepeat.ContainsKey(group_id))
            {
                System.Timers.Timer Talking = dicGroupRepeat[group_id];
                Talking.Stop();
                Talking.Start();
            }
            else
            {
                System.Timers.Timer Talking = new System.Timers.Timer();
                Talking.Elapsed += RaiseTalking;
                Talking.SynchronizingObject = this;
                Talking.Interval = group.CoolDownTime * 1000;
                Talking.AutoReset = false;
                Talking.Start();
                dicGroupRepeat.Add(group_id, Talking);
            }
            return;
        }

        //冷场说话
        private async void RaiseTalking(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                long group_id = dicGroupRepeat.First(obj => { return obj.Value == (System.Timers.Timer)sender; }).Key;
                JsonGroup group = lstGroupSetting.First(obj => { return obj.group_id == group_id; });

                string strBasePath = Application.StartupPath + @"\image\Kyal\";
                List<string> lstImage = Directory.EnumerateFiles(strBasePath).ToList();
                //string strPathImage = WebUtility.HtmlEncode(lstImage[rnd.Next() % lstImage.Count]);
                string strPathImage = lstImage[rnd.Next() % lstImage.Count].Replace("[", "&#91;").Replace("]", "&#93;");

                string strValue = TemplateBuilder.BuildSendMessage(group.group_id, "[CQ:image,file=" + strPathImage + "]没有活人了喵？", false);
                await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strValue)), WebSocketMessageType.Text, true, cancelState);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Warning");
            }

        }
    }
}
