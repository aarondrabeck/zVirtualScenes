using System;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace zvs.Processor
{
    public static class Utils
    {
        public static string ApplicationName
        {
            get
            {
                return "zVirtualScenes";
            }
        }
        public static string AppPath
        {
            get
            {
                return Path.GetDirectoryName(typeof(Utils).Assembly.Location);
            }
        }

        public static string ApplicationVersionLong
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var vrs = assembly.GetName().Version;
                return string.Format("{0}.{1}", vrs.Major, vrs.Minor);
            }
        }


        public static string GetHostDetails
        {
            get
            {
                var Data = new StringBuilder();
                Data.AppendLine(string.Format("OSVersion: {0}", Environment.OSVersion));
                Data.AppendLine(string.Format("Is64BitOperatingSystem: {0}", Environment.Is64BitOperatingSystem));
                Data.AppendLine(string.Format("MachineName: {0}", Environment.MachineName));
                Data.AppendLine(string.Format("UserDomainName: {0}", Environment.UserDomainName));
                Data.AppendLine(string.Format("UserName: {0}", Environment.UserName));
                Data.AppendLine(string.Format("Version: {0}", Environment.Version));
                return Data.ToString();
            }
        }

        public static string ApplicationNameAndVersion
        {
            get
            {
                return string.Format("{0} {1}", ApplicationName, ApplicationVersionLong);
            }
        }

        public static string AppDataPath
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var path = System.IO.Path.Combine(appData, @"zVirtualScenes");
                if (!Directory.Exists(path))
                {
                    try { Directory.CreateDirectory(path); }
                    catch { }
                }
                return path + "\\";
            }
        }

        public static bool DebugMode
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static string DbName = "zvsDBEFCF8.sdf";
        public static string DbNamePlusFullPath = Path.Combine(AppDataPath, DbName);

        public static bool HasDotNet45()
        {
            using (var rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", false))
            {
                if (rk != null)
                {
                    var val = rk.GetValue("Release");
                    if (val != null)
                        return true;
                }
            }
            return false;
        }

        public static bool HasSQLCE4()
        {
            using (var rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Compact Edition\v4.0\ENU", false))
            {
                if (rk != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
