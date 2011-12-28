using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using zVirtualScenesCommon.Entity;
using System.IO;
using zVirtualScenesCommon.Util;

namespace zVirtualScenesApplication
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {           
                //Check is already running
                string proc = Process.GetCurrentProcess().ProcessName;
                Process[] processes = Process.GetProcessesByName(proc);
                if (processes.Length > 1)
                {
                    MessageBox.Show("Sorry, zVirtualScenes is already running!", "zVirtualScenes");
                    Environment.Exit(1);
                }

                //#region Handle Embeded DLLs
                //AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                //{
                //    String resourceName = "zVirtualScenesApplication.lib." + new AssemblyName(args.Name).Name + ".dll";

                //    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                //    {
                //        //Assembly a = Assembly.GetExecutingAssembly();
                //        //string[] b = a.GetManifestResourceNames(); 
                //        Byte[] assemblyData = new Byte[stream.Length];
                //        stream.Read(assemblyData, 0, assemblyData.Length);
                //        return Assembly.Load(assemblyData);
                //    }

                //};
                //#endregion

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //Check for VCRedist
                if(!DetectVCRedist.IsVCRedistInstalled())
                {
                    MessageBox.Show(string.Format("Visual C++ Redistributable Missing!\n\n zVirtualScenes cannot open because Visual C++ Redistributable is not installed.\n\nPlease check the following path: {0}.", zvsEntityControl.GetDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //Check if DB exists
                FileInfo database = new FileInfo(zvsEntityControl.GetDBPath);
                if (!database.Exists)
                {
                    FileInfo blank_database = new FileInfo(zvsEntityControl.GetBlankDBPath);
                    if (!database.Exists)
                    {
                        MessageBox.Show(string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", zvsEntityControl.GetDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    blank_database.CopyTo(zvsEntityControl.GetDBPath);
                }

                //Check DB integrity
                string name = zvsEntityControl.zvsContext.ExecuteStoreQuery<string>(@"SELECT name FROM sqlite_master WHERE name='devices';").SingleOrDefault();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show(string.Format("Database Empty!\n\n zVirtualScenes cannot open because the database is empty or corrupt.\n\nPlease check the following path: {0}.", zvsEntityControl.GetDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            
                Application.Run(new MainForm());
            
        }
    }
}
