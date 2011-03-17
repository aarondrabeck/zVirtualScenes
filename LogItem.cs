using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zVirtualScenesApplication
{
    public enum UrgencyLevel
    {
        INFO = 1,
        ERROR = 2,
        WARNING = 3
    }    

    class LogItem
    {
        public DateTime datetime { get; set; }
        
        public string description { get; set; }

        public UrgencyLevel urgency { get; set; }

        public string InterfaceName {get; set;}

        public LogItem()
        {
            this.datetime = DateTime.Now;
        }

        public override string ToString()
        {
            return this.datetime.ToString("s") + " | " + this.urgency.ToString() + " | " + InterfaceName + ":" + this.description + Environment.NewLine;
        }
    }
}
