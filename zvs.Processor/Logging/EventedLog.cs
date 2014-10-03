using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net.Appender;
using log4net.Core;

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

        private static MemoryAppender _logger;
        private static bool _enabled = true;
        private static readonly object Lock = new object();
        public static bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                lock (Lock)
                {
                    _enabled = value;
                    if (_enabled)
                    {
                        if (_logger == null)
                        {
                            var l = LogManager.FindMemoryLogger();
                            if (l == null) return;
                            _logger = (MemoryAppender)l;
                            //log4net.Config.BasicConfigurator.Configure(logger);

                            // Since there are no events to catch on logging, we dedicate  
                            // a thread to watching for logging events  
                            _logWatcher = new Thread(new ThreadStart(LogWatcher));
                            _logWatcher.Start();
                        }
                    }
                    else
                    {
                        _logWatcher.Abort();
                    }
                }
            }
        }
        public static void Clear()
        {
            items.Clear();
            _logger.Clear();
            OnLogItemsCleared(null, new LogItemsClearedEventArgs());
        }

        private static Thread _logWatcher;

        static EventedLog()
        {
        }
        public static IEnumerable<LogItem> Items { get { return items; } }
        static readonly List<LogItem> items = new List<LogItem>();

        private static void LogWatcher()
        {
            // we loop until the Form is closed  
            while (_enabled)
            {
                var events = _logger.GetEvents();
                foreach (var logItem in events.Select(e => new LogItem(e)))
                {
                    items.Add(logItem);
                    Console.WriteLine(logItem.ToString());
                }
                _logger.Clear();
                if (events.Length > 0) OnLogItemArrived(null, new LogItemArrivedEventArgs(Items));
                // nap for a while, don't need the events on the millisecond.  
                Thread.Sleep(1000);

            }
        }

    }
}