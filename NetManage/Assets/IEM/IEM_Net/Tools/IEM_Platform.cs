using UnityEngine;
using System.Collections;

public class IEM_Platform
{
    private static IEM_Platform instance;
    public static IEM_Platform getinstance
    {

        get
        {
            if (instance == null)
            {     
                instance = new IEM_Platform();
            }
            return instance;
        }

    }

    public  string macNoInfoFile;
    public  string studentInfoFile;
    public  string teacherInfoFile;
    public  string XMLFilePath;
    public  string XMLFileName;
    public   string fileSavePathThumbnail ;
    public   string fileSavePathImage;

    public void ChangePathPlatform()
    {

        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                macNoInfoFile = @"C:\WSK\IEM\WSKConfig\macNoInfo.txt";
                studentInfoFile = @"C:\WSK\IEM\WSKConfig\studentInfo.txt";
                teacherInfoFile = @"C:\WSK\IEM\WSKConfig\teacherInfo.txt";

                XMLFilePath = @"C:\WSK\IEM\WSKConfig";
                XMLFileName = @"C:\WSK\IEM\WSKConfig\ClientData.XML";
                fileSavePathThumbnail = @"C:\WSK\IEL\PIC\Server\Thumbnail\";
                fileSavePathImage = @"C:\WSK\IEL\PIC\Server\Image\"; 
                break;
            case RuntimePlatform.WebGLPlayer:
                macNoInfoFile = @"C:\WSK\IEM\WSKConfig\macNoInfo.txt";
                studentInfoFile = @"C:\WSK\IEM\WSKConfig\studentInfo.txt";
                teacherInfoFile = @"C:\WSK\IEM\WSKConfig\teacherInfo.txt";
                XMLFilePath = @"C:\WSK\IEM\WSKConfig";
                XMLFileName = @"C:\WSK\IEM\WSKConfig\ClientData.XML";
                fileSavePathThumbnail = @"C:\WSK\IEL\PIC\Server\Thumbnail\";
                fileSavePathImage = @"C:\WSK\IEL\PIC\Server\Image\";
                break;
            case RuntimePlatform.WindowsEditor:
                macNoInfoFile = Application.dataPath + @"/Resources/IEL_Resources/IEL_Config/IEL_StudentConfig/macNoInfo.txt";
                studentInfoFile = Application.dataPath + @"/Resources/IEL_Resources/IEL_Config/IEL_StudentConfig/studentInfo.txt";
                teacherInfoFile = Application.dataPath + @"/Resources/IEL_Resources/IEL_Config/IEL_StudentConfig/teacherInfo.txt";
                XMLFilePath = Application.dataPath + @"/Resources/IEL_Resources/IEL_Config/IEL_StudentConfig";
                XMLFileName = Application.dataPath + @"/Resources/IEL_Resources/IEL_Config/IEL_StudentConfig/ClientData.XML";
                fileSavePathThumbnail = @"C:\WSK\IEL\PIC\Server\Thumbnail\";
                fileSavePathImage = @"C:\WSK\IEL\PIC\Server\Image\";
                break;
            case RuntimePlatform.Android:
                macNoInfoFile = "/sdcard/WSK/IEM/WSKConfig/macNoInfo.txt";
                studentInfoFile = "/sdcard/WSK/IEM/WSKConfig/studentInfo.txt";
                teacherInfoFile = "/sdcard/WSK/IEM/WSKConfig/teacherInfo.txt";

                XMLFilePath = "/sdcard/WSK/IEM/WSKConfig";
                XMLFileName = "/sdcard/WSK/IEM/WSKConfig/ClientData.XML";
                fileSavePathThumbnail = "/sdcard/WSK/IEL/PIC/Thumbnail/";
                fileSavePathImage =  "/sdcard/WSK/IEL/PIC/Image/";      
                break;
            default:
                break;
        }
        IESFileTrans.TransFileServer.OnInitPath();
    }
    public string ChangePathPlatform(string path)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                return Application.dataPath + path;
            case RuntimePlatform.Android:
                return "/sdcard/" + path;
            default:
                return @"C://" + path;
        }
    }
    public string ChangePathPlatformD(string path) {
        if (Application.platform == RuntimePlatform.Android)
        {
            return "/sdcard/" + path;
        }
        else {
            return @"D://" + path;
        }
    }
    public string ChangePathShotPic()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return "/sdcard/WSK/IEL/PIC/Thumbnail/";
        }
        else
        {
            return @"C:\WSK\IEL\PIC\Client\Thumbnail\";
        }
    }
    public string ChangePathShotPicCall()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
             return"/sdcard/WSK/IEL/PIC/Image/";
        }
        else
        {
            return @"C:\WSK\IEL\PIC\Client\Image\";
        }
    }
    /// <summary>
    /// LOG PATH
    /// </summary>
    public static string path
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return "/sdcard/WSK/IEM/wsklog.txt";
            }
            else
            {
                return @"C://WSK\IEM\wskNetlog.txt";
            }
        }
    }
}
