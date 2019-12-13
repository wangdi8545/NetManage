using Assets.MessageLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.NetService
{
    public interface Servicelet
    {
        void access(Message msg);
    }
}
