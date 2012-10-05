using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Repository;

namespace zvs.Processor.Logging
{
    public class LogManager
    {
        public static void ConfigureLogging()
        {
            //nunit friendly way to get the loggin config file
            string dll = typeof(LogManager).Assembly.CodeBase;
            System.Uri u;
            System.Uri.TryCreate(dll, UriKind.RelativeOrAbsolute, out u);

            System.IO.FileInfo fi = new System.IO.FileInfo(u.LocalPath);
            string file = "log4net.config";
            if (Utils.DebugMode) file = "log4net.config";
            
            string filename = System.IO.Path.Combine(fi.Directory.FullName, file);
            if (System.IO.File.Exists(filename))
            {
                System.IO.FileInfo configFile = new System.IO.FileInfo(filename);
                log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);
            }
            EventedLog.Enabled = true;
        }
        public static Logging.ILog GetLogger<T>()
        {
            return new Logging.log4netLogger<T>();
        }

        public static string DefaultLogFile
        {
            get
            {
                foreach (ILoggerRepository r in log4net.LogManager.GetAllRepositories())
                {
                    foreach (IAppender ap in r.GetAppenders())
                    {
                        var rf = (ap as RollingFileAppender);
                        if (rf != null)
                        {
                            return rf.File;
                        }
                    }
                }
                return null;
            }
        }
    }
}