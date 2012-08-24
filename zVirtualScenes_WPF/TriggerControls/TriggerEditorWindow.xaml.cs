using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using zvs.Entities;


namespace zVirtualScenesGUI.TriggerControls
{
    /// <summary>
    /// Interaction logic for TriggerEditorWindow.xaml
    /// </summary>
    public partial class TriggerEditorWindow : Window
    {
        private zvsContext context;
        private DeviceValueTrigger trigger;
        public bool Canceled = true;

        public TriggerEditorWindow(DeviceValueTrigger trigger, zvsContext context)
        {
            this.context = context;
            this.trigger = trigger;
            InitializeComponent();
        }

        ~TriggerEditorWindow()
        {
            Debug.WriteLine("TriggerEditorWindow Deconstructed.");
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            context.Devices.ToList();
            context.Scenes.ToList();

            System.Windows.Data.CollectionViewSource deviceViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("deviceViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            deviceViewSource.Source = context.Devices.Local;

            OperatorCmboBx.ItemsSource = Enum.GetValues(typeof(TriggerOperator));
            OperatorCmboBx.SelectedIndex = 0;

            System.Windows.Data.CollectionViewSource sceneViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("sceneViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            sceneViewSource.Source = context.Scenes.Local;

            //Set presets
            if (trigger.DeviceValue != null)
            {
                if (trigger.DeviceValue.Device != null)
                    DeviceCmboBx.SelectedItem = trigger.DeviceValue.Device;

                if (trigger.DeviceValue != null)
                    ValueCmboBx.SelectedItem = trigger.DeviceValue;
            }
            
            try
            {
                OperatorCmboBx.Text = Enum.GetName(typeof(TriggerOperator), trigger.Operator);
            }
            catch { }
            
            if (trigger.Value != null)
                ValueTxtBx.Text = trigger.Value;

            if (trigger.Scene != null)
            {                
                SceneCmbBx.SelectedItem = trigger.Scene;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            Device d = (Device)DeviceCmboBx.SelectedItem;
            if (d == null)
            {
                DeviceCmboBx.Focus();
                DeviceCmboBx.BorderBrush = new SolidColorBrush(Colors.Red);
                ColorAnimation anim = new ColorAnimation(Color.FromArgb(255, 112, 112, 112), new Duration(TimeSpan.FromSeconds(1.5)));
                DeviceCmboBx.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                e.Handled = false;

                return;
            }

            DeviceValue dv = (DeviceValue)ValueCmboBx.SelectedItem;
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
                trigger.DeviceValue = dv;
            }

            Scene s = (Scene)SceneCmbBx.SelectedItem;
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
                trigger.Scene = s;
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
                trigger.Value = ValueTxtBx.Text;
            }

            trigger.Operator = (TriggerOperator)OperatorCmboBx.SelectedItem;

            Canceled = false;
            this.Close();
        }
    }
}
