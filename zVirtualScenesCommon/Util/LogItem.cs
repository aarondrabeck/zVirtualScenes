using System;

namespace zVirtualScenesCommon.Util
{    
    public class LogItem
    {
        public DateTime Datetime { get; set; }

        public string DatetimeLog
        {
            get
            {
                return this.Datetime.ToString("MM/dd/yyyy HH:mm:ss fff tt");
            }
        }
        
        public string Description { get; set; }

        public Urgency Urgency { get; set; }

        public string Source {get; set;}

        public LogItem()
        {
            Datetime = DateTime.Now;
        }

        public LogItem(Urgency urgency, string desc, string source)
        {
            Datetime = DateTime.Now;
            Urgency = urgency;
            Description = desc;
            Source = source;
        }

        public override string ToString()
        {
            return Datetime.ToString("s") + " | " + Urgency + " | " + Source + ":" + Description + Environment.NewLine;
        }
    }
}
