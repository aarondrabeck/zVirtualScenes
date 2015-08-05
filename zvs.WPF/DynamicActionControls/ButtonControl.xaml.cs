using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace zvs.WPF.DynamicActionControls
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class ButtonControl
    {
        #region Dependecy Properties
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ButtonControl), new PropertyMetadata(string.Empty));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ButtonControl), new PropertyMetadata(string.Empty));

        public string ButtonContent
        {
            get { return (string)GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonContentProperty =
            DependencyProperty.Register("ButtonContent", typeof(string), typeof(ButtonControl), new PropertyMetadata(string.Empty));

        #endregion

        private Func<Task> SendCommandAction { get; }
        public ButtonControl(Func<Task> sendCommandAction, ImageSource signalIcon)
        {
            SendCommandAction = sendCommandAction;
            InitializeComponent();
            SignalImg.Source = signalIcon;
        }

        private async Task SendCommandAsync()
        {
            if (SendCommandAction == null)
                return;

            SignalImg.Opacity = 1;

            await SendCommandAction();

            var da = new DoubleAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(.8)) };
            SignalImg.BeginAnimation(OpacityProperty, da);
        }

        private async void ToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
                await SendCommandAsync();
        }
    }
}
