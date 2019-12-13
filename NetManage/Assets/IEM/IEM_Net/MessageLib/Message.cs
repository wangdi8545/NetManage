using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Session;

namespace Assets.MessageLib
{
    public class Message
    {
        public const int Call = 1;
        public const int Return = 2;
        public const int Send = 3;
        public const int _Action = 11;
        public const string TYPE = "NetMessageType";
        public const string MID = "NetMessageMid";
        public const string COMMAND = "NetMessageCommand";
        public int type;
        public String mid;
        public String command;
        public JSONObject parameter;
        public ISession session;
        private long createTime= System.DateTime.Now.Ticks / 10000;

        public Message(){}
        public Message(JSONObject js,ISession session)
        {
            parameter = js;
			if (session != null) {
				this.session = session;
			} else {
				this.session = null;
			}
            if (parameter != null)
            {
                if (parameter[TYPE] != null)
                {
                    type = (int)parameter.GetField(TYPE).i;
                }
                if (parameter[MID] != null)
                {
                    mid = parameter.GetField(MID).str;
                }
                if (parameter[COMMAND] != null)
                {
                    command = parameter.GetField(COMMAND).str;
                }
            }
            else
            {
                //LableString.addtolable("收到的消息参数为空：" + this.ToString());
            }
            ////LableString.addtolable("收到消息："+this.ToString());
        }
        public bool isTimeOut()
        {
            return (createTime > (System.DateTime.Now.Ticks / 10000) + (1000 * 60));
        }
        public void create()
        {
			parameter.SetField(TYPE, type);
			parameter.SetField(MID, mid);
			parameter.SetField(COMMAND, command);
        }
        internal JSONObject toJson()
        {
            return parameter;
        }
        /**
         * 将对象转化成字节流
         */
        public byte[] toBytes()
        {
            return PackageTools.toPackageBytes(parameter.ToString());
        }
        public override string ToString()
        {
            return ""+mid+","+command+","+type+","+parameter;
        }
    }
}
 