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
using zvs.WPF.DynamicActionControls;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceCommands.xaml
    /// </summary>
    public partial class DeviceProperties : UserControl
    {
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));
        private zvsContext context;
        private int DeviceID = 0;

        public DeviceProperties(int DeviceID)
        {
            this.DeviceID = DeviceID;
            InitializeComponent();
        }

        private async void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            context = new zvsContext();
            await LoadCommandsAsync();
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {

        }

        private async Task LoadCommandsAsync()
        {
            PropertiesStkPnl.Children.Clear();

            var device = await context.Devices
                .Include(o=> o.DeviceSettingValues)
                .FirstOrDefaultAsync(dv => dv.Id == DeviceID);

            if (device == null)
                return;

            #region Device Properties
            foreach (var s in await context.DeviceSettings.ToListAsync())
            {
                var deviceSetting = s;

                //See if the device has a value stored for it for this property
                var deviceSettingValue = await context.DeviceSettingValues.FirstOrDefaultAsync(v => v.DeviceSetting.Id == deviceSetting.Id &&
                    v.DeviceId == device.Id);

                string _default = deviceSettingValue == null ? deviceSetting.Value : deviceSettingValue.Value;

                switch (deviceSetting.ValueType)
                {
                    case DataType.BOOL:
                        {
                            //get the current value from the value table list
                            bool DefaultValue = false;
                            bool.TryParse(_default, out DefaultValue);

                            CheckboxControl control = new CheckboxControl(deviceSetting.Name, deviceSetting.Description, DefaultValue, async (isChecked) =>
                            {
                                if (deviceSettingValue != null)
                                {
                                    deviceSettingValue.Value = isChecked.ToString();
                                }
                                else
                                {
                                    device.DeviceSettingValues.Add(new DeviceSettingValue()
                                    {
                                        DeviceSetting = deviceSetting,
                                        Value = isChecked.ToString()
                                    });
                                }

                                var result = await context.TrySaveChangesAsync();
                                if (result.HasError)
                                    ((App)App.Current).zvsCore.log.Error(result.Message);
                            },
                            icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }
                    case DataType.DECIMAL:
                        {
                            NumericControl control = new NumericControl(deviceSetting.Name,
                                deviceSetting.Description,
                                _default,
                                NumericControl.NumberType.Decimal,
                                async (value) =>
                                {
                                    if (deviceSettingValue != null)
                                    {
                                        deviceSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        device.DeviceSettingValues.Add(new DeviceSettingValue()
                                        {
                                            DeviceSetting = deviceSetting,
                                            Value = value
                                        });
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }
                    case DataType.INTEGER:
                        {
                            NumericControl control = new NumericControl(deviceSetting.Name,
                                deviceSetting.Description,
                                _default,
                                NumericControl.NumberType.Integer,
                                async  (value) =>
                                {
                                    if (deviceSettingValue != null)
                                    {
                                        deviceSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        device.DeviceSettingValues.Add(new DeviceSettingValue()
                                        {
                                            DeviceSetting = deviceSetting,
                                            Value = value
                                        });
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }
                    case DataType.BYTE:
                        {
                            NumericControl control = new NumericControl(deviceSetting.Name,
                                deviceSetting.Description,
                                _default,
                                NumericControl.NumberType.Byte,
                                async (value) =>
                                {
                                    if (deviceSettingValue != null)
                                    {
                                        deviceSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        device.DeviceSettingValues.Add(new DeviceSettingValue()
                                        {
                                            DeviceSetting = deviceSetting,
                                            Value = value
                                        });
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }
                    case DataType.SHORT:
                        {
                            NumericControl control = new NumericControl(deviceSetting.Name,
                               deviceSetting.Description,
                                _default,
                                NumericControl.NumberType.Short,
                               async (value) =>
                                {
                                    if (deviceSettingValue != null)
                                    {
                                        deviceSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        device.DeviceSettingValues.Add(new DeviceSettingValue()
                                        {
                                            DeviceSetting = deviceSetting,
                                            Value = value
                                        });
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }

                    case DataType.STRING:
                        {
                            StringControl control = new StringControl(deviceSetting.Name,
                                deviceSetting.Description,
                                _default,
                                async (value) =>
                                {
                                    if (deviceSettingValue != null)
                                    {
                                        deviceSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        device.DeviceSettingValues.Add(new DeviceSettingValue()
                                        {
                                            DeviceSetting = deviceSetting,
                                            Value = value
                                        });
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }

                    case DataType.LIST:
                        {
                            ComboboxControl control = new ComboboxControl(deviceSetting.Name,
                                deviceSetting.Description,
                                deviceSetting.Options.Select(o => o.Name).ToList(),
                                _default,
                                async (value) =>
                                {
                                    if (deviceSettingValue != null)
                                    {
                                        deviceSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        device.DeviceSettingValues.Add(new DeviceSettingValue()
                                        {
                                            DeviceSetting = deviceSetting,
                                            Value = value
                                        });
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }
                }
            }
            #endregion
        }
    }
}
