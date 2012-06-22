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
using zVirtualScenes;
using zVirtualScenesGUI.DeviceControls;
using zVirtualScenesGUI.Groups;
using zVirtualScenesGUI.PluginManager;
using zVirtualScenesModel;
using System.Threading;
using System.Diagnostics;

namespace zVirtualScenesGUI
{
    /// <summary>
    /// interaction logic for MainWindow.xaml
    /// </summary>
    public partial class zvsMainWindow : Window
    {
        private App app = (App)Application.Current;
        private zvsLocalDBEntities context;
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
            context = new zvsLocalDBEntities();
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
            dataView.SortDescriptions.Add(new SortDescription("Datetime", ListSortDirection.Descending));
            //refresh the view which in turn refresh the grid
            dataView.Refresh();

            dList1.ShowMore = false;

            this.Title = Utils.ApplicationNameAndVersion;
        }
        
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.F12)
            //{
            //    SetNamesDevOnly();
            //}
        }

        //private void SetNamesDevOnly()
        //{
        //    foreach (device d in context.devices)
        //    {
        //        switch (d.node_id)
        //        {
        //            case 1:
        //                d.friendly_name = "Aeon Labs Z-Stick Series 2";
        //                break;
        //            case 3:
        //                d.friendly_name = "Master Bathtub Light";
        //                break;
        //            case 4:
        //                d.friendly_name = "Masterbath Mirror Light";
        //                break;
        //            case 5:
        //                d.friendly_name = "Masterbed Hallway Light";
        //                break;
        //            case 6:
        //                d.friendly_name = "Masterbed East Light";
        //                break;
        //            case 7:
        //                d.friendly_name = "Masterbed Bed Light";
        //                break;
        //            case 8:
        //                d.friendly_name = "Office Light";
        //                d.current_level_int = d.current_level_int + 1;
        //                d.current_level_txt = (d.current_level_int).ToString() + "%";
        //                break;
        //            case 9:
        //                d.friendly_name = "Family Hallway Light";
        //                break;
        //            case 10:
        //                d.friendly_name = "Outside Entry Light";
        //                break;
        //            case 11:
        //                d.friendly_name = "Entryway Light";
        //                break;
        //            case 12:
        //                d.friendly_name = "Can Lights";
        //                break;
        //            case 13:
        //                d.friendly_name = "Porch Light";
        //                break;
        //            case 14:
        //                d.friendly_name = "Dining Table Light";
        //                break;
        //            case 15:
        //                d.friendly_name = "Fan Light";
        //                break;
        //            case 16:
        //                d.friendly_name = "Kitchen Light";
        //                break;
        //            case 17:
        //                d.friendly_name = "Rear Garage Light";
        //                break;
        //            case 18:
        //                d.friendly_name = "Driveway Light";
        //                break;
        //            case 19:
        //                d.friendly_name = "TV Backlight";
        //                break;
        //            case 20:
        //                d.friendly_name = "Fireplace Light";
        //                break;
        //            case 22:
        //                d.friendly_name = "Label Printer";
        //                break;
        //            case 23:
        //                d.friendly_name = "Brother Printer";
        //                break;
        //            case 24:
        //                d.friendly_name = "South Thermostat";
        //                break;
        //            case 25:
        //                d.friendly_name = "Masterbed Window Fan";
        //                break;
        //            case 26:
        //                d.friendly_name = "Masterbed Thermostat";
        //                break;
        //            case 27:
        //                d.friendly_name = "Aeon Labs Z-Stick Series 1 (Secondary)";
        //                break;
        //            case 28:
        //                d.friendly_name = "Xmas Lights";
        //                break;
        //            case 29:
        //                d.friendly_name = "Music Room Rope Light";
        //                break;
        //            case 30:
        //                d.friendly_name = "Not Used";
        //                break;
        //            case 31:
        //                d.friendly_name = "Family Room Rope Light";
        //                break;
        //        }
        //        context.SaveChanges();
        //        //device.CallOnContextUpdated();
        //    }
        //}

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

            if(context!= null)
                context.Dispose();
        }

        private void RepollAllMI_Click_1(object sender, RoutedEventArgs e)
        {
            builtin_commands cmd = context.builtin_commands.FirstOrDefault(c => c.name == "REPOLL_ALL");
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
                MessageBox.Show("Unable to launch Windows Explorer.", "Error", MessageBoxButton.OK,MessageBoxImage.Error );
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
    }
}
