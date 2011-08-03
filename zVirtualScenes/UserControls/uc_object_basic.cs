using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zVirtualScenesAPI;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_object_basic : UserControl
    {
        int _objId;
        string _objName;

        public uc_object_basic()
        {
            InitializeComponent();
        }

        public void UpdateControl(int objId)
        {
            this._objId = objId;
            _objName = API.Object.GetObjectName(_objId);
            textBoxName.Text = _objName;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Object Name
            if (String.IsNullOrEmpty(textBoxName.Text))
                MessageBox.Show("Invalid Object Name", API.GetProgramNameAndVersion);              
            else
                API.Object.SetObjectName(_objId, textBoxName.Text);
        }
    }
}
