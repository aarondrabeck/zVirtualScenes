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
using zvs.WPF.AdapterManager;
using System.Data.Entity;
using System.Threading;
using System.Diagnostics;
using zvs.Processor.Backup;
using zvs.Entities;
using zvs.WPF.JavaScript;
using zvs.Processor.Logging;
using System.Threading.Tasks;

namespace zvs.WPF
{
    /// <summary>
    /// interaction logic for MainWindow.xaml
    /// </summary>
    public partial class zvsMainWindow : ChromeWindow
    {
        private App app = (App)Application.Current;
        private zvsContext context;
        public WindowState lastOpenedWindowState = WindowState.Normal;
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<zvsMainWindow>();

        public zvsMainWindow()
        {
            InitializeComponent();

            context = new zvsContext();

            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["ListViewSource"];

                myCollectionViewSource.Source = logSource;
            }
            zvs.Processor.Logging.EventedLog.OnLogItemsCleared += EventedLog_OnLogItemsCleared;

        }

        private void EventedLog_OnLogItemsCleared(object sender, EventedLog.LogItemsClearedEventArgs e)
        {
            logSource.Clear();
        }

        private void EventedLog_OnLogItemArrived(object sender, EventedLog.LogItemArrivedEventArgs e)
        {
            if (!this.Dispatcher.HasShutdownStarted)
            {
                try
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        logSource.Clear();
                        foreach (var item in e.NewItems)
                        {
                            logSource.Add(item);
                        }
                        var entry = e.NewItems.LastOrDefault();
                        if (entry != null)
                        {
                            StatusBarDescriptionTxt.Text = entry.Description;
                            StatusBarSourceTxt.Text = entry.Source;
                            StatusBarUrgencyTxt.Text = entry.Urgency.ToString();
                        }
                    }));
                }
                catch (Exception)
                {
                }
            }
        }
#if DEBUG
        ~zvsMainWindow()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("zvsMainWindow Deconstructed.");
        }
#endif

        private ObservableCollection<LogItem> logSource = new ObservableCollection<LogItem>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EventedLog.OnLogItemArrived += EventedLog_OnLogItemArrived;

            log.InfoFormat("{0} User Interface Loaded", Utils.ApplicationName);//, Utils.ApplicationName + " GUI");

            Task.Run(async () =>
            {
                ICollectionView dataView = CollectionViewSource.GetDefaultView(logListView.ItemsSource);
                //clear the existing sort order
                dataView.SortDescriptions.Clear();

                //create a new sort order for the sorting that is done lastly            
                ListSortDirection dir = ListSortDirection.Ascending;

                var option = await context.ProgramOptions.FirstOrDefaultAsync(o => o.UniqueIdentifier == "LOGDIRECTION");
                if (option != null && option.Value == "Descending")
                    dir = ListSortDirection.Descending;

                dataView.SortDescriptions.Add(new SortDescription("Datetime", dir));
                //refresh the view which in turn refresh the grid
                dataView.Refresh();               
            });

            dList1.ShowMore = false;

            this.Title = Utils.ApplicationNameAndVersion;
        }
        
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            EventedLog.OnLogItemArrived -= EventedLog_OnLogItemArrived;
            zvs.Processor.Logging.EventedLog.OnLogItemsCleared -= EventedLog_OnLogItemsCleared;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowOrCreateWindow<GroupEditor>();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            ShowOrCreateWindow<AdapterManagerWindow>();
        }

        private void ManagePluginsMI(object sender, RoutedEventArgs e)
        {
            ShowOrCreateWindow<PluginManagerWindow>();
        }

        private void ShowOrCreateWindow<T>() where T : Window, new()
        {
            var w = app.Windows.OfType<T>().FirstOrDefault();
            if (w != null)
            {
                w.Activate();
                return;
            }

            var new_window = new T();
            new_window.Owner = this;
            new_window.Show();
        }

        private void ActivateGroupMI_Click_1(object sender, RoutedEventArgs e)
        {
            ShowOrCreateWindow<ActivateGroup>();
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

                EventedLog.OnLogItemArrived -= EventedLog_OnLogItemArrived;
                log = null;
            }
        }

        private void MainWindow_Closed_1(object sender, EventArgs e)
        {
            app = null;

            if (context != null)
                context.Dispose();
        }

        private async void RepollAllMI_Click_1(object sender, RoutedEventArgs e)
        {
            BuiltinCommand cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "REPOLL_ALL");
            if (cmd == null)
                return;

            CommandProcessor cp = new CommandProcessor(app.zvsCore);
            await cp.RunCommandAsync(this, cmd);
        }

        private void ExitMI_Click_1(object sender, RoutedEventArgs e)
        {
            app.ShutdownZVS();
        }

        private void ViewLogsMI_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string logFile = zvs.Processor.Logging.LogManager.DefaultLogFile;
                if (System.IO.File.Exists(logFile))
                {
                    System.Diagnostics.Process.Start(logFile);
                }
                else
                {
                    MessageBox.Show("Could not resolve the log file, make sure logging is configured correctly");
                }
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

        private async void ExportDeviceNamesMI_Click_1(object sender, RoutedEventArgs e)
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
                var result = await Backup.ExportDevicesAsync(dlg.FileName);

                if(result.HasError)
                    log.Error(result.Message);
                else
                    log.Info(result.Message);
            }
        }

        private async void ImportDeviceNamesMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "DevicesBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            bool? r = dlg.ShowDialog();

            if (r ?? true)
            {

                var result = await Backup.ImportDevicesAsync(dlg.FileName);
                if (result.HasError)
                    log.Error(result.Message);
                else
                    log.Info(result.Summary);
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
                Backup.ExportScenesAsync(dlg.FileName, (result) =>
                {
                    log.Info(result);
                });
            }
        }

        private async void ImportScenesMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "ScenesBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            bool? r = dlg.ShowDialog();

            if (r ?? true)
            {
                await Backup.ImportScenesAsync(dlg.FileName, (result) =>
                {
                    log.Info(result);
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
                Backup.ExportTriggerAsync(dlg.FileName, (result) =>
                {
                    log.Info(result);
                });
            }
        }

        private async void ImportTriggersMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "TriggersBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            bool? r = dlg.ShowDialog();

            if (r ?? true)
            {
                await Backup.ImportTriggersAsync(dlg.FileName, (result) =>
                {
                    log.Info(result);
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
                Backup.ExportGroupsAsync(dlg.FileName, (result) =>
                {
                    log.Info(result);
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
                Backup.ImportGroupsAsync(dlg.FileName, (result) =>
                {
                    log.Info(result);
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
                Backup.ExportScheduledTaskAsync(dlg.FileName, (result) =>
                {
                    log.Info(result);
                });
            }
        }

        private async void ImportScheduledTaskMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "ScheduledTaskBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            bool? r = dlg.ShowDialog();

            if (r ?? true)
            {
                await Backup.ImportScheduledTaskAsync(dlg.FileName, (result) =>
                {
                    log.Info(result);
                });
            }
        }

        private void ImportJSMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "JSCommandsBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            bool? r = dlg.ShowDialog();

            if (r ?? true)
            {
                Backup.ImportJavaScriptAsync(dlg.FileName, (result) =>
                {
                    log.Info(result);
                });
            }
        }

        private void ExportJSMI_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "JSCommandsBackup"; // Default file name
            dlg.DefaultExt = ".zvs"; // Default file extension
            dlg.Filter = "Zvs Files (.zvs)|*.zvs"; // Filter files by extension

            // Show open file dialog box
            bool? r = dlg.ShowDialog();

            // Process open file dialog box results
            if (r ?? true)
            {
                //string path = System.IO.Path.Combine(Utils.AppDataPath, "zvsDeviceNameExport.xml");
                Backup.ExportJavaScriptAsync(dlg.FileName, (result) =>
                {
                    log.Info(result);
                });
            }
        }

        private void SettingMI_Click_1(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.Owner = this;
            settingWindow.ShowDialog();
        }

        private void AddEditJSCmds_Click_1(object sender, RoutedEventArgs e)
        {
            JavaScriptAddRemove jsWindow = new JavaScriptAddRemove();
            jsWindow.Owner = this;
            jsWindow.ShowDialog();
        }

        private void ClearLogsMI_Click(object sender, RoutedEventArgs e)
        {
            zvs.Processor.Logging.EventedLog.Clear();

        }






    }
    public class ContentToMarginConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Thickness(0, 0, -((ContentPresenter)value).ActualHeight, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ContentToPathConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ps = new PathSegmentCollection(4);
            ContentPresenter cp = (ContentPresenter)value;
            double h = cp.ActualHeight > 10 ? 1.4 * cp.ActualHeight : 10;
            double w = cp.ActualWidth > 10 ? 1.25 * cp.ActualWidth : 10;
            ps.Add(new LineSegment(new Point(1, 0.7 * h), true));
            ps.Add(new BezierSegment(new Point(1, 0.9 * h), new Point(0.1 * h, h), new Point(0.3 * h, h), true));
            ps.Add(new LineSegment(new Point(w, h), true));
            ps.Add(new BezierSegment(new Point(w + 0.6 * h, h), new Point(w + h, 0), new Point(w + h * 1.3, 0), true));
            PathFigure figure = new PathFigure(new Point(1, 0), ps, false);
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
