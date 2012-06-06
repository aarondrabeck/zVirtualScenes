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
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceValues.xaml
    /// </summary>
    public partial class DeviceValues : UserControl
    {
        private zvsLocalDBEntities context;
        private int DeviceID = 0;
        private device d;

        public DeviceValues(int DeviceID)
        {
            this.DeviceID = DeviceID;
            InitializeComponent();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            context = new zvsLocalDBEntities();
            zvsLocalDBEntities.onDeviceValueChanged += zvsLocalDBEntities_onDeviceValueChanged;

            d = context.devices.FirstOrDefault(dv => dv.id == DeviceID);
            if (d != null)
            {
                d.device_values.OrderBy(dv => dv.id).ToList();

                // Do not load your data at design time.
                if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    //Load your data here and assign the result to the CollectionViewSource.
                    System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["device_valuesViewSource"];
                    myCollectionViewSource.Source = d.device_values;
                }
            }
        }

        void zvsLocalDBEntities_onDeviceValueChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (args.ChangeType == System.Data.EntityState.Added)
                {
                    //Gets new devices
                    d.device_values.ToList();
                }
                else
                {
                    //Reloads context from DB when modifcations happen
                    foreach (var ent in context.ChangeTracker.Entries<device_values>())
                        ent.Reload();

                    ValuesDataGrid.CancelEdit();
                    ValuesDataGrid.Items.Refresh();
                }
            }));
        }

        private void DataGrid_Unloaded_1(object sender, RoutedEventArgs e)
        {
            context.Dispose();
        }
    }
}
