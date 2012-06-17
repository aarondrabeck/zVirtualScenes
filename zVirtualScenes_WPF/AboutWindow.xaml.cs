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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using zVirtualScenes;

namespace zVirtualScenes_WPF
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void AboutWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            FullNameTxtBl.Text = Utils.ApplicationName;
            VersionTxtBl.Text = string.Format("Version {0}", Utils.ApplicationVersion);
        }

        private void WebsiteLink_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://code.google.com/p/zvirtualscenes/");
            }
            catch (Exception ex)
            {
                //BUG FIX: #2055
                MessageBox.Show(ex.Message, "Error launching Google maps");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
