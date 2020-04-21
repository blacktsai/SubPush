using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Xml.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using Newtonsoft.Json;
using System.Net;

namespace SubPush
{
    public class PushUtils
    {
        public PushUtils()
        {
        }

        static public string QueryTA(TcpClient tcpclient, string req, int onetrytime, int retrytimes)
        {
            NetworkStream ns = tcpclient.GetStream();
            if (ns.CanWrite)
            {

                byte[] reqBytes = Encoding.Default.GetBytes(req);
                ns.Write(reqBytes, 0, reqBytes.Length);
                /*byte[] reqBytes = Encoding.Default.GetBytes("/TASP/TASBSComMgmt.dll?GetNFSLists");
                ns.Write(reqBytes, 0, reqBytes.Length);
                ns.WriteByte(01);
                reqBytes = Encoding.Default.GetBytes("dwel=1&lino=70&tapage=GetSBSSort&date=103/03/14");
                ns.Write(reqBytes, 0, reqBytes.Length);
                ns.WriteByte(02);*/
            }
            if (ns.CanRead)
            {

                int numberOfBytesRead = 0;
                byte[] repdata = new byte[10240];
                Array.Clear(repdata, 0, repdata.Length);
                string repstr = string.Empty;
                int retry = 0;
                while (true)
                {
                    numberOfBytesRead = 0;
                    try
                    {
                        if (tcpclient.Available > 0)
                        {
                            numberOfBytesRead += ns.Read(repdata, 0, repdata.Length);
                            if (numberOfBytesRead > 0)
                            {

                                if (repdata[numberOfBytesRead - 1] == 03)
                                {
                                    repstr += Encoding.Default.GetString(repdata, 0, numberOfBytesRead - 1);
                                    return repstr;
                                }
                                else
                                {
                                    repstr += Encoding.Default.GetString(repdata, 0, numberOfBytesRead);
                                }
                            }
                            else return repstr;
                        }
                        else
                        {
                            Thread.Sleep(onetrytime);
                            if (++retry >= retrytimes)
                                return "<TAEXCEPTION>" + "Query Timeout!!" + "</TAEXCEPTION>";
                        }
                    }
                    catch (Exception ex)
                    {
                        return "<TAEXCEPTION>" + ex.Message + "</TAEXCEPTION>";
                    }

                }//while

            }//if (ns.CanRead)
            return null;
        }

        /// <summary>
        /// 民國年轉西元年
        /// </summary>
        /// <param name="RepublicDate"></param>
        /// <returns></returns>
        //網路上抄來的
        public static string RepublicToAD(string RepublicDate)
        {
            try
            {
                if (RepublicDate != null && RepublicDate.Length > 0)
                {
                    string[] temps = RepublicDate.Split('/');

                    if (temps.Length == 3)
                    {
                        RepublicDate = string.Format("{0}/{1}/{2}",
                            Convert.ToInt32(temps[0]) + 1911,
                            temps[1],
                            temps[2]);
                    }
                }
            }
            catch { }

            return RepublicDate;
        }

        static public string GetTaiwanToday()
        {
            TaiwanCalendar twC = new TaiwanCalendar();
            return (twC.GetYear(DateTime.Today) + "/" + twC.GetMonth(DateTime.Today) + "/" + twC.GetDayOfMonth(DateTime.Today));
            //twC = null;
        }

        static public string GetTALastTradeDate(TcpClient tcpclient, int onetrytime, int retrytimes)
        {
            string reqstr = ConfigurationManager.AppSettings["QueryLastTradeDateURL"] + "\x01" +
                        ConfigurationManager.AppSettings["QueryLastTradeDateParam"] + "\x02";
            string repxml = QueryTA(tcpclient, reqstr, int.Parse(ConfigurationManager.AppSettings["OneTryTime"]), 
                int.Parse(ConfigurationManager.AppSettings["RetryTimes"]));
            TextReader sr = new StringReader(repxml);
            XElement xe = XElement.Load(sr);
            sr.Close();
            var qitems = from qitem in xe.Descendants("NCCCALND")
                         select new
                         {
                             LastTradeDate = qitem.Attribute("prevdate") == null ? "" : qitem.Attribute("prevdate").Value,
                             GoodTradeDate = qitem.Attribute("date") == null ? "" : qitem.Attribute("date").Value
                         };
            if (qitems.Count() > 0)
            {
                return qitems.First().LastTradeDate;
            }else
                return "";
            
        }

        static public string QueryTAStockRefer(TcpClient tcpclient, string symbol, int onetrytime, int retrytimes)
        {
            string ldate = GetTALastTradeDate(tcpclient, onetrytime, retrytimes);
            if (string.IsNullOrEmpty(ldate))
                return "0";
            string param = string.Format(ConfigurationManager.AppSettings["QueryStockPriceParam"], PushUtils.GetTaiwanToday(), symbol);
            string reqstr = ConfigurationManager.AppSettings["QueryStockPriceURL"] + "\x01" + param + "\x02";

            string repxml = QueryTA(tcpclient, reqstr, int.Parse(ConfigurationManager.AppSettings["OneTryTime"]),
                int.Parse(ConfigurationManager.AppSettings["RetryTimes"]));
            TextReader sr = new StringReader(repxml);
            XElement xe = XElement.Load(sr);
            sr.Close();
            var qitems = from qitem in xe.Descendants("NFSSNAPS")
                         select new
                         {
                             Refer = qitem.Attribute("refe") == null ? "" : qitem.Attribute("refe").Value
                         };
            if (qitems.Count() > 0)
            {
                return qitems.First().Refer;
            }
            else
                return "0";
        }

        static public string QueryTAStockRefer(TcpClient tcpclient, string symbol, string LastTradeDate, int onetrytime, int retrytimes)
        {
            //string ldate = GetTALastTradeDate(tcpclient, onetrytime, retrytimes);
            //if (string.IsNullOrEmpty(ldate))
            //    return "0";
            string param = string.Format(ConfigurationManager.AppSettings["QueryStockPriceParam"], LastTradeDate, symbol);
            string reqstr = ConfigurationManager.AppSettings["QueryStockPriceURL"] + "\x01" + param + "\x02";

            string repxml = QueryTA(tcpclient, reqstr, int.Parse(ConfigurationManager.AppSettings["OneTryTime"]),
                int.Parse(ConfigurationManager.AppSettings["RetryTimes"]));
            TextReader sr = new StringReader(repxml);
            XElement xe = XElement.Load(sr);
            sr.Close();
            var qitems = from qitem in xe.Descendants("NFSSNAPS")
                         select new
                         {
                             Refer = qitem.Attribute("refe") == null ? "" : qitem.Attribute("refe").Value
                         };
            if (qitems.Count() > 0)
            {
                return qitems.First().Refer;
            }
            else
                return "0";
        }

        static public bool PushNotify(SqlDataReader dr, string content, out NotifyMsgReply rep)
        {
            NotifyMsg msg = new NotifyMsg();
            bool retvalue;
            msg.UserId = dr["ID"].ToString();
            msg.Content = content;
            string resbody = JsonConvert.SerializeObject(msg);
            HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["ProdNotifyURL"]);
            //StreamWriter sw = new StreamWriter(httpreq.GetRequestStream());
            httpreq.ContentType = "application/json; charset=UTF-8";
            httpreq.ProtocolVersion = HttpVersion.Version11;
            httpreq.Method = "POST";
            byte[] postBytes = Encoding.UTF8.GetBytes(resbody);
            httpreq.ContentLength = postBytes.Length;
            Stream rs = httpreq.GetRequestStream();
            rs.Write(postBytes, 0, postBytes.Length);
            rs.Flush();
            rs.Close();
            rs = null;

            HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
            //Stream stream = httpres.GetResponseStream();
            //byte[] datas = new byte[1024];
            //int cnt = stream.Read(datas, 0, 1024);
            //textBoxTokenReply.Text += Encoding.UTF8.GetString(datas, 0, cnt);

            StreamReader sr = new StreamReader(httpres.GetResponseStream());
            rep = JsonConvert.DeserializeObject<NotifyMsgReply>(sr.ReadToEnd());
            int num=0;
            sr.Close();
            httpreq = null;
            if (int.TryParse(rep.MsgNum, out num) && num == 0)
                retvalue = true;
            else
                retvalue = false;

            string addstr = "insert into UserPushLog (PushDate, PushTime, ID, NotifyMsgID, Symbol, PushMsg, ResultCode, ResultMessage, Channel, Range, "+
                "Type, Condition, Value, Times, Duration, UpdateTime, NotifySeq, ClientSeq) values (@PushDate, @PushTime, @ID, @NotifyMsgID, @Symbol, @PushMsg, "+
                "@ResultCode, @ResultMessage, @Channel, @Range, @Type, @Condition, @Value, @Times, @Duration, @UpdateTime, @NotifySeq, @ClientSeq)";
            if (GD.SubAppDbTool == null)
                retvalue = false;
            SqlConnection conn = GD.SubAppDbTool.GetDbConnection();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            string today=DateTime.Today.ToString("yyyy/MM/dd");
            try
            {
                conn.Open();
                //cmd.Parameters.Clear();
                cmd.CommandText = addstr;
                cmd.Parameters.AddWithValue("@PushDate", today);
                cmd.Parameters.AddWithValue("@PushTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@ID", dr["ID"].ToString());
                cmd.Parameters.AddWithValue("@NotifyMsgID", dr["Serial"].ToString());
                cmd.Parameters.AddWithValue("@Symbol",  (object)dr["Symbol"]?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PushMsg", resbody);
                cmd.Parameters.AddWithValue("@ResultCode",  (object)rep.MsgNum?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ResultMessage",  (object)rep.MsgTxt?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Channel",  (object)dr["Channel"]?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Range",  (object)dr["Range"]?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Type",  (object)dr["Type"]?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Condition",  (object)dr["Condition"]?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Value",  (object)dr["Value"]?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Times",  (object)dr["Times"]?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Duration",  (object)dr["Duration"]?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdateTime",  (object)dr["UpdateTime"]?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NotifySeq",  (object)rep.NotifySeq?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ClientSeq",  (object)rep.ClientSeq?? DBNull.Value);
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                GD.aplogger.Error(string.Format("PushNotify Exception - {0}", ex.Message));
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                conn.Close();
            }
            return retvalue;           
        }
    }

    static public class GD
    {
        static public int TAHeaderSize = 13;
        private const int WM_APP = 0x8000;//定義ID
        public const int Push_SvrStatus = WM_APP + 100;
        public const int Push_Exception = WM_APP + 101;
        public const int Push_AddClient = WM_APP + 102;
        public const int Push_RemoveClient = WM_APP + 103;
        public const int Push_SubStockInfo = WM_APP + 104;

        static public ILog reqlogger;
        static public ILog aplogger;
        static public string logconfig = AppDomain.CurrentDomain.BaseDirectory + @"log4net_config.xml";
        static public ListenSvr pushsvr;
        static public BindingSource bsClientInfo;
        static public BindingSource bsSubStockList;
        static public ClientInfoList<ClientInfo> clientlist = null;
        static public SubStockList stocklist = null;
        static public DbTool SubAppDbTool = null;
        static public System.Timers.Timer TimerPush = null;

        public const  string constAddSubRec = "6001";
        public const string constDelSubRec = "6051";
        public const string constQuerySubRec = "6101";

        public const string constOKCode = "0000";
        public const string constExpSqlCode = "1001";
        public const string constExpSocketCode = "2001";
        public const string constExpJsonCode = "3001";
        public const string constExpDefalutCode = "9001";
        //public const int APP_QuoteAdded = 101;
        //static public ParseTAQuote ptaQuote = null;
        //static public MSGHEAD msghead = new MSGHEAD();
        static public int TABodyLength(byte[] barray)
        {
            if (barray.Length == 4)
            {
                return (barray[0] * 16777216 +
                                    barray[1] * 65536 + barray[2] * 256 +
                                     (Int32)barray[3]);
            }
            else
            {
                return 0;
            }
        }

        
    }

    public class SubStock
    {
        public string Symbol {get;set; }
        public string SymbolName { get; set; }
        public string Market { get; set; }
        public string StartDate { get; set; }
        public string StopDate { get; set; }
        public string DebitDate { get; set; }//扣款日
        public string DrawDate { get; set; }//抽簽日
        public string Share { get; set; }//申購單位數
        public string TempRefer { get; set; }//暫時承銷價
        public string Refer { get; set; }//實際承銷價
        public string LastDeal { get; set; }//昨收
        public string Volumn { get; set; }//總量
        public string CouponDate { get; set; }//撥券日
        public string RefundDate { get; set; }//扣款日
        public string Txun { get; set; }//每單位股數
        public string TAMarket { get; set; }//大州市場別mark
        public string PriceDiff { 
            get {
                decimal refer=0m;
                if (!string.IsNullOrEmpty(Refer))
                    refer = decimal.Parse(Refer);
                else
                    refer = decimal.Parse(TempRefer);
                if (!string.IsNullOrEmpty(LastDeal))
                {
                    return (decimal.Parse(LastDeal) - refer).ToString();
                }
                else
                    return "";
            } }//價差
        public string PriceDiffRatio {
            get
            {
                decimal refer = 0m;
                if (!string.IsNullOrEmpty(Refer))
                    refer = decimal.Parse(Refer);
                else
                    refer = decimal.Parse(TempRefer);
                if (!string.IsNullOrEmpty(LastDeal) && refer != 0)
                {
                    return (((decimal.Parse(LastDeal) - refer)/refer * 100).ToString("0.00") + "%");
                }
                else
                    return "";
            }
            
        }//價差幅度

        public SubStock()
        {
        }

    }
    public class SubStockList : Dictionary<string, SubStock>
    {
        public SubStockList()
        {
        }

        public void LoadList(string xml)
        {
            string referdumm = "";
            this.Clear();
            TextReader sr = new StringReader(xml);
            XElement xe = XElement.Load(sr);
            sr.Close();
           
            //SubStock item = null;
            /*var qitems = from qitem in xe.Descendants("NFSLISTS")
                         select new
                         {
                             Symbol = qitem.Attribute("symb") == null ? "" : qitem.Attribute("symb").Value,
                             SymbolName = qitem.Attribute("name") == null ? "" : qitem.Attribute("name").Value,
                             Market = qitem.Attribute("mark") == null ? "" : qitem.Attribute("mark").Value,
                             StartDate = qitem.Attribute("name") == null ? "" : qitem.Attribute("name").Value
                             qitem.Element("NFSLIDURS").Element()
                         };*/
            var qitems = from qitem in xe.Descendants("NFSLISTS")
                         select qitem;
            TcpClient tcpClientQuote = new TcpClient();
            string ldate="";
            try
            {
                tcpClientQuote.Connect(ConfigurationManager.AppSettings["QueryHost"], int.Parse(ConfigurationManager.AppSettings["QueryPort"]));
                ldate = PushUtils.GetTALastTradeDate(tcpClientQuote, int.Parse(ConfigurationManager.AppSettings["OneTryTime"]), int.Parse(ConfigurationManager.AppSettings["RetryTimes"]));
            }
            catch (Exception ex)
            {
                GD.aplogger.Error(string.Format("SubStock loadlist TCP connect exception - {0}", ex.Message));
            }
            foreach (var qitem in qitems)
            {
                SubStock ss = new SubStock();
                var nitems = from nitem in qitem.Descendants("NFSLIDUR")
                             select nitem;
                referdumm = "";
                foreach (var nitem in nitems)
                {
                    if  (nitem.Attribute("duno") == null )
                        continue;
                    if (int.Parse(nitem.Attribute("duno").Value) == 0)
                    {
                        ss.StartDate = nitem.Attribute("duii").Value;
                        ss.StopDate = nitem.Attribute("ducl").Value;
                    }
                    else if (int.Parse(nitem.Attribute("duno").Value) == 5)
                    {
                        ss.DebitDate = nitem.Attribute("duii").Value;
                    }
                    else if (int.Parse(nitem.Attribute("duno").Value) == 10)
                    {
                        ss.DrawDate = nitem.Attribute("duii").Value;
                    }
                    else if (int.Parse(nitem.Attribute("duno").Value) == 20)
                    {
                        ss.RefundDate = nitem.Attribute("duii").Value;
                    }
                    else if (int.Parse(nitem.Attribute("duno").Value) == 30)
                    {
                        ss.CouponDate = nitem.Attribute("duii").Value;
                    }
                    else if (int.Parse(nitem.Attribute("duno").Value) == 91)
                    {
                        referdumm = nitem.Attribute("dumm").Value;
                        //if (referdumm == null || int.Parse(referdumm) > 0) 
                        //int result= 0;
                        decimal result = 0;
                        //if (!int.TryParse(referdumm, out result) || result == 0)
                       //     referdumm = null;
                        if (!decimal.TryParse(referdumm, out result) || result == 0)
                            referdumm = null;
                    }
                }
                ss.Symbol = qitem.Attribute("symb") == null ? "" : qitem.Attribute("symb").Value;
                string memo = qitem.Attribute("memo") == null ? "" : qitem.Attribute("memo").Value;
                ss.SymbolName = qitem.Attribute("name") == null ? "" : qitem.Attribute("name").Value;
                if (memo.IndexOf('(') >= 0)
                {
                    ss.Market = memo.Substring(memo.IndexOf('(') + 1, memo.IndexOf(')') - memo.IndexOf('(') - 1);
                    ss.SymbolName = memo.Substring(0, memo.IndexOf('('));
                }
                ss.Share = qitem.Attribute("shar") == null ? "" : qitem.Attribute("shar").Value;
                ss.TempRefer = string.IsNullOrEmpty(referdumm) ? qitem.Attribute("refe").Value : referdumm;
                if (DateTime.Today > DateTime.Parse(PushUtils.RepublicToAD(ss.DebitDate)))
                    ss.Refer = qitem.Attribute("refe") == null ? "" : qitem.Attribute("refe").Value;
                ss.Volumn = qitem.Attribute("valu") == null ? "" : qitem.Attribute("valu").Value;
                ss.Txun = qitem.Attribute("txun") == null ? "" : qitem.Attribute("txun").Value;
                ss.TAMarket = qitem.Attribute("mark") == null ? "" : qitem.Attribute("mark").Value;
                /*try{
                    tcpClientQuote.Connect(ConfigurationManager.AppSettings["QueryHost"], int.Parse(ConfigurationManager.AppSettings["QueryPort"]));
                    if (tcpClientQuote.Connected) {
                        ss.LastDeal = PushUtils.QueryTAStockRefer(tcpClientQuote, ss.Symbol, int.Parse(ConfigurationManager.AppSettings["OneTryTime"]), int.Parse(ConfigurationManager.AppSettings["RetryTimes"]));
                    }
                }catch (Exception ex) {
                    GD.aplogger.Error(string.Format("SubStock loadlist exception - {0}", ex.Message));
                }finally{
                    if (tcpClientQuote != null)
                        tcpClientQuote.Close();
                }*/
                
                //Add(ss.Symbol, ss);
                if (DateTime.Today > DateTime.Parse(PushUtils.RepublicToAD(ss.CouponDate)))
                    ss = null;
                else
                {
                    try
                    {
                        if (tcpClientQuote.Connected)
                        {
                            ss.LastDeal = PushUtils.QueryTAStockRefer(tcpClientQuote, ss.Symbol, ldate, 
                                int.Parse(ConfigurationManager.AppSettings["OneTryTime"]), int.Parse(ConfigurationManager.AppSettings["RetryTimes"]));
                        }
                    }
                    catch (Exception ex)
                    {
                        GD.aplogger.Error(string.Format("SubStock loadlist LastDeal exception - {0}", ex.Message));
                    }
                     Add(ss.Symbol, ss);
                }
            }//foreach
            if (tcpClientQuote != null)
                tcpClientQuote.Close();
        }

        public void SaveToDB()
        {
            
            SqlCommand cmd = null;
            if (GD.SubAppDbTool == null) return;
            SqlConnection conn = GD.SubAppDbTool.GetDbConnection();
            try
            {
                conn.Open();
                string delstr = "delete from SubStockInfo where Symbol=@Symbol and StartDate=@StartDate";
                string addstr = "insert into SubStockInfo (Symbol, SymbolName, Market, StartDate, StopDate, DebitDate, DrawDate, "+
                        "Share, Txun, TempRefer, Refer, Volumn, CouponDate, RefundDate, UpdateTime, LastTDPrice, PriceDiff, PriceDiffRatio, TAMarket) "+
                        "values (@Symbol, @SymbolName, @Market, @StartDate, @StopDate, @DebitDate, @DrawDate, @Share, @Txun, "+
                        "@TempRefer, @Refer, @Volumn, @CouponDate, @RefundDate, @UpdateTime, @LastTDPrice, @PriceDiff, @PriceDiffRatio, @TAMarket)";
                //SqlCommand cmd = new SqlCommand(delstr, conn);
                cmd = new SqlCommand();
                cmd.Connection = conn;
                foreach (KeyValuePair<string, SubStock> item in this)
                {
                    SubStock stock = (SubStock)item.Value;
                    cmd.Parameters.Clear();
                    cmd.CommandText = delstr;
                    cmd.Parameters.AddWithValue("@Symbol", stock.Symbol);
                    cmd.Parameters.AddWithValue("@StartDate", PushUtils.RepublicToAD(stock.StartDate));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.CommandText = addstr;
                    cmd.Parameters.AddWithValue("@Symbol", stock.Symbol);
                    cmd.Parameters.AddWithValue("@SymbolName", stock.SymbolName);
                    cmd.Parameters.AddWithValue("@Market", stock.Market);
                    if (!string.IsNullOrEmpty(stock.StartDate))
                        cmd.Parameters.AddWithValue("@StartDate", PushUtils.RepublicToAD(stock.StartDate));
                    else
                        cmd.Parameters.AddWithValue("@StartDate", DBNull.Value);
                    if (!string.IsNullOrEmpty(stock.StopDate))
                        cmd.Parameters.AddWithValue("@StopDate", PushUtils.RepublicToAD(stock.StopDate));
                    else
                        cmd.Parameters.AddWithValue("@StopDate", DBNull.Value);
                    if (!string.IsNullOrEmpty(stock.DebitDate))
                        cmd.Parameters.AddWithValue("@DebitDate", PushUtils.RepublicToAD(stock.DebitDate));
                    else
                        cmd.Parameters.AddWithValue("@DebitDate", DBNull.Value);
                    if (!string.IsNullOrEmpty(stock.DrawDate))
                        cmd.Parameters.AddWithValue("@DrawDate", PushUtils.RepublicToAD(stock.DrawDate));
                    else
                        cmd.Parameters.AddWithValue("@DrawDate", DBNull.Value);
                    if (!string.IsNullOrEmpty(stock.Share))
                        cmd.Parameters.AddWithValue("@Share", stock.Share);
                    else
                        cmd.Parameters.AddWithValue("@Share", 0);
                    if (!string.IsNullOrEmpty(stock.Txun))
                        cmd.Parameters.AddWithValue("@Txun", stock.Txun);
                    else
                        cmd.Parameters.AddWithValue("@Txun", 0);
                    if (!string.IsNullOrEmpty(stock.TempRefer))
                        cmd.Parameters.AddWithValue("@TempRefer", stock.TempRefer);
                    else
                        cmd.Parameters.AddWithValue("@TempRefer", 0);
                    if (!string.IsNullOrEmpty(stock.Refer))
                        cmd.Parameters.AddWithValue("@Refer", stock.Refer);
                    else
                        cmd.Parameters.AddWithValue("@Refer", 0);
                    if (!string.IsNullOrEmpty(stock.Volumn))
                        cmd.Parameters.AddWithValue("@Volumn", stock.Volumn);
                    else
                        cmd.Parameters.AddWithValue("@Volumn", 0);
                    if (!string.IsNullOrEmpty(stock.CouponDate))
                        cmd.Parameters.AddWithValue("@CouponDate", PushUtils.RepublicToAD(stock.CouponDate));
                    else
                        cmd.Parameters.AddWithValue("@CouponDate", DBNull.Value);
                    if (!string.IsNullOrEmpty(stock.RefundDate))
                        cmd.Parameters.AddWithValue("@RefundDate", PushUtils.RepublicToAD(stock.RefundDate));
                    else
                        cmd.Parameters.AddWithValue("@RefundDate", DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                    if (!string.IsNullOrEmpty(stock.LastDeal))
                        cmd.Parameters.AddWithValue("@LastTDPrice", stock.LastDeal);
                    else
                        cmd.Parameters.AddWithValue("@LastTDPrice", DBNull.Value);
                    if (!string.IsNullOrEmpty(stock.PriceDiff))
                        cmd.Parameters.AddWithValue("@PriceDiff", stock.PriceDiff);
                    else
                        cmd.Parameters.AddWithValue("@PriceDiff", DBNull.Value);
                    if (!string.IsNullOrEmpty(stock.PriceDiffRatio))
                        cmd.Parameters.AddWithValue("@PriceDiffRatio", stock.PriceDiffRatio.Trim('%'));
                    else
                        cmd.Parameters.AddWithValue("@PriceDiffRatio", DBNull.Value);
                    cmd.Parameters.AddWithValue("@TAMarket", (object)stock.TAMarket ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
                
            }
            catch (SqlException ex)
            {
                GD.aplogger.Error(ex.Message);
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                conn.Close();
            }

            //using (SqlConnection SqlConn = new SqlConnection(constr))
            //{
            //   /*SqlCommand Cmd = new SqlCommand(strSQL, SqlConn);
            //   SqlDataAdapter da = new SqlDataAdapter();
            //   try
            //   {
            //     .........
            //   }
            //   catch (Exception ex)
            //   {
                   
            //   }
            //   finally
            //   {
            //     SqlConnection.ClearPool(SqlConn);

            //    }*/
            //}
        }

    }

    public class DbTool
    {
        private string ConnectoinStr;

        public DbTool(string constr)
        {
            ConnectoinStr = constr;
        }

        public SqlConnection GetDbConnection()
        {
            return new SqlConnection(ConnectoinStr);  
        }
    }

    public class UserReq
    {
        public ClientInfo clientinfo;
        public string FunctionID;
        public string DateTime;
        public int DataLen;
        public string MsgID;
        public string Channel;
        public string jsonreq;

        public UserReq()
        {
        }
    }

    public class UserSubRec
    {
        public string NotifyMsgID;
        public string Channel;
        public string ID;
        public string Range;
        public string Symbol;
        public string Type;
        public string Condition;
        public string Value;
        public string Times;
        public string Duration;

        public UserSubRec()
        {
        }
    }

    public class ResAddSubRec
    {
        public string ResultCode;
        public string ResultMessage;
        public string NotifyMsgID;

        public ResAddSubRec(){
        }
    }

    public class ResQrySubDetail
    {
        public string NotifyMsgID;
        //public string Channel;
        //public string ID;
        public string Range;
        public string Symbol;
        public string Type;
        public string Condition;
        public string Value;
        public string Times;
        public string Duration;
        public string SymbolName;
        public string Market;
        public string TempRefer;
        public string Refer;
        public string CouponDate;

        public ResQrySubDetail()
        {
        }
    }

    public class ResQrySubRec
    {
        public string ResultCode;
        public string ResultMessage;
        public string ID;
        public ResQrySubDetail[] Detail;

        public ResQrySubRec()
        {
        }
    }

    public class NotifyMsg
    {
        public string Operator="CSAPP";
        public string Dispatcher="T";
        public string ClientSeq="";
        public string UserId="";
        public string Device="1";//先設iPhone
        public string Channel="2";//先設為致新iPhone
        public string Content="";
        public string Expire="";
        public string Custom="";
        public string Group="1";

        public NotifyMsg() {
        }
    }

    public class NotifyMsgReply
    {
        public string NotifySeq ;
        public string ClientSeq;
        public string MsgNum;
        public string MsgTxt;

        public NotifyMsgReply()
        {
        }
    }
}
