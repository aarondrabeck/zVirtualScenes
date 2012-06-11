using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using zVirtualScenes.Triggers;
using System.Windows.Threading;
using zVirtualScenesModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;

namespace zVirtualScenes
{
    public class Core
    {        
        public PluginManager pluginManager;
        public TriggerManager triggerManager;
        public ScheduledTaskManager scheduledTaskManager;
        public Logger Logger;
        public Dispatcher Dispatcher;

        public Core(Dispatcher Dispatcher)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Utils.AppDataPath);

            this.Dispatcher = Dispatcher;
            //TODO: THESE SHOULD BE FUNCTIONS SYNCRONOUS

            //Check for VCRedist
            if (!DetectVCRedist.IsVCRedistInstalled())
            {
               // MessageBox.Show(string.Format("Visual C++ Redistributable Missing!\n\n zVirtualScenes cannot open because Visual C++ Redistributable is not installed.\n\nPlease check the following path: {0}.", zvsEntityControl.GetDBPath), zvsEntityControl.zvsNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            //Check if DB exists
            FileInfo database = new FileInfo(Utils.DBNamePlusFullPath);
            if (!database.Exists)
            {
                FileInfo blank_database = new FileInfo(Utils.BlankDBNamePlusFullPath);
                if (!blank_database.Exists)
                {
                    MessageBox.Show(string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", Utils.BlankDBNamePlusFullPath), Utils.ApplicationNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                blank_database.CopyTo(Utils.DBNamePlusFullPath);
            }

            //Check DB integrity
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                try
                {
                    string name = context.Database.SqlQuery<string>(@"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'devices'").SingleOrDefault();
                    if (string.IsNullOrEmpty(name))
                    {
                        throw new Exception("Database Empty!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Database Empty!\n\n zVirtualScenes cannot open because the database is empty or corrupt.\n\nPlease check the following DB: {0}.", Utils.BlankDBNamePlusFullPath), Utils.ApplicationNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
            }

            ////Check DB version
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
                        using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                        {
                            string curr_db_version_str = context.Database.SqlQuery<string>(@"SELECT info_value FROM db_info WHERE info_name like 'Version';").SingleOrDefault();
                            int curr_db_version;
                            int.TryParse(curr_db_version_str, out curr_db_version);

                            if (new_db_version > curr_db_version)
                            {
                                if (new_db_version == curr_db_version + 1)
                                {
                                    upgradeScript = upgradeScript.Replace(upgradeScript.Substring(0, upgradeScript.IndexOf('\n')), "");
                                    context.Database.ExecuteSqlCommand(upgradeScript, null);
                                    context.Database.ExecuteSqlCommand(@"UPDATE db_info SET info_value = '" + new_db_version + "' WHERE info_name='Version';", null);
                                }
                                else
                                {
                                    if (MessageBox.Show("Your database is over 1 version old. Upgrading is not supported. Would you like to replace your database with a new blank one?", "Database too old.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                    {
                                        FileInfo blank_database = new FileInfo(Utils.BlankDBNamePlusFullPath);
                                        if (!database.Exists)
                                        {
                                            MessageBox.Show(string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", Utils.BlankDBNamePlusFullPath), Utils.ApplicationNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Error);
                                            Environment.Exit(0);
                                        }

                                        blank_database.CopyTo(Utils.DBNamePlusFullPath);
                                    }
                                    else
                                        Environment.Exit(0);

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (MessageBox.Show("We can't auto upgrade your database. Would you like to replace your database with a new blank one?", "Database too old.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            FileInfo blank_database = new FileInfo(Utils.BlankDBNamePlusFullPath);
                            if (!database.Exists)
                            {
                                MessageBox.Show(string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", Utils.BlankDBNamePlusFullPath), Utils.ApplicationNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Error);
                                Environment.Exit(0);
                            }

                            blank_database.CopyTo(Utils.DBNamePlusFullPath);
                        }
                        else
                            Environment.Exit(0);
                    }
                }
            }
                       

            //Create a instace of the logger
            Logger = new Logger();

            //Create the Plugin Manager thereby loading the plugins
            try
            {
                pluginManager = new PluginManager(this);
            }
            catch (System.Reflection.ReflectionTypeLoadException reflectionEx)
            {
                string error = "Cannot load one or more plug-ins.";
                Exception ex = reflectionEx.LoaderExceptions.FirstOrDefault();
                if (ex != null)
                {
                    error = ex.ToString() + 
                        Environment.NewLine + 
                        Environment.NewLine +
                        string.Format("This plug-in might not be compatible with {0}. Try removing the plugin and re-launching the application. ", Utils.ApplicationNameAndVersion);
                }
                if (MessageBox.Show(error, "Fatal plug-in load error", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK) == MessageBoxResult.OK)
                {
                    Environment.Exit(1);
                    return;
                }
            }
            catch (Exception e)
            {                
                if (MessageBox.Show(e.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK) == MessageBoxResult.OK)
                {
                    Environment.Exit(1);
                    return;
                }
            }            
            
            triggerManager = new TriggerManager();
            scheduledTaskManager = new ScheduledTaskManager();

        }
    }
}
