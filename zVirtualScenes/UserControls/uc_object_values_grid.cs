using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesApplication.Structs;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesAPI.Structs;
using zVirtualScenesAPI;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_object_values_grid : UserControl
    {
        public DataTable _loadedValues;

        public uc_object_values_grid()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            dataListViewStates.DataSource = null;
            dataListViewStates.ClearObjects();
        }

        public void UpdateControl(int ObjectID)
        {
            BindingList<zwObjectValue> Values = new BindingList<zwObjectValue>();            
            _loadedValues = DatabaseControl.GetObjectValues(ObjectID);
            foreach (DataRow dr in _loadedValues.Rows)
            {
                zwObjectValue objState = new zwObjectValue();
                objState.Label = dr["txt_label_name"].ToString();
                objState.Value = dr["txt_value"].ToString();
                Values.Add(objState);
            }

            dataListViewStates.DataSource = null;
            dataListViewStates.DataSource = Values;
        }

        
    }    
}
