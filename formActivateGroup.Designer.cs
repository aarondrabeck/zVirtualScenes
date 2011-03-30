namespace zVirtualScenesApplication
{
    partial class formActivateGroup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formActivateGroup));
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.btn_On = new System.Windows.Forms.Button();
            this.comboBoxGroups = new System.Windows.Forms.ComboBox();
            this.btn_OFF = new System.Windows.Forms.Button();
            this.button_Set = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxLevel = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // btn_On
            // 
            this.btn_On.Location = new System.Drawing.Point(68, 45);
            this.btn_On.Name = "btn_On";
            this.btn_On.Size = new System.Drawing.Size(48, 20);
            this.btn_On.TabIndex = 2;
            this.btn_On.Text = "ON";
            this.btn_On.UseVisualStyleBackColor = true;
            this.btn_On.Click += new System.EventHandler(this.btn_On_Click);
            // 
            // comboBoxGroups
            // 
            this.comboBoxGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGroups.FormattingEnabled = true;
            this.comboBoxGroups.Location = new System.Drawing.Point(68, 19);
            this.comboBoxGroups.Name = "comboBoxGroups";
            this.comboBoxGroups.Size = new System.Drawing.Size(266, 21);
            this.comboBoxGroups.TabIndex = 1;
            // 
            // btn_OFF
            // 
            this.btn_OFF.Location = new System.Drawing.Point(122, 45);
            this.btn_OFF.Name = "btn_OFF";
            this.btn_OFF.Size = new System.Drawing.Size(45, 20);
            this.btn_OFF.TabIndex = 3;
            this.btn_OFF.Text = "OFF";
            this.btn_OFF.UseVisualStyleBackColor = true;
            this.btn_OFF.Click += new System.EventHandler(this.btn_OFF_Click);
            // 
            // button_Set
            // 
            this.button_Set.Location = new System.Drawing.Point(173, 45);
            this.button_Set.Name = "button_Set";
            this.button_Set.Size = new System.Drawing.Size(43, 20);
            this.button_Set.TabIndex = 4;
            this.button_Set.Text = "SET";
            this.button_Set.UseVisualStyleBackColor = true;
            this.button_Set.Click += new System.EventHandler(this.button_Set_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(222, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "Custom Level";
            // 
            // textBoxLevel
            // 
            this.textBoxLevel.Location = new System.Drawing.Point(299, 46);
            this.textBoxLevel.Name = "textBoxLevel";
            this.textBoxLevel.Size = new System.Drawing.Size(35, 20);
            this.textBoxLevel.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(272, 94);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 20);
            this.button1.TabIndex = 6;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.comboBoxGroups);
            this.groupBox1.Controls.Add(this.btn_On);
            this.groupBox1.Controls.Add(this.textBoxLevel);
            this.groupBox1.Controls.Add(this.btn_OFF);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button_Set);
            this.groupBox1.Location = new System.Drawing.Point(6, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(349, 88);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::zVirtualScenesApplication.Properties.Resources.Broadcast48;
            this.pictureBox1.Location = new System.Drawing.Point(6, 14);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(56, 51);
            this.pictureBox1.TabIndex = 40;
            this.pictureBox1.TabStop = false;
            // 
            // formActivateGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 121);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formActivateGroup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Activate Groups / Zones";
            this.Load += new System.EventHandler(this.formActivateGroup_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.Button btn_On;
        private System.Windows.Forms.ComboBox comboBoxGroups;
        private System.Windows.Forms.Button btn_OFF;
        private System.Windows.Forms.Button button_Set;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxLevel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}