﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace zvs.WPF.DynamicActionControls
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class CheckboxControl
    {
        #region Dependecy Properties
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(CheckboxControl), new PropertyMetadata(string.Empty));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(CheckboxControl), new PropertyMetadata(string.Empty));

        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(CheckboxControl), new PropertyMetadata(false));

        #endregion

        private Func<bool, Task> SendCommandAction { get; }

        public CheckboxControl(Func<bool, Task> sendCommandAction, ImageSource signalIcon)
        {
            SendCommandAction = sendCommandAction;
            InitializeComponent();
            SignalImg.Source = signalIcon;
        }

        private async Task SendCommandAsync(bool state)
        {
            if (SendCommandAction == null)
                return;

            SignalImg.Opacity = 1;

            await SendCommandAction(state);

            var da = new DoubleAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(.8)) };
            SignalImg.BeginAnimation(OpacityProperty, da);
        }

        private async void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
                await SendCommandAsync(ToggleButton.IsChecked ?? false);
        }

        private async void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
                await SendCommandAsync(ToggleButton.IsChecked ?? false);
        }
    }
}
