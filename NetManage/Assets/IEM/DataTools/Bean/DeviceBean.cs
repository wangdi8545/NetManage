using LitJson;

public class DeviceBean {

	private string id;
	private string mac;
	private string deviceNo;
	private int studentEndNo;
	public DeviceBean(JsonData jd){
		mac = jd ["mac"].ToString();
		deviceNo = jd ["deviceNo"].ToString();
		studentEndNo = int.Parse(jd ["studentEndNo"].ToString());
	}
	public string getId() {
		return id;
	}
	public void setId(string id) {
		this.id = id;
	}
	public string getMac() {
		return mac;
	}
	public void setMac(string mac) {
		this.mac = mac;
	}
	public string getDeviceNo() {
		return deviceNo;
	}
	public void setDeviceNo(string deviceNo) {
		this.deviceNo = deviceNo;
	}
	public int getStudentEndNo() {
		return studentEndNo;
	}
	public void setStudentEndNo(int studentEndNo) {
		this.studentEndNo = studentEndNo;
	}
}
