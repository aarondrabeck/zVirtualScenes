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

using zvs.WPF.DynamicActionControls;
using System.Diagnostics;
using zvs.Entities;

namespace zvs.WPF.PluginManager
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
            InitializeComponent();
        }

        ~PluginManagerWindow()
        {
            Debug.WriteLine("PluginManagerWindow Deconstructed.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            context = new zvsContext();

            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource zvsEntities2ViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("zvsEntities2pluginsViewSource")));

                //Get plug-ins from database where currently loaded plug-ins match plug-ins from the db.  
                //This will prevent plug-ins that are in the DB but not loaded from being configurable.
                //zvsEntities2ViewSource.Source = context.plugins.Local.Where(p=> mainWindow.manager.pluginManager.GetPlugins().Any(o => o.Name == p.name));

                //Get a list of loaded plug-ins
                 var loadedPlugins = application.zvsCore.pluginManager.GetPlugins().ToList();
                
                context.Plugins.ToList();

                //Only load the plug-in options for the plug-ins that are currently loaded.
                zvsEntities2ViewSource.Source = context.Plugins.Local.Where(o => loadedPlugins.Any(x => x.UniqueIdentifier == o.UniqueIdentifier));
            }

            zvsContext.onPluginsChanged += zvsContext_onPluginsChanged;
         
        }

        void zvsContext_onPluginsChanged(object sender, zvsContext.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (context != null)
                {
                    context.Plugins.ToList();
                }
            }));
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            zvsContext.onPluginsChanged -= zvsContext_onPluginsChanged;            
            context.Dispose();
        }  

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {            
            this.Close();
        }

        private void PluginLstVw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ControlsStkPnl.Children.Clear();

            Plugin p = (Plugin)PluginLstVw.SelectedItem;
            if (p != null)
            {
                //ADD THE ENABLED BUTTON
                CheckboxControl c = new CheckboxControl(string.Format("Enabled the '{0}'", p.Name),
                    "Starts and stops the selected plug-in.",
                    p.isEnabled,
                    (isChecked) =>
                    {
                        //Save to the database
                        p.isEnabled = isChecked;                        
                        context.SaveChanges();

                        //STOP OR START
                        var Plugin = application.zvsCore.pluginManager.GetPlugins().FirstOrDefault(o => o.Name == p.Name);
                        if (Plugin != null)
                            if (isChecked) { Plugin.Start(); } else { Plugin.Stop(); }
                    },
                icon);
                ControlsStkPnl.Children.Add(c);


                //Add all the settings
                foreach (PluginSetting ps in p.Settings)
                {
                    PluginSetting _ps = ps;

                    switch (_ps.ValueType)
                    {
                        case DataType.BOOL:
                            {
                                bool DefaultValue = false;
                                bool.TryParse(_ps.Value, out DefaultValue);

                                CheckboxControl control = new CheckboxControl(_ps.Name,
                                    _ps.Description,
                                    DefaultValue,
                                    (isChecked) =>
                                    {
                                        _ps.Value = isChecked.ToString();                                        
                                        context.SaveChanges();
                                        application.zvsCore.pluginManager.NotifyPluginSettingsChanged(_ps);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.DECIMAL:
                            {
                                NumericControl control = new NumericControl(_ps.Name,
                                    _ps.Description,
                                    _ps.Value,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        _ps.Value = value;                                        
                                        context.SaveChanges();
                                        application.zvsCore.pluginManager.NotifyPluginSettingsChanged(_ps);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.BYTE:
                            {
                                NumericControl control = new NumericControl(_ps.Name,
                                    _ps.Description,
                                    _ps.Value,
                                    NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        _ps.Value = value;                                        
                                        context.SaveChanges();
                                        application.zvsCore.pluginManager.NotifyPluginSettingsChanged(_ps);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.INTEGER:
                            {
                                NumericControl control = new NumericControl(_ps.Name,
                                    _ps.Description,
                                    _ps.Value,
                                    NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        _ps.Value = value;                                       
                                        context.SaveChanges();
                                        application.zvsCore.pluginManager.NotifyPluginSettingsChanged(_ps);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.SHORT:
                            {
                                NumericControl control = new NumericControl(_ps.Name,
                                    _ps.Description,
                                    _ps.Value,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        _ps.Value = value;                                        
                                        context.SaveChanges();
                                        application.zvsCore.pluginManager.NotifyPluginSettingsChanged(_ps);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.COMPORT:
                            {
                                NumericControl control = new NumericControl(_ps.Name,
                                    _ps.Description,
                                    _ps.Value,
                                    NumericControl.NumberType.ComPort,
                                    (value) =>
                                    {
                                        _ps.Value = value;
                                        context.SaveChanges();
                                        application.zvsCore.pluginManager.NotifyPluginSettingsChanged(_ps);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.STRING:
                            {
                                StringControl control = new StringControl(_ps.Name,
                                    _ps.Description,
                                    _ps.Value,
                                    (value) =>
                                    {
                                        _ps.Value = value;
                                        context.SaveChanges();
                                        application.zvsCore.pluginManager.NotifyPluginSettingsChanged(_ps);
                                    },
                                icon);
                                ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case DataType.LIST:
                            {
                                ComboboxControl control = new ComboboxControl(_ps.Name,
                                    _ps.Description,
                                    _ps.Options.Select(o => o.Name).ToList(),
                                    _ps.Value,
                                    (value) =>
                                    {
                                        _ps.Value = value;
                                        context.SaveChanges();
                                        application.zvsCore.pluginManager.NotifyPluginSettingsChanged(_ps);
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
