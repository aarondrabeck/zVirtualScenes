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
        }

        private void WebsiteLink_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start("https://github.com/aarondrabeck/zVirtualScenes");
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
