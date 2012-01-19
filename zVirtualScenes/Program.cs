using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using zVirtualScenesCommon.Entity;
using System.IO;
using zVirtualScenesCommon.Util;
using System.Data.SQLite;

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
            //string proc = Process.GetCurrentProcess().ProcessName;
            //Process[] processes = Process.GetProcessesByName(proc);
            //if (processes.Length > 1)
            //{
            //    MessageBox.Show("Sorry, zVirtualScenes is already running!", "zVirtualScenes");
            //    Environment.Exit(1);
            //}

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
            if (!DetectVCRedist.IsVCRedistInstalled())
            {
                MessageBox.Show(string.Format("Visual C++ Redistributable Missing!\n\n zVirtualScenes cannot open because Visual C++ Redistributable is not installed.\n\nPlease check the following path: {0}.", zvsEntityControl.GetDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Check if DB exists
            FileInfo database = new FileInfo(zvsEntityControl.GetDBPath);
            if (!database.Exists)
            {
                FileInfo blank_database = new FileInfo(zvsEntityControl.GetBlankDBPath);
                if (!blank_database.Exists)
                {
                    MessageBox.Show(string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", zvsEntityControl.GetBlankDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                blank_database.CopyTo(zvsEntityControl.GetDBPath);
            }

            //Check DB integrity
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                string name = db.ExecuteStoreQuery<string>(@"SELECT name FROM sqlite_master WHERE name='devices';").SingleOrDefault();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show(string.Format("Database Empty!\n\n zVirtualScenes cannot open because the database is empty or corrupt.\n\nPlease check the following path: {0}.", zvsEntityControl.GetDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            //Check DB version
            String upgradeScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"database\upgrade.sql");

            if (File.Exists(upgradeScriptPath))
            {                
                StreamReader upgradeScriptSR = new StreamReader(upgradeScriptPath);
                String upgradeScript = upgradeScriptSR.ReadToEnd();
                upgradeScriptSR.Close();

                if (upgradeScript.ToLower().StartsWith("db_version"))
                {
                    int new_db_version;
                    int.TryParse(upgradeScript.Substring(upgradeScript.IndexOf("=") + 1, upgradeScript.IndexOf('\n') - upgradeScript.IndexOf("=")), out new_db_version);
                    try
                    {
                        using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                        {
                            string curr_db_version_str = db.ExecuteStoreQuery<string>(@"SELECT info_value FROM db_info WHERE info_name='Version';").SingleOrDefault();
                            int curr_db_version;
                            int.TryParse(curr_db_version_str, out curr_db_version);

                            if (new_db_version > curr_db_version)
                            {
                                if (new_db_version == curr_db_version + 1)
                                {
                                    upgradeScript = upgradeScript.Replace(upgradeScript.Substring(0, upgradeScript.IndexOf('\n')), "");
                                    db.ExecuteStoreCommand(upgradeScript, null);
                                    db.ExecuteStoreCommand(@"UPDATE db_info SET info_value = '" + new_db_version + "' WHERE info_name='Version';", null);
                                }
                                else
                                {
                                    if (MessageBox.Show("Your database is over 1 version old. Upgrading is not supported. Would you like to replace your database with a new blank one?", "Database too old.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    {
                                        FileInfo blank_database = new FileInfo(zvsEntityControl.GetBlankDBPath);
                                        if (!database.Exists)
                                        {
                                            MessageBox.Show(string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", zvsEntityControl.GetBlankDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }

                                        blank_database.CopyTo(zvsEntityControl.GetDBPath);
                                    }
                                    else
                                        return;

                                }
                            }
                        }
                    }
                    catch (SQLiteException e)
                    {
                        if (MessageBox.Show("We can't auto upgrade your database. Would you like to replace your database with a new blank one?", "Database too old.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            FileInfo blank_database = new FileInfo(zvsEntityControl.GetBlankDBPath);
                            if (!database.Exists)
                            {
                                MessageBox.Show(string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", zvsEntityControl.GetBlankDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            blank_database.CopyTo(zvsEntityControl.GetDBPath);
                        }
                        else
                            return;
                    }
                }
            }

            Application.Run(new MainForm());

        }
    }
}
