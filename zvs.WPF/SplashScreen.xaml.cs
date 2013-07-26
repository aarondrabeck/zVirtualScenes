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
