using System.Windows.Forms;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_script_editor : UserControl
    {
        public uc_script_editor()
        {
            InitializeComponent();
        }

        public void SetScript(string script)
        {
            txtScript.Text = script;
        }

        public string GetScript()
        {
            return txtScript.Text;
        }
    }
}
