using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.MessageLib
{
    public class PackageTools
    {
        public static byte[] toPackageBytes(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            byte[] dataLength = BitConverter.GetBytes(data.Length);
            byte[] package = new byte[4 + data.Length];
            dataLength.CopyTo(package, 0);
            data.CopyTo(package, 4);
            return package;
        }
        public static string fromBytes(byte[] data)
        {
            return null;
        }
    }
}
