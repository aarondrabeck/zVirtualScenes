using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using zvs.DataModel;
using zvs.WPF.Commands;
using System.Data.Entity;

namespace zvs.WPF.TriggerControls
{
    /// <summary>
    /// Interaction logic for TriggerEditorWindow.xaml
    /// </summary>
    public partial class TriggerEditorWindow : Window
    {
        private readonly ZvsContext _context;
        private readonly Int64 _deviceValueTriggerId;

        public DeviceValueTrigger Trigger
        {
            get { return (DeviceValueTrigger)GetValue(TriggerProperty); }
            set { SetValue(TriggerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScheduledTask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TriggerProperty =
            DependencyProperty.Register("ScheduledTask", typeof(DeviceValueTrigger), typeof(TriggerEditorWindow), new PropertyMetadata(null));

        public bool Canceled = true;

        public TriggerEditorWindow(Int64 deviceValueTriggerId, ZvsContext context)
        {
            _context = context;
            _deviceValueTriggerId = deviceValueTriggerId;
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
            Trigger = await _context.DeviceValueTriggers
                .Include(o => o.DeviceValue)
                .Include(o => o.StoredCommand)
                .Include(o => o.DeviceValue.Device)
                .FirstOrDefaultAsync(o => o.Id == _deviceValueTriggerId);

            if (Trigger == null)
            {
                Trigger = new DeviceValueTrigger {Name = "New ScheduledTask"};
            }

            var eagarLoad2 = await _context.Devices
                .Include(o => o.Values)
                .ToListAsync();

            var scene = await _context.Scenes.ToListAsync();

            var deviceViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("deviceViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            deviceViewSource.Source = _context.Devices.Local;

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
            catch (Exception)
            { }

            if (Trigger.Value != null)
                ValueTxtBx.Text = Trigger.Value;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            var d = (Device)DeviceCmboBx.SelectedItem;
            if (d == null)
            {
                DeviceCmboBx.Focus();
                DeviceCmboBx.BorderBrush = new SolidColorBrush(Colors.Red);
                var anim = new ColorAnimation(Color.FromArgb(255, 112, 112, 112), new Duration(TimeSpan.FromSeconds(1.5)));
                DeviceCmboBx.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                e.Handled = false;

                return;
            }

            var dv = (DeviceValue)ValueCmboBx.SelectedItem;
            if (dv == null)
            {
                ValueCmboBx.Focus();
                ValueCmboBx.BorderBrush = new SolidColorBrush(Colors.Red);
                var anim = new ColorAnimation(Color.FromArgb(255, 112, 112, 112), new Duration(TimeSpan.FromSeconds(1.5)));
                ValueCmboBx.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                e.Handled = false;

                return;
            }
            Trigger.DeviceValue = dv;

            if (Trigger.StoredCommand == null)
            {
                AddUpdateCommand.Focus();
                AddUpdateCommand.BorderBrush = new SolidColorBrush(Colors.Red);
                var anim = new ColorAnimation(Color.FromArgb(255, 112, 112, 112), new Duration(TimeSpan.FromSeconds(1.5)));
                AddUpdateCommand.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                e.Handled = false;
                return;
            }

            if (string.IsNullOrEmpty(ValueTxtBx.Text))
            {
                ValueTxtBx.Focus();
                ValueTxtBx.Background = new SolidColorBrush(Colors.Pink);
                var anim = new ColorAnimation(Colors.White, new Duration(TimeSpan.FromSeconds(1.5)));
                ValueTxtBx.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                e.Handled = false;
                return;
            }
            Trigger.Value = ValueTxtBx.Text;

            Trigger.Operator = (TriggerOperator)OperatorCmboBx.SelectedItem;

            //Update the description
            Trigger.SetDescription();

            Canceled = false;
            Close();
        }

        private async void AddUpdateCommand_Click(object sender, RoutedEventArgs e)
        {
            //Create a Stored Command if there is not one...
            var newSc = new StoredCommand();

            //Send it to the command builder to get filled with a command
            var cbWindow = Trigger.StoredCommand == null ? new CommandBuilder(_context, newSc) : new CommandBuilder(_context, Trigger.StoredCommand);

            cbWindow.Owner = this;

            if (!(cbWindow.ShowDialog() ?? false)) return;
            Trigger.StoredCommand = Trigger.StoredCommand ?? newSc;

            var result = await _context.TrySaveChangesAsync();
            if (result.HasError)
                ((App)Application.Current).ZvsEngine.log.Error(result.Message);
        }
    }
}
