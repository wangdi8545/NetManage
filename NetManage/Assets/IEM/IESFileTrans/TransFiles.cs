using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace IESFileTrans
{
	public class TransferFiles
	{
		public static bool SendVarData (Socket s, byte[] data)
		{
			int total = 0;
			int size = data.Length;
			int dataleft = size;
			int sent;
			byte[] datasize = new byte[4];

			try {
				datasize = BitConverter.GetBytes (size);
				sent = s.Send (datasize);

				while (total < size) {
					sent = s.Send (data, total, dataleft, SocketFlags.None);
					total += sent;
					dataleft -= sent;
				}

				return true;
			} catch(Exception ex) {
				Debuger.Log ("TransferFiles.SendVarData Error" + ex.ToString());
				return false;

			}
		}

		public static byte[] ReceiveVarData (Socket s)
		{

			int size = 4;
			int dataleft = size;
			byte[] data = new byte[size];
			int total = 0, recv;
			while (total < size) {
				recv = s.Receive (data, total, dataleft, SocketFlags.None);
				if (recv == 0) {
					Thread.Sleep (20);
				}
				total += recv;
				dataleft -= recv;
			}



			size = BitConverter.ToInt32 (data, 0);
			dataleft = size;
			data = new byte[size];
			total = 0;
			while (total < size) {
				recv = s.Receive (data, total, dataleft, SocketFlags.None);
				if (recv == 0) {
					Thread.Sleep (20);
				}
				total += recv;
				dataleft -= recv;
			}
			return data;
		}
	}
}

