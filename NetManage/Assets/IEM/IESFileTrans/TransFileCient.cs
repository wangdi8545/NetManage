using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace IESFileTrans
{
	public static class TransFileCient
	{
		static string IP = Assets.NetService.NetSDKManager.ip; 
		static int Port = 8225;

		private static bool SendFile(Socket client, string fullPath)
		{
			bool flag = true;
			//包的大小
			int PacketSize = 1000;
			//创建一个文件对象
			FileInfo EzoneFile = new FileInfo (fullPath);
			//打开文件流
			FileStream EzoneStream = EzoneFile.OpenRead ();
			//包的数量
			int PacketCount = (int)(EzoneStream.Length / ((long)PacketSize));
			//最后一个包的大小
			int LastDataPacket = (int)(EzoneStream.Length - ((long)(PacketSize * PacketCount)));

			//获得客户端节点对象
			//IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;

			//发送[文件名]到客户端
			flag = TransferFiles.SendVarData (client, System.Text.Encoding.Unicode.GetBytes (EzoneFile.Name));
			if (!flag)
				return false;
			//发送[包的大小]到客户端
			flag = TransferFiles.SendVarData (client, System.Text.Encoding.Unicode.GetBytes (PacketSize.ToString ()));
			if (!flag)
				return false;
			//发送[包的总数量]到客户端
			flag = TransferFiles.SendVarData (client, System.Text.Encoding.Unicode.GetBytes (PacketCount.ToString ()));
			if (!flag)
				return false;
			//发送[最后一个包的大小]到客户端
			flag = TransferFiles.SendVarData (client, System.Text.Encoding.Unicode.GetBytes (LastDataPacket.ToString ()));
			if (!flag)
				return false;

			//数据包
			byte[] data = new byte[PacketSize];
			//开始循环发送数据包
			for (int i = 0; i < PacketCount; i++) {
				//从文件流读取数据并填充数据包
				EzoneStream.Read (data, 0, data.Length);
				//发送数据包
				flag = TransferFiles.SendVarData (client, data);
				if (!flag) {
					break;
				}
			}

			//如果还有多余的数据包，则应该发送完毕！
			if (flag && LastDataPacket != 0) {
				data = new byte[LastDataPacket];
				EzoneStream.Read (data, 0, data.Length);
				flag = TransferFiles.SendVarData (client, data);
			}
			//关闭文件流
			EzoneStream.Close ();
			return flag;
		}

		public static bool SendFiles (List<string> fullPaths)
		{
			//指向远程服务端节点
			IPEndPoint ipep = new IPEndPoint (IPAddress.Parse (IP), Port);
			//创建套接字
			Socket client = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//连接到发送端
			try {
				client.Connect (ipep);
				Debuger.Log ("连接服务器成功！");
			} catch {
				Debuger.Log ("连接服务器失败！");
				client.Close ();
				client = null;
				return false;
			}

			// 发送文件个数
			bool flag = false;
			flag = TransferFiles.SendVarData (client, System.Text.Encoding.Unicode.GetBytes (fullPaths.Count.ToString()));
			if (!flag) {
				client.Close ();
				client = null;
				return false;
			}

			//发送文件
			foreach (string fullPath in fullPaths) {
				flag = false;
				flag = SendFile (client, fullPath);
				if (!flag) {
					client.Close ();
					client = null;
					return false;
				}

			}

			//关闭套接字
			client.Close ();
			client = null;
			return true;
		}
	}
}

