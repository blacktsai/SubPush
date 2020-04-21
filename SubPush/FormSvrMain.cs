using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using log4net;
using System.IO;
using System.Net.Sockets;
using System.Globalization;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Timers;
using System.Threading.Tasks;

namespace SubPush
{
    public partial class FormSvrMain : Form
    {
        //ILog secquotelogger;
        //ILog aplogger;
        string logconfig = AppDomain.CurrentDomain.BaseDirectory + @"log4net_config.xml";
        public FormSvrMain()
        {
            InitializeComponent();
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormSvrMain_Load(object sender, EventArgs e)
        {
            textBoxListenPort.Text = ConfigurationManager.AppSettings["ListenPort"];
            log4net.Config.XmlConfigurator.Configure(new FileInfo(logconfig));
            //secquotelogger = LogManager.GetLogger("secquotelogger");
            GD.aplogger = LogManager.GetLogger("aplogger");
            GD.reqlogger = LogManager.GetLogger("reqlogger");
            dgvClientInfo.AutoGenerateColumns = false;
            dgvSubStockList.AutoGenerateColumns = false;
            string constr = ConfigurationManager.ConnectionStrings["SubStockConn"].ToString();
            constr = string.Format(constr, ConfigurationManager.AppSettings["DBIP"], ConfigurationManager.AppSettings["DBName"],
                ConfigurationManager.AppSettings["DBUser"], ConfigurationManager.AppSettings["DBPassword"]);
            GD.SubAppDbTool = new DbTool(constr);
            GD.TimerPush = new System.Timers.Timer();
            GD.TimerPush.Elapsed += new ElapsedEventHandler(OnTimePushEvent);
            GD.TimerPush.Interval = 1000;
            GD.TimerPush.Enabled = true;
            GD.TimerPush.AutoReset = true;
            tsbStartSvr.PerformClick();
        }

     private void OnTimePushEvent(object sender, ElapsedEventArgs e)
　{
　     string curtime=e.SignalTime.ToString("HH:mm:ss");
         string stocktime = ConfigurationManager.AppSettings["SubStockInfoTime"];
         string shutdowntime = ConfigurationManager.AppSettings["ShutdownTime"];
         if (string.Compare(stocktime, curtime) == 0)
         {
             this.UISyncMsg(GD.Push_SubStockInfo, null, null, null);
         }
         if (string.Compare(shutdowntime, curtime) == 0)
         {
             Application.Exit();
         }
    }

        public void UISyncMsg(uint MsgType, object MsgPara, object ExtraPara, string MsgText)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<uint, object, object, string>(UISyncMsg), MsgType, MsgPara, ExtraPara, MsgText);
                return;
            }
            switch (MsgType)
            {
                case GD.Push_SvrStatus :
                    tsslSvrStatus.Text = (int)MsgPara == 0 ? "未執行" : "執行中";
                    tsslSvrStatus.ForeColor = (int)MsgPara == 0 ? Color.Blue : Color.Red;
                    tsbStartSvr.Text = (int)MsgPara == 0 ? "啟動" : "停止";
                    break;
                case GD.Push_Exception:
                    if (MsgText != null)
                    {
                        GD.aplogger.Error(MsgText);
                    }
                    break;
                case GD.Push_AddClient :
                    if (GD.bsClientInfo == null)
                    {
                        GD.bsClientInfo = new BindingSource();
                    }
                    if (GD.clientlist.Count > 0)
                    {
                        GD.bsClientInfo.DataSource = null;
                        GD.bsClientInfo.DataSource = GD.clientlist;
                        dgvClientInfo.DataSource = GD.bsClientInfo;
                        //dgvClientInfo.AutoResizeColumns();
                        //dgvClientInfo.Refresh();
                    }
                    GD.aplogger.Info(string.Format("{0} Connected", ((ClientInfo)MsgPara).ClientIP));
                    break;
                case GD.Push_RemoveClient:
                    ClientInfo cinfo = (ClientInfo)MsgPara;
                    if (cinfo != null)
                    {
                        GD.clientlist.SafeRemove(cinfo);
                        GD.bsClientInfo.DataSource = null;
                        GD.bsClientInfo.DataSource = GD.clientlist;
                        dgvClientInfo.DataSource = GD.bsClientInfo;
                        GD.aplogger.Info(string.Format("{0} Disconnected", cinfo.ClientIP));
                    }
                    break;
                case GD.Push_SubStockInfo:
                    tsbQuerySub.PerformClick();
                    break;
            }
            //textBoxReply2.AppendText(text);
            //secquotelogger.Info(text);
            //if (text.IndexOf("Exception", StringComparison.OrdinalIgnoreCase) > 0) 
            //  MessageBox.Show(text);
        }

        private void tsbStartSvr_Click(object sender, EventArgs e)
        {
            if (GD.pushsvr != null)
            {
                GD.pushsvr.EndTCPListen();
                GD.pushsvr = null;
                UISyncMsg(GD.Push_SvrStatus, 0, null, null);
                return;
            }
            GD.pushsvr = new ListenSvr(int.Parse(textBoxListenPort.Text.Trim()), this);
            GD.pushsvr.TCPListen();
            UISyncMsg(GD.Push_SvrStatus, 1, null, null);
        }

        private void tsbQuerySub_Click(object sender, EventArgs e)
        {
            TcpClient tcpClientQuote = new TcpClient();
            string respxml = null;
            Stopwatch stopWatch = new Stopwatch();
            
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                tcpClientQuote.Connect(ConfigurationManager.AppSettings["QueryHost"], int.Parse(ConfigurationManager.AppSettings["QueryPort"]));
                if (tcpClientQuote.Connected)
                {
                    //TaiwanCalendar twC = new TaiwanCalendar();
                    //string tdate = twC.GetYear(DateTime.Today) + "/" + twC.GetMonth(DateTime.Today) + "/" + twC.GetDayOfMonth(DateTime.Today);
                   string ldate = PushUtils.GetTALastTradeDate(tcpClientQuote, int.Parse(ConfigurationManager.AppSettings["OneTryTime"]), int.Parse(ConfigurationManager.AppSettings["RetryTimes"]));
                   string param = string.Format(ConfigurationManager.AppSettings["QuerySubParam"], ldate);
                    stopWatch.Start();
                    
                    respxml = PushUtils.QueryTA(tcpClientQuote, ConfigurationManager.AppSettings["QuerySubURL"] + "\x01" + param + "\x02",
                        int.Parse(ConfigurationManager.AppSettings["OneTryTime"]), int.Parse(ConfigurationManager.AppSettings["RetryTimes"]));
                    stopWatch.Stop();
                    textBox1.Text = stopWatch.ElapsedMilliseconds.ToString();
                }
                tcpClientQuote.Close();
                tcpClientQuote = null;
                if (GD.stocklist == null)
                {
                    GD.stocklist = new SubStockList();
                }
                if (GD.bsSubStockList == null)
                {
                    GD.bsSubStockList = new BindingSource();
                }
                stopWatch.Restart();
                if (!string.IsNullOrEmpty(respxml))
                    GD.stocklist.LoadList(respxml);
                stopWatch.Stop();
                textBox2.Text = stopWatch.ElapsedMilliseconds.ToString();
                stopWatch.Restart();
                if (GD.stocklist.Count > 0)
                {
                    GD.bsSubStockList.DataSource = null;
                    GD.bsSubStockList.DataSource = GD.stocklist.Values;
                    dgvSubStockList.DataSource = GD.bsSubStockList;

                    GD.stocklist.SaveToDB();
                }
                stopWatch.Stop();
                textBox3.Text = stopWatch.ElapsedMilliseconds.ToString();
            }
            catch (Exception ex)
            {
                GD.aplogger.Error(string.Format("Query Sub Stock exception {0}", ex.Message));
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void tsbSubPush_Click(object sender, EventArgs e)
        {
            //tsbQuerySub_Click(this.tsbQuerySub, e);
            Task.Factory.StartNew(PushNotify);
        }

        private void PushNotify()
        {
            string qrystr = "select a.*, b.SymbolName, b.Market, b.TempRefer, b.Refer, b.CouponDate, b.LastTDPrice, b.PriceDiff, " +
                    "b.PriceDiffRatio from UserSubRec a, SubStockInfo b " +
                    "where a.Symbol=b.Symbol and b.StopDate>=@StopDate order by a.Serial";
            if (GD.SubAppDbTool == null) return;
            SqlConnection conn = GD.SubAppDbTool.GetDbConnection();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            SqlDataReader dr = null;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                conn.Open();
                //cmd.Parameters.Clear();
                cmd.CommandText = qrystr;
                //cmd.Parameters.AddWithValue("@StopDate", DateTime.Today.ToString("yyyy/MM/dd"));
                cmd.Parameters.AddWithValue("@StopDate", "2014/04/04");
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (int.Parse(dr["Type"].ToString()) == 2)//漲幅
                    {
                        if (decimal.Parse(dr["Value"].ToString()) <= decimal.Parse(dr["PriceDiffRatio"].ToString()))
                        {
                            string content = string.Format(ConfigurationManager.AppSettings["MsgPriceDiffRatio"], dr["SymbolName"].ToString(), dr["Symbol"].ToString(),
                                dr["PriceDiffRatio"].ToString(), dr["Value"].ToString().Trim('0'));
                            NotifyMsgReply rep;
                            PushUtils.PushNotify(dr, content, out rep);
                        }
                    }
                    else if (int.Parse(dr["Type"].ToString()) == 0)//差價
                    {
                        if (decimal.Parse(dr["Value"].ToString()) <= decimal.Parse(dr["PriceDiff"].ToString()))
                        {
                            string content = string.Format(ConfigurationManager.AppSettings["MsgPriceDiff"], dr["SymbolName"].ToString(), dr["Symbol"].ToString(),
                                dr["PriceDiff"].ToString(), dr["Value"].ToString().Trim('0'));
                            NotifyMsgReply rep;
                            PushUtils.PushNotify(dr, content, out rep);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GD.aplogger.Error(string.Format("Sub Push Exception - {0}", ex.Message));
            }
            finally
            {
                if (dr != null && cmd != null)
                {
                    cmd.Cancel();
                    dr.Close();
                    cmd.Dispose();
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                Cursor.Current = Cursors.Default;
            }
        }

    }
}
