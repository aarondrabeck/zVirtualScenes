namespace zVirtualScenesApplication
{
    partial class formDeviceProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formDeviceProperties));
            this.checkBoxPerDEviceJabberEnable = new System.Windows.Forms.CheckBox();
            this.txtb_deviceName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxMaxAlert = new System.Windows.Forms.TextBox();
            this.textBoxMinAlert = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.comboBoxJabberNotifLevel = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBoxDevice = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelMoreInfo = new System.Windows.Forms.Label();
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxDevice.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxPerDEviceJabberEnable
            // 
            this.checkBoxPerDEviceJabberEnable.AutoSize = true;
            this.checkBoxPerDEviceJabberEnable.Location = new System.Drawing.Point(6, 19);
            this.checkBoxPerDEviceJabberEnable.Name = "checkBoxPerDEviceJabberEnable";
            this.checkBoxPerDEviceJabberEnable.Size = new System.Drawing.Size(254, 17);
            this.checkBoxPerDEviceJabberEnable.TabIndex = 26;
            this.checkBoxPerDEviceJabberEnable.Text = "Enable Jabber/Gtalk Notifcations for this Device";
            this.checkBoxPerDEviceJabberEnable.UseVisualStyleBackColor = true;
            // 
            // txtb_deviceName
            // 
            this.txtb_deviceName.Location = new System.Drawing.Point(90, 19);
            this.txtb_deviceName.Name = "txtb_deviceName";
            this.txtb_deviceName.Size = new System.Drawing.Size(250, 20);
            this.txtb_deviceName.TabIndex = 24;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Device Name: ";
            // 
            // textBoxMaxAlert
            // 
            this.textBoxMaxAlert.Location = new System.Drawing.Point(90, 45);
            this.textBoxMaxAlert.Name = "textBoxMaxAlert";
            this.textBoxMaxAlert.Size = new System.Drawing.Size(51, 20);
            this.textBoxMaxAlert.TabIndex = 29;
            // 
            // textBoxMinAlert
            // 
            this.textBoxMinAlert.Location = new System.Drawing.Point(90, 19);
            this.textBoxMinAlert.Name = "textBoxMinAlert";
            this.textBoxMinAlert.Size = new System.Drawing.Size(51, 20);
            this.textBoxMinAlert.TabIndex = 30;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(9, 25);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(84, 13);
            this.label19.TabIndex = 32;
            this.label19.Text = "Min Alert Temp: ";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 51);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(87, 13);
            this.label18.TabIndex = 31;
            this.label18.Text = "Max Alert Temp: ";
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
            this.comboBoxJabberNotifLevel.Location = new System.Drawing.Point(90, 71);
            this.comboBoxJabberNotifLevel.Name = "comboBoxJabberNotifLevel";
            this.comboBoxJabberNotifLevel.Size = new System.Drawing.Size(51, 21);
            this.comboBoxJabberNotifLevel.TabIndex = 28;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 76);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(90, 13);
            this.label17.TabIndex = 27;
            this.label17.Text = "Notication Level: ";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(247, 232);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(93, 25);
            this.btn_Save.TabIndex = 34;
            this.btn_Save.Text = "Save Changes";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // groupBoxDevice
            // 
            this.groupBoxDevice.Controls.Add(this.txtb_deviceName);
            this.groupBoxDevice.Controls.Add(this.groupBox2);
            this.groupBoxDevice.Controls.Add(this.btn_Save);
            this.groupBoxDevice.Controls.Add(this.groupBox1);
            this.groupBoxDevice.Controls.Add(this.label12);
            this.groupBoxDevice.Location = new System.Drawing.Point(0, 7);
            this.groupBoxDevice.Name = "groupBoxDevice";
            this.groupBoxDevice.Size = new System.Drawing.Size(348, 261);
            this.groupBoxDevice.TabIndex = 35;
            this.groupBoxDevice.TabStop = false;
            this.groupBoxDevice.Text = "Device";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxPerDEviceJabberEnable);
            this.groupBox2.Location = new System.Drawing.Point(9, 45);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(331, 73);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Device Notifcation Methods";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelMoreInfo);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.textBoxMinAlert);
            this.groupBox1.Controls.Add(this.comboBoxJabberNotifLevel);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.textBoxMaxAlert);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Location = new System.Drawing.Point(9, 124);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(331, 104);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Notification Properties";
            // 
            // labelMoreInfo
            // 
            this.labelMoreInfo.AutoSize = true;
            this.labelMoreInfo.Location = new System.Drawing.Point(143, 79);
            this.labelMoreInfo.Name = "labelMoreInfo";
            this.labelMoreInfo.Size = new System.Drawing.Size(59, 13);
            this.labelMoreInfo.TabIndex = 33;
            this.labelMoreInfo.Text = "more info...";
            this.toolTipNotificationLevel.SetToolTip(this.labelMoreInfo, resources.GetString("labelMoreInfo.ToolTip"));
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // formDeviceProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 273);
            this.Controls.Add(this.groupBoxDevice);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(366, 311);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(366, 311);
            this.Name = "formDeviceProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Device Properties";
            this.groupBoxDevice.ResumeLayout(false);
            this.groupBoxDevice.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxPerDEviceJabberEnable;
        private System.Windows.Forms.TextBox txtb_deviceName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxMaxAlert;
        private System.Windows.Forms.TextBox textBoxMinAlert;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox comboBoxJabberNotifLevel;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.GroupBox groupBoxDevice;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelMoreInfo;
        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
    }
}