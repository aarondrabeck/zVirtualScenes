using System;
using System.Diagnostics;
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
            Title = $"{Utils.ApplicationNameAndVersion} has crashed";
            TitleTxtBl.Text =
                $"Woops! {Utils.ApplicationName} has encountered a problem and needs to close. We are sorry for the inconvenience.";
            ErrorTxtBx.Text = Error;
        }

        private void SendErrorBtn_Click(object sender, RoutedEventArgs e)
        {
            var targetURL =
                $@"mailto:{HttpUtility.UrlEncode("zvsErrorReports@noncelabs.com")}?Subject={
                    HttpUtility.UrlEncode(Utils.ApplicationNameAndVersion + "Fatal Exception Error Report")}&Body={
                    HttpUtility.UrlEncode(Error)}";
            
            try
            {
                Process.Start(targetURL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening e-mail client");
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
    }
}
