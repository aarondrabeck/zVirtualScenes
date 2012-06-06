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
using zVirtualScenes_WPF.DynamicActionControls;
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceCommands.xaml
    /// </summary>
    public partial class DeviceProperties : UserControl
    {
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes_WPF;component/Images/save_check.png"));
        private zvsLocalDBEntities context;
        private int DeviceID = 0;

        public DeviceProperties(int DeviceID)
        {
            this.DeviceID = DeviceID;
            InitializeComponent();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            context = new zvsLocalDBEntities();
            LoadCommands();
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            context.Dispose();
        }

        private void LoadCommands()
        {
            PropertiesStkPnl.Children.Clear();

            device d = context.devices.FirstOrDefault(dv => dv.id == DeviceID);
            if (d != null)
            {
                #region Device Properties
                foreach (device_propertys dp in context.device_propertys)
                {
                    device_propertys _dp = dp;
                    device_property_values _dpv = d.device_property_values.FirstOrDefault(v => v.device_property_id == _dp.id);
                    string _default = _dpv == null ? _dp.default_value : _dpv.value;

                    switch ((Data_Types)_dp.value_data_type)
                    {
                        case Data_Types.BOOL:
                            {
                                //get the current value from the value table list
                                bool DefaultValue = false;
                                bool.TryParse(_default, out DefaultValue);

                                CheckboxControl control = new CheckboxControl(_dp.friendly_name, string.Empty, DefaultValue, (isChecked) =>
                                {
                                    if (_dpv != null)
                                    {
                                        _dpv.value = isChecked.ToString();
                                    }
                                    else
                                    {
                                        d.device_property_values.Add(new device_property_values()
                                        {
                                            device_property_id = _dp.id,
                                            value = isChecked.ToString()
                                        });
                                    }

                                    context.SaveChanges();
                                },
                                icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.DECIMAL:
                            {
                                NumericControl control = new NumericControl(_dp.friendly_name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.value = value;
                                        }
                                        else
                                        {
                                            d.device_property_values.Add(new device_property_values()
                                            {
                                                device_property_id = _dp.id,
                                                value = value
                                            });
                                        }

                                        context.SaveChanges();
                                    }, 
                                    icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.INTEGER:
                            {
                                NumericControl control = new NumericControl(_dp.friendly_name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.value = value;
                                        }
                                        else
                                        {
                                            d.device_property_values.Add(new device_property_values()
                                            {
                                                device_property_id = _dp.id,
                                                value = value
                                            });
                                        }

                                        context.SaveChanges();
                                    },
                                    icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.BYTE:
                            {
                                NumericControl control = new NumericControl(_dp.friendly_name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.value = value;
                                        }
                                        else
                                        {
                                            d.device_property_values.Add(new device_property_values()
                                            {
                                                device_property_id = _dp.id,
                                                value = value
                                            });
                                        }

                                        context.SaveChanges();
                                    },
                                    icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.SHORT:
                            {
                                NumericControl control = new NumericControl(_dp.friendly_name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.value = value;
                                        }
                                        else
                                        {
                                            d.device_property_values.Add(new device_property_values()
                                            {
                                                device_property_id = _dp.id,
                                                value = value
                                            });
                                        }

                                        context.SaveChanges();
                                    },
                                    icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }

                        case Data_Types.STRING:
                            {
                                StringControl control = new StringControl(_dp.friendly_name,
                                    string.Empty,
                                    _default,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.value = value;
                                        }
                                        else
                                        {
                                            d.device_property_values.Add(new device_property_values()
                                            {
                                                device_property_id = _dp.id,
                                                value = value
                                            });
                                        }

                                        context.SaveChanges();
                                    },
                                    icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }

                        case Data_Types.LIST:
                            {
                                ComboboxControl control = new ComboboxControl(_dp.friendly_name,
                                    string.Empty,
                                    _dp.device_property_options.Select(o => o.name).ToList(),
                                    _default,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.value = value;
                                        }
                                        else
                                        {
                                            d.device_property_values.Add(new device_property_values()
                                            {
                                                device_property_id = _dp.id,
                                                value = value
                                            });
                                        }

                                        context.SaveChanges();
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
}
