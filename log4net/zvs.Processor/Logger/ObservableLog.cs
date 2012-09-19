using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Core;

namespace zvs.Processor.Logging
{
    public class ObservableLog
    {
        private log4net.Appender.MemoryAppender logger;
        private bool logWatching = true;
        ObservableCollection<LogItem> _Log;
        public ObservableCollection<LogItem> Log { get { return _Log; } }
        private Thread logWatcher;
        System.Windows.Threading.Dispatcher ThreadDispatcher;

        public string Filter { get; set; }

        public ObservableLog(System.Windows.Threading.Dispatcher ThreadDispatcher)
        {
            this.ThreadDispatcher = ThreadDispatcher;
            this.ThreadDispatcher.ShutdownStarted += ThreadDispatcher_ShutdownStarted;
            _Log = new ObservableCollection<LogItem>();
            logger = new log4net.Appender.MemoryAppender();
            log4net.Config.BasicConfigurator.Configure(logger);

            // Since there are no events to catch on logging, we dedicate  
            // a thread to watching for logging events  
            logWatcher = new Thread(new ThreadStart(LogWatcher));
            logWatcher.Start();
        }

        void ThreadDispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Stop();
        }
        public void Stop()
        {
            logWatching = false;
        }

        private void LogWatcher()
        {
            // we loop until the Form is closed  
            while (logWatching)
            {
                LoggingEvent[] events = logger.GetEvents();
                ObservableCollection<LogItem> items = new ObservableCollection<LogItem>();
                foreach (var e in events)
                {
                    if (!string.IsNullOrEmpty(Filter) && (Filter.Contains(e.Level.ToString().ToUpper()) || Filter.Contains("ALL")))
                    {
                        items.Add(new LogItem(e));
                    }
                }               

                if (events != null && events.Length > 0 && !this.ThreadDispatcher.HasShutdownStarted)
                {
                    this.ThreadDispatcher.Invoke(new Action(() =>
                    {
                        foreach (var e in items)
                        {
                            if (logWatching)
                            {
                                _Log.Add(e);
                            }
                        }
                        
                    }));
                }
                logger.Clear();
                // nap for a while, don't need the events on the millisecond.  
                Thread.Sleep(1000);
            }
        }
    }
}