using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using zvs.DataModel;
using zvs.Processor;

namespace zvs.WPF.JavaScript
{
    /// <summary>
    /// Interaction logic for JavaScriptEditor.xaml
    /// </summary>
    public partial class JavaScriptEditor
    {
        private ObservableCollection<LogEntry> LogEntries { get; }
        private ZvsContext Context { get; }
        private readonly App _app = (App)Application.Current;
        private IFeedback<LogEntry> Log { get; }

        public JavaScriptEditor()
        {
            Context = new ZvsContext(_app.EntityContextConnection);
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Javascript Editor" };
            LogEntries = new ObservableCollection<LogEntry>();
            InitializeComponent();
        }

        private async void JavaScriptEditor_OnInitialized(object sender, EventArgs e)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            //Do not load your data at design time.
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            await Context.JavaScriptCommands
                .ToListAsync();

            //Load your data here and assign the result to the CollectionViewSource.
            var myCollectionViewSource = (CollectionViewSource)Resources["JavascriptCommandViewSource"];
            myCollectionViewSource.Source = Context.JavaScriptCommands.Local;
#if DEBUG
            sw.Stop();
            Debug.WriteLine("Javascript creator initialized in {0}", sw.Elapsed.ToString() as object);
#endif
        }

        private async void ButtonDeleteCommand_OnClick(object sender, RoutedEventArgs e)
        {
            var jsCommand = JavascriptGrid.SelectedItem as JavaScriptCommand;
            if (jsCommand == null) return;

            if (MessageBox.Show($"Are you sure you want to delete the '{jsCommand.Name}' JavaScript Command?",
                    "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            Context.JavaScriptCommands.Local.Remove(jsCommand);
            await SaveChangesAsync();

            JavascriptGrid.Focus();
        }

        private async void JavascriptGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            var s = e.Row.DataContext as Scene;
            if (s != null)
                if (string.IsNullOrEmpty(s.Name))
                    s.Name = "New Command";

            //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated in time for this event
            await SaveChangesAsync();
        }

        private void JavascriptGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LogEntries.Clear();
            
            var jsCommand = JavascriptGrid.SelectedItem as JavaScriptCommand;
            if (jsCommand == null) return;

            JsEditor.Editor.ResetText();

            if (jsCommand.Script != null)
                JsEditor.Editor.AppendText(jsCommand.Script);

            EvaluateSaveButton();
        }

        private void EvaluateSaveButton()
        {
            var jsCommand = JavascriptGrid.SelectedItem as JavaScriptCommand;
            if (jsCommand == null) return;

            if (jsCommand.Script == null)
                jsCommand.Script = string.Empty;

            if(JsEditor.Editor.Text == null)
                JsEditor.Editor.Text = string.Empty;

            var sha256 = SHA256.Create();
            var editorText = Encoding.UTF8.GetBytes(JsEditor.Editor.Text);
            var editorTextStream = new MemoryStream(editorText);
            var editorTextHash = sha256.ComputeHash(editorTextStream);

            var commandText = Encoding.UTF8.GetBytes(jsCommand.Script);
            var commandTextStream = new MemoryStream(commandText);
            var commandTextHash = sha256.ComputeHash(commandTextStream);

            SaveButton.IsEnabled = !Encoding.Default.GetString(editorTextHash).Equals(Encoding.Default.GetString(commandTextHash));
        }

        private async void JsEditor_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                ButtonTestScript_OnClick(this, null);

            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
                await SaveChangesAsync();
             
            EvaluateSaveButton();
        }

        private async Task SaveChangesAsync()
        {
            var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving Javascript command. {0}", result.Message);

            SignalImg.Opacity = 1;
            var da = new DoubleAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(.8)) };
            SignalImg.BeginAnimation(OpacityProperty, da);

            EvaluateSaveButton();
        }

        private async void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            var jsCommand = JavascriptGrid.SelectedItem as JavaScriptCommand;
            if (jsCommand == null) return;

            jsCommand.Script = JsEditor.Editor.Text;
            await SaveChangesAsync();
        }

        private async void ButtonTestScript_OnClick(object sender, RoutedEventArgs e)
        {
            LogEntries.Clear();

            var jsCommand = JavascriptGrid.SelectedItem as JavaScriptCommand;
            if (jsCommand == null) return;

            var inMemoryLog = new InMemoryFeedback { Source = "JavaScript Test Window" };

            inMemoryLog.LogEntries.CollectionChanged += LogEntries_CollectionChanged;

            var myCollectionViewSource = (CollectionViewSource)Resources["LogEntryViewSource"];
            if (myCollectionViewSource != null)
                myCollectionViewSource.Source = LogEntries;

            await inMemoryLog.ReportInfoAsync("JavaScript test run started...", CancellationToken.None);

            var script = JsEditor.Editor.Text;
            if (string.IsNullOrEmpty(script)) return;
            TestButton.IsEnabled = false;

            var javaScriptRunner = new JavaScriptRunner(inMemoryLog, new CommandProcessor(_app.ZvsEngine.AdapterManager, _app.EntityContextConnection, inMemoryLog), _app.EntityContextConnection);

            var result = await javaScriptRunner.ExecuteScriptAsync(script, CancellationToken.None);
            if (result.HasError)
                await inMemoryLog.ReportErrorAsync(result.Message, CancellationToken.None);
            else
                await inMemoryLog.ReportInfoAsync(result.Message, CancellationToken.None);

            TestButton.IsEnabled = true;
        }

        private void LogEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.NewItems.Count <= 0) return;

                foreach (var item in e.NewItems)
                    LogEntries.Add(item as LogEntry);
            });
        }

        private void AddFileMI_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                DefaultExt = "js",
                InitialDirectory = Utils.AppPath,
                Multiselect = true,
                Title = "Choose a JavaScript file to include..."
            };

            ofd.ShowDialog(Window.GetWindow(this));

            if (string.IsNullOrEmpty(ofd.FileName) || !File.Exists(ofd.FileName)) return;
            var path = ofd.FileName;
            if (path.StartsWith(Utils.AppPath))
            {
                path = path.Replace(Utils.AppPath, ".");
            }

            var script = $"require('{path.Replace("\\", "\\\\")}')\n";
            JsEditor.Editor.InsertText(script);
        }

        private void ExecShellMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "shell(\"http://google.com\", \"\");\n";
            JsEditor.Editor.InsertText(script);
        }

        private void RunDeviceCommandMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "runCommand(1, 2, '99');\n";
            JsEditor.Editor.InsertText(script);
        }

        private void AddDelayMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "setTimeout(\"logInfo(\"3 seconds have gone by!\");\", 3000);\n";
            JsEditor.Editor.InsertText(script);
        }

        private void LogInfoMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "logInfo(\"All done!\");\n";
            JsEditor.Editor.InsertText(script);
        }

        private void LogErrorMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "logError(\"Oh Snap!\");\n";
            JsEditor.Editor.InsertText(script);
        }

        private void LogWarningMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "logWarn(\"Warning file missing!\");\n";
            JsEditor.Editor.InsertText(script);
        }

    }
}
