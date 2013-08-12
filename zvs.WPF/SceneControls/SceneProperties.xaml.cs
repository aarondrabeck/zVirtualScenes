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
                //TODO: CHANGE
                //LoadCommandsAsync();
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
            this.SceneID = SceneID;
        }

        private async void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                await LoadCommandsAsync();
            }
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {

        }

        private async Task LoadCommandsAsync()
        {
            PropertiesStkPnl.Children.Clear();

            var scene = await context.Scenes
                .Include(o => o.SettingValues)
                .FirstOrDefaultAsync(sc => sc.Id == SceneID);

            if (scene == null)
                return;

            #region Scene Properties
            foreach (var sp in await context.SceneSettings.ToListAsync())
            {
                var sceneSetting = sp;
                var sceneSettingValue = await context.SceneSettingValues
                    .FirstOrDefaultAsync(v => v.SceneProperty.Id == sceneSetting.Id &&
                        v.SceneId == scene.Id);

                string _default = sceneSettingValue == null ? sceneSetting.Value : sceneSettingValue.Value;

                switch (sceneSetting.ValueType)
                {
                    case DataType.BOOL:
                        {
                            //get the current value from the value table list
                            bool DefaultValue = false;
                            bool.TryParse(_default, out DefaultValue);

                            CheckboxControl control = new CheckboxControl(sceneSetting.Name, string.Empty, DefaultValue, async (isChecked) =>
                            {
                                if (sceneSettingValue != null)
                                {
                                    sceneSettingValue.Value = isChecked.ToString();
                                }
                                else
                                {
                                    scene.SettingValues.Add(new SceneSettingValue()
                                    {
                                        SceneProperty = sceneSetting,
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
                            NumericControl control = new NumericControl(sceneSetting.Name,
                                string.Empty,
                                _default,
                                NumericControl.NumberType.Decimal,
                                async (value) =>
                                {
                                    if (sceneSettingValue != null)
                                    {
                                        sceneSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        scene.SettingValues.Add(new SceneSettingValue()
                                        {
                                            SceneProperty = sceneSetting,
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
                            NumericControl control = new NumericControl(sceneSetting.Name,
                                string.Empty,
                                _default,
                                NumericControl.NumberType.Integer,
                                async (value) =>
                                {
                                    if (sceneSettingValue != null)
                                    {
                                        sceneSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        scene.SettingValues.Add(new SceneSettingValue()
                                        {
                                            SceneProperty = sceneSetting,
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
                            NumericControl control = new NumericControl(sceneSetting.Name,
                                string.Empty,
                                _default,
                                NumericControl.NumberType.Byte,
                                async (value) =>
                                {
                                    if (sceneSettingValue != null)
                                    {
                                        sceneSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        scene.SettingValues.Add(new SceneSettingValue()
                                        {
                                            SceneProperty = sceneSetting,
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
                            NumericControl control = new NumericControl(sceneSetting.Name,
                                string.Empty,
                                _default,
                                NumericControl.NumberType.Short,
                                async (value) =>
                                {
                                    if (sceneSettingValue != null)
                                    {
                                        sceneSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        scene.SettingValues.Add(new SceneSettingValue()
                                        {
                                            SceneProperty = sceneSetting,
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
                            StringControl control = new StringControl(sceneSetting.Name,
                                string.Empty,
                                _default,
                               async (value) =>
                               {
                                   if (sceneSettingValue != null)
                                   {
                                       sceneSettingValue.Value = value;
                                   }
                                   else
                                   {
                                       scene.SettingValues.Add(new SceneSettingValue()
                                       {
                                           SceneProperty = sceneSetting,
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
                            ComboboxControl control = new ComboboxControl(sceneSetting.Name,
                                string.Empty,
                                sceneSetting.Options.Select(o => o.Name).ToList(),
                                _default,
                                async (value) =>
                                {
                                    if (sceneSettingValue != null)
                                    {
                                        sceneSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        scene.SettingValues.Add(new SceneSettingValue()
                                        {
                                            SceneProperty = sceneSetting,
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
