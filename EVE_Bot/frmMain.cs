using EVE_Bot.AILogic;
using EVE_Bot.Helper;
using EVE_Bot.JsonObject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVE_Bot
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        private ClientWebSocket ws = new ClientWebSocket();
        private CancellationToken cancelState = new CancellationToken();
        private BackgroundWorker bgw = new BackgroundWorker();

        private Dictionary<Int64, Int64> dicCoolDown = new Dictionary<long, long>();
        private Int64 lastRaiseTime = 0;

        private void frmMain_Load(object sender, EventArgs e)
        {
            bgw.DoWork += Bgw_DoWork;
            bgw.ProgressChanged += Bgw_ProgressChanged;
            bgw.RunWorkerAsync();

            dicCoolDown.Add(1075423869, 3600);
            dicCoolDown.Add(443029533, 300);
        }

        private void Bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

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

                    await DealWithMessage(ws, jsonGrpMsg);
                }
                catch (Exception ex)
                {
                    string strError = TemplateBuilder.BuildSendMessage(jsonGrpMsg.group_id, "出错啦：" + ex.Message, false);
                    await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strError)), WebSocketMessageType.Text, true, cancelState);
                }

            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //var url = "ws://127.0.0.1:5700";
            //ClientWebSocket ws = new ClientWebSocket();
            //await ws.ConnectAsync(new Uri(url), cancelState);

            //string strValue = JsonConvert.SerializeObject();

            if (ws.State == WebSocketState.Open)
            {
                return;
            }
            bgw.RunWorkerAsync();

        }
        private Random rnd = new Random(100);
        private Dictionary<Int64, Int64> dicGroupRepeat = new Dictionary<long, long>();
        private async Task DealWithMessage(ClientWebSocket ws, JORecvGroupMsg jsonGrpMsg)
        {
            if (jsonGrpMsg.time - lastRaiseTime < 2)
            {
                return;
            }
            if (jsonGrpMsg.message == null)
            {
                return;
            }


            string strValue = string.Empty;
            Int64 nCoolDown = dicCoolDown.ContainsKey(jsonGrpMsg.group_id) ? dicCoolDown[jsonGrpMsg.group_id] : 60;
            if (dicGroupRepeat.ContainsKey(jsonGrpMsg.group_id))
            {
                if (jsonGrpMsg.time - dicGroupRepeat[jsonGrpMsg.group_id] > nCoolDown)
                {
                    await SendMessage(ws, jsonGrpMsg.group_id, jsonGrpMsg.time, jsonGrpMsg.message);
                    return;
                }
            }


            if (jsonGrpMsg.message.Contains("骚") || jsonGrpMsg.message.Contains("淫"))
            {
                List<string> lstYulu = new List<string>();
                lstYulu.Add("爬");
                lstYulu.Add("滚滚滚");
                lstYulu.Add("怪起来了");
                strValue += lstYulu[rnd.Next() % lstYulu.Count];
            }

            if (jsonGrpMsg.message.Contains("[CQ:image,file="))
            {
                //strValue = "[CQ:image,file=https://images.ceve-market.org/status/status.png]";
            }
            else if (jsonGrpMsg.message.Contains("[CQ:at,qq=" + jsonGrpMsg.self_id.ToString()))
            //else if (jsonGrpMsg.message.Contains("查询"))
            {
                strValue = await EVERequest.DealAtRequest(ws, jsonGrpMsg);
            }
            else if (jsonGrpMsg.message.Contains("喵"))
            {
                int nIndex = jsonGrpMsg.message.IndexOf("喵");
                string strLast = jsonGrpMsg.message.Substring(nIndex);
                strLast += rnd.Next() % 10 > 6 ? "喵？" : "喵！";
                strValue += strLast;

            }

            if (!string.IsNullOrEmpty(strValue))
            {
                await SendMessage(ws, jsonGrpMsg.group_id, jsonGrpMsg.time, strValue);
            }

        }

        private async Task<string> SendMessage(ClientWebSocket ws, long group_id, long time, string strValue)
        {
            strValue = TemplateBuilder.BuildSendMessage(group_id, strValue, false);
            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(strValue)), WebSocketMessageType.Text, true, cancelState);
            if (dicGroupRepeat.ContainsKey(group_id))
            {
                dicGroupRepeat[group_id] = time;
            }
            else
            {
                dicGroupRepeat.Add(group_id, time);
            }

            return strValue;
        }



    }
}
