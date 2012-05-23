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
using System.Windows.Navigation;
using System.Windows.Shapes;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenes_WPF.DynamicSettingsControls
{
    /// <summary>
    /// Interaction logic for BoolSettingsControl.xaml
    /// </summary>
    public partial class BoolSettingsControl : UserControl, DynamicSettingsInterface
    {
        zvsEntities2 context;
        plugin_settings plugin_setting;

        public BoolSettingsControl()
        {
            InitializeComponent();
        }

        bool hasLoaded = false;
        public BoolSettingsControl(zvsEntities2 context, plugin_settings plugin_setting)
        {
            InitializeComponent();

            this.context = context;
            this.plugin_setting = plugin_setting;

            if (plugin_setting != null)
            {
                
                this.Label.Text = plugin_setting.friendly_name;
                this.Details.Text = plugin_setting.description;

                if (!String.IsNullOrEmpty(plugin_setting.value))
                {
                    bool bval = true;
                    bool.TryParse(plugin_setting.value, out bval);
                    checkBox.IsChecked = bval;
                }
            }
            hasLoaded = true;
        }

        public void SaveToContext()
        {
            if (plugin_setting != null)
            {
                plugin_setting.value = checkBox.IsChecked.ToString();
            }
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (hasLoaded)
                _haschanged = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (hasLoaded)
                _haschanged = true;
        }

        private bool _haschanged = false;
        public bool hasChanged
        {
            get
            {
                return _haschanged;
            }
        }
    }
}
