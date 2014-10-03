using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using zvs.WPF.DynamicActionControls;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.WPF.AdapterManager
{
    /// <summary>
    /// Interaction logic for PluginManager.xaml
    /// </summary>
    public partial class AdapterManagerWindow : Window, IDisposable
    {
        private App application = (App)Application.Current;
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));
        private zvsContext context;

        public AdapterManagerWindow()
        {
            context = new zvsContext();

            InitializeComponent();

            zvsContext.ChangeNotifications<Adapter>.onEntityAdded += AdapterManagerWindow_onEntityAdded;
            zvsContext.ChangeNotifications<Adapter>.onEntityDeleted += AdapterManagerWindow_onEntityDeleted;
            zvsContext.ChangeNotifications<Adapter>.onEntityUpdated += AdapterManagerWindow_onEntityUpdated;
        }

        void AdapterManagerWindow_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Adapter>.EntityUpdatedArgs e)
        {
            UpdateAdapterList();
        }

        void AdapterManagerWindow_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Adapter>.EntityDeletedArgs e)
        {
            UpdateAdapterList();
        }

        void AdapterManagerWindow_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Adapter>.EntityAddedArgs e)
        {
            UpdateAdapterList();
        }

#if DEBUG
        ~AdapterManagerWindow()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("AdapterManagerWindow Deconstructed.");
        }
#endif

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                var zvsEntities2ViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("zvsEntities2adapterViewSource")));

                //Get plug-ins from database where currently loaded plug-ins match plug-ins from the db.  
                //This will prevent plug-ins that are in the DB but not loaded from being configurable.
                //zvsEntities2ViewSource.Source = context.plugins.Local.Where(p=> mainWindow.manager.pluginManager.GetPlugins().Any(o => o.Name == p.name));

                //Get a list of loaded plug-ins
                UpdateAdapterList();

                //Only load the plug-in options for the plug-ins that are currently loaded.
                zvsEntities2ViewSource.Source = context.Adapters.Local;
            }
        }

        private void UpdateAdapterList()
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                var loadedAdapterGuids = application.zvsCore.AdapterManager.AdapterGuidToAdapterDictionary.Keys.ToList();
                await context.Adapters.Where(o => loadedAdapterGuids.Contains(o.AdapterGuid)).ToListAsync();
            }));
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            zvsContext.ChangeNotifications<Adapter>.onEntityAdded -= AdapterManagerWindow_onEntityAdded;
            zvsContext.ChangeNotifications<Adapter>.onEntityDeleted -= AdapterManagerWindow_onEntityDeleted;
            zvsContext.ChangeNotifications<Adapter>.onEntityUpdated -= AdapterManagerWindow_onEntityUpdated;
            context.Dispose();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PluginLstVw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ControlsStkPnl.Children.Clear();

            var adapter = (Adapter)AdapterLstVw.SelectedItem;
            if (adapter != null)
            {
                //ADD THE ENABLED BUTTON
                CheckboxControl c = new CheckboxControl(string.Format("{0} is enabled", adapter.Name),
                    "Starts and stops the selected adapter",
                    adapter.IsEnabled,
                    async (isChecked) =>
                    {
                        //Save to the database
                        adapter.IsEnabled = isChecked;

                        var result = await context.TrySaveChangesAsync();
                        if (result.HasError)
                            ((App)App.Current).zvsCore.log.Error(result.Message);

                        //STOP OR START
                        if (isChecked)
                            application.zvsCore.AdapterManager.EnableAdapterAsync(adapter.AdapterGuid);
                        else
                            application.zvsCore.AdapterManager.DisableAdapterAsync(adapter.AdapterGuid);
                    },
                icon);
                ControlsStkPnl.Children.Add(c);


                //Add all the settings
                foreach (AdapterSetting a in adapter.Settings)
                {
                    AdapterSetting adapterSetting = a;

                    switch (adapterSetting.ValueType)
                    {
                        case DataType.BOOL:
                            {
                                bool DefaultValue = false;
                                bool.TryParse(adapterSetting.Value, out DefaultValue);

                                CheckboxControl control = new CheckboxControl(adapterSetting.Name,
                                    adapterSetting.Description,
                                    DefaultValue,
                                    async (isChecked) =>
                                    {
                                        adapterSetting.Value = isChecked.ToString();
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.AdapterManager.NotifyAdapterSettingsChanged(adapterSetting);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.DECIMAL:
                            {
                                NumericControl control = new NumericControl(adapterSetting.Name,
                                    adapterSetting.Description,
                                    adapterSetting.Value,
                                    NumericControl.NumberType.Decimal,
                                    async (value) =>
                                    {
                                        adapterSetting.Value = value;
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.AdapterManager.NotifyAdapterSettingsChanged(adapterSetting);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.BYTE:
                            {
                                NumericControl control = new NumericControl(adapterSetting.Name,
                                    adapterSetting.Description,
                                    adapterSetting.Value,
                                    NumericControl.NumberType.Byte,
                                    async (value) =>
                                    {
                                        adapterSetting.Value = value;

                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.AdapterManager.NotifyAdapterSettingsChanged(adapterSetting);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.INTEGER:
                            {
                                NumericControl control = new NumericControl(adapterSetting.Name,
                                    adapterSetting.Description,
                                    adapterSetting.Value,
                                    NumericControl.NumberType.Integer,
                                    async (value) =>
                                    {
                                        adapterSetting.Value = value;

                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.AdapterManager.NotifyAdapterSettingsChanged(adapterSetting);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.SHORT:
                            {
                                NumericControl control = new NumericControl(adapterSetting.Name,
                                    adapterSetting.Description,
                                    adapterSetting.Value,
                                    NumericControl.NumberType.Short,
                                    async (value) =>
                                    {
                                        adapterSetting.Value = value;
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.AdapterManager.NotifyAdapterSettingsChanged(adapterSetting);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.COMPORT:
                            {
                                NumericControl control = new NumericControl(adapterSetting.Name,
                                    adapterSetting.Description,
                                    adapterSetting.Value,
                                    NumericControl.NumberType.ComPort,
                                    async (value) =>
                                    {
                                        adapterSetting.Value = value;
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.AdapterManager.NotifyAdapterSettingsChanged(adapterSetting);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.STRING:
                            {
                                StringControl control = new StringControl(adapterSetting.Name,
                                    adapterSetting.Description,
                                    adapterSetting.Value,
                                    async (value) =>
                                    {
                                        adapterSetting.Value = value;
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.AdapterManager.NotifyAdapterSettingsChanged(adapterSetting);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.LIST:
                            {
                                ComboboxControl control = new ComboboxControl(adapterSetting.Name,
                                    adapterSetting.Description,
                                    adapterSetting.Options.Select(o => o.Name).ToList(),
                                    adapterSetting.Value,
                                    async (value) =>
                                    {
                                        adapterSetting.Value = value;
                                        var result = await context.TrySaveChangesAsync();
                                        if (result.HasError)
                                            ((App)App.Current).zvsCore.log.Error(result.Message);

                                        application.zvsCore.AdapterManager.NotifyAdapterSettingsChanged(adapterSetting);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                    }
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.context == null)
                {
                    return;
                }

                context.Dispose();
            }
        }
    }
}
