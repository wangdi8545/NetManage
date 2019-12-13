using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.NetSession;

namespace Assets.MessageLib
{
	public class MessageManager
	{
		private static MessageQueue recrveMq = new MessageQueue ();
		private static MessageQueue sendMq = new MessageQueue ();
		private static MessageQueue receivedMqNoUnity = new MessageQueue ();

		public static void init ()
		{
			
		}

		public static bool addRecrveMessage (Message msg, int type)
		{
			/***
			 * if type is 0, means this message will hanlde by Unity Thread, maybe update the ui,
			 * and if type is 1, means this message target to C Sharp Thread.
			 */
			if (type == 0) {
				bool b = recrveMq.AddMessage (msg);
				////LableString.addtolable("消息队列增加后长度："+recrveMq.GetCount());
				return b;
			} else {
				return receivedMqNoUnity.AddMessage (msg);
			}
		}

		public static Message getRecrveMessage (int type)
		{
			if (type == 0) {
				if (recrveMq.GetCount () <= 0)
					return null;
				return recrveMq.GetMessage ();
			} else {
				if (receivedMqNoUnity.GetCount () <= 0)
					return null;
				return receivedMqNoUnity.GetMessage ();
			}
		}

		public static void putRecrveMessage (Message msg)
		{
			recrveMq.AddMessage (msg);
		}

		public static Message getSendMessage ()
		{
			if (sendMq.GetCount () <= 0)
				return null;
			return sendMq.GetMessage ();
		}

		public static void putSendMessage (Message msg)
		{
			sendMq.AddMessage (msg);
		}

		public static int getRecrveMessageCount ()
		{
			return recrveMq.GetCount ();
		}

		public static void clearSendMessage()
		{
			sendMq.clearMessageQueue ();
		}

		public static void clearReceivedMessage()
		{
			recrveMq.clearMessageQueue ();	
			receivedMqNoUnity.clearMessageQueue ();
		}

        internal static void clearSendMessage(NetSession.Session session)
        {
            sendMq.removeMessageQueue(session);
        }

        internal static void clearReceivedMessage(NetSession.Session session)
        {
            recrveMq.removeMessageQueue(session);
            receivedMqNoUnity.removeMessageQueue(session);
        }
    }
}
