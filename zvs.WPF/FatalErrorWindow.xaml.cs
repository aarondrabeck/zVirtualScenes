using System;
using System.Web;
using System.Windows;
using zvs.Processor;

namespace zvs.WPF
{
    /// <summary>
    /// Interaction logic for FatalErrorWindow.xaml
    /// </summary>
    public partial class FatalErrorWindow : Window
    {
        private string Error = string.Empty;

        public FatalErrorWindow(string Error)
        {
            this.Error = Error;
            InitializeComponent();
        }

        private void FatalErrorWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            Title = string.Format("{0} has crashed",Utils.ApplicationNameAndVersion);
            TitleTxtBl.Text = string.Format("Woops! {0} has encountered a problem and needs to close. We are sorry for the inconvenience.", Utils.ApplicationName);
            ErrorTxtBx.Text = Error;
        }

        private void SendErrorBtn_Click(object sender, RoutedEventArgs e)
        {
            var targetURL = string.Format(@"mailto:{0}?Subject={1}&Body={2}",
                HttpUtility.UrlEncode("zvsErrorReports@noncelabs.com"), 
                HttpUtility.UrlEncode(Utils.ApplicationNameAndVersion + "Fatal Exception Error Report"), 
                HttpUtility.UrlEncode(Error));
            
            try
            {
                System.Diagnostics.Process.Start(targetURL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening e-mail client");
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
    }
}
