namespace zVirtualScenesApplication
{
    partial class formPropertiesScene
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formPropertiesScene));
            this.txtb_sceneName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBoxDevice = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxHotKeys = new System.Windows.Forms.ComboBox();
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxDevice.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtb_sceneName
            // 
            this.txtb_sceneName.Location = new System.Drawing.Point(129, 17);
            this.txtb_sceneName.Name = "txtb_sceneName";
            this.txtb_sceneName.Size = new System.Drawing.Size(205, 20);
            this.txtb_sceneName.TabIndex = 24;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(53, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(75, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Scene Name: ";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(247, 225);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(93, 25);
            this.btn_Save.TabIndex = 34;
            this.btn_Save.Text = "Save Changes";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // groupBoxDevice
            // 
            this.groupBoxDevice.Controls.Add(this.pictureBox2);
            this.groupBoxDevice.Controls.Add(this.groupBox1);
            this.groupBoxDevice.Controls.Add(this.txtb_sceneName);
            this.groupBoxDevice.Controls.Add(this.btn_Save);
            this.groupBoxDevice.Controls.Add(this.label12);
            this.groupBoxDevice.Location = new System.Drawing.Point(4, 5);
            this.groupBoxDevice.Name = "groupBoxDevice";
            this.groupBoxDevice.Size = new System.Drawing.Size(346, 256);
            this.groupBoxDevice.TabIndex = 35;
            this.groupBoxDevice.TabStop = false;
            this.groupBoxDevice.Text = "Scene";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::zVirtualScenesApplication.Properties.Resources.scene_32;
            this.pictureBox2.Location = new System.Drawing.Point(12, 15);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(35, 34);
            this.pictureBox2.TabIndex = 39;
            this.pictureBox2.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxHotKeys);
            this.groupBox1.Location = new System.Drawing.Point(6, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(330, 48);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global Hotkey Assignment";
            // 
            // comboBoxHotKeys
            // 
            this.comboBoxHotKeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHotKeys.FormattingEnabled = true;
            this.comboBoxHotKeys.Location = new System.Drawing.Point(6, 19);
            this.comboBoxHotKeys.Name = "comboBoxHotKeys";
            this.comboBoxHotKeys.Size = new System.Drawing.Size(318, 21);
            this.comboBoxHotKeys.TabIndex = 39;
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // formSceneProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 264);
            this.Controls.Add(this.groupBoxDevice);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formSceneProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Scene Properties";
            this.Load += new System.EventHandler(this.formSceneProperties_Load);
            this.groupBoxDevice.ResumeLayout(false);
            this.groupBoxDevice.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtb_sceneName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.GroupBox groupBoxDevice;
        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxHotKeys;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}