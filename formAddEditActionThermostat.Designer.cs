namespace zVirtualScenesApplication
{
    partial class formAddEditActionThermostat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formAddEditActionThermostat));
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxAction = new System.Windows.Forms.GroupBox();
            this.checkBoxeditHP = new System.Windows.Forms.CheckBox();
            this.checkBoxeditCP = new System.Windows.Forms.CheckBox();
            this.textBoxCoolPoint = new System.Windows.Forms.TextBox();
            this.btn_RunCommand = new System.Windows.Forms.Button();
            this.comboBoxEnergyMode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label_DeviceName = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.comboBoxHeatCoolMode = new System.Windows.Forms.ComboBox();
            this.comboBoxFanMode = new System.Windows.Forms.ComboBox();
            this.txtbx_HeatPoint = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.groupBoxAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // groupBoxAction
            // 
            this.groupBoxAction.Controls.Add(this.checkBoxeditHP);
            this.groupBoxAction.Controls.Add(this.checkBoxeditCP);
            this.groupBoxAction.Controls.Add(this.textBoxCoolPoint);
            this.groupBoxAction.Controls.Add(this.btn_RunCommand);
            this.groupBoxAction.Controls.Add(this.comboBoxEnergyMode);
            this.groupBoxAction.Controls.Add(this.label2);
            this.groupBoxAction.Controls.Add(this.label_DeviceName);
            this.groupBoxAction.Controls.Add(this.label7);
            this.groupBoxAction.Controls.Add(this.pictureBox2);
            this.groupBoxAction.Controls.Add(this.comboBoxHeatCoolMode);
            this.groupBoxAction.Controls.Add(this.comboBoxFanMode);
            this.groupBoxAction.Controls.Add(this.txtbx_HeatPoint);
            this.groupBoxAction.Controls.Add(this.label6);
            this.groupBoxAction.Location = new System.Drawing.Point(5, 6);
            this.groupBoxAction.Name = "groupBoxAction";
            this.groupBoxAction.Size = new System.Drawing.Size(515, 194);
            this.groupBoxAction.TabIndex = 36;
            this.groupBoxAction.TabStop = false;
            this.groupBoxAction.Text = "Create or Edit Action";
            // 
            // checkBoxeditHP
            // 
            this.checkBoxeditHP.AutoSize = true;
            this.checkBoxeditHP.Location = new System.Drawing.Point(134, 163);
            this.checkBoxeditHP.Name = "checkBoxeditHP";
            this.checkBoxeditHP.Size = new System.Drawing.Size(97, 17);
            this.checkBoxeditHP.TabIndex = 22;
            this.checkBoxeditHP.Text = "Edit Heat Point";
            this.checkBoxeditHP.UseVisualStyleBackColor = true;
            this.checkBoxeditHP.CheckedChanged += new System.EventHandler(this.checkBoxeditHP_CheckedChanged);
            // 
            // checkBoxeditCP
            // 
            this.checkBoxeditCP.AutoSize = true;
            this.checkBoxeditCP.Location = new System.Drawing.Point(250, 162);
            this.checkBoxeditCP.Name = "checkBoxeditCP";
            this.checkBoxeditCP.Size = new System.Drawing.Size(95, 17);
            this.checkBoxeditCP.TabIndex = 21;
            this.checkBoxeditCP.Text = "Edit Cool Point";
            this.checkBoxeditCP.UseVisualStyleBackColor = true;
            this.checkBoxeditCP.CheckedChanged += new System.EventHandler(this.checkBoxeditCP_CheckedChanged);
            // 
            // textBoxCoolPoint
            // 
            this.textBoxCoolPoint.Enabled = false;
            this.textBoxCoolPoint.Location = new System.Drawing.Point(268, 136);
            this.textBoxCoolPoint.Name = "textBoxCoolPoint";
            this.textBoxCoolPoint.Size = new System.Drawing.Size(51, 20);
            this.textBoxCoolPoint.TabIndex = 19;
            // 
            // btn_RunCommand
            // 
            this.btn_RunCommand.Location = new System.Drawing.Point(406, 12);
            this.btn_RunCommand.Name = "btn_RunCommand";
            this.btn_RunCommand.Size = new System.Drawing.Size(103, 20);
            this.btn_RunCommand.TabIndex = 38;
            this.btn_RunCommand.Text = "Test Action";
            this.btn_RunCommand.UseVisualStyleBackColor = true;
            this.btn_RunCommand.Click += new System.EventHandler(this.btn_RunCommand_Click);
            // 
            // comboBoxEnergyMode
            // 
            this.comboBoxEnergyMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEnergyMode.FormattingEnabled = true;
            this.comboBoxEnergyMode.Location = new System.Drawing.Point(174, 102);
            this.comboBoxEnergyMode.Name = "comboBoxEnergyMode";
            this.comboBoxEnergyMode.Size = new System.Drawing.Size(133, 21);
            this.comboBoxEnergyMode.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(78, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Heat/Cool Mode: ";
            // 
            // label_DeviceName
            // 
            this.label_DeviceName.AutoSize = true;
            this.label_DeviceName.Location = new System.Drawing.Point(66, 19);
            this.label_DeviceName.Name = "label_DeviceName";
            this.label_DeviceName.Size = new System.Drawing.Size(78, 13);
            this.label_DeviceName.TabIndex = 40;
            this.label_DeviceName.Text = "Device Name: ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(96, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Energy Mode:";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::zVirtualScenesApplication.Properties.Resources.temperature48;
            this.pictureBox2.Location = new System.Drawing.Point(7, 19);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(53, 50);
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            // 
            // comboBoxHeatCoolMode
            // 
            this.comboBoxHeatCoolMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHeatCoolMode.FormattingEnabled = true;
            this.comboBoxHeatCoolMode.Items.AddRange(new object[] {
            "",
            "Off",
            "Auto",
            "Heat",
            "Cool"});
            this.comboBoxHeatCoolMode.Location = new System.Drawing.Point(175, 48);
            this.comboBoxHeatCoolMode.Name = "comboBoxHeatCoolMode";
            this.comboBoxHeatCoolMode.Size = new System.Drawing.Size(132, 21);
            this.comboBoxHeatCoolMode.TabIndex = 11;
            // 
            // comboBoxFanMode
            // 
            this.comboBoxFanMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFanMode.FormattingEnabled = true;
            this.comboBoxFanMode.Items.AddRange(new object[] {
            "",
            "AutoLow",
            "OnLow"});
            this.comboBoxFanMode.Location = new System.Drawing.Point(175, 75);
            this.comboBoxFanMode.Name = "comboBoxFanMode";
            this.comboBoxFanMode.Size = new System.Drawing.Size(132, 21);
            this.comboBoxFanMode.TabIndex = 14;
            // 
            // txtbx_HeatPoint
            // 
            this.txtbx_HeatPoint.Enabled = false;
            this.txtbx_HeatPoint.Location = new System.Drawing.Point(155, 136);
            this.txtbx_HeatPoint.Name = "txtbx_HeatPoint";
            this.txtbx_HeatPoint.Size = new System.Drawing.Size(51, 20);
            this.txtbx_HeatPoint.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(108, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Fan Mode: ";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(5, 220);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(515, 20);
            this.btn_Save.TabIndex = 37;
            this.btn_Save.Text = "Save Action";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Location = new System.Drawing.Point(8, 205);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(35, 13);
            this.lbl_Status.TabIndex = 39;
            this.lbl_Status.Text = "status";
            // 
            // formAddEditActionThermostat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 245);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.groupBoxAction);
            this.Controls.Add(this.btn_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formAddEditActionThermostat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thermostat Properties";
            this.groupBoxAction.ResumeLayout(false);
            this.groupBoxAction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.GroupBox groupBoxAction;
        private System.Windows.Forms.Button btn_RunCommand;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Label label_DeviceName;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.CheckBox checkBoxeditHP;
        private System.Windows.Forms.CheckBox checkBoxeditCP;
        private System.Windows.Forms.TextBox textBoxCoolPoint;
        private System.Windows.Forms.ComboBox comboBoxEnergyMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxHeatCoolMode;
        private System.Windows.Forms.ComboBox comboBoxFanMode;
        private System.Windows.Forms.TextBox txtbx_HeatPoint;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}