
using System.Collections;
using System.Xml;
using System.IO;
using System;
using System.Net.NetworkInformation;

public class XMLInterface
{

    private static XMLInterface Instance = null;
    private static string XMLFileResoures = "IEL_Resources/IEL_Config/IEL_StudentConfig/ClientData";

    private static string XMLFilePath;
    private static string XMLFileName;
    static XmlDocument xmlDoc = new XmlDocument();
    public static XMLInterface ins()
    {
        if (Instance == null)
        {
            pathInit();
            if (!fileExist())
            {
                XMLSave();
            }
            else {
                OnintDecrypt();
            }
            Instance = new XMLInterface();
        }
        return Instance;
    }
    static void pathInit()
    {
        IEM_Platform.getinstance.ChangePathPlatform();
        XMLFilePath = IEM_Platform.getinstance.XMLFilePath;
        XMLFileName = IEM_Platform.getinstance.XMLFileName;
    }
    public int DeviceNumber
    {
        get
        {
            return Int32.Parse(getXMLElement(0));
        }
        set
        {
            setXMLElement(0, value.ToString());
        }
    }

    public string Name
    {
        get
        {
            return getXMLElement(1);
        }
        set
        {
            setXMLElement(1, value);
        }
    }

    public string PadIP
    {
        get
        {
            return getXMLElement(2);
        }
        set
        {
            setXMLElement(2, value);
        }
    }

    public string PcIP
    {
        get
        {
            return getXMLElement(3);
        }
        set
        {
            setXMLElement(3, value);
        }
    }

    //teacher name
    public string TeacherName
    {
        get
        {
            return getXMLElement(4);
        }
        set
        {
            setXMLElement(4, value);
        }
    }
    public string GridName
    {

        get
        {
            return getXMLElement(5);
        }
        set
        {
            setXMLElement(5, value);
        }
    }
    public string ClassName
    {
        get
        {
            return getXMLElement(6);
        }
        set
        {
            setXMLElement(6, value);
        }
    }

    public string LessonName
    {
        get
        {
            return getXMLElement(7);
        }
        set
        {
            setXMLElement(7, value);
        }
    }


    public string Macid
    {
        get
        {
            string str = IEM_Android2Unity.Getmacid();
            if (str.Equals(""))
            {
                str = getXMLElement(8);
            }
            //更新MAC@王迪          
            return str;
        }
        set
        {
            setXMLElement(8, value);
        }
    }

    //取多节点
    public string[] getXMLElements(int index)
    {
        try
        {
            XmlNode RootNode = xmlDoc.DocumentElement;
            XmlNodeList xmlnodelist = RootNode.ChildNodes[index].ChildNodes;
            string[] temp = new string[xmlnodelist.Count];
            for (int i = 0; i < xmlnodelist.Count; i++)
            {
                temp[i] = xmlnodelist[i].InnerText;
            }
            return temp;
        }
        catch (Exception e)
        {
            DeletFile();
            XMLSave();
            return null;
        }
    }
    public string[] GetJurisdiction(int ty)
    {
        return getXMLElements(ty);
    }

    public void SetJurisdiction(string[] value, int ty)
    {
        setXMLElements(ty, value);
    }
    private void setXMLElements(int index, string[] value)
    {
        try
        {
            XmlNode RootNode = xmlDoc.DocumentElement;
            XmlNode tempnode = RootNode.ChildNodes[index];//父节点
            int farnodeNum = tempnode.ChildNodes.Count;
            int num = farnodeNum - value.Length;
            if (num > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    XmlNode temp = tempnode.ChildNodes[i];
                    tempnode.RemoveChild(temp);//
                }
            }
            else if (num < 0)
            {
                for (int i = 0; i < (-num); i++)
                {
                    XmlElement temp = xmlDoc.CreateElement("num" + (farnodeNum + i + 1));
                    temp.InnerText = "455";
                    tempnode.AppendChild(temp);
                }
            }
            for (int i = 0; i < tempnode.ChildNodes.Count; i++)
            {
                tempnode.ChildNodes[i].InnerText = value[i];
            }
            xmlDoc.Save(XMLFileName);
            SaveENcryptFile();
        }
        catch (Exception e)
        {
            DeletFile();
            XMLSave();
        }
    }
    public string getXMLElement(int index)
    {
        try
        {
             // XmlDocument xmlDoc = new XmlDocument();
             //  xmlDoc.Load(XMLFileName);
            XmlNode RootNode = xmlDoc.DocumentElement;
            return RootNode.ChildNodes[index].InnerText;
        }
        catch (Exception e)
        {
            Debuger.Log("xml异常（确定元素是否存在xml中）" + e);
            return null;
        }
    }

    public void setXMLElement(int index, string value)
    {
        try
        {
            
           // xmlDoc.Load(XMLFileName);
            XmlNode RootNode = xmlDoc.DocumentElement;
            RootNode.ChildNodes[index].InnerText = value;
            xmlDoc.Save(XMLFileName);
            SaveENcryptFile();
        }
        catch (Exception e)
        {
            Debuger.Log("xml异常（确定元素是否存在xml中）" + e);     
        }
    }


    private static bool fileExist()
    {
        if (!Directory.Exists(XMLFilePath))
        {
            Directory.CreateDirectory(XMLFilePath);
        }
        return File.Exists(XMLFileName);
    }
    public static void DeletFile()
    {
        File.Delete(XMLFileName);
    }

    private static void XMLSave()
    {
        //创建XmlDocument对象xmlDoc  
       // XmlDocument xmlDoc = new XmlDocument();
        //创建一个XML文档声明，并添加到文档  
        XmlDeclaration declare = xmlDoc.CreateXmlDeclaration("1.0", "utf-8",
                                     "yes");
        xmlDoc.AppendChild(declare);
        //创建并添加UserList结点  
        XmlElement clientDataEle = xmlDoc.CreateElement("ClientData");
        xmlDoc.AppendChild(clientDataEle);

        XmlElement deviceNumberEle = xmlDoc.CreateElement("DeviceNumber");
        deviceNumberEle.InnerText = "-1";
        clientDataEle.AppendChild(deviceNumberEle);

        XmlElement nameEle = xmlDoc.CreateElement("Name");
        nameEle.InnerText = "姓名";
        clientDataEle.AppendChild(nameEle);

        XmlElement PadIPEle = xmlDoc.CreateElement("PadIP");
        PadIPEle.InnerText = "192.168.31.41";
        clientDataEle.AppendChild(PadIPEle);

        XmlElement PCIPEle = xmlDoc.CreateElement("PCIP");
        PCIPEle.InnerText = "192.168.31.5";
        clientDataEle.AppendChild(PCIPEle);

        XmlElement teacher = xmlDoc.CreateElement("TeacherName");
        teacher.InnerText = "没初始化";
        clientDataEle.AppendChild(teacher);

        XmlElement griad = xmlDoc.CreateElement("GridName");
        griad.InnerText = "一";
        clientDataEle.AppendChild(griad);

        XmlElement ClassName = xmlDoc.CreateElement("ClassName");
        ClassName.InnerText = "1";
        clientDataEle.AppendChild(ClassName);

        XmlElement LessonName = xmlDoc.CreateElement("LessonName");
        LessonName.InnerText = "1";
        clientDataEle.AppendChild(LessonName);


        XmlElement macid = xmlDoc.CreateElement("macid");
        macid.InnerText = "1";
        clientDataEle.AppendChild(macid);


        XmlElement isPadServerPEle = xmlDoc.CreateElement("isPadServer");
        isPadServerPEle.InnerText = "0";
        clientDataEle.AppendChild(isPadServerPEle);

        XmlElement Jurisdiction = xmlDoc.CreateElement("Jurisdiction");
        XmlElement xel1 = xmlDoc.CreateElement("num1");
        xel1.InnerText = "455";
        Jurisdiction.AppendChild(xel1);
        clientDataEle.AppendChild(Jurisdiction);

        //通过Save()方法保存数据到XML文件
        xmlDoc.Save(XMLFileName);
        SaveENcryptFile();
    }
    //保存加密后的文件
    static void SaveENcryptFile()
    {
        IEM_File.SaveENcryptFile(XMLFileName);       
    }
    static void OnintDecrypt()
    {
        try {
            IEM_File.SaveENcryptFile(XMLFileName);//先解密
            xmlDoc.Load(XMLFileName);//获取实例
            IEM_File.SaveENcryptFile(XMLFileName);//再次加密
        } catch (Exception e) {
            DeletFile();
            XMLSave();
        }
       
    }
}
