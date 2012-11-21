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


namespace zvs.WPF.SceneControls
{
    /// <summary>
    /// Interaction logic for SceneProperties.xaml
    /// </summary>
    public partial class SceneProperties : UserControl
    {
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));
        private zvsContext context = null;


        private int _SceneID = 0;
        public int SceneID
        {
            get
            {
                return this._SceneID;
            }
            set
            {
                this._SceneID = value;
                LoadCommands();
            }
        }

        public SceneProperties()
        {
            
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                context = new zvsContext();
            }
        }

        public SceneProperties(int SceneID)
            : base()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.SceneID = SceneID;
                LoadCommands();
            }
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            LoadCommands();
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void LoadCommands()
        {
            PropertiesStkPnl.Children.Clear();

            Scene s = context.Scenes.FirstOrDefault(sc => sc.Id == SceneID);
            if (s != null)
            {
                #region Scene Properties
                foreach (SceneProperty sp in context.SceneProperties)
                {
                    SceneProperty _property = sp;
                    ScenePropertyValue _property_value = s.PropertyValues.FirstOrDefault(v => v.SceneProperty == _property);
                    string _default = _property_value == null ? _property.Value : _property_value.Value;

                    switch (_property.ValueType)
                    {
                        case DataType.BOOL:
                            {
                                //get the current value from the value table list
                                bool DefaultValue = false;
                                bool.TryParse(_default, out DefaultValue);

                                CheckboxControl control = new CheckboxControl(_property.Name, string.Empty, DefaultValue, (isChecked) =>
                                {
                                    if (_property_value != null)
                                    {
                                        _property_value.Value = isChecked.ToString();
                                    }
                                    else
                                    {
                                        s.PropertyValues.Add(new ScenePropertyValue()
                                        {
                                            SceneProperty = _property,
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
                                NumericControl control = new NumericControl(_property.Name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.Value = value;
                                        }
                                        else
                                        {
                                            s.PropertyValues.Add(new ScenePropertyValue()
                                            {
                                                SceneProperty = _property,
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
                                NumericControl control = new NumericControl(_property.Name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.Value = value;
                                        }
                                        else
                                        {
                                            s.PropertyValues.Add(new ScenePropertyValue()
                                            {
                                                SceneProperty = _property,
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
                                NumericControl control = new NumericControl(_property.Name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.Value = value;
                                        }
                                        else
                                        {
                                            s.PropertyValues.Add(new ScenePropertyValue()
                                            {
                                                SceneProperty = _property,
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
                                NumericControl control = new NumericControl(_property.Name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.Value = value;
                                        }
                                        else
                                        {
                                            s.PropertyValues.Add(new ScenePropertyValue()
                                            {
                                                SceneProperty = _property,
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
                                StringControl control = new StringControl(_property.Name,
                                    string.Empty,
                                    _default,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.Value = value;
                                        }
                                        else
                                        {
                                            s.PropertyValues.Add(new ScenePropertyValue()
                                            {
                                                SceneProperty = _property,
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
                                ComboboxControl control = new ComboboxControl(_property.Name,
                                    string.Empty,
                                    _property.Options.Select(o => o.Name).ToList(),
                                    _default,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.Value = value;
                                        }
                                        else
                                        {
                                            s.PropertyValues.Add(new ScenePropertyValue()
                                            {
                                                SceneProperty = _property,
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
