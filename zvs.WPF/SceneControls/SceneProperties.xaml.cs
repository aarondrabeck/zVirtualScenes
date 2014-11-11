using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using zvs.Processor;
using zvs.WPF.DynamicActionControls;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.WPF.SceneControls
{
    /// <summary>
    /// Interaction logic for SceneProperties.xaml
    /// </summary>
    public partial class SceneProperties
    {
        private readonly BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));
        private ZvsContext Context { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private readonly App _app = (App)Application.Current;
        public int SceneId { get; set; }

        public SceneProperties()
        {
            SceneId = 0;
            Context = new ZvsContext(_app.EntityContextConnection);
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Scene Properties" };

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            InitializeComponent();
        }

        public SceneProperties(int sceneId)
        {
            SceneId = sceneId;
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

            var scene = await Context.Scenes
                .Include(o => o.SettingValues)
                .FirstOrDefaultAsync(sc => sc.Id == SceneId);

            if (scene == null)
                return;

            #region Scene Properties
            foreach (var sp in await Context.SceneSettings.ToListAsync())
            {
                var sceneSetting = sp;
                var sceneSettingValue = await Context.SceneSettingValues
                    .FirstOrDefaultAsync(v => v.SceneSetting.Id == sceneSetting.Id &&
                        v.SceneId == scene.Id);

                var _default = sceneSettingValue == null ? sceneSetting.Value : sceneSettingValue.Value;

                switch (sceneSetting.ValueType)
                {
                    case DataType.BOOL:
                        {
                            //get the current value from the value table list
                            bool defaultValue;
                            bool.TryParse(_default, out defaultValue);

                            var control = new CheckboxControl(sceneSetting.Name, string.Empty, defaultValue, async isChecked =>
                            {
                                if (sceneSettingValue != null)
                                {
                                    sceneSettingValue.Value = isChecked.ToString();
                                }
                                else
                                {
                                    sceneSettingValue = new SceneSettingValue()
                                    {
                                        SceneSetting = sceneSetting,
                                        Value = isChecked.ToString()
                                    };
                                    scene.SettingValues.Add(sceneSettingValue);
                                }

                                var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                if (result.HasError)
                                    await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scene. {0}", result.Message);
                            },
                            icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }
                    case DataType.DECIMAL:
                        {
                            var control = new NumericControl(sceneSetting.Name,
                                string.Empty,
                                _default,
                                NumericControl.NumberType.Decimal,
                                async value =>
                                {
                                    if (sceneSettingValue != null)
                                    {
                                        sceneSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        sceneSettingValue = new SceneSettingValue()
                                        {
                                            SceneSetting = sceneSetting,
                                            Value = value
                                        };
                                        scene.SettingValues.Add(sceneSettingValue);
                                    }

                                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scene. {0}", result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }
                    case DataType.INTEGER:
                        {
                            var control = new NumericControl(sceneSetting.Name,
                                string.Empty,
                                _default,
                                NumericControl.NumberType.Integer,
                                async value =>
                                {
                                    if (sceneSettingValue != null)
                                    {
                                        sceneSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        sceneSettingValue = new SceneSettingValue()
                                        {
                                            SceneSetting = sceneSetting,
                                            Value = value
                                        };
                                        scene.SettingValues.Add(sceneSettingValue);
                                    }

                                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scene. {0}", result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }
                    case DataType.BYTE:
                        {
                            var control = new NumericControl(sceneSetting.Name,
                                string.Empty,
                                _default,
                                NumericControl.NumberType.Byte,
                                async value =>
                                {
                                    if (sceneSettingValue != null)
                                    {
                                        sceneSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        sceneSettingValue = new SceneSettingValue()
                                        {
                                            SceneSetting = sceneSetting,
                                            Value = value
                                        };
                                        scene.SettingValues.Add(sceneSettingValue);
                                    }

                                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scene. {0}", result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }
                    case DataType.SHORT:
                        {
                            var control = new NumericControl(sceneSetting.Name,
                                string.Empty,
                                _default,
                                NumericControl.NumberType.Short,
                                async value =>
                                {
                                    if (sceneSettingValue != null)
                                    {
                                        sceneSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        sceneSettingValue = new SceneSettingValue()
                                        {
                                            SceneSetting = sceneSetting,
                                            Value = value
                                        };
                                        scene.SettingValues.Add(sceneSettingValue);
                                    }

                                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scene. {0}", result.Message);
                                },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }

                    case DataType.STRING:
                        {
                            var control = new StringControl(sceneSetting.Name,
                                string.Empty,
                                _default,
                               async value =>
                               {
                                   if (sceneSettingValue != null)
                                   {
                                       sceneSettingValue.Value = value;
                                   }
                                   else
                                   {
                                       sceneSettingValue = new SceneSettingValue()
                                       {
                                           SceneSetting = sceneSetting,
                                           Value = value
                                       };
                                       scene.SettingValues.Add(sceneSettingValue);
                                   }

                                   var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                   if (result.HasError)
                                       await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scene. {0}", result.Message);
                               },
                                icon);
                            PropertiesStkPnl.Children.Add(control);

                            break;
                        }

                    case DataType.LIST:
                        {
                            var control = new ComboboxControl(sceneSetting.Name,
                                string.Empty,
                                sceneSetting.Options.Select(o => o.Name).ToList(),
                                _default,
                                async value =>
                                {
                                    if (sceneSettingValue != null)
                                    {
                                        sceneSettingValue.Value = value;
                                    }
                                    else
                                    {
                                        //assign sceneSettingValue so above null check is ran in the case this is called 2 times before the save changes hits.
                                        sceneSettingValue = new SceneSettingValue()
                                        {
                                            SceneSetting = sceneSetting,
                                            Value = value
                                        };
                                        scene.SettingValues.Add(sceneSettingValue);
                                    }

                                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scene. {0}", result.Message);
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
