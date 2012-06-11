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
        public bool Canceled = true;

        public TriggerEditorWindow(device_value_triggers trigger, zvsLocalDBEntities context)
        {
            this.context = context;
            this.trigger = trigger;
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            context.devices.ToList();
            context.scenes.ToList();

            System.Windows.Data.CollectionViewSource deviceViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("deviceViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            deviceViewSource.Source = context.devices.Local;

            OperatorCmboBx.ItemsSource = Enum.GetValues(typeof(device_value_triggers.TRIGGER_OPERATORS));
            OperatorCmboBx.SelectedIndex = 0;

            System.Windows.Data.CollectionViewSource sceneViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("sceneViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            sceneViewSource.Source = context.scenes.Local;

            //Set presets
            if (trigger.device_values != null)
            {
                device d = context.devices.FirstOrDefault(o => o.id == trigger.device_values.device_id);
                if (d != null)
                    DeviceCmboBx.SelectedItem = d;

                device_values dv = context.device_values.FirstOrDefault(o => o.id == trigger.device_value_id);
                if (dv != null)
                    ValueCmboBx.SelectedItem = dv;
            }

            if (trigger.trigger_operator.HasValue)
            {
                try
                {
                    OperatorCmboBx.Text = Enum.GetName(typeof(device_value_triggers.TRIGGER_OPERATORS), trigger.trigger_operator.Value);
                }
                catch { }
            }

            if (trigger.trigger_value != null)
                ValueTxtBx.Text = trigger.trigger_value;            

            if (trigger.scene_id.HasValue)
            {
                scene s = context.scenes.FirstOrDefault(o => o.id == trigger.scene_id.Value);
                SceneCmbBx.SelectedItem = s;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            device d = (device)DeviceCmboBx.SelectedItem;
            if (d == null)
            {
                DeviceCmboBx.Focus();
                DeviceCmboBx.BorderBrush = new SolidColorBrush(Colors.Red);
                ColorAnimation anim = new ColorAnimation(Color.FromArgb(255, 112, 112, 112), new Duration(TimeSpan.FromSeconds(1.5)));
                DeviceCmboBx.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
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
            else
            {
                trigger.device_values = dv;
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
            else
            {
                trigger.scene = s;
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
            else
            {
                trigger.trigger_value = ValueTxtBx.Text;
            }

            trigger.trigger_operator = (int)OperatorCmboBx.SelectedItem;

            Canceled = false;
            this.Close();
        }
    }
}
