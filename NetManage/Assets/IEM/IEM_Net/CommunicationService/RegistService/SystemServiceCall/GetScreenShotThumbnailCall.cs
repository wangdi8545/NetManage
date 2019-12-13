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

public class GetScreenShotThumbnailCall : ServiceCall
{

	private static GetScreenShotThumbnailCall instance = new GetScreenShotThumbnailCall ();

    //@any 去除宏定义
    private string ThumbnailPath ="";
	


	public static void regist ()
	{
		ServiceManager.registCall (Assets.TestMessage.Protocol._GETTHUMBNAIL, instance);
	}

	List<string> SendFilesFullPaths = new List<string> ();

	void myStaticThreadMethod ()
	{
		TransFileCient.SendFiles (SendFilesFullPaths);
//		bool result = TransFileCient.SendFiles (SendFilesFullPaths);
//		while (!result) {
//			Thread.Sleep (1000);
//			result = TransFileCient.SendFiles (SendFilesFullPaths);
//		}
	}


	public JSONObject access (Message msg)
	{
        ThumbnailPath = IEM_Platform.getinstance.ChangePathShotPic();
        // 开始发送缩略图 使用线程
        SendFilesFullPaths.Clear ();
		DirectoryInfo folder = new DirectoryInfo (ThumbnailPath);
		if (!folder.Exists) {
//			Debuger.Log ("文件夹不存在");
		} else {
			foreach (FileInfo file in folder.GetFiles("*.jpg")) {
				SendFilesFullPaths.Add (file.FullName);
			}
		}

		Debuger.Log ("缩略图总共有:" + SendFilesFullPaths.Count);
		Thread thread1 = new Thread (myStaticThreadMethod);  
		thread1.Start ();  // 只要使用Start方法，线程才会运行  

		//TODO
		JSONObject js = new JSONObject ();

		// 将 SendFilesFullPaths 处理之后 塞入 js 里面
		return js;
	}
}
