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
using zVirtualScenes_WPF.DynamicSettingsControls;
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.PluginManager
{
    /// <summary>
    /// Interaction logic for PluginManager.xaml
    /// </summary>
    public partial class PluginManagerWindow : Window
    {

        private App application = (App)Application.Current;
        private zvsLocalDBEntities context;
        private bool isLoaded = false; 

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

            zvsLocalDBEntities.onPluginsChanged+=zvsLocalDBEntities_onPluginsChanged; 
            isLoaded = true;
        }

        void zvsLocalDBEntities_onPluginsChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                context.plugins.ToList();
            })); 
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            zvsLocalDBEntities.onPluginsChanged -= zvsLocalDBEntities_onPluginsChanged; 
            context.Dispose();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            PromptSaveNowIfChanged();
            this.Close();
        }

        private void PromptSaveNowIfChanged()
        {
            foreach (DynamicSettingsInterface setting in this.ControlsStkPnl.Children.OfType<DynamicSettingsInterface>())
            {
                if (setting.hasChanged)
                {
                    if (MessageBox.Show("Would you like to save the changes to this plug-in?", "Save Changes?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        //Save the changes made to the context
                        foreach (DynamicSettingsInterface s in this.ControlsStkPnl.Children.OfType<DynamicSettingsInterface>())
                            s.SaveToContext();

                        context.SaveChanges();

                        //TODO: RESTART PLUGIN?
                    }
                    return;
                }
            }
        }

        private void PluginLstVw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PromptSaveNowIfChanged();

            this.ControlsStkPnl.Children.Clear();

            plugin p = (plugin)PluginLstVw.SelectedItem;
            if (p != null)
            {
                foreach (plugin_settings ps in p.plugin_settings)
                {
                    switch ((Data_Types)ps.value_data_type)
                    {
                        case Data_Types.BOOL:
                            {
                                this.ControlsStkPnl.Children.Add(new BoolSettingsControl(context, ps));

                                break;
                            }
                        case Data_Types.DECIMAL:
                            {
                                NumericSettingsControl control = new NumericSettingsControl(context, ps);
                                control.MaxValue = Decimal.MaxValue;
                                control.MinValue = Decimal.MinValue;
                                this.ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case Data_Types.BYTE:
                            {
                                NumericSettingsControl control = new NumericSettingsControl(context, ps);
                                control.MaxValue = Byte.MaxValue;
                                control.MinValue = Byte.MinValue;
                                control.ForceWholeNumber = true;
                                this.ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case Data_Types.INTEGER:
                            {
                                NumericSettingsControl control = new NumericSettingsControl(context, ps);
                                control.MaxValue = Int64.MaxValue;
                                control.MinValue = Int64.MinValue;
                                control.ForceWholeNumber = true;
                                this.ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case Data_Types.SHORT:
                            {
                                NumericSettingsControl control = new NumericSettingsControl(context, ps);
                                control.MaxValue = short.MaxValue;
                                control.MinValue = short.MinValue;
                                control.ForceWholeNumber = true;
                                this.ControlsStkPnl.Children.Add(control);
                                break;
                            }
                        case Data_Types.STRING:
                            {
                                this.ControlsStkPnl.Children.Add(new StringSettingsControl(context, ps));
                                break;
                            }
                        case Data_Types.COMPORT:
                            {
                                NumericSettingsControl control = new NumericSettingsControl(context, ps);
                                control.MaxValue = 0;
                                control.MinValue = 99;
                                control.ForceWholeNumber = true;
                                this.ControlsStkPnl.Children.Add(control);
                                break;
                            }
                    }
                }
            }
        }

        private void EnabledChkBx_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent this from triggering as the UI loads and sets the checkbox
            if (isLoaded)
            {
                context.SaveChanges();

                plugin p = (plugin)PluginLstVw.SelectedItem;
                if (p != null)
                {
                    var Plugin = application.zvsCore.pluginManager.GetPlugins().FirstOrDefault(o => o.Name == p.name);
                    if (Plugin != null)
                        Plugin.Start();
                }
            }
        }

        private void EnabledChkBx_Unchecked(object sender, RoutedEventArgs e)
        {
            //Prevent this from triggering as the UI loads and sets the checkbox
            if (isLoaded)
            {
                context.SaveChanges();

                plugin p = (plugin)PluginLstVw.SelectedItem;
                if (p != null)
                {
                    var Plugin = application.zvsCore.pluginManager.GetPlugins().FirstOrDefault(o => o.Name == p.name);
                    if (Plugin != null)
                        Plugin.Stop();
                }
            }
        }
    }
}
