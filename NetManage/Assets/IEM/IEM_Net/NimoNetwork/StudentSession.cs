using Assets.NetSession;
using System.Collections;
using System.Net.Sockets;

public class StudentSession : Session
{
	// === Delete ???
	private int deviceNumber;
	private string deviceName;
	// === 

	StudentBean _studentBean;

	private bool isFirstLogout = true;

	public bool IsFirstLogout {
		get {
			return isFirstLogout;
		}
		set {
			isFirstLogout = value;
		}
	}

    public int DeviceNumber {
		get {
			return deviceNumber;
		}
		set {
			deviceNumber = value;
		}
	}

	public string DeviceName {
		get {
			return deviceName;
		}
		set {
			deviceName = value;
		}
	}

	public StudentBean thisStudentBean {
		get {
			return _studentBean;
		}
		set {
			_studentBean = value;
		}
	}		
		
    public StudentSession(TcpClient client) : base(client)
    {
		deviceNumber = -1;
		deviceName = "";
		_studentBean = null;
    }

    public override bool isTeacher()
    {
        return false;
    }
}


