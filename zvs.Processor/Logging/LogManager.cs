using System;
using System.Linq;
using log4net.Appender;

namespace zvs.Processor.Logging
{
    public class LogManager
    {
        public static void ConfigureLogging()
        {
            //nunit friendly way to get the loggin config file
            var dll = typeof(LogManager).Assembly.CodeBase;
            Uri u;
            Uri.TryCreate(dll, UriKind.RelativeOrAbsolute, out u);

            var fi = new System.IO.FileInfo(u.LocalPath);
            var file = "log4net.config";
            if (Utils.DebugMode) file = "log4net.debug.config";

            if (fi.Directory != null)
            {
                var filename = System.IO.Path.Combine(fi.Directory.FullName, file);
                if (System.IO.File.Exists(filename))
                {
                    var configFile = new System.IO.FileInfo(filename);
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);
                }
            }
            EventedLog.Enabled = true;
        }
        public static ILog GetLogger<T>()
        {
            return new Log4NetLogger<T>();
        }
        public static object FindMemoryLogger()
        {
            return log4net.LogManager.GetAllRepositories().SelectMany(r => r.GetAppenders(), (r, ap) => (ap as MemoryAppender)).Where(rf => rf != null).FirstOrDefault(rf => rf.Name == "UIMemoryAppender");
        }

        public static string DefaultLogFile
        {
            get
            { return (from r in log4net.LogManager.GetAllRepositories() from ap in r.GetAppenders() select (ap as RollingFileAppender) into rf where rf != null select rf.File).FirstOrDefault(); }
        }
    }
}