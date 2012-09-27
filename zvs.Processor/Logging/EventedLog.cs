using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Core;
using zvs.Processor.Logging;

namespace zvs.Processor.Logging
{
    public class EventedLog
    {
        public delegate void LogItemArrived(List<LogItem> NewItems);
        public static event LogItemArrived OnLogItemArrived;
        private static log4net.Appender.MemoryAppender logger;
        private static bool _Enabled = true;
        private static object _lock = new object();
        public static bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                lock (_lock)
                {
                    _Enabled = value;
                    if (_Enabled)
                    {
                        if (logger == null)
                        {
                            logger = new log4net.Appender.MemoryAppender();
                            log4net.Config.BasicConfigurator.Configure(logger);

                            // Since there are no events to catch on logging, we dedicate  
                            // a thread to watching for logging events  
                            logWatcher = new Thread(new ThreadStart(LogWatcher));
                            logWatcher.Start();
                        }
                        else
                        {
                            //logWatcher.Resume();
                        }
                    }
                    else
                    {
                        logWatcher.Abort();
                    }
                }
            }
        }
        public static void Clear()
        {
            items.Clear();
            logger.Clear();
        }
        private static Thread logWatcher;

        static EventedLog()
        {
        }
        public static List<LogItem> Items { get { return items; } }
        static List<LogItem> items = new List<LogItem>();
        public static void LogWatcher()
        {
            // we loop until the Form is closed  
            while (_Enabled)
            {
                LoggingEvent[] events = logger.GetEvents();
                foreach (var e in events)
                {
                    items.Add(new LogItem(e));
                }
                logger.Clear();
                if (items.Count > 0 && OnLogItemArrived != null) OnLogItemArrived(items);
                // nap for a while, don't need the events on the millisecond.  
                Thread.Sleep(1000);

            }
        }

    }
}