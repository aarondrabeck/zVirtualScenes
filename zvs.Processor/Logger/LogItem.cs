
﻿using System;

namespace zvs.Processor.Logging
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
        public string Source { get; set; }
        public string Urgency { get; set; }

        public LogItem()
        {
            Datetime = DateTime.Now;
        }

        public LogItem(log4net.Core.LoggingEvent Event)
        {
            Datetime = Event.TimeStamp;
            Urgency = Event.Level.ToString();
            Description = Event.RenderedMessage;
            Source = Event.LoggerName;
        }

        public override string ToString()
        {
            return String.Format("{0:yyyy-MM-dd-hh:mm:ss:fff}|{1,6}|{2,-20}|{3}", Datetime, Urgency, Source, Description);
        }
    }
}