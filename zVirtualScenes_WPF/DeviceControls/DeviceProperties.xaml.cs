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
using zVirtualScenesGUI.DynamicActionControls;
using zvs.Entities;


namespace zVirtualScenesGUI.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceCommands.xaml
    /// </summary>
    public partial class DeviceProperties : UserControl
    {
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenesGUI;component/Images/save_check.png"));
        private zvsContext context;
        private int DeviceID = 0;

        public DeviceProperties(int DeviceID)
        {
            this.DeviceID = DeviceID;
            InitializeComponent();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            context = new zvsContext();
            LoadCommands();
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void LoadCommands()
        {
            PropertiesStkPnl.Children.Clear();

            Device d = context.Devices.FirstOrDefault(dv => dv.DeviceId == DeviceID);
            if (d != null)
            {
                #region Device Properties
                foreach (DeviceProperty dp in context.DeviceProperties)
                {
                    DeviceProperty _dp = dp;

                    //See if the device has a value stored for it for this property
                    DevicePropertyValue _dpv = d.PropertyValues.FirstOrDefault(v => v.DeivceProperty == _dp);                    
                    string _default = _dpv == null ? _dp.Value : _dpv.Value;

                    switch (_dp.ValueType)
                    {
                        case DataType.BOOL:
                            {
                                //get the current value from the value table list
                                bool DefaultValue = false;
                                bool.TryParse(_default, out DefaultValue);

                                CheckboxControl control = new CheckboxControl(_dp.Name, _dp.Description, DefaultValue, (isChecked) =>
                                {
                                    if (_dpv != null)
                                    {
                                        _dpv.Value = isChecked.ToString();
                                    }
                                    else
                                    {
                                        d.PropertyValues.Add(new DevicePropertyValue()
                                        {
                                            DeivceProperty = _dp,
                                            Value = isChecked.ToString()
                                        });
                                    }

                                    context.SaveChanges();
                                },
                                icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.DECIMAL:
                            {
                                NumericControl control = new NumericControl(_dp.Name,
                                    _dp.Description,
                                    _default,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.Value = value;
                                        }
                                        else
                                        {
                                            d.PropertyValues.Add(new DevicePropertyValue()
                                            {
                                                DeivceProperty = _dp,
                                                Value = value
                                            });
                                        }

                                        context.SaveChanges();
                                    }, 
                                    icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.INTEGER:
                            {
                                NumericControl control = new NumericControl(_dp.Name,
                                    _dp.Description,
                                    _default,
                                    NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.Value = value;
                                        }
                                        else
                                        {
                                            d.PropertyValues.Add(new DevicePropertyValue()
                                            {
                                                DeivceProperty = _dp,
                                                Value = value
                                            });
                                        }

                                        context.SaveChanges();
                                    },
                                    icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.BYTE:
                            {
                                NumericControl control = new NumericControl(_dp.Name,
                                    _dp.Description,
                                    _default,
                                    NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.Value = value;
                                        }
                                        else
                                        {
                                            d.PropertyValues.Add(new DevicePropertyValue()
                                            {
                                                DeivceProperty = _dp,
                                                Value = value
                                            });
                                        }

                                        context.SaveChanges();
                                    },
                                    icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }
                        case DataType.SHORT:
                            {
                                NumericControl control = new NumericControl(_dp.Name,
                                   _dp.Description,
                                    _default,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.Value = value;
                                        }
                                        else
                                        {
                                            d.PropertyValues.Add(new DevicePropertyValue()
                                            {
                                                DeivceProperty = _dp,
                                                Value = value
                                            });
                                        }

                                        context.SaveChanges();
                                    },
                                    icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }

                        case DataType.STRING:
                            {
                                StringControl control = new StringControl(_dp.Name,
                                    _dp.Description,
                                    _default,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.Value = value;
                                        }
                                        else
                                        {
                                            d.PropertyValues.Add(new DevicePropertyValue()
                                            {
                                                DeivceProperty = _dp,
                                                Value = value
                                            });
                                        }

                                        context.SaveChanges();
                                    },
                                    icon);
                                PropertiesStkPnl.Children.Add(control);

                                break;
                            }

                        case DataType.LIST:
                            {
                                ComboboxControl control = new ComboboxControl(_dp.Name,
                                    _dp.Description,
                                    _dp.Options.Select(o => o.Name).ToList(),
                                    _default,
                                    (value) =>
                                    {
                                        if (_dpv != null)
                                        {
                                            _dpv.Value = value;
                                        }
                                        else
                                        {
                                            d.PropertyValues.Add(new DevicePropertyValue()
                                            {
                                                DeivceProperty = _dp,
                                                Value = value
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
