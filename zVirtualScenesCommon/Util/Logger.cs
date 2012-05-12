using System;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;

namespace zVirtualScenesCommon.Util
{
    public static class Logger
    {
        #region private Variables
        public static ObservableCollection<LogItem> LOG = new ObservableCollection<LogItem>();
        // Flag to switch on/off console logging
        private static bool _logConsole = true;
        // default Log file name
        private static string _logFileName = "zVirtualScenes.log";
        // Max log lines we cache in our internal log
        private static int _cacheMaxLogLines = 1000;
        // Min of logs we keep in our internal log after we wrote to the file
        private static int _cacheMinLogLines = 50;
        // Max file size in KB
        private static int _maxFileSize = 100;
        // Number of log files to keep
        private static int _maxFileCount = 5;

        public static string LogPath
        {
            get
            {
                string path = Path.Combine(Paths.AppDataPath, @"logs\");
                if (!Directory.Exists(path))
                {
                    try { Directory.CreateDirectory(path); }
                    catch { }
                }
                return path + "\\";
            }
        }

        #endregion local Variables

       // public delegate void LogItemAddEventHandler(object sender, EventArgs e);
       // public static event LogItemAddEventHandler LogItemPostAdd;

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
                LogItem[] result = new LogItem[LOG.Count];
                // we wont give full access to our internal log list
                lock (LOG)
                    LOG.CopyTo(result, 0);

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
            ObservableCollection<LogItem> copyLog = new ObservableCollection<LogItem>();

            lock (LOG)
            {
                LOG.Add(item);
                // check if we reached upper level .. 
                if (LOG.Count >= CacheMaxLogLines)
                {
                    // yes .. then take all until the lover level
                    for (int index = 0; index < CacheMaxLogLines - CacheMinLogLines; index++)
                    {
                        copyLog.Add(LOG[0]);
                        LOG.RemoveAt(0);
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
        }

        public static LogItem GetLastEntry()
        {
            return LOG[LOG.Count - 1];
        }

        public static void SaveLogToFile()
        {
            SaveLogToFile(LOG);
            LOG.Clear();
        }
        #endregion public methods

        #region private methods
        private static void SaveLogToFile(ObservableCollection<LogItem> logs)
        {
            // use filename to ensure just one thread at a time can access that region
            lock (_logFileName)
            {
                // Environment.CurrentDirectory returns wrong directory in Service env. so we have to make a trick
                //if (_logPath == null)
                //{
                   // _logPath = Paths.AppDataPath;
                        //System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
               // }

                string logFile = Path.Combine(LogPath, _logFileName);
                if (File.Exists(logFile) == true)
                {
                    // Check file size.
                    if (new FileInfo(logFile).Length >= MaxFileSize * 1024)
                    {
                        // move file
                        File.Move(logFile, logFile.Replace(".log", String.Format("_{0:yyyy-MM-dd-hhmmssfff}.log", DateTime.Now)));
                        // now check how much logfiles we have and delete the oldest
                        var fileList = from files in new DirectoryInfo(LogPath).GetFiles(_logFileName.Replace(".log", "_*.log"))
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
