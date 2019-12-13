using UnityEngine;
using System.Collections;

public class IEM_XMLBean  {
    public const int DeviceNumber=0;//设备号。IELSDK老版本
    public const int Name = 1;//当前用户名。IELSDK老版本
    public const int PadIP = 2;//padip
    public const int PcIP = 3;//padip 
    public const int TeacherName = 4;//教师名
    public const int GridName = 5;//年级
    public const int ClassName = 6;//班级
    public const int LessonName = 7;//课程名
    public const int Macid = 8;//MAC
    public const int ispadserver = 9;//MAC
    public const int Jurisdiction = 10;//Jurisdiction -- XMLInterface.ins().getXMLElements() 使用这个方法

    //....如果xml变更，依照如上
}
