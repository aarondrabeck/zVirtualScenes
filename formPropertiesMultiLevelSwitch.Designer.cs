namespace zVirtualScenesApplication
{
    partial class formPropertiesMultiLevelSwitch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formPropertiesMultiLevelSwitch));
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.txtb_deviceName = new System.Windows.Forms.TextBox();
            this.checkBoxPerDEviceJabberEnable = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBoxDeviceOptions = new System.Windows.Forms.GroupBox();
            this.btn_SaveOptions = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBoxDeviceOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // txtb_deviceName
            // 
            this.txtb_deviceName.Location = new System.Drawing.Point(122, 22);
            this.txtb_deviceName.Name = "txtb_deviceName";
            this.txtb_deviceName.Size = new System.Drawing.Size(250, 20);
            this.txtb_deviceName.TabIndex = 39;
            // 
            // checkBoxPerDEviceJabberEnable
            // 
            this.checkBoxPerDEviceJabberEnable.AutoSize = true;
            this.checkBoxPerDEviceJabberEnable.Location = new System.Drawing.Point(9, 59);
            this.checkBoxPerDEviceJabberEnable.Name = "checkBoxPerDEviceJabberEnable";
            this.checkBoxPerDEviceJabberEnable.Size = new System.Drawing.Size(254, 17);
            this.checkBoxPerDEviceJabberEnable.TabIndex = 26;
            this.checkBoxPerDEviceJabberEnable.Text = "Enable Jabber/Gtalk Notifcations for this Device";
            this.checkBoxPerDEviceJabberEnable.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(47, 25);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 13);
            this.label12.TabIndex = 40;
            this.label12.Text = "Device Name: ";
            // 
            // groupBoxDeviceOptions
            // 
            this.groupBoxDeviceOptions.Controls.Add(this.btn_SaveOptions);
            this.groupBoxDeviceOptions.Controls.Add(this.pictureBox1);
            this.groupBoxDeviceOptions.Controls.Add(this.checkBoxPerDEviceJabberEnable);
            this.groupBoxDeviceOptions.Controls.Add(this.txtb_deviceName);
            this.groupBoxDeviceOptions.Controls.Add(this.label12);
            this.groupBoxDeviceOptions.Location = new System.Drawing.Point(4, 5);
            this.groupBoxDeviceOptions.Name = "groupBoxDeviceOptions";
            this.groupBoxDeviceOptions.Size = new System.Drawing.Size(515, 103);
            this.groupBoxDeviceOptions.TabIndex = 37;
            this.groupBoxDeviceOptions.TabStop = false;
            this.groupBoxDeviceOptions.Text = "Options";
            // 
            // btn_SaveOptions
            // 
            this.btn_SaveOptions.Location = new System.Drawing.Point(408, 75);
            this.btn_SaveOptions.Name = "btn_SaveOptions";
            this.btn_SaveOptions.Size = new System.Drawing.Size(101, 20);
            this.btn_SaveOptions.TabIndex = 39;
            this.btn_SaveOptions.Text = "Save and Close";
            this.btn_SaveOptions.UseVisualStyleBackColor = true;
            this.btn_SaveOptions.Click += new System.EventHandler(this.btn_SaveOptions_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::zVirtualScenesApplication.Properties.Resources.dial_32;
            this.pictureBox1.InitialImage = global::zVirtualScenesApplication.Properties.Resources.switch_icon32;
            this.pictureBox1.Location = new System.Drawing.Point(10, 17);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(31, 36);
            this.pictureBox1.TabIndex = 45;
            this.pictureBox1.TabStop = false;
            // 
            // formPropertiesMultiLevelSwitch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 113);
            this.Controls.Add(this.groupBoxDeviceOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formPropertiesMultiLevelSwitch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Multilevel Switch Properties";
            this.groupBoxDeviceOptions.ResumeLayout(false);
            this.groupBoxDeviceOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.TextBox txtb_deviceName;
        private System.Windows.Forms.CheckBox checkBoxPerDEviceJabberEnable;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBoxDeviceOptions;
        private System.Windows.Forms.Button btn_SaveOptions;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}