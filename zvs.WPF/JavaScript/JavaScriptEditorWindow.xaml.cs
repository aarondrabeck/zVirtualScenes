using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using zvs.DataModel;
using zvs.Processor;


namespace zvs.WPF.JavaScript
{
    /// <summary>
    /// Interaction logic for JSTriggerEditorWindow.xaml
    /// </summary>
    public partial class JavaScriptEditorWindow
    {
        private IFeedback<LogEntry> Log { get; set; }
        private readonly App _app = (App)Application.Current;
        private readonly ZvsContext _context;
        private readonly JavaScriptCommand _command;
        public bool Canceled = true;
        private readonly ObservableCollection<JSResult> _results = new ObservableCollection<JSResult>();

        private bool _isRunning;
        private bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                Dispatcher.Invoke(() =>
                {
                    TestMI.IsEnabled = !value;
                });
                _isRunning = value;
            }
        }

        public JavaScriptEditorWindow(ZvsContext context, JavaScriptCommand command)
        {
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "JavaScript Editor" };
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = context;
            _command = command;
            InitializeComponent();
        }
#if DEBUG
        ~JavaScriptEditorWindow()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("TriggerEditorWindow Deconstructed.");
        }
#endif
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (_command.Script != null)
                TriggerScriptEditor.Editor.AppendText(_command.Script);

            CmdNameTxtBx.Text = _command.Name;
            var jSResultViewSource = ((CollectionViewSource)(FindResource("jSResultViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            jSResultViewSource.Source = _results;
        }

        private void Run()
        {
            _results.Clear();
            var script = TriggerScriptEditor.Editor.Text;
            if (string.IsNullOrEmpty(script)) return;
            IsRunning = true;
            SetFeedBackText("Executing JavaScript...");

            //This is run outside of CommandProcessor because it is not a command yet.  It is for testing JavaScript
            //TODO: enable
            //var jse = new Processor.JavaScriptExecuter(this, _app.ZvsEngine);
            //jse.onReportProgress += (sender, args) =>
            //{
            //    SetFeedBackText(args.Progress);
            //    _app.ZvsEngine.log.Info(args.Progress);
            //};
            //JavaScriptExecuter.JavaScriptResult result = await jse.ExecuteScriptAsync(script, _context);
            //IsRunning = false;
            //_app.ZvsEngine.log.Info(result.Details);
            //SetFeedBackText(result.Details);
        }

        public void SetFeedBackText(string text)
        {
            Dispatcher.Invoke(() => _results.Add(new JSResult { Description = text }));
        }

        private void TriggerScriptEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                TestMI_Click(this, null);

            if (e.Key == Key.Escape)
                CancelMI_Click();

            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
                SaveMI_Click(this, null);
        }

        private void RunSceneMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "runScene('All On');\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void RunDeviceCommandMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "runDeviceCommand('Office Light','Set Level', '99');\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void AddDelayMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "delay(\"runDeviceCommand('Office Light','Set Level', '99');\", 3000);\n";
            TriggerScriptEditor.Editor.InsertText(script);
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

            ofd.ShowDialog(this);

            if (string.IsNullOrEmpty(ofd.FileName) || !File.Exists(ofd.FileName)) return;
            var path = ofd.FileName;
            if (path.StartsWith(Utils.AppPath))
            {
                path = path.Replace(Utils.AppPath, ".");
            }

            var script = string.Format("require('{0}')\n", path.Replace("\\", "\\\\"));
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void ExecShellMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "shell(\"http://google.com\", \"\");\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void ReportProgressMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "reportProgress(\"Hello World!\");\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void LogInfoMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "log(\"All done!\");\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void LogErrorMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "error(\"Oh Snap!\");\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void LogWarningMI_Click(object sender, RoutedEventArgs e)
        {
            const string script = "warn(\"Warning file missing!\");\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void CancelMI_Click()
        {
            Close();
        }

        private void SaveMI_Click(object sender, RoutedEventArgs e)
        {
            _command.Name = CmdNameTxtBx.Text;
            _command.Script = TriggerScriptEditor.Editor.Text;
            Canceled = false;
            Close();
        }

        private void TestMI_Click(object sender, RoutedEventArgs e)
        {
            if (!IsRunning)
                Run();
        }

        private void CanceleMI_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
