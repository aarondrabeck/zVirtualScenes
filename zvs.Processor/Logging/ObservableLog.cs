using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using log4net.Core;

namespace zvs.Processor.Logging
{
    public class ObservableLog
    {
        private readonly log4net.Appender.MemoryAppender _logger;
        private bool _logWatching = true;
        readonly ObservableCollection<LogItem> _log;
        readonly System.Windows.Threading.Dispatcher _threadDispatcher;

        private string Filter { get; set; }

        public ObservableLog(System.Windows.Threading.Dispatcher threadDispatcher)
        {
            _threadDispatcher = threadDispatcher;
            _threadDispatcher.ShutdownStarted += ThreadDispatcher_ShutdownStarted;
            _log = new ObservableCollection<LogItem>();
            _logger = new log4net.Appender.MemoryAppender();
            log4net.Config.BasicConfigurator.Configure(_logger);

            // Since there are no events to catch on logging, we dedicate  
            // a thread to watching for logging events  
            var logWatcher = new Thread(LogWatcher);
            logWatcher.Start();
        }

        void ThreadDispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Stop();
        }

        private void Stop()
        {
            _logWatching = false;
        }

        private void LogWatcher()
        {
            // we loop until the Form is closed  
            while (_logWatching)
            {
                var events = _logger.GetEvents();
                var items = new ObservableCollection<LogItem>();
                foreach (var e in events.Where(e => !string.IsNullOrEmpty(Filter) && (Filter.Contains(e.Level.ToString().ToUpper()) || Filter.Contains("ALL"))))
                {
                   items.Add(new LogItem(e));
                }               

                if (events != null && events.Length > 0 && !_threadDispatcher.HasShutdownStarted)
                {
                    _threadDispatcher.Invoke(new Action(() =>
                    {
                        foreach (var e in items.Where(e => _logWatching))
                        {
                            _log.Add(e);
                        }
                        
                    }));
                }
                _logger.Clear();
                // nap for a while, don't need the events on the millisecond.  
                Thread.Sleep(1000);
            }
        }
    }
}