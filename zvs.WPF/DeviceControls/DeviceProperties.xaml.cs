using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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

        public NumericControl.NumberType GetNumberControlType(DataType dataType)
        {
            if (dataType == DataType.INTEGER)
                return NumericControl.NumberType.Integer;
            else if (dataType == DataType.DECIMAL)
                return NumericControl.NumberType.Decimal;
            else if (dataType == DataType.BYTE)
                return NumericControl.NumberType.Byte;
            else
                return NumericControl.NumberType.Short;
        }

        private async Task LoadCommandsAsync()
        {
            PropertiesStkPnl.Children.Clear();

            var device = await context.Devices
                .Include(o => o.DeviceSettingValues)
                .Include(o => o.Type.Settings)
                .FirstOrDefaultAsync(dv => dv.Id == DeviceID);

            if (device == null)
                return;

            #region Device Type Settings
            foreach (var deviceTypeSetting in device.Type.Settings)
            {
                //default 
                string value = deviceTypeSetting.Value;

                //check if this settings has already been set
                var deviceTypeSettingValue = await context.DeviceTypeSettingValues
                    .Where(o => o.DeviceId == device.Id)
                    .FirstOrDefaultAsync(o => o.DeviceTypeSettingId == deviceTypeSetting.Id);

                if (deviceTypeSettingValue != null)
                    value = deviceTypeSettingValue.Value;

                switch (deviceTypeSetting.ValueType)
                {
                    case DataType.BOOL:
                        {
                            #region CheckboxControl
                            //get the current value from the value table list
                            bool DefaultValue = false;
                            bool.TryParse(value, out DefaultValue);

                            CheckboxControl control = new CheckboxControl(deviceTypeSetting.Name, deviceTypeSetting.Description, DefaultValue,
                            async (isChecked) =>
                            {
                                if (deviceTypeSettingValue != null)
                                    deviceTypeSettingValue.Value = isChecked.ToString();
                                else
                                {
                                    deviceTypeSettingValue = new DeviceTypeSettingValue()
                                    {
                                        DeviceId = device.Id,
                                        DeviceTypeSettingId = deviceTypeSetting.Id,
                                        Value = isChecked.ToString()
                                    };

                                    context.DeviceTypeSettingValues.Add(deviceTypeSettingValue);
                                }

                                var result = await context.TrySaveChangesAsync();
                                if (result.HasError)
                                    ((App)App.Current).zvsCore.log.Error(result.Message);
                            },
                            icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }
                    case DataType.INTEGER:
                    case DataType.DECIMAL:
                    case DataType.SHORT:
                    case DataType.BYTE:
                        {
                            #region NumericControl
                            NumericControl control = new NumericControl(deviceTypeSetting.Name, deviceTypeSetting.Description, value,
                                GetNumberControlType(deviceTypeSetting.ValueType),
                                async (v) =>
                                {
                                    if (deviceTypeSettingValue != null)
                                        deviceTypeSettingValue.Value = v;
                                    else
                                    {
                                        deviceTypeSettingValue = new DeviceTypeSettingValue()
                                        {
                                            DeviceId = device.Id,
                                            DeviceTypeSettingId = deviceTypeSetting.Id,
                                            Value = v
                                        };
                                        context.DeviceTypeSettingValues.Add(deviceTypeSettingValue);
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }
                    case DataType.STRING:
                        {
                            #region StringControl
                            StringControl control = new StringControl(deviceTypeSetting.Name, deviceTypeSetting.Description, value,
                                async (v) =>
                                {
                                    if (deviceTypeSettingValue != null)
                                        deviceTypeSettingValue.Value = v;
                                    else
                                    {
                                        deviceTypeSettingValue = new DeviceTypeSettingValue()
                                        {
                                            DeviceId = device.Id,
                                            DeviceTypeSettingId = deviceTypeSetting.Id,
                                            Value = v
                                        };
                                        context.DeviceTypeSettingValues.Add(deviceTypeSettingValue);
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }

                    case DataType.LIST:
                        {
                            #region ComboboxControl
                            ComboboxControl control = new ComboboxControl(deviceTypeSetting.Name,
                                deviceTypeSetting.Description,
                                deviceTypeSetting.Options.Select(o => o.Name).ToList(),
                                value,
                                async (v) =>
                                {
                                    if (deviceTypeSettingValue != null)
                                        deviceTypeSettingValue.Value = v;
                                    else
                                    {
                                        deviceTypeSettingValue = new DeviceTypeSettingValue()
                                        {
                                            DeviceId = device.Id,
                                            DeviceTypeSettingId = deviceTypeSetting.Id,
                                            Value = v
                                        };
                                        context.DeviceTypeSettingValues.Add(deviceTypeSettingValue);
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }
                }
            }


            #endregion
            #region Device Settings
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
                            #region CheckboxControl
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
                                    deviceSettingValue = new DeviceSettingValue()
                                    {
                                        DeviceSetting = deviceSetting,
                                        Value = isChecked.ToString()
                                    };
                                    device.DeviceSettingValues.Add(deviceSettingValue);
                                }

                                var result = await context.TrySaveChangesAsync();
                                if (result.HasError)
                                    ((App)App.Current).zvsCore.log.Error(result.Message);
                            },
                            icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }
                    case DataType.INTEGER:
                    case DataType.DECIMAL:
                    case DataType.SHORT:
                    case DataType.BYTE:
                        {
                            #region NumericControl
                            NumericControl control = new NumericControl(deviceSetting.Name,
                                deviceSetting.Description,
                                _default,
                               GetNumberControlType(deviceSetting.ValueType),
                                async (value) =>
                                {
                                    if (deviceSettingValue != null)
                                    {
                                        deviceSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        deviceSettingValue = new DeviceSettingValue()
                                        {
                                            DeviceSetting = deviceSetting,
                                            Value = value
                                        };
                                        device.DeviceSettingValues.Add(deviceSettingValue);
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }
                    case DataType.STRING:
                        {
                            #region StringControl
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
                                        deviceSettingValue = new DeviceSettingValue()
                                        {
                                            DeviceSetting = deviceSetting,
                                            Value = value
                                        };
                                        device.DeviceSettingValues.Add(deviceSettingValue);
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }

                    case DataType.LIST:
                        {
                            #region ComboboxControl
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
                                        deviceSettingValue = new DeviceSettingValue()
                                        {
                                            DeviceSetting = deviceSetting,
                                            Value = value
                                        };
                                        device.DeviceSettingValues.Add(deviceSettingValue);
                                    }

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }
                }
            }
            #endregion
        }
    }
}
