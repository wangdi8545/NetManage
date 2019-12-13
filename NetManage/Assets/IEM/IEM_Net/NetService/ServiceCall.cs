using Assets.MessageLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.NetService
{
    public interface ServiceCall
    {
        JSONObject access(Message msg);
    }
}
