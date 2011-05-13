using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zVirtualScenesApplication
{
    public class RepollingTimer : System.Timers.Timer
    {
        public delegate void RepollingTimerElapsedEventHandler(byte node);
        public event RepollingTimerElapsedEventHandler RepollingTimerElapsed;
        public byte _node {get;set;}  

        public RepollingTimer()
        {
            this.Elapsed += new System.Timers.ElapsedEventHandler(MyTimer_Elapsed);
        }

        void MyTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OnRepollingTimerElapsed(_node);           
        }

        protected virtual void OnRepollingTimerElapsed(byte node)
        {
            if (RepollingTimerElapsed != null)
                RepollingTimerElapsed(node);                    
        }
    }

    public class RepollingTimerArgs
    {
       

        public byte node { get; set; }
    }
}

