using UnityEngine;
using System.Collections;
using System.IO;
///------------------------------说明---------------------------
///
/// 为方便解密在此文件夹下有解密工程。SVN\Repository\技术\jiamijiemi



using System.Collections.Generic;
using System;

public class IEM_File  {
    public static string key = "weishikuen";//加密
   
   public static string  GetOneFile(string path) {
        string str = "";
        if (checkFile(path))
        {
            StreamReader r = new StreamReader(path);           
            str = r.ReadToEnd();
            r.Close();
        }
        return str;
    }
    public static void SetOneFile(string path,string mes) {
        File.WriteAllText(path, mes, System.Text.Encoding.GetEncoding("UTF-8"));
    }
  static  bool checkFile(string path) {
        if (File.Exists(path)) {
            return true;
        }
        return false;
    }
    
    public static void deletFile(string path)
    {
        if (File.Exists(path))//如果不存在就创建file文件夹
        {
            File.Delete(path);
        }
    }      
     static string TextEncrypt(string content, string secretKey)
    {
        char[] data = content.ToCharArray();
        char[] key = secretKey.ToCharArray();
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key[i % key.Length];
        }
        return new string( data);
    }
     static string TextDecrypt(string sdata, string secretKey)
    {
        char[] data = sdata.ToCharArray(); 
        char[] key = secretKey.ToCharArray();
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key[i % key.Length];
        }
        return new string(data);
    }

    //加密
    public static void SaveENcryptFile(string path)
    {
        string temp = TextEncrypt(IEM_File.GetOneFile(path), key);
        IEM_File.SetOneFile(path, temp);
    }
    //解密
    public static void GetDecryptFile(string path)
    {
        string temp = TextDecrypt(GetOneFile(path), key);
        IEM_File.SetOneFile(path, temp);
    }
}
