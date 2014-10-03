using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace zvs.WPF.DynamicActionControls
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class ButtonControl : UserControl
    {
        private string BtnName = string.Empty;
        private string Description = string.Empty;
        private Action ButtonClickAction = null;

        public ButtonControl(string BtnName, string Description, Action ButtonClickAction, BitmapImage icon)
        {
            this.BtnName = BtnName;
            this.Description = Description;
            this.ButtonClickAction = ButtonClickAction;

            InitializeComponent();

            this.SignalImg.Source = icon;
        }

        private void ButtonBtm_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonClickAction != null)
            {
                ButtonClickAction.DynamicInvoke();

                SignalImg.Opacity = 1;
                DoubleAnimation da = new DoubleAnimation();
                da.From = 1;
                da.To = 0;
                da.Duration = new Duration(TimeSpan.FromSeconds(.8));
                SignalImg.BeginAnimation(OpacityProperty, da);          
            }
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            ButtonBtn.Content = BtnName;
            ButtonBtn.ToolTip = Description;
            DescTxt.Text = Description;
        }
    }
}
