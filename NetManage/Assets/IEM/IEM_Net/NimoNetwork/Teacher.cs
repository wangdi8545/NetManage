using Assets.MessageLib;
using Assets.Session;
using System.Collections;
using System.Collections.Generic;

public class Teacher {
	static public Teacher ins = new Teacher ();
	static private Dictionary<string, string> IpAddressMap =new Dictionary<string, string>();

	public Teacher()
	{
		
	}

	public void Start ()
	{
		Debuger.Log ("[Teacher.Start] Init a Teacher, and Prepare to Start Server.cs");
		Server.ins.StartServer ();
	}

	public void OnConnect(StudentSession _ISession)
	{
        //_ISession.sendActionWithMyself(ActionCommand.OnConnect_com, new JSONObject(), _ISession);
		JSONObject returnJson = new JSONObject();
		returnJson.SetField("connect_student_ip", _ISession.getIpAddress());
		MessageManager.addRecrveMessage (MessageTools.createReturnMessage ("PERSIST_StudentConnect", returnJson, _ISession), 0);
    }

	public void OnDisconnect(StudentSession _ISession)
	{
        //_ISession.sendActionWithMyself(ActionCommand.OnDisConnect_com, new JSONObject(), _ISession);        
		JSONObject returnJson = new JSONObject();
		returnJson.SetField("disconnect_student_ip", _ISession.getIpAddress());
		MessageManager.addRecrveMessage (MessageTools.createReturnMessage ("PERSIST_StudentDisConnect", returnJson, _ISession), 0);
        
    }

	public void setIpAndMac(string ip, string mac){
		IpAddressMap [ip] = mac;
	}

	public string getMacWithIp(string ip){
		string ans = "";
		if (IpAddressMap.ContainsKey (ip)) {
			ans = IpAddressMap [ip];
		}
		return ans;
	}
}
