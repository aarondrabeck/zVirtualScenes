using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace zVirtualScenesGUI.DynamicActionControls
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class StringControl : UserControl
    {
        private string name = string.Empty;
        private string Description = string.Empty;
        private string defaultVaule = string.Empty;
        private Action<string> SendCommandAction = null;
        bool isLoaded = false;
        string lastValue = string.Empty;
        bool hasChanged = false;

        public StringControl(string Name, string Description, string defaultVaule, Action<string> SendCommandAction, BitmapImage icon)
        {
            this.name = Name;
            this.defaultVaule = defaultVaule;
            this.Description = Description;
            this.SendCommandAction = SendCommandAction;

            InitializeComponent();

            this.SignalImg.Source = icon;
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Description))
                DescTxt.Visibility = System.Windows.Visibility.Collapsed;

            NameTxt.Text = name;
            TextBox.ToolTip = Description;
            DescTxt.Text = Description;

            TextBox.Text = defaultVaule;
            lastValue = defaultVaule;

            isLoaded = true;
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (isLoaded)
            {
                if (isEntryValid(TextBox.Text))
                {
                    TextBox.Background = new SolidColorBrush(Colors.White);
                    
                    if (lastValue != TextBox.Text)                    
                        hasChanged = true;                    
                }
                else
                    TextBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 198, 198));

            }
        }

        private void TextBox_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!isEntryValid(TextBox.Text))                
                    TextBox.Text = defaultVaule;
                else                
                    SendCommand(); 

                e.Handled = true;
                return;
            }
        }

        private void TextBox_LostFocus_1(object sender, RoutedEventArgs e)
        {
            if (isEntryValid(TextBox.Text))            
                SendCommand();
        }

        private void SendCommand()
        {
            if (SendCommandAction != null && hasChanged)
            {
                lastValue = TextBox.Text;
                hasChanged = false;
                SendCommandAction.DynamicInvoke(TextBox.Text);

                SignalImg.Opacity = 1;
                DoubleAnimation da = new DoubleAnimation();
                da.From = 1;
                da.To = 0;
                da.Duration = new Duration(TimeSpan.FromSeconds(.8));
                SignalImg.BeginAnimation(OpacityProperty, da);               
            }
        }

        private bool isEntryValid(string value)
        {
            if (!string.IsNullOrEmpty(TextBox.Text))
                return true;
            
            return false;
        }        
    }
}
