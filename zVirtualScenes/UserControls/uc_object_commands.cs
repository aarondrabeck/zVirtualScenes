using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesAPI;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesCommon.Util;
using System.Drawing;
using zVirtualScenesApplication.Globals;
using System.Collections.Generic;
using zVirtualScenesAPI.Structs;
using zVirtualScenesAPI.Events;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_object_commands : UserControl
    {
        private string _objectName;
        private int _objectId;

        public uc_object_commands()
        {
            InitializeComponent();
        }

        public void UpdateObject(int objectId)
        {
            _objectId = objectId;
            pnlSettings.Controls.Clear();            
            int top = 0;

            _objectName = API.Object.GetObjectName(_objectId);
                       
            #region Object Commands
            DataTable object_commands = API.Commands.GetAllObjectCommandsForObject(objectId);

            Label CommandLabel = new Label();
            CommandLabel.Text = "'" +_objectName + "' Commands:" + (object_commands.Rows.Count > 0 ? "" : " none.");
            CommandLabel.AutoSize = true;
            CommandLabel.Font = new System.Drawing.Font(CommandLabel.Font.Name, CommandLabel.Font.Size, FontStyle.Bold);
            CommandLabel.Top = top;
            CommandLabel.Left = 0;
            CommandLabel.Height = 23;
            pnlSettings.Controls.Add(CommandLabel);
            top += 25;

            foreach (DataRow dr in object_commands.Rows)
            {
                int left = 0;
                string buttonText = String.Empty;
                int paramType;
                int.TryParse(dr["param_type"].ToString(), out paramType);

                int CommandId;
                int.TryParse(dr["id"].ToString(), out CommandId);
                string CommandIdentifier = CommandId + "-" + cmdType.Object.ToString() + "arg";

                #region Add Input Control Depending on type

                //ONLY NEEDED FOR LIST TYPES
                List<string> options = API.Commands.GetObjectCommandOptions(CommandId);
                string value = API.Object.Value.Get(objectId, dr["txt_custom_data1"].ToString());

                left = GlobalMethods.DrawDynamicUserInputBoxes(pnlSettings, (ParamType)paramType, top, left, CommandIdentifier, dr["txt_command_friendly_name"].ToString(), options, value, "");

                #endregion

                #region Add Button
                Button btn = new Button();
                btn.Name = dr["id"].ToString();
                btn.Text = dr["txt_command_friendly_name"].ToString();
                btn.Click += btnClick;
                btn.Tag = new btnInfo((ParamType)paramType, cmdType.Object);
                btn.Top = top;
                btn.Left = left;
                pnlSettings.Controls.Add(btn);

                using (Graphics cg = this.CreateGraphics())
                {
                    SizeF size = cg.MeasureString(btn.Text, btn.Font);
                    size.Width += 10; //add some padding
                    btn.Width = (int)size.Width;
                    left = (int)size.Width + left + 5;
                }
                #endregion

                #region Label

                Label CmdLabel = new Label();
                CmdLabel.Text = dr["txt_cmd_help"].ToString();
                CmdLabel.Top = top + 5;
                CmdLabel.Left = left;
                CmdLabel.Height = 23;
                pnlSettings.Controls.Add(CmdLabel);

                using (Graphics cg = this.CreateGraphics())
                {
                    SizeF size = cg.MeasureString(CmdLabel.Text, CmdLabel.Font);
                    size.Width += 6; //add some padding
                    CmdLabel.Width = (int)size.Width;
                    left += (int)size.Width + 5;
                }

                #endregion
                top += 35;
            }

            #endregion

            #region Object Type Commands

            DataTable object_type_commands = API.Commands.GetAllObjectTypeCommandForObject(objectId);
            string type = API.Object.GetObjectType(_objectId);

            Label TypeCommandLabel = new Label();
            TypeCommandLabel.Text = type + " Commands:" + (object_type_commands.Rows.Count > 0 ? "" : " none.");       
            TypeCommandLabel.AutoSize = true;
            TypeCommandLabel.Font = new System.Drawing.Font(TypeCommandLabel.Font.Name, TypeCommandLabel.Font.Size, FontStyle.Bold);
            TypeCommandLabel.Top = top;
            TypeCommandLabel.Left = 0;
            TypeCommandLabel.Height = 23;
            pnlSettings.Controls.Add(TypeCommandLabel);
            top += 25;

            foreach (DataRow dr in object_type_commands.Rows)
            {
                int left = 0;
                string buttonText = String.Empty;
                int paramType;
                int.TryParse(dr["param_type"].ToString(), out paramType);

                int CommandId;
                int.TryParse(dr["id"].ToString(), out CommandId);
                string CommandIdentifier = CommandId + "-" + cmdType.ObjectType.ToString() + "arg";

                #region Add Input Control Depending on type

                //ONLY NEEDED FOR LIST TYPES
                List<string> options = API.Commands.GetObjectTypeCommandOptions(CommandId);
                string value = API.Object.Value.Get(objectId, dr["txt_custom_data1"].ToString());

                left = GlobalMethods.DrawDynamicUserInputBoxes(pnlSettings, (ParamType)paramType, top, left, CommandIdentifier, dr["txt_command_friendly_name"].ToString(), options, value, "");

                #endregion

                #region Add Button
                Button btn = new Button();
                btn.Name = dr["id"].ToString();
                btn.Text = dr["txt_command_friendly_name"].ToString();
                btn.Click += btnClick;
                btn.Tag = new btnInfo((ParamType)paramType, cmdType.ObjectType);
                btn.Top = top;
                btn.Left = left;
                pnlSettings.Controls.Add(btn);

                using (Graphics cg = this.CreateGraphics())
                {
                    SizeF size = cg.MeasureString(btn.Text, btn.Font);
                    size.Width += 10; //add some padding
                    btn.Width = (int)size.Width;
                    left = (int)size.Width + left + 5;
                }
                #endregion

                #region Label

                Label CmdLabel = new Label();
                CmdLabel.Text = dr["txt_cmd_help"].ToString();
                CmdLabel.Top = top + 5;
                CmdLabel.Left = left;
                CmdLabel.Height = 23;
                pnlSettings.Controls.Add(CmdLabel);

                using (Graphics cg = this.CreateGraphics())
                {
                    SizeF size = cg.MeasureString(CmdLabel.Text, CmdLabel.Font);
                    size.Width += 6; //add some padding
                    CmdLabel.Width = (int)size.Width;
                    left = (int)size.Width + left + 5;
                }

                #endregion
                top += 35;
            }

            #endregion

            #region Builtin Commands

            DataTable builtin_commands = API.Commands.GetBuiltinCommands();

            //Are there any built commands to show?
            bool bltinCMDsToShow = false;
            foreach (DataRow dr in builtin_commands.Rows)
            {
                bool showOnObjList = true;
                bool.TryParse(dr["show_on_dynamic_obj_list"].ToString(), out showOnObjList);

                if (showOnObjList)
                {
                    bltinCMDsToShow =true;
                    break;
                }
            }

            Label BuiltinLabel = new Label();
            BuiltinLabel.Text = "Built-in Commands:" + (bltinCMDsToShow ? "" : " none.");            
            BuiltinLabel.AutoSize = true;
            BuiltinLabel.Font = new System.Drawing.Font(BuiltinLabel.Font.Name, BuiltinLabel.Font.Size, FontStyle.Bold);
            BuiltinLabel.Top = top;
            BuiltinLabel.Left = 0;
            BuiltinLabel.Height = 23;
            pnlSettings.Controls.Add(BuiltinLabel);
            top += 25;

            foreach (DataRow dr in builtin_commands.Rows)
            {
                bool showOnObjList = true;
                bool.TryParse(dr["show_on_dynamic_obj_list"].ToString(), out showOnObjList);

                if (!showOnObjList)
                    continue;

                int left = 0;
                string buttonText = String.Empty;
                int paramType;
                int.TryParse(dr["param_type"].ToString(), out paramType);

                int CommandId;
                int.TryParse(dr["id"].ToString(), out CommandId);
                string CommandIdentifier = CommandId + "-" + cmdType.Builtin.ToString() + "arg";

                #region Add Input Control Depending on type

                //ONLY NEEDED FOR LIST TYPES
                List<string> options = API.Commands.GetBuiltinCommandOptions(CommandId);
                string value = API.Object.Value.Get(objectId, dr["txt_custom_data1"].ToString());

                left = GlobalMethods.DrawDynamicUserInputBoxes(pnlSettings, (ParamType)paramType, top, left, CommandIdentifier, dr["txt_command_friendly_name"].ToString(), options, value, "");

                #endregion

                #region Add Button
                Button btn = new Button();
                btn.Name = dr["id"].ToString();
                btn.Text = dr["txt_command_friendly_name"].ToString();
                btn.Click += btnClick;
                btn.Tag = new btnInfo((ParamType)paramType, cmdType.Builtin);
                btn.Top = top;
                btn.Left = left;
                pnlSettings.Controls.Add(btn);

                using (Graphics cg = this.CreateGraphics())
                {
                    SizeF size = cg.MeasureString(btn.Text, btn.Font);
                    size.Width += 10; //add some padding
                    btn.Width = (int)size.Width;
                    left = (int)size.Width + left + 5;
                }
                #endregion

                #region Label

                Label CmdLabel = new Label();
                CmdLabel.Text = dr["txt_cmd_help"].ToString();
                CmdLabel.Top = top + 5;
                CmdLabel.Left = left;
                CmdLabel.Height = 23;
                pnlSettings.Controls.Add(CmdLabel);

                using (Graphics cg = this.CreateGraphics())
                {
                    SizeF size = cg.MeasureString(CmdLabel.Text, CmdLabel.Font);
                    size.Width += 6; //add some padding
                    CmdLabel.Width = (int)size.Width;
                    left = (int)size.Width + left + 5;
                }

                #endregion
                top += 35;
            }

            #endregion

        }

        private void btnClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int commandId;
            int.TryParse(btn.Name, out commandId);
            string arg = String.Empty;
            string CommandIdentifier = commandId + "-" + ((btnInfo)btn.Tag).zCommandType.ToString() + "arg";
            
            switch (((btnInfo)btn.Tag).Param)
            {
                case ParamType.NONE:
                    break;
                case ParamType.DECIMAL:
                case ParamType.INTEGER:
                case ParamType.BYTE:
                    //NumericUpDown have built in self validation
                    if (pnlSettings.Controls.ContainsKey(CommandIdentifier))
                        arg = ((NumericUpDown)pnlSettings.Controls[CommandIdentifier]).Value.ToString();
                    break;
                case ParamType.BOOL:
                    if (pnlSettings.Controls.ContainsKey(CommandIdentifier))
                        arg = ((CheckBox)pnlSettings.Controls[CommandIdentifier]).Checked.ToString();
                    break;
                case ParamType.STRING:
                    if (pnlSettings.Controls.ContainsKey(CommandIdentifier))
                        arg = ((TextBox)pnlSettings.Controls[CommandIdentifier]).Text;
                    break;
                case ParamType.LIST:
                    if (pnlSettings.Controls.ContainsKey(CommandIdentifier))
                        arg = ((ComboBox)pnlSettings.Controls[CommandIdentifier]).Text;
                    break;
            }
            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = ((btnInfo)btn.Tag).zCommandType, CommandId = commandId, ObjectId = _objectId, Argument = arg });           
        }
    }
    
}
