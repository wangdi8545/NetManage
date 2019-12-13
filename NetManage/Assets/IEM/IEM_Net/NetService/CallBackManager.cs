using Assets.MessageLib;
using Assets.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.NetService
{
    public class CallBackManager
    {
        static Dictionary<string, CallBack> callbackMap = new Dictionary<string, CallBack>();
        public static void putCallBack(CallBack callback)
        {
            lock (callbackMap)
            {
				if (callbackMap.ContainsKey (callback.getMessage ().mid)) {
					return;
				}
                callbackMap.Add(callback.getMessage().mid, callback);
            }
        }
        public static CallBack getCallBack(String mid)
        {
            lock (callbackMap)
            {
                if (callbackMap.ContainsKey(mid))
                {
                    CallBack callback = callbackMap[mid];
//					PERSIST表示不会被销毁的Return，用于客户端教师端上下线使用
					if (mid.StartsWith ("PERSIST_")) {
//						Debuger.Log ("Persist 测试:" + mid);
					} else {
//						Debuger.Log ("@@@ delete :" + mid);
						callbackMap.Remove(mid);
					}
                    return callback;
                }
                return null;
            }
        }
        /**
         * 未处理的消息重发
         */
        public static void addMessageToQueueBySession(ISession session)
        {
            lock (callbackMap)
            {
                foreach (KeyValuePair<string, CallBack> entry in callbackMap)
                {
                    try
                    {
                        CallBack callback = entry.Value;
                        if (session.Equals(callback.getMessage().session))
                        {
                            //MessageManager.addRecrveMessage(callback.getMessage());
                            MessageManager.putSendMessage(callback.getMessage());
                        }
                    }
                    catch (Exception ex)
                    {
						Debuger.LogError("[CallBackManager.addMessageToQueueBySession]"+ex.ToString());
                    }
                }
            }
        }
        /**
         * 清理未处理的Callback
         */
        public static void clearCallBackBySession(ISession session)
        {
            lock (callbackMap)
            {
                List<string> removeKeys = new List<string>();
                foreach (KeyValuePair<string, CallBack> entry in callbackMap)
                {
                    try
                    {
                        CallBack callback = entry.Value;
                        if (session.Equals(callback.getMessage().session))
                        {
                            removeKeys.Add(entry.Key);
                        }
                    }
                    catch (Exception ex)
                    {
						Debuger.LogError("[CallBackManager.clearCallBackBySession]"+ex.ToString());
                    }
                }
                foreach (string key in removeKeys)
                {
                    callbackMap.Remove(key);
                }
            }
        }
    }
}
