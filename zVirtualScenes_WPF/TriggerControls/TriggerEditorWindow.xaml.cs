using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.TriggerControls
{
    /// <summary>
    /// Interaction logic for TriggerEditorWindow.xaml
    /// </summary>
    public partial class TriggerEditorWindow : Window
    {
        private zvsLocalDBEntities context;
        private device_value_triggers trigger; 

        public TriggerEditorWindow(device_value_triggers trigger)
        {            
            this.trigger = trigger;
            InitializeComponent();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            context.Dispose();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            context = new zvsLocalDBEntities();
            context.devices.ToList();
            context.scenes.ToList();

            System.Windows.Data.CollectionViewSource deviceViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("deviceViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            deviceViewSource.Source = context.devices.Local;

            ConditionCmboBx.ItemsSource = Enum.GetValues(typeof(device_value_triggers.TRIGGER_OPERATORS));
            ConditionCmboBx.SelectedIndex = 0;

            System.Windows.Data.CollectionViewSource sceneViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("sceneViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            sceneViewSource.Source = context.scenes.Local;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            device d = (device)DeviceCmboBx.SelectedItem;
            if (d == null)
            {
                DeviceCmboBx.Focus();
                DeviceCmboBx.BorderBrush = new SolidColorBrush(Colors.Red);
                ColorAnimation anim = new ColorAnimation(Color.FromArgb(255, 112,112,112), new Duration(TimeSpan.FromSeconds(1.5)));
                DeviceCmboBx.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                e.Handled = false;

                return;
            }
            
            if (ValueCmboBx.SelectedItem == null)
            {
                ValueCmboBx.Focus();
                ValueCmboBx.BorderBrush = new SolidColorBrush(Colors.Red);
                ColorAnimation anim = new ColorAnimation(Color.FromArgb(255, 112, 112, 112), new Duration(TimeSpan.FromSeconds(1.5)));
                ValueCmboBx.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                e.Handled = false;

                return;
            }

            scene s = (scene)SceneCmbBx.SelectedItem;
            if (s == null)
            {
                SceneCmbBx.Focus();
                SceneCmbBx.BorderBrush = new SolidColorBrush(Colors.Red);
                ColorAnimation anim = new ColorAnimation(Color.FromArgb(255, 112, 112, 112), new Duration(TimeSpan.FromSeconds(1.5)));
                ValueCmboBx.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                e.Handled = false;

                return;
            }
                        
            if (string.IsNullOrEmpty(ValueTxtBx.Text))
            {
                ValueTxtBx.Focus();
                ValueTxtBx.Background = new SolidColorBrush(Colors.Pink);
                ColorAnimation anim = new ColorAnimation(Colors.White, new Duration(TimeSpan.FromSeconds(1.5)));
                ValueTxtBx.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                e.Handled = false;

                return;
            }

            device_values dv = (device_values)ValueCmboBx.SelectedItem;
            if (dv == null)
            {
                ValueCmboBx.Focus();
                ValueCmboBx.BorderBrush = new SolidColorBrush(Colors.Red);
                ColorAnimation anim = new ColorAnimation(Color.FromArgb(255, 112, 112, 112), new Duration(TimeSpan.FromSeconds(1.5)));
                ValueCmboBx.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                e.Handled = false;

                return;
            }

            DialogResult = true;
            this.Close();
        }
    }
}
