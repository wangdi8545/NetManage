using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.NetService;
using System.Net.Sockets;
using Assets.MessageLib;
using Assets.TestMessage;
using System.Net;
using Assets.Session;
using Data;
using Nimo.Tools;

namespace Assets.NetSession
{
	public abstract class Session : ISession
	{
		protected TcpClient client = null;

		protected const long timeOutDelay = 5000;
		protected long lastCommunicationTime;
		protected bool isConnect;
		protected byte[] data;
		protected int datalength;
		protected int messagelength;
        private const int bufferLength = 1024 * 1024;
        private const int tempBufferLength = 1024 * 100;
		private short communicationFrequency = 0;
        string ip = "";
		string macAddress = string.Empty;
		private CheckFrequencyFilter _checkFrequencyFilter = new CheckFrequencyFilter (1000, 10);

		public Session (TcpClient client)
		{
			setSession (client);
            

        }

		public string getMacAddress ()
		{
			return macAddress;
		}


		public void setMacAddress (string address)
		{
			macAddress = address;
			Debuger.Log ("Mac Set to :" + address);
		}

        private string getIpAddresslocal()
        {
            return ((IPEndPoint)(client.Client.RemoteEndPoint)).Address.ToString();
        }
        public string getIpAddress()
        {
            return ip;
        }

        public void setSession (TcpClient client)
		{
			isConnect = true;
			lastCommunicationTime = System.DateTime.Now.Ticks / 10000;
			data = new byte[bufferLength];
			datalength = 0;
			messagelength = 0;
			communicationFrequency = 0;
			this.client = client;
			ip = getIpAddresslocal();
            if (NetSDKManager.RECONNECT_DATA_RESEND)
            {
                CallBackManager.addMessageToQueueBySession(this);
            }
            else
            {
                CallBackManager.clearCallBackBySession(this);
            }
            if (NetSDKManager.RECONNECT_CLEAN_MESSAGE_QUEUE)
            {
                if (this.isTeacher())
                {
                    MessageManager.clearSendMessage();
                    MessageManager.clearReceivedMessage();
                }else
                {
                    MessageManager.clearSendMessage(this);
                    MessageManager.clearReceivedMessage(this);
                }
            }
		}

//		//This method is expired. 
//		private bool checkSendFrequency()
//		{
//			if (System.DateTime.Now.Ticks / 10000 - lastCommunicationTime < 10) {
//				communicationFrequency++;
//				if (communicationFrequency > 30) {
//					if (communicationFrequency==30||(communicationFrequency - 40) % 10 == 0) {
//						Debuger.Log ("[Session.checkSendFrequency] Send Frequency too busy.[communicationFrequency > 30(" + communicationFrequency + ")]");
//					}
//				}else
//				if (communicationFrequency > 15) {
//					Debuger.Log ("[Session.checkSendFrequency] Send Frequency too busy.[communicationFrequency > 15]");
//				}else
//				if (communicationFrequency > 5) {
//					Debuger.Log ("[Session.checkSendFrequency] Send Frequency too busy.[communicationFrequency > 5]");
//				}
//			} else {
//				communicationFrequency = 0;
//			}
//		}

		public void sendCall (string command, JSONObject data, Action<Message> incallback)
		{
			Message msg = MessageTools.createCallMessage (command, data, this);
//			if(!command.Equals("Pulse")){
//				Debuger.Log ("发送的消息是："+msg.parameter.ToString()+","+msg.mid);
//			}
			if (incallback != null) {
				CallBackManager.putCallBack (new CallBack (msg, incallback));
			}
			sendToMq (msg);
		}

		public void send (string command, JSONObject data)
		{
			//Message msg = MessageTools.createCallMessage(command, data, this);
            Message msg = MessageTools.createServiceletMessage(command, data, this);
            sendToMq (msg);
		}

		public void sendErrorReturn (string mid, Exception ex)
		{
			JSONObject js = new JSONObject ();
			js.AddField ("Exception", ex.ToString ());
			Message msg = MessageTools.createReturnMessage (mid, js, this);
			sendToMq (msg);
		}

		public void sendReturn (string mid, JSONObject data)
		{
			Message msg = MessageTools.createReturnMessage (mid, data, this);
			sendToMq (msg);

//			if (!data.GetField("NetMessageCommand").str.Equals ("Pulse")) {
//				//LableString.addtolable ("返回的消息ID是：" + msg.mid+","+data.GetField("NetMessageCommand")+","+(data.GetField("NetMessageCommand").str.Equals ("Pulse")));
//			}
		}
        public void sendActionWithMyself(string mid, JSONObject data , ISession _ISession)
        {
            Message msg = MessageTools.createActionMessage(mid, data, _ISession);
            MessageManager.addRecrveMessage(msg, 1);
        }

        private void sendToMq (Message msg)
		{
			MessageManager.putSendMessage (msg);
		}

		public void sendToNet (Message msg)
        {
			_checkFrequencyFilter.check();
			if (!isConnect) {
				return;
			}
            try
            {           
            //Debuger.Log("1发送了字节数：" );
            byte[] package  = msg.toBytes ();
//            if(package.Length>500)
//                Debuger.Log("2发送了字节数：" + package.Length);
            //client.GetStream ().Write (b, 0, b.Length);

            const int bufferSize = 1024;

            int bytesSent = 0;
            int bytesLeft = package.Length;
            while (bytesLeft > 0)
            {
                int nextPacketSize = (bytesLeft > bufferSize) ? bufferSize : bytesLeft;
                client.GetStream().Write(package, bytesSent, nextPacketSize);
                bytesSent += nextPacketSize;
                bytesLeft -= nextPacketSize;
            }
            client.GetStream().Flush();


//            if (package.Length > 500)
//                Debuger.Log("3发送了字节数：" + package.Length);
            }
            catch (Exception ex)
            {
                isConnect = false;
                Debuger.Log("[Session.SendToNet] Error: " + ex.ToString());
            }
        }


		public TcpClient getTcpClient ()
		{
			return client;
		}

		public abstract bool isTeacher ();

		public bool checkTimeOut ()
		{
            if (!isConnect) return true;
			if (System.DateTime.Now.Ticks / 10000 - lastCommunicationTime > timeOutDelay) {
				isConnect = false;
                return true;
			}
			return false;
		}

		public void readDataToMessageQueue ()
		{
		    try
		    {
                if (!isConnect) return;
                NetworkStream netStream = client.GetStream();
                int count = 10;
                while (netStream.DataAvailable && count > 0)
                {
                    lastCommunicationTime = System.DateTime.Now.Ticks / 10000;

                    byte[] commonBuffer = new byte[tempBufferLength];
                    int bufferLength = netStream.Read(commonBuffer, 0, tempBufferLength);
                    addSessionData(commonBuffer, bufferLength);
                    count--;
                }
            }
		    catch (Exception)
		    {
		        
		        throw;
		    }
		}

		private void addSessionData (byte[] buffer, int len)
		{
            
			if (datalength + len > bufferLength) {
                Debuger.LogError("[Session.addSessionData] 数据包长度超长，超过了640K：" + (datalength + len));
                //LableString.addtolable("数据包长度超长，超过了640K：" + (datalength + len));
			} else {
				Array.Copy (buffer, 0, data, datalength, len);
				datalength += len;
			}
            string uRstr = "1";
            while (uRstr!=null)
            {
                uRstr = createPackage();
                if (uRstr != null)
                {
                    addToMessageQueue(uRstr);
                    if (uRstr.Length > 100)
                    {
                        //LableString.addtolable("收到数据：" + uRstr.Length + "，增加到消息队列成功失败：" + b);
                    }
                }
            }

        }
        private string createPackage()
        {
            if (datalength < 4)
            {
                return null;
            }
            if (messagelength == 0)
            {
                messagelength = BitConverter.ToInt32(data, 0);
                ////LableString.addtolable("数据长度：" + messagelength);
            }
            //暂时不处理出现收到数据包长度为零的情况
            if (datalength < 4 + messagelength)
            {
                return null;
            }

            byte[] message = new byte[messagelength];
            Array.Copy(data, 4, message, 0, messagelength);
            string uRstr = Encoding.UTF8.GetString(message);
            
            
            messagelength += 4;

            arrayMove(data, messagelength,datalength);
            datalength -= messagelength;

            messagelength = 0;

            return uRstr;
        }
        private void arrayMove(byte[] arr,int index,int maxdatalength)
        {
            for (int i = index; i < maxdatalength; i++)
                data[i - index] = data[i];
        }
		// 添加失败异常应该怎么处理 TODO
		private bool addToMessageQueue (string uRstr)
		{
			JSONObject ReceivedJson = null;
//			Debuger.Log ("##" + uRstr);
			int type = 0;
			try {
                ReceivedJson = new JSONObject(uRstr);
                type = (int)ReceivedJson.GetField("type").i;
			}
			catch {
				type = 0;
			}
				
			return MessageManager.addRecrveMessage (new Message (ReceivedJson, this), type);
			
		}

		public void sendPulse ()
		{
			JSONObject js = new JSONObject ();
			js.SetField ("type", 1);
			try {
			    this.sendCall (Protocol._PULSE, js, backPulse);
			}
			catch(Exception e) {
				Debuger.Log ("[Session.sendPulse]" + e.ToString());
				//LableString.addtolable ("发送失败 " + e.ToString());
			}
//			Debuger.Log ("Pulse Send");
		}

		public void backPulse (Message msg)
		{			
//            if (msg.parameter.GetField("flag").i==1)
//            {
//                //服务器认为我的连接已经断开了，需要重新告诉上层进行登录操作,是否会存在重复登录问题？
//                if (((Assets.NetSession.Session)(msg.session)).IsConnect) { 
//                    NetSDKManager.OnConnect(msg.session);
//                }
//            }
//			Debuger.Log ("[Pulse] Callback");
		}

		public void checkIsSendPulse ()
		{
			if ((System.DateTime.Now.Ticks / 10000 - lastCommunicationTime > 1000)) {
				this.sendPulse ();
//				Debuger.Log ("[Session.checkIsSendPulse] Send Pulse");				
			}
		}

		public bool IsConnect {
			get {
				return isConnect;
			}
			
		}
	}
}
