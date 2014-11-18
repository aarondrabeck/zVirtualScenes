using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace zvs.WPF.DynamicActionControls
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class ComboboxControl : UserControl
    {
        private string TitleName { get; set; }
        private string Description { get; set; }
        Func<object, Task> SelectionChangedAction { get; set; }
        private bool IsReady { get; set; }
        IEnumerable<object> ComboValues { get; set; }
        object SelectedValue { get; set; }

        public ComboboxControl(string titleName, string description, IEnumerable<object> comboValues, object selectedValue, Func<object, Task> selectionChangedAction, ImageSource icon)
        {
            TitleName = string.Empty;
            Description = string.Empty;
            ComboValues = new List<object>();
            SelectedValue = string.Empty;

            TitleName = titleName;
            Description = description;
            ComboValues = comboValues;
            SelectedValue = selectedValue;
            SelectionChangedAction = selectionChangedAction;
            InitializeComponent();

            SignalImg.Source = icon;
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            IsReady = false;

            foreach (var option in ComboValues)
                ComboBox.Items.Add(option);

            ComboBox.SelectedValue = SelectedValue;

            if (string.IsNullOrEmpty(Description))
                DescTxt.Visibility = Visibility.Collapsed;

            NameTxt.Text = TitleName;
            ComboBox.ToolTip = Description;
            DescTxt.Text = Description;

            IsReady = true;
        }

        private async void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (!IsReady) return;
            if (SelectionChangedAction == null || ComboBox.SelectedItem == null) return;
            await SelectionChangedAction(ComboBox.SelectedItem);

            SignalImg.Opacity = 1;
            var da = new DoubleAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(.8)) };
            SignalImg.BeginAnimation(OpacityProperty, da);
        }
    }
}
