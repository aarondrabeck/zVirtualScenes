using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesCommon;
using System.ComponentModel;
using zVirtualScenes.Triggers;
using System.Windows.Threading;
using zvsModel;
using System.Collections.ObjectModel;

namespace zVirtualScenes
{
    public class Core
    {
        public zvsLocalDBEntities context;

        public PluginManager pluginManager;
        public TriggerManager triggerManager;
        public ScheduledTaskManager scheduledTaskManager;
        public Logger Logger;

        public Core()
        {
            //TODO: THESE SHOULD BE FUNCTIONS SYNCRONOUS
            #region temp
            //Check for VCRedist
            if (!DetectVCRedist.IsVCRedistInstalled())
            {
               // MessageBox.Show(string.Format("Visual C++ Redistributable Missing!\n\n zVirtualScenes cannot open because Visual C++ Redistributable is not installed.\n\nPlease check the following path: {0}.", zvsEntityControl.GetDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            
            //            //Check if DB exists
            //            FileInfo database = new FileInfo(zvsEntityControl.GetDBPath);
            //            if (!database.Exists)
            //            {
            //                FileInfo blank_database = new FileInfo(zvsEntityControl.GetBlankDBPath);
            //                if (!blank_database.Exists)
            //                {
            //                    MessageBox.Show(string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", zvsEntityControl.GetBlankDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Error);
            //                     Environment.Exit(0);
            //                }

            //                blank_database.CopyTo(zvsEntityControl.GetDBPath);
            //            }

            //            //Check DB integrity
            //            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            //            {
            //                try
            //                {
            //                    string name = db.ExecuteStoreQuery<string>(@"SELECT        TABLE_NAME
            //FROM            INFORMATION_SCHEMA.TABLES
            //WHERE        (TABLE_NAME = 'devices');").SingleOrDefault();
            //                    if (string.IsNullOrEmpty(name))
            //                    {
            //                        throw new Exception("Database Empty!");
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    MessageBox.Show(string.Format("Database Empty!\n\n zVirtualScenes cannot open because the database is empty or corrupt.\n\nPlease check the following path: {0}.", zvsEntityControl.GetDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Error);
            //                    Environment.Exit(0);
            //                }
            //            }

            ////Check DB version
            //String upgradeScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"database\upgrade.sql");

            //if (File.Exists(upgradeScriptPath))
            //{
            //    StreamReader upgradeScriptSR = new StreamReader(upgradeScriptPath);
            //    String upgradeScript = upgradeScriptSR.ReadToEnd();
            //    upgradeScriptSR.Close();

            //    if (upgradeScript.ToLower().StartsWith("db_version"))
            //    {
            //        int new_db_version;
            //        int.TryParse(upgradeScript.Substring(upgradeScript.IndexOf("=") + 1, upgradeScript.IndexOf('\n') - upgradeScript.IndexOf("=")), out new_db_version);
            //        try
            //        {
            //            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            //            {
            //                string curr_db_version_str = db.ExecuteStoreQuery<string>(@"SELECT info_value FROM db_info WHERE info_name like 'Version';").SingleOrDefault();
            //                int curr_db_version;
            //                int.TryParse(curr_db_version_str, out curr_db_version);

            //                if (new_db_version > curr_db_version)
            //                {
            //                    if (new_db_version == curr_db_version + 1)
            //                    {
            //                        upgradeScript = upgradeScript.Replace(upgradeScript.Substring(0, upgradeScript.IndexOf('\n')), "");
            //                        db.ExecuteStoreCommand(upgradeScript, null);
            //                        db.ExecuteStoreCommand(@"UPDATE db_info SET info_value = '" + new_db_version + "' WHERE info_name='Version';", null);
            //                    }
            //                    else
            //                    {
            //                        if (MessageBox.Show("Your database is over 1 version old. Upgrading is not supported. Would you like to replace your database with a new blank one?", "Database too old.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //                        {
            //                            FileInfo blank_database = new FileInfo(zvsEntityControl.GetBlankDBPath);
            //                            if (!database.Exists)
            //                            {
            //                                MessageBox.Show(string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", zvsEntityControl.GetBlankDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Error);
            //                                 Environment.Exit(0);
            //                            }

            //                            blank_database.CopyTo(zvsEntityControl.GetDBPath);
            //                        }
            //                        else
            //                             Environment.Exit(0);

            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            if (MessageBox.Show("We can't auto upgrade your database. Would you like to replace your database with a new blank one?", "Database too old.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //            {
            //                FileInfo blank_database = new FileInfo(zvsEntityControl.GetBlankDBPath);
            //                if (!database.Exists)
            //                {
            //                    MessageBox.Show(string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", zvsEntityControl.GetBlankDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Error);
            //                     Environment.Exit(0);
            //                }

            //                blank_database.CopyTo(zvsEntityControl.GetDBPath);
            //            }
            //            else
            //                Environment.Exit(0);
            //        }
            //    }
            //}
            #endregion

            //Create our context
            context = new zvsLocalDBEntities();

            //Create a instace of the logger
            Logger = new Logger();

            pluginManager = new PluginManager(this);
            triggerManager = new TriggerManager(this);
            scheduledTaskManager = new ScheduledTaskManager(this);

        }
    }
}
