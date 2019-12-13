using UnityEngine;
using System.Collections;
using Assets.MessageLib;
using Assets.NimoNetwork;
using Assets.NetService;
using Assets.TestMessage;
using UnityEngine.UI;
using Assets.Session;

public class TestStudent : MonoBehaviour {

	// Use this for initialization
	void Start () {
//        
//        NetSDKManager.OnConnect+= test;
//		NetSDKManager.OnDisConnect += testDis;
    }
	void MyPrint (Message msg)
	{
		Debug.Log ("MyPrint " + msg.parameter.ToString ());

	}

    int now = 1;
    // Update is called once per frame
    void Update () {
		SessionManager.update ();
        GameObject.Find("/Canvas/Text").GetComponent<Text>().text = now.ToString();
        now++;
        if (now > 10) now = 1;

    }

	private bool isrunning = false;
	private string ipAddress = "127.0.0.1";
	void OnGUI()
	{
		GUI.skin.button.fontSize = 70;
		GUI.skin.label.fontSize = 30;
		GUI.skin.textField.fontSize = 70;

		ipAddress = GUILayout.TextField (ipAddress);

		if (!isrunning && GUILayout.Button ("使用上面的IP")) {
			NetSDKManager.ip = ipAddress;
			NetSDKManager.RegistMyselfConnect (MyPrint);
			NetSDKManager.RegistMyselfDisConnect (MyPrint);


			NetSDKManager.init(NetSDKManager.StartClient);
			isrunning = true;
		}

		//if (!isrunning && GUILayout.Button ("连接电脑")) {
		//	NetSDKManager.ip = "192.168.1.97";
		//	NetSDKManager.init(NetSDKManager.StartClient);
		//	isrunning = true;
		//}

		//if (!isrunning && GUILayout.Button ("连接Test 5.8G")) {
		//	NetSDKManager.ip = "192.168.188.200";
		//	NetSDKManager.init(NetSDKManager.StartClient);
		//	isrunning = true;
		//}

//		if (GUILayout.Button ("询问学生个数")) {
//			new SendTest().testSend();
//		}
		if (GUILayout.Button ("退出")) {
			Application.Quit ();
		}

		GUILayout.Label ("===========");
		//GUILayout.Label (//LableString.showlable());
	}

	void OnDestroy()
	{
		NetSDKManager.destroyNetworkThread ();
	}

}
