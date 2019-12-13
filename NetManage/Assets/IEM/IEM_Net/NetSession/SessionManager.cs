using Assets.MessageLib;
using Assets.NetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Assets.Session;

namespace Assets.NimoNetwork
{
	public class SessionManager
	{
		static Dictionary<string, StudentSession> stuMap = new Dictionary<string, StudentSession> ();
		static List<StudentSession> stuList = new List<StudentSession> ();
		static TeacherSession ts = null;

		public static void init ()
		{

		}

		public static void update ()
		{
			ServiceManager.update (0);
		}

		public static ISession createOrGetTeacherSession (TcpClient client)
		{
			if (ts == null) {
				ts = new TeacherSession (client);
			} else {
				ts.setSession (client);
			}
			return ts;
		}

		public static ISession createOrGetSession (string ip, TcpClient client)
		{
			StudentSession s = null;
			if (stuMap.ContainsKey (ip)) {
				s = stuMap [ip];
				s.setSession (client);
				s.IsFirstLogout = true;
			} else {
				s = new StudentSession (client);
				stuMap.Add (ip, s);
				stuList.Add (s);
			}
			return s;
		}

		public static ISession getSession (string stuKey)
		{
			if (stuMap.ContainsKey(stuKey))
				return stuMap [stuKey];
			return null;
		}

		public static void SendToStudentCall (string command, JSONObject msg, string stuKey, Action<Message> incallback)
		{
			//找到对应的StudentSession并进行发送
			getSession (stuKey).sendCall (command, msg, incallback);
		}

		public static void SendToTeacherCall (string command, JSONObject msg, Action<Message> incallback)
		{
            if (ts != null)
            {
                ts.sendCall(command, msg, incallback);
            }
            
		}

		public static void SendToAllStudentCall (string command, JSONObject msg, Action<Message> incallback)
		{
			Dictionary<string, StudentSession>.ValueCollection valueColl = stuMap.Values;
			foreach (StudentSession session in valueColl) {
				try {
					session.sendCall (command, msg, incallback);
				} catch (Exception ex) {
					Debuger.LogError ("[SessionManager.SendToALLStudentCall]"+ex);
				}
			}
		}

		public static void SendToStudentLet (string command, JSONObject msg, string stuKey)
		{
			//找到对应的StudentSession并进行发送
			getSession (stuKey).send (command, msg);
		}

		public static void SendToTeacherLet (string command, JSONObject msg)
		{
            if (ts != null)
            {
                ts.send(command, msg);
            }
			
		}

		public static void SendToAllStudentLet (string command, JSONObject msg)
		{
			Dictionary<string, StudentSession>.ValueCollection valueColl = stuMap.Values;
			foreach (StudentSession session in valueColl) {
				try {
					session.send (command, msg);
				} catch (Exception ex) {
					Debuger.LogError ("[SessionManager.SendToALLStudentLet]"+ex);
				}
			}
		}

		public static void setTeacherSession (TeacherSession session)
		{
            if (ts != null)
            {
                ts = session;
            }
		}

		public static List<StudentSession> getStumapStudentSessions (){
			return stuList;
		}

	}
}














