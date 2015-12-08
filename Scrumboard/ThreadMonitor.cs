using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumboard {
    class ThreadMonitor {
        public delegate void ThreadEventType(string message);
        public event ThreadEventType ThreadEvent;

        string Name;

        public ThreadMonitor(string name) {
            this.Name = name;
        }

        public void ThreadAction(string message) {
            ThreadEvent(message);
        }

    }
}
