using System.Windows;

namespace zvs.WPF
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        // <summary>
        /// Sets the loading text
        /// </summary>
        /// <param name="msg"></param>
        public void SetLoadingText(string msg)
        {
            this.txtLoading.Text = string.Format("{0}...", msg);
        }

        public void SetLoadingTextFormat(string format, params object[] args)
        {
            SetLoadingText(string.Format(format, args));
        }
    }

}
