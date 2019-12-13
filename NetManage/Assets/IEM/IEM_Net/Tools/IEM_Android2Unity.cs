using UnityEngine;
using System.Collections;
using System.Net.NetworkInformation;

public class IEM_Android2Unity : MonoBehaviour
{
    //PID    
    public static string Getmacid()
    {     
        if (Application.platform == RuntimePlatform.Android)
        {
           return Getmacid_Android();
        }
        else {
          return  Getmacid_other();
        }
    } 

   public static string Getmacid_Android()
    {
         AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		 AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
         Debug.Log("SendtoAndroid_getmacid");
		return jo.Call<string>("SendtoAndroid_getmacid");        
    }
    public static string Getmacid_other()
    {
        if (NetworkInterface.GetAllNetworkInterfaces().Length > 1)
            return NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
        Debuger.Log("没有pc端查询mac的方法");
        return "";
    }
}
