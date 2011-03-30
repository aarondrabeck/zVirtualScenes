namespace zVirtualScenesApplication
{
    partial class formAddEditActionMultiLevelSwitch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formAddEditActionMultiLevelSwitch));
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxAction = new System.Windows.Forms.GroupBox();
            this.checkBoxSkipLight = new System.Windows.Forms.CheckBox();
            this.checkBoxSkipDark = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label_DeviceName = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtbox_level = new System.Windows.Forms.TextBox();
            this.btn_RunCommand = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.groupBoxAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.groupBoxAction.Controls.Add(this.pictureBox1);
            this.groupBoxAction.Controls.Add(this.label_DeviceName);
            this.groupBoxAction.Controls.Add(this.label5);
            this.groupBoxAction.Controls.Add(this.label4);
            this.groupBoxAction.Controls.Add(this.txtbox_level);
            this.groupBoxAction.Controls.Add(this.btn_RunCommand);
            this.groupBoxAction.Location = new System.Drawing.Point(4, 5);
            this.groupBoxAction.Name = "groupBoxAction";
            this.groupBoxAction.Size = new System.Drawing.Size(422, 119);
            this.groupBoxAction.TabIndex = 36;
            this.groupBoxAction.TabStop = false;
            this.groupBoxAction.Text = "Create or Edit Action";
            // 
            // checkBoxSkipLight
            // 
            this.checkBoxSkipLight.AutoSize = true;
            this.checkBoxSkipLight.Location = new System.Drawing.Point(276, 38);
            this.checkBoxSkipLight.Name = "checkBoxSkipLight";
            this.checkBoxSkipLight.Size = new System.Drawing.Size(138, 17);
            this.checkBoxSkipLight.TabIndex = 50;
            this.checkBoxSkipLight.TabStop = false;
            this.checkBoxSkipLight.Text = "Skip when light outside.";
            this.checkBoxSkipLight.UseVisualStyleBackColor = true;
            // 
            // checkBoxSkipDark
            // 
            this.checkBoxSkipDark.AutoSize = true;
            this.checkBoxSkipDark.Location = new System.Drawing.Point(276, 15);
            this.checkBoxSkipDark.Name = "checkBoxSkipDark";
            this.checkBoxSkipDark.Size = new System.Drawing.Size(140, 17);
            this.checkBoxSkipDark.TabIndex = 49;
            this.checkBoxSkipDark.TabStop = false;
            this.checkBoxSkipDark.Text = "Skip when dark outside.";
            this.checkBoxSkipDark.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::zVirtualScenesApplication.Properties.Resources.dial_32;
            this.pictureBox1.InitialImage = global::zVirtualScenesApplication.Properties.Resources.switch_32;
            this.pictureBox1.Location = new System.Drawing.Point(8, 17);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(31, 36);
            this.pictureBox1.TabIndex = 48;
            this.pictureBox1.TabStop = false;
            // 
            // label_DeviceName
            // 
            this.label_DeviceName.AutoSize = true;
            this.label_DeviceName.Location = new System.Drawing.Point(45, 19);
            this.label_DeviceName.Name = "label_DeviceName";
            this.label_DeviceName.Size = new System.Drawing.Size(41, 13);
            this.label_DeviceName.TabIndex = 47;
            this.label_DeviceName.Text = "Device";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(149, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 46;
            this.label5.Text = "Dim = 0 to 99";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(53, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 43;
            this.label4.Text = "Level";
            // 
            // txtbox_level
            // 
            this.txtbox_level.Location = new System.Drawing.Point(92, 44);
            this.txtbox_level.Name = "txtbox_level";
            this.txtbox_level.Size = new System.Drawing.Size(51, 20);
            this.txtbox_level.TabIndex = 1;
            this.txtbox_level.Text = "0";
            this.txtbox_level.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtbox_level_KeyDown);
            // 
            // btn_RunCommand
            // 
            this.btn_RunCommand.Location = new System.Drawing.Point(313, 90);
            this.btn_RunCommand.Name = "btn_RunCommand";
            this.btn_RunCommand.Size = new System.Drawing.Size(103, 20);
            this.btn_RunCommand.TabIndex = 2;
            this.btn_RunCommand.Text = "Run Action Now";
            this.btn_RunCommand.UseVisualStyleBackColor = true;
            this.btn_RunCommand.Click += new System.EventHandler(this.btn_RunCommand_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(4, 146);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(423, 20);
            this.btn_Save.TabIndex = 3;
            this.btn_Save.Text = "Save Action";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Location = new System.Drawing.Point(9, 129);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(35, 13);
            this.lbl_Status.TabIndex = 39;
            this.lbl_Status.Text = "status";
            // 
            // formAddEditActionMultiLevelSwitch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 171);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.groupBoxAction);
            this.Controls.Add(this.btn_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formAddEditActionMultiLevelSwitch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Multilevel Switch Action";
            this.Load += new System.EventHandler(this.formAddEditActionMultiLevelSwitch_Load);
            this.groupBoxAction.ResumeLayout(false);
            this.groupBoxAction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.GroupBox groupBoxAction;
        private System.Windows.Forms.Button btn_RunCommand;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtbox_level;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.Label label_DeviceName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBoxSkipLight;
        private System.Windows.Forms.CheckBox checkBoxSkipDark;
    }
}