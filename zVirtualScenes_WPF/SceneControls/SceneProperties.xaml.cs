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
using zVirtualScenesModel;

namespace zVirtualScenesGUI.SceneControls
{
    /// <summary>
    /// Interaction logic for SceneProperties.xaml
    /// </summary>
    public partial class SceneProperties : UserControl
    {
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenesGUI;component/Images/save_check.png"));
        private zvsLocalDBEntities context = null;


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
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                context = new zvsLocalDBEntities();
            }
        }

        public SceneProperties(int SceneID)
            : base()
        {
            this.SceneID = SceneID;
            LoadCommands();
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

            scene s = context.scenes.FirstOrDefault(sc => sc.id == SceneID);
            if (s != null)
            {
                #region Scene Properties
                foreach (scene_property sp in context.scene_property)
                {
                    scene_property _property = sp;
                    scene_property_value _property_value = s.scene_property_value.FirstOrDefault(v => v.scene_property_id == _property.id);
                    string _default = _property_value == null ? _property.defualt_value : _property_value.value;

                    switch ((Data_Types)_property.value_data_type)
                    {
                        case Data_Types.BOOL:
                            {
                                //get the current value from the value table list
                                bool DefaultValue = false;
                                bool.TryParse(_default, out DefaultValue);

                                CheckboxControl control = new CheckboxControl(_property.friendly_name, string.Empty, DefaultValue, (isChecked) =>
                                {
                                    if (_property_value != null)
                                    {
                                        _property_value.value = isChecked.ToString();
                                    }
                                    else
                                    {
                                        s.scene_property_value.Add(new scene_property_value()
                                        {
                                            scene_property_id = _property.id,
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
                                NumericControl control = new NumericControl(_property.friendly_name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.value = value;
                                        }
                                        else
                                        {
                                            s.scene_property_value.Add(new scene_property_value()
                                            {
                                                scene_property_id = _property.id,
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
                                NumericControl control = new NumericControl(_property.friendly_name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.value = value;
                                        }
                                        else
                                        {
                                            s.scene_property_value.Add(new scene_property_value()
                                            {
                                                scene_property_id = _property.id,
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
                                NumericControl control = new NumericControl(_property.friendly_name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.value = value;
                                        }
                                        else
                                        {
                                            s.scene_property_value.Add(new scene_property_value()
                                            {
                                                scene_property_id = _property.id,
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
                                NumericControl control = new NumericControl(_property.friendly_name,
                                    string.Empty,
                                    _default,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.value = value;
                                        }
                                        else
                                        {
                                            s.scene_property_value.Add(new scene_property_value()
                                            {
                                                scene_property_id = _property.id,
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
                                StringControl control = new StringControl(_property.friendly_name,
                                    string.Empty,
                                    _default,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.value = value;
                                        }
                                        else
                                        {
                                            s.scene_property_value.Add(new scene_property_value()
                                            {
                                                scene_property_id = _property.id,
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
                                ComboboxControl control = new ComboboxControl(_property.friendly_name,
                                    string.Empty,
                                    _property.scene_property_option.Select(o => o.options).ToList(),
                                    _default,
                                    (value) =>
                                    {
                                        if (_property_value != null)
                                        {
                                            _property_value.value = value;
                                        }
                                        else
                                        {
                                            s.scene_property_value.Add(new scene_property_value()
                                            {
                                                scene_property_id = _property.id,
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
