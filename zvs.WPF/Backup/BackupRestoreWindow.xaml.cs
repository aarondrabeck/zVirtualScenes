using System;
using System.Collections.Generic;
using System.IO;
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
using zvs.Processor.Backup;
using zvs;

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
            var backupRestores = new List<BackupRestore>();
            backupRestores.Add(new DeviceBackupRestore());
            backupRestores.Add(new GroupBackupRestore());
            backupRestores.Add(new JavascriptBackupRestore());
            backupRestores.Add(new TriggerBackupRestore());

            foreach (var backupRestore in backupRestores)
            {
                var uc = new BackupRestoreControl();
                uc.BackupRestore = backupRestore;
                RestoreBackupListView.Items.Add(uc);
            }

            BackupDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
                var file = System.IO.Path.Combine(BackupDirectory, uc.BackupRestore.FileName);
                var result = await uc.BackupRestore.ExportAsync(file);

                if (result.HasError)
                {
                    ((App)App.Current).zvsCore.log.Error(result.Message);
                    uc.State = BackupRestoreControl.BackupRestoreControlState.Error;
                }
                else
                {
                    ((App)App.Current).zvsCore.log.Info(result.Message);
                    uc.State = BackupRestoreControl.BackupRestoreControlState.Complete;
                }
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
        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                BackupDirectory = dlg.SelectedPath;
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
                var file = System.IO.Path.Combine(BackupDirectory, uc.BackupRestore.FileName);
                var result = await uc.BackupRestore.ImportAsync(file);

                if (result.HasError)
                {
                    ((App)App.Current).zvsCore.log.Error(result.Message);
                    uc.State = BackupRestoreControl.BackupRestoreControlState.Error;
                }
                else
                {
                    ((App)App.Current).zvsCore.log.Info(result.Message);
                    uc.State = BackupRestoreControl.BackupRestoreControlState.Complete;
                }
            }

            BackupBtn.IsEnabled = true;
        }
    }
}
