using Assets.MessageLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.NetService
{
    public class CallBack
    {
        Action<Message> callback;
        Message msg;
        public CallBack(Message msg,Action<Message> incallback)
        {
            this.msg = msg;
            this.callback = incallback;
        }
        public Message getMessage()
        {
            return msg;
        }
        public void Return(Message msg)
        {
			
//			callback.BeginInvoke(msg, new AsyncCallback(asyncRet =>
//            {
//                //doing... 
//                callback.EndInvoke(asyncRet);
//
//            }), null);
			callback.Invoke(msg);
        }
    }
    
}
