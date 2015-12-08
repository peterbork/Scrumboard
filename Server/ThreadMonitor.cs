using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class ThreadMonitor {
        public delegate void ThreadEventType(string message);
        public event ThreadEventType ThreadEvent;

        public void ThreadAction(string message) {
            ThreadEvent(message);
        }
    }
}
