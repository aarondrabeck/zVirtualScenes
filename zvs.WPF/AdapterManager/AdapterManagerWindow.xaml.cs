using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using zvs.DataModel;
using zvs.Processor;
using zvs.WPF.DynamicActionControls;
using System.Data.Entity;

namespace zvs.WPF.AdapterManager
{
    /// <summary>
    /// Interaction logic for PluginManager.xaml
    /// </summary>
    public partial class AdapterManagerWindow : IDisposable
    {
        private IFeedback<LogEntry> Log { get; set; }
        private App App { get; set; }
        private readonly BitmapImage _icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));
        private ZvsContext Context { get; set; }

        public AdapterManagerWindow()
        {
            App = (App)Application.Current;
            Log = new DatabaseFeedback(App.EntityContextConnection) { Source = "Adapter Manager Window" };
            Context = new ZvsContext(App.EntityContextConnection);
            InitializeComponent();

            NotifyEntityChangeContext.ChangeNotifications<Adapter>.OnEntityAdded += AdapterManagerWindow_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Adapter>.OnEntityDeleted += AdapterManagerWindow_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<Adapter>.OnEntityUpdated += AdapterManagerWindow_onEntityUpdated;
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
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            //Load your data here and assign the result to the CollectionViewSource.
            var zvsEntities2ViewSource = ((CollectionViewSource)(FindResource("ZvsEntities2AdapterViewSource")));

            //Get a list of loaded plug-ins
            UpdateAdapterList();

            //Only load the plug-in options for the plug-ins that are currently loaded.
            zvsEntities2ViewSource.Source = Context.Adapters.Local;
        }

        private void UpdateAdapterList()
        {
            if (Context == null)
                return;

            Dispatcher.Invoke(async () =>
            {
                var loadedAdapterGuids = App.ZvsEngine.AdapterManager.GetZvsAdapters().Select(o => o.AdapterGuid).ToList();
                await Context.Adapters.Where(o => loadedAdapterGuids.Contains(o.AdapterGuid)).ToListAsync();
            });
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            NotifyEntityChangeContext.ChangeNotifications<Adapter>.OnEntityAdded -= AdapterManagerWindow_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Adapter>.OnEntityDeleted -= AdapterManagerWindow_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<Adapter>.OnEntityUpdated -= AdapterManagerWindow_onEntityUpdated;
            Context.Dispose();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AdapterListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ControlsStkPnl.Children.Clear();

            var adapter = (Adapter)AdapterListView.SelectedItem;
            if (adapter == null) return;
            //ADD THE ENABLED BUTTON
            var c = new CheckboxControl(
                async isChecked =>
                {
                    //Save to the database
                    adapter.IsEnabled = isChecked;

                    var result = await Context.TrySaveChangesAsync(App.Cts.Token);
                    if (result.HasError)
                        await Log.ReportErrorFormatAsync(App.Cts.Token, "Error enabling adapter. {0}", result.Message);

                    //STOP OR START
                    if (isChecked)
                        await App.ZvsEngine.AdapterManager.EnableAdapterAsync(adapter.AdapterGuid, App.Cts.Token);
                    else
                        await App.ZvsEngine.AdapterManager.DisableAdapterAsync(adapter.AdapterGuid, App.Cts.Token);
                },
                                        _icon)
            {
                Header = string.Format("{0} is enabled", adapter.Name),
                Description = "Starts and stops the selected adapter",
                Value = adapter.IsEnabled
            };
            ControlsStkPnl.Children.Add(c);


            //Add all the settings
            foreach (var a in adapter.Settings)
            {
                var adapterSetting = a;

                switch (adapterSetting.ValueType)
                {
                    case DataType.BOOL:
                        {
                            bool defaultValue;
                            bool.TryParse(adapterSetting.Value, out defaultValue);

                            var control = new CheckboxControl(async isChecked =>
                                {
                                    adapterSetting.Value = isChecked.ToString();
                                    var result = await Context.TrySaveChangesAsync(App.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(App.Cts.Token, "Error saving adapter setting. {0}", result.Message);
                                },
                                        _icon)
                            {
                                Header = adapterSetting.Name,
                                Description = adapterSetting.Description,
                                Value = defaultValue
                            };
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.BYTE:
                    case DataType.DECIMAL:
                    case DataType.SHORT:
                    case DataType.INTEGER:
                    case DataType.COMPORT:
                        {
                            var control = new NumericControl(async value =>
                                {
                                    adapterSetting.Value = value;
                                    var result = await Context.TrySaveChangesAsync(App.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(App.Cts.Token, "Error saving adapter setting. {0}", result.Message);
                                },
                                        _icon, adapterSetting.ValueType)
                            {
                                Header = adapterSetting.Name,
                                Description = adapterSetting.Description,
                                Value = adapterSetting.Value
                            };
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.STRING:
                        {
                            var control = new StringControl(
                                async value =>
                                {
                                    adapterSetting.Value = value;
                                    var result = await Context.TrySaveChangesAsync(App.Cts.Token);
                                    if (result.HasError)
                                        await
                                            Log.ReportErrorFormatAsync(App.Cts.Token,
                                                "Error saving adapter setting. {0}", result.Message);
                                },
                                _icon)
                            {
                                Header = adapterSetting.Name,
                                Description = adapterSetting.Description,
                                Value = adapterSetting.Value
                            };


                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.LIST:
                        {
                            var control = new ComboboxControl(async value =>
                                {
                                    adapterSetting.Value = value.ToString();
                                    var result = await Context.TrySaveChangesAsync(App.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(App.Cts.Token, "Error saving adapter setting. {0}", result.Message);
                                },
                                 _icon,
                             adapterSetting.Options.Select(o => o.Name).ToList())
                            {
                                Header = adapterSetting.Name,
                                Description = adapterSetting.Description,
                                SelectedItem = adapterSetting.Value
                            };
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
