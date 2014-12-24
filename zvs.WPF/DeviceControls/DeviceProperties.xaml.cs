using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using zvs.Processor;
using zvs.WPF.DynamicActionControls;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceCommands.xaml
    /// </summary>
    public partial class DeviceProperties
    {
        private IFeedback<LogEntry> Log { get; set; }
        private readonly App _app = (App)Application.Current;
        private readonly BitmapImage _icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));
        private ZvsContext Context { get; set; }
        private readonly int _deviceId;

        public DeviceProperties(int deviceId)
        {
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Device Property Window" };
            _deviceId = deviceId;
            InitializeComponent();
        }

        private async void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            Context = new ZvsContext(_app.EntityContextConnection);
            await LoadCommandsAsync();
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
        }

        private async Task LoadCommandsAsync()
        {
            PropertiesStkPnl.Children.Clear();

            var device = await Context.Devices
                .Include(o => o.DeviceSettingValues)
                .Include(o => o.Type.Settings)
                .FirstOrDefaultAsync(dv => dv.Id == _deviceId);

            if (device == null)
                return;

            #region Device Type Settings
            foreach (var deviceTypeSetting in device.Type.Settings)
            {
                //default 
                var value = deviceTypeSetting.Value;

                //check if this settings has already been set
                var setting = deviceTypeSetting;
                var deviceTypeSettingValue = await Context.DeviceTypeSettingValues
                    .Where(o => o.DeviceId == device.Id)
                    .FirstOrDefaultAsync(o => o.DeviceTypeSettingId == setting.Id);

                if (deviceTypeSettingValue != null)
                    value = deviceTypeSettingValue.Value;

                switch (deviceTypeSetting.ValueType)
                {
                    case DataType.BOOL:
                        {
                            #region CheckboxControl
                            //get the current value from the value table list
                            bool defaultValue;
                            bool.TryParse(value, out defaultValue);

                            var control = new CheckboxControl(async isChecked =>
                            {
                                if (deviceTypeSettingValue != null)
                                    deviceTypeSettingValue.Value = isChecked.ToString();
                                else
                                {
                                    deviceTypeSettingValue = new DeviceTypeSettingValue
                                    {
                                        DeviceId = device.Id,
                                        DeviceTypeSettingId = setting.Id,
                                        Value = isChecked.ToString()
                                    };

                                    Context.DeviceTypeSettingValues.Add(deviceTypeSettingValue);
                                }

                                var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                if (result.HasError)
                                    await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving device type setting. {0}", result.Message);

                            },
                                    _icon)
                            {
                                Header = deviceTypeSetting.Name,
                                Description = deviceTypeSetting.Description,
                                Value = defaultValue
                            };
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
                            var control = new NumericControl(
                                async v =>
                                {
                                    if (deviceTypeSettingValue != null)
                                        deviceTypeSettingValue.Value = v;
                                    else
                                    {
                                        deviceTypeSettingValue = new DeviceTypeSettingValue()
                                        {
                                            DeviceId = device.Id,
                                            DeviceTypeSettingId = setting.Id,
                                            Value = v
                                        };
                                        Context.DeviceTypeSettingValues.Add(deviceTypeSettingValue);
                                    }

                                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving device type setting. {0}", result.Message);
                                },
                                    _icon, deviceTypeSetting.ValueType)
                            {
                                Header = deviceTypeSetting.Name,
                                Description = deviceTypeSetting.Description,
                                Value = value
                            };
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }
                    case DataType.STRING:
                        {
                            #region StringControl
                            var control = new StringControl(async v =>
                                {
                                    if (deviceTypeSettingValue != null)
                                        deviceTypeSettingValue.Value = v;
                                    else
                                    {
                                        deviceTypeSettingValue = new DeviceTypeSettingValue()
                                        {
                                            DeviceId = device.Id,
                                            DeviceTypeSettingId = setting.Id,
                                            Value = v
                                        };
                                        Context.DeviceTypeSettingValues.Add(deviceTypeSettingValue);
                                    }

                                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving device type setting. {0}", result.Message);
                                },
                                        _icon)
                            {
                                Header = deviceTypeSetting.Name,
                                Description = deviceTypeSetting.Description,
                                Value = value,
                            };
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }

                    case DataType.LIST:
                        {
                            #region ComboboxControl
                            var control = new ComboboxControl(async v =>
                                {
                                    if (deviceTypeSettingValue != null)
                                        deviceTypeSettingValue.Value = v.ToString();
                                    else
                                    {
                                        deviceTypeSettingValue = new DeviceTypeSettingValue
                                        {
                                            DeviceId = device.Id,
                                            DeviceTypeSettingId = setting.Id,
                                            Value = v.ToString()
                                        };
                                        Context.DeviceTypeSettingValues.Add(deviceTypeSettingValue);
                                    }

                                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving device type setting. {0}", result.Message);
                                },
                                 _icon,
                           deviceTypeSetting.Options.Select(o => o.Name).ToList())
                            {
                                Header = deviceTypeSetting.Name,
                                Description = deviceTypeSetting.Description,
                                SelectedItem = value
                            };
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }
                }
            }


            #endregion
            #region Device Settings
            foreach (var s in await Context.DeviceSettings.ToListAsync())
            {
                var deviceSetting = s;

                //See if the device has a value stored for it for this property
                var deviceSettingValue = await Context.DeviceSettingValues.FirstOrDefaultAsync(v => v.DeviceSetting.Id == deviceSetting.Id &&
                    v.DeviceId == device.Id);

                var _default = deviceSettingValue == null ? deviceSetting.Value : deviceSettingValue.Value;

                switch (deviceSetting.ValueType)
                {
                    case DataType.BOOL:
                        {
                            #region CheckboxControl
                            //get the current value from the value table list
                            bool defaultValue;
                            bool.TryParse(_default, out defaultValue);

                            var control = new CheckboxControl(async isChecked =>
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

                                var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                if (result.HasError)
                                    await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving device setting. {0}", result.Message);
                            },
                                        _icon)
                            {
                                Header = deviceSetting.Name,
                                Description = deviceSetting.Description,
                                Value = defaultValue
                            };
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
                            var control = new NumericControl(async value =>
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

                                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving device setting. {0}", result.Message);
                                }, _icon, deviceSetting.ValueType)
                            {
                                Header = deviceSetting.Name,
                                Description = deviceSetting.Description,
                                Value = _default
                            };
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }
                    case DataType.STRING:
                        {
                            #region StringControl
                            var control = new StringControl(
                                async value =>
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

                                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving device setting. {0}", result.Message);
                                },
                                        _icon)
                            {
                                Header = deviceSetting.Name,
                                Description = deviceSetting.Description,
                                Value = _default,
                            };
                            PropertiesStkPnl.Children.Add(control);

                            break;
                            #endregion
                        }

                    case DataType.LIST:
                        {
                            #region ComboboxControl
                            var control = new ComboboxControl(async value =>
                                {
                                    if (deviceSettingValue != null)
                                    {
                                        deviceSettingValue.Value = value.ToString();
                                    }
                                    else
                                    {
                                        deviceSettingValue = new DeviceSettingValue
                                        {
                                            DeviceSetting = deviceSetting,
                                            Value = value.ToString()
                                        };
                                        device.DeviceSettingValues.Add(deviceSettingValue);
                                    }

                                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving device setting. {0}", result.Message);
                                },
                                _icon,
                                deviceSetting.Options.Select(o => o.Name).ToList())
                            {
                                Header = deviceSetting.Name,
                                Description = deviceSetting.Description,
                                SelectedItem = _default
                            };
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
