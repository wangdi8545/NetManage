using System.Collections;
using LitJson;

public class TeacherBean {
	private string id;
	private string gradeName;
	private string Subject;
	private string name;
	public TeacherBean(JsonData jd){
		gradeName = jd ["gradeName"].ToString();
		Subject = jd ["subject"].ToString();
		name = jd ["name"].ToString();
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
	public string getName() {
		return name;
	}
	public void setName(string name) {
		this.name = name;
	}
	public string getSubject() {
		return Subject;
	}
	public void setSubject(string subject) {
		Subject = subject;
	}
}
