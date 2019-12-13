using System;
using System.Net;
using System.Net.NetworkInformation;


public class IEM_Tools
{
	static string _macAddress;

	public static string getMacAddress() {
		return _macAddress;
	}

	static public void init(){				
		_macAddress = XMLInterface.ins ().Macid;
		Debuger.Log ("mac init:" + _macAddress);
	}
	static string getMacAddressPC ()
	{
		string macAddress = "";
		NetworkInterface[] networkinterfaces = NetworkInterface.GetAllNetworkInterfaces ();
		foreach (NetworkInterface networkinterface in networkinterfaces) {
			PhysicalAddress physicalAddress = networkinterface.GetPhysicalAddress ();
			if (!physicalAddress.ToString ().Equals (String.Empty)) {
				macAddress = physicalAddress.ToString ();
				return macAddress;
			}
		}
		return "Error";
	}
}


