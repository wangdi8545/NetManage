using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.IEM.IEM_Net.Net.Lock
{
    class MyLock
    {
        private bool b = true;
        public bool getLock(){
            if (!b) return false;
            lock (this) { 
                if(!b) return false;
                b=false;
                return true;
            }
        }
        public bool returnLock()
        {
            lock (this)
            {
                if (b) return false;
                b = true;
                return b;
            }
        }
    }
}
