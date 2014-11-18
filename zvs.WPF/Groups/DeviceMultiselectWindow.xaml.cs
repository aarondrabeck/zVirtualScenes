using System.Data.Entity;
using System.Linq;
using System.Windows;
using zvs.DataModel;

namespace zvs.WPF.Groups
{
    /// <summary>
    /// Interaction logic for DeviceMultiselectWindow.xaml
    /// </summary>
    public partial class DeviceMultiselectWindow
    {
        private ZvsEntityContextConnection ZvsEntityContextConnection { get;set; }

        public DeviceMultiselectWindow(ZvsEntityContextConnection zvsEntityContextConnection)
        {
            ZvsEntityContextConnection = zvsEntityContextConnection;
            InitializeComponent();
        }

        private async void DeviceMultiselectWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            using (var context = new ZvsContext(ZvsEntityContextConnection))
            {
                //Fill the device combo box from db
                await context.Devices.ToListAsync();

                DevicesCmboBox.ItemsSource = context.Devices.Local.OrderBy(o => o.Name);
                if (DevicesCmboBox.Items.Count > 0)
                    DevicesCmboBox.SelectedIndex = 0;
            }
        }
    }
}
