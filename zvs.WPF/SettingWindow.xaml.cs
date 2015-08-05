using System.Data.Entity;
using System.Windows;
using zvs.DataModel;

namespace zvs.WPF
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow
    {
        private readonly App _app = (App)Application.Current;
        private bool _isLoading = true;
        public SettingWindow()
        {
            InitializeComponent();
        }

        private async void SettingWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            using (var context = new ZvsContext(_app.EntityContextConnection))
            {
                var option = await context.ProgramOptions.FirstOrDefaultAsync(o => o.UniqueIdentifier == "LOGDIRECTION");
                if (option != null && option.Value == "Descending")
                    DecenLogOrderRadioBtn.IsChecked = true;
                else
                    AcenLogOrderRadioBtn.IsChecked = true;
            }
            _isLoading = false;
        }

        private async void AcenLogOrderRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            DecenLogOrderRadioBtn.IsChecked = false;
            if (_isLoading) return;
            using (var context = new ZvsContext(_app.EntityContextConnection))
            {
                await ProgramOption.TryAddOrEditAsync(context, new ProgramOption()
                {
                    UniqueIdentifier = "LOGDIRECTION",
                    Value = "Ascending"
                }, _app.Cts.Token);
            }
        }

        private async void DecenLogOrderRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            AcenLogOrderRadioBtn.IsChecked = false;
            if (_isLoading) return;
            using (var context = new ZvsContext(_app.EntityContextConnection))
            {
                await ProgramOption.TryAddOrEditAsync(context, new ProgramOption()
                {
                    UniqueIdentifier = "LOGDIRECTION",
                    Value = "Descending"
                }, _app.Cts.Token);
            }
        }

        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
