using System.Collections;
using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Assets.MessageLib;
using Assets.NimoNetwork;
using Assets.NetService;
using System.Collections.Generic;
using Assets.Session;

public class Server
{
	/// <summary>
	/// server_branch
	/// </summary>
	public static Server ins = new Server ();

	const int MAX_DEVICENUMBER = 105;

	TcpListener tcpLister = null;
	Thread Thread_Start, Thread_Receive;
	int port = NetSDKManager.port;

	public bool isServerRunning = false;

	public void StartServer ()
	{	
		IEM_Log.ins.Log ("开启服务器 [Server.StartServer]");
		isServerRunning = true;
		// 开线程启动TCPListen服务器
		Thread_Start = new Thread (ServerDeamon);
		Thread_Start.IsBackground = true;
		Thread_Start.Start ();

		Thread_Receive = new Thread (ServerReceive);
		Thread_Receive.IsBackground = true;
		Thread_Receive.Start ();

	}

	void ServerReceive ()
	{
		// 处理接受信息的单线程
		while (isServerRunning) {
			try {
				List<StudentSession> studentSessions = SessionManager.getStumapStudentSessions ();
				int len = studentSessions.Count;
				for (int i = 0; i < len; i++) {
					StudentSession ss = studentSessions [i];

					if (ss.checkTimeOut ()) {
						if (ss.IsFirstLogout) {
//							Debuger.Log ("[Server.ServerReceive] Shutdown: " + ss.DeviceNumber);
//							IEM_Log.ins.Log ("[Server.saveStudentSession] 学生断线: " + ss.DeviceNumber);
							Teacher.ins.OnDisconnect (ss);

							ss.IsFirstLogout = false;
						}
					} else {
						ss.readDataToMessageQueue ();
					}
				}
				Thread.Sleep (10);
			} catch (Exception e) {
				Debuger.Log ("[Server:ServerReceive] Error" + e.ToString ());
			}
		}
	}

	void ServerDeamon ()
	{
		while (isServerRunning) {
			try {
				// 启动TCPListen服务器
				Debuger.Log ("[Server.ServerDeamon] Listen for student connect..." + " ip:" + IPAddress.Any + "port:" + port);
				tcpLister = new TcpListener (IPAddress.Any, port);
				tcpLister.Start ();
				NoUnityMessageManager.getIns ().StartThread ();
				while (isServerRunning) {
					try {
						TcpClient client = tcpLister.AcceptTcpClient ();
						// 这里会发生阻塞
						saveStudentSession (client);
					} catch {
					}
				}
			} catch (Exception ex) {
				Debuger.Log ("[Server.ServerDeamon]" + ex.ToString ());
			} finally {
				tcpLister.Stop ();
				tcpLister = null;
				NoUnityMessageManager.getIns ().StopThread ();
			}
		}
	}

	void saveStudentSession (TcpClient client)
	{
		string ip = ((IPEndPoint)(client.Client.RemoteEndPoint)).Address.ToString ();
		//StudentSession ss = SessionManager.createOrGetSession (ip, client) as StudentSession;
		Debuger.Log ("[Server.saveStudentSession] Student come in: " + ip);
		IEM_Log.ins.Log ("[Server.saveStudentSession] 学生连入: " + ip);
		//Teacher.ins.OnConnect (ss);
		SessionManager.createOrGetSession (ip, client);
	}

	public void StopServer ()
	{
		try {
			Debuger.Log ("[Server.StopServer] Kill Backgroud Thread");
			if (tcpLister != null) {
				tcpLister.Stop ();
			}
			isServerRunning = false;
			Thread_Start.Abort ();
			Thread_Receive.Abort ();
			NoUnityMessageManager.getIns ().StopThread ();
			//GC.Collect ();
		} catch (Exception ex) {
			Debuger.Log ("[Server.StopServer Error]" + ex.ToString ());
		}
	}
}
