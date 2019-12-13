using System.Collections;
using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Assets.MessageLib;
using Assets.NimoNetwork;
using Assets.NetService;
using Assets.TestMessage;

public class Client
{
	public static Client ins = new Client ();

	Thread Thread_Start;
	TeacherSession teacherSession = null;
	bool isClientRunning = false;

	bool tryConnectOnce (TcpClient client)
	{
		try {
			Debuger.Log ("[Client.tryConnectOnce] Try to Connect");
			client.Connect (new IPEndPoint (IPAddress.Parse (NetSDKManager.ip), NetSDKManager.port));
		} catch (Exception ex) {
			Debuger.Log ("[Client.tryConnectOnce] Error :" + ex.ToString ());
			return false;
		}
		return true;
	}

	void ConnectToServer (TcpClient client)
	{
		bool isConnected = false;
		while (!isConnected) {
			isConnected = tryConnectOnce (client);

			if (isConnected == false) {
				Debuger.Log ("[Client.ConnectToServer] Connect failed ");
				IEM_Log.ins.Log ("[Client.tryConnectOnce] 学生连接失败");
				Thread.Sleep (500);
			}
		}
	}

	public void StartTheClient ()
	{
		IEM_Log.ins.Log ("[Client.StartTheClient] 学生启动");
		isClientRunning = true;
		Thread_Start = new Thread (ClientDeamon);
		Thread_Start.IsBackground = true;
		Thread_Start.Start ();
	}

	public void StopTheClient ()
	{
		IEM_Log.ins.Log ("[Client.StopTheClient] 学生关闭");
		NoUnityMessageManager.getIns().StopThread();
		isClientRunning = false;
		Thread_Start.Abort ();
	}

	void SendFirstToServer (TeacherSession teacherSession)
	{
		JSONObject js = new JSONObject ();
		string address = string.Empty;
		try {
			address = IEM_Tools.getMacAddress();
			js.SetField ("mac", address);
			js.SetField("type", 1);
		} catch (Exception e) {
			Debuger.Log ("get MacAddress Error" + e.ToString ());
		}
		Debuger.Log ("mac:" + js.ToString ());
		teacherSession.send (Protocol._FirstConnectCall, js);
	}

	void ClientDeamon ()
	{
		Debuger.Log ("----conect");
		while (isClientRunning) {
			try {
				Debuger.Log ("[Client.ClientDeamon] Prepare To connect");
				TcpClient client = new TcpClient ();
				ConnectToServer (client);
				teacherSession = SessionManager.createOrGetTeacherSession (client) as TeacherSession;
				Student.ins.OnConnect (teacherSession);
				Debuger.Log ("[Client.ClientDeamon] Connect Success ");
				IEM_Log.ins.Log ("[Client.ClientDeamon] 学生连接成功");
				SendFirstToServer (teacherSession);
				NoUnityMessageManager.getIns().StartThread();
				int count = 0;
				while (teacherSession.IsConnect && isClientRunning) {
					if (count > 200) {
						teacherSession.checkIsSendPulse ();
						if (teacherSession.checkTimeOut ())
							break;                       
						count = 0;
					} else {
						count++;
					}
					teacherSession.readDataToMessageQueue ();
					Thread.Sleep (5);
				}
				Debuger.Log ("[Client.ClientDeamon] . Disconnect with Server !");
				Student.ins.OnDisConnect (teacherSession);
				NoUnityMessageManager.getIns().StopThread();
			} catch (Exception e) {
				Debuger.Log ("[Client.ClientDeamon] Error " + e.ToString ());
			}
		}
	}
}