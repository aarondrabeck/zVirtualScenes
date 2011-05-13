using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zVirtualScenesApplication
{
    public class RepollingTimer : System.Timers.Timer
    {
        public event RepollingTimerElapsedEventHandler RepollingTimerElapsed;
        public delegate void RepollingTimerElapsedEventHandler(byte node);        
        public byte NodeID {get;set;}  

        public RepollingTimer()
        {
            this.Elapsed += new System.Timers.ElapsedEventHandler(MyTimer_Elapsed);
        }

        void MyTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OnRepollingTimerElapsed(NodeID);           
        }

        protected virtual void OnRepollingTimerElapsed(byte node)
        {
            if (RepollingTimerElapsed != null)
                RepollingTimerElapsed(node);                    
        }
    }
}

