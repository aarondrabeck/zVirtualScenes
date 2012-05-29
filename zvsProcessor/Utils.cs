using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zVirtualScenes
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

        public static string ApplicationVersion
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                Version vrs = assembly.GetName().Version;
                return string.Format("{0}.{1}.{2}.{3}", vrs.Major, vrs.Minor, vrs.Build, vrs.Revision);
            }
        }

        public static string ApplicationNameAndVersion
        {
            get
            {
                return string.Format("{0} {1}", ApplicationName, ApplicationVersion);
            }
        }

        public static string AppDataPath
        {
            get
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string path = Path.Combine(appData, @"zVirtualScenes");
                if (!Directory.Exists(path))
                {
                    try { Directory.CreateDirectory(path); }
                    catch { }
                }
                return path + "\\";
            }
        }
    }
}
