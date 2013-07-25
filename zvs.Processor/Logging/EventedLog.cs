using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;
using zvs.Processor.Logging;

namespace zvs.Processor.Logging
{
    public class EventedLog
    {
        public class LogItemsClearedEventArgs : EventArgs { }       
        public class LogItemArrivedEventArgs : EventArgs
        {
            public IEnumerable<LogItem> NewItems { get; private set; }
            public LogItemArrivedEventArgs(IEnumerable<LogItem> newItems)
            {
                NewItems = newItems;
            }
        }

        public delegate void LogItemArrivedHandler(object sender, LogItemArrivedEventArgs e);
        public static event LogItemArrivedHandler OnLogItemArrived = delegate { };

        public delegate void LogItemsClearedHandler(object sender, LogItemsClearedEventArgs e);
        public static event LogItemsClearedHandler OnLogItemsCleared = delegate { };

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
                            var l = LogManager.FindMemoryLogger();
                            if (l != null)
                            {
                                logger = (MemoryAppender)l;
                                    //log4net.Config.BasicConfigurator.Configure(logger);

                                // Since there are no events to catch on logging, we dedicate  
                                    // a thread to watching for logging events  
                                logWatcher = new Thread(new ThreadStart(LogWatcher));
                                logWatcher.Start();
                            }
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
            OnLogItemsCleared(null, new LogItemsClearedEventArgs());
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
                if (events.Length > 0) OnLogItemArrived(null, new LogItemArrivedEventArgs(Items));
                // nap for a while, don't need the events on the millisecond.  
                Thread.Sleep(1000);

            }
        }

    }
}