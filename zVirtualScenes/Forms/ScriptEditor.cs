using System.Data;
using System.Windows.Forms;
using zVirtualScenesCommon.DatabaseCommon;

namespace zVirtualScenesApplication.Forms
{
    public partial class ScriptEditor : Form
    {
        private int _scriptId;

        private DataTable _objects;
        private DataTable _events;

        public ScriptEditor(int scriptId)
        {
            InitializeComponent();
            
            _scriptId = scriptId;
        }

        private void ScriptEditor_Load(object sender, System.EventArgs e)
        {
            UpdateObjectsList();
            LoadScript();
        }

        private void cboObjects_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboObjects.SelectedIndex > -1)
            {
                UpdateEventList();
            }
        }

        private void UpdateObjectsList()
        {
            _objects = DatabaseControl.GetObjects(false);
            cboObjects.Items.Clear();
            foreach (DataRow dr in _objects.Rows)
            {
                cboObjects.Items.Add(dr["txt_object_name"].ToString());
            }
        }

        private void UpdateEventList()
        {
            if (cboObjects.SelectedIndex > -1)
            {
                int objectId;
                int.TryParse(_objects.Rows[cboObjects.SelectedIndex]["id"].ToString(), out objectId);
                _events = DatabaseControl.GetObjectValues(objectId);
                cboEvents.Items.Clear();
                foreach (DataRow dr in _events.Rows)
                {
                    cboEvents.Items.Add(dr["txt_label_name"].ToString());
                }
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            if (cboObjects.SelectedIndex > -1 && cboEvents.SelectedIndex > -1 &&
                !string.IsNullOrEmpty(txtScriptName.Text) && !string.IsNullOrEmpty(uc_script_editor1.GetScript()))
            {
                int objectId;
                int eventId;

                int.TryParse(_objects.Rows[cboObjects.SelectedIndex]["id"].ToString(), out objectId);
                int.TryParse(_events.Rows[cboEvents.SelectedIndex]["id"].ToString(), out eventId);

                if (_scriptId == 0)
                {
                    // Creating a new script
                    DatabaseControl.AddEventScript(objectId, eventId, txtScriptName.Text, uc_script_editor1.GetScript());
                }
                else
                {
                    // Updated a old script
                    DatabaseControl.UpdateEventScript(_scriptId, objectId, eventId, txtScriptName.Text, uc_script_editor1.GetScript());
                }

                MainForm.UpdateScripts = true;
                Close();
            }
        }

        private void LoadScript()
        {
            if (_scriptId > 0)
            {
                DataTable dt = DatabaseControl.GetEventScriptById(_scriptId);
                if (dt.Rows.Count == 1)
                {
                    txtScriptName.Text = dt.Rows[0]["txt_event_name"].ToString();
                    uc_script_editor1.SetScript(dt.Rows[0]["txt_script"].ToString());
                    cboObjects.Text = dt.Rows[0]["txt_object_name"].ToString();
                    UpdateEventList();
                    cboEvents.Text = dt.Rows[0]["evt_name"].ToString();
                }
            }
        }

        private void ScriptEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Escape))
            {
                this.Close();
            }
        }
    }
}
