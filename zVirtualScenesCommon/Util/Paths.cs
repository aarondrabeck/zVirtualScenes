using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace zVirtualScenesCommon.Util
{
    public class Paths
    {
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
