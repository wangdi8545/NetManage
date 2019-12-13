using Assets.MessageLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Session
{
    class MessageTools
    {
        private static int nowNumber = 0;
        public static Message createCallMessage(String command, JSONObject data,ISession session)
        {

            Message msg = new Message();
			msg.parameter = new JSONObject(getDataString(data));
            msg.mid = createMid();
            msg.command = command;
            msg.type = Message.Call;
            msg.session = session;
            msg.create();
            return msg;
        }
        public static Message createReturnMessage(String mid ,JSONObject data, ISession session)
        {
            Message msg = new Message();
			msg.parameter = new JSONObject(getDataString(data));
            msg.mid = mid;
            msg.type = Message.Return;
            msg.session = session;
            msg.create();
            return msg;
        }
        //public static Message createServiceletMessage(String command, JSONObject data, ISession session)
        //{
        //    Message msg = new Message();
		//    msg.parameter = new JSONObject(getDataString(data));
        //    msg.command = command;
        //    msg.type = Message.Send;
        //    msg.session = session;
        //    msg.create();
        //    return msg;
        //}
        public static Message createServiceletMessage(String command, JSONObject data, ISession session)
        {
            Message msg = new Message();
			msg.parameter = new JSONObject(getDataString(data));
            msg.command = command;
            msg.type = Message.Send;
            msg.session = session;
            msg.create();
            return msg;
        }
        //事件消息
        public static Message createActionMessage(String command, JSONObject data, ISession session)
        {
            Message msg = new Message();
			msg.parameter = new JSONObject(getDataString(data));
            msg.command = command;
            msg.type = Message._Action;
            msg.session = session;
            msg.create();
            return msg;
        }

		public static Message createFakeMessageOnlyMid (string mid)
		{
			Message msg = new Message();
			msg.mid = mid;
			return msg;
		}

        private static Object lockObj = new Object();
        private static string createMid()
        {
            lock (lockObj)
            {
                ++nowNumber;
                return nowNumber.ToString();
            }

        }

		private static string getDataString(JSONObject data)
		{
			string temp = "";
			try {
				temp = data.ToString();
			} catch(Exception)
			{
				temp = "";
			}
			return temp;
		}
    }
}
