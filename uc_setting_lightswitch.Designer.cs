namespace zVirtualScenesApplication
{
    partial class uc_setting_lightswitch
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBoxLSSortDevices = new System.Windows.Forms.CheckBox();
            this.checkBoxLSEnabled = new System.Windows.Forms.CheckBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox_HideLSPassword = new System.Windows.Forms.CheckBox();
            this.checkBoxLSDebugVerbose = new System.Windows.Forms.CheckBox();
            this.textBoxLSLimit = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxLSpwd = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // checkBoxLSSortDevices
            // 
            this.checkBoxLSSortDevices.AutoSize = true;
            this.checkBoxLSSortDevices.Location = new System.Drawing.Point(7, 157);
            this.checkBoxLSSortDevices.Name = "checkBoxLSSortDevices";
            this.checkBoxLSSortDevices.Size = new System.Drawing.Size(121, 17);
            this.checkBoxLSSortDevices.TabIndex = 37;
            this.checkBoxLSSortDevices.Text = "Sort Device List A-Z";
            this.checkBoxLSSortDevices.UseVisualStyleBackColor = true;
            this.checkBoxLSSortDevices.CheckedChanged += new System.EventHandler(this.checkBoxLSSortDevices_CheckedChanged);
            // 
            // checkBoxLSEnabled
            // 
            this.checkBoxLSEnabled.AutoSize = true;
            this.checkBoxLSEnabled.Location = new System.Drawing.Point(7, 6);
            this.checkBoxLSEnabled.Name = "checkBoxLSEnabled";
            this.checkBoxLSEnabled.Size = new System.Drawing.Size(151, 17);
            this.checkBoxLSEnabled.TabIndex = 26;
            this.checkBoxLSEnabled.Text = "Enable LightSwitch Server";
            this.checkBoxLSEnabled.UseVisualStyleBackColor = true;
            this.checkBoxLSEnabled.CheckedChanged += new System.EventHandler(this.checkBoxLSEnabled_CheckedChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label24.Location = new System.Drawing.Point(227, 98);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(259, 13);
            this.label24.TabIndex = 36;
            this.label24.Text = "Limits max. number of active connections. Default: 50";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(74, 63);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(75, 13);
            this.label13.TabIndex = 30;
            this.label13.Text = "Listen on Port:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label23.Location = new System.Drawing.Point(227, 67);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(413, 13);
            this.label23.TabIndex = 35;
            this.label23.Text = "You might want to forward this port outside your network so you can connect remot" +
    "ely.";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 125);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(127, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Authentication Password:";
            // 
            // checkBox_HideLSPassword
            // 
            this.checkBox_HideLSPassword.AutoSize = true;
            this.checkBox_HideLSPassword.Checked = true;
            this.checkBox_HideLSPassword.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_HideLSPassword.Location = new System.Drawing.Point(337, 126);
            this.checkBox_HideLSPassword.Name = "checkBox_HideLSPassword";
            this.checkBox_HideLSPassword.Size = new System.Drawing.Size(97, 17);
            this.checkBox_HideLSPassword.TabIndex = 34;
            this.checkBox_HideLSPassword.Text = "Hide Password";
            this.checkBox_HideLSPassword.UseVisualStyleBackColor = true;
            this.checkBox_HideLSPassword.CheckedChanged += new System.EventHandler(this.checkBox_HideLSPassword_CheckedChanged);
            // 
            // checkBoxLSDebugVerbose
            // 
            this.checkBoxLSDebugVerbose.AutoSize = true;
            this.checkBoxLSDebugVerbose.Location = new System.Drawing.Point(7, 29);
            this.checkBoxLSDebugVerbose.Name = "checkBoxLSDebugVerbose";
            this.checkBoxLSDebugVerbose.Size = new System.Drawing.Size(142, 17);
            this.checkBoxLSDebugVerbose.TabIndex = 28;
            this.checkBoxLSDebugVerbose.Text = "Enable Verbose Logging";
            this.checkBoxLSDebugVerbose.UseVisualStyleBackColor = true;
            this.checkBoxLSDebugVerbose.CheckedChanged += new System.EventHandler(this.checkBoxLSDebugVerbose_CheckedChanged);
            // 
            // textBoxLSLimit
            // 
            this.textBoxLSLimit.Location = new System.Drawing.Point(155, 92);
            this.textBoxLSLimit.Name = "textBoxLSLimit";
            this.textBoxLSLimit.Size = new System.Drawing.Size(65, 20);
            this.textBoxLSLimit.TabIndex = 33;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(62, 95);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 13);
            this.label14.TabIndex = 32;
            this.label14.Text = "Connection Limit:";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(155, 60);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(65, 20);
            this.textBoxPort.TabIndex = 38;
            // 
            // textBoxLSpwd
            // 
            this.textBoxLSpwd.Location = new System.Drawing.Point(155, 122);
            this.textBoxLSpwd.Name = "textBoxLSpwd";
            this.textBoxLSpwd.Size = new System.Drawing.Size(176, 20);
            this.textBoxLSpwd.TabIndex = 39;
            this.textBoxLSpwd.UseSystemPasswordChar = true;
            // 
            // uc_setting_lightswitch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxLSpwd);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.checkBoxLSSortDevices);
            this.Controls.Add(this.checkBoxLSEnabled);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.checkBox_HideLSPassword);
            this.Controls.Add(this.checkBoxLSDebugVerbose);
            this.Controls.Add(this.textBoxLSLimit);
            this.Controls.Add(this.label14);
            this.Name = "uc_setting_lightswitch";
            this.Size = new System.Drawing.Size(665, 187);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxLSSortDevices;
        private System.Windows.Forms.CheckBox checkBoxLSEnabled;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBox_HideLSPassword;
        private System.Windows.Forms.CheckBox checkBoxLSDebugVerbose;
        private System.Windows.Forms.TextBox textBoxLSLimit;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxLSpwd;
    }
}
