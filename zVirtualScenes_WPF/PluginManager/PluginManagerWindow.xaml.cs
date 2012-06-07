using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections;
using zVirtualScenesModel;
using zVirtualScenes_WPF.DynamicActionControls;

namespace zVirtualScenes_WPF.PluginManager
{
    /// <summary>
    /// Interaction logic for PluginManager.xaml
    /// </summary>
    public partial class PluginManagerWindow : Window
    {
        private App application = (App)Application.Current;
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes_WPF;component/Images/save_check.png"));
        private zvsLocalDBEntities context;

        public PluginManagerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            context = new zvsLocalDBEntities();

            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource zvsEntities2ViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("zvsEntities2pluginsViewSource")));

                //Get plug-ins from database where currently loaded plug-ins match plug-ins from the db.  
                //This will prevent plug-ins that are in the DB but not loaded from being configurable.
                //zvsEntities2ViewSource.Source = context.plugins.Local.Where(p=> mainWindow.manager.pluginManager.GetPlugins().Any(o => o.Name == p.name));

                context.plugins.ToList();
                zvsEntities2ViewSource.Source = context.plugins.Local;
            }

            zvsLocalDBEntities.onPluginsChanged += zvsLocalDBEntities_onPluginsChanged;
         
        }

        void zvsLocalDBEntities_onPluginsChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (context != null)
                {
                    context.plugins.ToList();
                }
            }));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            zvsLocalDBEntities.onPluginsChanged -= zvsLocalDBEntities_onPluginsChanged;
            context.Dispose();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {            
            this.Close();
        }

        private void PluginLstVw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ControlsStkPnl.Children.Clear();
            
            plugin p = (plugin)PluginLstVw.SelectedItem;
            if (p != null)
            {
                //ADD THE ENABLED BUTTON
                CheckboxControl c = new CheckboxControl(string.Format("Enabled the '{0}'", p.friendly_name),
                    "Starts and stops the selected plugin.",
                    p.enabled,
                    (isChecked) =>
                    {
                        //Save to the database
                        p.enabled = isChecked;
                        context.SaveChanges();

                        //STOP OR START
                        var Plugin = application.zvsCore.pluginManager.GetPlugins().FirstOrDefault(o => o.Name == p.name);
                        if (Plugin != null)
                            if (isChecked) { Plugin.Start(); } else { Plugin.Stop(); }
                    },
                icon);
                ControlsStkPnl.Children.Add(c);


                //Add all the settings
                foreach (plugin_settings ps in p.plugin_settings)
                {
                    plugin_settings _ps = ps;

                    switch ((Data_Types)_ps.value_data_type)
                    {
                        case Data_Types.BOOL:
                            {
                                bool DefaultValue = false;
                                bool.TryParse(_ps.value, out DefaultValue);

                                CheckboxControl control = new CheckboxControl(_ps.friendly_name,
                                    _ps.description,
                                    DefaultValue,
                                    (isChecked) =>
                                    {
                                        _ps.value = isChecked.ToString();
                                        context.SaveChanges();
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case Data_Types.DECIMAL:
                            {
                                NumericControl control = new NumericControl(_ps.friendly_name,
                                    _ps.description,
                                    _ps.value,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        _ps.value = value;
                                        context.SaveChanges();
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case Data_Types.BYTE:
                            {
                                NumericControl control = new NumericControl(_ps.friendly_name,
                                    _ps.description,
                                    _ps.value,
                                    NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        _ps.value = value;
                                        context.SaveChanges();
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case Data_Types.INTEGER:
                            {
                                NumericControl control = new NumericControl(_ps.friendly_name,
                                    _ps.description,
                                    _ps.value,
                                    NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        _ps.value = value;
                                        context.SaveChanges();
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case Data_Types.SHORT:
                            {
                                NumericControl control = new NumericControl(_ps.friendly_name,
                                    _ps.description,
                                    _ps.value,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        _ps.value = value;
                                        context.SaveChanges();
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case Data_Types.COMPORT:
                            {
                                NumericControl control = new NumericControl(_ps.friendly_name,
                                    _ps.description,
                                    _ps.value,
                                    NumericControl.NumberType.ComPort,
                                    (value) =>
                                    {
                                        _ps.value = value;
                                        context.SaveChanges();
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case Data_Types.STRING:
                            {
                                StringControl control = new StringControl(_ps.friendly_name,
                                    _ps.description,
                                    _ps.value,
                                    (value) =>
                                    {
                                        _ps.value = value;
                                        context.SaveChanges();
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case Data_Types.LIST:
                            {
                                ComboboxControl control = new ComboboxControl(_ps.friendly_name,
                                    _ps.description,
                                    _ps.plugin_setting_options.Select(o => o.options).ToList(),
                                    _ps.value,
                                    (value) =>
                                    {
                                        _ps.value = value;
                                        context.SaveChanges();
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
