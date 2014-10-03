using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using zvs.Entities;
using zvs.WPF.Commands;
using System.Data.Entity;

namespace zvs.WPF.TriggerControls
{
    /// <summary>
    /// Interaction logic for TriggerEditorWindow.xaml
    /// </summary>
    public partial class TriggerEditorWindow : Window
    {
        private zvsContext context;
        private Int64 DeviceValueTriggerId;

        public DeviceValueTrigger Trigger
        {
            get { return (DeviceValueTrigger)GetValue(TriggerProperty); }
            set { SetValue(TriggerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Trigger.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TriggerProperty =
            DependencyProperty.Register("Trigger", typeof(DeviceValueTrigger), typeof(TriggerEditorWindow), new PropertyMetadata(null));

        public bool Canceled = true;

        public TriggerEditorWindow(Int64 deviceValueTriggerId, zvsContext context)
        {
            this.context = context;
            this.DeviceValueTriggerId = deviceValueTriggerId;
            InitializeComponent();
        }

#if DEBUG
        ~TriggerEditorWindow()
        {
            Debug.WriteLine("TriggerEditorWindow Deconstructed.");
        }
#endif

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            Trigger = await context.DeviceValueTriggers
                .Include(o => o.DeviceValue)
                .Include(o => o.StoredCommand)
                .Include(o => o.DeviceValue.Device)
                .FirstOrDefaultAsync(o => o.Id == DeviceValueTriggerId);

            if (Trigger == null)
            {
                Trigger = new DeviceValueTrigger();
                Trigger.Name = "New Trigger";
            }

            var eagarLoad2 = await context.Devices
                .Include(o => o.Values)
                .ToListAsync();

            var scene = await context.Scenes.ToListAsync();

            System.Windows.Data.CollectionViewSource deviceViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("deviceViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            deviceViewSource.Source = context.Devices.Local;

            OperatorCmboBx.ItemsSource = Enum.GetValues(typeof(TriggerOperator));
            OperatorCmboBx.SelectedIndex = 0;

            //Set presets
            if (Trigger != null && Trigger.DeviceValue != null)
            {
                if (Trigger.DeviceValue.Device != null)
                    DeviceCmboBx.SelectedItem = Trigger.DeviceValue.Device;

                if (Trigger.DeviceValue != null)
                    ValueCmboBx.SelectedItem = Trigger.DeviceValue;
            }

            try
            {
                OperatorCmboBx.Text = Enum.GetName(typeof(TriggerOperator), Trigger.Operator);
            }
            catch { }

            if (Trigger.Value != null)
                ValueTxtBx.Text = Trigger.Value;
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
                Trigger.DeviceValue = dv;
            }

            if (Trigger.StoredCommand == null)
            {
                AddUpdateCommand.Focus();
                AddUpdateCommand.BorderBrush = new SolidColorBrush(Colors.Red);
                ColorAnimation anim = new ColorAnimation(Color.FromArgb(255, 112, 112, 112), new Duration(TimeSpan.FromSeconds(1.5)));
                AddUpdateCommand.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
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
            else
            {
                Trigger.Value = ValueTxtBx.Text;
            }

            Trigger.Operator = (TriggerOperator)OperatorCmboBx.SelectedItem;

            //Update the description
            Trigger.SetDescription(context);

            Canceled = false;
            this.Close();
        }

        private async void AddUpdateCommand_Click(object sender, RoutedEventArgs e)
        {
            //Create a Stored Command if there is not one...
            StoredCommand newSC = new StoredCommand();

            //Send it to the command builder to get filled with a command
            CommandBuilder cbWindow;
            if (Trigger.StoredCommand == null)
                cbWindow = new CommandBuilder(context, newSC);
            else
                cbWindow = new CommandBuilder(context, Trigger.StoredCommand);

            cbWindow.Owner = this;

            if (cbWindow.ShowDialog() ?? false)
            {
                if (Trigger.StoredCommand == null) //if this was a new command, assign it.
                    Trigger.StoredCommand = newSC;
                else
                    Trigger.StoredCommand = Trigger.StoredCommand;

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).zvsCore.log.Error(result.Message);
            }

        }
    }
}
