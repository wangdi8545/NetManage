using System;
using System.Collections;
using LitJson;
using System.IO;
using System.Net;
using System.Text;
using Assets.NetService;

namespace Data
{
	public class DataLoadManager
	{
		private static DataLoadManager instance = new DataLoadManager ();

		public static DataLoadManager getInstance ()
		{
			return instance;
		}
        private static string macNoInfoFileCopy = "IEL_Resources/IEL_Config/IEL_StudentConfig/macNoInfo";
        private static string studentInfoFileCopy = "IEL_Resources/IEL_Config/IEL_StudentConfig/studentInfo";
        private static string teacherInfoFileCopy = "IEL_Resources/IEL_Config/IEL_StudentConfig/teacherInfo";


		private static string macNoInfoURL = @"http://" + NetSDKManager.LoadResourceIP +  @"/queryByMac/";
		private static string studentInfoURL = @"http://" + NetSDKManager.LoadResourceIP +  @"/fakeQueryForStudentInfo";
		private static string teacherInfoURL = @"http://" + NetSDKManager.LoadResourceIP +  @"/fakeQueryForTeacherInfo";


        public string macNoInfoFile;
        public string studentInfoFile;
        public string teacherInfoFile;

        public ArrayList macs = new ArrayList();
        public DataLoadManager ()
		{
           
		}
        void OninitPath() {
            IEM_Platform.getinstance.ChangePathPlatform();
            macNoInfoFile = IEM_Platform.getinstance.macNoInfoFile;
            studentInfoFile = IEM_Platform.getinstance.studentInfoFile;
            teacherInfoFile = IEM_Platform.getinstance.teacherInfoFile;

        }

		public bool load ()
		{      
            loadMacInfo();
			loadTeacherInfo ();
			loadStudentInfo ();
			return true;
		}
        void CheckPath(string outpath,string inPath) {
            
            if (!File.Exists(outpath))
            {
                string x =LoadTxtWithResources.ResLoadWithUnity(inPath);
                if (x != null)
                {
                    //File.Copy(XMLFileCopyName, XMLFileName, true);
                    IEM_FileTools.FielCopyCreat(outpath, x);
                }
                else
                {
                    Debuger.LogError("Resources下没有文件");
                }
               // File.Copy(inPath, outpath, true);
            }
        }
        
		public ArrayList loadMacInfo ()
        {
            OninitPath();
            CheckPath(macNoInfoFile, macNoInfoFileCopy);
            ArrayList macList = new ArrayList ();
			JsonData macjd = null;
			if (NetSDKManager.isloadResrouceFromURL) {
				macjd = getJsonDataFromFileOrURL (macNoInfoURL + XMLInterface.ins ().Macid);
			} else {
				Debuger.Log (macNoInfoFile);
				macjd = getJsonDataFromFileOrURL (macNoInfoFile);
			}
//			Debuger.Log ("macjd.count:" + macjd.Count);
			for (int i = 0; i < macjd.Count; i++) {
				DeviceBean db = new DeviceBean (macjd [i]);
				macList.Add (db);
          
//				Debuger.Log ("TestJsonFile========================== db:" + db.getMac ());
            }
            macs = macList;
            return macList;
		}

		public ArrayList loadStudentInfo ()
		{
            CheckPath(studentInfoFile, studentInfoFileCopy);
            ArrayList studentList = new ArrayList ();
			JsonData studentjd = null;
			if (NetSDKManager.isloadResrouceFromURL) {
				studentjd = getJsonDataFromFileOrURL (studentInfoURL);
			} else {
				studentjd = getJsonDataFromFileOrURL (studentInfoFile);
			}				
//			Debug.Log ("studentjd.count:" + studentjd.Count);
			for (int i = 0; i < studentjd.Count; i++) {
				StudentBean db = new StudentBean (studentjd [i]);
				studentList.Add (db);
//				Debuger.Log ("TestJsonFile========================== db:" + db.getStudentName ());
			}
			return studentList;
		}

		public ArrayList loadTeacherInfo ()
		{
            CheckPath(teacherInfoFile, teacherInfoFileCopy);
            ArrayList teacherList = new ArrayList ();
			JsonData teacherjd = null;
			if (NetSDKManager.isloadResrouceFromURL) {
				teacherjd = getJsonDataFromFileOrURL (teacherInfoURL);
			} else {
				teacherjd = getJsonDataFromFileOrURL (teacherInfoFile);
			}
//			Debuger.Log ("teacherjd.count:" + teacherjd.Count);
			for (int i = 0; i < teacherjd.Count; i++) {
				TeacherBean db = new TeacherBean (teacherjd [i]);
				teacherList.Add (db);
//				Debuger.Log ("TestJsonFile========================== db:" + db.getName ());
			}
			return teacherList;
		}

		string getJsonFromURL (string url)
		{
			string result = "";
			try {
				WebClient MyWebClient = new WebClient ();
				MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
				Byte[] pageData = MyWebClient.DownloadData (url); //从指定网站下载数据 
				string pageHtml = Encoding.UTF8.GetString (pageData); //如果获取网站页面采用的是UTF-8，则使用这句
				result = pageHtml;	
			} catch (WebException webEx) {
				Debuger.Log (webEx.ToString ());
				result = "";
			}
			return result;
		}
			
		public JsonData getJsonDataFromFileOrURL (string fileNameOrURL)
		{
			JsonData jd = null;
			if (NetSDKManager.isloadResrouceFromURL && fileNameOrURL.StartsWith("http")) {
				string result = getJsonFromURL (fileNameOrURL);
				Debuger.Log (result);
				jd = JsonMapper.ToObject (result);

			} else {
				Debuger.Log ("==============2============" + fileNameOrURL);
				string json = this.getStringFromFile (fileNameOrURL);
				jd = JsonMapper.ToObject (json);
			}
			return jd;
		}



		public string getStringFromFile (string fileName)
		{
			try {
				if (!File.Exists (fileName)) {
					Debuger.LogError ("TestJsonFile========================== FileNotFount!" + fileName);
					return null;
				}
				StreamReader sr = new StreamReader (fileName);
				if (sr == null) {
					Debuger.LogError ("TestJsonFile========================== sr is null!");
					return null;
				}
				string json = sr.ReadToEnd ();
				return json;
			} catch (Exception ex) {
				Debuger.LogError (ex);

				Debuger.LogError ("TestJsonFile========================== " + ex.ToString ());
			}
			return null;
		}
	}
}

