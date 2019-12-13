using Assets.MessageLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.NetService
{
    public class ServiceManager
    {
        public static void init()
        {
            
        }
        private static Dictionary<string,ServiceCall> callmap = new Dictionary<string, ServiceCall>();
//        private static Dictionary<string, Servicelet> letmap = new Dictionary<string, Servicelet>();
        public static bool registCall(String command,ServiceCall obj)
        {
            if (callmap.ContainsKey(command)) return false;
            callmap.Add(command, obj);
            return true;
        }
		public static bool deleteCall(String command)
		{
			lock (callmap) {
				if (!callmap.ContainsKey (command))
					return true;
				callmap.Remove (command);
				return true;
			}
		}
        //public static Boolean registlet(String command,Servicelet obj)
        //{
        //    if (letmap.ContainsKey(command)) return false;
        //    letmap.Add(command, obj);
        //    return true;
        //}
        /* 每隔一段时间检查一次消息队列，如果存在消息，拿出来解析*/
		public static void update(int type = 0)
        {
			Message msg = MessageManager.getRecrveMessage(type);
            int i = 0;
            while (msg != null)
            {
                try
                {
//					if (type == 1) {// only print the no unity message 
//						Debuger.Log("[ServiceManager.update] message:" + msg.parameter.ToString() );
//					}
                    if (msg.type == Message.Call)
                    {
                        call(msg);
                    }
                    else if (msg.type == Message.Return)
                    {
                        Return(msg);
                    }
                    else if (msg.type == Message.Send)
                    {
                        call(msg);
                    }
                    else if (msg.type == Message._Action)
                    {
                        _Action(msg);
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                if (i++ < 10) { 
					msg = MessageManager.getRecrveMessage(type);
                }else
                {
                    //每次update最多接受处理10条消息，超出的下次update执行
                    msg=null;
                }
            }
        }
        public static void call(Message msg)
        {
            if (callmap.ContainsKey(msg.command))
            {
                ServiceCall call = callmap[msg.command];
                JSONObject js = null;
                try
                {
                    js = call.access(msg);
                    //增加判断，消息来源如果是Send，那么不返回结果
                    if (msg.type == Message.Send) return;

                    if (js == null)
                    {
                        //Debuger.Log("无返回值调用！");
                        //无返回值也要发送
                        js = new JSONObject();
                    }
                }
                catch (Exception ex)
                {
					msg.session.sendErrorReturn(msg.mid, ex);
                }
                msg.session.sendReturn(msg.mid, js);
            }else
            {
                //没有服务处理类
                Debuger.LogError("不存在CALL服务处理类或处理类未注册："+msg.command);
				IEM_Log.ins.Log ("不存在CALL服务处理类或处理类未注册："+msg.command);
            }
        }
        public static void Return(Message msg)
        {
            //		查找CallBack的服务
            CallBack callback = CallBackManager.getCallBack(msg.mid);
            //		执行返回
            if(callback!=null) callback.Return(msg);
        }
		// 作废，保留以便以后使用
        public static void _Action(Message msg)
        {
			Debuger.Log ("_Action expire! 过期");
//            switch (msg.command)
//            {
//                case ActionCommand.OnConnect_com:
//                    Assets.NetService.NetSDKManager.OnConnect(msg.session);
//                    break;
//                case ActionCommand.OnDisConnect_com:
//                    Assets.NetService.NetSDKManager.OnDisConnect(msg.session);
//                    break;
//			case ActionCommand.OnFinishImageTrans_fake:
//				Debuger.Log ("UI Thread: TODO" + msg.parameter.ToString());
//					break;
//                default:
//                    break;
//            }
        }

        //public static void let(Message msg)
        //{
        //    if (letmap.ContainsKey(msg.command))
        //    {
        //        Servicelet let = letmap[msg.command];
        //        let.access(msg);
        //    }
        //    else
        //    {
        //        //没有服务处理类
        //        Debuger.LogError("不存在Servicelet服务处理类或处理类未注册：" + msg.command);
        //    }
        //}
    }
}
