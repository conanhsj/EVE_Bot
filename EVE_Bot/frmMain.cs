using Bot.ExtendInterface;
using EVE_Bot.AILogic;
using EVE_Bot.Helper;
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

        public static Regex regCQCode = new Regex(@"(?:\[CQ:.*\])");

        public frmMain()
        {
            InitializeComponent();
        }
        #region
        //ESI保存容器
        private static Dictionary<long, ESIAccessKey> dicESIContainer = JsonConvert.DeserializeObject<Dictionary<long, ESIAccessKey>>(FilesHelper.ReadJsonFile("ESISetting"));
        //Q群冷却计时器
        private Dictionary<Int64, System.Timers.Timer> dicGroupRepeat = new Dictionary<long, System.Timers.Timer>();
        //模块化
        private Dictionary<string, string> dicModuleConfig = new Dictionary<string, string>() { { "狼人杀", "WolfRequest" }, { "ROLL", "RollRequest" }, { "占卜", "UraraRequest" }, { "赛马娘", "DerbyRequest" }, { "COC", "CoCRequest" } };
        //模块化
        private Dictionary<string, Lazy<IMessageRequest>> dicRequestModule = new Dictionary<string, Lazy<IMessageRequest>>();
        private Dictionary<string, IMessageRequest> dicModule = new Dictionary<string, IMessageRequest>();

        private System.Timers.Timer timerSchedule = new System.Timers.Timer();

        #endregion

        //初期后台加载
        private void frmMain_Load(object sender, EventArgs e)
        {
            CheckMoodTrigger();

            CreateScheduleTimer();

            LoadingModule();

            LoadingDllModule();

            bgw.DoWork += Bgw_DoWork;
            bgw.ProgressChanged += Bgw_ProgressChanged;
            bgw.RunWorkerAsync();
        }
        private void CheckMoodTrigger()
        {
            if (MoodRequest.lstWarningWord == null)
            {
                MoodRequest.lstWarningWord = new List<string>();
                this.rtbInput.Text += "敏感词加载失败，已重建\n";
            }
            else
            {
                this.rtbInput.Text += "敏感词加载成功，数量" + MoodRequest.lstWarningWord.Count + "\n";
            }
            if (MoodRequest.dicAnswer == null)
            {
                MoodRequest.dicAnswer = new Dictionary<string, string>();
                this.rtbInput.Text += "专有问答加载失败，已重建";
            }
            else
            {

            }

        }

        private void CreateScheduleTimer()
        {

            timerSchedule.Elapsed += DoSchedule;
            timerSchedule.SynchronizingObject = this;
            timerSchedule.Interval = 60 * 1000;
            timerSchedule.AutoReset = true;
            timerSchedule.Start();

            string strBasePath = Application.StartupPath;
            string strFinalName = strBasePath + Constants.Path_Schedule;
            if (!File.Exists(strFinalName))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(strFinalName));
                File.Create(strFinalName).Close();
            }
        }

        private void LoadingModule()
        {
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
        }

        private void LoadingDllModule()
        {
            string strBasePath = Application.StartupPath;
            string strFinalName = Application.StartupPath + @"\Plugins\";
            if (!Directory.Exists(strFinalName))
            {
                Directory.CreateDirectory(strFinalName);
            }
            List<string> lstDLL = Directory.GetFiles(strFinalName, "*.dll", SearchOption.AllDirectories).ToList();
            foreach (string dllPath in lstDLL)
            {
                Assembly Extend = Assembly.LoadFile(dllPath);
                var catalog = new AssemblyCatalog(Extend.CodeBase);
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
            }
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
                        JsonGroup group = MoodRequest.lstGroupSetting.Find(obj => obj.group_id == jsonGrpMsg.group_id);
                        if (group == null)
                        {
                            group = new JsonGroup();
                            group.group_id = jsonGrpMsg.group_id;
                            group.CoolDownTime = 7200;
                            group.last_time = jsonGrpMsg.time;
                            group.SetuOpen = true;
                            MoodRequest.lstGroupSetting.Add(group);
                            FilesHelper.OutputJsonFile("GroupSetting", JsonConvert.SerializeObject(MoodRequest.lstGroupSetting, Formatting.Indented));
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

                    string strErrReport = "在群：" + jsonGrpMsg.group_id + "的消息出错啦！\n";
                    strErrReport += jsonGrpMsg.sender.nickname + "(" + jsonGrpMsg.user_id + ")" + "说了:" + jsonGrpMsg.message + "\n";
                    strErrReport += "然后就出这个错：\n" + ex.ToString();
                    string strMessage = TemplateBuilder.BuildSendMessagePrivate(Constants.MasterQQ, strErrReport, true);
                    await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strMessage)), WebSocketMessageType.Text, true, cancelState);
                }

            }
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
            foreach (JsonGroup group in MoodRequest.lstGroupSetting)
            {
                string strBasePath = Application.StartupPath + @"\image\Back\";
                List<string> lstImage = Directory.EnumerateFiles(strBasePath).ToList();
                string strPathImage = lstImage[Commons.rnd.Next() % lstImage.Count].Replace("[", "&#91;").Replace("]", "&#93;");

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
            //获取对应群设定
            JsonGroup group = MoodRequest.lstGroupSetting.Find(obj => obj.group_id == jsonGrpMsg.group_id);

            //处理+去除CQ码
            string strMessage = jsonGrpMsg.message;
            bool bIsAtMe = false;
            while (regCQCode.IsMatch(strMessage))
            {
                string strCQCode = regCQCode.Match(strMessage).Value;
                if (jsonGrpMsg.message.Contains("[CQ:at,qq=" + jsonGrpMsg.self_id.ToString()))
                {
                    bIsAtMe = true;
                }
                strMessage = regCQCode.Replace(strMessage, "");
            }
            //过于频繁
            if (!string.IsNullOrEmpty(group.RepeatCheck) && strMessage.Contains(group.RepeatCheck)
                && jsonGrpMsg.time - group.last_time < 15)
            {
                strValue = Commons.rnd.Next() % 100 < 49 ? jsonGrpMsg.sender.card + "你烦不烦" : "你们好烦呐";
                await SendMessage(ws, jsonGrpMsg.group_id, jsonGrpMsg.time, strValue);
                return;
            }
            //队形打断
            if (!string.IsNullOrEmpty(group.RepeatCheck) && group.RepeatCheck == strMessage)
            {
                strValue = "不许学我说话";
                if (strMessage.Split(new string[] { "喵" }, StringSplitOptions.None).Length > 3)
                {

                    strValue += "，你喵那么多干嘛啊！";
                    return;
                }
                await SendMessage(ws, jsonGrpMsg.group_id, jsonGrpMsg.time, strValue);
                return;
            }
            //套娃禁止
            if (!string.IsNullOrEmpty(group.RepeatCheck) && strMessage.Contains(group.RepeatCheck))
            {
                int nIndex = jsonGrpMsg.message.IndexOf(group.RepeatCheck);
                string strBefore = jsonGrpMsg.message.Substring(0, nIndex);
                string strAfter = jsonGrpMsg.message.Substring(nIndex + group.RepeatCheck.Length);
                if ((!string.IsNullOrEmpty(strBefore) && group.RepeatCheck.Contains(strBefore)) ||
                     (!string.IsNullOrEmpty(strAfter) && group.RepeatCheck.Contains(strAfter)))
                {
                    strValue = "不许套娃";
                    await SendMessage(ws, jsonGrpMsg.group_id, jsonGrpMsg.time, strValue);
                    return;
                }
            }
            //情话→彩虹屁
            if (MoodRequest.lstPrattle.Where(Prattle => { return strMessage.Contains(Prattle); }).ToList().Count > 0)
            {
                string strFlatter = MoodRequest.lstFlatter[Commons.rnd.Next() % MoodRequest.lstFlatter.Count];
                strValue += strFlatter;
            }
            //敏感词→脏话
            if (MoodRequest.lstWarningWord.Where(Dirty => { return strMessage.Contains(Dirty); }).ToList().Count > 0)
            {
                string strDirty = MoodRequest.lstDirtyWord[Commons.rnd.Next() % MoodRequest.lstDirtyWord.Count];
                strValue += strDirty;
            }

            if (bIsAtMe)
            {
                strValue = MoodRequest.DealAtRequest(ws, jsonGrpMsg);
            }
            else if (dicModuleConfig.Keys.Where(Keys => { return strMessage.ToUpper().StartsWith(Keys); }).ToList().Count > 0)
            {
                string strKey = dicModuleConfig.Keys.Where(Keys => { return strMessage.ToUpper().StartsWith(Keys); }).First();
                Lazy<IMessageRequest> objModuleLazy = dicRequestModule[dicModuleConfig[strKey]];

                if (!objModuleLazy.IsValueCreated)
                {
                    IMessageRequest objModule = objModuleLazy.Value;
                    dicModule.Add(strKey, objModule);
                    Application.DoEvents();
                    strValue += dicModule[strKey].DealGroupRequest(jsonGrpMsg);
                }
                else
                {
                    strValue += dicModule[strKey].DealGroupRequest(jsonGrpMsg);
                }
            }
            else if (strMessage.EndsWith("色图") || strMessage.Contains("涩图"))
            {
                if (!group.SetuOpen)
                {
                    strValue = "看个屁的色图啊，找群主要啊";
                }
                else if (jsonGrpMsg.time - group.last_setu_time < 300)
                {
                    string strBasePath = Application.StartupPath + @"\image\CoolDown\";
                    List<string> lstImage = Directory.EnumerateFiles(strBasePath).ToList();
                    //string strPathImage = WebUtility.HtmlEncode(lstImage[rnd.Next() % lstImage.Count]);
                    string strPathImage = lstImage[Commons.rnd.Next() % lstImage.Count].Replace("[", "&#91;").Replace("]", "&#93;");
                    strValue = "[CQ:image,file=" + strPathImage + "]MD老娘死过一次叻，你们老实点";
                }
                else
                {
                    group.last_setu_time = jsonGrpMsg.time;
                    JOResponse jOResponse = HSORequest.GetSetu();
                    if (jOResponse.code == 0)
                    {
                        foreach (JsonSetu.JOData Image in jOResponse.data)
                        {
                            //strValue += "[CQ:image,file=" + Image.url + ",cache=1]";
                            strValue += "作者：" + Image.author + " 标题：" + Image.title + "\n";
                            strValue += Image.url;
                        }
                    }
                }
            }
            //本业
            else if (strMessage.StartsWith("!") || strMessage.StartsWith("！"))
            {
                strValue = EVERequest.DealSearchRequest(ws, jsonGrpMsg);
                await SendMessage(ws, jsonGrpMsg.group_id, jsonGrpMsg.time, strValue);
                return;
            }
            //研究
            else
            {
                strValue += AIRequest.DealOtherRequest(jsonGrpMsg);
            }


            // 避免喵子的触发
            if (strMessage.Split(new string[] { "喵" }, StringSplitOptions.None).Length >
                strMessage.Split(new string[] { "喵子", "喵祖", "喵隼", "喵帕斯" }, StringSplitOptions.None).Length)
            {
                int nCount = strMessage.Split(new string[] { "喵" }, StringSplitOptions.None).Length;
                if (nCount > 5)
                {
                    strValue += "你喵那么多干嘛啦";
                }
                else
                {
                    //int nIndex = jsonGrpMsg.message.IndexOf("喵");
                    //string strLast = jsonGrpMsg.message.Substring(nIndex + 1);
                    string strLast = Commons.rnd.Next() % 10 > 6 ? "喵？" : "喵！";
                    strValue += strMessage + strLast;
                }
            }

            if (!string.IsNullOrEmpty(strValue))
            {
                await SendMessage(ws, jsonGrpMsg.group_id, jsonGrpMsg.time, strValue);
            }
        }
        //私聊处理
        private async Task DealWithMessagePrivateAsync(ClientWebSocket ws, JORecvGroupMsg jsonGrpMsg)
        {
            try
            {
                string strValue = string.Empty;


                if (dicModuleConfig.Keys.Where(Keys => { return jsonGrpMsg.message.ToUpper().StartsWith(Keys); }).ToList().Count > 0)
                {
                    string strKey = dicModuleConfig.Keys.Where(Keys => { return jsonGrpMsg.message.ToUpper().StartsWith(Keys); }).First();
                    Lazy<IMessageRequest> objModuleLazy = dicRequestModule[dicModuleConfig[strKey]];

                    if (!objModuleLazy.IsValueCreated)
                    {
                        IMessageRequest objModule = objModuleLazy.Value;
                        dicModule.Add(strKey, objModule);
                        Application.DoEvents();
                        strValue += dicModule[strKey].DealPrivateRequest(jsonGrpMsg);
                    }
                    else
                    {
                        strValue += dicModule[strKey].DealPrivateRequest(jsonGrpMsg);
                    }
                }
                else if (jsonGrpMsg.message.EndsWith("色图") || jsonGrpMsg.message.Contains("涩图"))
                {
                    JOResponse jOResponse = HSORequest.GetSetu();
                    if (jOResponse.code == 0)
                    {
                        foreach (JsonSetu.JOData Image in jOResponse.data)
                        {
                            //strValue += "[CQ:image,file=" + Image.url + ",cache=1]";
                            strValue += "作者：" + Image.author + " 标题：" + Image.title + "\n";
                            strValue += Image.url;
                        }
                    }
                }
                else if (jsonGrpMsg.message.IndexOf("群发至：") >= 0)
                {
                    strValue = jsonGrpMsg.message.Substring(jsonGrpMsg.message.IndexOf("群发至："));
                }
                else if (jsonGrpMsg.message.StartsWith("释放"))
                {
                    dicRequestModule.Clear();
                    dicModule.Clear();
                    LoadingModule();
                    LoadingDllModule();
                    strValue += "成功加载" + dicRequestModule.Count + "模块 \n";
                    foreach (string strKey in dicRequestModule.Keys)
                    {
                        strValue += strKey + "\n";
                    }
                }
                else if (jsonGrpMsg.message.StartsWith("!") || jsonGrpMsg.message.StartsWith("！"))
                {
                    strValue = EVERequest.DealSearchRequest(ws, jsonGrpMsg);
                }
                else
                {
                    if (dicRequestModule.ContainsKey("AIRequest"))
                    {
                        Lazy<IMessageRequest> objModuleLazy = dicRequestModule["AIRequest"];
                        if (!objModuleLazy.IsValueCreated)
                        {
                            IMessageRequest objModule = objModuleLazy.Value;
                            dicModule.Add("AIRequest", objModule);
                            Application.DoEvents();
                            strValue += dicModule["AIRequest"].DealPrivateRequest(jsonGrpMsg);
                        }
                        else
                        {
                            strValue += dicModule["AIRequest"].DealPrivateRequest(jsonGrpMsg);
                        }
                    }
                }
                string strMessageGroup = string.Empty;
                if (strValue.IndexOf("群发至：") >= 0)
                {
                    strMessageGroup = strValue.Substring(strValue.IndexOf("群发至：") + 4).Trim();
                    long groupID = long.Parse(strMessageGroup.Substring(0, strMessageGroup.IndexOf(' ')));

                    strMessageGroup = strMessageGroup.Substring(strMessageGroup.IndexOf(' ') + 1).Trim();

                    strMessageGroup = TemplateBuilder.BuildSendMessage(groupID, strMessageGroup, false);
                    await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strMessageGroup)), WebSocketMessageType.Text, true, cancelState);
                }


                strValue += Commons.rnd.Next() % 100 < 10 ? "喵" : "";
                string strMessage = TemplateBuilder.BuildSendMessagePrivate(jsonGrpMsg.user_id, strValue, false);
                await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strMessage)), WebSocketMessageType.Text, true, cancelState);
            }
            catch (Exception ex)
            {
                string strMessage = TemplateBuilder.BuildSendMessagePrivate(Constants.MasterQQ, jsonGrpMsg.user_id + "说：" + jsonGrpMsg.message.ToUpper() + "的时候弄出BUG啦：\n" + ex.ToString(), false);
                await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strMessage)), WebSocketMessageType.Text, true, cancelState);
            }

        }

        //发送消息功能重构
        private async Task SendMessage(ClientWebSocket ws, long group_id, long time, string strValue)
        {
            strValue += Commons.rnd.Next() % 100 < 10 ? "喵" : "";
            string strMessage = TemplateBuilder.BuildSendMessage(group_id, strValue, false);
            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strMessage)), WebSocketMessageType.Text, true, cancelState);
            JsonGroup group = MoodRequest.lstGroupSetting.Find(obj => obj.group_id == group_id);
            if (group != null)
            {
                group.RepeatCheck = Commons.RemoveCQCode(strValue);
                group.last_time = time;
            }

            if (dicGroupRepeat.ContainsKey(group_id))
            {
                System.Timers.Timer Talking = dicGroupRepeat[group_id];
                Talking.Stop();
                Talking.Interval = group.CoolDownTime * 1000;
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
                //long group_id = dicGroupRepeat.First(obj => { return obj.Value == (System.Timers.Timer)sender; }).Key;
                //JsonGroup group = MoodRequest.lstGroupSetting.First(obj => { return obj.group_id == group_id; });

                //string strBasePath = Application.StartupPath + @"\image\Kyal\";
                //List<string> lstImage = Directory.EnumerateFiles(strBasePath).ToList();
                ////string strPathImage = WebUtility.HtmlEncode(lstImage[rnd.Next() % lstImage.Count]);
                //string strPathImage = lstImage[Commons.rnd.Next() % lstImage.Count].Replace("[", "&#91;").Replace("]", "&#93;");

                //string strWord = string.Empty;
                //if (DateTime.Now.Hour < 11)
                //{
                //    strWord += "都还没睡醒呢喵？";
                //}
                //else if (DateTime.Now.Hour < 13)
                //{
                //    strWord += "都去吃饭了喵？";
                //}
                //else if (DateTime.Now.Hour < 18)
                //{
                //    strWord += "还在工作呢喵？";
                //}
                //else if (DateTime.Now.Hour < 22)
                //{
                //    strWord += "都跑哪里去了喵？";
                //}
                //else
                //{
                //    strWord += "都睡着了喵？";
                //}

                //strWord += "已经两个小时没理我了";

                //string strValue = TemplateBuilder.BuildSendMessage(group.group_id, "[CQ:image,file=" + strPathImage + "]" + strWord, false);
                //await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strValue)), WebSocketMessageType.Text, true, cancelState);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Warning");
            }
        }

        private async void DoSchedule(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //读取任务内容
                string strContents;
                string strBasePath = Application.StartupPath;
                string strFinalName = strBasePath + Constants.Path_Schedule;
                if (!File.Exists(strFinalName))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(strFinalName));
                    File.Create(strFinalName).Close();
                }
                using (var sr = new StreamReader(strFinalName, Encoding.UTF8))
                {
                    strContents = sr.ReadToEnd();
                }

                if (string.IsNullOrEmpty(strContents))
                {
                    return;
                }
                Dictionary<DateTime, JsonSchedule> result = JsonConvert.DeserializeObject<Dictionary<DateTime, JsonSchedule>>(strContents);
                if (result == null)
                {
                    result = new Dictionary<DateTime, JsonSchedule>();
                }

                foreach (DateTime dtKey in result.Keys)
                {
                    if (dtKey < DateTime.Now)
                    {
                        JsonSchedule schedule = result[dtKey];

                        if (schedule.Type.ToUpper() == "GROUP")
                        {
                            SendMessage(ws, schedule.ID, DateTime.Now.Ticks, schedule.Info);
                        }
                    }
                }

                List<DateTime> lstKey = result.Keys.OrderBy(date => date.Ticks).ToList();
                for (int n = 0; n < lstKey.Count; n++)
                {
                    if (lstKey[n] < DateTime.Now)
                    {
                        if (!string.IsNullOrEmpty(result[lstKey[n]].Repeat))
                        {

                        }
                        result.Remove(lstKey[n]);
                    }
                }
            }
            catch (Exception ex)
            {
                string strMessage = TemplateBuilder.BuildSendMessagePrivate(Constants.MasterQQ, "定时器出问题啦：\n" + ex.ToString(), false);
                await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strMessage)), WebSocketMessageType.Text, true, cancelState);
            }
        }
    }
}
