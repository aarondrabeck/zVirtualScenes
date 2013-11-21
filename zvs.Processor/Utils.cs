using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using zvs.Entities;
using System.Data.Entity.Migrations;
using zvs.Context.Migrations;
using System.Data.Entity;

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
                return System.IO.Path.GetDirectoryName(typeof(Utils).Assembly.Location);
            }
        }

        public static string ApplicationVersionLong
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                Version vrs = assembly.GetName().Version;
                return string.Format("{0}.{1}.{2}", vrs.Major, vrs.Minor, vrs.Build);
            }
        }


        public static string GetHostDetails
        {
            get
            {
                StringBuilder Data = new StringBuilder();
                Data.AppendLine(string.Format("OSVersion: {0}", System.Environment.OSVersion));
                Data.AppendLine(string.Format("Is64BitOperatingSystem: {0}", System.Environment.Is64BitOperatingSystem));
                Data.AppendLine(string.Format("MachineName: {0}", System.Environment.MachineName));
                Data.AppendLine(string.Format("UserDomainName: {0}", System.Environment.UserDomainName));
                Data.AppendLine(string.Format("UserName: {0}", System.Environment.UserName));
                Data.AppendLine(string.Format("Version: {0}", System.Environment.Version));
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
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string path = System.IO.Path.Combine(appData, @"zVirtualScenes");
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

        public static string DBName = "zvsDBEFCF7.sdf";
        public static string DBNamePlusFullPath = Path.Combine(AppDataPath, DBName);

        public static bool HasDotNet45()
        {
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", false))
            {
                if (rk != null)
                {
                    object val = rk.GetValue("Release");
                    if (val != null)
                        return true;
                }
            }
            return false;
        }

        public static bool HasSQLCE4()
        {
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Compact Edition\v4.0\ENU", false))
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
