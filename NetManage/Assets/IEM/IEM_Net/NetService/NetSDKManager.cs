using Assets.IEM.IEM_Net.NetService;
using Assets.MessageLib;
using Assets.NimoNetwork;
using Assets.Session;
using Assets.TestMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IESFileTrans;
using Data;

namespace Assets.NetService
{

	public class NetSDKManager
	{
		/**
            1、调用NetSDKManager.init()方法进行初始化
                学生机（客户端）：NetSDKManager.init(NetSDKManager.StartClient)
                教师机（服务器）：NetSDKManager.init(NetSDKManager.StartServer)

            2、Unity的Update时或者帧停时调用NetSDKManager.update()

            3、发送消息时调用方法
                NetSDKManager.SendToStudent("自定义服务名称",JSONObject 参数, 学生标识,callback)
                NetSDKManager.SendToTeacher("自定义服务名称",JSONObject 参数,callback)
                NetSDKManager.SendToStudentAll("自定义服务名称",JSONObject 参数,callback)

            方法调用成功后会自动调用callback方法，回传String类型返回值

            4、注册服务类
                1、定义服务类，实现ServiceCall接口，在access方法中处理接收到的参数，处理完毕后返回JSONObject对象作为返回值、
                2、注册服务类，NetSDKManager.regist(new 上面定义的服务类对象);

            5、无返回服务注册
                1、定义服务类，实现Servicelet接口，在access方法中处理接收到的参数
                2、注册服务类，NetSDKManager.regist("自定义服务名称",new 上面定义的服务类对象);
            6、清除网络通信
                   程序退出时。
                   NetSDKManager.destroyNetworkThread();

			7、建议在Camera等不会销毁的地方挂载OutLog.cs脚本，用来将Log文件写入到手机SD卡内部。
			

			8、 获取学生截图缩略图GetThumbnailList， 获取学生截图原图GetImage
			建议：
                注册服务的时候服务名称采用如下规则（可以不使用，没有限定）：
                S_CALL_GETNOWFLAG :S表示对Student学生的调用，CALL表示有返回，GETNOWFLAG表示要执行的动作
                T_CALL_GETNOW_STUDENTNUMBER:T表示对教师机的调用，CALL表示有返回，GETNOW_STUDENTNUMBER表示要执行的动作
                

            例子：参照SendTest和testServiceCall
         * 
         */

		/**
		* 连接资源服务器的IP地址:  用于http://sdk.wizsch.com/queryByMac/macAddress
		* 请务必保证服务器的Mac地址有效
         */
		public static string LoadResourceIP = "sdk.wizsch.com";
		public static bool isloadResrouceFromURL = false;
		//public const string ip = "192.168.1.96";
		public static string ip = "127.0.0.1";
		//客户端重连或者客户端关闭重开后，未处理的请求是否重新发送
		public readonly static bool RECONNECT_DATA_RESEND = false;
		public readonly static bool RECONNECT_CLEAN_MESSAGE_QUEUE = true;
		/**
         * 连接服务器的端口号
         */
		public const int port = 8083;

		/**
         * 启动服务器标识
         */
		public const int StartServer = 1;
		/**
         * 启动客户端标识
         */
		public const int StartClient = 2;

		static int serviceType = 0;

		/**
         * 初始化方法
         * 学生机带入参数：StartClient
         * 教师机带入参数：StartServer
         */
		public static void init (int startType)
		{
//            GetIpFormXml();	
			IEM_Tools.init ();
			NoUnityMessageManager.getIns ().init ();

			serviceType = startType;
			switch (startType) {
			case StartServer:
                    //启动服务器
				DataManager.getInstance ().load (XMLInterface.ins().GridName, XMLInterface.ins().ClassName);
				ServiceRegistManager.getInstance ().initTeacherServiceRegist ();
				Teacher.ins.Start ();
				TransFileServer.Init ();

				break;
			case StartClient:
                    //启动客户端
				ServiceRegistManager.getInstance ().initStudentServiceRegist ();
				Student.ins.Start ();
				break;
			}
			SessionManager.init ();
			MessageManager.init ();
			ServiceManager.init ();
			MessageSendManager.init ();
		}

		/**
         * Unity主线程运行，每帧停调用或者update调用
         */
		public static void update ()
		{
			SessionManager.update ();
		}

		/**
         * 调用某学生的ServiceCall服务，返回后执行incallback
         */
		public static void SendToStudentCall (string command, JSONObject msg, string stuKey, Action<Message> incallback)
		{
			SessionManager.SendToStudentCall (command, msg, stuKey, incallback);
		}

		/**
         * 调用教师机的ServiceCall服务，返回后执行incallback
         */
		public static void SendToTeacherCall (string command, JSONObject msg, Action<Message> incallback)
		{
			SessionManager.SendToTeacherCall (command, msg, incallback);
		}

		/**
         * 调用所有学生机的ServiceCall服务，返回后执行incallback
         */
		public static void SendToAllStudentCall (string command, JSONObject msg, Action<Message> incallback)
		{
			SessionManager.SendToAllStudentCall (command, msg, incallback);
		}

		/**
         * 调用某学生的ServiceLet服务无返回值
         */
		public static void SendToStudentLet (string command, JSONObject msg, string stuKey)
		{
			SessionManager.SendToStudentLet (command, msg, stuKey);
		}

		/**
         * 调用教师机的ServiceLet服务无返回值
         */
		public static void SendToTeacherLet (string command, JSONObject msg)
		{
			SessionManager.SendToTeacherLet (command, msg);
		}

		/**
         * 调用所有学生机的ServiceLet服务无返回值
         */
		public static void SendToAllStudentLet (string command, JSONObject msg)
		{
			SessionManager.SendToAllStudentLet (command, msg);
		}

		/**
         * 有返回的服务注册
         */
		public static bool registCall (String command, ServiceCall obj)
		{
			return ServiceManager.registCall (command, obj);
		}

		/**
         * 销毁Server.cs 或者 Client.cs的线程
         */
		public static void destroyNetworkThread ()
		{
			switch (serviceType) {
			case StartServer:
				//启动服务器
				Server.ins.StopServer ();
				TransFileServer.Exit ();
				break;
			case StartClient:
				//启动客户端
				Client.ins.StopTheClient ();
				break;
			}
		}

        /**
         * 开启或者关闭 Debuger 唯一接口！！！！！@any 使用者自己控制log的启动。EnableWriteToExternalTxt是写入txt。EnableLog是否启动Unity的log
         */
        public static void SetDebugerStatus (bool debugerStatus)
		{
            Debuger.EnableWriteToExternalTxt = debugerStatus;
            Debuger.EnableLog = debugerStatus;
		}

		/**
         * 查看 Debuger状态
         */
		public static bool GetDebugrStatus ()
		{
			return Debuger.EnableLog;
		}

        /**
       * 将本地LOGtext//清除txt 使用者自己控制log是不是每次启动都清除。
       */
        public static void DebugrWriteText()
        {
            IEM_Log.ins.ClearLog();
        }

        /**
     * Unity与android交互
     */
        public static void GetIpFormXml ()
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            NetSDKManager.ip = XMLInterface.ins().PcIP;
#elif UNITY_ANDROID && !UNITY_EDITOR
            NetSDKManager.ip = XMLInterface.ins().PadIP;
#endif
        }

       
		public static void GetThumbnailList(string studentIp, Action<Message> incallback){
			
			Message msg = MessageTools.createFakeMessageOnlyMid (studentIp+ "_T" );
			CallBackManager.putCallBack (new CallBack (msg,incallback));
			JSONObject js = new JSONObject ();
			js.SetField ("type", 1);

			NetSDKManager.SendToStudentCall (Protocol._GETTHUMBNAIL, js, studentIp, null);

			//
		}

		// Return false meaning this picture is already exist.
		public static bool GetImage(string studentIp, String name, Action<Message> incallback){
			name = name.Remove (0, 1);
			name = "I" + name;

			string studentName = TransFileServer.getStudentNumberByMac (Teacher.ins.getMacWithIp (studentIp));
			lock (TransFileServer.ServerPictureDict)
			{
				if (TransFileServer.ServerPictureDict.ContainsKey (studentName + name)) {
					if (TransFileServer.ServerPictureDict [studentName + name] == TransFileServer.PictureStatus.Done) {
						return false;
					}
				}
				}
			Message msg = MessageTools.createFakeMessageOnlyMid (studentIp+ "_" + name);
			CallBackManager.putCallBack (new CallBack (msg,incallback));

			JSONObject js = new JSONObject ();
			js.SetField ("imgName", name);
			js.SetField ("type", 1);
			NetSDKManager.SendToStudentCall (Protocol._GETIMAGE, js, studentIp, null);
			return true;
		}


		// Teacher use =====
		public static void RegistStudentConnect (Action<Message> incallback)
		{
			// Fake Return
			Message msg = MessageTools.createFakeMessageOnlyMid ("PERSIST_StudentConnect");
			CallBackManager.putCallBack (new CallBack (msg,incallback));
			
		}
			
		public static void RegistStudentDisConnect (Action<Message> incallback)
		{
			// Fake Return
			Message msg = MessageTools.createFakeMessageOnlyMid ("PERSIST_StudentDisConnect");
			CallBackManager.putCallBack (new CallBack (msg,incallback));
		}

		public static void RegistMyselfConnect (Action<Message> incallback)
		{
			// Fake Return
			Message msg = MessageTools.createFakeMessageOnlyMid ("PERSIST_MyselfConnect");
			CallBackManager.putCallBack (new CallBack (msg,incallback));

		}

		public static void RegistMyselfDisConnect (Action<Message> incallback)
		{
			// Fake Return
			Message msg = MessageTools.createFakeMessageOnlyMid ("PERSIST_MyselfDisConnect");
			CallBackManager.putCallBack (new CallBack (msg,incallback));
		}
	
		public XMLInterface getXmlInterfaceIns(){
			return XMLInterface.ins();
		}

		public DataManager getDataManagerIns() {
			return DataManager.getInstance ();
		}
	}
}
