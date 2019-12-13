using System;
using System.Collections.Generic;
using System.Collections;
namespace Data
{
	public class DataManager
	{
		private static DataManager instance=new DataManager();
		public static DataManager getInstance(){
			return instance;
		}
		private Dictionary<string,DeviceBean> devMap=new Dictionary<string, DeviceBean>();
		private Dictionary<string,Dictionary<int,StudentBean>> studentMap=new Dictionary<string, Dictionary<int,StudentBean>>();
		/** 科目 映射*/
		private Dictionary<string,ArrayList> teacherMap=new Dictionary<string, ArrayList>();
		/** 科目年级 映射*/
		private Dictionary<string,ArrayList> teacherMap1=new Dictionary<string, ArrayList>();
		private string nowClassKey;
		private Dictionary<int,StudentBean> nowClassStudent;
		public DataManager ()
		{
			
		}

		public Dictionary<int, StudentBean> NowClassStudent {
			get {
				return nowClassStudent;
			}
		}
        public Dictionary<int,DeviceBean> AllMacid
        {
           
            get
            {
                Dictionary<int, DeviceBean> temp = new Dictionary<int, DeviceBean>();
                int i = 1;
                foreach (DeviceBean item in DataLoadManager.getInstance().macs)
                {
                    temp.Add(i,item);
                    i++;
                }
                return temp;
            }
        }

        public void load(string gradeName,string className)
		{
            Debuger.Log("==="+gradeName+ className);
			this.registDevice(DataLoadManager.getInstance().loadMacInfo());
			this.registStudent(DataLoadManager.getInstance().loadStudentInfo());
			this.registTeacher(DataLoadManager.getInstance().loadTeacherInfo());
			setNowClassInfo (gradeName, className);
		}


		public int getDeviceIdFromMac(string mac){
			if (devMap.ContainsKey (mac)) {
				return devMap [mac].getStudentEndNo ();
			}
			return -1;
		}
        /**
		 * 设置当前上课的年级和班级
		*/
        public void setNowClassInfo(string gradeName, string className) {
        
            nowClassKey = getMyKey(gradeName, className);
            if (studentMap.ContainsKey(nowClassKey))
            {
                nowClassStudent = studentMap[nowClassKey];
            }
            else {
                nowClassStudent = new Dictionary<int, StudentBean>();
            }                          			
		}
		/**
		 * 根据Mac获取学生信息StudentBean
		*/
		public StudentBean getNowClassStudentNameByMac(string mac){
			if (!devMap.ContainsKey (mac)) {
				return null;
			}
			int studentNo = devMap [mac].getStudentEndNo ();
            if (nowClassStudent.ContainsKey(studentNo))
            {
                return nowClassStudent[studentNo];
            }
            return null;
		}
		/**
		 * 根据学号获取学生姓名StudentBean
		*/
		public StudentBean getNowClassStudentNameByNo(int studentNo){
			if (!nowClassStudent.ContainsKey (studentNo)) {
				return null;
			}
			return nowClassStudent [studentNo];
		}
		/**
		 * 根据年级获取教师列表ArrayList<TeacherBean>
		*/
		public ArrayList getTeacherList(string subject){
			if (!teacherMap.ContainsKey (subject)) {
				return null;
			}
			return teacherMap [subject];
		}
		/**
		 * 根据科目，年级获取教师列表ArrayList<TeacherBean>
		*/
		public ArrayList getTeacherList(string subject,string gradeName){
			string temp = getMyKey (subject, gradeName);
			if (!teacherMap.ContainsKey (temp)) {
				return null;
			}
			return teacherMap [temp];
		}
		/**
		 * 注册设备
		*/
		public bool registDevice(ArrayList list){
			for (int i = 0; i < list.Count; i++) {
				DeviceBean db = (DeviceBean)list [i];
				devMap [db.getMac ()] = db;
			}
			return true;
		}
		/**
		 * 注册学生
		*/
		public bool registStudent(ArrayList list){
			for (int i = 0; i < list.Count; i++) {
				StudentBean db = (StudentBean)list [i];
				string key = getMyKey (db.getGradeName (), db.getClassName ());

				Dictionary<int,StudentBean> tempMap;
				if (studentMap.ContainsKey (key)) {
					tempMap = studentMap [key];
				} else {
					tempMap = new Dictionary<int, StudentBean> ();
					studentMap [key] = tempMap;
				}
				tempMap [db.getStudentNo ()] = db;
			}
			return true;
		}
		/**
		 * 注册教师
		*/
		public bool registTeacher(ArrayList list){
			for (int i = 0; i < list.Count; i++) {
				TeacherBean db = (TeacherBean)list [i];
				ArrayList tempList = getOrCreateList (teacherMap,db.getSubject());
				tempList.Add (db);
				tempList = getOrCreateList (teacherMap1,getMyKey(db.getSubject(),db.getGradeName()));
				tempList.Add (db);
			}
			return true;
		}
		/**获取合成Key*/
		public string getMyKey(string key1,string key2){
			return key1 + "_" + key2;
		}
		public ArrayList getOrCreateList(Dictionary<string,ArrayList> map,string key){
			if (!map.ContainsKey(key)) {
				map [key] = new ArrayList ();
			}
			return map [key];
		}
	}
}

