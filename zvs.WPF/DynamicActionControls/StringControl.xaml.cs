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
    public partial class StringControl
    {
        #region Dependecy Properties
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(StringControl), new PropertyMetadata(string.Empty));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(StringControl), new PropertyMetadata(string.Empty));

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(StringControl), new PropertyMetadata(string.Empty));


        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(StringControl), new PropertyMetadata(string.Empty));

        #endregion

        private Func<string, Task> SendCommandAction { get; set; }

        public StringControl(Func<string, Task> sendCommandAction, ImageSource signalIcon)
        {
            SendCommandAction = sendCommandAction;
            InitializeComponent();
            SignalImg.Source = signalIcon;
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            ValidateEntry();
        }

        private async void TextBox_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            if (IsEntryValid(TextBox.Text))
                await SendCommandAsync();

            e.Handled = true;
        }

        private async void TextBox_LostFocus_1(object sender, RoutedEventArgs e)
        {
            ValidateEntry();

            if (IsEntryValid(TextBox.Text))
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

        private static bool IsEntryValid(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        private void ValidateEntry()
        {
            ErrorMessage = IsEntryValid(TextBox.Text) ? string.Empty : "Invalid Entry";
        }
    }
}
