using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using zvs.Processor;

namespace zvs.WPF
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

#if DEBUG        
        ~AboutWindow()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("AboutWindow Deconstructed.");
        }
#endif

        private void AboutWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            FullNameTxtBl.Text = Utils.ApplicationName;
            VersionTxtBl.Text = string.Format("Version {0}", Utils.ApplicationVersionLong);
            Copyrighttxtbl.Text = string.Format("© {0} Nonce Labs", DateTime.Now.Year);
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
