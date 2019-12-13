using System;
using System.Threading;

namespace Assets.NetService
{
	public class NoUnityMessageManager
	{
		static NoUnityMessageManager instance = new NoUnityMessageManager ();

		static public NoUnityMessageManager getIns ()
		{
			return instance;
		}

		public void init ()
		{
			isRunning = false;
		}

		private Thread Thread_HandleMessage;
		public bool isRunning = false;

		public  void StartThread ()
		{
			if (isRunning)
				return;
			Thread_HandleMessage = new Thread (HandleDeamon);
			Thread_HandleMessage.IsBackground = true;
			isRunning = true;
			Thread_HandleMessage.Start ();
		}

		public void StopThread ()
		{
			if (!isRunning)
				return;
			isRunning = false;
			Thread_HandleMessage.Abort ();
			Thread_HandleMessage = null;
		}

		void HandleDeamon ()
		{
			while (isRunning) {
				try {
					ServiceManager.update (1);
					Thread.Sleep (10);
				} catch (Exception e) {
					Debuger.Log ("ERROR [NoUnityThread handle message]" + e.ToString ());
					IEM_Log.ins.Log ("ERROR [NoUnityThread handle message]" + e.ToString ());
				}
			}
		}
	}
}

