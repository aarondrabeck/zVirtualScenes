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
using zVirtualScenesCommon.Util;
using zVirtualScenesCommon;
using System.Collections.ObjectModel;
using System.ComponentModel;
using zvsProcessor;
using zVirtualScenesCommon.Entity;
using zVirtualScenes_WPF.DeviceControls;
using zVirtualScenes_WPF.Groups;

namespace zVirtualScenes_WPF
{
    /// <summary>
    /// interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<LogItem> Log = new ObservableCollection<LogItem>();
        public int MaxLogLines = 1000;
        public zvsManager manager;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.LOG.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(LOG_CollectionChanged);
            logListView.ItemsSource = Log;

            Logger.WriteToLog(Urgency.INFO, "STARTED", "zVirtualScenes");

            //start the processor
            BackgroundWorker managerWorker = new BackgroundWorker();
            managerWorker.DoWork += new DoWorkEventHandler(processorWorker_DoWork);
            managerWorker.RunWorkerAsync();

            System.Windows.Data.CollectionViewSource zvsEntities2ViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("zvsEntities2ViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // zvsEntities2ViewSource.Source = [generic data source]
        }

        private void processorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            manager = new zvsManager();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Logger.LOG.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(LOG_CollectionChanged);
        }

        void LOG_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                foreach (LogItem item in e.NewItems)
                {
                    Log.Insert(0, item);

                    if (Log.Count > MaxLogLines)
                    {
                        //remove the last log entry if > than max
                        Log.RemoveAt(Log.Count - 1);
                    }
                }
            }));
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                SetNamesDevOnly();
            }
        }

        private void SetNamesDevOnly()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                foreach (device d in db.devices)
                {
                    switch (d.node_id)
                    {
                        case 1:
                            d.friendly_name = "Aeon Labs Z-Stick Series 2";
                            break;
                        case 3:
                            d.friendly_name = "Master Bathtub Light";
                            break;
                        case 4:
                            d.friendly_name = "Masterbath Mirror Light";
                            break;
                        case 5:
                            d.friendly_name = "Masterbed Hallway Light";
                            break;
                        case 6:
                            d.friendly_name = "Masterbed East Light";
                            break;
                        case 7:
                            d.friendly_name = "Masterbed Bed Light";
                            break;
                        case 8:
                            d.friendly_name = "Office Light";
                            break;
                        case 9:
                            d.friendly_name = "Family Hallway Light";
                            break;
                        case 10:
                            d.friendly_name = "Outside Entry Light";
                            break;
                        case 11:
                            d.friendly_name = "Entryway Light";
                            break;
                        case 12:
                            d.friendly_name = "Can Lights";
                            break;
                        case 13:
                            d.friendly_name = "Porch Light";
                            break;
                        case 14:
                            d.friendly_name = "Dining Table Light";
                            break;
                        case 15:
                            d.friendly_name = "Fan Light";
                            break;
                        case 16:
                            d.friendly_name = "Kitchen Light";
                            break;
                        case 17:
                            d.friendly_name = "Rear Garage Light";
                            break;
                        case 18:
                            d.friendly_name = "Driveway Light";
                            break;
                        case 19:
                            d.friendly_name = "TV Backlight";
                            break;
                        case 20:
                            d.friendly_name = "Fireplace Light";
                            break;
                        case 22:
                            d.friendly_name = "Label Printer";
                            break;
                        case 23:
                            d.friendly_name = "Brother Printer";
                            break;
                        case 24:
                            d.friendly_name = "South Thermostat";
                            break;
                        case 25:
                            d.friendly_name = "Masterbed Window Fan";
                            break;
                        case 26:
                            d.friendly_name = "Masterbed Thermostat";
                            break;
                        case 27:
                            d.friendly_name = "Aeon Labs Z-Stick Series 1 (Secondary)";
                            break;
                        case 28:
                            d.friendly_name = "Xmas Lights";
                            break;
                    }
                    db.SaveChanges();
                    d.CallChanged("friendly_name");
                }
                
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
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


    }
}
