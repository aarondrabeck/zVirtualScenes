using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using zVirtualScenesModel;

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
                return string.Format("{0}.{1}", vrs.Major, vrs.Minor);
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

        public static string DBName = "zvsLocalDB.sdf";
        public static string DBNamePlusFullPath = Path.Combine(AppDataPath, DBName);

        public static string BlankDBName = "zvsLocalDB-blank.sdf";
        public static string BlankDBNamePlusFullPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BlankDBName);
            }
        }

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

        /// <summary>
        /// Returns null if no error, else the error
        /// </summary>
        /// <returns></returns>
        public static string PreReqChecks()
        {
            #region Pre App Start Checks

            //.net 4.5
            if (!Utils.HasDotNet45())
            {
                return string.Format("Microsoft .NET Framework 4.5 Full/Extended is required to run {0}. \r\n\r\nPlease install Microsoft .NET Framework 4.5 and re-launch the application.", Utils.ApplicationName);
            }

            if (!Utils.HasSQLCE4())
            {
                return string.Format("Microsoft® SQL Server® Compact 4.0 SP1 is required to run {0}. \r\n\r\nPlease install Microsoft® SQL Server® Compact 4.0 SP1 and re-launch the application.", Utils.ApplicationName);
            }

            //Check for VCRedist
            if (!DetectVCRedist.IsVCRedistInstalled())
            {
                return "Visual C++ Redistributable Missing!\n\n zVirtualScenes cannot open because Visual C++ Redistributable is not installed.";
            }

            //Check if DB exists
            FileInfo database = new FileInfo(Utils.DBNamePlusFullPath);
            if (!database.Exists)
            {
                FileInfo blank_database = new FileInfo(Utils.BlankDBNamePlusFullPath);
                if (!blank_database.Exists)
                {
                    return string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", Utils.BlankDBNamePlusFullPath);
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
                    return string.Format("Database Empty!\n\n zVirtualScenes cannot open because the database is empty or corrupt.\n\nPlease check the following DB: {0}.", Utils.BlankDBNamePlusFullPath);

                }
            }

            ////Check DB version
            String upgradeScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"upgrade.sql");

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
                            var ver = context.db_info.FirstOrDefault(o => o.info_name == "Version");
                            if (ver != null)
                            {
                                int curr_db_version = 1;
                                int.TryParse(ver.info_value, out curr_db_version);

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
                                        Window WpfBugWindow = new Window()
                                        {
                                            AllowsTransparency = true,
                                            Background = System.Windows.Media.Brushes.Transparent,
                                            WindowStyle = WindowStyle.None,
                                            Top = 0,
                                            Left = 0,
                                            Width = 1,
                                            Height = 1,
                                            ShowInTaskbar = false
                                        };
                                        WpfBugWindow.Show();
                                        if (MessageBox.Show("Your database is over 1 version old. Upgrading is not supported. Would you like to replace your database with a new blank one?", "Database too old.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                        {
                                            FileInfo blank_database = new FileInfo(Utils.BlankDBNamePlusFullPath);
                                            if (!database.Exists)
                                            {
                                                WpfBugWindow.Close();
                                                return string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", Utils.BlankDBNamePlusFullPath);
                                            }

                                            blank_database.CopyTo(Utils.DBNamePlusFullPath);
                                        }
                                        else
                                        {
                                            WpfBugWindow.Close();
                                            Environment.Exit(1);
                                        }

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Window WpfBugWindow = new Window()
                        {
                            AllowsTransparency = true,
                            Background = System.Windows.Media.Brushes.Transparent,
                            WindowStyle = WindowStyle.None,
                            Top = 0,
                            Left = 0,
                            Width = 1,
                            Height = 1,
                            ShowInTaskbar = false
                        };
                        WpfBugWindow.Show();
                        if (MessageBox.Show("We can't auto upgrade your database. Would you like to replace your database with a new blank one?", "Database too old.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            FileInfo blank_database = new FileInfo(Utils.BlankDBNamePlusFullPath);
                            if (!database.Exists)
                            {
                                return string.Format("Database Missing!\n\n zVirtualScenes cannot open because the database is missing.\n\nPlease check the following path: {0}.", Utils.BlankDBNamePlusFullPath);
                            }

                            blank_database.CopyTo(Utils.DBNamePlusFullPath);
                        }
                        else
                        {
                            WpfBugWindow.Close();
                            Environment.Exit(1);
                        }
                    }
                }
            }
            #endregion
            return null;
        }
    }
}
