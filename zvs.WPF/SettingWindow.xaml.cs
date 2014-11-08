using System.Windows;
using zvs.DataModel;
using System.Data.Entity;

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

        private async void SettingWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            EnableJavaScriptDebugger.IsChecked = zvs.Processor.JavaScriptExecuter.JavascriptDebugEnabled; 
            using (var context = new ZvsContext())
            {
                var option = await context.ProgramOptions.FirstOrDefaultAsync(o => o.UniqueIdentifier == "LOGDIRECTION");
                if (option != null && option.Value == "Descending")
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
                using (var context = new ZvsContext())
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
                using (var context = new ZvsContext())
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
            if(EnableJavaScriptDebugger.IsChecked.HasValue) zvs.Processor.JavaScriptExecuter.JavascriptDebugEnabled = EnableJavaScriptDebugger.IsChecked.Value;
            this.Close();
        }


    }
}
