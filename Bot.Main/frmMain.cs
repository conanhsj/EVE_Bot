using Bot.Eve.Helper;
using Bot.ExtendInterface;
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
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bot.Eve
{
    public partial class frmMain : Form
    {
        private ClientWebSocket ws = new ClientWebSocket();
        private CancellationToken cancelState = new CancellationToken();
        private BackgroundWorker bgw = new BackgroundWorker();

        // 通过反射及导出方式实现的异步加载
        private Dictionary<string, Lazy<IMessageRequest>> dicRequestModule = new Dictionary<string, Lazy<IMessageRequest>>();
        // 已经加载的模块
        private Dictionary<string, IMessageRequest> dicModule = new Dictionary<string, IMessageRequest>();

        // 关键词名对应模块名(触发用)
        private Dictionary<string, string> dicConfig = new Dictionary<string, string>();
        // 模块名对应关键词,启动时检索用
        private Dictionary<string, string> dicCheck = new Dictionary<string, string>();


        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadingModule();

            LoadingDllModule();

            LoadingConfig();

            string strBasePath = Application.StartupPath;
            string strFinalName = Application.StartupPath + @"\Config\";
            if (!Directory.Exists(strFinalName))
            {
                Directory.CreateDirectory(strFinalName);
            }


            bgw.DoWork += Bgw_DoWork;
            bgw.ProgressChanged += Bgw_ProgressChanged;
        }

        private void Bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        //后台进程
        private async void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            await ws.ConnectAsync(new Uri("ws://127.0.0.1:5700"), cancelState);

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
                        await DealGroupRequest(ws, jsonGrpMsg);
                    }
                    else if (jsonGrpMsg.message_type == "private")
                    {
                        await DealPrivateRequest(ws, jsonGrpMsg);
                    }

                }
                catch (Exception ex)
                {
                    string strError = TemplateBuilder.BuildSendMessage(jsonGrpMsg.group_id, "出错啦：" + ex.Message, false);
                    await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strError)), WebSocketMessageType.Text, true, cancelState);
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

        private void LoadingConfig()
        {
            string strMessage = "";
            foreach (string strKey in dicRequestModule.Keys)
            {
                strMessage += strKey + Environment.NewLine;
            }

            this.txtMessage.Text += "成功加载下列功能:" + Environment.NewLine + strMessage;
        }

        private async Task DealGroupRequest(ClientWebSocket ws, JORecvGroupMsg jsonGrpMsg)
        {
            if (jsonGrpMsg.message == null)
            {
                return;
            }
            string strValue = string.Empty;
            //JsonGroup group = MoodRequest.lstGroupSetting.Find(obj => obj.group_id == jsonGrpMsg.group_id);

            if (dicConfig.Keys.Where(Keys => { return jsonGrpMsg.message.ToUpper().StartsWith(Keys); }).ToList().Count > 0)
            {
                string strKey = dicConfig.Keys.Where(Keys => { return jsonGrpMsg.message.ToUpper().StartsWith(Keys); }).First();
                Lazy<IMessageRequest> objModuleLazy = dicRequestModule[dicConfig[strKey]];

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
            return;


        }

        private Task DealPrivateRequest(ClientWebSocket ws, JORecvGroupMsg jsonGrpMsg)
        {
            throw new NotImplementedException();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!bgw.IsBusy)
            {
                bgw.RunWorkerAsync();
                this.btnStart.Text = "停止";
            }
            else
            {
                bgw.CancelAsync();
                this.btnStart.Text = "启动";
            }

        }
    }
}
