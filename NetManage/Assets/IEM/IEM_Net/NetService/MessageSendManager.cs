using Assets.MessageLib;
using Assets.NetSession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Assets.IEM.IEM_Net.NetService
{
    public class MessageSendManager
    {
        static MessageSendManager instance = new MessageSendManager();
        public static void init()
        {
            Thread clientThread = new Thread(new ParameterizedThreadStart(instance.run));
            clientThread.Start(null);
        }

        public void run(Object o)
        {
            while (true)
            {
                Message msg = MessageManager.getSendMessage();
                try {
                    while (msg != null)
                    {
                        //Debuger.Log("1发送次数：" + (count));
                        msg.session.sendToNet(msg);
                        //Debuger.Log("2发送次数："+(count++));
                        //if (((Session)(msg.session)).IsConnect) {
                        //}
                        //else
                        //{
                        //    MessageManager.putSendMessage(msg);
                        //}
                        msg = MessageManager.getSendMessage();
                    }
                }catch(Exception ex)
                {
                    Debuger.LogError("[MessageSendManager.run]"+ex.ToString());
                    //无需再次发送，直接丢弃，重发机制改到Callback中处理
                    //MessageManager.putSendMessage(msg);
                    //LableString.addtolable("" + ex.ToString());
                }
                Thread.Sleep(5);
            }
        }
    }
}
