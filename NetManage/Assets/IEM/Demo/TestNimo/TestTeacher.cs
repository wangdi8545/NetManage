using UnityEngine;
using System.Collections;
using Assets.Session;
using Assets.MessageLib;
using Assets.NimoNetwork;
using Assets.NetService;
using Assets.TestMessage;
using UnityEngine.UI;
using Data;
using System;

public class TestTeacher : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		ServiceRegistManager.getInstance ().initTeacherServiceRegist ();
//		CGraetText.fnDeleteFile ();
		Debuger.EnableWriteToExternalTxt = true;
//		NetSDKManager.OnConnect += test;
//		NetSDKManager.OnDisConnect += testDis;

		IEM_Tools.init ();
		Debuger.Log (IEM_Tools.getMacAddress());
	}

	// Update is called once per frame
	void Update ()
	{
		SessionManager.update ();
	}

	void OnGUI ()
	{
		GUI.skin.textField.fontSize = 10;
		GUI.skin.button.fontSize = 10;
		GUI.skin.label.fontSize = 70;
//		GUILayout.Label (OutLog.outpath);
       
		if (GUILayout.Button ("Start Teacher")) {
			NetSDKManager.RegistStudentConnect (MyConnectPrint);
			NetSDKManager.RegistStudentDisConnect (MyDisConectPrint);
			NetSDKManager.init (NetSDKManager.StartServer);
		}

		if (GUILayout.Button ("要缩略图")) {

			NetSDKManager.GetThumbnailList ("127.0.0.1", MyPrint);


		}
		if (GUILayout.Button ("要大图")) {

			NetSDKManager.GetImage ("127.0.0.1", "T_20170203_1.jpg", MyPrint);
		}

		if (GUILayout.Button ("test")) {
			NetSDKManager.SendToAllStudentCall (Protocol.S_CALL_GETSTUDENTNAME, null, MyPrint);
		}


	}

	void MyPrint (Message msg)
	{
		Debug.Log ("MyPrint " + msg.parameter.ToString ());
		Debug.Log ("MyPrint:" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
	}

	void MyConnectPrint (Message msg)
	{
//		Debug.Log ("!!OK!! " + msg.parameter.ToString ());
		StudentBean sb = ((StudentSession)msg.session).thisStudentBean;
		Debug.Log ("[MyConnectPrint:] 学生连入" + sb.getStudentName());
	}



	void MyDisConectPrint (Message msg)
	{
		Debug.Log ("!!OK!! " + msg.parameter.ToString ());
		StudentBean sb = ((StudentSession)msg.session).thisStudentBean;
		Debuger.Log (sb.getStudentName());

	}

	void OnDestroy ()
	{
		NetSDKManager.destroyNetworkThread ();
	}

}
