using LitJson;

public class StudentBean {
	private string id;
	private string gradeName;
	private string className;
	private int studentNo;
	private string studentName;

	public StudentBean(JsonData jd){
		gradeName = jd ["gradeName"].ToString();
		className = jd ["className"].ToString();
		studentNo = int.Parse(jd ["studentNo"].ToString());
		studentName = jd ["studentName"].ToString();
	}

	public string getId() {
		return id;
	}
	public void setId(string id) {
		this.id = id;
	}
	public string getGradeName() {
		return gradeName;
	}
	public void setGradeName(string gradeName) {
		this.gradeName = gradeName;
	}
	public string getClassName() {
		return className;
	}
	public void setClassName(string className) {
		this.className = className;
	}
	public int getStudentNo() {
		return studentNo;
	}
	public void setStudentNo(int studentNo) {
		this.studentNo = studentNo;
	}
	public void setStudentNo(string studentNo) {
		this.studentNo = int.Parse(studentNo);
	}
	public string getStudentName() {
		return studentName;
	}
	public void setStudentName(string studentName) {
		this.studentName = studentName;
	}
}
