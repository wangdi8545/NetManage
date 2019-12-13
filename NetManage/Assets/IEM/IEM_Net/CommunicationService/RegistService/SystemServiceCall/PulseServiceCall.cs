using Assets.NetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.MessageLib;
using Assets.Session;

namespace Assets.TestMessage
{
	public class PulseServiceCall : ServiceCall
	{
		private static PulseServiceCall instance = new PulseServiceCall();
		public static void regist()
		{
			ServiceManager.registCall(Protocol._PULSE, instance);
		}

		public JSONObject access(Message msg)
		{
			JSONObject parameter = msg.parameter;
			ISession session = msg.session;

//			Debuger.Log ("[PulseServiceCall] Received a Callback");
			JSONObject js = new JSONObject();
            js.AddField("flag",((Assets.NetSession.Session)(msg.session)).IsConnect?"0":"1");
			return js;
		}
	}
}
