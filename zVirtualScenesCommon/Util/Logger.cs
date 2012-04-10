using System;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Linq;

namespace zVirtualScenesCommon.Util
{
    public static class Logger
    {
        #region private Variables
        private static BindingList<LogItem> _log;
        // Flag to switch on/off console logging
        private static bool _logConsole = true;
        // default Log file name
        private static string _logFileName = "zVirtualScenes.log";
        // Max log lines we cache in our internal log
        private static int _cacheMaxLogLines = 2;
        // Min of logs we keep in our internal log after we wrote to the file
        private static int _cacheMinLogLines = 1;
        // Max file size in KB
        private static int _maxFileSize = 100;
        // Number of log files to keep
        private static int _maxFileCount = 5;
        
        private static string _logPath = null;
        #endregion local Variables

        public delegate void LogItemAddEventHandler(object sender, EventArgs e);
        public static event LogItemAddEventHandler LogItemPostAdd;

        #region Properties 
        public static bool LogConsole
        {
            get { return _logConsole; }
            set { _logConsole = value; }
        }
        public static string LogFileName
        {
            get { return _logFileName; }
            set { _logFileName = value; }
        }
        public static int CacheMaxLogLines
        {
            get { return _cacheMaxLogLines; }
            set
            {
                if (value < 1)
                    value = 1;
                // ensure Max is allways > min
                if (value < CacheMinLogLines)
                    CacheMinLogLines = value - 1;

                _cacheMaxLogLines = value;
            }
        }
        public static int CacheMinLogLines
        {
            get { return _cacheMinLogLines; }
            set
            {
                if (value < 0)
                    value = 0;

                // ensure Max is allways > min
                if (value > CacheMaxLogLines)
                    CacheMaxLogLines = value + 1;

                _cacheMinLogLines = value;
            }
        }
        public static int MaxFileSize
        {
            get { return _maxFileSize; }
            set { _maxFileSize = value; }
        }
        public static int MaxFileCount
        {
            get { return _maxFileCount; }
            set { _maxFileCount = value; }
        }
        public static LogItem[] Log
        {
            get
            {
                LogItem[] result = new LogItem[_log.Count];
                // we wont give full access to our internal log list
                lock (_log)
                    _log.CopyTo(result, 0);

                return result;
            }
        }
        #endregion Properties

        #region public methods
        public static void WriteToLog(Urgency urgency, string log, string source)
        {
            if (LogConsole)
                Console.WriteLine(log);

            LogItem item = new LogItem(urgency, log, source);

            if (_log == null)
                _log = new BindingList<LogItem>();

            BindingList<LogItem> copyLog = new BindingList<LogItem>();

            lock (_log)
            {
                _log.Add(item);
                // check if we reached upper level .. 
                if (_log.Count >= CacheMaxLogLines)
                {
                    // yes .. then take all until the lover level
                    for (int index = 0; index < CacheMaxLogLines - CacheMinLogLines; index++)
                    {
                        copyLog.Add(_log[0]);
                        _log.RemoveAt(0);
                    }
                }
            }
            // save the logs in a new thread
            if (copyLog.Count > 0)
            {
                if (CacheMaxLogLines == 1)
                {
                    SaveLogToFile(copyLog);
                }
                else
                {
                    new Thread(() =>
                        {
                            SaveLogToFile(copyLog);
                        }).Start();
                }
            }
            // Call delegate in case we have a subscriber
            if (LogItemPostAdd != null)
                LogItemPostAdd("Logger", new EventArgs());
        }

        public static LogItem GetLastEntry()
        {
            return _log[_log.Count - 1];
        }

        public static void SaveLogToFile()
        {
            SaveLogToFile(_log);
            _log.Clear();
        }
        #endregion public methods

        #region private methods
        private static void SaveLogToFile(BindingList<LogItem> logs)
        {
            // use filename to ensure just one thread at a time can access that region
            lock (_logFileName)
            {
                // Environment.CurrentDirectory returns wrong directory in Service env. so we have to make a trick
                if (_logPath == null)
                {
                    _logPath = System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                }

                string logFile = Path.Combine(_logPath, _logFileName);
                if (File.Exists(logFile) == true)
                {
                    // Check file size.
                    if (new FileInfo(logFile).Length >= MaxFileSize * 1024)
                    {
                        // move file
                        File.Move(logFile, logFile.Replace(".log", String.Format("_{0:yyyy-MM-dd-hhmmssfff}.log", DateTime.Now)));
                        // now check how much logfiles we have and delete the oldest
                        var fileList = from files in new DirectoryInfo(_logPath).GetFiles(_logFileName.Replace(".log", "_*.log"))
                                       orderby files.Name ascending
                                       select files;
                        if (fileList.Count() > MaxFileCount)
                        {
                            for (int index = 0; index < fileList.Count() - MaxFileCount; index++)
                            {
                                fileList.ElementAt(index).Delete();
                            }
                        }
                    }
                }
                using (TextWriter stream = new StreamWriter(logFile, true))
                {
                    foreach (LogItem item in logs)
                    {
                        stream.WriteLine(item.ToString());
                    }
                }
            }
        }
        #endregion private methods
    }
}
