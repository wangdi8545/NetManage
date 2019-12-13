using System.Collections;
using System.IO;

public class IEM_FileTools {

    public static void FielCopyCreat(string OutFilePath, string message)
    {
        StreamWriter t_sStreamWriter; // 文件流信息
        FileInfo t_fFileInfo = new FileInfo(OutFilePath);
        if (!t_fFileInfo.Exists)
        {
            t_sStreamWriter = t_fFileInfo.CreateText();  // 如果此文件不存在则创建
        }
        else
        {
            t_sStreamWriter = t_fFileInfo.AppendText(); // 如果此文件存在则打开
        }
        t_sStreamWriter.WriteLine(message); // 以行的形式写入信息 
        t_sStreamWriter.Close(); //关闭流
        t_sStreamWriter.Dispose(); // 销毁流
    }
}
