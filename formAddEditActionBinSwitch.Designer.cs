namespace zVirtualScenesApplication
{
    partial class formAddEditActionBinSwitch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formAddEditActionBinSwitch));
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxAction = new System.Windows.Forms.GroupBox();
            this.checkBoxSkipLight = new System.Windows.Forms.CheckBox();
            this.checkBoxSkipDark = new System.Windows.Forms.CheckBox();
            this.labelMomentaryMode = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label_DeviceName = new System.Windows.Forms.Label();
            this.comboBoxBinaryONOFF = new System.Windows.Forms.ComboBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.btn_RunCommand = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.groupBoxAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // groupBoxAction
            // 
            this.groupBoxAction.Controls.Add(this.checkBoxSkipLight);
            this.groupBoxAction.Controls.Add(this.checkBoxSkipDark);
            this.groupBoxAction.Controls.Add(this.labelMomentaryMode);
            this.groupBoxAction.Controls.Add(this.label1);
            this.groupBoxAction.Controls.Add(this.label_DeviceName);
            this.groupBoxAction.Controls.Add(this.comboBoxBinaryONOFF);
            this.groupBoxAction.Controls.Add(this.pictureBox3);
            this.groupBoxAction.Controls.Add(this.btn_RunCommand);
            this.groupBoxAction.Location = new System.Drawing.Point(4, 6);
            this.groupBoxAction.Name = "groupBoxAction";
            this.groupBoxAction.Size = new System.Drawing.Size(440, 140);
            this.groupBoxAction.TabIndex = 36;
            this.groupBoxAction.TabStop = false;
            this.groupBoxAction.Text = "Create or Edit Action";
            // 
            // checkBoxSkipLight
            // 
            this.checkBoxSkipLight.AutoSize = true;
            this.checkBoxSkipLight.Location = new System.Drawing.Point(294, 38);
            this.checkBoxSkipLight.Name = "checkBoxSkipLight";
            this.checkBoxSkipLight.Size = new System.Drawing.Size(138, 17);
            this.checkBoxSkipLight.TabIndex = 99;
            this.checkBoxSkipLight.TabStop = false;
            this.checkBoxSkipLight.Text = "Skip when light outside.";
            this.checkBoxSkipLight.UseVisualStyleBackColor = true;
            // 
            // checkBoxSkipDark
            // 
            this.checkBoxSkipDark.AutoSize = true;
            this.checkBoxSkipDark.Location = new System.Drawing.Point(294, 15);
            this.checkBoxSkipDark.Name = "checkBoxSkipDark";
            this.checkBoxSkipDark.Size = new System.Drawing.Size(140, 17);
            this.checkBoxSkipDark.TabIndex = 99;
            this.checkBoxSkipDark.TabStop = false;
            this.checkBoxSkipDark.Text = "Skip when dark outside.";
            this.checkBoxSkipDark.UseVisualStyleBackColor = true;
            // 
            // labelMomentaryMode
            // 
            this.labelMomentaryMode.AutoSize = true;
            this.labelMomentaryMode.Location = new System.Drawing.Point(5, 118);
            this.labelMomentaryMode.Name = "labelMomentaryMode";
            this.labelMomentaryMode.Size = new System.Drawing.Size(115, 13);
            this.labelMomentaryMode.TabIndex = 43;
            this.labelMomentaryMode.Text = "Momentary Mode: OFF";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 41;
            this.label1.Text = "Set State: ";
            // 
            // label_DeviceName
            // 
            this.label_DeviceName.AutoSize = true;
            this.label_DeviceName.Location = new System.Drawing.Point(49, 19);
            this.label_DeviceName.Name = "label_DeviceName";
            this.label_DeviceName.Size = new System.Drawing.Size(78, 13);
            this.label_DeviceName.TabIndex = 40;
            this.label_DeviceName.Text = "Device Name: ";
            // 
            // comboBoxBinaryONOFF
            // 
            this.comboBoxBinaryONOFF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBinaryONOFF.FormattingEnabled = true;
            this.comboBoxBinaryONOFF.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.comboBoxBinaryONOFF.Location = new System.Drawing.Point(130, 45);
            this.comboBoxBinaryONOFF.Name = "comboBoxBinaryONOFF";
            this.comboBoxBinaryONOFF.Size = new System.Drawing.Size(53, 21);
            this.comboBoxBinaryONOFF.TabIndex = 1;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::zVirtualScenesApplication.Properties.Resources.switch_32;
            this.pictureBox3.InitialImage = global::zVirtualScenesApplication.Properties.Resources.switch_32;
            this.pictureBox3.Location = new System.Drawing.Point(8, 19);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(35, 37);
            this.pictureBox3.TabIndex = 9;
            this.pictureBox3.TabStop = false;
            // 
            // btn_RunCommand
            // 
            this.btn_RunCommand.Location = new System.Drawing.Point(334, 114);
            this.btn_RunCommand.Name = "btn_RunCommand";
            this.btn_RunCommand.Size = new System.Drawing.Size(100, 20);
            this.btn_RunCommand.TabIndex = 2;
            this.btn_RunCommand.Text = "Run Action Now";
            this.btn_RunCommand.UseVisualStyleBackColor = true;
            this.btn_RunCommand.Click += new System.EventHandler(this.btn_RunCommand_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(4, 166);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(440, 20);
            this.btn_Save.TabIndex = 3;
            this.btn_Save.Text = "Save Action";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Location = new System.Drawing.Point(1, 149);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(35, 13);
            this.lbl_Status.TabIndex = 38;
            this.lbl_Status.Text = "status";
            // 
            // formAddEditActionBinSwitch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 191);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.groupBoxAction);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formAddEditActionBinSwitch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Binary Switch Properties";
            this.Load += new System.EventHandler(this.formAddEditActionBinSwitch_Load);
            this.groupBoxAction.ResumeLayout(false);
            this.groupBoxAction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.GroupBox groupBoxAction;
        private System.Windows.Forms.ComboBox comboBoxBinaryONOFF;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Button btn_RunCommand;
        private System.Windows.Forms.Label label_DeviceName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Label labelMomentaryMode;
        private System.Windows.Forms.CheckBox checkBoxSkipLight;
        private System.Windows.Forms.CheckBox checkBoxSkipDark;
    }
}