using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using zvs.DataModel;

namespace zvs.WPF.Groups
{
    /// <summary>
    /// Interaction logic for DeviceMultiselectWindow.xaml
    /// </summary>
    public partial class DeviceMultiselectWindow
    {
        private ZvsEntityContextConnection ZvsEntityContextConnection { get; set; }
        private IEnumerable<int> DeviceIdsToExclude { get; set; }

        public IEnumerable<Device> SelectedDevices { get; set; }

        public DeviceMultiselectWindow(ZvsEntityContextConnection zvsEntityContextConnection, IEnumerable<int> deviceIdsToExclude)
        {
            if (deviceIdsToExclude == null)
                throw new ArgumentNullException("deviceIdsToExclude");

            SelectedDevices = new List<Device>();

            ZvsEntityContextConnection = zvsEntityContextConnection;
            DeviceIdsToExclude = deviceIdsToExclude;
            InitializeComponent();
        }

        private async void DeviceMultiselectWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            using (var context = new ZvsContext(ZvsEntityContextConnection))
            {
                var deviceViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("DeviceViewSource")));

                //Fill the device combo box from db
                await context.Devices.Where(o => !DeviceIdsToExclude.Contains(o.Id)).ToListAsync();
                deviceViewSource.Source = context.Devices.Local.OrderBy(o => o.Name);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectedDevices = DeviceListBox.SelectedItems.OfType<Device>();
            DialogResult = true;
            Close();
        }
    }
}
