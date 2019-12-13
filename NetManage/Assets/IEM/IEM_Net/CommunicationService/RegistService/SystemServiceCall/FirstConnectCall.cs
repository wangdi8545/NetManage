using System.Collections;
using Assets.NetService;
using Assets.MessageLib;
using Assets.Session;
using Data;
using IESFileTrans;

namespace Assets.TestMessage
{
	public class FirstConnectCall : ServiceCall
	{
		private static FirstConnectCall instance = new FirstConnectCall();
		public static void regist()
		{
			ServiceManager.registCall(Protocol._FirstConnectCall, instance);
		}

		public JSONObject access(Message msg)
		{
			JSONObject parameter = msg.parameter;
			ISession session = msg.session;
			string address = string.Empty;
			try{
				address = parameter.GetField("mac").str;
			}
			catch {
				address = string.Empty;
			}
//			session.setMacAddress(address);
			Teacher.ins.setIpAndMac (session.getIpAddress(), address);
			Debuger.Log ("[FirstConnectCall] First Connect Recall macAddress: "+ session.getIpAddress()+ " -->" + address);

			//init
			StudentSession ss = (StudentSession)Assets.NimoNetwork.SessionManager.getSession (session.getIpAddress ());
			// Clear the PictureServer
			TransFileServer.CleanOneStudentServerPictureStatus (address);
			if (ss == null) {
				Debuger.Log ("??? session is null");
			}
			ss.setMacAddress (address);
			ss.thisStudentBean = DataManager.getInstance ().getNowClassStudentNameByMac (address);
//			Teacher.ins.OnConnect (ss);

			// fake return tell UI thread
			Teacher.ins.OnConnect(ss);

			return null;
		}

	}
}
