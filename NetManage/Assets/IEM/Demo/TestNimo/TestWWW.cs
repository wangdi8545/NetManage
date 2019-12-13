using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.Text;

public class TestWWW : MonoBehaviour
{

	string macaddress = "mac";
	string url_root = @"http://127.0.0.1:8080/queryByMac/";

	IEnumerator getJsonFromURLWWW ()
	{
		WWW www = new WWW (url_root + macaddress);
		Debug.Log (url_root + macaddress);
		yield return www;
		if (www.error == null) {
			Debug.Log (www.text);
		} else {
			Debug.Log ("ERROR: " + www.error);
		}        
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
			Debug.Log ("ERROR: ");
			result = "";
		}
		return result;
	}


	// Use this for initialization
	void Start ()
	{
//		StartCoroutine(getJsonFromURL());
		Debug.Log(getJsonFromURL(url_root + macaddress));

	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
