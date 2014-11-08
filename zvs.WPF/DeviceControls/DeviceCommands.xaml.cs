using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using zvs.WPF.DynamicActionControls;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceCommands.xaml
    /// </summary>
    public partial class DeviceCommands : UserControl, IDisposable
    {
        readonly App _app = (App)Application.Current;
        private readonly BitmapImage _icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/send_signal.png"));
        private ZvsContext _context;
        private readonly int _deviceId = 0;

        public DeviceCommands(int deviceId)
        {
            _deviceId = deviceId;
            InitializeComponent();
        }

        private async void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            _context = new ZvsContext(_app.EntityContextConnection);
            await LoadCommandsAsync();
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {

        }

        private async Task LoadCommandsAsync()
        {
            DeviceCommandsStkPnl.Children.Clear();
            TypeCommandsStkPnl.Children.Clear();

            var d = await _context.Devices
                .Include(o => o.Values)
                .Include(o => o.Type.Commands)
                .FirstOrDefaultAsync(dv => dv.Id == _deviceId);

            if (d != null)
            {
                #region Device Commands
                foreach (var dc in d.Commands.OrderByDescending(c => c.SortOrder))
                {
                    var deviceCommand = dc;
                    var tip = string.Format("{0} (Device Id:{1},Command Id: {2})", deviceCommand.Description, deviceCommand.Device.Id, deviceCommand.Id);
                    switch (deviceCommand.ArgumentType)
                    {
                        case DataType.NONE:
                            {
                                var bc = new ButtonControl(deviceCommand.Name, deviceCommand.Description, async () =>
                                {
                                    await _app.ZvsEngine.RunCommandAsync(deviceCommand.Id, string.Empty, string.Empty, CancellationToken.None);
                                }, _icon) { ToolTip = tip };
                                DeviceCommandsStkPnl.Children.Add(bc);
                                break;
                            }
                        case DataType.BOOL:
                            {
                                //get the current value from the value table list
                                var defaultValue = false;
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceCommand.CustomData2);
                                if (dv != null)
                                {
                                    bool.TryParse(dv.Value, out defaultValue);
                                }

                                var control = new CheckboxControl(deviceCommand.Name, deviceCommand.Description, defaultValue, async isChecked =>
                                {
                                    await _app.ZvsEngine.RunCommandAsync(deviceCommand.Id, isChecked.ToString(), string.Empty, CancellationToken.None);
                                }, _icon) { ToolTip = tip };
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.DECIMAL:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceCommand.CustomData2);
                                if (dv != null)
                                    defaultValue = dv.Value;

                                var control = new NumericControl(deviceCommand.Name,
                                    deviceCommand.Description,
                                    defaultValue,
                                    NumericControl.NumberType.Decimal,
                                    async value =>
                                    {
                                        await _app.ZvsEngine.RunCommandAsync(deviceCommand.Id, value, string.Empty, CancellationToken.None);
                                    },
                                    _icon) { ToolTip = tip };
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.INTEGER:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }

                                var control = new NumericControl(deviceCommand.Name,
                                    deviceCommand.Description,
                                    defaultValue,
                                    NumericControl.NumberType.Integer,
                                    async value =>
                                    {
                                        await _app.ZvsEngine.RunCommandAsync(deviceCommand.Id, value, string.Empty, CancellationToken.None);
                                    },
                                    _icon) { ToolTip = tip };
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.BYTE:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }

                                var control = new NumericControl(deviceCommand.Name,
                                    deviceCommand.Description,
                                    defaultValue,
                                    NumericControl.NumberType.Byte,
                                    async value =>
                                    {
                                        await _app.ZvsEngine.RunCommandAsync(deviceCommand.Id, value, string.Empty, CancellationToken.None);
                                    },
                                    _icon) { ToolTip = tip };
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.SHORT:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }

                                var control = new NumericControl(deviceCommand.Name,
                                    deviceCommand.Description,
                                    defaultValue,
                                    NumericControl.NumberType.Short,
                                    async value =>
                                    {
                                        await _app.ZvsEngine.RunCommandAsync(deviceCommand.Id, value, string.Empty, CancellationToken.None);
                                    },
                                    _icon) { ToolTip = tip };
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }

                        case DataType.STRING:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }

                                var control = new StringControl(deviceCommand.Name,
                                    deviceCommand.Description,
                                    defaultValue,
                                    async value =>
                                    {
                                        await _app.ZvsEngine.RunCommandAsync(deviceCommand.Id, value, string.Empty, CancellationToken.None);
                                    },
                                    _icon) { ToolTip = tip };
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.LIST:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }

                                var control = new ComboboxControl(deviceCommand.Name,
                                    deviceCommand.Description,
                                    deviceCommand.Options.Select(o => o.Name).ToList(),
                                    defaultValue,
                                    async value =>
                                    {
                                        await _app.ZvsEngine.RunCommandAsync(deviceCommand.Id, value, string.Empty, CancellationToken.None);
                                    },
                                    _icon) { ToolTip = tip };
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                    }
                }
                #endregion

                #region Device Type Commands
                foreach (var dtc in d.Type.Commands.OrderByDescending(c => c.SortOrder))
                {
                    var deviceTypeCommand = dtc;
                    switch (deviceTypeCommand.ArgumentType)
                    {
                        case DataType.NONE:
                            {
                                var bc = new ButtonControl(deviceTypeCommand.Name, deviceTypeCommand.Description, async () =>
                                {
                                    await _app.ZvsEngine.RunCommandAsync(deviceTypeCommand.Id, string.Empty, d.Id.ToString(CultureInfo.InvariantCulture), CancellationToken.None);
                                },
                                    _icon);
                                TypeCommandsStkPnl.Children.Add(bc);
                                break;
                            }
                        case DataType.BOOL:
                            {
                                //get the current value from the value table list
                                var defaultValue = false;
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceTypeCommand.CustomData2);
                                if (dv != null)
                                {
                                    bool.TryParse(dv.Value, out defaultValue);
                                }

                                var control = new CheckboxControl(deviceTypeCommand.Name, deviceTypeCommand.Description, defaultValue, async isChecked =>
                                {
                                    await _app.ZvsEngine.RunCommandAsync(deviceTypeCommand.Id, isChecked.ToString(), d.Id.ToString(CultureInfo.InvariantCulture), CancellationToken.None);
                                },
                                    _icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.DECIMAL:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceTypeCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }

                                var control = new NumericControl(deviceTypeCommand.Name,
                                    deviceTypeCommand.Description,
                                    defaultValue,
                                    NumericControl.NumberType.Decimal,
                                   async value =>
                                   {
                                       await _app.ZvsEngine.RunCommandAsync(deviceTypeCommand.Id, value, d.Id.ToString(CultureInfo.InvariantCulture), CancellationToken.None);
                                   },
                                    _icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.INTEGER:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceTypeCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }

                                var control = new NumericControl(deviceTypeCommand.Name,
                                    deviceTypeCommand.Description,
                                    defaultValue,
                                   NumericControl.NumberType.Integer,
                                    async value =>
                                    {
                                        await _app.ZvsEngine.RunCommandAsync(deviceTypeCommand.Id, value, d.Id.ToString(CultureInfo.InvariantCulture), CancellationToken.None);
                                    },
                                    _icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.SHORT:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceTypeCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }

                                var control = new NumericControl(deviceTypeCommand.Name,
                                    deviceTypeCommand.Description,
                                    defaultValue,
                                    NumericControl.NumberType.Short,
                                    async value =>
                                    {
                                        await _app.ZvsEngine.RunCommandAsync(deviceTypeCommand.Id, value, d.Id.ToString(CultureInfo.InvariantCulture), CancellationToken.None);
                                    },
                                    _icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.BYTE:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceTypeCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }

                                var control = new NumericControl(deviceTypeCommand.Name,
                                    deviceTypeCommand.Description,
                                    defaultValue,
                                   NumericControl.NumberType.Byte,
                                    async value =>
                                    {
                                        await _app.ZvsEngine.RunCommandAsync(deviceTypeCommand.Id, value, d.Id.ToString(CultureInfo.InvariantCulture), CancellationToken.None);
                                    },
                                    _icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.STRING:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceTypeCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }

                                var control = new StringControl(deviceTypeCommand.Name,
                                    deviceTypeCommand.Description,
                                    defaultValue,
                                    async value =>
                                    {
                                        await _app.ZvsEngine.RunCommandAsync(deviceTypeCommand.Id, value, d.Id.ToString(CultureInfo.InvariantCulture), CancellationToken.None);
                                    },
                                    _icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.LIST:
                            {
                                //get the current value from the value table list
                                var defaultValue = "0";
                                var dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == deviceTypeCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }

                                var control = new ComboboxControl(deviceTypeCommand.Name,
                                    deviceTypeCommand.Description,
                                    deviceTypeCommand.Options.Select(o => o.Name).ToList(),
                                    defaultValue,
                                    async value =>
                                    {
                                        await _app.ZvsEngine.RunCommandAsync(deviceTypeCommand.Id, value, d.Id.ToString(CultureInfo.InvariantCulture), CancellationToken.None);
                                    },
                                    _icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                    }
                }
                #endregion
            }

            if (DeviceCommandsStkPnl.Children.Count == 0)
                DeviceGrpBx.Visibility = Visibility.Collapsed;

            if (TypeCommandsStkPnl.Children.Count == 0)
                TypeGrpBx.Visibility = Visibility.Collapsed;

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_context == null)
                return;
            
            _context.Dispose();
        }
    }
}
