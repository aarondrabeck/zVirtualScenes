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
            this.checkBoxGrowlEnabled = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayinLightSwitch = new System.Windows.Forms.CheckBox();
            this.txtb_GroupName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_SaveOptions = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxRepolling = new System.Windows.Forms.TextBox();
            this.groupBoxDeviceOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // txtb_deviceName
            // 
            this.txtb_deviceName.Location = new System.Drawing.Point(86, 19);
            this.txtb_deviceName.Name = "txtb_deviceName";
            this.txtb_deviceName.Size = new System.Drawing.Size(250, 20);
            this.txtb_deviceName.TabIndex = 1;
            this.txtb_deviceName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtb_deviceName_KeyDown);
            // 
            // checkBoxPerDEviceJabberEnable
            // 
            this.checkBoxPerDEviceJabberEnable.AutoSize = true;
            this.checkBoxPerDEviceJabberEnable.Location = new System.Drawing.Point(10, 80);
            this.checkBoxPerDEviceJabberEnable.Name = "checkBoxPerDEviceJabberEnable";
            this.checkBoxPerDEviceJabberEnable.Size = new System.Drawing.Size(254, 17);
            this.checkBoxPerDEviceJabberEnable.TabIndex = 3;
            this.checkBoxPerDEviceJabberEnable.Text = "Enable Jabber/Gtalk Notifcations for this Device";
            this.checkBoxPerDEviceJabberEnable.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 13);
            this.label12.TabIndex = 40;
            this.label12.Text = "Device Name: ";
            // 
            // groupBoxDeviceOptions
            // 
            this.groupBoxDeviceOptions.Controls.Add(this.label3);
            this.groupBoxDeviceOptions.Controls.Add(this.checkBoxGrowlEnabled);
            this.groupBoxDeviceOptions.Controls.Add(this.textBoxRepolling);
            this.groupBoxDeviceOptions.Controls.Add(this.checkBoxDisplayinLightSwitch);
            this.groupBoxDeviceOptions.Controls.Add(this.txtb_GroupName);
            this.groupBoxDeviceOptions.Controls.Add(this.label1);
            this.groupBoxDeviceOptions.Controls.Add(this.btn_SaveOptions);
            this.groupBoxDeviceOptions.Controls.Add(this.checkBoxPerDEviceJabberEnable);
            this.groupBoxDeviceOptions.Controls.Add(this.txtb_deviceName);
            this.groupBoxDeviceOptions.Controls.Add(this.label12);
            this.groupBoxDeviceOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDeviceOptions.Location = new System.Drawing.Point(0, 0);
            this.groupBoxDeviceOptions.Name = "groupBoxDeviceOptions";
            this.groupBoxDeviceOptions.Size = new System.Drawing.Size(438, 178);
            this.groupBoxDeviceOptions.TabIndex = 37;
            this.groupBoxDeviceOptions.TabStop = false;
            this.groupBoxDeviceOptions.Text = "Options";
            // 
            // checkBoxGrowlEnabled
            // 
            this.checkBoxGrowlEnabled.AutoSize = true;
            this.checkBoxGrowlEnabled.Location = new System.Drawing.Point(10, 103);
            this.checkBoxGrowlEnabled.Name = "checkBoxGrowlEnabled";
            this.checkBoxGrowlEnabled.Size = new System.Drawing.Size(219, 17);
            this.checkBoxGrowlEnabled.TabIndex = 4;
            this.checkBoxGrowlEnabled.Text = "Enable Growl Notifcations for this Device";
            this.checkBoxGrowlEnabled.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisplayinLightSwitch
            // 
            this.checkBoxDisplayinLightSwitch.AutoSize = true;
            this.checkBoxDisplayinLightSwitch.Location = new System.Drawing.Point(10, 126);
            this.checkBoxDisplayinLightSwitch.Name = "checkBoxDisplayinLightSwitch";
            this.checkBoxDisplayinLightSwitch.Size = new System.Drawing.Size(129, 17);
            this.checkBoxDisplayinLightSwitch.TabIndex = 5;
            this.checkBoxDisplayinLightSwitch.Text = "Display in LightSwitch";
            this.checkBoxDisplayinLightSwitch.UseVisualStyleBackColor = true;
            // 
            // txtb_GroupName
            // 
            this.txtb_GroupName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtb_GroupName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtb_GroupName.Location = new System.Drawing.Point(86, 45);
            this.txtb_GroupName.Name = "txtb_GroupName";
            this.txtb_GroupName.Size = new System.Drawing.Size(250, 20);
            this.txtb_GroupName.TabIndex = 2;
            this.txtb_GroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtb_GroupName_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 47;
            this.label1.Text = "Group Name: ";
            // 
            // btn_SaveOptions
            // 
            this.btn_SaveOptions.Location = new System.Drawing.Point(372, 147);
            this.btn_SaveOptions.Name = "btn_SaveOptions";
            this.btn_SaveOptions.Size = new System.Drawing.Size(60, 25);
            this.btn_SaveOptions.TabIndex = 6;
            this.btn_SaveOptions.Text = "&Save";
            this.btn_SaveOptions.UseVisualStyleBackColor = true;
            this.btn_SaveOptions.Click += new System.EventHandler(this.btn_SaveOptions_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 13);
            this.label3.TabIndex = 104;
            this.label3.Text = "Repolling Interval (seconds):";
            // 
            // textBoxRepolling
            // 
            this.textBoxRepolling.Location = new System.Drawing.Point(158, 150);
            this.textBoxRepolling.Name = "textBoxRepolling";
            this.textBoxRepolling.Size = new System.Drawing.Size(71, 20);
            this.textBoxRepolling.TabIndex = 105;
            this.textBoxRepolling.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxRepolling_KeyPress);
            // 
            // formPropertiesMultiLevelSwitch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 178);
            this.Controls.Add(this.groupBoxDeviceOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formPropertiesMultiLevelSwitch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit this ZWave Switch Device";
            this.Load += new System.EventHandler(this.formPropertiesMultiLevelSwitch_Load);
            this.groupBoxDeviceOptions.ResumeLayout(false);
            this.groupBoxDeviceOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.TextBox txtb_deviceName;
        private System.Windows.Forms.CheckBox checkBoxPerDEviceJabberEnable;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBoxDeviceOptions;
        private System.Windows.Forms.Button btn_SaveOptions;
        private System.Windows.Forms.TextBox txtb_GroupName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxDisplayinLightSwitch;
        private System.Windows.Forms.CheckBox checkBoxGrowlEnabled;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxRepolling;
    }
}