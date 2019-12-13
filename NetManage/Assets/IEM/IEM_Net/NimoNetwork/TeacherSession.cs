/*
 * Student Save the Teacher Status 
*/

using Assets.MessageLib;
using Assets.NetSession;
using System.Collections;
using System.Net.Sockets;

public class TeacherSession : Session
{
    public TeacherSession(TcpClient client) : base(client)
    {
    }
		
    public override bool isTeacher()
    {
        return true;
    }
}


