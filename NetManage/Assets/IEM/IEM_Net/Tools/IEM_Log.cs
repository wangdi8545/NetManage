using System.Collections;
using System;
using System.IO;
using UnityEngine;

public class IEM_Log
{

    private static string path;
	private static IEM_Log Instance = null;
    public static IEM_Log ins
    {
        get {
            if (Instance == null) {
                Instance = new IEM_Log();           
            }
            path = IEM_Platform.path;
            return Instance;
        }
    }

    
    public void ClearLog() {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    private void writeFileFile(String fileName, String write_str)
    {
        lock (this)
        {
            FileStream fs;
            if (!File.Exists(path))
            {
                fs = new FileStream(path, FileMode.Create);
            }
            else
            {
                fs = new FileStream(path, FileMode.Append);
            }
            try
            {
                byte[] bytes = System.Text.Encoding.Default.GetBytes(write_str);
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
                fs.Close();
            }
            catch (Exception e)
            {
                fs.Flush();
                fs.Close();
               // Debuger.LogError(e);
            }
        }
    }
    // Interface
    public void Log(string mymsg)
    {       
        try
        {
            ins.writeFileFile(path, System.DateTime.Now + " : " + mymsg
                    + "\r\n");
        }
        catch (IOException e)
        {
           // Debuger.LogError(e);
        }
    }
    public void LogError(String mymsg)
    {      
        try
        {
            ins.writeFileFile(path, System.DateTime.Now + " : " + mymsg
                    + "\r\n");
        }
        catch (IOException e)
        {
           // Debuger.LogError(e);
        }
    }
}
