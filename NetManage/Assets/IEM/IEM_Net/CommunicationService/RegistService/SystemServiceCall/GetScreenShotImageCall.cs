using Assets.NetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.MessageLib;
using Assets.Session;
using IESFileTrans;
using System.Threading;
using System.IO;

public class GetScreenShotImageCall : ServiceCall
{
	private static GetScreenShotImageCall instance = new GetScreenShotImageCall ();
    private string ImagePath;
    public static void regist ()
	{
        

        ServiceManager.registCall (Assets.TestMessage.Protocol._GETIMAGE, instance);
	}

	//List<string> SendFilesFullPaths = new List<string> ();

	
	

	void myStaticThreadMethod (object _object)
	{
        ImagePath = IEM_Platform.getinstance.ChangePathShotPicCall();
#if UNITY_ANDROID
        List<string> SendFilesFullPaths = (List<string>)_object;
        TransFileCient.SendFiles (SendFilesFullPaths);
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
		List<string> SendFilesFullPaths = (List<string>)_object;
        TransFileCient.SendFiles (SendFilesFullPaths);
#else
		Debug.Log("Not Support this platform");
#endif
    }

    public JSONObject access (Message msg)
	{
        ImagePath = IEM_Platform.getinstance.ChangePathShotPicCall();
        List<string> SendFilesFullPaths = new List<string>();
        SendFilesFullPaths.Clear();

        DirectoryInfo folder = new DirectoryInfo (ImagePath);
		if (!folder.Exists) {
			Debuger.Log ("传输 IMG 的时候 ， 文件夹不存在");
			return null;
		}

		string imgName = "";
		try {
			imgName = msg.parameter.GetField ("imgName").str;
		} catch (Exception ex) {
		}

		if (imgName.Equals (""))
			return null;

		imgName = imgName.Remove (0, 1);
		imgName = "I" + imgName;
	Debuger.Log (ImagePath + imgName);
		SendFilesFullPaths.Add (ImagePath + imgName);
		Thread thread1 = new Thread (new ParameterizedThreadStart(myStaticThreadMethod));
		thread1.Start (SendFilesFullPaths);  // 只要使用Start方法，线程才会运行

		return null;
	}


}


