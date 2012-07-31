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
using zVirtualScenesModel;

namespace zVirtualScenesGUI.TriggerControls
{
    /// <summary>
    /// Interaction logic for JSTriggerEditorWindow.xaml
    /// </summary>
    public partial class JSTriggerEditorWindow : Window
    {
        private zvsLocalDBEntities context;
        private javascript_triggers trigger;
        public bool Canceled = true;

        public JSTriggerEditorWindow(javascript_triggers trigger, zvsLocalDBEntities context)
        {
            this.context = context;
            this.trigger = trigger;
            InitializeComponent();
        }

        ~JSTriggerEditorWindow()
        {
            Debug.WriteLine("TriggerEditorWindow Deconstructed.");
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //context.devices.ToList();
           // context.scenes.ToList();
            
            ScriptTxtBx.AppendText(trigger.Script);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            trigger.Script = new TextRange(ScriptTxtBx.Document.ContentStart, ScriptTxtBx.Document.ContentEnd).Text;

            Canceled = false;
            this.Close();
        }
    }
}
