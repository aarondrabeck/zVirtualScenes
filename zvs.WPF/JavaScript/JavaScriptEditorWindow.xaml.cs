using System;
using System.Collections.Generic;
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


namespace zvs.WPF.JavaScript
{
    /// <summary>
    /// Interaction logic for JSTriggerEditorWindow.xaml
    /// </summary>
    public partial class JavaScriptEditorWindow : Window
    {
        private zvsContext Context;
        private JavaScriptCommand Command;
        public bool Canceled = true;

        public JavaScriptEditorWindow(zvsContext context, JavaScriptCommand command)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Context = context;
                this.Command = command;
                InitializeComponent();
            }
        }

        ~JavaScriptEditorWindow()
        {
            Debug.WriteLine("TriggerEditorWindow Deconstructed.");
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (Command.Script != null)
                TriggerScriptEditor.Editor.AppendText(Command.Script);

            CmdNameTxtBx.Text = Command.Name;
            TriggerScriptEditor.Editor.KeyUp += Editor_KeyUp;
        }

        void Editor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F5)
                Run();
        }
        private void Run()
        {
            string script = TriggerScriptEditor.Editor.Text;
            if (!string.IsNullOrEmpty(script))
            {
                zvs.Processor.JavaScriptExecuter jse = new Processor.JavaScriptExecuter();
                jse.ExecuteScript(script, Context);
            }

        }
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            Command.Name = CmdNameTxtBx.Text;
            Command.Script = TriggerScriptEditor.Editor.Text;
            Canceled = false;
            this.Close();
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            Run();
        }
    }
}
