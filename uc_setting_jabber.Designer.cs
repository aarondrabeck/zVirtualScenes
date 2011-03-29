namespace zVirtualScenesApplication
{
    partial class uc_setting_jabber
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
            this.label22 = new System.Windows.Forms.Label();
            this.checkBoxJabberEnabled = new System.Windows.Forms.CheckBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.checkBox_HideJabberPassword = new System.Windows.Forms.CheckBox();
            this.textBoxJabberPassword = new System.Windows.Forms.TextBox();
            this.textBoxJabberUserTo = new System.Windows.Forms.TextBox();
            this.textBoxJabberUser = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxJabberVerbose = new System.Windows.Forms.CheckBox();
            this.textBoxJabberServer = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label22.Location = new System.Drawing.Point(406, 115);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(99, 13);
            this.label22.TabIndex = 35;
            this.label22.Text = "example: gmail.com\r\n";
            // 
            // checkBoxJabberEnabled
            // 
            this.checkBoxJabberEnabled.AutoSize = true;
            this.checkBoxJabberEnabled.Location = new System.Drawing.Point(12, 3);
            this.checkBoxJabberEnabled.Name = "checkBoxJabberEnabled";
            this.checkBoxJabberEnabled.Size = new System.Drawing.Size(128, 17);
            this.checkBoxJabberEnabled.TabIndex = 24;
            this.checkBoxJabberEnabled.Text = "Enable Jabber/GTalk";
            this.checkBoxJabberEnabled.UseVisualStyleBackColor = true;
            this.checkBoxJabberEnabled.CheckedChanged += new System.EventHandler(this.checkBoxJabberEnabled_CheckedChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label21.Location = new System.Drawing.Point(260, 168);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(316, 13);
            this.label21.TabIndex = 34;
            this.label21.Text = "(comma seperated) example: user1@gmail.com,user2@gmail.com";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(73, 87);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(91, 13);
            this.label11.TabIndex = 28;
            this.label11.Text = "Jabber Password:";
            // 
            // checkBox_HideJabberPassword
            // 
            this.checkBox_HideJabberPassword.AutoSize = true;
            this.checkBox_HideJabberPassword.Checked = true;
            this.checkBox_HideJabberPassword.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_HideJabberPassword.Location = new System.Drawing.Point(409, 87);
            this.checkBox_HideJabberPassword.Name = "checkBox_HideJabberPassword";
            this.checkBox_HideJabberPassword.Size = new System.Drawing.Size(97, 17);
            this.checkBox_HideJabberPassword.TabIndex = 27;
            this.checkBox_HideJabberPassword.Text = "Hide Password";
            this.checkBox_HideJabberPassword.UseVisualStyleBackColor = true;
            this.checkBox_HideJabberPassword.CheckedChanged += new System.EventHandler(this.checkBox_HideJabberPassword_CheckedChanged);
            // 
            // textBoxJabberPassword
            // 
            this.textBoxJabberPassword.Location = new System.Drawing.Point(167, 82);
            this.textBoxJabberPassword.Name = "textBoxJabberPassword";
            this.textBoxJabberPassword.Size = new System.Drawing.Size(236, 20);
            this.textBoxJabberPassword.TabIndex = 29;
            this.textBoxJabberPassword.UseSystemPasswordChar = true;
            // 
            // textBoxJabberUserTo
            // 
            this.textBoxJabberUserTo.Location = new System.Drawing.Point(167, 145);
            this.textBoxJabberUserTo.Name = "textBoxJabberUserTo";
            this.textBoxJabberUserTo.Size = new System.Drawing.Size(409, 20);
            this.textBoxJabberUserTo.TabIndex = 33;
            // 
            // textBoxJabberUser
            // 
            this.textBoxJabberUser.Location = new System.Drawing.Point(167, 56);
            this.textBoxJabberUser.Name = "textBoxJabberUser";
            this.textBoxJabberUser.Size = new System.Drawing.Size(236, 20);
            this.textBoxJabberUser.TabIndex = 26;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(88, 113);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(76, 13);
            this.label15.TabIndex = 30;
            this.label15.Text = "Jabber Server:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(19, 139);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(142, 26);
            this.label16.TabIndex = 32;
            this.label16.Text = "The usernames of Jabber \r\nusers to send notifcations to:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(71, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Jabber Username:";
            // 
            // checkBoxJabberVerbose
            // 
            this.checkBoxJabberVerbose.AutoSize = true;
            this.checkBoxJabberVerbose.Location = new System.Drawing.Point(12, 26);
            this.checkBoxJabberVerbose.Name = "checkBoxJabberVerbose";
            this.checkBoxJabberVerbose.Size = new System.Drawing.Size(142, 17);
            this.checkBoxJabberVerbose.TabIndex = 23;
            this.checkBoxJabberVerbose.Text = "Enable Verbose Logging";
            this.checkBoxJabberVerbose.UseVisualStyleBackColor = true;
            this.checkBoxJabberVerbose.CheckedChanged += new System.EventHandler(this.checkBoxJabberVerbose_CheckedChanged);
            // 
            // textBoxJabberServer
            // 
            this.textBoxJabberServer.Location = new System.Drawing.Point(167, 108);
            this.textBoxJabberServer.Name = "textBoxJabberServer";
            this.textBoxJabberServer.Size = new System.Drawing.Size(236, 20);
            this.textBoxJabberServer.TabIndex = 31;
            // 
            // uc_setting_jabber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label22);
            this.Controls.Add(this.checkBoxJabberEnabled);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.checkBox_HideJabberPassword);
            this.Controls.Add(this.textBoxJabberPassword);
            this.Controls.Add(this.textBoxJabberUserTo);
            this.Controls.Add(this.textBoxJabberUser);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxJabberVerbose);
            this.Controls.Add(this.textBoxJabberServer);
            this.Name = "uc_setting_jabber";
            this.Size = new System.Drawing.Size(592, 195);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.CheckBox checkBoxJabberEnabled;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkBox_HideJabberPassword;
        private System.Windows.Forms.TextBox textBoxJabberPassword;
        private System.Windows.Forms.TextBox textBoxJabberUserTo;
        private System.Windows.Forms.TextBox textBoxJabberUser;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxJabberVerbose;
        private System.Windows.Forms.TextBox textBoxJabberServer;

    }
}
