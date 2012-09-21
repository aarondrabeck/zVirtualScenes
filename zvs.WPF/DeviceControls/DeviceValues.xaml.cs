using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using zvs.Entities;
using zvs.Processor;


namespace zvs.WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceValues.xaml
    /// </summary>
    public partial class DeviceValues : UserControl
    {
        private App app = (App)Application.Current;
        private zvsContext context;
        private int DeviceID = 0;
        private Device d;

        public DeviceValues(int DeviceID)
        {
            this.DeviceID = DeviceID;
            InitializeComponent();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            context = new zvsContext();
            zvsContext.onDeviceValueChanged += zvsContext_onDeviceValueChanged;

            d = context.Devices.FirstOrDefault(dv => dv.DeviceId == DeviceID);
            if (d != null)
            {
                d.Values.OrderBy(dv => dv.DeviceValueId).ToList();

                // Do not load your data at design time.
                if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    //Load your data here and assign the result to the CollectionViewSource.
                    System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["DeviceValueViewSource"];
                    myCollectionViewSource.Source = d.Values;
                }
            }
        }

        void zvsContext_onDeviceValueChanged(object sender, zvsContext.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (context != null)
                {
                    if (args.ChangeType == System.Data.EntityState.Added)
                    {
                        //Gets new devices
                        d.Values.ToList();
                    }
                    else
                    {
                        //Reloads context from DB when modifications happen
                        foreach (var ent in context.ChangeTracker.Entries<DeviceValue>())
                            ent.Reload();
                    }
                }
            }));
        }

        private void DataGrid_Unloaded_1(object sender, RoutedEventArgs e)
        {
            zvsContext.onDeviceValueChanged -= zvsContext_onDeviceValueChanged;
           
        }

        private void RepollLnk_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (d != null)
            {
                BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "REPOLL_ME");
                if (cmd != null)
                {
                    CommandProcessor cp = new CommandProcessor(app.zvsCore);
                    cp.RunBuiltinCommand(context, cmd, d.DeviceId.ToString());
                }
            }
        }
    }
}
