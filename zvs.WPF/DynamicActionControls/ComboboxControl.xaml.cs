using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace zvs.WPF.DynamicActionControls
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class ComboboxControl
    {
        #region Dependecy Properties
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ComboboxControl), new PropertyMetadata(string.Empty));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ComboboxControl), new PropertyMetadata(string.Empty));

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ComboboxControl), new PropertyMetadata(0));


        #endregion

        private Func<object, Task> SendCommandAction { get; }

        public ComboboxControl(Func<object, Task> sendCommandAction, ImageSource signalIcon, IEnumerable<object> comboValues)
        {
            if (comboValues == null)
                throw new ArgumentNullException(nameof(comboValues));

            SendCommandAction = sendCommandAction;
            InitializeComponent();
            comboValues.ToList().ForEach(i => ComboBox.Items.Add(i));
            SignalImg.Source = signalIcon;
        }

        private async Task SendCommandAsync(object item)
        {
            if (SendCommandAction == null)
                return;

            SignalImg.Opacity = 1;

            await SendCommandAction(item);

            var da = new DoubleAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(.8)) };
            SignalImg.BeginAnimation(OpacityProperty, da);
        }

        private async void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            await SendCommandAsync(ComboBox.SelectedItem);
        }
    }
}
