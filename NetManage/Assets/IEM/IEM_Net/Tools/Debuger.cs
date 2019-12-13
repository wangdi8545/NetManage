
using System.Collections;
using System;
using UnityEngine;

public class Debuger
{
	static public event Action<object> DebugCallBack;
	static public event Action<object,object> DebugCallBack2Parm;
	static public bool EnableLog = true;
    static public event Action<string,object> DebugCallBackWithText;
    static public bool EnableWriteToExternalTxt = true;
	public static void Log (object message)
	{       
        Log (message, null);
	}

	public static void Log (object message, object context)
	{
        if (EnableWriteToExternalTxt)
        {			
			IEM_Log.ins.Log(message+"");
        }
		if (EnableLog) {
			Debug.Log (message);
		}
	}

	static public void LogError (object message)
	{       
        LogError (message, null);
	}

	static public void LogError (object message, object context)
	{
        if (EnableWriteToExternalTxt)
        {
            IEM_Log.ins.LogError(message + "");
        }
        if (EnableLog) {
			Debug.LogError (message);		
		}
	}

	static public void LogWarning (object message)
	{      
        LogWarning (message, null);
	}

	static public void LogWarning (object message, object context)
	{
        if (EnableWriteToExternalTxt)
        {
            IEM_Log.ins.Log(message + "");
        }
        if (EnableLog) {
			Debug.LogWarning (message);			
		}
	}
}
public class LogTAG {
    public const string LOG = "LOG";
    public const string LOG_waring = "LOG_waring";
    public const string LOG_Error = "LOG_Error";
}
