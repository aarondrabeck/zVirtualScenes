using System;
using System.ComponentModel;

namespace zVirtualScenesCommon.Util
{
    public static class Logger
    {
        public static BindingList<LogItem> Log;

        public delegate void LogItemAddEventHandler(object sender, EventArgs e);
        public static event LogItemAddEventHandler LogItemPostAdd; 

        public static void WriteToLog(Urgency urgency, string log, string source)
        {
            // TODO: Make this write to a log file somewhere instead of the console
            Console.WriteLine(log);
            
            LogItem item = new LogItem(urgency, log, source);
            
            if (Log == null)
                Log = new BindingList<LogItem>();

            lock (Log)
                Log.Add(item);

            LogItemPostAdd("Logger", new EventArgs());             
        }

        public static LogItem GetLastEntry()
        {
            return Log[Log.Count-1];
        }        

        public static void SaveLogToFile()
        {
            // TODO: DO STUFF HERE!
        }
    }
}
