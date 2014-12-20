using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace zvs.WPF.DynamicActionControls
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class NumericControl
    {
        #region Dependecy Properties
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(NumericControl), new PropertyMetadata(string.Empty));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(NumericControl), new PropertyMetadata(string.Empty));

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(NumericControl), new PropertyMetadata(string.Empty));


        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(NumericControl), new PropertyMetadata(string.Empty));

        #endregion

        private DataType Type { get; set; }
        private Func<string, Task> SendCommandAction { get; set; }

        public NumericControl(Func<string, Task> sendCommandAction, ImageSource signalIcon, DataType type)
        {
            SendCommandAction = sendCommandAction;
            Type = type;
            InitializeComponent();
            SignalImg.Source = signalIcon;
        }
        private void ValidateEntry()
        {
            ErrorMessage = IsEntryValid(TextBox.Text, Type) ? string.Empty : "Invalid Entry";
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            ValidateEntry();
        }

        private async void TextBox_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            //Allow arrown and delete keys
            switch (e.Key)
            {
                case Key.Back:
                case Key.Delete:
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    return;
                case Key.Enter:
                    if (IsEntryValid(TextBox.Text, Type))
                        await SendCommandAsync();

                    e.Handled = true;
                    break;
            }
        }

        private async void TextBox_LostFocus_1(object sender, RoutedEventArgs e)
        {
            ValidateEntry();

            if (IsEntryValid(TextBox.Text, Type))
                await SendCommandAsync();
        }

        private async Task SendCommandAsync()
        {
            if (SendCommandAction == null)
                return;

            SignalImg.Opacity = 1;

            await SendCommandAction(TextBox.Text);

            var da = new DoubleAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(.8)) };
            SignalImg.BeginAnimation(OpacityProperty, da);
        }

        private bool IsEntryValid(string value, DataType type)
        {
            if (string.IsNullOrEmpty(TextBox.Text))
                return false;

            switch (type)
            {
                case DataType.INTEGER:
                    {
                        int num;
                        return int.TryParse(value, out num);
                    }
                case DataType.DECIMAL:
                    {
                        decimal num;
                        return decimal.TryParse(value, out num);
                    }
                case DataType.BYTE:
                    {
                        byte num;
                        return byte.TryParse(value, out num);
                    }
                case DataType.SHORT:
                    {
                        short num;
                        return short.TryParse(value, out num);
                    }
                case DataType.COMPORT:
                    {
                        int num;
                        if (int.TryParse(value, out num))
                        {
                            if (num >= 0 && num <= 99)
                                return true;
                        }
                        break;
                    }
            }
            return false;
        }
    }
}
