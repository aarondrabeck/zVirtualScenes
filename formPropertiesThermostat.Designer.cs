namespace zVirtualScenesApplication
{
    partial class formPropertiesThermostat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formPropertiesThermostat));
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.labelMoreInfo = new System.Windows.Forms.Label();
            this.txtb_deviceName = new System.Windows.Forms.TextBox();
            this.checkBoxPerDEviceJabberEnable = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBoxDeviceOptions = new System.Windows.Forms.GroupBox();
            this.checkBoxDisplayinLightSwitch = new System.Windows.Forms.CheckBox();
            this.txtb_GroupName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.textBoxMinAlert = new System.Windows.Forms.TextBox();
            this.comboBoxJabberNotifLevel = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.textBoxMaxAlert = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.btn_SaveOptions = new System.Windows.Forms.Button();
            this.checkBoxGrowlEnabled = new System.Windows.Forms.CheckBox();
            this.groupBoxDeviceOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // labelMoreInfo
            // 
            this.labelMoreInfo.AutoSize = true;
            this.labelMoreInfo.Location = new System.Drawing.Point(146, 149);
            this.labelMoreInfo.Name = "labelMoreInfo";
            this.labelMoreInfo.Size = new System.Drawing.Size(59, 13);
            this.labelMoreInfo.TabIndex = 47;
            this.labelMoreInfo.Text = "more info...";
            this.toolTipNotificationLevel.SetToolTip(this.labelMoreInfo, resources.GetString("labelMoreInfo.ToolTip"));
            // 
            // txtb_deviceName
            // 
            this.txtb_deviceName.Location = new System.Drawing.Point(87, 19);
            this.txtb_deviceName.Name = "txtb_deviceName";
            this.txtb_deviceName.Size = new System.Drawing.Size(250, 20);
            this.txtb_deviceName.TabIndex = 39;
            // 
            // checkBoxPerDEviceJabberEnable
            // 
            this.checkBoxPerDEviceJabberEnable.AutoSize = true;
            this.checkBoxPerDEviceJabberEnable.Location = new System.Drawing.Point(181, 79);
            this.checkBoxPerDEviceJabberEnable.Name = "checkBoxPerDEviceJabberEnable";
            this.checkBoxPerDEviceJabberEnable.Size = new System.Drawing.Size(254, 17);
            this.checkBoxPerDEviceJabberEnable.TabIndex = 26;
            this.checkBoxPerDEviceJabberEnable.Text = "Enable Jabber/Gtalk Notifcations for this Device";
            this.checkBoxPerDEviceJabberEnable.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 13);
            this.label12.TabIndex = 40;
            this.label12.Text = "Device Name: ";
            // 
            // groupBoxDeviceOptions
            // 
            this.groupBoxDeviceOptions.Controls.Add(this.checkBoxGrowlEnabled);
            this.groupBoxDeviceOptions.Controls.Add(this.checkBoxDisplayinLightSwitch);
            this.groupBoxDeviceOptions.Controls.Add(this.txtb_GroupName);
            this.groupBoxDeviceOptions.Controls.Add(this.label1);
            this.groupBoxDeviceOptions.Controls.Add(this.labelMoreInfo);
            this.groupBoxDeviceOptions.Controls.Add(this.label19);
            this.groupBoxDeviceOptions.Controls.Add(this.textBoxMinAlert);
            this.groupBoxDeviceOptions.Controls.Add(this.comboBoxJabberNotifLevel);
            this.groupBoxDeviceOptions.Controls.Add(this.label17);
            this.groupBoxDeviceOptions.Controls.Add(this.textBoxMaxAlert);
            this.groupBoxDeviceOptions.Controls.Add(this.label18);
            this.groupBoxDeviceOptions.Controls.Add(this.btn_SaveOptions);
            this.groupBoxDeviceOptions.Controls.Add(this.checkBoxPerDEviceJabberEnable);
            this.groupBoxDeviceOptions.Controls.Add(this.txtb_deviceName);
            this.groupBoxDeviceOptions.Controls.Add(this.label12);
            this.groupBoxDeviceOptions.Location = new System.Drawing.Point(5, 4);
            this.groupBoxDeviceOptions.Name = "groupBoxDeviceOptions";
            this.groupBoxDeviceOptions.Size = new System.Drawing.Size(515, 171);
            this.groupBoxDeviceOptions.TabIndex = 37;
            this.groupBoxDeviceOptions.TabStop = false;
            this.groupBoxDeviceOptions.Text = "Options";
            // 
            // checkBoxDisplayinLightSwitch
            // 
            this.checkBoxDisplayinLightSwitch.AutoSize = true;
            this.checkBoxDisplayinLightSwitch.Location = new System.Drawing.Point(181, 124);
            this.checkBoxDisplayinLightSwitch.Name = "checkBoxDisplayinLightSwitch";
            this.checkBoxDisplayinLightSwitch.Size = new System.Drawing.Size(129, 17);
            this.checkBoxDisplayinLightSwitch.TabIndex = 41;
            this.checkBoxDisplayinLightSwitch.Text = "Display in LightSwitch";
            this.checkBoxDisplayinLightSwitch.UseVisualStyleBackColor = true;
            // 
            // txtb_GroupName
            // 
            this.txtb_GroupName.Location = new System.Drawing.Point(87, 45);
            this.txtb_GroupName.Name = "txtb_GroupName";
            this.txtb_GroupName.Size = new System.Drawing.Size(250, 20);
            this.txtb_GroupName.TabIndex = 48;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 49;
            this.label1.Text = "Group Name: ";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 90);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(84, 13);
            this.label19.TabIndex = 46;
            this.label19.Text = "Min Alert Temp: ";
            // 
            // textBoxMinAlert
            // 
            this.textBoxMinAlert.Location = new System.Drawing.Point(87, 84);
            this.textBoxMinAlert.Name = "textBoxMinAlert";
            this.textBoxMinAlert.Size = new System.Drawing.Size(51, 20);
            this.textBoxMinAlert.TabIndex = 44;
            // 
            // comboBoxJabberNotifLevel
            // 
            this.comboBoxJabberNotifLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxJabberNotifLevel.FormattingEnabled = true;
            this.comboBoxJabberNotifLevel.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.comboBoxJabberNotifLevel.Location = new System.Drawing.Point(93, 141);
            this.comboBoxJabberNotifLevel.Name = "comboBoxJabberNotifLevel";
            this.comboBoxJabberNotifLevel.Size = new System.Drawing.Size(51, 21);
            this.comboBoxJabberNotifLevel.TabIndex = 42;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 146);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(90, 13);
            this.label17.TabIndex = 41;
            this.label17.Text = "Notication Level: ";
            // 
            // textBoxMaxAlert
            // 
            this.textBoxMaxAlert.Location = new System.Drawing.Point(87, 110);
            this.textBoxMaxAlert.Name = "textBoxMaxAlert";
            this.textBoxMaxAlert.Size = new System.Drawing.Size(51, 20);
            this.textBoxMaxAlert.TabIndex = 43;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 116);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(87, 13);
            this.label18.TabIndex = 45;
            this.label18.Text = "Max Alert Temp: ";
            // 
            // btn_SaveOptions
            // 
            this.btn_SaveOptions.Location = new System.Drawing.Point(410, 144);
            this.btn_SaveOptions.Name = "btn_SaveOptions";
            this.btn_SaveOptions.Size = new System.Drawing.Size(101, 20);
            this.btn_SaveOptions.TabIndex = 39;
            this.btn_SaveOptions.Text = "Save and Close";
            this.btn_SaveOptions.UseVisualStyleBackColor = true;
            this.btn_SaveOptions.Click += new System.EventHandler(this.btn_SaveOptions_Click);
            // 
            // checkBoxGrowlEnabled
            // 
            this.checkBoxGrowlEnabled.AutoSize = true;
            this.checkBoxGrowlEnabled.Location = new System.Drawing.Point(181, 101);
            this.checkBoxGrowlEnabled.Name = "checkBoxGrowlEnabled";
            this.checkBoxGrowlEnabled.Size = new System.Drawing.Size(219, 17);
            this.checkBoxGrowlEnabled.TabIndex = 50;
            this.checkBoxGrowlEnabled.Text = "Enable Growl Notifcations for this Device";
            this.checkBoxGrowlEnabled.UseVisualStyleBackColor = true;
            // 
            // formPropertiesThermostat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 183);
            this.Controls.Add(this.groupBoxDeviceOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formPropertiesThermostat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit this ZWave Thermostat Device";
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
        private System.Windows.Forms.Label labelMoreInfo;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBoxMinAlert;
        private System.Windows.Forms.ComboBox comboBoxJabberNotifLevel;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBoxMaxAlert;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtb_GroupName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxDisplayinLightSwitch;
        private System.Windows.Forms.CheckBox checkBoxGrowlEnabled;
    }
}