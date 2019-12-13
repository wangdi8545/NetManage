using Assets.NetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.MessageLib;
using Assets.Session;
using Assets.NimoNetwork;

namespace Assets.TestMessage
{
    public class GetStudentNumberServiceCall : ServiceCall
    {
        private static GetStudentNumberServiceCall instance = new GetStudentNumberServiceCall();
        public static void regist()
        {
            ServiceManager.registCall(Protocol.T_CALL_GETSTUDENTNUMBER, instance);
        }
         
        public JSONObject access(Message msg)
        {
            JSONObject parameter = msg.parameter;
            ISession session = msg.session;
            
			Debuger.Log ("Received a Student msg: " + msg+","+ session.isTeacher());
            JSONObject js = new JSONObject();
			js.AddField("studentnum", SessionManager.getStumapStudentSessions().Count);
            return js;
        }
    }
}
