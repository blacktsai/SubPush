using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubPush
{
    public class ReqHandler
    {
        public ClientInfo clientinfo;
        public string FunctionID;
        public string DateTime;
        public int DataLen;
        public string MsgID;
        public string jsonreq;
        
        public ReqHandler()
        {
        }

        public void ThreadTask(UserReq reqobj)
        {
            switch (int.Parse(reqobj.FunctionID))
            {
                case 6001 :
                    break;
            }
        }
    }
}
