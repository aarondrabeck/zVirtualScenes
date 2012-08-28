using System;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Collections.Generic;

namespace zvs.Processor
{
    public class Logger
    {
        public ObservableCollection<LogItem> LOG;

        #region private Variables

        /// <summary>
        /// Flag to switch on/off console logging
        /// </summary>
        private bool _logConsole;

        /// <summary>
        /// default Log file name
        /// </summary>
        private string _logFileName = "zVirtualScenes.log";

        /// <summary>
        /// Max log lines we cache in our internal log
        /// </summary>
        private int _cacheMaxLogLines;

        /// <summary>
        /// Min of logs we keep in our internal log after we wrote to the file
        /// </summary>
        private int _cacheMinLogLines;

        /// <summary>
        ///  Max file size in KB
        /// </summary>        
        private int _maxFileSize;

        /// <summary>
        ///  Number of log files to keep
        /// </summary>
        private int _maxFileCount;

        public static string LogPath
        {
            get
            {
                string path = Path.Combine(Utils.AppDataPath, @"logs\");
                if (!Directory.Exists(path))
                {
                    try { Directory.CreateDirectory(path); }
                    catch { }
                }
                return path + "\\";
            }
        }

        #endregion

        public Logger()
        {
            LOG = new ObservableCollection<LogItem>();
            _logConsole = true;
            _logFileName = "zVirtualScenes.log";
            _cacheMaxLogLines = 1000;
            _cacheMinLogLines = 50;
            _maxFileSize = 100;
            _maxFileCount = 5;
        }

        #region Properties
        public bool LogConsole
        {
            get { return _logConsole; }
            set { _logConsole = value; }
        }
        public string LogFileName
        {
            get { return _logFileName; }
            set { _logFileName = value; }
        }
        public int CacheMaxLogLines
        {
            get { return _cacheMaxLogLines; }
            set
            {
                if (value < 1)
                    value = 1;
                // ensure Max is always > min
                if (value < CacheMinLogLines)
                    CacheMinLogLines = value - 1;

                _cacheMaxLogLines = value;
            }
        }
        public int CacheMinLogLines
        {
            get { return _cacheMinLogLines; }
            set
            {
                if (value < 0)
                    value = 0;

                // ensure Max is always > min
                if (value > CacheMaxLogLines)
                    CacheMaxLogLines = value + 1;

                _cacheMinLogLines = value;
            }
        }
        public int MaxFileSize
        {
            get { return _maxFileSize; }
            set { _maxFileSize = value; }
        }
        public int MaxFileCount
        {
            get { return _maxFileCount; }
            set { _maxFileCount = value; }
        }
        #endregion Properties

        #region public methods
        public void WriteToLog(Urgency urgency, string log, string source)
        {
            
            if (LogConsole)
                Console.WriteLine(log);
          
            LogItem item = new LogItem(urgency, log, source);
            List<LogItem> copyLog = new List<LogItem>();

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

            // save the logs in a async
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

        public LogItem GetLastEntry()
        {
            return LOG[LOG.Count - 1];
        }

        public void SaveLogToFile()
        {
            SaveLogToFile(LOG.ToList());
            LOG.Clear();
        }
        #endregion public methods

        #region private methods
        private async void SaveLogToFile(List<LogItem> logs)
        {
            string logFile = Path.Combine(LogPath, _logFileName);
           
            if (File.Exists(logFile))
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
                    await stream.WriteLineAsync(item.ToString());
                }
            }
        }
        #endregion private methods
    }
}
