namespace zVirtualScenesApplication
{
    partial class formEditUserSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formEditUserSettings));
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.labelSaveStatus = new System.Windows.Forms.Label();
            this.listBoxCategory = new System.Windows.Forms.ListBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.uc_setting_xmlsocket1 = new zVirtualScenesApplication.uc_setting_xmlsocket();
            this.uc_setting_http1 = new zVirtualScenesApplication.uc_setting_http();
            this.uc_setting_lightswitch1 = new zVirtualScenesApplication.uc_setting_lightswitch();
            this.uc_setting_jabber1 = new zVirtualScenesApplication.uc_setting_jabber();
            this.uc_setting_general = new zVirtualScenesApplication.uc_setting_general();
            this.SuspendLayout();
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // labelSaveStatus
            // 
            this.labelSaveStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSaveStatus.AutoSize = true;
            this.labelSaveStatus.Location = new System.Drawing.Point(21, 344);
            this.labelSaveStatus.Name = "labelSaveStatus";
            this.labelSaveStatus.Size = new System.Drawing.Size(10, 13);
            this.labelSaveStatus.TabIndex = 23;
            this.labelSaveStatus.Text = "-";
            // 
            // listBoxCategory
            // 
            this.listBoxCategory.FormattingEnabled = true;
            this.listBoxCategory.Items.AddRange(new object[] {
            "General",
            "Jabber",
            "HTTP",
            "LightSwitch",
            "XML Socket"});
            this.listBoxCategory.Location = new System.Drawing.Point(5, 7);
            this.listBoxCategory.Name = "listBoxCategory";
            this.listBoxCategory.Size = new System.Drawing.Size(154, 264);
            this.listBoxCategory.TabIndex = 43;
            this.listBoxCategory.SelectedIndexChanged += new System.EventHandler(this.listBoxCategory_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(756, 375);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 49;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // uc_setting_xmlsocket1
            // 
            this.uc_setting_xmlsocket1.Location = new System.Drawing.Point(166, 7);
            this.uc_setting_xmlsocket1.Name = "uc_setting_xmlsocket1";
            this.uc_setting_xmlsocket1.Size = new System.Drawing.Size(649, 377);
            this.uc_setting_xmlsocket1.TabIndex = 48;
            this.uc_setting_xmlsocket1.Visible = false;
            // 
            // uc_setting_http1
            // 
            this.uc_setting_http1.Location = new System.Drawing.Point(166, 7);
            this.uc_setting_http1.Name = "uc_setting_http1";
            this.uc_setting_http1.Size = new System.Drawing.Size(766, 116);
            this.uc_setting_http1.TabIndex = 47;
            this.uc_setting_http1.Visible = false;
            // 
            // uc_setting_lightswitch1
            // 
            this.uc_setting_lightswitch1.Location = new System.Drawing.Point(166, 7);
            this.uc_setting_lightswitch1.Name = "uc_setting_lightswitch1";
            this.uc_setting_lightswitch1.Size = new System.Drawing.Size(665, 187);
            this.uc_setting_lightswitch1.TabIndex = 46;
            this.uc_setting_lightswitch1.Visible = false;
            // 
            // uc_setting_jabber1
            // 
            this.uc_setting_jabber1.Location = new System.Drawing.Point(166, 12);
            this.uc_setting_jabber1.Name = "uc_setting_jabber1";
            this.uc_setting_jabber1.Size = new System.Drawing.Size(592, 195);
            this.uc_setting_jabber1.TabIndex = 45;
            this.uc_setting_jabber1.Visible = false;
            // 
            // uc_setting_general
            // 
            this.uc_setting_general.Location = new System.Drawing.Point(166, 7);
            this.uc_setting_general.Name = "uc_setting_general";
            this.uc_setting_general.Size = new System.Drawing.Size(548, 377);
            this.uc_setting_general.TabIndex = 44;
            this.uc_setting_general.Visible = false;
            // 
            // formEditUserSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 406);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.uc_setting_xmlsocket1);
            this.Controls.Add(this.uc_setting_http1);
            this.Controls.Add(this.uc_setting_lightswitch1);
            this.Controls.Add(this.uc_setting_jabber1);
            this.Controls.Add(this.uc_setting_general);
            this.Controls.Add(this.listBoxCategory);
            this.Controls.Add(this.labelSaveStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formEditUserSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.Label labelSaveStatus;
        private System.Windows.Forms.ListBox listBoxCategory;
        private uc_setting_general uc_setting_general;
        private uc_setting_jabber uc_setting_jabber1;
        private uc_setting_lightswitch uc_setting_lightswitch1;
        private uc_setting_http uc_setting_http1;
        private uc_setting_xmlsocket uc_setting_xmlsocket1;
        private System.Windows.Forms.Button buttonOK;
    }
}