using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using zvs.Processor;
using zvs.WPF.DeviceControls;
using zvs.WPF.Groups;
using zvs.WPF.PluginManager;

using System.Threading;
using System.Diagnostics;
using zvs.Processor.Backup;
using zvs.Entities;

namespace zvs.WPF
{
    /// <summary>
    /// interaction logic for MainWindow.xaml
    /// </summary>
    public partial class zvsMainWindow : Window
    {
        private App app = (App)Application.Current;
        private zvsContext context;
        public WindowState lastOpenedWindowState = WindowState.Normal;

        public zvsMainWindow()
        {
            InitializeComponent();

        }

        ~zvsMainWindow()
        {
            Debug.WriteLine("zvsMainWindow Deconstructed.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            context = new zvsContext();
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["ListViewSource"];
                myCollectionViewSource.Source = app.zvsCore.Logger.LOG;

            }
            app.zvsCore.Logger.WriteToLog(Urgency.INFO, string.Format("{0} User Interface Loaded", Utils.ApplicationName), Utils.ApplicationName + " GUI");

            ICollectionView dataView = CollectionViewSource.GetDefaultView(logListView.ItemsSource);
            //clear the existing sort order
            dataView.SortDescriptions.Clear();

            //create a new sort order for the sorting that is done lastly            
            ListSortDirection dir = ListSortDirection.Ascending;

            string direction = ProgramOption.GetProgramOption(context, "LOGDIRECTION");
            if (direction != null && direction == "Descending")
                dir = ListSortDirection.Descending;

            dataView.SortDescriptions.Add(new SortDescription("Datetime", dir));
            //refresh the view which in turn refresh the grid
            dataView.Refresh();

            dList1.ShowMore = false;

            this.Title = Utils.ApplicationNameAndVersion;

            LogItem i = app.zvsCore.Logger.LOG.LastOrDefault();
            if(i != null)
            {
                StatusBarDescriptionTxt.Text = i.Description;
                StatusBarSourceTxt.Text = i.Source;
                StatusBarUrgencyTxt.Text = i.Urgency.ToString();   
            }

            app.zvsCore.Logger.LOG.CollectionChanged += (s, a) =>
            {
                if (a.NewItems != null && a.NewItems.Count > 0)
                {
                    LogItem entry = (LogItem)a.NewItems[0];
                    this.Dispatcher.Invoke(new Action(() =>
                        {
                            StatusBarDescriptionTxt.Text = entry.Description;
                            StatusBarSourceTxt.Text = entry.Source;
                            StatusBarUrgencyTxt.Text = entry.Urgency.ToString();
                        }));
                }
            };
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
        }
        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in app.Windows)
            {
                if (window.GetType() == typeof(GroupEditor))
                {
                    window.Activate();
                    return;
                }
            }

            GroupEditor groupEditor = new GroupEditor();
            groupEditor.Owner = this;
            groupEditor.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (Window window in app.Windows)
            {
                if (window.GetType() == typeof(PluginManagerWindow))
                {
                    window.Activate();
                    return;
                }
            }

            PluginManagerWindow new_window = new PluginManagerWindow();
            new_window.Owner = this;
            new_window.Show();
        }

        private void ActivateGroupMI_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (Window window in app.Windows)
            {
                if (window is ActivateGroup)
                {
                    window.Activate();
                    return;
                }
            }

            ActivateGroup groupEditor = new ActivateGroup();
            groupEditor.Owner = this;
            groupEditor.Show();
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            if (!app.isShuttingDown)
            {
                if (app.taskbarIcon != null)
                {
                    app.taskbarIcon.ShowBalloonTip(Utils.ApplicationName, Utils.ApplicationNameAndVersion + " is still running", 3000, System.Windows.Forms.ToolTipIcon.Info);
                }

                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["ListViewSource"];
                myCollectionViewSource.Source = null;
            }
        }

        private void MainWindow_Closed_1(object sender, EventArgs e)
        {
            app = null;

            if (context != null)
                context.Dispose();
        }

        private void RepollAllMI_Click_1(object sender, RoutedEventArgs e)
        {
            BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "REPOLL_ALL");
            if (cmd != null)
                cmd.Run(context);
        }

        private void ExitMI_Click_1(object sender, RoutedEventArgs e)
        {
            app.ShutdownZVS();
        }

        private void ViewLogsMI_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Logger.LogPath);
            }
            catch
            {
                MessageBox.Show("Unable to launch Windows Explorer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewDBMI_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Utils.AppDataPath);
            }
            catch
            {
                MessageBox.Show("Unable to launch Windows Explorer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_StateChanged_1(object sender, EventArgs e)
        {
            if (WindowState != System.Windows.WindowState.Minimized)
                lastOpenedWindowState = WindowState;

            if (WindowState == WindowState.Minimized)
            {
                this.Close();
            }
        }

        private void AboutMI_Click_1(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWin = new AboutWindow();
            aboutWin.Owner = this;
            aboutWin.ShowDialog();
        }

        private void ExportDeviceNamesMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "DevicesBackup"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            // Show open file dialog box
            bool? r = dlg.ShowDialog();

            // Process open file dialog box results
            if (r ?? true)
            {
                //string path = System.IO.Path.Combine(Utils.AppDataPath, "zvsDeviceNameExport.xml");
                Backup.ExportDevicesAsyc(dlg.FileName, (result) =>
                {
                    app.zvsCore.Logger.WriteToLog(Urgency.INFO, result, Utils.ApplicationName + " GUI"); 
                });
            }
        }

        private void ImportDeviceNamesMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "DevicesBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            bool? r = dlg.ShowDialog();

            if (r ?? true)
            {
                Backup.ImportDevicesAsyn(dlg.FileName, (result) =>
                {
                    app.zvsCore.Logger.WriteToLog(Urgency.INFO, result, Utils.ApplicationName + " GUI"); 
                });
            }
        }

        private void ExportScenesMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "ScenesBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            // Show open file dialog box
            bool? r = dlg.ShowDialog();

            // Process open file dialog box results
            if (r ?? true)
            {
                //string path = System.IO.Path.Combine(Utils.AppDataPath, "zvsDeviceNameExport.xml");
                Backup.ExportScenesAsyc(dlg.FileName, (result) =>
                {
                    app.zvsCore.Logger.WriteToLog(Urgency.INFO, result, Utils.ApplicationName + " GUI"); 
                });
            }
        }

        private void ImportScenesMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "ScenesBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            bool? r = dlg.ShowDialog();

            if (r ?? true)
            {
                Backup.ImportScenesAsyn(dlg.FileName, (result) =>
                {
                    app.zvsCore.Logger.WriteToLog(Urgency.INFO, result, Utils.ApplicationName + " GUI"); 
                });
            }
        }

        private void ExportTriggersMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "TriggersBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            // Show open file dialog box
            bool? r = dlg.ShowDialog();

            // Process open file dialog box results
            if (r ?? true)
            {
                //string path = System.IO.Path.Combine(Utils.AppDataPath, "zvsDeviceNameExport.xml");
                Backup.ExportTriggerAsyc(dlg.FileName, (result) =>
                {
                    app.zvsCore.Logger.WriteToLog(Urgency.INFO, result, Utils.ApplicationName + " GUI"); 
                });
            }
        }

        private void ImportTriggersMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "TriggersBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            bool? r = dlg.ShowDialog();

            if (r ?? true)
            {
                Backup.ImportTriggersAsyn(dlg.FileName, (result) =>
                {
                    app.zvsCore.Logger.WriteToLog(Urgency.INFO, result, Utils.ApplicationName + " GUI"); 
                });
            }
        }

        private void ExportGroupsMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "GroupsBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            // Show open file dialog box
            bool? r = dlg.ShowDialog();

            // Process open file dialog box results
            if (r ?? true)
            {
                //string path = System.IO.Path.Combine(Utils.AppDataPath, "zvsDeviceNameExport.xml");
                Backup.ExportGroupsAsyc(dlg.FileName, (result) =>
                {
                    app.zvsCore.Logger.WriteToLog(Urgency.INFO, result, Utils.ApplicationName + " GUI"); 
                });
            }
        }

        private void ImportGroupsMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "GroupsBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            bool? r = dlg.ShowDialog();

            if (r ?? true)
            {
                Backup.ImportGroupsAsyn(dlg.FileName, (result) =>
                {
                    app.zvsCore.Logger.WriteToLog(Urgency.INFO, result,Utils.ApplicationName + " GUI"); 
                });
            }
        }

        private void ExportScheduledTaskMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "ScheduledTaskBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            // Show open file dialog box
            bool? r = dlg.ShowDialog();

            // Process open file dialog box results
            if (r ?? true)
            {
                //string path = System.IO.Path.Combine(Utils.AppDataPath, "zvsDeviceNameExport.xml");
                Backup.ExportScheduledTaskAsyc(dlg.FileName, (result) =>
                {
                    app.zvsCore.Logger.WriteToLog(Urgency.INFO, result, Utils.ApplicationName + " GUI");
                });
            }
        }

        private void ImportScheduledTaskMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "ScheduledTaskBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            bool? r = dlg.ShowDialog();

            if (r ?? true)
            {
                Backup.ImportScheduledTaskAsyn(dlg.FileName, (result) =>
                {
                    app.zvsCore.Logger.WriteToLog(Urgency.INFO, result, Utils.ApplicationName + " GUI");
                });
            }
        }

        private void SettingMI_Click_1(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.Owner = this;
            settingWindow.ShowDialog();
        }        
    }
}
