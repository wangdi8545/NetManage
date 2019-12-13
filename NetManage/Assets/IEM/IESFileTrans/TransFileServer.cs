using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using Assets.MessageLib;
using Assets.Session;
using Data;
using System.Collections.Generic;

namespace IESFileTrans
{
	public static class TransFileServer
	{
        //@any
//		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
//		static private string fileSavePathThumbnail = @"C:\WSK\IEL\PIC\Server\Thumbnail\";
//		static private string fileSavePathImage = @"C:\WSK\IEL\PIC\Server\Image\";
//#elif UNITY_IPHONE
//		static private string fileSavePath = Application.persistentDataPath + "/FileServerfiles/"; 
//		static private string fileSavePathThumbnail = Application.persistentDataPath + "/FileServerThumbnailfiles/"; 
//#elif UNITY_ANDROID
//		static private string fileSavePathThumbnail = "/sdcard/WSK/IEL/PIC/Thumbnail/";
//		static private string fileSavePathImage = "/sdcard/WSK/IEL/PIC/Image/";
//#endif
        static private string fileSavePathThumbnail;
        static private string fileSavePathImage ;

        static private int FileNetWorkPort = 8225;
		private static Socket serverSocket = null;
		private static bool isServerRunning = false;
		public static Dictionary<string, PictureStatus> ServerPictureDict = new Dictionary<string, PictureStatus> ();
        public static void OnInitPath() {
            fileSavePathThumbnail = IEM_Platform.getinstance.fileSavePathThumbnail;
            fileSavePathImage = IEM_Platform.getinstance.fileSavePathImage;
        }
		public enum PictureStatus
		{
			Done,
			Doing,
			Empty}

		;

		public static bool Init ()
		{
			if (isServerRunning)
				return true;
			try {
				serverSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IPEndPoint endpoint = new IPEndPoint (IPAddress.Any, FileNetWorkPort);
				serverSocket.Bind (endpoint);
				serverSocket.Listen (50);    //设定最多10个排队连接请求
				Debuger.Log ("TransFileServerSocket init Success");
				isServerRunning = true;
				//通过Clientsoket发送数据
				Thread myThread = new Thread (ListenClientConnect);
				myThread.Start ();
			} catch (Exception ex) {
				Debuger.Log ("FileServerSocket Init Error :" + ex.ToString ());
				isServerRunning = false;
				serverSocket = null;
				return false;
			}
			return true;
		}

		public static void Exit ()
		{
			if (!isServerRunning) {
				serverSocket = null;
				return;
			}
			isServerRunning = false;
			serverSocket.Close ();
			serverSocket = null;
		}

		private static void ListenClientConnect ()
		{
			while (isServerRunning && (serverSocket != null)) {
				try {
					Socket clientSocket = serverSocket.Accept ();
					Thread receiveThread = new Thread (Create);
					receiveThread.Start (clientSocket);
				} catch {
					break;
				}
				
			}
		}

		static string getFileDirectoryPath (string sendFileName, string studentNumber)
		{
			string result = string.Empty;
			// Thumbnail
			if (sendFileName.IndexOf ("T") >= 0) {
				result = Path.Combine (fileSavePathThumbnail, studentNumber); 
			} else {
				result = Path.Combine (fileSavePathImage, studentNumber); 
			}
			return result;

		}

		public static bool checkFileSize (long FileSize, string filepath, string fileName)
		{
			FileInfo fileInfo = new FileInfo (filepath);
			if (fileInfo.Length == FileSize)
				return true;
			if (File.Exists (filepath)) {
				File.Delete (filepath);
			}
			return false;
		}

		public static string getStudentNumberByMac (string clientMacAddress)
		{
			StudentBean sb = DataManager.getInstance ().getNowClassStudentNameByMac (clientMacAddress);
			string studentNumber = String.Empty;
			if (sb == null) {
				studentNumber = DataManager.getInstance ().getDeviceIdFromMac (clientMacAddress).ToString ();
			} else {
				studentNumber = sb.getStudentNo ().ToString ();
			}
			return studentNumber;
		}

		public static void CleanOneStudentServerPictureStatus(string clientMacAddress)
		{
			string studentNumber = getStudentNumberByMac (clientMacAddress);
			lock (ServerPictureDict) {
				List<string> keyList = new List<string> (ServerPictureDict.Keys);
				int len = keyList.Count;
				for (int i = 0; i < len; i++) {
					if (keyList [i].StartsWith (studentNumber)) {
//		Debuger.Log ("Remove:" + keyList[i]);
						ServerPictureDict.Remove (keyList [i]);
					}
				
				}
			
			}
		}

		public static void Create (object clientSocket)
		{
			Socket client = clientSocket as Socket;
			//获取学生学号
			string clientIpAddress = ((IPEndPoint)client.RemoteEndPoint).Address.ToString ();
			string clientMacAddress = Teacher.ins.getMacWithIp (clientIpAddress);
			string studentNumber = getStudentNumberByMac (clientMacAddress);
			bool ReceivedResult = true;
			string SendFileName = "";
			int count = -1;
			FileStream MyFileStream = null;
			
			try {
				count = int.Parse (System.Text.Encoding.Unicode.GetString (TransferFiles.ReceiveVarData (client)));
//				Debuger.Log ("num:" + count);
				for (int i = 0; i < count; i++) {
					//获得[文件名]   
					SendFileName = System.Text.Encoding.Unicode.GetString (TransferFiles.ReceiveVarData (client));					
					//获得[包的大小]   
					int bagSize = int.Parse (System.Text.Encoding.Unicode.GetString (TransferFiles.ReceiveVarData (client)));
					//获得[包的总数量]   
					int bagCount = int.Parse (System.Text.Encoding.Unicode.GetString (TransferFiles.ReceiveVarData (client)));
					//获得[最后一个包的大小]   
					int bagLast = int.Parse (System.Text.Encoding.Unicode.GetString (TransferFiles.ReceiveVarData (client)));

					long FileSize = bagSize * bagCount + bagLast;
					string fileSavePathWithNo = getFileDirectoryPath (SendFileName, studentNumber);
					string fullPath = Path.Combine (fileSavePathWithNo, SendFileName);
					if (!Directory.Exists (fileSavePathWithNo)) {
						Directory.CreateDirectory (fileSavePathWithNo);
					}

					bool CanSkip = false;
					lock (ServerPictureDict) {
						if (ServerPictureDict.ContainsKey (studentNumber + SendFileName)) {
							switch (ServerPictureDict [studentNumber + SendFileName]) {
							case PictureStatus.Done:
								CanSkip = true;
								break;
							case PictureStatus.Doing:
								throw new System.Exception ("[FilsTransServer.Create] Other Thread downloading this file." + studentNumber + SendFileName); 
								break;
							case PictureStatus.Empty:
								if (File.Exists (fullPath)) {
									File.Delete (fullPath);
								}
								break;
							default:
								throw new System.Exception ("[FilsTransServer.Create] Fatal switch Error ." + studentNumber + SendFileName); 
								break;
							}
						} else {
							if (File.Exists (fullPath)) {
								if (checkFileSize (FileSize, fullPath, SendFileName)) {
									ServerPictureDict.Add (studentNumber + SendFileName, PictureStatus.Done);
									CanSkip = true;
								} else {
									File.Delete (fullPath);
									ServerPictureDict.Add (studentNumber + SendFileName, PictureStatus.Empty);
								}
							} else {
								ServerPictureDict.Add (studentNumber + SendFileName, PictureStatus.Empty);
							}								
						}
					}
//					Debuger.Log (studentNumber + SendFileName + " | status. Canskip:" + CanSkip);
					if (CanSkip) {
						int SendedCount = 0;
						if (bagLast > 0)
							bagCount++;
						for (SendedCount = 0; SendedCount < bagCount; SendedCount++) {
							byte[] data = TransferFiles.ReceiveVarData (client);
						}
					} else {
						lock (ServerPictureDict) {
							ServerPictureDict [studentNumber + SendFileName] = PictureStatus.Doing;
						}
						try {
							//创建一个新文件
							MyFileStream = null;
							MyFileStream = new FileStream (fullPath, FileMode.Create, FileAccess.Write);
							if (!MyFileStream.CanWrite) {
								throw new System.Exception ("[FilsTransServer.Create] FileStream New Fail."); 
							}
					
							int SendedCount = 0;
							if (bagLast > 0)
								bagCount++;
							for (SendedCount = 0; SendedCount < bagCount; SendedCount++) {
								byte[] data = TransferFiles.ReceiveVarData (client);
								MyFileStream.Write (data, 0, data.Length);
				
							}
				
							//关闭文件流   
							MyFileStream.Close ();
							MyFileStream = null;
							ReceivedResult = checkFileSize (FileSize, fullPath, SendFileName);
							if (ReceivedResult) {
								lock (ServerPictureDict) {
									ServerPictureDict [studentNumber + SendFileName] = PictureStatus.Done;
								}
							} else {
								throw new System.Exception ("[FilsTransServer.Create] checkFileSize Error."); 
							}
						} catch (Exception ex) {
							ReceivedResult = false;
							lock (ServerPictureDict) {
								if (ServerPictureDict.ContainsKey (studentNumber + SendFileName)) {
									ServerPictureDict [studentNumber + SendFileName] = PictureStatus.Empty;
								}
							}
							IEM_Log.ins.Log ("FileServer Write to File Error: " + studentNumber + SendFileName + ex.ToString ());
							Debuger.Log ("FileServer Write to File Error: " + studentNumber + SendFileName + ex.ToString ());
							if (MyFileStream != null) {
								MyFileStream.Close ();
							}

						}
						
					}
				} 
			} catch (Exception ex) {
				ReceivedResult = false;
				IEM_Log.ins.Log ("FileServer IO Error: " + studentNumber + SendFileName + ex.ToString ());
				Debuger.Log ("FileServer IO Error: " + studentNumber + SendFileName + ex.ToString ());
			}
			
			// 这个时候通知说下载完毕, 通过消息队列发送给UI线程 
			
			JSONObject finishMessageJson = new JSONObject ();
			finishMessageJson.SetField ("MacAddress", clientMacAddress);
			finishMessageJson.SetField ("result", ReceivedResult);

			if (count == 0 || (!SendFileName.Equals (String.Empty))) {
				if (count == 0 || SendFileName.IndexOf ("T") >= 0) {
					MessageManager.addRecrveMessage (MessageTools.createReturnMessage (clientIpAddress + "_T", finishMessageJson, null), 0);
				} else {
					finishMessageJson.SetField ("name", SendFileName);
					MessageManager.addRecrveMessage (MessageTools.createReturnMessage (clientIpAddress + "_" + SendFileName, finishMessageJson, null), 0);
				}
			}
			//关闭套接字   
			client.Close ();
		}
	}
}

