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
using zvs.Entities;


namespace zvs.WPF
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        private bool isLoading = true;
        public SettingWindow()
        {
            InitializeComponent();
        }

        private void SettingWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            using (zvsContext context = new zvsContext())
            {
                string direction = ProgramOption.GetProgramOption(context, "LOGDIRECTION");
                if (direction != null && direction == "Descending")
                    DecenLogOrderRadioBtn.IsChecked = true;
                else
                    AcenLogOrderRadioBtn.IsChecked = true;
            }
            isLoading = false;
        }

        private async void AcenLogOrderRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            DecenLogOrderRadioBtn.IsChecked = false;
            if (!isLoading)
            {
                using (zvsContext context = new zvsContext())
                {
                    await ProgramOption.TryAddOrEditAsync(context, new ProgramOption()
                    {
                        UniqueIdentifier = "LOGDIRECTION",
                        Value = "Ascending"
                    });
                }
            }
        }

        private async void DecenLogOrderRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            AcenLogOrderRadioBtn.IsChecked = false;
            if (!isLoading)
            {
                using (zvsContext context = new zvsContext())
                {
                    await ProgramOption.TryAddOrEditAsync(context, new ProgramOption()
                    {
                        UniqueIdentifier = "LOGDIRECTION",
                        Value = "Descending"
                    });
                }
            }
        }

        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
