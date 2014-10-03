using System.Windows.Controls;

namespace zvs.WPF.JavaScript
{
    /// <summary>
    /// Interaction logic for JSEditor.xaml
    /// </summary>
    public partial class JSEditor : UserControl
    {
        public JSEditor()
        {
            InitializeComponent();

            SCEditor.ConfigurationManager.Language = "js";
            SCEditor.Margins[0].Width = 20;            
            this.Editor = SCEditor;
        }
        public ScintillaNET.Scintilla Editor { get; set; }
    }
}
