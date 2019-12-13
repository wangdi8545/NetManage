using Assets.MessageLib;
using System.Collections;
using System;
using Assets.NetSession;
using Assets.Session;

public class Student
{
	static public Student ins = new Student ();

	public Student ()
	{

	}
	public void Start ()
	{
		Debuger.Log ("[Student.Start] Init a Student, and Prepare to Start Client.cs");
		Client.ins.StartTheClient ();
	}

	public void OnConnect(TeacherSession _ISession)
	{
        //触发事件
        //_ISession.sendActionWithMyself(ActionCommand.OnConnect_com, new JSONObject(), _ISession);
        //if (Assets.NetService.NetSDKManager.OnConnect == null) return;        
        //    Assets.NetService.NetSDKManager.OnConnect(_ISession);
		JSONObject returnJson = new JSONObject();
		returnJson.SetField("OnConnect", true);
		MessageManager.addRecrveMessage (MessageTools.createReturnMessage ("PERSIST_MyselfConnect", returnJson, _ISession), 0);

    }

	public void OnDisConnect (TeacherSession _ISession)
	{
        //_ISession.sendActionWithMyself(ActionCommand.OnDisConnect_com, new JSONObject(), _ISession);
		JSONObject returnJson = new JSONObject();
		returnJson.SetField("OnDisConnect", true);
		MessageManager.addRecrveMessage (MessageTools.createReturnMessage ("PERSIST_MyselfDisConnect", returnJson, _ISession), 0);

    }
}
