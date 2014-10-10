using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using zvs.WPF.DynamicActionControls;
using zvs.Entities;
using System.Data.Entity;
using System.Threading.Tasks;

namespace zvs.WPF
{
    /// <summary>
    /// Interaction logic for PluginManager.xaml
    /// </summary>
    public partial class PluginManagerWindow : Window
    {
        private App application = (App)Application.Current;
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));
        private zvsContext context;

        public PluginManagerWindow()
        {
            context = new zvsContext();
            InitializeComponent();

            zvsContext.ChangeNotifications<Plugin>.OnEntityUpdated += PluginManagerWindow_onEntityUpdated;
            zvsContext.ChangeNotifications<Plugin>.OnEntityAdded += PluginManagerWindow_onEntityAdded;
            zvsContext.ChangeNotifications<Plugin>.OnEntityDeleted += PluginManagerWindow_onEntityDeleted;
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
                var zvsEntities2ViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("zvsEntities2PluginViewSource")));

                //Get a list of loaded plug-ins
                await GetLoadedPlugins();

                //Only load the plug-in options for the plug-ins that are currently loaded.
                zvsEntities2ViewSource.Source = context.Plugins.Local;
            }
        }

        private async Task GetLoadedPlugins()
        {
            var loadedPluginsGuids = application.zvsCore.PluginManager.PluginGuidToPluginDictionary.Keys.ToList();
            await context.Plugins.Where(o => loadedPluginsGuids.Contains(o.PluginGuid)).ToListAsync();
        }

        void PluginManagerWindow_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Plugin>.EntityAddedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                await GetLoadedPlugins();
            }));
        }

        void PluginManagerWindow_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Plugin>.EntityDeletedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                await GetLoadedPlugins();
            }));
        }

        void PluginManagerWindow_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Plugin>.EntityUpdatedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                await GetLoadedPlugins();
            }));
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            zvsContext.ChangeNotifications<Plugin>.OnEntityUpdated -= PluginManagerWindow_onEntityUpdated;
            zvsContext.ChangeNotifications<Plugin>.OnEntityAdded -= PluginManagerWindow_onEntityAdded;
            zvsContext.ChangeNotifications<Plugin>.OnEntityDeleted -= PluginManagerWindow_onEntityDeleted;
            context.Dispose();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PluginLstVw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ControlsStkPnl.Children.Clear();

            var plugin = (Plugin)PluginLstVw.SelectedItem;
            if (plugin != null)
            {
                //ADD THE ENABLED BUTTON
                CheckboxControl c = new CheckboxControl(string.Format("{0} is enabled", plugin.Name),
                    "Starts and stops the selected plug-in",
                    plugin.IsEnabled,
                    async (isChecked) =>
                    {
                        //Save to the database
                        plugin.IsEnabled = isChecked;

                        var result = await context.TrySaveChangesAsync();
                        if (result.HasError)
                            ((App)App.Current).zvsCore.log.Error(result.Message);

                        //STOP OR START
                        if (isChecked)
                            application.zvsCore.PluginManager.EnablePluginAsync(plugin.PluginGuid);
                        else
                            application.zvsCore.PluginManager.DisablePluginAsync(plugin.PluginGuid);
                    },
                icon);
                ControlsStkPnl.Children.Add(c);


                //Add all the settings
                foreach (var a in plugin.Settings)
                {
                    var pluginSettings = a;

                    switch (pluginSettings.ValueType)
                    {
                        case DataType.BOOL:
                            {
                                bool DefaultValue = false;
                                bool.TryParse(pluginSettings.Value, out DefaultValue);

                                CheckboxControl control = new CheckboxControl(pluginSettings.Name,
                                    pluginSettings.Description,
                                    DefaultValue,
                                    async (isChecked) =>
                                    {
                                        pluginSettings.Value = isChecked.ToString();
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.PluginManager.NotifyPluginSettingsChanged(pluginSettings);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.DECIMAL:
                            {
                                NumericControl control = new NumericControl(pluginSettings.Name,
                                    pluginSettings.Description,
                                    pluginSettings.Value,
                                    NumericControl.NumberType.Decimal,
                                    async (value) =>
                                    {
                                        pluginSettings.Value = value;
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.PluginManager.NotifyPluginSettingsChanged(pluginSettings);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.BYTE:
                            {
                                NumericControl control = new NumericControl(pluginSettings.Name,
                                    pluginSettings.Description,
                                    pluginSettings.Value,
                                    NumericControl.NumberType.Byte,
                                    async (value) =>
                                    {
                                        pluginSettings.Value = value;

                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.PluginManager.NotifyPluginSettingsChanged(pluginSettings);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.INTEGER:
                            {
                                NumericControl control = new NumericControl(pluginSettings.Name,
                                    pluginSettings.Description,
                                    pluginSettings.Value,
                                    NumericControl.NumberType.Integer,
                                    async (value) =>
                                    {
                                        pluginSettings.Value = value;

                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.PluginManager.NotifyPluginSettingsChanged(pluginSettings);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.SHORT:
                            {
                                NumericControl control = new NumericControl(pluginSettings.Name,
                                    pluginSettings.Description,
                                    pluginSettings.Value,
                                    NumericControl.NumberType.Short,
                                    async (value) =>
                                    {
                                        pluginSettings.Value = value;
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.PluginManager.NotifyPluginSettingsChanged(pluginSettings);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.COMPORT:
                            {
                                NumericControl control = new NumericControl(pluginSettings.Name,
                                    pluginSettings.Description,
                                    pluginSettings.Value,
                                    NumericControl.NumberType.ComPort,
                                    async (value) =>
                                    {
                                        pluginSettings.Value = value;
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.PluginManager.NotifyPluginSettingsChanged(pluginSettings);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.STRING:
                            {
                                StringControl control = new StringControl(pluginSettings.Name,
                                    pluginSettings.Description,
                                    pluginSettings.Value,
                                    async (value) =>
                                    {
                                        pluginSettings.Value = value;
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.PluginManager.NotifyPluginSettingsChanged(pluginSettings);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.LIST:
                            {
                                ComboboxControl control = new ComboboxControl(pluginSettings.Name,
                                    pluginSettings.Description,
                                    pluginSettings.Options.Select(o => o.Name).ToList(),
                                    pluginSettings.Value,
                                    async (value) =>
                                    {
                                        pluginSettings.Value = value;
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.PluginManager.NotifyPluginSettingsChanged(pluginSettings);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                    }
                }
            }
        }
    }
}
