using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        private readonly App _app = (App)Application.Current;
        private readonly BitmapImage _icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));
        private readonly ZvsContext _context;

        public AdapterManagerWindow()
        {
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Adapter Manager Window" };
            _context = new ZvsContext(_app.EntityContextConnection);

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
            var zvsEntities2ViewSource = ((CollectionViewSource)(FindResource("zvsEntities2adapterViewSource")));

            //Get a list of loaded plug-ins
            UpdateAdapterList();

            //Only load the plug-in options for the plug-ins that are currently loaded.
            zvsEntities2ViewSource.Source = _context.Adapters.Local;
        }

        private void UpdateAdapterList()
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                var loadedAdapterGuids = _app.ZvsEngine.AdapterManager.GetZvsAdapters().Select(o => o.AdapterGuid).ToList();
                await _context.Adapters.Where(o => loadedAdapterGuids.Contains(o.AdapterGuid)).ToListAsync();
            }));
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            NotifyEntityChangeContext.ChangeNotifications<Adapter>.OnEntityAdded -= AdapterManagerWindow_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Adapter>.OnEntityDeleted -= AdapterManagerWindow_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<Adapter>.OnEntityUpdated -= AdapterManagerWindow_onEntityUpdated;
            _context.Dispose();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PluginLstVw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ControlsStkPnl.Children.Clear();

            var adapter = (Adapter)AdapterLstVw.SelectedItem;
            if (adapter == null) return;
            //ADD THE ENABLED BUTTON
            var c = new CheckboxControl(string.Format("{0} is enabled", adapter.Name),
                "Starts and stops the selected adapter",
                adapter.IsEnabled,
                async isChecked =>
                {
                    //Save to the database
                    adapter.IsEnabled = isChecked;

                    var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                    if (result.HasError)
                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error enabling adapter. {0}", result.Message);

                    //STOP OR START
                    if (isChecked)
                        await _app.ZvsEngine.AdapterManager.EnableAdapterAsync(adapter.AdapterGuid, _app.Cts.Token);
                    else
                        await _app.ZvsEngine.AdapterManager.DisableAdapterAsync(adapter.AdapterGuid, _app.Cts.Token);
                },
                _icon);
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

                            var control = new CheckboxControl(adapterSetting.Name,
                                adapterSetting.Description,
                                defaultValue,
                                async isChecked =>
                                {
                                    adapterSetting.Value = isChecked.ToString();
                                    var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving adapter setting. {0}", result.Message);
                                },
                                _icon);
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.DECIMAL:
                        {
                            var control = new NumericControl(adapterSetting.Name,
                                adapterSetting.Description,
                                adapterSetting.Value,
                                NumericControl.NumberType.Decimal,
                                async value =>
                                {
                                    adapterSetting.Value = value;
                                    var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving adapter setting. {0}", result.Message);
                                },
                                _icon);
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.BYTE:
                        {
                            var control = new NumericControl(adapterSetting.Name,
                                adapterSetting.Description,
                                adapterSetting.Value,
                                NumericControl.NumberType.Byte,
                                async value =>
                                {
                                    adapterSetting.Value = value;

                                    var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving adapter setting. {0}", result.Message);
                                },
                                _icon);
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.INTEGER:
                        {
                            var control = new NumericControl(adapterSetting.Name,
                                adapterSetting.Description,
                                adapterSetting.Value,
                                NumericControl.NumberType.Integer,
                                async value =>
                                {
                                    adapterSetting.Value = value;

                                    var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving adapter setting. {0}", result.Message);
                                },
                                _icon);
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.SHORT:
                        {
                            var control = new NumericControl(adapterSetting.Name,
                                adapterSetting.Description,
                                adapterSetting.Value,
                                NumericControl.NumberType.Short,
                                async value =>
                                {
                                    adapterSetting.Value = value;
                                    var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving adapter setting. {0}", result.Message);
                                },
                                _icon);
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.COMPORT:
                        {
                            var control = new NumericControl(adapterSetting.Name,
                                adapterSetting.Description,
                                adapterSetting.Value,
                                NumericControl.NumberType.ComPort,
                                async value =>
                                {
                                    adapterSetting.Value = value;
                                    var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving adapter setting. {0}", result.Message);
                                },
                                _icon);
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.STRING:
                        {
                            var control = new StringControl(adapterSetting.Name,
                                adapterSetting.Description,
                                adapterSetting.Value,
                                async value =>
                                {
                                    adapterSetting.Value = value;
                                    var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving adapter setting. {0}", result.Message);
                                },
                                _icon);
                            ControlsStkPnl.Children.Add(control);
                            break;
                        }
                    case DataType.LIST:
                        {
                            var control = new ComboboxControl(adapterSetting.Name,
                                adapterSetting.Description,
                                adapterSetting.Options.Select(o => o.Name).ToList(),
                                adapterSetting.Value,
                                async value =>
                                {
                                    adapterSetting.Value = value.ToString();
                                    var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving adapter setting. {0}", result.Message);
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
            if (_context == null)
            {
                return;
            }

            _context.Dispose();
        }
    }
}
