using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

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

            #region Handle Embeded DLLs
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
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            //ONLY LAUCH IF WE CAN CONNECT TO THE DATABASE
            //if (!String.IsNullOrEmpty(DatabaseControl.ExamineDatabase()) || DatabaseControl.GetOutdatedDbVersion() != null)
            //{
            //    DatabaseConnection FormDatabaseConnection = new DatabaseConnection();
            //    FormDatabaseConnection.ShowDialog();

            //    if (FormDatabaseConnection.DialogResult == DialogResult.Cancel)
            //    {
            //        return;
            //    }
            //}

           


            Application.Run(new MainForm());
        }
    }
}
