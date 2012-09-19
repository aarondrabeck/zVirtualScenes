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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace zvs.WPF.DynamicActionControls
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class CheckboxControl : UserControl
    {
        private string name = string.Empty;
        private string Description = string.Empty;
        private Action<bool> CheckedChangedAction = null;
        private bool isLoaded = false;
        private bool defaultIsChecked = false;

        public CheckboxControl(string Name, string Description, bool defaultIsChecked, Action<bool> CheckedChangedAction, BitmapImage icon)
        {
            this.name = Name;
            this.Description = Description;
            this.CheckedChangedAction = CheckedChangedAction;
            this.defaultIsChecked = defaultIsChecked;
            InitializeComponent();

            this.SignalImg.Source = icon;
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            isLoaded = false;

            CheckBx.Content = name;
            CheckBx.ToolTip = Description;
            CheckBx.IsChecked = defaultIsChecked;
            

            if (string.IsNullOrEmpty(Description))
                DescTxt.Visibility = System.Windows.Visibility.Collapsed;

            DescTxt.Text = Description;

            isLoaded = true;
        }

        private void CheckBx_Checked(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
                SendCommand(CheckBx.IsChecked ?? false);
        }

        private void CheckBx_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
                SendCommand(CheckBx.IsChecked ?? false);
        }

        private void SendCommand(bool isChecked)
        {
            if (CheckedChangedAction != null)
            {
                CheckedChangedAction.DynamicInvoke(isChecked);

                SignalImg.Opacity = 1;
                DoubleAnimation da = new DoubleAnimation();
                da.From = 1;
                da.To = 0;
                da.Duration = new Duration(TimeSpan.FromSeconds(.8));
                SignalImg.BeginAnimation(OpacityProperty, da);
            }
        }
    }
}
