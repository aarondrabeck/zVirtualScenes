using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using zvs.DataModel;
using zvs.Processor;
using zvs.WPF.DynamicActionControls;

namespace zvs.WPF.PluginManager
{
    /// <summary>
    /// Interaction logic for PluginManager.xaml
    /// </summary>
    public partial class PluginManagerWindow : IDisposable
    {
        private IFeedback<LogEntry> Log { get; set; }
        private App App { get; set; }
        private readonly BitmapImage _icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));
        private ZvsContext Context { get; set; }

        public PluginManagerWindow()
        {
            App = (App)Application.Current;
            Log = new DatabaseFeedback(App.EntityContextConnection) { Source = "Plugin Manager Window" };
            Context = new ZvsContext(App.EntityContextConnection);
            InitializeComponent();

            NotifyEntityChangeContext.ChangeNotifications<Plugin>.OnEntityUpdated += PluginManagerWindow_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<Plugin>.OnEntityAdded += PluginManagerWindow_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Plugin>.OnEntityDeleted += PluginManagerWindow_onEntityDeleted;
        }

#if DEBUG
        ~PluginManagerWindow()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("PluginManagerWindow Deconstructed.");
        }
#endif

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                var zvsEntities2ViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("ZvsEntities2PluginViewSource")));

                //Get a list of loaded plug-ins
                UpdatePluginList();

                //Only load the plug-in options for the plug-ins that are currently loaded.
                zvsEntities2ViewSource.Source = Context.Plugins.Local;
            }
        }

        private void UpdatePluginList()
        {
            if (Context == null)
                return;

            Dispatcher.Invoke(async () =>
            {
                var loadedPluginGuids = App.ZvsEngine.PluginManager.GetZvsPlugins().Select(o => o.PluginGuid).ToList();
                await Context.Plugins.Where(o => loadedPluginGuids.Contains(o.PluginGuid)).ToListAsync();
            });
        }

        void PluginManagerWindow_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Plugin>.EntityAddedArgs e)
        {
            UpdatePluginList();
        }

        void PluginManagerWindow_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Plugin>.EntityDeletedArgs e)
        {
            UpdatePluginList();
        }

        void PluginManagerWindow_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Plugin>.EntityUpdatedArgs e)
        {
            UpdatePluginList();
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            NotifyEntityChangeContext.ChangeNotifications<Plugin>.OnEntityUpdated -= PluginManagerWindow_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<Plugin>.OnEntityAdded -= PluginManagerWindow_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Plugin>.OnEntityDeleted -= PluginManagerWindow_onEntityDeleted;
            Context.Dispose();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PluginListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ControlsStkPnl.Children.Clear();

            var plugin = PluginListView.SelectedItem as Plugin;
            if (plugin == null) return;

            //ADD THE ENABLED BUTTON
            var c = new CheckboxControl(async isChecked =>
                {
                    //Save to the database
                    plugin.IsEnabled = isChecked;

                    var result = await Context.TrySaveChangesAsync(App.Cts.Token);
                    if (result.HasError)
                        await Log.ReportErrorFormatAsync(App.Cts.Token, "Error enabling plugin. {0}", result.Message);

                    //STOP OR START
                    if (isChecked)
                        await App.ZvsEngine.PluginManager.EnablePluginAsync(plugin.PluginGuid, App.Cts.Token);
                    else
                        await App.ZvsEngine.PluginManager.DisablePluginAsync(plugin.PluginGuid, App.Cts.Token);
                },
                _icon)
            {
                Header = string.Format("{0} is enabled", plugin.Name),
                Description = "Starts and stops the selected plug-in",
                Value = plugin.IsEnabled
            };
            ControlsStkPnl.Children.Add(c);


            //Add all the settings
            foreach (var a in plugin.Settings)
            {
                var pluginSettings = a;

                switch (pluginSettings.ValueType)
                {
                    case DataType.BOOL:
                        {
                            bool defaultValue;
                            bool.TryParse(pluginSettings.Value, out defaultValue);

                            var control = new CheckboxControl(async isChecked =>
                                {
                                    pluginSettings.Value = isChecked.ToString();
                                    var result = await Context.TrySaveChangesAsync(App.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(App.Cts.Token, "Error saving plugin setting. {0}", result.Message);
                                },
                                _icon)
                            {
                                Header = pluginSettings.Name,
                                Description = pluginSettings.Description,
                                Value = defaultValue
                            };
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.DECIMAL:
                    case DataType.BYTE:
                    case DataType.INTEGER:
                    case DataType.SHORT:
                    case DataType.COMPORT:
                        {
                            var control = new NumericControl(async value =>
                                {
                                    pluginSettings.Value = value;
                                    var result = await Context.TrySaveChangesAsync(App.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(App.Cts.Token, "Error saving plugin setting. {0}", result.Message);
                                },
                                        _icon, pluginSettings.ValueType)
                            {
                                Header = pluginSettings.Name,
                                Description = pluginSettings.Description,
                                Value = pluginSettings.Value
                            };
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.STRING:
                        {
                            var control = new StringControl(
                                async value =>
                                {
                                    pluginSettings.Value = value;
                                    var result = await Context.TrySaveChangesAsync(App.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(App.Cts.Token, "Error saving plugin setting. {0}", result.Message);
                                },
                                            _icon)
                            {
                                Header = pluginSettings.Name,
                                Description = pluginSettings.Description,
                                Value = pluginSettings.Value,
                            };
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.LIST:
                        {
                            var control = new ComboboxControl(pluginSettings.Name,
                                pluginSettings.Description,
                                pluginSettings.Options.Select(o => o.Name).ToList(),
                                pluginSettings.Value,
                                async value =>
                                {
                                    pluginSettings.Value = value.ToString();
                                    var result = await Context.TrySaveChangesAsync(App.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(App.Cts.Token, "Error saving plugin setting. {0}", result.Message);
                                },
                                _icon);
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (Context == null)
            {
                return;
            }

            Context.Dispose();
        }
    }
}
