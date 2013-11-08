using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using zvs.Entities;
using zvs.Processor;


namespace zvs.WPF.JavaScript
{
    /// <summary>
    /// Interaction logic for JSTriggerEditorWindow.xaml
    /// </summary>
    public partial class JavaScriptEditorWindow : Window
    {
        private App app = (App)Application.Current;
        private zvsContext Context;
        private JavaScriptCommand Command;
        public bool Canceled = true;
        private ObservableCollection<JSResult> Results = new ObservableCollection<JSResult>();

        private bool _isRunning = false;
        private bool isRunning
        {
            get { return _isRunning; }
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    TestMI.IsEnabled = !value;
                });
                _isRunning = value;
            }
        }


        public JavaScriptEditorWindow(zvsContext context, JavaScriptCommand command)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Context = context;
                this.Command = command;
                InitializeComponent();
            }
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
            if (Command.Script != null)
                TriggerScriptEditor.Editor.AppendText(Command.Script);

            CmdNameTxtBx.Text = Command.Name;
            System.Windows.Data.CollectionViewSource jSResultViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("jSResultViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            jSResultViewSource.Source = Results;
        }

        private async void Run()
        {
            Results.Clear();
            string script = TriggerScriptEditor.Editor.Text;
            if (!string.IsNullOrEmpty(script))
            {
                isRunning = true;
                SetFeedBackText("Executing JavaScript...");

                //This is run outside of CommandProcessor because it is not a command yet.  It is for testing JavaScript
                zvs.Processor.JavaScriptExecuter jse = new Processor.JavaScriptExecuter(this, app.zvsCore);
                jse.onReportProgress += (sender, args) =>
                {
                    SetFeedBackText(args.Progress);
                    app.zvsCore.log.Info(args.Progress);
                };
                JavaScriptExecuter.JavaScriptResult result = await jse.ExecuteScriptAsync(script, Context);
                isRunning = false;
                app.zvsCore.log.Info(result.Details);
                SetFeedBackText(result.Details);
            }
        }

        public void SetFeedBackText(string Text)
        {
            this.Dispatcher.Invoke(() =>
                {
                    Results.Add(new JSResult() { Description = Text });
                });
        }

        private void TriggerScriptEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                TestMI_Click(this, null);

            if (e.Key == Key.Escape)
                CancelMI_Click(this, null);

            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
                SaveMI_Click(this, null);
        }

        private void RunSceneMI_Click(object sender, RoutedEventArgs e)
        {
            var script = "runScene('All On');\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void RunDeviceCommandMI_Click(object sender, RoutedEventArgs e)
        {
            var script = "runDeviceCommand('Office Light','Set Level', '99');\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void AddDelayMI_Click(object sender, RoutedEventArgs e)
        {
            var script = "delay(\"runDeviceCommand('Office Light','Set Level', '99');\", 3000);\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void AddFileMI_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.DefaultExt = "js";
            ofd.InitialDirectory = zvs.Processor.Utils.AppPath;
            ofd.Multiselect = true;
            ofd.Title = "Choose a JavaScript file to include...";

            ofd.ShowDialog(this);

            if (!string.IsNullOrEmpty(ofd.FileName) && System.IO.File.Exists(ofd.FileName))
            {
                string path = ofd.FileName;
                if (path.StartsWith(zvs.Processor.Utils.AppPath))
                {
                    path = path.Replace(zvs.Processor.Utils.AppPath, ".");
                }

                var script = string.Format("require('{0}')\n", path.Replace("\\", "\\\\"));
                TriggerScriptEditor.Editor.InsertText(script);
            }
        }

        private void ExecShellMI_Click(object sender, RoutedEventArgs e)
        {
            var script = "shell(\"http://google.com\", \"\");\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void ReportProgressMI_Click(object sender, RoutedEventArgs e)
        {
            var script = "reportProgress(\"Hello World!\");\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void LogInfoMI_Click(object sender, RoutedEventArgs e)
        {
            var script = "log(\"All done!\");\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void LogErrorMI_Click(object sender, RoutedEventArgs e)
        {
            var script = "error(\"Oh Snap!\");\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void LogWarningMI_Click(object sender, RoutedEventArgs e)
        {
            var script = "warn(\"Warning file missing!\");\n";
            TriggerScriptEditor.Editor.InsertText(script);
        }

        private void CancelMI_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveMI_Click(object sender, RoutedEventArgs e)
        {
            Command.Name = CmdNameTxtBx.Text;
            Command.Script = TriggerScriptEditor.Editor.Text;
            Canceled = false;
            this.Close();
        }

        private void TestMI_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
                Run();
        }

        private void CanceleMI_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
