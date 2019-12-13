

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class IEM_Editor : Editor
{

    const string MenuName = "IEM";
    static string getDirepathICON = Application.dataPath + "/IEM/Plugin/Icon";//有图标文件夹
    static string getDirepathNOICON = Application.dataPath + "/IEM/Plugin/NoIcon";//无图标文件夹
    static string mainfeast = "AndroidManifest.xml";
    static string classjar = "classes_.jar";
    static string setpath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
    //[MenuItem(MenuName+"/NoIcon")]
    public static void DoSomethingWithANoIcon()
    {
        UpDateXml(false, setpath);
        //Debuger.Log("无图标版本");
        AssetDatabase.Refresh();
    }
    //[MenuItem(MenuName + "/Icon")]
    public static void DoSomethingWithAIcon()
    {
        UpDateXml(true, setpath);
        //Debuger.Log("有图标版本");
        AssetDatabase.Refresh();
    }

    public static void SetIcon(bool icon)
    {
        if (icon)
            DoSomethingWithAIcon();
        else
            DoSomethingWithANoIcon();
    }

    static void CopyFolder(string srcPath, string tarPath)
    {
        if (!Directory.Exists(srcPath))
        {
            Debug.LogError("IEM本地文件夹丢失：（Plugins）");
            return;
        }
        if (!Directory.Exists(tarPath))
        {
            Debug.LogError("新建立的Plugins没有签名！！！（查看Assets/Plugins/Android）");
            Directory.CreateDirectory(tarPath);
        }
        else
        {
            CheckFileWithDir(tarPath);
        }
        CopyFile(srcPath, tarPath);
        string[] directionName = Directory.GetDirectories(srcPath);
        foreach (string dirPath in directionName)
        {
            string directionPathTemp = tarPath + "\\" + dirPath.Substring(srcPath.Length + 1);
            CopyFolder(dirPath, directionPathTemp);
        }
    }
    static void CopyFile(string srcPath, string tarPath)
    {
        string[] filesList = Directory.GetFiles(srcPath);
        foreach (string f in filesList)
        {
            string fTarPath = tarPath + "\\" + f.Substring(srcPath.Length + 1);
            if (File.Exists(fTarPath))
            {
                File.Copy(f, fTarPath, true);
            }
            else
            {
                File.Copy(f, fTarPath);
            }
        }
    }

    static void CheckFileWithDir(string dirpath)
    {
        DirectoryInfo di = new DirectoryInfo(dirpath);
        DirectoryInfo[] diA = di.GetDirectories();//获得了所有一级子目录
        FileInfo[] fiA = di.GetFiles();//获得了所有起始目录下的文件
        for (int i = 0; i < fiA.Length; i++)
        {
            if (fiA[i].Name.Equals(mainfeast) || fiA[i].Name.Equals(classjar))
            {
                Debug.Log(fiA[i].Name);
                File.Delete(fiA[i].ToString());
            }
        }
    }


    //更新XML  
    public static void UpDateXml(bool isIcon, string filePath)
    {
        //检测xml是否存在  
        if (File.Exists(filePath))
        {
            //新建实例  
            XmlDocument xmlDoc = new XmlDocument();
            //根据路径将xml读取出来  
            xmlDoc.Load(filePath);
            //得到根节点  
            XmlNodeList nodeList =
               xmlDoc.SelectSingleNode
                   ("manifest").ChildNodes;

            //遍历所有子节点  
            for (int t = 0; t < nodeList.Count; t++)
            {
                if (nodeList[t].Name.Equals("#comment")) continue;
                XmlElement xe = nodeList[t] as XmlElement;
                //}
                //(XmlElement xe in nodeList)
                //{
                if (xe.Name.Equals("application"))
                {
                    for (int i = 0; i < xe.ChildNodes.Count; i++)
                    {

                        if (xe.ChildNodes[i].Name.Equals("#comment")) continue;
                        if (xe.ChildNodes[i].Name.Equals("activity"))
                        {
                            XmlElement x1 = xe.ChildNodes[i] as XmlElement;
                            AlterXmlExcludeFromRecents(isIcon, filePath, xmlDoc, x1);
                            foreach (XmlElement x2 in x1.ChildNodes)
                            {
                                if (x2.Name.Equals("intent-filter"))
                                {
                                    Exchange(isIcon, filePath, xmlDoc, x2);
                                    //for (int j = 0; j < x2.ChildNodes.Count; j++)
                                    //{
                                 

                                    //}
                                    //x2.RemoveAll();
                                    //addxnlDocument(isIcon, filePath, xmlDoc, x2);
                                }
                            }
                        }

                    }

                }
                xmlDoc.Save(filePath);
                // Debuger.Log("UpDateXML OK!");
            }
        }
        else
        {
            Debug.LogError("请配置Plugins（Assets/Plugins/Android）");
        }


    }

    protected static void AlterXmlExcludeFromRecents(bool iscon, string filePath, XmlDocument doc, XmlElement parxml)
    {
        parxml.SetAttribute("android:excludeFromRecents", !iscon ? "true" : "false");
    }

    private static void Exchange(bool iscon, string filePath, XmlDocument doc, XmlElement parxml)
    {

        bool ishavecon = false;
        for (int j = 0; j < parxml.ChildNodes.Count; j++)
        {
            string arg = parxml.ChildNodes[j].Attributes[0].ChildNodes[0].Value;
            if (arg.Equals("android.intent.category.LEANBACK_LAUNCHER"))
            {
                ishavecon = true;
                break;
            }
        }
        if (!ishavecon)
        {
            if (iscon)
            {
                //创建节点
                XmlElement xmlElement1 = doc.CreateElement("category");
                //添加属性
                xmlElement1.SetAttribute("android:name", "http://schemas.android.com/apk/res/android",
                    "android.intent.category.LEANBACK_LAUNCHER");
                XmlElement xmlElement2 = doc.CreateElement("category");
                //添加属性
                xmlElement2.SetAttribute("android:name", "http://schemas.android.com/apk/res/android",
                    "android.intent.category.LAUNCHER");
                parxml.PrependChild(xmlElement2);
                //将节点加入到指定的节点下
                parxml.PrependChild(xmlElement1);
            }

        }
        else
        {
            if (!iscon)
            {
                List<XmlNode> RemoveList = new List<XmlNode>() ;
                for (int j = 0; j < parxml.ChildNodes.Count; j++)
                {
                    string arg = parxml.ChildNodes[j].Attributes[0].ChildNodes[0].Value;


                    if (arg.Equals("android.intent.category.LEANBACK_LAUNCHER"))
                    {
                        if(!RemoveList.Contains(parxml.ChildNodes[j]))
                            RemoveList.Add(parxml.ChildNodes[j]);
                    }
                    if (arg.Equals("android.intent.category.LAUNCHER"))
                    {
                        if (!RemoveList.Contains(parxml.ChildNodes[j]))
                            RemoveList.Add(parxml.ChildNodes[j]);
                    }
                }

                for (int i = 0; i < RemoveList.Count; i++)
                {
                    parxml.RemoveChild(RemoveList[i]);
                }
                RemoveList = null;
            }

        }
        doc.Save(filePath);

    }

    protected static void addxnlDocument(bool iscon, string filePath, XmlDocument doc, XmlElement parxml)
    {
        return;
        if (iscon)
        {
            //创建节点
            XmlElement xmlElement1 = doc.CreateElement("category");
            //添加属性
            xmlElement1.SetAttribute("android:name", "http://schemas.android.com/apk/res/android", "android.intent.category.LEANBACK_LAUNCHER");
            XmlElement xmlElement2 = doc.CreateElement("category");
            //添加属性
            xmlElement2.SetAttribute("android:name", "http://schemas.android.com/apk/res/android", "android.intent.category.LAUNCHER");
            parxml.PrependChild(xmlElement2);
            //将节点加入到指定的节点下
            parxml.PrependChild(xmlElement1);
        }


        XmlElement xmlElement3 = doc.CreateElement("action");
        //添加属性
        xmlElement3.SetAttribute("android:name", "http://schemas.android.com/apk/res/android", "android.intent.action.MAIN");


        parxml.PrependChild(xmlElement3);
        doc.Save(filePath);
    }

}
