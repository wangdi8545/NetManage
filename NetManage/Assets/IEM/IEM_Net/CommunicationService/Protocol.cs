using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.TestMessage
{
    public class Protocol
    {
        /**
         * 带返回调用，学生机获取当前教师机的学生数量
         */
        public const string T_CALL_GETSTUDENTNUMBER = "T_CALL_GETSTUDENTNUMBER";
        /**
         * 询问学生姓名
         */
        public const string S_CALL_GETSTUDENTNAME = "S_CALL_GETSTUDENTNAME";
		/**
         * 心跳
         */
		public const string _PULSE = "Pulse";
		/**
		 * 获得截图缩略图列表
		 */
		public const string _GETTHUMBNAIL = "T_CALL_GETTHUMBNAIL";
		/**
		 * 获得截图原图
		 */
		public const string _GETIMAGE = "T_CALL_GETIMAGE";
		/**
		 * 学生机第一次连接需要发送一些信息，包括Mac地址
		 */
		public const string _FirstConnectCall = "T_CALL_FIRSTCONNECTCALL";

    }
}
