using System.Windows.Forms;
using ScintillaNET;

namespace zvs.WPF.JavaScript
{
    /// <summary>
    /// Interaction logic for JSEditor.xaml
    /// </summary>
    public partial class JsEditor
    {
        public JsEditor()
        {
            InitializeComponent();

            SCEditor.ConfigurationManager.Language = "js";
            SCEditor.Margins[0].Width = 20;
            SCEditor.BorderStyle = BorderStyle.None;      
            Editor = SCEditor;
        }
        public Scintilla Editor { get; set; }
    }
}
