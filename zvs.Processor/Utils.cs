using System;
using System.IO;
using System.Reflection;
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
                var assembly = Assembly.GetExecutingAssembly();
                var vrs = assembly.GetName().Version;
                return $"{vrs.Major}.{vrs.Minor}";
            }
        }


        public static string GetHostDetails
        {
            get
            {
                var Data = new StringBuilder();
                Data.AppendLine($"OSVersion: {Environment.OSVersion}");
                Data.AppendLine($"Is64BitOperatingSystem: {Environment.Is64BitOperatingSystem}");
                Data.AppendLine($"MachineName: {Environment.MachineName}");
                Data.AppendLine($"UserDomainName: {Environment.UserDomainName}");
                Data.AppendLine($"UserName: {Environment.UserName}");
                Data.AppendLine($"Version: {Environment.Version}");
                return Data.ToString();
            }
        }

        public static string ApplicationNameAndVersion
        {
            get
            {
                return $"{ApplicationName} {ApplicationVersionLong}";
            }
        }

        public static string AppDataPath
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var path = Path.Combine(appData, @"zVirtualScenes");
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
