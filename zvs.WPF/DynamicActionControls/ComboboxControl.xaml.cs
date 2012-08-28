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
    public partial class ComboboxControl : UserControl
    {
        private string name = string.Empty;
        private string Description = string.Empty;
        private Action<string> SelectionChangedAction = null;
        private bool isLoaded = false;
        List<string> OptionObjects = new List<string>();
        string SelectedObject = string.Empty;

        public ComboboxControl(string Name, string Description, List<string> OptionObjects, string SelectedObject, Action<string> SelectionChangedAction, BitmapImage icon)
        {
            this.name = Name;
            this.Description = Description;
            this.OptionObjects = OptionObjects;
            this.SelectedObject = SelectedObject;
            this.SelectionChangedAction = SelectionChangedAction;
            InitializeComponent();

            this.SignalImg.Source = icon;
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            isLoaded = false;

            foreach (object option in OptionObjects)
            {
                ComboBox.Items.Add(option);
            }
            ComboBox.SelectedValue = SelectedObject;
                        
            if (string.IsNullOrEmpty(Description))
                DescTxt.Visibility = System.Windows.Visibility.Collapsed;

            NameTxt.Text = name;
            ComboBox.ToolTip = Description;
            DescTxt.Text = Description;

            isLoaded = true;
        } 

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded)
            {
                if (SelectionChangedAction != null && ComboBox.SelectedItem != null)
                {
                    SelectionChangedAction.DynamicInvoke(ComboBox.SelectedItem);

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
}
