using UnityEngine;
using System.Collections;

public class LoadTxtWithResources  {
    public static string ResLoadWithUnity(string path) {        
       return Resources.Load(path).ToString();
    }
}
