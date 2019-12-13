using Assets.MessageLib;
using Assets.NetService;
using System;

namespace Assets.Session
{
    public interface ISession
    {

        void sendCall(String command, JSONObject data, Action<Message> incallback);
        void send(string command, JSONObject data);
        void sendReturn(string mid, JSONObject str);
        void sendErrorReturn(string mid, Exception ex);
        bool isTeacher();
        void sendToNet(Message msg);
        string getIpAddress();
		string getMacAddress();
		void setMacAddress(string address);

    }
}