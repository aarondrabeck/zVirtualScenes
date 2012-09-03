using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
