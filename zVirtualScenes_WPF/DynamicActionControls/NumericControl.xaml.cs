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

namespace zVirtualScenes_WPF.DynamicActionControls
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class NumericControl : UserControl
    {
        private string name = string.Empty;
        private string Description = string.Empty;
        private string defaultVaule = string.Empty;
        private Action<string> SendCommandAction = null;
        private NumberType numType = NumberType.Integer;
        bool isLoaded = false;
        string lastValue = string.Empty;
        bool hasChanged = false;

        public NumericControl(string Name, string Description, string defaultVaule, NumberType numType, Action<string> SendCommandAction, BitmapImage icon)
        {
            this.name = Name;
            this.numType = numType;
            this.defaultVaule = defaultVaule;
            this.Description = Description;
            this.SendCommandAction = SendCommandAction;

            InitializeComponent();

            this.SignalImg.Source = icon;
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Description))
            {
                NameTxt.Text = string.Format("{0} ({1})", name, numType.ToString());
                DescTxt.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                NameTxt.Text = name;
                DescTxt.Text = string.Format("{0} ({1})", Description, numType.ToString());
            }

            TextBox.ToolTip = string.Format("{0} ({1})", Description, numType.ToString());
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
            //Allow arrown and delete keys
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
                return; 

            if (e.Key == Key.Enter)
            {
                if (!isEntryValid(TextBox.Text))                
                    TextBox.Text = defaultVaule;
                else                
                    SendCommand(); 

                e.Handled = true;
                return;
            }

            //Deny certain keys
            Regex regex = new Regex("[0-9.]");
            if (!regex.IsMatch(e.Key.ToString()))
            {
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
            if (string.IsNullOrEmpty(TextBox.Text))
                return false;

            switch (numType)
            {
                case NumberType.Byte:
                    {
                        byte num = 0;
                        if (byte.TryParse(value, out num))
                        {
                            if (num >= byte.MinValue && num <= byte.MaxValue)
                                return true;
                        }
                        break;
                    }
                case NumberType.Decimal:
                    {
                        decimal num = 0;
                        if (decimal.TryParse(value, out num))
                        {
                            if (num >= decimal.MinValue && num <= decimal.MaxValue)
                                return true;
                        }
                        break;
                    }
                case NumberType.Integer:
                    {
                        int num = 0;
                        if (int.TryParse(value, out num))
                        {
                            if (num >= int.MinValue && num <= int.MaxValue)
                                return true;
                        }
                        break;
                    }
                case NumberType.Short:
                    {
                        short num = 0;
                        if (short.TryParse(value, out num))
                        {
                            if (num >= short.MinValue && num <= short.MaxValue)
                                return true;
                        }
                        break;
                    }
                case NumberType.ComPort:
                    {
                        int num = 0;
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
        
        public enum NumberType
        {
            Integer,
            Decimal,
            Byte,
            Short,
            ComPort
        }

        
    }
}
