using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.TestMessage
{
    public class ServiceRegistManager
    {
        static ServiceRegistManager instance = new ServiceRegistManager();
        public static ServiceRegistManager getInstance()
        {
            return instance;
        }
        public void initTeacherServiceRegist()
        {
			PulseServiceCall.regist ();
			FirstConnectCall.regist ();
        }
        public void initStudentServiceRegist()
        {
			GetScreenShotImageCall.regist ();
			GetScreenShotThumbnailCall.regist ();
        }
    }
}
