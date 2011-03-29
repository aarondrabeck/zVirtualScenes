namespace zVirtualScenesApplication
{
    partial class uc_setting_xmlsocket
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
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxAllowAndroid = new System.Windows.Forms.CheckBox();
            this.textBoxAndroidPassword = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.checkBoxHideAndroidPassword = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableSocketInt = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label30 = new System.Windows.Forms.Label();
            this.checkBoxAllowiViewer = new System.Windows.Forms.CheckBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.checkBoxSocketVerbose = new System.Windows.Forms.CheckBox();
            this.label26 = new System.Windows.Forms.Label();
            this.textBoxSocketListenPort = new System.Windows.Forms.TextBox();
            this.textBoxSocketConnectionLimit = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBoxAllowAndroid);
            this.groupBox5.Controls.Add(this.textBoxAndroidPassword);
            this.groupBox5.Controls.Add(this.label28);
            this.groupBox5.Controls.Add(this.checkBoxHideAndroidPassword);
            this.groupBox5.Location = new System.Drawing.Point(7, 222);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(398, 99);
            this.groupBox5.TabIndex = 48;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Android Clients";
            // 
            // checkBoxAllowAndroid
            // 
            this.checkBoxAllowAndroid.AutoSize = true;
            this.checkBoxAllowAndroid.Location = new System.Drawing.Point(21, 19);
            this.checkBoxAllowAndroid.Name = "checkBoxAllowAndroid";
            this.checkBoxAllowAndroid.Size = new System.Drawing.Size(124, 17);
            this.checkBoxAllowAndroid.TabIndex = 37;
            this.checkBoxAllowAndroid.Text = "Allow Android Clients";
            this.checkBoxAllowAndroid.UseVisualStyleBackColor = true;
            this.checkBoxAllowAndroid.CheckedChanged += new System.EventHandler(this.checkBoxAllowAndroid_CheckedChanged);
            // 
            // textBoxAndroidPassword
            // 
            this.textBoxAndroidPassword.Location = new System.Drawing.Point(110, 57);
            this.textBoxAndroidPassword.Name = "textBoxAndroidPassword";
            this.textBoxAndroidPassword.Size = new System.Drawing.Size(176, 20);
            this.textBoxAndroidPassword.TabIndex = 27;
            this.textBoxAndroidPassword.UseSystemPasswordChar = true;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(18, 60);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(93, 13);
            this.label28.TabIndex = 25;
            this.label28.Text = "Password Protect:";
            // 
            // checkBoxHideAndroidPassword
            // 
            this.checkBoxHideAndroidPassword.AutoSize = true;
            this.checkBoxHideAndroidPassword.Checked = true;
            this.checkBoxHideAndroidPassword.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHideAndroidPassword.Location = new System.Drawing.Point(292, 60);
            this.checkBoxHideAndroidPassword.Name = "checkBoxHideAndroidPassword";
            this.checkBoxHideAndroidPassword.Size = new System.Drawing.Size(97, 17);
            this.checkBoxHideAndroidPassword.TabIndex = 33;
            this.checkBoxHideAndroidPassword.Text = "Hide Password";
            this.checkBoxHideAndroidPassword.UseVisualStyleBackColor = true;
            this.checkBoxHideAndroidPassword.CheckedChanged += new System.EventHandler(this.checkBoxHideAndroidPassword_CheckedChanged);
            // 
            // checkBoxEnableSocketInt
            // 
            this.checkBoxEnableSocketInt.AutoSize = true;
            this.checkBoxEnableSocketInt.Location = new System.Drawing.Point(7, 6);
            this.checkBoxEnableSocketInt.Name = "checkBoxEnableSocketInt";
            this.checkBoxEnableSocketInt.Size = new System.Drawing.Size(166, 17);
            this.checkBoxEnableSocketInt.TabIndex = 39;
            this.checkBoxEnableSocketInt.Text = "Enable XML Socket Interface";
            this.checkBoxEnableSocketInt.UseVisualStyleBackColor = true;
            this.checkBoxEnableSocketInt.CheckedChanged += new System.EventHandler(this.checkBoxEnableSocketInt_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label30);
            this.groupBox3.Controls.Add(this.checkBoxAllowiViewer);
            this.groupBox3.Location = new System.Drawing.Point(7, 117);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(315, 99);
            this.groupBox3.TabIndex = 47;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "iViewer Clients";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label30.Location = new System.Drawing.Point(39, 39);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(273, 52);
            this.label30.TabIndex = 39;
            this.label30.Text = "Not recommended due to lack of authentication.\r\nEnabling this on a port forwarded" +
    " outside your \r\nnetwork allows unauthorized users the ability to connect \r\nwitho" +
    "ut entering a password!\r\n";
            // 
            // checkBoxAllowiViewer
            // 
            this.checkBoxAllowiViewer.AutoSize = true;
            this.checkBoxAllowiViewer.Location = new System.Drawing.Point(21, 19);
            this.checkBoxAllowiViewer.Name = "checkBoxAllowiViewer";
            this.checkBoxAllowiViewer.Size = new System.Drawing.Size(122, 17);
            this.checkBoxAllowiViewer.TabIndex = 37;
            this.checkBoxAllowiViewer.Text = "Allow iViewer Clients";
            this.checkBoxAllowiViewer.UseVisualStyleBackColor = true;
            this.checkBoxAllowiViewer.CheckedChanged += new System.EventHandler(this.checkBoxAllowiViewer_CheckedChanged);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(74, 63);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(75, 13);
            this.label29.TabIndex = 41;
            this.label29.Text = "Listen on Port:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label25.Location = new System.Drawing.Point(227, 98);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(259, 13);
            this.label25.TabIndex = 46;
            this.label25.Text = "Limits max. number of active connections. Default: 50";
            // 
            // checkBoxSocketVerbose
            // 
            this.checkBoxSocketVerbose.AutoSize = true;
            this.checkBoxSocketVerbose.Location = new System.Drawing.Point(7, 29);
            this.checkBoxSocketVerbose.Name = "checkBoxSocketVerbose";
            this.checkBoxSocketVerbose.Size = new System.Drawing.Size(142, 17);
            this.checkBoxSocketVerbose.TabIndex = 40;
            this.checkBoxSocketVerbose.Text = "Enable Verbose Logging";
            this.checkBoxSocketVerbose.UseVisualStyleBackColor = true;
            this.checkBoxSocketVerbose.CheckedChanged += new System.EventHandler(this.checkBoxSocketVerbose_CheckedChanged);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label26.Location = new System.Drawing.Point(227, 67);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(413, 13);
            this.label26.TabIndex = 45;
            this.label26.Text = "You might want to forward this port outside your network so you can connect remot" +
    "ely.";
            // 
            // textBoxSocketListenPort
            // 
            this.textBoxSocketListenPort.Location = new System.Drawing.Point(155, 60);
            this.textBoxSocketListenPort.Name = "textBoxSocketListenPort";
            this.textBoxSocketListenPort.Size = new System.Drawing.Size(66, 20);
            this.textBoxSocketListenPort.TabIndex = 42;
            // 
            // textBoxSocketConnectionLimit
            // 
            this.textBoxSocketConnectionLimit.Location = new System.Drawing.Point(156, 91);
            this.textBoxSocketConnectionLimit.Name = "textBoxSocketConnectionLimit";
            this.textBoxSocketConnectionLimit.Size = new System.Drawing.Size(65, 20);
            this.textBoxSocketConnectionLimit.TabIndex = 44;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(62, 95);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(88, 13);
            this.label27.TabIndex = 43;
            this.label27.Text = "Connection Limit:";
            // 
            // uc_setting_xmlsocket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.checkBoxEnableSocketInt);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.checkBoxSocketVerbose);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.textBoxSocketListenPort);
            this.Controls.Add(this.textBoxSocketConnectionLimit);
            this.Controls.Add(this.label27);
            this.Name = "uc_setting_xmlsocket";
            this.Size = new System.Drawing.Size(647, 335);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox checkBoxAllowAndroid;
        private System.Windows.Forms.TextBox textBoxAndroidPassword;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.CheckBox checkBoxHideAndroidPassword;
        private System.Windows.Forms.CheckBox checkBoxEnableSocketInt;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.CheckBox checkBoxAllowiViewer;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.CheckBox checkBoxSocketVerbose;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox textBoxSocketListenPort;
        private System.Windows.Forms.TextBox textBoxSocketConnectionLimit;
        private System.Windows.Forms.Label label27;
    }
}
