using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using zvs.Processor.Backup;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.WPF.Backup
{
    /// <summary>
    /// Interaction logic for BackupRestoreWindow.xaml
    /// </summary>
    public partial class BackupRestoreWindow : Window
    {
        public string BackupDirectory
        {
            get { return (string)GetValue(BackupDirectoryProperty); }
            set { SetValue(BackupDirectoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackupDirectory.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackupDirectoryProperty =
            DependencyProperty.Register("BackupDirectory", typeof(string), typeof(BackupRestoreWindow), new PropertyMetadata(""));


        public BackupRestoreWindow()
        {
            InitializeComponent();
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RestoreBackupListView_Loaded(object sender, RoutedEventArgs e)
        {
            //note order matters here when importing
            var backupRestores = new List<BackupRestore>();
            backupRestores.Add(new DeviceBackupRestore());
            backupRestores.Add(new GroupBackupRestore());
            backupRestores.Add(new JavascriptBackupRestore());
            backupRestores.Add(new SceneBackupRestore());
            backupRestores.Add(new TriggerBackupRestore());
            backupRestores.Add(new ScheduledTaskBackupRestore());

            foreach (var backupRestore in backupRestores)
            {
                var uc = new BackupRestoreControl();
                uc.BackupRestore = backupRestore;
                RestoreBackupListView.Items.Add(uc);
            }
        }

        private async void BackupBtn_Click(object sender, RoutedEventArgs e)
        {
            await ResetUI();
            BackupBtn.IsEnabled = false;

            if (!Directory.Exists(BackupDirectory))
            {
                MessageBox.Show("Backup Failed", "Please select a valid directory first.", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return;
            }

            var enabledUC = RestoreBackupListView.Items
                .OfType<BackupRestoreControl>()
                .Where(o => o.ShouldRun)
                .ToList();

            foreach (var uc in enabledUC)
            {
                uc.State = BackupRestoreControl.BackupRestoreControlState.Working;
                await Task.Delay(50);
                var file = System.IO.Path.Combine(BackupDirectory, uc.BackupRestore.FileName);
                var result = await uc.BackupRestore.ExportAsync(file);

                if (result.HasError)
                {
                    ((App)App.Current).ZvsEngine.log.Error(result.Message);
                    uc.State = BackupRestoreControl.BackupRestoreControlState.Error;
                }
                else
                {
                    ((App)App.Current).ZvsEngine.log.Info(result.Message);
                    uc.State = BackupRestoreControl.BackupRestoreControlState.Complete;
                }
                await Task.Delay(50);
            }

            BackupBtn.IsEnabled = true;
        }

        private async Task ResetUI()
        {
            var ucs = RestoreBackupListView.Items
               .OfType<BackupRestoreControl>()
               .ToList();

            foreach (var uc in ucs)
                uc.State = BackupRestoreControl.BackupRestoreControlState.None;

            await Task.Delay(100);
        }
        private async void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                BackupDirectory = dlg.SelectedPath;

                //Save selected DIR to database
                using (var context = new ZvsContext())
                {
                    await ProgramOption.TryAddOrEditAsync(context, new ProgramOption()
                    {
                        UniqueIdentifier = "BackupLocation",
                        Value = dlg.SelectedPath
                    });
                }
            }
        }

        private async void RestoreBtn_Click(object sender, RoutedEventArgs e)
        {
            await ResetUI();
            BackupBtn.IsEnabled = false;

            if (!Directory.Exists(BackupDirectory))
            {
                MessageBox.Show("Backup Failed", "Please select a valid directory first.", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return;
            }

            var enabledUC = RestoreBackupListView.Items
                .OfType<BackupRestoreControl>()
                .Where(o => o.ShouldRun)
                .ToList();

            foreach (var uc in enabledUC)
            {
                uc.State = BackupRestoreControl.BackupRestoreControlState.Working;
                await Task.Delay(50);
                var file = System.IO.Path.Combine(BackupDirectory, uc.BackupRestore.FileName);
                var result = await uc.BackupRestore.ImportAsync(file);

                if (result.HasError)
                {
                    ((App)App.Current).ZvsEngine.log.Error(result.Message);
                    uc.State = BackupRestoreControl.BackupRestoreControlState.Error;
                }
                else
                {
                    ((App)App.Current).ZvsEngine.log.Info(result.Message);
                    uc.State = BackupRestoreControl.BackupRestoreControlState.Complete;
                }
                await Task.Delay(50);
            }

            var app = (App)Application.Current;
            await app.RefreshCommandDescripitions();
            await app.RefreshTriggerDescripitions();

            BackupBtn.IsEnabled = true;
        }

        private async void this_Loaded(object sender, RoutedEventArgs e)
        {
            using (ZvsContext context = new ZvsContext())
            {
                var option = await context.ProgramOptions.FirstOrDefaultAsync(o => o.UniqueIdentifier == "BackupLocation");
                if (option != null)
                    BackupDirectory = option.Value;
                else
                    BackupDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }
    }
}
