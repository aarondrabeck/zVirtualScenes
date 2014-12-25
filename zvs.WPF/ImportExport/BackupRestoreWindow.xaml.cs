using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using zvs.DataModel;
using zvs.Processor;
using zvs.Processor.ImportExport;

namespace zvs.WPF.ImportExport
{
    /// <summary>
    /// Interaction logic for BackupRestoreWindow.xaml
    /// </summary>
    public partial class BackupRestoreWindow
    {
        private IFeedback<LogEntry> Log { get; set; }
        private IEntityContextConnection EntityContextConnection { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        public string BackupDirectory
        {
            get { return (string)GetValue(BackupDirectoryProperty); }
            set { SetValue(BackupDirectoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackupDirectory.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackupDirectoryProperty =
            DependencyProperty.Register("BackupDirectory", typeof(string), typeof(BackupRestoreWindow), new PropertyMetadata(""));

        public BackupRestoreWindow(IFeedback<LogEntry> log, IEntityContextConnection entityContextConnection)
        {
            Log = log;
            EntityContextConnection = entityContextConnection;
            CancellationTokenSource = new CancellationTokenSource();
            InitializeComponent();
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RestoreBackupListView_Loaded(object sender, RoutedEventArgs e)
        {
            //note order matters here when importing
            var backupRestores = new List<BackupRestore>
            {
                new DeviceBackupRestore(EntityContextConnection),
                new GroupBackupRestore(EntityContextConnection),
                new JavascriptBackupRestore(EntityContextConnection),
                new SceneBackupRestore(EntityContextConnection),
                new TriggerBackupRestore(EntityContextConnection),
                new ScheduledTaskBackupRestore(EntityContextConnection)
            };

            foreach (var uc in backupRestores.Select(backupRestore => new BackupRestoreControl { BackupRestore = backupRestore }))
            {
                RestoreBackupListView.Items.Add(uc);
            }
        }

        private async void BackupBtn_Click(object sender, RoutedEventArgs e)
        {
            await ResetUi();
            BackupBtn.IsEnabled = false;

            if (!Directory.Exists(BackupDirectory))
            {
                MessageBox.Show("Backup Failed", "Please select a valid directory first.", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return;
            }

            var enabledUc = RestoreBackupListView.Items
                .OfType<BackupRestoreControl>()
                .Where(o => o.ShouldRun)
                .ToList();

            foreach (var uc in enabledUc)
            {
                uc.State = BackupRestoreControl.BackupRestoreControlState.Working;
                await Task.Delay(50);
                var file = Path.Combine(BackupDirectory, uc.BackupRestore.FileName);
                var result = await uc.BackupRestore.ExportAsync(file, CancellationTokenSource.Token);

                if (result.HasError)
                {
                    await Log.ReportErrorAsync(result.Message, CancellationTokenSource.Token);
                    uc.State = BackupRestoreControl.BackupRestoreControlState.Error;
                }
                else
                {
                    await Log.ReportInfoAsync(result.Message, CancellationTokenSource.Token);
                    uc.State = BackupRestoreControl.BackupRestoreControlState.Complete;
                }
                await Task.Delay(50);
            }

            BackupBtn.IsEnabled = true;
        }

        private async Task ResetUi()
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
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = dlg.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.OK) return;
            BackupDirectory = dlg.SelectedPath;

            //Save selected DIR to database
            using (var context = new ZvsContext(EntityContextConnection))
            {
                await ProgramOption.TryAddOrEditAsync(context, new ProgramOption()
                {
                    UniqueIdentifier = "BackupLocation",
                    Value = dlg.SelectedPath
                }, CancellationTokenSource.Token);
            }
        }

        private async void RestoreBtn_Click(object sender, RoutedEventArgs e)
        {
            await ResetUi();
            BackupBtn.IsEnabled = false;

            if (!Directory.Exists(BackupDirectory))
            {
                MessageBox.Show("Backup Failed", "Please select a valid directory first.", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return;
            }

            var enabledUc = RestoreBackupListView.Items
                .OfType<BackupRestoreControl>()
                .Where(o => o.ShouldRun)
                .ToList();

            foreach (var uc in enabledUc)
            {
                uc.State = BackupRestoreControl.BackupRestoreControlState.Working;
                await Task.Delay(50);
                var file = Path.Combine(BackupDirectory, uc.BackupRestore.FileName);
                var result = await uc.BackupRestore.ImportAsync(file, CancellationTokenSource.Token);

                if (result.HasError)
                {
                    await Log.ReportErrorAsync(result.Message, CancellationTokenSource.Token);
                    uc.State = BackupRestoreControl.BackupRestoreControlState.Error;
                }
                else
                {
                    await Log.ReportInfoAsync(result.Message, CancellationTokenSource.Token);
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
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var option = await context.ProgramOptions.FirstOrDefaultAsync(o => o.UniqueIdentifier == "BackupLocation");
                BackupDirectory = option != null ? option.Value : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }
    }
}
