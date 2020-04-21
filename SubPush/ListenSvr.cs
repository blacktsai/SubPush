using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Data.SqlClient;
using System.Data;

namespace SubPush
{
    public class ListenSvr
    {
        //private string HostIP;
        private int HostPort;
        private TcpListener tcplistener;
        //private static ManualResetEvent resetevent = new ManualResetEvent(false);
        //private volatile bool StopListen = false;
        //public ClientBags<ClientInfo> clientlist = null;
        private FormSvrMain HandleForm;


        public ListenSvr(int aPort, FormSvrMain aForm)
        {
            //HostIP = aIP;
            HostPort = aPort;
            HandleForm = aForm;
            GD.clientlist = new ClientInfoList<ClientInfo>();
        }

        public void EndTCPListen()
        {
            //StopListen = true;
            if (tcplistener != null)
            {
                tcplistener.Stop();
            }
        }

        public void TCPListen()
        {
            tcplistener = new TcpListener(IPAddress.Any, HostPort);
            tcplistener.Start();
            if (tcplistener != null)
                tcplistener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptTCP), null);
        }

        public void OnAcceptTCP(IAsyncResult ar)
        {
            if (tcplistener == null || tcplistener.Server == null || !tcplistener.Server.IsBound)
                return;
            if (tcplistener != null)
                tcplistener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptTCP), tcplistener);
            TcpClient client = tcplistener.EndAcceptTcpClient(ar);
            //判斷是否斷線
            if (client == null || !client.Connected)
                return;
            ClientInfo cinfo = new ClientInfo();
            IPEndPoint ipinfo = (IPEndPoint)client.Client.RemoteEndPoint;
            cinfo.ClientIP = ipinfo.Address.ToString();
            cinfo.ClientPort = ipinfo.Port;
            cinfo.ConnectionTime = DateTime.Now.ToString("HH:mm:ss");
            cinfo.Status = 'C';
            cinfo.TCPConnection = client;
            GD.clientlist.SafeAdd(cinfo);
            HandleForm.UISyncMsg(GD.Push_AddClient, cinfo, null, null);
            try
            {

                if (client.Connected)
                {
                    NetworkStream ns = client.GetStream();
                    if (ns.CanRead)
                    {
                        ns.BeginRead(cinfo.buf, 0, cinfo.bufsize,
                            new AsyncCallback(OnReadData), cinfo);
                        //receiveDone.WaitOne();
                    }
                    /*if (ns.CanWrite)
                    {
                        String req = "1002@,0x01010001,,0xFFFF00FF:,0x01010002,,0xFFFF00FF:" +
                                     ",0x01020001,,0xFFFF00FF:,0x01020002,,0xFFFF00FF:" +
                                     ",0x01040001,,0xFFFF00FF:,0x01040002,,0xFFFF00FF" + '\u0001';//證行情訂閱
                        byte[] reqBytes = Encoding.Default.GetBytes(req);
                        ns.BeginWrite(reqBytes, 0, reqBytes.Length,
                                new AsyncCallback(EndWriteCallback), state);
                        //sendDone.WaitOne();
                    }*/

                }
                else
                {
                    //HandleForm.DisplayText(string.Format("EndConnectCallback Exception Happen {0}\n", "Connect Failed!"));
                    //DisplayStatus(string.Format("Ready (last error: {0})", "Connect Failed!"));
                }
            }
            catch (Exception ex)
            {
                if (client != null)
                {
                    client.Close();
                    client = null;
                }
                HandleForm.UISyncMsg(GD.Push_Exception, ex, null, string.Format("OnAcceptTCP Exception Happen {0}\n", ex.Message));
                //HandleForm.DisplayText(string.Format("OnReadData Exception Happen {0}\n", ex.Message));
            }

        }

        private void OnReadData(IAsyncResult ar)
        {
            ClientInfo cinfo = (ClientInfo)ar.AsyncState;
            int bytesRead = 0;
            byte[] testByte = new byte[1];
            if (cinfo.TCPConnection == null || !cinfo.TCPConnection.Connected)
            {
                HandleForm.UISyncMsg(GD.Push_Exception, null, null, "Detect client disconnect");
                HandleForm.UISyncMsg(GD.Push_RemoveClient, cinfo, null, null);
                return;
            }
            try
            {
                //使用Peek測試連線是否仍存在
                if (cinfo.TCPConnection.Client.Poll(0, SelectMode.SelectRead) &&
                        cinfo.TCPConnection.Client.Receive(testByte, SocketFlags.Peek) == 0)
                {
                    HandleForm.UISyncMsg(GD.Push_Exception, null, null, "Detect client disconnect");
                    HandleForm.UISyncMsg(GD.Push_RemoveClient, cinfo, null, null);
                    //HandleForm.DisplayText("Detect client disconnect\n");
                    return;
                }
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10054)
                {
                    //HandleForm.UISyncMsg(GD.Push_Exception, ex, null, string.Format("{0} Disconnected!", cinfo.ClientIP));
                    HandleForm.UISyncMsg(GD.Push_RemoveClient, cinfo, null, null);
                } else
                    HandleForm.UISyncMsg(GD.Push_Exception, ex, null, string.Format("OnReadData Exception {0}", ex.Message));
                //HandleForm.DisplayText(string.Format("OnReadData Exception Happen {0}\n", ex.Message));
                return;
            }
            NetworkStream stream = cinfo.TCPConnection.GetStream();
            //int BodyLen = 0;
            //byte[] blenarray = new byte[4];

            //block until data is available
            try
            {
                bytesRead = stream.EndRead(ar);
            }
            catch (SocketException ex)
            {
                HandleForm.UISyncMsg(GD.Push_Exception, ex, null, string.Format("stream.EndRead Exception {0}", ex.Message));
                //HandleForm.DisplayText(string.Format("stream.EndRead Exception Happen {0}\n", ex.Message));
            }


            if (bytesRead > 0)
            {
                //state.Data.Append(Encoding.UTF8.GetString(state.Buffer, 0, bytesRead));
                try
                {
                    Array.Copy(cinfo.buf, 0, cinfo.tmpbuf, cinfo.tailidx, bytesRead);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    HandleForm.UISyncMsg(GD.Push_Exception, ex, null, string.Format("OnReadData Exception Happen {0}\n"));
                    //HandleForm.DisplayText(string.Format("OnReadData Exception Happen {0}\n", ex.Message));
                }

                cinfo.tailidx += bytesRead;
                try
                {
                    while (true)
                    {
                        if ((cinfo.tailidx - cinfo.headidx) >= AppMsgHeader.HeaderLen)
                        {
                            Array.Clear(cinfo.MsgHeader.HeaderData, 0, AppMsgHeader.HeaderLen);
                            Array.Copy(cinfo.tmpbuf, cinfo.headidx, cinfo.MsgHeader.HeaderData, 0, AppMsgHeader.HeaderLen);
                            //BodyLen = GlobalData.TABodyLength(blenarray);
                            //建名的訊息bodylength不含header,但大州的含header length,故資料PARSE邏輯不一樣
                            //cinfo.headidx += AppMsgHeader.HeaderLen;
                            if (cinfo.MsgHeader.BodyLength == 0)
                                break;
                            if ((cinfo.tailidx - cinfo.headidx) >= (cinfo.MsgHeader.BodyLength + AppMsgHeader.HeaderLen))
                            {
                                string jsonstr = Encoding.UTF8.GetString(cinfo.tmpbuf, cinfo.headidx + AppMsgHeader.HeaderLen,
                                        cinfo.MsgHeader.BodyLength);
                                if (jsonstr.Length > 0)
                                {
                                    UserReq req = new UserReq();
                                    req.clientinfo = cinfo;
                                    req.FunctionID = Encoding.UTF8.GetString(cinfo.MsgHeader.FunctionID);
                                    req.DateTime = Encoding.UTF8.GetString(cinfo.MsgHeader.DateTime);
                                    req.DataLen = cinfo.MsgHeader.BodyLength;
                                    req.MsgID = Encoding.UTF8.GetString(cinfo.MsgHeader.MsgID).Trim(Convert.ToChar(0));
                                    req.Channel = Encoding.UTF8.GetString(cinfo.MsgHeader.Channel).Trim(Convert.ToChar(0));
                                    req.jsonreq = jsonstr;
                                    cinfo.ReqQueue.Enqueue(req);
                                    ThreadPool.QueueUserWorkItem(new WaitCallback(cinfo.ThreadReqTask));
                                    //cinfo.mrevent.Set();
                                    //req.FunctionID = 

                                    //HandleForm.DisplayText(string.Format("BodyLength - {0}\r\n{1}", AppMsgHeader.BodyLength, jsonstr));
                                    //Emps emps = JsonConvert.DeserializeObject<Emps>(jsonstr);
                                    //HandleForm.DisplayText(string.Format("\r\nfirstname : {0}\r\nlastname : {1}", emps.employees[0].firstname, emps.employees[0].lastname));
                                    //HandleForm.DisplayText(string.Format("\r\nfirstname : {0}\r\nlastname : {1}", emps.employees[1].firstname, emps.employees[1].lastname));
                                }
                                else
                                {
                                    //HandleForm.DisplayText("Heart Beat!!");
                                }
                                cinfo.headidx += cinfo.MsgHeader.BodyLength + AppMsgHeader.HeaderLen;
                            }
                            else
                            {
                                int leftsize = cinfo.tailidx - cinfo.headidx;
                                Array.Copy(cinfo.tmpbuf, cinfo.headidx, cinfo.tmpbuf, 0, leftsize);
                                cinfo.headidx = 0;
                                cinfo.tailidx = leftsize;
                                Array.Clear(cinfo.tmpbuf, cinfo.tailidx, cinfo.tmpbuf.Length - leftsize);
                                break;
                            }

                        }
                        else
                        {
                            int leftsize = cinfo.tailidx - cinfo.headidx;
                            Array.Copy(cinfo.tmpbuf, cinfo.headidx, cinfo.tmpbuf, 0, leftsize);
                            cinfo.headidx = 0;
                            cinfo.tailidx = leftsize;
                            Array.Clear(cinfo.tmpbuf, cinfo.tailidx, cinfo.tmpbuf.Length - leftsize);
                            break;
                        }

                    }//while
                    stream.BeginRead(cinfo.buf, 0, cinfo.bufsize,
                        new AsyncCallback(OnReadData), cinfo);
                }
                catch (Exception ex)
                {
                    HandleForm.UISyncMsg(GD.Push_Exception, ex, null, string.Format("EndReadCallback Exception {0}", ex.Message));
                    //HandleForm.DisplayText(string.Format("EndReadCallback Exception Happen {0}\n", ex.Message));
                    return;
                }

            }
            else
            {
                HandleForm.UISyncMsg(GD.Push_Exception, null, null, "Detect client disconnect read 0 byte\n");
                //HandleForm.DisplayText("Detect client disconnect read 0 byte\n");
                return;
            }
        }

        //private void ThreadReqTask(object userreq)
        //{
        //    ((UserReq)userreq).clientinfo.ReqQueue.Enqueue((UserReq)userreq);
        //}
    }

    public class ClientInfo
    {
        public string ClientIP {get;set;}
        public int ClientPort { get; set; }
        public string ConnectionTime { get; set; }
        public char Status { get; set; }//'C' - connected ; 'D' - Disconnected
        public TcpClient TCPConnection { get; set; }
        public byte[] buf;
        public byte[] tmpbuf;
        public int bufsize = 1024;
        public int headidx = 0;
        public int tailidx = 0;

        public ConcurrentQueue<UserReq> ReqQueue;
        public ManualResetEvent mrevent;
        public AppMsgHeader MsgHeader;

        public ClientInfo()
        {
            buf = new byte[bufsize];
            tmpbuf = new byte[2 * bufsize];
            ReqQueue = new ConcurrentQueue<UserReq>();
            mrevent = new ManualResetEvent(false);
            MsgHeader = new AppMsgHeader();
        }

        private byte[] ResponseBytes(string funid, string dtime, string msgid, string channel, string jsonbody)
        {
            AppMsgHeader header = new AppMsgHeader();
            byte[] body = Encoding.UTF8.GetBytes(jsonbody);
            byte[] msg = new byte[AppMsgHeader.HeaderLen + body.Length];
            Array.Clear(msg,0 ,  msg.Length);
            //int blength = body.Length;
            header.FunctionID = Encoding.UTF8.GetBytes(funid);
            header.DateTime = Encoding.UTF8.GetBytes(dtime);
            header.DataLen = Encoding.UTF8.GetBytes(body.Length.ToString("D5"));
            header.MsgID = Encoding.UTF8.GetBytes(msgid);
            header.Channel = Encoding.UTF8.GetBytes(channel);
            Array.Copy(header.HeaderData, 0, msg, 0, AppMsgHeader.HeaderLen);
            Array.Copy(body, 0, msg, AppMsgHeader.HeaderLen, body.Length);
            return msg;
        }

        private void WriteResponse(UserReq ureq, string resultcode, string resultmsg, UserSubRec urec)
        {
            string resbody = null;
            byte[] resmsg = null;
            NetworkStream ns = null;
            switch (ureq.FunctionID)
            {
                case GD.constAddSubRec :
                case GD.constDelSubRec:
                    ResAddSubRec resadd = new ResAddSubRec();
                    resadd.ResultCode = resultcode;
                    resadd.ResultMessage = resultmsg;
                    resadd.NotifyMsgID = urec.NotifyMsgID;
                    resbody = JsonConvert.SerializeObject(resadd);
                    resmsg = ResponseBytes(ureq.FunctionID, DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"), ureq.MsgID, ureq.Channel, resbody);
                    ns = TCPConnection.GetStream();
                    if (ns.CanWrite)
                    {
                        ns.Write(resmsg, 0, resmsg.Length);
                    }
                    resmsg = null;
                    GD.reqlogger.Info(string.Format("IP-{0} ; FunID-{1} ; MsgID-{2} ; Response-{3}",
                            ureq.clientinfo.ClientIP, ureq.FunctionID, ureq.MsgID, resbody));
                    break;
                /*case GD.constDelSubRec:
                    ResAddSubRec resdel = new ResAddSubRec();
                    resdel.ResultCode = resultcode;
                    resdel.ResultMessage = resultmsg;
                    resdel.NotifyMsgID = urec.NotifyMsgID;
                    resbody = JsonConvert.SerializeObject(resdel);
                    resmsg = ResponseBytes(ureq.FunctionID, DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"), ureq.MsgID, resbody);
                    ns = TCPConnection.GetStream();
                    if (ns.CanWrite)
                    {
                        ns.Write(resmsg, 0, resmsg.Length);
                    }
                    resmsg = null;
                    break;*/
            }

            
        }

        private void WriteResponse(UserReq ureq, string resultcode, string resultmsg, UserSubRec urec, SqlDataReader dr)
        {
            string resbody = null;
            byte[] resmsg = null;
            NetworkStream ns = null;
            List<ResQrySubDetail> dlist = new List<ResQrySubDetail>();
            switch (ureq.FunctionID)
            {
                case GD.constQuerySubRec:
                    ResQrySubRec resqry = new ResQrySubRec();
                    resqry.ResultCode = resultcode;
                    resqry.ResultMessage = resultmsg;
                    resqry.ID = urec.ID;
                    while(dr.Read()){
                        ResQrySubDetail item = new ResQrySubDetail();
                        item.NotifyMsgID = dr["Serial"].ToString();
                        item.Range = dr["Range"].ToString();
                        item.Symbol = dr["Symbol"].ToString();
                        item.Type = dr["Type"].ToString();
                        item.Condition = dr["Condition"].ToString();
                        item.Value = dr["Value"].ToString();
                        item.Times = dr["Times"].ToString();
                        item.Duration = dr["Duration"].ToString();
                        item.SymbolName = dr["SymbolName"].ToString();
                        item.Market = dr["Market"].ToString();
                        item.TempRefer = dr["TempRefer"].ToString();
                        item.Refer = dr["Refer"].ToString();
                        item.CouponDate = dr.GetDateTime(dr.GetOrdinal("CouponDate")).ToString("yyyy/MM/dd");
                        //Convert.ToDateTime(dr["CouponDate"]).ToString("yyyy/MM/dd")
                        dlist.Add(item);
                    }
                    resqry.Detail = dlist.ToArray();

                    resbody = JsonConvert.SerializeObject(resqry);
                    resmsg = ResponseBytes(ureq.FunctionID, DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"), ureq.MsgID, ureq.Channel, resbody);
                    ns = TCPConnection.GetStream();
                    if (ns.CanWrite)
                    {
                        ns.Write(resmsg, 0, resmsg.Length);
                    }
                    resmsg = null;
                    GD.reqlogger.Info(string.Format("IP-{0} ; FunID-{1} ; MsgID-{2} ; Response-{3}",
                            ureq.clientinfo.ClientIP, ureq.FunctionID, ureq.MsgID, resbody));
                    break;
            }


        }

 
        private void WriteExpResponse(UserReq ureq, string resultcode, string resultmsg, Exception ex)
        {
            string resbody = null;
            byte[] resmsg = null;
            NetworkStream ns = null;
            switch (ureq.FunctionID)
            {
                case GD.constAddSubRec:
                case GD.constDelSubRec:
                    ResAddSubRec resadd = new ResAddSubRec();
                    resadd.ResultCode = resultcode;
                    resadd.ResultMessage = resultmsg;
                    resadd.NotifyMsgID = "";
                    resbody = JsonConvert.SerializeObject(resadd);
                    resmsg = ResponseBytes(ureq.FunctionID, DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"), ureq.MsgID, ureq.Channel, resbody);
                    ns = TCPConnection.GetStream();
                    if (ns.CanWrite)
                    {
                        ns.Write(resmsg, 0, resmsg.Length);
                    }
                    resmsg = null;
                    GD.reqlogger.Info(string.Format("IP-{0} ; FunID-{1} ; MsgID-{2} ; Response-{3}",
                            ureq.clientinfo.ClientIP, ureq.FunctionID, ureq.MsgID, resbody));
                    break;
                /*case GD.constDelSubRec:
                    ResAddSubRec resdel = new ResAddSubRec();
                    resdel.ResultCode = resultcode;
                    resdel.ResultMessage = resultmsg;
                    resdel.NotifyMsgID = "";
                    resbody = JsonConvert.SerializeObject(resdel);
                    resmsg = ResponseBytes(ureq.FunctionID, DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"), ureq.MsgID, resbody);
                    ns = TCPConnection.GetStream();
                    if (ns.CanWrite)
                    {
                        ns.Write(resmsg, 0, resmsg.Length);
                    }
                    break;*/
            }
            
        }

        //private string resjson(short restype, object obj)
        //{
        //    switch (restype)
        //    {
        //        case 1 :
        //            break;
        //        default:

        //            break;
        //    }
        //}

        public void ThreadReqTask(Object obj)
        {
            UserReq req;
            SqlCommand cmd = null;
            UserSubRec rec = null;
            SqlConnection conn = null;
            SqlDataReader dr = null;
            while (ReqQueue.TryDequeue(out req))
            {
                GD.reqlogger.Info(string.Format("IP-{0} ; FunID-{1} ; MsgID-{2} ; Req-{3}", 
                            req.clientinfo.ClientIP, req.FunctionID, req.MsgID, req.jsonreq));
                switch (req.FunctionID)
                {
                    case GD.constAddSubRec:
                        try
                        {
                            rec = JsonConvert.DeserializeObject<UserSubRec>(req.jsonreq);
                        }catch(Exception ex){
                            WriteExpResponse(req, GD.constExpJsonCode, ex.Message, ex);
                            rec = null;
                        }
                        string addstr = "insert into UserSubRec (Channel, ID, Range, Symbol, Type, Condition, Value, Times, " +
                        "Duration, UpdateTime) values (@Channel, @ID, @Range, @Symbol, @Type, @Condition, @Value, @Times, @Duration, @UpdateTime);SELECT CAST(scope_identity() AS int)";
                        if (GD.SubAppDbTool == null || rec == null) return;
                        conn = GD.SubAppDbTool.GetDbConnection();
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        try
                        {
                            conn.Open();
                            //cmd.Parameters.Clear();
                            cmd.CommandText = addstr;
                            cmd.Parameters.AddWithValue("@Channel", (object)req.Channel ?? DBNull.Value);
                            /*if (!string.IsNullOrEmpty(rec.Channel))
                                cmd.Parameters.AddWithValue("@Channel", rec.Channel);
                            else
                                cmd.Parameters.AddWithValue("@Channel", DBNull.Value);*/
                            if (!string.IsNullOrEmpty(rec.ID))
                                cmd.Parameters.AddWithValue("@ID", rec.ID);
                            else
                                cmd.Parameters.AddWithValue("@ID", DBNull.Value);
                            if (!string.IsNullOrEmpty(rec.Range))
                                cmd.Parameters.AddWithValue("@Range", rec.Range);
                            else
                                cmd.Parameters.AddWithValue("@Range", DBNull.Value);
                            if (!string.IsNullOrEmpty(rec.Symbol))
                                cmd.Parameters.AddWithValue("@Symbol", rec.Symbol);
                            else
                                cmd.Parameters.AddWithValue("@Symbol", DBNull.Value);
                            if (!string.IsNullOrEmpty(rec.Type))
                                cmd.Parameters.AddWithValue("@Type", rec.Type);
                            else
                                cmd.Parameters.AddWithValue("@Type", DBNull.Value);
                            if (!string.IsNullOrEmpty(rec.Condition))
                                cmd.Parameters.AddWithValue("@Condition", rec.Condition);
                            else
                                cmd.Parameters.AddWithValue("@Condition", DBNull.Value);
                            if (!string.IsNullOrEmpty(rec.Value))
                                cmd.Parameters.AddWithValue("@Value", rec.Value);
                            else
                                cmd.Parameters.AddWithValue("@Value", DBNull.Value);
                            if (!string.IsNullOrEmpty(rec.Times))
                                cmd.Parameters.AddWithValue("@Times", rec.Times);
                            else
                                cmd.Parameters.AddWithValue("@Times", DBNull.Value);
                            if (!string.IsNullOrEmpty(rec.Duration))
                                cmd.Parameters.AddWithValue("@Duration", rec.Duration);
                            else
                                cmd.Parameters.AddWithValue("@Duration", DBNull.Value);
                            if (!string.IsNullOrEmpty(req.DateTime))
                                cmd.Parameters.AddWithValue("@UpdateTime", req.DateTime);
                            else
                                cmd.Parameters.AddWithValue("@UpdateTime", DBNull.Value);
                            //cmd.ExecuteNonQuery();
                            rec.NotifyMsgID = ((int)cmd.ExecuteScalar()).ToString().Trim();
                            WriteResponse(req, GD.constOKCode, "", rec);
                            
                            //ResAddSubRec resadd = new ResAddSubRec();
                            //resadd.ResultCode = "0000";
                            //resadd.ResultMessage = "";
                            //resadd.NotifyMsgID = rec.Serial;
                            //string resbody = JsonConvert.SerializeObject(resadd);
                            //byte[] resmsg = ResponseBytes(req.FunctionID, DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"), req.MsgID, resbody);
                            //NetworkStream ns = TCPConnection.GetStream();
                            //if (ns.CanWrite)
                            //{
                            //    ns.Write(resmsg, 0, resmsg.Length);
                            //}
                            //cmd.Parameters.Clear();
                        }
                        catch (SqlException sqlex)
                        {
                            WriteExpResponse(req, GD.constExpSqlCode, sqlex.Message, sqlex);
                            //ResAddSubRec resadd = new ResAddSubRec();
                            //resadd.ResultCode = GD.constExceptionSqlCode;
                            //resadd.ResultMessage = sqlex.Message;
                            //resadd.NotifyMsgID = "";
                            //string resbody = JsonConvert.SerializeObject(resadd);
                            //byte[] resmsg = ResponseBytes(req.FunctionID, DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"), req.MsgID, resbody);
                            //NetworkStream ns = TCPConnection.GetStream();
                            //if (ns.CanWrite)
                            //{
                            //    ns.Write(resmsg, 0, resmsg.Length);
                            //}
                        }
                        catch (SocketException socketex)
                        {
                            WriteExpResponse(req, GD.constExpSocketCode, socketex.Message, socketex);
                            //ResAddSubRec resadd = new ResAddSubRec();
                            //resadd.ResultCode = GD.constExceptionSqlCode;
                            //resadd.ResultMessage = socketex.Message;
                            //resadd.NotifyMsgID = "";
                            //string resbody = JsonConvert.SerializeObject(resadd);
                            //byte[] resmsg = ResponseBytes(req.FunctionID, DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"), req.MsgID, resbody);
                            //NetworkStream ns = TCPConnection.GetStream();
                            //if (ns.CanWrite)
                            //{
                            //    ns.Write(resmsg, 0, resmsg.Length);
                            //}
                        }
                        catch (Exception ex)
                        {
                            WriteExpResponse(req, GD.constExpDefalutCode, ex.Message, ex);
                            //ResAddSubRec resadd = new ResAddSubRec();
                            //resadd.ResultCode = GD.constExceptionSqlCode;
                            //resadd.ResultMessage = ex.Message;
                            //resadd.NotifyMsgID = "";
                            //string resbody = JsonConvert.SerializeObject(resadd);
                            //byte[] resmsg = ResponseBytes(req.FunctionID, DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"), req.MsgID, resbody);
                            //NetworkStream ns = TCPConnection.GetStream();
                            //if (ns.CanWrite)
                            //{
                            //    ns.Write(resmsg, 0, resmsg.Length);
                            //}
                        }
                        finally
                        {
                            if (cmd != null)
                                cmd.Dispose();
                            conn.Close();
                        }
                        
                        break;
                    case GD.constDelSubRec:
                        try
                        {
                            rec = JsonConvert.DeserializeObject<UserSubRec>(req.jsonreq);
                        }
                        catch (Exception ex)
                        {
                            WriteExpResponse(req, GD.constExpJsonCode, ex.Message,ex);
                            rec = null;
                        }
                        string delstr = "delete from UserSubRec where Channel=@Channel and ID=@ID and Serial=@NotifyMsgID";
                        if (GD.SubAppDbTool == null) return;
                        conn = GD.SubAppDbTool.GetDbConnection();
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        try
                        {
                            conn.Open();
                            //cmd.Parameters.Clear();
                            cmd.CommandText = delstr;
                            if (!string.IsNullOrEmpty(req.Channel))
                                cmd.Parameters.AddWithValue("@Channel", req.Channel);
                            else
                                cmd.Parameters.AddWithValue("@Channel", DBNull.Value);
                            if (!string.IsNullOrEmpty(rec.ID))
                                cmd.Parameters.AddWithValue("@ID", rec.ID);
                            else
                                cmd.Parameters.AddWithValue("@ID", DBNull.Value);
                            if (!string.IsNullOrEmpty(rec.NotifyMsgID))
                                cmd.Parameters.AddWithValue("@NotifyMsgID", rec.NotifyMsgID);
                            else
                                cmd.Parameters.AddWithValue("@NotifyMsgID", DBNull.Value);
                            cmd.ExecuteNonQuery();
                            WriteResponse(req, GD.constOKCode, "", rec);
                        }
                        catch (SqlException sqlex)
                        {
                            WriteExpResponse(req, GD.constExpSqlCode, sqlex.Message, sqlex);
                        }
                        catch (SocketException socketex)
                        {
                            WriteExpResponse(req, GD.constExpSocketCode, socketex.Message, socketex);
                        }
                        catch (Exception ex)
                        {
                            WriteExpResponse(req, GD.constExpDefalutCode, ex.Message, ex);
                        }
                        finally
                        {
                            if (cmd != null)
                                cmd.Dispose();
                            conn.Close();
                        }
                        break;
                    case GD.constQuerySubRec:
                        try
                        {
                            rec = JsonConvert.DeserializeObject<UserSubRec>(req.jsonreq);
                        }
                        catch (Exception ex)
                        {
                            WriteExpResponse(req, GD.constExpJsonCode, ex.Message, ex);
                            rec = null;
                        }
                        string qrystr = "select a.*, b.SymbolName, b.Market, b.TempRefer, b.Refer, b.CouponDate from UserSubRec a, "+
                            "SubStockInfo b where a.Symbol=b.Symbol and b.CouponDate>=@CouponDate and a.ID=@ID " +
                            "and a.Channel=@Channel order by a.Serial desc";
                        if (GD.SubAppDbTool == null) return;
                        conn = GD.SubAppDbTool.GetDbConnection();
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        try
                        {
                            conn.Open();
                            //cmd.Parameters.Clear();
                            cmd.CommandText = qrystr;
                            if (!string.IsNullOrEmpty(req.Channel))
                                cmd.Parameters.AddWithValue("@Channel", req.Channel);
                            else
                                cmd.Parameters.AddWithValue("@Channel", DBNull.Value);
                            if (!string.IsNullOrEmpty(rec.ID))
                                cmd.Parameters.AddWithValue("@ID", rec.ID);
                            else
                                cmd.Parameters.AddWithValue("@ID", DBNull.Value);
                            cmd.Parameters.AddWithValue("@CouponDate", DateTime.Today.ToString("yyyy/MM/dd"));
                            dr = cmd.ExecuteReader();
                            WriteResponse(req, GD.constOKCode, "", rec, dr);
                        }
                        catch (SqlException sqlex)
                        {
                            WriteExpResponse(req, GD.constExpSqlCode, sqlex.Message, sqlex);
                        }
                        catch (SocketException socketex)
                        {
                            WriteExpResponse(req, GD.constExpSocketCode, socketex.Message, socketex);
                        }
                        catch (Exception ex)
                        {
                            WriteExpResponse(req, GD.constExpDefalutCode, ex.Message, ex);
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
                        }
                        break;
                }
            }
            //mrevent.Reset();
           // mrevent.WaitOne();
        }
    }

    public class AppMsgHeader
    {
        public const int HeaderLen = 52;
        public byte[] HeaderData = new byte[HeaderLen];
        public byte[] FunctionID
        {
            get
            {
                byte[] ba = new byte[4];
                Array.Copy(HeaderData, 0, ba, 0, 4);
                return ba;
            }
            set
            {
                Array.Copy(value, 0, HeaderData, 0, 4);
            }
        }
        public byte[] DateTime
        {
            get
            {
                byte[] ba = new byte[21];
                Array.Copy(HeaderData, 4, ba, 0, 21);
                return ba;
            }
            set
            {
                Array.Copy(value, 0, HeaderData, 4, value.Length);
            }
        }
        public byte[] DataLen
        {
            get
            {
                byte[] ba = new byte[5];
                Array.Copy(HeaderData, 25, ba, 0, 5);
                return ba;
            }
            set
            {
                Array.Copy(value, 0, HeaderData, 25, value.Length);
            }
        }
        public int BodyLength
        {
            get
            {
                int ia = 0;
                if (Int32.TryParse(System.Text.Encoding.Default.GetString(DataLen), out ia))
                    return ia;
                else
                    return 0;
            }
        }
        public byte[] MsgID
        {
            get
            {
                byte[] ba = new byte[20];
                Array.Copy(HeaderData, 30, ba, 0, 20);
                return ba;
            }
            set
            {
                Array.Copy(value, 0, HeaderData, 30, value.Length);
            }
        }
        public byte[] Channel
        {
            get
            {
                byte[] ba = new byte[2];
                Array.Copy(HeaderData, 50, ba, 0, 2);
                return ba;
            }
            set
            {
                Array.Copy(value, 0, HeaderData, 50, value.Length);
            }
        }
        public void Clear()
        {
            Array.Clear(HeaderData, 0, HeaderLen);
        }
    }

    public class ClientInfoList<T> : List<T>
    {
        public ClientInfoList()
        {
        }

        public void SafeAdd(T item)
        {
            lock (this)
            {
                Add(item);
            }
        }

        public void SafeRemove(T item)
        {
            lock (this)
            {
                Remove(item);
            }
        }

    }

}
