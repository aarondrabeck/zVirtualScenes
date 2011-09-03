namespace zVirtualScenesApplication.UserControls
{
    partial class uc_plugin_properties_form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_plugin_properties_form));
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.cbEnablePlugin = new System.Windows.Forms.CheckBox();
            this.labelPluginTitle = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.lblPluginDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pnlSettings
            // 
            this.pnlSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSettings.AutoScroll = true;
            this.pnlSettings.Location = new System.Drawing.Point(15, 96);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(638, 227);
            this.pnlSettings.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(579, 326);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbEnablePlugin
            // 
            this.cbEnablePlugin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbEnablePlugin.AutoSize = true;
            this.cbEnablePlugin.Location = new System.Drawing.Point(581, 6);
            this.cbEnablePlugin.Name = "cbEnablePlugin";
            this.cbEnablePlugin.Size = new System.Drawing.Size(65, 17);
            this.cbEnablePlugin.TabIndex = 2;
            this.cbEnablePlugin.Text = "Enabled";
            this.cbEnablePlugin.UseVisualStyleBackColor = true;
            // 
            // labelPluginTitle
            // 
            this.labelPluginTitle.AutoSize = true;
            this.labelPluginTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPluginTitle.Location = new System.Drawing.Point(9, 7);
            this.labelPluginTitle.Name = "labelPluginTitle";
            this.labelPluginTitle.Size = new System.Drawing.Size(53, 13);
            this.labelPluginTitle.TabIndex = 3;
            this.labelPluginTitle.Text = "Plugin 1";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(15, 83);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(631, 10);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.Location = new System.Drawing.Point(12, 326);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(561, 23);
            this.labelStatus.TabIndex = 26;
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPluginDescription
            // 
            this.lblPluginDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPluginDescription.AutoEllipsis = true;
            this.lblPluginDescription.Location = new System.Drawing.Point(15, 29);
            this.lblPluginDescription.Name = "lblPluginDescription";
            this.lblPluginDescription.Size = new System.Drawing.Size(636, 57);
            this.lblPluginDescription.TabIndex = 27;
            this.lblPluginDescription.Text = resources.GetString("lblPluginDescription.Text");
            // 
            // uc_plugin_properties_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblPluginDescription);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.cbEnablePlugin);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelPluginTitle);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.pnlSettings);
            this.Name = "uc_plugin_properties_form";
            this.Size = new System.Drawing.Size(657, 352);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox cbEnablePlugin;
        private System.Windows.Forms.Label labelPluginTitle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label lblPluginDescription;
    }
}
